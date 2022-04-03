using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    /// <summary>
    /// reload the curret level
    /// </summary>
    public void Retry()
    {
        GameManager.gameManager.sceneFader.FadeTo(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// load the main menu
    /// </summary>
    public void Menu()
    {
        GameManager.gameManager.sceneFader.FadeTo(GameManager.gameManager.menuSceneName);
    }
}
