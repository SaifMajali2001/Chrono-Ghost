using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Hub Room");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
