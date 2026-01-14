using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;



public class MessageScreen : MonoBehaviour
{
    [SerializeField] private MessageScreenSO messageScreenSO;
    [SerializeField] private TMP_Text messageText, WaiterBox;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private PlayerOptions playerOptions;

    

    private SimpleWaiter waiter;
    private string playerName;
    private int encounterIndex;

    private InputSystem_Actions controls;

    void Awake()
    {
        controls = new InputSystem_Actions();
    }
    void OnEnable()
    {
        controls.Enable();
        controls.UI.Submit.performed += ctx => TextHelper.endTyping = TextHelper.isTyping;
        if(SaveManager.IsPlayerNameSaved())
        {
            LoadGameData();
        }
    }

    void OnDisable()
    {
        controls.UI.Submit.performed -= ctx => TextHelper.endTyping = TextHelper.isTyping;
        controls.Disable();
    }

    private void Start()
    {
        if(WaiterBox != null)
        {
            waiter = WaiterBox.GetComponent<SimpleWaiter>();
            WaiterBox.gameObject.SetActive(false);
        }
        if (messageScreenSO != null && messageText != null)
        {
            StartCoroutine(DisplayMessages(encounterIndex));
        }
    }

    IEnumerator InitialSalute()
    {
        TextHelper.TypeTextOnto("Hello...", messageText);
        yield return StartCoroutine(DefaultWait());
        TextHelper.TypeTextOnto("hmm...", messageText, 0.3f);
        yield return StartCoroutine(DefaultWait());
        TextHelper.TypeTextOnto("You are not...", messageText);
        yield return StartCoroutine(DefaultWait());
        TextHelper.TypeTextOnto("How may I call you?", messageText);
        yield return StartCoroutine(DefaultWait());
        inputField.gameObject.SetActive(true); inputField.transform.SetParent(playerOptions.transform);
        inputField.GetComponent<RectTransform>().sizeDelta = new Vector2(550, 128);
        inputField.pointSize = 100;
        inputField.ActivateInputField(); inputField.characterLimit = 15; inputField.Select();
        inputField.onEndEdit.AddListener(delegate { 
            playerName = inputField.text;
            SaveManager.SavePlayerName(playerName);
            inputField.gameObject.SetActive(false);
        });
        yield return StartCoroutine(DefaultWait());
    }

    IEnumerator DisplayMessages(int encounter)
    {
        if (SaveManager.IsPlayerNameSaved())
        {
            playerName = SaveManager.LoadPlayerName();
        }
        else
        {
            yield return StartCoroutine(InitialSalute());
        }
        var messages = messageScreenSO.messageEncounters[encounter].messages;
        foreach (var message in messages)
        {
            TextHelper.TypeTextOnto(message, messageText);
            yield return StartCoroutine(DefaultWait());
        }
        List<Button> buttons = playerOptions.GenerateResponseButtons(new string[] { "Play", "No" });
        buttons[0].onClick.AddListener(delegate {
            //SaveManager.SaveGameProgress(encounterIndex + 1);
            UnityEngine.SceneManagement.SceneManager.LoadScene("EncounterScreen");
        });
        buttons[1].onClick.AddListener(delegate {
            TextHelper.TypeTextOnto("Maybe next time then...", messageText);
        });
    }

    IEnumerator DefaultWait()
    {
        yield return new WaitWhile(() => TextHelper.isTyping);
        WaiterBox.gameObject.SetActive(true);
        yield return new WaitUntil(() => !waiter.isWaiting);
        WaiterBox.gameObject.SetActive(false);
    }

    private void LoadGameData()
    {
        playerName = SaveManager.LoadPlayerName();
        encounterIndex = SaveManager.LoadGameProgress();
    }
}
