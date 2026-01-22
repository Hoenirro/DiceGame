using UnityEngine;

public enum SackOwner
{
    Player,
    NPC
}

public class DiceCounter : MonoBehaviour
{

    public SackOwner owner;
    private TMPro.TMP_Text miTexto;
    void Awake()
    {
        miTexto = GetComponent<TMPro.TMP_Text>();
    }
    void Start()
    {
        if (owner == SackOwner.Player)
        {
            miTexto.text = SaveManager.LoadPlayerDice().ToString();
            SaveManager.playerDiceChanged += UpdateDisplay;
        }
        else
        {
            miTexto.text = NPCDiceSack.LoadNPCDice().ToString();
            NPCDiceSack.npcDiceChanged += UpdateDisplay;
        }
    }

    private void UpdateDisplay()
    {
        if (owner == SackOwner.Player)
        {
            miTexto.text = SaveManager.LoadPlayerDice().ToString();
        }
        else
        {
            miTexto.text = NPCDiceSack.LoadNPCDice().ToString();
        }
    }

    private void OnDestroy()
    {
        if (owner == SackOwner.Player)
        {
            SaveManager.playerDiceChanged -= UpdateDisplay;
        }
        else
        {
            NPCDiceSack.npcDiceChanged -= UpdateDisplay;
        }
    }

}
