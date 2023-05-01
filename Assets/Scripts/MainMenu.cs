using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        print("Going to Level 1");
        SceneManager.LoadScene("LevelOne");
    }

    public void QuitGame()
    {
        print("Quit!");
        Application.Quit();
    }

    public void ReturnMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
