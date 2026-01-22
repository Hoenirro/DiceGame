using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class EncounterScreen : MonoBehaviour
{
    string playerName;
    int encounterIndex;
    int playerDice = 0;
    [SerializeField] private TMP_Text messageText, WaiterBox;
    [SerializeField] private MessageScreenSO EncounterScreenSO;
    [SerializeField] private NPCDiceResults npcDiceResults, playerDiceResults;
    [SerializeField] private PlayerOptions playerOptions;
    private SimpleWaiter waiter;

    void Start()
    {
        if(WaiterBox != null)
        {
            waiter = WaiterBox.GetComponent<SimpleWaiter>();
            WaiterBox.gameObject.SetActive(false);
        }
        playerName = SaveManager.IsPlayerNameSaved()? SaveManager.LoadPlayerName() : "NotFound";
        Debug.Log("Player Name: " + playerName);
        encounterIndex = SaveManager.LoadGameProgress();
        StartCoroutine(InitializeEncounter());
    }

    IEnumerator InitializeEncounter()
    {
        if (encounterIndex == 0)
        {
            TextHelper.TypeTextOnto($"Hmmm... let's see.... {playerName}.. huh?..", messageText);
            yield return StartCoroutine(DefaultWait());
        }

        if (EncounterScreenSO != null && messageText != null)
        {
            yield return StartCoroutine(DisplayMessages(encounterIndex));
        }

       yield return StartCoroutine(BattleLoop());
    }

    #region BattleLoop
    static int[] diceOptionsValue = {1, 3, 6};
    static string[] actions = {"Roll Up", "Roll Down", "Equal!"};
    static string[] diceOptions = {"1 Die", "3 Dice", "6 Dice"};

    IEnumerator BattleLoop()
    {
        NPCDiceSack.ClearNPCDice();
        NPCDiceSack.addNPCDice(encounterIndex + 2);
        TextHelper.TypeTextOnto("The battle begins!", messageText);
        yield return StartCoroutine(DefaultWait());
        TextHelper.TypeTextOnto("Let's roll!", messageText);
        yield return StartCoroutine(DefaultWait());
        
        if (SaveManager.LoadPlayerDice() == 0)
        {
            TextHelper.TypeTextOnto($"{playerName} has {SaveManager.LoadPlayerDice()} die!", messageText);
            yield return StartCoroutine(DefaultWait());
            TextHelper.TypeTextOnto("this thing wins the game!", messageText);
            yield return StartCoroutine(DefaultWait());
            TextHelper.TypeTextOnto("just kidding...", messageText);
            yield return StartCoroutine(DefaultWait());
            TextHelper.TypeTextOnto($"Awarding {SaveManager.addPlayerDice(3)} dice to {playerName}... just this once", messageText);
            yield return StartCoroutine(DefaultWait());
        }

        TextHelper.TypeTextOnto($"{playerName} has {SaveManager.LoadPlayerDice()} dice.", messageText);
        yield return StartCoroutine(DefaultWait());
        TextHelper.TypeTextOnto($"This thing is playing with {NPCDiceSack.LoadNPCDice()} dice.", messageText);
        yield return StartCoroutine(DefaultWait());

        do
        {
            bool isPlayerTurn = false;

            TextHelper.TypeTextOnto($"Let's decide who starts, throw one die.", messageText);
            yield return StartCoroutine(DefaultWait());
            yield return StartCoroutine(DecidingTurn(result =>
            {
                isPlayerTurn = result;
                Debug.Log($"Is Player Turn: {isPlayerTurn}");
            }));

            // Collect choices
            int Action = -1;
            int playerDiceChoice = -1;
            int npcDiceChoice = -1;

            if (isPlayerTurn)
            {
                // Player chooses action + dice
                yield return StartCoroutine(PlayerTurn(true,
                    a => Action = a,
                    d => playerDiceChoice = d));

                // NPC chooses dice only (action only on first turn)
                yield return StartCoroutine(NPCTurn(false,
                    a => Action = a,
                    d => npcDiceChoice = d));
            }
            else
            {
                // NPC chooses first
                yield return StartCoroutine(NPCTurn(true,
                    a => Action = a,
                    d => npcDiceChoice = d));

                // Player chooses dice only (action only on first turn)
                yield return StartCoroutine(PlayerTurn(false,
                    a => Action = a,
                    d => playerDiceChoice = d));
        }

        // Convert diceChoice index → actual dice count
        int pDiceCount = diceOptionsValue[playerDiceChoice];
        int nDiceCount = diceOptionsValue[npcDiceChoice];

        // Roll dice
        var pDice = RollDice(pDiceCount);
        var nDice = RollDice(nDiceCount);

        playerDiceResults.SpawnDiceResults(pDice.ToArray());
        npcDiceResults.SpawnDiceResults(nDice.ToArray());
        yield return StartCoroutine(DefaultWait());

        // Sort based on chosen action
        SortDice(pDice, Action);
        SortDice(nDice, Action);

        // Display dice results
        playerDiceResults.ClearDiceResults();
        npcDiceResults.ClearDiceResults();

        playerDiceResults.SpawnDiceResults(pDice.ToArray());
        yield return StartCoroutine(DefaultWait());

        npcDiceResults.SpawnDiceResults(nDice.ToArray());
        yield return StartCoroutine(DefaultWait());

        // Compare
        var result = CompareDice(pDice, nDice, Action);

        // Show result
        TextHelper.TypeTextOnto(
            $"{playerName} Score: {result.pScore} | NPC Score: {result.nScore}",
            messageText
        );
        yield return StartCoroutine(DefaultWait());

        // Deduct dice from loser
        if (result.pScore > result.nScore)
        {
            NPCDiceSack.addNPCDice(-1);
            TextHelper.TypeTextOnto("NPC loses 1 die!", messageText);
        }
        else if (result.nScore > result.pScore)
        {
            SaveManager.addPlayerDice(-1);
            TextHelper.TypeTextOnto($"{playerName} loses 1 die!", messageText);
        }
        else
        {
            TextHelper.TypeTextOnto("It's a tie!", messageText);
        }

        yield return StartCoroutine(DefaultWait());

        } while (SaveManager.LoadPlayerDice() > 0 && NPCDiceSack.LoadNPCDice() > 0);
        Debug.Log("Battle Over");
    }

    #endregion

    List<int> RollDice(int count)
    {
        var result = new List<int>();
        for (int i = 0; i < count; i++)
            result.Add(UnityEngine.Random.Range(1, 7));
        return result;
    }

void SortDice(List<int> dice, int action)
{
    // 0 = Roll Up, 1 = Roll Down, 2 = Equal
    if (action == 0)      // Roll Up
    {
        dice.Sort();
        dice.Reverse();
    }
    else if (action == 1) // Roll Down
    {
        dice.Sort();
    }
    // Equal = no sorting
}

    (int pScore, int nScore) CompareDice(List<int> p, List<int> n, int action)
    {
        int rounds = Mathf.Min(p.Count, n.Count);
        int pScore = 0;
        int nScore = 0;

        switch (action)
        {
            case 0: // roll up
                for (int i = 0; i < rounds; i++)
                {
                    if (p[i] > n[i]) pScore++;
                    else if (n[i] > p[i]) nScore++;
                }
                return (pScore, nScore);
            case 1: // roll down
                for (int i = 0; i < rounds; i++)
                {
                    if (p[i] < n[i]) pScore++;
                    else if (n[i] < p[i]) nScore++;
                }
                return (pScore, nScore);
            case 2: 
                return (pScore, nScore);
            default:
                return (pScore, nScore);
        }


    }


    IEnumerator DecidingTurn(Action<bool> isPlayerTurn)
    {
        int npcRoll = UnityEngine.Random.Range(1,7);
        int playerRoll = UnityEngine.Random.Range(1,7);
        npcDiceResults.ClearDiceResults();
        playerDiceResults.ClearDiceResults();
        npcDiceResults.SpawnDiceResults(new int[] { npcRoll });
        yield return StartCoroutine(DefaultWait());
        playerDiceResults.SpawnDiceResults(new int[] { playerRoll });
        yield return StartCoroutine(DefaultWait());
        while (npcRoll == playerRoll)
        {
            TextHelper.TypeTextOnto("It's a tie! Rerolling...", messageText);
            yield return StartCoroutine(DefaultWait());
            npcRoll = UnityEngine.Random.Range(1,7);
            playerRoll = UnityEngine.Random.Range(1,7);
            npcDiceResults.ClearDiceResults();
            playerDiceResults.ClearDiceResults();
            npcDiceResults.SpawnDiceResults(new int[] { npcRoll });
            yield return StartCoroutine(DefaultWait());
            playerDiceResults.SpawnDiceResults(new int[] { playerRoll });
            yield return StartCoroutine(DefaultWait());
        }
        string resultMessage = playerRoll > npcRoll ? "Your turn!" :
                               playerRoll < npcRoll ? $"NPC's turn!" :
                               "It's a tie!";
        TextHelper.TypeTextOnto(resultMessage, messageText);
        yield return StartCoroutine(DefaultWait());
        isPlayerTurn(playerRoll > npcRoll);
    }

    IEnumerator PlayerTurn(bool firstTurn, Action<int> actionChosen, Action<int> diceChosen)
    {
        
        if (firstTurn)
        {
            TextHelper.TypeTextOnto("Your Turn! Choose your action:", messageText);
            yield return StartCoroutine(WaitForActionChoice(actions, result =>
            {
                actionChosen(result);
            }, " how many dice are you rolling?"));

        }
        TextHelper.TypeTextOnto("how many dice are you rolling?", messageText);
        yield return StartCoroutine(WaitForActionChoice(diceOptions, result =>
        {
            diceChosen(result);
        }));
    }

    IEnumerator NPCTurn(bool firstTurn, Action<int> actionChosen, Action<int> diceChosen)
    {
        if (firstTurn)
        {
            TextHelper.TypeTextOnto("NPC's Turn! Choosing action...", messageText);
            yield return StartCoroutine(DefaultWait());
            int npcAction = UnityEngine.Random.Range(0, actions.Length);
            TextHelper.TypeTextOnto($"NPC chose to {actions[npcAction]}!", messageText);
            yield return StartCoroutine(DefaultWait());
            actionChosen(npcAction);
        }
        TextHelper.TypeTextOnto("NPC is choosing how many dice to roll...", messageText);
        yield return StartCoroutine(DefaultWait());
        int npcDice = UnityEngine.Random.Range(0, diceOptions.Length);
        TextHelper.TypeTextOnto($"NPC chose to roll {diceOptions[npcDice]}!", messageText);
        yield return StartCoroutine(DefaultWait());
        diceChosen(npcDice);
    }

    IEnumerator WaitForActionChoice(string[] choices, Action<int> chose, string prompt = null)
    {
        int chosen = -1;
        var buttons = playerOptions.GenerateResponseButtons(choices);
        buttons[0].onClick.AddListener(() => {
            TextHelper.TypeTextOnto($"{choices[0]}!{prompt}", messageText);
            chosen = 0;
            playerOptions.ClearButtons();
        });
        buttons[1].onClick.AddListener(() => {
            TextHelper.TypeTextOnto($"{choices[1]}!{prompt}", messageText);
            chosen = 1;
            playerOptions.ClearButtons();
        });

        buttons[2].onClick.AddListener(() => {
            TextHelper.TypeTextOnto($"{choices[2]}!{prompt}", messageText);
            chosen = 2;
            playerOptions.ClearButtons();
        });

        yield return new WaitUntil(() => chosen != -1);

        chose(chosen);
    }


    IEnumerator DisplayMessages(int encounter)
    {
        var messages = EncounterScreenSO.messageEncounters[encounter].messages;
        foreach (var message in messages)
        {
            TextHelper.TypeTextOnto(message, messageText);
            yield return StartCoroutine(DefaultWait());
        }
    }

    IEnumerator DefaultWait()
    {
        yield return new WaitWhile(() => TextHelper.isTyping);
        WaiterBox.gameObject.SetActive(true);
        yield return new WaitUntil(() => !waiter.isWaiting);
        WaiterBox.gameObject.SetActive(false);
    }

    

}


