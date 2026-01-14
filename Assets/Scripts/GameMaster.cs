using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GameMaster : MonoBehaviour
{
    public static string GameMasterName = "Loki";
    public static string PlayerName = "Player";

    [SerializeField] private Canvas PlayerCanvas;
    [SerializeField] private TMP_Text SysMessagesBox, WaiterBox;
    [SerializeField] private SysMessages sysMessages;


    private SimpleWaiter waiter;
    private InputSystem_Actions controls;

    private void OnEnable()
    {
        controls.UI.Submit.performed += OnSubmit;
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.UI.Submit.performed -= OnSubmit;
        controls.Disable();
    }

    private void Awake() => controls = new InputSystem_Actions();
    private void Start()
    {
        if(WaiterBox != null)
        {
        waiter = WaiterBox.GetComponent<SimpleWaiter>();
        WaiterBox.gameObject.SetActive(false);
        }
        StartCoroutine(Salutations());
    }

    private bool waitingForExternalAction = false;

    /// <summary>
    /// Call this to signal the external action has started (optional).
    /// </summary>
    public void StartExternalAction() => waitingForExternalAction = true;

    /// <summary>
    /// Call this from your external UI or handler to resume message flow.
    /// </summary>
    public void EndExternalAction() => waitingForExternalAction = false;

    /// <summary>
    /// Example helper that a UnityEvent can call to request the player's name.
    /// When your input UI finishes, call EndExternalAction().
    /// </summary>
    public void RequestPlayerName()
    {
        StartExternalAction();
        // creates an active input field or UI for the player to enter their name.
        TMP_InputField nameInput = PlayerCanvas.GetComponentInChildren<TMP_InputField>(true);
        if (nameInput != null)
        {
            nameInput.gameObject.SetActive(true);
            nameInput.onEndEdit.RemoveAllListeners();
            nameInput.onEndEdit.AddListener((string input) =>
            {
                if (!string.IsNullOrWhiteSpace(input))
                {
                    PlayerName = input.Trim();
                }
                nameInput.gameObject.SetActive(false);
                EndExternalAction();
            });
            nameInput.ActivateInputField();
        }
        else
        {
            TextHelper.TypeTextOnto("... a quiet one...  I'm calling you 'Player'...:d", SysMessagesBox, 0.1f);
        }


        Debug.Log("RequestPlayerName called - open your input UI and call EndExternalAction() when done.");
        // TODO: implement opening your input UI here and hook its completion to EndExternalAction().
    }

    private IEnumerator Salutations()
    {
        foreach (var entry in sysMessages.salute)
        {
            // use entry.text; entry.type is available if you want different styling/behavior per type
            TextHelper.TypeTextOnto(entry.text, SysMessagesBox, 0.05f);
            yield return new WaitWhile(() => TextHelper.isTyping);

            // If the message defines an onShow event, invoke it. If it requests to wait for completion,
            // set waiting flag and yield until EndExternalAction() is called.
            if (entry.onShow != null)
            {
                if (entry.waitForEventCompletion)
                {
                    waitingForExternalAction = true; // start waiting before invoking so handlers don't need to set it themselves
                }
                entry.onShow.Invoke();
                if (entry.waitForEventCompletion)
                {
                    yield return new WaitUntil(() => !waitingForExternalAction);
                    // continue to next message after external action completes
                    continue;
                }
            }

            // Default 'press to continue' behavior
            WaiterBox.gameObject.SetActive(true);
            SimpleWaiter waiter = WaiterBox.GetComponent<SimpleWaiter>();
            yield return new WaitUntil(() => !waiter.isWaiting);
            WaiterBox.gameObject.SetActive(false);
        }
    }

    private void OnSubmit(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (TextHelper.isTyping) TextHelper.endTyping = true;
        waiter.isWaiting = false;
    }

}
