using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(WaveSpawner))]
[RequireComponent(typeof(Utils))]
[RequireComponent(typeof(BuildManager))]
public class GameManager : MonoBehaviour
{
    // static reference for singelton pattern
    private static GameManager _gameManager;

    public static GameManager gameManager
    {
        get
        {
            return _gameManager;
        }
    }

    public string menuSceneName = "MainMenu";

    [HideInInspector]
    public PlayerStats playerStats;

    [HideInInspector]
    public WaveSpawner waveSpawner;

    [HideInInspector]
    public BuildManager buildManager;

    [HideInInspector]
    public Utils Utils;

    [HideInInspector]
    public SceneFader sceneFader;

    [HideInInspector]
    public PrefabManager prefabManager;

    public GameObject GameOverUI;

    public GameObject LevelWonUI;

    // list to track enemies in the level
    private List<Enemy> _enemies;

    public List<Enemy> Enemies {
        get => _enemies;
    }

    // list to track turret in the level
    private List<Turret> _turrets;

    public List<Turret> Turrets
    {
        get => _turrets;
    }
    
    public bool GameEnded
    {
        get;
        private set;
    } = false;

    [HideInInspector]
    public bool GamePaused = false;

    // events for AI program
    public static event Action GameLostEvent;
    public static event Action GameWonEvent;

    /// <summary>
    /// called when the script instance is being loaded
    /// </summary>
    private void Awake()
    {
        // populate the game manager static reference if needed
        if (_gameManager == null)
        {
            _gameManager = this;
        }
        else if (_gameManager != this)
        {
            Destroy(gameObject);
        }

        // get references to the game management objects
        playerStats = GetComponent<PlayerStats>();
        waveSpawner = GetComponent<WaveSpawner>();
        buildManager = GetComponent<BuildManager>();
        Utils = GetComponent<Utils>();
        sceneFader = FindObjectOfType<SceneFader>();
        prefabManager = GetComponent<PrefabManager>();
        _enemies = new List<Enemy>();
        _turrets = new List<Turret>();
    }

    /// <summary>
    /// Update phase in the native player loop
    /// </summary>
    private void Update()
    {
        if (GameEnded)
        {
            return;
        }
        // pressing the 'e' key win end the game
        if (Input.GetKeyDown("e"))
        {
            EndGame();
        }
    }

    /// <summary>
    /// Handles when level is lost
    /// </summary>
    public void EndGame()
    {
        // do nothing if game was already done
        if (GameEnded)
        {
            return;
        }

        GameEnded = true;

        // enables ui to display the game over screen
        GameOverUI.SetActive(true);
        //print("Game Over!");

        // invoke game lost event (for AI program)
        GameLostEvent?.Invoke();
    }

    /// <summary>
    /// Handles when level is won
    /// </summary>
    public void WinLevel()
    {
        // do nothing if game was already done
        if (GameEnded)
        {
            return;
        }

        GameEnded = true;
        // enable ui to display the game won screen
        LevelWonUI.SetActive(true);

        // invoke game won event (for AI program)
        GameWonEvent?.Invoke();
    }

    /// <summary>
    /// Adds an enemy to the list of enemies
    /// </summary>
    public void AddEnemy(Enemy enemy)
    {
        _enemies.Add(enemy);
    }

    /// <summary>
    /// Removes an enemy from the list of enemies
    /// </summary>
    public void RemoveEnemy(Enemy enemy)
    {
        _enemies.Remove(enemy);
    }
}
