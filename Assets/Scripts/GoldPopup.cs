using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class GoldPopup : MonoBehaviour
{
    private TextMeshPro _textMesh;

    [Tooltip("The speed the popup goes upwards")]
    public float moveSpeed = 20f;
    [Tooltip("The time before the popup start to disappear")]
    public float DisappearTimer = 1f;
    [Tooltip("The speed of the popup disappearence")]
    public float DisappearSpeed = 3f;
    [Tooltip("The amount of change in scale used")]
    public float ScaleAmount = 1f;

    private float _disappearTimer;
    private Color _textColor;
    // used to order the rendering order of the ui elements
    private static int sortingOrder;

    /// <summary>
    /// Create the prefab and sets it up
    /// </summary>
    /// <param name="position">the position to create in</param>
    /// <param name="goldAmount">the amount of gold</param>
    /// <returns>the gold popup created</returns>
    public static GoldPopup Create(Vector3 position, int goldAmount)
    {
        // do nothing if there is no prefab for gold popup
        if (GameManager.gameManager.prefabManager.GoldPopupPrefab == null)
        {
            return null;
        }

        // create popup and set the amount of gold to display
        Transform goldPopupTransform = Instantiate(GameManager.gameManager.prefabManager.GoldPopupPrefab, position,
            GameManager.gameManager.prefabManager.GoldPopupPrefab.rotation);
        GoldPopup goldPopup = goldPopupTransform?.GetComponent<GoldPopup>();
        goldPopup?.Setup(goldAmount);

        return goldPopup;
    }

    /// <summary>
    /// called when the script instance is being loaded
    /// </summary>
    private void Awake()
    {
        // initialize variables
        _textMesh = transform.GetComponent<TextMeshPro>();
        _disappearTimer = DisappearTimer;
        _textColor = _textMesh.color;
    }

    /// <summary>
    /// Sets up the popup
    /// </summary>
    /// <param name="goldAmount">the amount of gold the popup show</param>
    private void Setup(int goldAmount)
    {
        _textMesh.SetText("+" + goldAmount.ToString());
        // get popup to be rendered above other objects
        sortingOrder++;
        _textMesh.sortingOrder = sortingOrder;
    }

    /// <summary>
    /// Update phase in the native player loop
    /// </summary>
    private void Update()
    {
        // move popup up as time passes
        transform.position += new Vector3(0, moveSpeed) * Time.deltaTime;

        if (_disappearTimer > DisappearTimer * 0.5f)
        {
            // move popup faster during the first half of the timer
            transform.localScale += Vector3.one * ScaleAmount * Time.deltaTime;
        }
        else
        {
            // move popup slower during the second half of the timer
            transform.localScale -= Vector3.one * ScaleAmount * Time.deltaTime;
        }

        _disappearTimer -= Time.deltaTime;

        // when timer expired fade popup away, then remove it from scene
        if (_disappearTimer < 0)
        {
            _textColor.a -= DisappearSpeed * Time.deltaTime;
            _textMesh.color = _textColor;

            if (_textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
