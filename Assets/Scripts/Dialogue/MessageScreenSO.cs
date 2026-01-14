using UnityEngine;


[CreateAssetMenu(fileName = "MessageScreenSO", menuName = "Scriptable Objects/MessageScreenSO")]
public class MessageScreenSO : ScriptableObject
{
    [System.Serializable]
    public class MessageEncounter
    {
        public string[] messages;
        public int encounterIndex;

        public string GetMessage(int index)
        {
            if (index >= 0 && index < messages.Length)
            {
                return messages[index];
            }
            return null;
        }
    }
    public MessageEncounter[] messageEncounters = new MessageEncounter[]
    {
        new MessageEncounter
        {
            encounterIndex = 0,
            messages = new string[]
            {
                "Welcome to the adventure!",
                "Your journey begins now.",
                "Prepare yourself for challenges ahead."
            }
        },
        new MessageEncounter
        {
            encounterIndex = 1,
            messages = new string[]
            {
                "You have entered the dark forest.",
                "Strange sounds surround you.",
                "Stay alert and proceed with caution."
            }
        }
    };
}
