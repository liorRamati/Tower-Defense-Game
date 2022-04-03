using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [HideInInspector]
    public int currentMoney;
    [Header("Money")]
    public int startingMoney = 400;
    public MoneyUI moneyText;

    [HideInInspector]
    public int currentLives;
    [Header("Lives")]
    public int startingLives = 5;
    public LivesUI livesText;

    [HideInInspector]
    public int enemiesKilled;

    /// <summary>
    /// called when the script instance is being loaded
    /// </summary>
    private void Awake()
    {
        // setting initial values
        currentMoney = startingMoney;
        currentLives = startingLives;
        enemiesKilled = 0;
    }

    /// <summary>
    /// called on the frame when a script is enabled just before any of the Update methods are called the first time
    /// </summary>
    private void Start()
    {
        // updates the text objects of the player's stats
        moneyText.UpdateText();
        livesText.UpdateText();
    }

    /// <summary>
    /// reduce the player's gold by the given amount
    /// </summary>
    public void ReduceMoney(int amount)
    {
        // reduce the amount for the player's gold, but keep the amount 0 or above
        if (amount <= currentMoney)
        {
            currentMoney -= amount;
        }
        else
        {
            currentMoney = 0;
        }
        // updates the text object for the player's gold
        moneyText.UpdateText();
    }

    /// <summary>
    /// increase the player's gold by the given amount
    /// </summary>
    public void IncreaseMoney(int amount)
    {
        currentMoney += amount;
        // updates the text object for the player's gold
        moneyText.UpdateText();
    }

    /// <summary>
    /// reduces the player's lives count by 1
    /// </summary>
    public void ReduceLives()
    {
        currentLives--;
        // make the lives count 0 if it is negative
        if (currentLives < 0)
        {
            currentLives = 0;
        }

        // if the lives count is zero, end the game
        if (currentLives <= 0 && !GameManager.gameManager.GameEnded)
        {
            GameManager.gameManager.EndGame();
        }

        // updates the text object for the player's lives count
        livesText.UpdateText();
    }

    /// <summary>
    /// increases the player's lives count by 1
    /// </summary>
    public void IncreaseLives()
    {
        currentLives++;
        // updates the text object for the player's lives count
        livesText.UpdateText();
    }
}
