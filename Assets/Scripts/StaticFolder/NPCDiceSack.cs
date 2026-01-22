using UnityEngine;

public static class NPCDiceSack
{
    public static event System.Action npcDiceChanged;
    public static int LoadNPCDice()
    {
        return PlayerPrefs.GetInt("NPCDice", 0);
    }

    public static int addNPCDice(int amount)
    {
        int currentDice = LoadNPCDice();
        currentDice += amount;
        PlayerPrefs.SetInt("NPCDice", currentDice);
        PlayerPrefs.Save();
        npcDiceChanged?.Invoke();
        return currentDice;
    }

    public static void ClearNPCDice()
    {
        PlayerPrefs.DeleteKey("NPCDice");
        npcDiceChanged?.Invoke();
    }
}