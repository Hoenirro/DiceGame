using UnityEngine;

public class EncounterScreen : MonoBehaviour
{
    string playerName;
    int encounterIndex;
    int playerDice = 0;

    void Start()
    {
        playerName = SaveManager.IsPlayerNameSaved()? SaveManager.LoadPlayerName() : "NotFound";
        Debug.Log("Player Name: " + playerName);
    }

    

}
