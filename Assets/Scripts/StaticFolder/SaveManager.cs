using UnityEngine;

public static class SaveManager
{
    #region PlayerName
    public static void SavePlayerName(string name)
    {
        PlayerPrefs.SetString("PlayerName", name);
        PlayerPrefs.Save();
    }

    public static string LoadPlayerName()
    {
        return PlayerPrefs.GetString("PlayerName", "Ijiraq");
    }

    public static void ClearPlayerName()
    {
        PlayerPrefs.DeleteKey("PlayerName");
    }

    public static bool IsPlayerNameSaved()
    {
        return PlayerPrefs.HasKey("PlayerName");
    }
    #endregion

    #region GameProgress
    public static void SaveGameProgress(int encounterIndex)
    {
        PlayerPrefs.SetInt("EncounterIndex", encounterIndex);
        PlayerPrefs.Save();
    }

    public static int LoadGameProgress()
    {
        return PlayerPrefs.GetInt("EncounterIndex", 0);
    }

    public static void ClearGameProgress()
    {
        PlayerPrefs.DeleteKey("EncounterIndex");
    }
    public static bool IsGameProgressSaved()
    {
        return PlayerPrefs.HasKey("EncounterIndex");
    }
    #endregion

    #region PlayerStats

    public static int LoadPlayerDice()
    {
        return PlayerPrefs.GetInt("PlayerDice", 0);
    }

    public static event System.Action playerDiceChanged;

    public static int addPlayerDice(int amount)
    {
        int currentDice = LoadPlayerDice();
        currentDice += amount;
        PlayerPrefs.SetInt("PlayerDice", currentDice);
        PlayerPrefs.Save();
        if (currentDice < 0) currentDice = 0;
        playerDiceChanged?.Invoke();
        return PlayerPrefs.GetInt("PlayerDice");
    } 

    #endregion

}
