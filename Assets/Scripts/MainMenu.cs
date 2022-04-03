using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string levelToLoad = "Level01";

    public SceneFader sceneFader;

    /// <summary>
    /// load the level selection screen
    /// </summary>
    public void Play()
    {
        sceneFader.FadeTo(levelToLoad);
    }

    /// <summary>
    /// exit the game
    /// </summary>
    public void Quit()
    {
        print("Exitting...");
        Application.Quit();
    }
}
