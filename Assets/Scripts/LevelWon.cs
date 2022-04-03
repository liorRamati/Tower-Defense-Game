using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelWon : MonoBehaviour
{
    public string nextLevel = "Level02";
    public int levelToReach = 2;

    /// <summary>
    /// load the next level
    /// </summary>
    public void Continue()
    {
        print("Level Won!");
        PlayerPrefs.SetInt("levelReached", levelToReach);
        GameManager.gameManager.sceneFader.FadeTo(nextLevel);
    }

    /// <summary>
    /// load the main menu
    /// </summary>
    public void Menu()
    {
        GameManager.gameManager.sceneFader.FadeTo(GameManager.gameManager.menuSceneName);
    }
}
