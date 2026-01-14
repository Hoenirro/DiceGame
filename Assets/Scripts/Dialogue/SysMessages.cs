using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "SysMessages", menuName = "Scriptable Objects/SysMessages")]
public class SysMessages : ScriptableObject
{
    [System.Serializable]
    public class MessageEntry
    {
        [TextArea]
        public string text;
        public MessageType type = MessageType.Message;
        [Tooltip("Optional: assign an event to run when this message is shown")] 
        public UnityEvent onShow;
        [Tooltip("If true, the message flow will wait until GameMaster.EndExternalAction() is called")] 
        public bool waitForEventCompletion = false;
    }

    public MessageEntry[] salute = new MessageEntry[]
    {
        new MessageEntry { text = "Welcome to the Game!"},
        new MessageEntry { text = $"My name is {GameMaster.GameMasterName}." },
        new MessageEntry { text = "How may I call you?", type = MessageType.Question }
    };
}

public enum MessageType
    {
        Message,
        Question,
        Farewell,
        Warning
    }
