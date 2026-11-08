using UnityEngine;

public static class CurrencyManager
{
    public static int Gold
    {
        get
        {
            if (LoadDataManager.UserData == null)
            {
                return 0;
            }

            return LoadDataManager.UserData.Gold;
        }
    }

    public static void AddGold(int amount)
    {
        if (LoadDataManager.UserData == null)
        {
            return;
        }

        LoadDataManager.UserData.Gold += amount;
        SaveCurrency();
    }

    public static bool TrySpendGold(int amount)
    {
        if (LoadDataManager.UserData == null)
        {
            return false;
        }

        if (LoadDataManager.UserData.Gold < amount)
        {
            return false;
        }

        LoadDataManager.UserData.Gold -= amount;
        SaveCurrency();

        return true;
    }

    private static void SaveCurrency()
    {
        if (LoadDataManager.firebaseUser == null)
        {
            return;
        }

        FirebaseDatabaseManager databaseManager =
            Object.FindObjectOfType<FirebaseDatabaseManager>();

        if (databaseManager == null)
        {
            return;
        }

        databaseManager.WriteDatabase(
            "Users/" + LoadDataManager.firebaseUser.UserId,
            LoadDataManager.UserData.ToString());
    }
}