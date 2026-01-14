using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class PlayerOptions : MonoBehaviour
{
    [SerializeField] private GameObject responseButton;

    public List<Button> GenerateResponseButtons(string[] responses)
    {
        Debug.Log("Generating Response Buttons");
        List<Button> Buttons = new List<Button>();
        foreach (var response in responses)
        {
            Buttons.Add(Instantiate(responseButton, transform).GetComponent<Button>());
            Buttons.Last().GetComponentInChildren<TMP_Text>().text = response;
            Buttons.Last().gameObject.SetActive(true);
        }

        // Configure explicit navigation
        for (int i = 0; i < Buttons.Count; i++)
        {
            Navigation nav = new Navigation
            {
                mode = Navigation.Mode.Explicit,
                selectOnRight = Buttons[(i - 1 + Buttons.Count) % Buttons.Count],
                selectOnLeft = Buttons[(i + 1) % Buttons.Count]
            };
            Buttons[i].navigation = nav;
        }
        if (Buttons.Count > 0) Buttons[0].Select();
        return Buttons;
    }
    void Start()
    {
        //EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
    }
}
