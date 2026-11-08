using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour
{
    public Tilemap tm_Ground;
    public Tilemap tm_FarmingDirt;
    public Tilemap tm_Tree;

    public TileBase tb_tree;
    public TileBase tb_farmingDirt;
    public TileBase tb_Ground;

    public TileBase tb_CarrotGrowing;
    public TileBase tb_CarrotReady;
    public TileBase tb_TomatoGrowing;
    public TileBase tb_TomatoReady;
    public TileBase tb_CornGrowing;
    public TileBase tb_CornReady;

    [Min(0.01f)]
    public float growthTimeMultiplier = 1f;

    private Map map;
    private FirebaseDatabaseManager databaseManager;
    private FirebaseUser user;
    private bool mapLoaded;

    public bool IsMapLoaded
    {
        get { return mapLoaded; }
    }

    private void Start()
    {
        map = new Map();
        databaseManager = FindObjectOfType<FirebaseDatabaseManager>();
        user = FirebaseAuth.DefaultInstance.CurrentUser;

        if (databaseManager == null || user == null)
        {
            Debug.LogWarning("Không tìm thấy Firebase hoặc người dùng. Bản đồ chạy ở chế độ cục bộ.");
            mapLoaded = true;
            return;
        }

        LoadMapForUser();
    }

    private void Update()
    {
        UpdateGrowingCrops();
    }

    public void LoadMapForUser()
    {
        if (user == null)
        {
            mapLoaded = true;
            return;
        }

        FirebaseDatabase.DefaultInstance.RootReference
            .Child("Users")
            .Child(user.UserId)
            .Child("Map")
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    Debug.LogError("Không tải được dữ liệu bản đồ.");
                    mapLoaded = true;
                    return;
                }

                DataSnapshot snapshot = task.Result;

                if (!snapshot.Exists || snapshot.Value == null)
                {
                    map = new Map();
                }
                else
                {
                    map = JsonConvert.DeserializeObject<Map>(snapshot.GetRawJsonValue())
                        ?? new Map();

                    if (map.ListTilemapDetail == null)
                    {
                        map.ListTilemapDetail = new List<TilemapDetails>();
                    }
                }

                mapLoaded = true;
                LoadMapToUI(map);
            });
    }

    public TilemapDetails GetTilemapDetail(int x, int y)
    {
        if (map == null || map.ListTilemapDetail == null)
        {
            return null;
        }

        for (int i = 0; i < map.ListTilemapDetail.Count; i++)
        {
            TilemapDetails details = map.ListTilemapDetail[i];

            if (details != null && details.x == x && details.y == y)
            {
                return details;
            }
        }

        return null;
    }

    public bool SetCropForTile(int x, int y, CropType cropType)
    {
        if (!mapLoaded || CropDatabase.GetCropData(cropType) == null)
        {
            return false;
        }

        TilemapDetails details = GetTilemapDetail(x, y);

        if (details == null)
        {
            details = new TilemapDetails(x, y, cropType, DateTime.UtcNow.Ticks);
            map.ListTilemapDetail.Add(details);
        }
        else
        {
            details.cropType = cropType;
            details.plantedAtTicks = DateTime.UtcNow.Ticks;
            details.tilemapState = TilemapState.Planted;
        }

        TilemapDetailToTileBase(details);
        SaveMap();

        return true;
    }

    public bool SetStateForTilemapDetail(int x, int y, TilemapState state)
    {
        if (!mapLoaded)
        {
            Debug.LogWarning("Bản đồ chưa tải xong.");
            return false;
        }

        TilemapDetails details = GetTilemapDetail(x, y);

        if (details == null)
        {
            details = new TilemapDetails(x, y, state);
            map.ListTilemapDetail.Add(details);
        }
        else
        {
            details.tilemapState = state;

            if (state != TilemapState.Planted
                && state != TilemapState.ReadyToHarvest)
            {
                details.cropType = CropType.None;
                details.plantedAtTicks = 0;
            }
        }

        TilemapDetailToTileBase(details);
        SaveMap();

        return true;
    }

    public void TilemapDetailToTileBase(TilemapDetails details)
    {
        if (details == null
            || tm_Ground == null
            || tm_FarmingDirt == null
            || tm_Tree == null)
        {
            return;
        }

        Vector3Int cellPos = new Vector3Int(details.x, details.y, 0);

        tm_Ground.SetTile(cellPos, null);
        tm_FarmingDirt.SetTile(cellPos, null);
        tm_Tree.SetTile(cellPos, null);

        switch (details.tilemapState)
        {
            case TilemapState.Ground:
                tm_Ground.SetTile(cellPos, tb_Ground);
                break;

            case TilemapState.FarmingDirt:
                tm_FarmingDirt.SetTile(cellPos, tb_farmingDirt);
                break;

            case TilemapState.Planted:
            case TilemapState.ReadyToHarvest:
                tm_FarmingDirt.SetTile(cellPos, tb_farmingDirt);
                tm_Tree.SetTile(
                    cellPos,
                    GetCropTile(
                        details.cropType,
                        details.tilemapState == TilemapState.ReadyToHarvest));
                break;
        }
    }

    public void LoadMapToUI(Map mapToLoad)
    {
        if (mapToLoad == null || mapToLoad.ListTilemapDetail == null)
        {
            return;
        }

        for (int i = 0; i < mapToLoad.ListTilemapDetail.Count; i++)
        {
            TilemapDetailToTileBase(mapToLoad.ListTilemapDetail[i]);
        }
    }

    private void UpdateGrowingCrops()
    {
        if (!mapLoaded || map == null || map.ListTilemapDetail == null)
        {
            return;
        }

        bool changed = false;
        long currentTicks = DateTime.UtcNow.Ticks;

        for (int i = 0; i < map.ListTilemapDetail.Count; i++)
        {
            TilemapDetails details = map.ListTilemapDetail[i];

            if (details == null || details.tilemapState != TilemapState.Planted)
            {
                continue;
            }

            if (details.plantedAtTicks <= 0)
            {
                details.plantedAtTicks = currentTicks;
                changed = true;
                continue;
            }

            CropData cropData = CropDatabase.GetCropData(details.cropType);

            if (cropData == null)
            {
                continue;
            }

            float elapsedSeconds =
                (currentTicks - details.plantedAtTicks) / (float)TimeSpan.TicksPerSecond;

            float requiredSeconds =
                cropData.growthTime * Mathf.Max(0.01f, growthTimeMultiplier);

            if (elapsedSeconds >= requiredSeconds)
            {
                details.tilemapState = TilemapState.ReadyToHarvest;
                TilemapDetailToTileBase(details);
                changed = true;
            }
        }

        if (changed)
        {
            SaveMap();
        }
    }

    private TileBase GetCropTile(CropType cropType, bool readyToHarvest)
    {
        TileBase cropTile = null;

        switch (cropType)
        {
            case CropType.Carrot:
                cropTile = readyToHarvest ? tb_CarrotReady : tb_CarrotGrowing;
                break;

            case CropType.Tomato:
                cropTile = readyToHarvest ? tb_TomatoReady : tb_TomatoGrowing;
                break;

            case CropType.Corn:
                cropTile = readyToHarvest ? tb_CornReady : tb_CornGrowing;
                break;
        }

        return cropTile != null ? cropTile : tb_tree;
    }

    private void SaveMap()
    {
        if (databaseManager == null || user == null || map == null)
        {
            return;
        }

        databaseManager.WriteDatabase(
            user.UserId + "/Map",
            map.ToString());
    }
}