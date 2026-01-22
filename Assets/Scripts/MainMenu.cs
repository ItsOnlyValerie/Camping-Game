using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void CreateSession()
    {
        SceneManager.LoadSceneAsync("Create Session");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
