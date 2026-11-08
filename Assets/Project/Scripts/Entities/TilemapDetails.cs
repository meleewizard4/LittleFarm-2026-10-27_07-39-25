using UnityEngine;
using Newtonsoft;
using Newtonsoft.Json;


public enum TilemapState
{
    Ground,
    FarmingDirt,
    Planted,
    ReadyToHarvest
}

public enum CropType
{
    None,
    Carrot,
    Tomato,
    Corn
}

public class TilemapDetails 
{

    

    public int x { get; set; }
    public int y { get; set; }
    public TilemapState tilemapState { get; set; }
    public CropType cropType { get; set; }
    public long plantedAtTicks { get; set; }

    public TilemapDetails()
    {
        cropType = CropType.None;
    }

    public TilemapDetails(
        int x,
        int y,
        TilemapState tilemapState)
    {
        this.x = x;
        this.y = y;
        this.tilemapState = tilemapState;
        cropType = CropType.None;
    }

    public TilemapDetails(
        int x,
        int y,
        CropType cropType,
        long plantedAtTicks)
    {
        this.x = x;
        this.y = y;
        this.tilemapState = TilemapState.Planted;
        this.cropType = cropType;
        this.plantedAtTicks = plantedAtTicks;
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
