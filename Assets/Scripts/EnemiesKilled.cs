using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemiesKilled : MonoBehaviour
{
    public Text scoreText;

    /// <summary>
    /// called when the object becomes enabled and active
    /// </summary>
    private void OnEnable()
    {
        StartCoroutine(AnimateText());
    }

    /// <summary>
    /// displaying an animation counting the enemies killed
    /// </summary>
    /// <returns></returns>
    IEnumerator AnimateText()
    {
        scoreText.text = "0";
        int count = 0;

        // show an animation of the numbers counting from 0 to the enemies count
        while (count <  GameManager.gameManager.playerStats.enemiesKilled)
        {
            count++;
            scoreText.text = count.ToString();

            yield return new WaitForSeconds(Mathf.Min(0.05f, 0.2f / GameManager.gameManager.playerStats.enemiesKilled));
        }
    }
}
