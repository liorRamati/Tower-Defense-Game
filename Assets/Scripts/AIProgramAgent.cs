using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine.SceneManagement;

public class AIProgramAgent : Agent
{
    enum TurretTypes
    {
        regular = 0,
        missileLauncher = 1,
        laserBeamer = 2,
        upgrade = 3,
        sell = 4
    }

    [Header("Rewards")]
    // reward values for defferent actions and results
    [SerializeField]
    [Tooltip("reward when a turret cannot be built in the location choosen")]
    private float _illegalLocationReward;
    [SerializeField]
    [Tooltip("reward when the location is legal but there is another turret in the area")]
    private float _illegalProximityReward;
    [SerializeField]
    private float _successfullyBuiltReward;
    [SerializeField]
    private float _noMoenyReward;
    [SerializeField]
    private float _noOpReward;
    [SerializeField]
    private float _winReward;
    [SerializeField]
    private float _lossReward;
    [SerializeField]
    private float _yetToFireReward;

    [Header("Links")]
    // prefabs for turrets
    [SerializeField]
    private Turret _reuglarTurretPrefab;
    [SerializeField]
    private Turret _missileLauncherPrefab;
    [SerializeField]
    private Turret _laserBeamerPrefab;

    [SerializeField]
    private Terrain _terrain;

    // for getting the next level to load
    [SerializeField]
    private LevelWon _levelWonObject;

    [Header("Agent values")]
    [SerializeField]
    [Tooltip("The maximum distance to search for a turret for upgrade or sell")]
    private int _maxDistanceTurretSearch;

    private Vector3 _terrainSize;

    private float _maxReward;

    private BufferSensorComponent _bufferSensor;

    public override void Initialize()
    {
        _terrainSize = _terrain.terrainData.size;
        _maxReward = Mathf.Max(Mathf.Abs(_illegalLocationReward), Mathf.Abs(_successfullyBuiltReward), Mathf.Abs(_noMoenyReward), Mathf.Abs(_noOpReward),
            Mathf.Abs(_winReward), Mathf.Abs(_lossReward));
        _bufferSensor = GetComponent<BufferSensorComponent>();
        InvokeRepeating("YetToFireReward", 0f, 0.1f);
    }

    /// <summary>
    /// set up an Agent instance at the beginning of an episode
    /// </summary>
    public override void OnEpisodeBegin()
    {
        GameManager.GameWonEvent += GameWonReward;
        GameManager.GameLostEvent += GameLostReward;
        Enemy.EnemyDiedEvent += EnemyDiedReward;
        Enemy.EnemyDamagedEvent += EnemyDamagedReward;
    }

    /// <summary>
    /// collect the vector observations of the agent for the step
    /// </summary>
    /// <param name="sensor"></param>
    public override void CollectObservations(VectorSensor sensor)
    {
        // add player states observations
        sensor.AddObservation(GameManager.gameManager.playerStats.currentLives);
        sensor.AddObservation(GameManager.gameManager.playerStats.currentMoney);

        float[] enemyObservations = new float[_bufferSensor.ObservableSize];
        // add observations for each enemy
        foreach (Enemy e in GameManager.gameManager.Enemies)
        {
             if (e == null)
            {
                continue;
            }
            // normalizing enemy position to be between 0 and 1 (no y coordinate as it is irrelevant)
            float x_pos = GameManager.gameManager.Utils.Map(e.transform.position.x, 0f, _terrainSize.x);
            enemyObservations[0] = x_pos;
            float z_pos = GameManager.gameManager.Utils.Map(e.transform.position.z, 0f, _terrainSize.z);
            enemyObservations[1] = z_pos;

            // add enemy health
            enemyObservations[2] = e.health / (float)e.startHealth;

            // add observation of 'enemy or turret'
            enemyObservations[3] = 1f;
            enemyObservations[4] = 0f;

            _bufferSensor.AppendObservation(enemyObservations);
        }

        float[] turretObservations = new float[_bufferSensor.ObservableSize];
        // add observations for each turret
        foreach (Turret t in GameManager.gameManager.Turrets)
        {
            if (t == null)
            {
                continue;
            }
            // normalizing turret position to be between 0 and 1 (no y coordinate as it is irrelevant)
            float x_pos = GameManager.gameManager.Utils.Map(t.transform.position.x, 0f, _terrainSize.x);
            turretObservations[0] = x_pos;
            float z_pos = GameManager.gameManager.Utils.Map(t.transform.position.z, 0f, _terrainSize.z);
            turretObservations[1] = z_pos;

            turretObservations[2] = 0f;
            // add observation of 'enemy or turret'
            turretObservations[3] = 0f;
            turretObservations[4] = 1f;
            _bufferSensor.AppendObservation(turretObservations);
        }
    }

    /// <summary>
    /// specify agent behavior at every step, based on the provided action
    /// </summary>
    public override void OnActionReceived(ActionBuffers actions)
    {
        // convert position from [-1,1] to terrain dimensions
        float x_pos = GameManager.gameManager.Utils.Map(actions.ContinuousActions[0], -1f, 1f, 0f, _terrainSize.x);
        float z_pos = GameManager.gameManager.Utils.Map(actions.ContinuousActions[1], -1f, 1f, 0f, _terrainSize.z);
        //Debug.LogFormat("{0}, {1}, {2}", actions.DiscreteActions[0], actions.ContinuousActions[0], actions.ContinuousActions[1]);


        // find the position on the game surface according to the x and z coordinates
        Vector3 surfacePosition = Vector3.zero;
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(x_pos, _terrainSize.y, z_pos), -Vector3.up, out hit))
        {
            surfacePosition = hit.point;
        }

        // try to build, upgrade or sell turret in the given location (or do nothing if no-op is choosen)
        int builtSuccess = 1;
        switch (actions.DiscreteActions[0])
        {
            case (int)TurretTypes.regular:
                builtSuccess = GameManager.gameManager.buildManager.BuildTurrent(_reuglarTurretPrefab, surfacePosition, Quaternion.identity, true);
                break;
            case (int)TurretTypes.missileLauncher:
                builtSuccess = GameManager.gameManager.buildManager.BuildTurrent(_missileLauncherPrefab, surfacePosition, Quaternion.identity, true);
                break;
            case (int)TurretTypes.laserBeamer:
                builtSuccess = GameManager.gameManager.buildManager.BuildTurrent(_laserBeamerPrefab, surfacePosition, Quaternion.identity, true);
                break;
            //case (int)TurretTypes.upgrade:
            //    // upgrade the closest turret to the selected position, within the max search range
            //    Turret t = FindClosestTurret(new Vector2(x_pos, z_pos));
            //    if (t == null)
            //    {
            //        builtSuccess = 1;
            //        break;
            //    }
            //    builtSuccess = GameManager.gameManager.buildManager.UpgradeTurret(t);
            //    break;
            //case (int)TurretTypes.sell:
            //    // sell the closest turret to the selected position, within the max search range
            //    t = FindClosestTurret(new Vector2(x_pos, z_pos));
            //    if (t == null)
            //    {
            //        builtSuccess = 1;
            //        break;
            //    }
            //    GameManager.gameManager.buildManager.SellTurret(t);
            //    builtSuccess = 0;
            //    break;
            default:
                // no-op
                builtSuccess = 4;
                break;
        }
        Debug.Log(builtSuccess);

        // reward the agent based on the result of the build try
        switch (builtSuccess)
        {
            case 1:
                AddNormalizedReward(_illegalLocationReward);
                break;
            case 2:
                AddNormalizedReward(_illegalProximityReward);
                break;
            case 3:
                AddNormalizedReward(_noMoenyReward);
                break;
            case 0:
                AddNormalizedReward(_successfullyBuiltReward);
                break;
            case 4:
                AddNormalizedReward(_noOpReward);
                break;
            default:
                //AddNormalizedReward(0f);
                break;
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        if (Input.GetMouseButton(0))
        {
            // get the position on the ground where the mouse is
            Vector3 newPos = Vector3.zero;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 500000f))
            {
                newPos = hit.point;
            }
            
            continuousActions[0] = GameManager.gameManager.Utils.Map(newPos.x, 0f, _terrainSize.x, -1f, 1f);
            continuousActions[1] = GameManager.gameManager.Utils.Map(newPos.z, 0f, _terrainSize.z, -1f, 1f);
            discreteActions[0] = 0;
        }
        else
        {
            discreteActions[0] = 3;
        }
    }

    /// <summary>
    /// Finds the closest turret to the given position (only x and z coordinates) but within a maximum distance.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns>the closest turret to the position, or null if there is no turret in the area around the position</returns>
    private Turret FindClosestTurret(Vector2 pos)
    {
        int closestDistance = _maxDistanceTurretSearch;
        Turret closestTurret = null;
        foreach (Turret t in GameManager.gameManager.Turrets)
        {
            if (Vector2.Distance(pos, new Vector2(t.transform.position.x, t.transform.position.z)) < closestDistance)
            {
                closestTurret = t;
            }
        }

        return closestTurret;
    }

    /// <summary>
    /// load a new level and end the episode
    /// </summary>
    private void Reset(string levelToLoad)
    {
        GameManager.GameWonEvent -= GameWonReward;
        GameManager.GameLostEvent -= GameLostReward;
        Enemy.EnemyDiedEvent -= EnemyDiedReward;
        Enemy.EnemyDamagedEvent -= EnemyDamagedReward;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// handle rewardinf the agent when game is won
    /// </summary>
    private void GameWonReward()
    {
        AddNormalizedReward(_winReward);
        Debug.Log("won " + GetCumulativeReward());
        Reset(_levelWonObject.nextLevel);
    }

    /// <summary>
    /// handle rewarding the agnet when game is lost
    /// </summary>
    private void GameLostReward()
    {
        AddNormalizedReward(_lossReward);
        Debug.Log("lost " + GetCumulativeReward());
        Reset(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// handle rewarding the agent when enemy is killed
    /// </summary>
    private void EnemyDiedReward(int rewardAmount)
    {
        AddNormalizedReward(rewardAmount);
    }

    /// <summary>
    /// handle rewarding the agent when enemy recieves damage
    /// </summary>
    /// <param name="amount"></param>
    private void EnemyDamagedReward(float amount)
    {
        AddNormalizedReward(amount / 100);
    }

    /// <summary>
    /// handle rewarding turret that didnt fire even once
    /// </summary>
    private void YetToFireReward()
    {
        foreach (Turret t in GameManager.gameManager.Turrets)
        {
            if (!t.didFireOnce)
            {
                AddNormalizedReward(_yetToFireReward * t.cost / 100);
            }
        }
    }

    /// <summary>
    /// add a reward value normalized by the biggest single reward
    /// </summary>
    private void AddNormalizedReward(float amount)
    {
        AddReward(amount / _maxReward);
    }
}
