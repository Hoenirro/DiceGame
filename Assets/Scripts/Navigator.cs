using UnityEngine;

public class Navigator : MonoBehaviour
{
    public void NavigateTo(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
    public void QuitApplication()
    {
        Application.Quit();
    }
}
