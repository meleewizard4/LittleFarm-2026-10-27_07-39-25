using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerFarmController : MonoBehaviour
{
    public Tilemap tm_Ground;
    public Tilemap tm_Tree;
    public Tilemap tm_FarmingDirt;
    public TilemapManager tilemapManager;

    public CropType selectedCrop = CropType.Carrot;

    private int farmGrowthUpgradeLevel;
    private string upgradePreferenceKey;

    private void Start()
    {
        if (tilemapManager == null)
        {
            tilemapManager = FindObjectOfType<TilemapManager>();
        }

        string userId = LoadDataManager.firebaseUser == null
            ? "Guest"
            : LoadDataManager.firebaseUser.UserId;

        upgradePreferenceKey = "LittleFarm.GrowthUpgradeLevel." + userId;
        farmGrowthUpgradeLevel = PlayerPrefs.GetInt(upgradePreferenceKey, 0);

        ApplyGrowthUpgrade();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedCrop = CropType.Carrot;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedCrop = CropType.Tomato;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedCrop = CropType.Corn;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            DigCurrentGround();
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            PlantSelectedCrop();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            HarvestCurrentCrop();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RestoreCurrentGround();
        }
    }

    private Vector3Int GetCurrentCell()
    {
        if (tm_Ground == null)
        {
            return Vector3Int.zero;
        }

        Vector3Int cellPos = tm_Ground.WorldToCell(transform.position);
        cellPos.z = 0;
        return cellPos;
    }

    private bool CanUseFarm()
    {
        if (tilemapManager == null || !tilemapManager.IsMapLoaded)
        {
            Debug.Log("Bản đồ đang tải hoặc chưa được gán.");
            return false;
        }

        if (tm_Ground == null || tm_Tree == null || tm_FarmingDirt == null)
        {
            Debug.LogError("Hãy gán đủ các Tilemap trong Inspector.");
            return false;
        }

        return true;
    }

    private void DigCurrentGround()
    {
        if (!CanUseFarm())
        {
            return;
        }

        Vector3Int cellPos = GetCurrentCell();

        if (tm_Ground.GetTile(cellPos) == null
            || tm_FarmingDirt.GetTile(cellPos) != null
            || tm_Tree.GetTile(cellPos) != null)
        {
            return;
        }

        tilemapManager.SetStateForTilemapDetail(
            cellPos.x,
            cellPos.y,
            TilemapState.FarmingDirt);
    }

    private void PlantSelectedCrop()
    {
        if (!CanUseFarm())
        {
            return;
        }

        Vector3Int cellPos = GetCurrentCell();

        if (tm_FarmingDirt.GetTile(cellPos) == null
            || tm_Tree.GetTile(cellPos) != null)
        {
            return;
        }

        CropData cropData = CropDatabase.GetCropData(selectedCrop);

        if (cropData == null)
        {
            Debug.LogError("Loại cây trồng không hợp lệ.");
            return;
        }

        if (!CurrencyManager.TrySpendGold(cropData.seedPrice))
        {
            Debug.Log("Không đủ Gold để mua hạt giống.");
            return;
        }

        if (!tilemapManager.SetCropForTile(cellPos.x, cellPos.y, selectedCrop))
        {
            CurrencyManager.AddGold(cropData.seedPrice);
            Debug.LogWarning("Không thể trồng cây; Gold đã được hoàn lại.");
            return;
        }

        Debug.Log("Đã trồng " + cropData.cropName + ".");
    }

    private void HarvestCurrentCrop()
    {
        if (!CanUseFarm())
        {
            return;
        }

        Vector3Int cellPos = GetCurrentCell();
        TilemapDetails details =
            tilemapManager.GetTilemapDetail(cellPos.x, cellPos.y);

        if (details == null
            || details.tilemapState != TilemapState.ReadyToHarvest)
        {
            Debug.Log("Cây chưa sẵn sàng để thu hoạch.");
            return;
        }

        CropData cropData = CropDatabase.GetCropData(details.cropType);

        if (cropData == null)
        {
            Debug.LogError("Không tìm thấy dữ liệu cây trồng.");
            return;
        }

        if (!tilemapManager.SetStateForTilemapDetail(
                cellPos.x,
                cellPos.y,
                TilemapState.FarmingDirt))
        {
            return;
        }

        CurrencyManager.AddGold(cropData.sellPrice);

        Debug.Log(
            "Đã thu hoạch " + cropData.cropName
            + ", nhận " + cropData.sellPrice + " Gold.");
    }

    private void RestoreCurrentGround()
    {
        if (!CanUseFarm())
        {
            return;
        }

        Vector3Int cellPos = GetCurrentCell();

        if (tm_Tree.GetTile(cellPos) != null)
        {
            return;
        }

        if (tm_FarmingDirt.GetTile(cellPos) == null)
        {
            return;
        }

        tilemapManager.SetStateForTilemapDetail(
            cellPos.x,
            cellPos.y,
            TilemapState.Ground);
    }

    private void TryUpgradeFarm()
    {
        int upgradePrice = 100 * (farmGrowthUpgradeLevel + 1);

        if (!CurrencyManager.TrySpendGold(upgradePrice))
        {
            Debug.Log("Không đủ Gold để nâng cấp.");
            return;
        }

        farmGrowthUpgradeLevel++;
        PlayerPrefs.SetInt(upgradePreferenceKey, farmGrowthUpgradeLevel);
        PlayerPrefs.Save();

        ApplyGrowthUpgrade();

        Debug.Log("Đã nâng cấp trang trại. Cây sinh trưởng nhanh hơn.");
    }

    private void ApplyGrowthUpgrade()
    {
        if (tilemapManager == null)
        {
            return;
        }

        tilemapManager.growthTimeMultiplier =
            Mathf.Pow(0.9f, farmGrowthUpgradeLevel);
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 270, 320), GUI.skin.box);

        GUILayout.Label("CỬA HÀNG / NÔNG TRẠI");
        GUILayout.Label("Gold: " + CurrencyManager.Gold);
        GUILayout.Label("Chọn hạt giống (trừ tiền khi trồng):");

        DrawCropButton(CropType.Carrot, "1 - Cà rốt");
        DrawCropButton(CropType.Tomato, "2 - Cà chua");
        DrawCropButton(CropType.Corn, "3 - Ngô");

        CropData selectedCropData = CropDatabase.GetCropData(selectedCrop);

        if (selectedCropData != null)
        {
            GUILayout.Label("Đang chọn: " + selectedCropData.cropName);
            GUILayout.Label(
                "Giá hạt: " + selectedCropData.seedPrice
                + " | Giá bán: " + selectedCropData.sellPrice);
            GUILayout.Label("Thời gian lớn: " + selectedCropData.growthTime + " giây");
        }

        if (GUILayout.Button("Trồng cây đang chọn (V)"))
        {
            PlantSelectedCrop();
        }

        if (GUILayout.Button("Thu hoạch (M)"))
        {
            HarvestCurrentCrop();
        }

        int upgradePrice = 100 * (farmGrowthUpgradeLevel + 1);

        if (GUILayout.Button("Nâng cấp tốc độ - " + upgradePrice + " Gold"))
        {
            TryUpgradeFarm();
        }

        GUILayout.Label("Nâng cấp hiện tại: " + farmGrowthUpgradeLevel);
        GUILayout.Label("C: Đào | X: Khôi phục đất");

        GUILayout.EndArea();
    }

    private void DrawCropButton(CropType cropType, string label)
    {
        if (GUILayout.Button(label))
        {
            selectedCrop = cropType;
        }
    }
}