using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    public Image img;

    // curve determining the animation of the fade effect
    public AnimationCurve fadeCurve;

    /// <summary>
    /// called on the frame when a script is enabled just before any of the Update methods are called the first time
    /// </summary>
    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    /// <summary>
    /// loading a new scene with a fade effect
    /// </summary>
    /// <param name="scene">name of the scene</param>
    public void FadeTo(string scene)
    {
        StartCoroutine(FadeOut(scene));
    }

    /// <summary>
    /// display a fade in effect, lowering the background color opaque according to the animation curve
    /// </summary>
    IEnumerator FadeIn()
    {
        float t = 1f;

        // lowering the opaque as time passes according to curve
        while (t > 0f)
        {
            t -= Time.deltaTime;
            Color tempClr = img.color;
            tempClr.a = fadeCurve.Evaluate(t);
            img.color = tempClr;
            yield return 0;
        }
    }

    /// <summary>
    /// display a fade in effect, rising the background color opaque according to the animation curve, then load a new scene
    /// </summary>
    /// <param name="scene">name of the new scene</param>
    IEnumerator FadeOut(string scene)
    {
        float t = 0f;

        // rising the opaque as time passes according to the curve
        while (t < 1f)
        {
            t += Time.deltaTime;
            Color tempClr = img.color;
            tempClr.a = fadeCurve.Evaluate(t);
            img.color = tempClr;
            yield return 0;
        }

        SceneManager.LoadScene(scene);
    }
}
