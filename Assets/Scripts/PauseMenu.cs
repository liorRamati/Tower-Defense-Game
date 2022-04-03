using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject ui;

    /// <summary>
    /// Update phase in the native player loop
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            Toggle();
        }
    }

    /// <summary>
    /// pausing and unpausing the game
    /// </summary>
    public void Toggle()
    {
        // pausing the game and showing the pause screen
        ui.SetActive(!ui.activeSelf);
        GameManager.gameManager.GamePaused = !GameManager.gameManager.GamePaused;

        // stopping movement in the game
        if (ui.activeSelf)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    /// <summary>
    /// reload the current level
    /// </summary>
    public void Retry()
    {
        Toggle();
        GameManager.gameManager.sceneFader.FadeTo(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// load the main menu
    /// </summary>
    public void Menu()
    {
        Toggle();
        GameManager.gameManager.sceneFader.FadeTo(GameManager.gameManager.menuSceneName);
    }
}
