using TMPro;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    [SerializeField] private Canvas PlayerCanvas;
    [SerializeField] private TMP_Text SysMessages;

    private void Start()
    {
        TextHelper.TypeTextOnto("Welcome to the Game!",SysMessages);
    }

}
