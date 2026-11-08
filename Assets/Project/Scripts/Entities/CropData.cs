using System;

[Serializable]
public class CropData
{
    public CropType cropType;
    public string cropName;
    public int seedPrice;
    public int sellPrice;
    public float growthTime;
}

public static class CropDatabase
{
    public static CropData GetCropData(CropType cropType)
    {
        switch (cropType)
        {
            case CropType.Carrot:
                return new CropData
                {
                    cropType = CropType.Carrot,
                    cropName = "Carrot",
                    seedPrice = 5,
                    sellPrice = 12,
                    growthTime = 10f
                };

            case CropType.Tomato:
                return new CropData
                {
                    cropType = CropType.Tomato,
                    cropName = "Tomato",
                    seedPrice = 10,
                    sellPrice = 25,
                    growthTime = 20f
                };

            case CropType.Corn:
                return new CropData
                {
                    cropType = CropType.Corn,
                    cropName = "Corn",
                    seedPrice = 20,
                    sellPrice = 50,
                    growthTime = 30f
                };

            default:
                return null;
        }
    }
}