using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{

    [System.Serializable]
    public class Wave
    {
        public List<EnemyTypesEnum> enemiesTypes;
        [Tooltip("the time to wait from last wave")]
        public float waveDelay = 0f;

        [Tooltip("the interval between the spawns of each enemy")]
        public float spawnInterval = 1f;

        [Tooltip("Whether or not to wait to previous enemies spawn to die")]
        public bool waitForPrevious = false;
    }

    [System.Serializable]
    public class EnemyType
    {
        public GameObject enemyPrefab = null;
        public int[] availableRoutesIndexes = null;
    }

    [SerializeField]
    [Range(1f,2f)]
    // a speed multiplier for the enemies
    private float _speedMultiplier = 1f;

    public enum EnemyTypesEnum
    {
        SmallEnemy = 0,
        MediumEnemy = 1,
        BigEnemy = 2
    }

    [SerializeField]
    // array of enemies types available in the level
    private EnemyType[] _enemyTypes = new EnemyType[System.Enum.GetNames(typeof(EnemyTypesEnum)).Length];

    [SerializeField]
    private Wave[] _waves = null;

    private List<Route> _routes = null;

    private Quaternion _startDir = Quaternion.AngleAxis(-90, Vector3.up);

    private int _waveCount = 0;
    // timer for the delay between waves
    private float _waveTimer;
    private Task _task = null;

    /// <summary>
    /// called when the script instance is being loaded
    /// </summary>
    private void Awake()
    {
        // populate the list of routes availables for enemies using the routes object
        _routes = new List<Route>();
        Transform routesTransform = GameObject.Find("Routes")?.transform;
        foreach (Transform child in routesTransform)
        {
            _routes.Add(child.GetComponent<Route>());
        }
    }

    /// <summary>
    /// called on the frame when a script is enabled just before any of the Update methods are called the first time
    /// </summary>
    private void Start()
    {
        // initialize timer for the delay for the first wave
        if (_waves != null && _waves.Length > 0)
        {
            _waveTimer = _waves[0].waveDelay;
        }
    }

    private void Update()
    {
        // if previous wave is still spawning, wait for it to finish
        if (_task != null && _task.Running)
        {
            return;
        }

        // spawn next wave if there is one
        if (_waves != null && _waveCount < _waves.Length)
        {
            if (_waveTimer > 0)
            {
                // if next wave waits for all previous waves to clear do nothing
                if (_waves[_waveCount].waitForPrevious && GameManager.gameManager.Enemies.Count > 0)
                {
                    return;
                }
                _waveTimer -= Time.deltaTime;
            }
            // if the delay between wave passed, spawn next wave
            else
            {
                // set the delay timer for the next wave (according to next wave delay value)
                if (_waveCount + 1 < _waves.Length)
                {
                    _waveTimer = _waves[_waveCount+1].waveDelay;
                }
                // start spawning the current wave
                _task = new Task(SpawnWave());
            }
        }
        // if all waves spawn and all enemies killed, the level is won
        else if (GameManager.gameManager.Enemies.Count <= 0)
        {
            GameManager.gameManager.WinLevel();
            this.enabled = false;
        }
    }

    /// <summary>
    /// spawn a wave of enemies
    /// </summary>
    IEnumerator SpawnWave()
    {
        for (int i = 0; i < _waves[_waveCount].enemiesTypes.Count; i++)
        {
            // spawn an enemy
            SpawnEnemy(i);
            // wait before next spawn, according to the wave spawn interval value
            yield return new WaitForSeconds(_waves[_waveCount].spawnInterval);
        }

        _waveCount++;
    }

    /// <summary>
    /// Spawn an enemy with the given index in the current wave
    /// </summary>
    /// <param name="index">the index of the enemy in the wave</param>
    private void SpawnEnemy(int index)
    { 
        // get enemy type from wave info
        EnemyType type = _enemyTypes[(int)_waves[_waveCount].enemiesTypes[index]];
        // chose a random route from the available routes for the enemy type
        int routeIndex = type.availableRoutesIndexes[Random.Range(0, type.availableRoutesIndexes.Length)];
        // create enemy and initialize it
        Enemy temp = Instantiate(type.enemyPrefab, _routes[routeIndex].StartLocation, _startDir).GetComponent<Enemy>();
        temp.Init(_routes[routeIndex].Locations, _speedMultiplier);
    }

    /// <summary>
    /// Editor-only function that Unity calls when the script is loaded or a value changes in the Inspector
    /// </summary>
    private void OnValidate()
    {
        // make sure the size of enemy types matches the enum representing it
        if (_enemyTypes.Length != System.Enum.GetNames(typeof(EnemyTypesEnum)).Length)
        {
            System.Array.Resize(ref _enemyTypes, System.Enum.GetNames(typeof(EnemyTypesEnum)).Length);
            Debug.LogWarning("enemies' type array size should not be changed!");
        }
    }
}
