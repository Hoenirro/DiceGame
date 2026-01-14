using UnityEngine;

public static class TextHelper
{
    public static bool isTyping = false;
    public static bool endTyping = false;
    public static void TypeTextOnto(string message, TMPro.TMP_Text textComponent, float? typeSpeed = 0.01f)
    {
        if (isTyping) return;
        isTyping = true;
        if (textComponent == null)
        {
            Debug.LogError("Text component is null.");
            return;
        }
        textComponent.text = "";
        textComponent.StartCoroutine(TypeTextCoroutine(message, textComponent, typeSpeed.Value));

    }
    private static System.Collections.IEnumerator TypeTextCoroutine(string message, TMPro.TMP_Text textComponent, float typeSpeed)
    {
        textComponent.text = "";
        foreach (char c in message)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(typeSpeed);
            if(endTyping)
            {
                textComponent.text = message;
                endTyping = false;
                break;
            }
        }
        isTyping = false;
    }
}
