using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[ExecuteAlways]
public class Turret : MonoBehaviour
{
    // target the gun will aim at
    private Transform target;
    private Enemy targetEnemy;

    // reference to the part of the turret that rotates
    public Transform partToRotate;

    // reference to the bullet object shot by the turret
    public GameObject bulletPrefab;

    // Location in which the bullets spawn
    public Transform firePoint;

    [Header("Use Lasers")]
    public bool useLasers = false;
    public int damageOverTime = 30;
    public float slowFactor = 0.5f;

    private LineRenderer lineRenderer;

    [Header("Stats")]
    public int cost;

    // Gun barrel rotation speed
    public float rotationSpeed = 10;

    // value for switching targets when distance to new target plus this value is smaller from current target
    public float switchDistance = 10;

    // Distance the turret can aim and fire from
    public float firingRange = 100;

    // The rate (shot/sec) the turret can shot
    public float fireRate = 1f;

    // The radius of empty space needed on the ground
    public int baseRadius = 7;

    [Header("Upgrade")]
    public int upgradeCost;
    // reference to the upgraded turret object
    public GameObject turretUpgrade;

    // Timer for interval between firing bullets
    private float fireTimer;

    // the rate a which to invoke the courutines for the turret
    private float _invokingRate = 0.03f;

    // for AI program
    [HideInInspector]
    public bool didFireOnce;

    /// <summary>
    /// called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        if (useLasers)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
        didFireOnce = false;
    }

    /// <summary>
    /// called on the frame when a script is enabled just before any of the Update methods are called the first time.
    /// </summary>
    private void Start()
    {
        // try to find a new target every 0.1 seconds
        InvokeRepeating("FindTarget", 0f, _invokingRate);
        InvokeRepeating("ActivateTurret", 0f, _invokingRate);
    }

    /// <summary>
    /// Update phase in the native player loop.
    /// </summary>
    private void Update()
    {
        if (target == null)
        {
            // disable the laser ray if no target in range
            if (useLasers)
            {
                if (lineRenderer.enabled)
                {
                    lineRenderer.enabled = false;
                }
            }

            return;
        }

        // rotate turret towards target
        Aim();
    }

    /// <summary>
    /// Handles the laser option for the turret
    /// </summary>
    private void Laser()
    {
        didFireOnce = true;

        // inflict damage to enemy
        targetEnemy.TakeDamage(damageOverTime * Time.deltaTime);
        // slow enemy
        targetEnemy.Slow(slowFactor);

        // enable ray display if not enabled
        if (!lineRenderer.enabled)
        {
            lineRenderer.enabled = true;
        }

        // set ray origin and end
        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, target.position);
    }

    /// <summary>
    /// searches for a target in firing range, or if a closer target is available
    /// </summary>
    public void FindTarget()
    {
        // if current target is now out of range, deselect it as target
        if (target != null && Vector3.Distance(target.position, transform.position) > firingRange)
        {
            target = null;
            targetEnemy.StopSlow();
            targetEnemy = null;
        }

        // look for new target
        foreach(Enemy enemy in GameManager.gameManager.Enemies)
        {
            // if there is an enemy in the firing range chekc if should switch to it
            if (Vector3.Distance(transform.position, enemy.transform.position) <= firingRange)
            {
                // if no current target, or the new eneny is closer than current target (including switching distance) switch
                if (target == null || Vector3.Distance(enemy.transform.position, transform.position) + switchDistance <
                    Vector3.Distance(target.position, transform.position))
                {
                    if (target != null)
                    {
                        targetEnemy.StopSlow();
                    }
                    target = enemy.transform;
                    targetEnemy = enemy;
                }
            }
        }
    }

    /// <summary>
    /// Gizmos are drawn only when the object is selected
    /// </summary>
    void OnDrawGizmosSelected()
    {
        // Draw a red sphere at the transform's position to show the firing range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, firingRange);
    }

    /// <summary>
    /// turn the turret towards the target
    /// </summary>
    private void Aim()
    {
        // get direction to target
        Vector3 dir = target.position - partToRotate.position;
        // find the necessary rotation of the turret and apply it
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, rotationSpeed * Time.deltaTime).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0, rotation.y, 0);
    }

    /// <summary>
    /// fire a bullet of the target
    /// </summary>
    private void Fire()
    {
        didFireOnce = true;

        // create bullet
        GameObject bulletGO = (GameObject)Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bullet = bulletGO.GetComponent<Bullet>();

        // set the target for the bullet
        if (bullet != null)
        {
            bullet.SetTarget(target);
        }
    }

    /// <summary>
    /// times the activation of the turret, damage the enemy with laser continously, or firing bullets at a given fire rate
    /// </summary>
    private void ActivateTurret()
    {
        if (useLasers)
        {
            if (target != null)
            {
                // use the laser of the target
                Laser();
            }
        }
        else
        {
            // if the time between shots passed since the last shot fired shot
            if (fireTimer <= 0)
            {
                if (target != null)
                {
                    Fire();
                    fireTimer = 1f / fireRate;
                }
            }
            // reduce time from timer for time between shots
            fireTimer -= _invokingRate;
        }
    }
}