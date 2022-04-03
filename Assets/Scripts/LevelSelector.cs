using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    public SceneFader sceneFader;

    // list of buttons for the different levels
    private List<Button> _levelButtons;

    [SerializeField]
    private Transform _buttonGrid;

    public GameObject LevelButtonPrefab;

    /// <summary>
    /// called when the script instance is being loaded
    /// </summary>
    private void Awake()
    {
        // initilize variables
        _buttonGrid = _buttonGrid ?? GameObject.FindGameObjectWithTag("Grid").transform;
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        int levelCount = 0;
        _levelButtons = new List<Button>();
        // create buttons for each level according to the different level scenes
        for (int i = 0; i < sceneCount; i++)
        {
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
            if (sceneName.StartsWith("Level") && sceneName != "LevelSelector")
            {
                levelCount++;
                GameObject temp = Instantiate<GameObject>(LevelButtonPrefab, _buttonGrid);
                temp.transform.FindDeepChild("Text").GetComponent<Text>().text = levelCount.ToString();
                Button tempButton = temp.GetComponent<Button>();
                _levelButtons.Add(tempButton);
                // set button to load the level
                tempButton.onClick.AddListener(delegate{ SelectLevel(sceneName); });
            }
        }
    }

    /// <summary>
    /// called on the frame when a script is enabled just before any of the Update methods are called the first time
    /// </summary>
    private void Start()
    {
        // get the latest level unlocked
        int levelReached = PlayerPrefs.GetInt("levelReached", 1);

        // enable the buttons for the levels that were already reached
        for (int i = 0; i < _levelButtons.Count; i++)
        {
            if (i + 1 > levelReached)
            {
                _levelButtons[i].interactable = false;
            }
        }
    }

    /// <summary>
    /// load the level with the given name
    /// </summary>
    public void SelectLevel(string levelName)
    {
        sceneFader.FadeTo(levelName);
    }
}
