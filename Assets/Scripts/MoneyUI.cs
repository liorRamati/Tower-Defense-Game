using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class MoneyUI : MonoBehaviour
{
    private Text moneyText;

    /// <summary>
    /// called when the script instance is being loaded
    /// </summary>
    private void Awake()
    {
        moneyText = GetComponent<Text>();
    }

    /// <summary>
    /// called on the frame when a script is enabled just before any of the Update methods are called the first time
    /// </summary>
    private void Start()
    {
        UpdateText();
    }

    /// <summary>
    /// updates the text to reflect the currentgold value
    /// </summary>
    public void UpdateText()
    {
        moneyText.text = GameManager.gameManager.playerStats.currentMoney.ToString();
    }
}
