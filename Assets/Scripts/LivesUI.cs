using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LivesUI : MonoBehaviour
{
    // ui element for the lives' text
    private Text livesText;

    /// <summary>
    /// called when the script instance is being loaded
    /// </summary>
    private void Awake()
    {
        livesText = GetComponent<Text>();
    }

    /// <summary>
    /// called on the frame when a script is enabled just before any of the Update methods are called the first time
    /// </summary>
    void Start()
    {
        UpdateText();
    }

    /// <summary>
    /// updates the ui text object according to the games' stats
    /// </summary>
    public void UpdateText()
    {
        livesText.text = GameManager.gameManager.playerStats.currentLives.ToString();
    }
}
