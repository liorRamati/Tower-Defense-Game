using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // the target object for the bullet
    private Transform target;

    public float speed = 100f;

    public int damage = 50;

    // radius of damage for exloding bullets
    public float explosionRadius = 0f;

    // an animation for impact with target
    public GameObject impactEffect;

    // true if bullet hit an enemy, to make sure 2 enemies are not hit at the same time
    private bool _hitOccured;
    
    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    /// <summary>
    /// Update phase in the native player loop
    /// </summary>
    void Update()
    {
        // if target is not valid anymore destroy the object
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // get direction to the target and set rotation and movement accordingly
        Vector3 dir = target.position - transform.position;
        transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);
        transform.LookAt(target);
    }

    /// <summary>
    /// Explode and damage all enemy in explotion radius
    /// </summary>
    public void Explosion()
    {
        // colliders disabled cause of navmeshagent (see below)
        //Collider[] inRange = Physics.OverlapSphere(transform.position, explosionRadius);
        //foreach (Collider c in inRange)

        for (int i = GameManager.gameManager.Enemies.Count - 1; i >= 0; i--)
        {
            // if enemy is in eplotion radius, damage it
            if (Vector3.Distance(transform.position, GameManager.gameManager.Enemies[i].transform.position) <= explosionRadius)
            {
                InflictDamage(GameManager.gameManager.Enemies[i]);
            }
        }
    }

    /// <summary>
    /// Damage the given enemy
    /// </summary>
    public void InflictDamage(Enemy enemy)
    {
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
    }

    /// <summary>
    /// Gizmos are drawn only when the object is selected
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    /// <summary>
    /// called when this collider/rigidbody has begun touching another rigidbody/collider
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        // if collided with an enemy inflict damage
        if (collision.transform.tag == "Enemy")
        {
            // make sure only the first enemy detected is hit
            if (_hitOccured)
            {
                return;
            }
            _hitOccured = true;

            // create impact visual effect
            //GameObject effectInstance = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);
            //Destroy(effectInstance, 2f);

            // if the bullet is an explisove one, damage enemies in the area
            if (explosionRadius > 0f)
            {
                Explosion();
            }
            // otherwise damage the enemy
            else
            {
                InflictDamage(collision.transform.GetComponent<Enemy>());
            }
            Destroy(gameObject);
            return;
        }
    }

    /// <summary>
    /// called once per frame for every collider/rigidbody that is touching rigidbody/collider
    /// </summary>
    private void OnCollisionStay(Collision collision)
    {
        // make sure collision is detected even if the entry is missed
        OnCollisionEnter(collision);
    }

    /// collider deal with enemy hits (so no need for this code)
    /// <summary>
    /// Checks if the bullet hit an enemy (within bullet hit radius) and damage enemies accordingly
    /// </summary>
    //public void CheckHit()
    //{
    //    foreach (Enemy enemy in GameManager.gameManager.Enemies)
    //    {
    //        // if an enemy is in the hit radius inflict damage to relevant enemies
    //        if (Vector3.Distance(transform.position, enemy.transform.position) <= HitRadius)
    //        {
    //            // create effect for hit impact and remove after 2 seconds
    //            //GameObject effectInstance = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);
    //            //Destroy(effectInstance, 2f);

    //            // if there is an explosion radius hit enemies in the radius
    //            if (explosionRadius > 0f)
    //            {
    //                Explosion();
    //            }
    //            // if expolsion radius is 0, damage only the enemy hit by the bullet
    //            else
    //            {
    //                return;
    //                //InflictDamage(enemy);
    //            }

    //            Destroy(gameObject);
    //            return;
    //        }
    //    }
    //}
}


