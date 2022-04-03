using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public float startHealth = 100;

    [HideInInspector]
    public float health;

    // the gold the player gets when killing the enemy
    public int moneyValue = 20;

    public Image healthBar;

    // waypoint for the road the enemy traverses
    private List<Vector3> _destinations;

    private int destIndex = 0;

    // used for slowing the enemy
    private float startSpeed;

    // a speed multiplier to make level more difficult
    private float _speedMultiplier = 1f;

    // agent for enemy AI movement
    private NavMeshAgent _agent;

    // for consistency
    private bool isDead = false;

    // events for AI program
    public static event Action<int> EnemyDiedEvent;
    public static event Action EnemyDamagedBaseEvent;
    public static event Action<float> EnemyDamagedEvent;

    // used to maked sure enemy return to original speed when slow stops
    private bool _slowed = false;

    /// <summary>
    /// initiate values for the movement of the enemy
    /// </summary>
    /// <param name="destinations">way point for the road the enemy traverses</param>
    /// <param name="speedMultiplier">speed multiplier for inhancing difficulty</param>
    public void Init(List<Vector3> destinations, float speedMultiplier)
    {
        this._destinations = destinations;
        _speedMultiplier = speedMultiplier;
        startSpeed = _agent.speed * _speedMultiplier;
    }

    /// <summary>
    /// called when the script instance is being loaded
    /// </summary>
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        health = startHealth;
    }

    /// <summary>
    /// called on the frame when a script is enabled just before any of the Update methods are called the first time
    /// </summary>
    private void Start()
    {
        GameManager.gameManager.AddEnemy(this);
    }

    /// <summary>
    /// inflict given damage to the enemy
    /// </summary>
    public void TakeDamage(float amount)
    {
        // remove damage for health
        health -= amount;
        EnemyDamagedEvent?.Invoke(amount);

        // adjust healthbar display
        healthBar.fillAmount = health / startHealth;

        // if health below 0 kill enemy
        if (health <= 0 && !isDead)
        {
            Die();
        }
    }

    /// <summary>
    /// slow enemy movement by given factor
    /// </summary>
    public void Slow(float factor)
    {
        _agent.speed = startSpeed * (1f - factor);
        _slowed = true;
    }

    /// <summary>
    /// return the enemy speed to the original speed
    /// </summary>
    public void StopSlow()
    {
        _agent.speed = startSpeed;
        _slowed = false;
    }

    /// <summary>
    /// deying squence for the enemy
    /// </summary>
    private void Die()
    {
        isDead = true;

        // add money to player
        GameManager.gameManager.playerStats.IncreaseMoney(moneyValue);
        // creates money popup
        GoldPopup.Create(healthBar.transform.position, moneyValue);
        // add to statistics
        GameManager.gameManager.playerStats.enemiesKilled++;
        // remove enemy from tracker
        GameManager.gameManager.RemoveEnemy(this);
        // invoke enemy death event (for AI program)
        EnemyDiedEvent?.Invoke(moneyValue);

        Destroy(gameObject);
        return;
    }

    /// <summary>
    /// Update phase in the native player loop
    /// </summary>
    private void Update()
    {
        // if there are no waypoint, do nothing. TO DO: throw execption
        if (_destinations.Count == 0)
        {
            return;
        }

        if (!_slowed)
        {
            StopSlow();
        }

        // if reached current waypoint, switch target to next checkpoint (if didn't do that already)
        if (!_agent.pathPending && _agent.remainingDistance < 15)
        {
            if (destIndex < _destinations.Count)
            {
                _agent.SetDestination(_destinations[destIndex]);
                destIndex++;
            }
            // if the last waypoint is reached, damage the player's base
            else
            {
                EndPath();
                return;
            }
        }
    }

    /// <summary>
    /// remove life from player's lives count, as enemy reach player's base
    /// </summary>
    private void EndPath()
    {
        // remove player life
        GameManager.gameManager.playerStats.ReduceLives();
        // remove enemy from tracker
        GameManager.gameManager.RemoveEnemy(this);
        // invoke event for enemy getting to player's base (for AI program)
        EnemyDamagedBaseEvent?.Invoke();

        Destroy(gameObject);
    }
}
