using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerShooting : MonoBehaviour
{

    PlayerControls controls;

    public Transform target;
    public float range = 1.5f;

    public GameObject bulletPrefab;
    public Transform firepoint;

    public string enemyTag = "enemyTag";

    private GameObject tempEnemy = null;

    public LayerMask targetable;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Gameplay.shoot.performed += ctx => Shoot();
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }


private void Start()
    {
        //InvokeRepeating("UpdateTarget", 0f, 0.5f);
    }

    private void Update()
    {
        UpdateTarget();
    }


    void Shoot()
    {
        if (target != null)
        {
            GameObject newProjectile = (GameObject)Instantiate(bulletPrefab, firepoint.position, firepoint.rotation);
            IProjectile projectile = newProjectile.GetComponent<IProjectile>();
            if (projectile != null)
            {
                projectile.ReceiveTarget(target);
            }
        }
    }

    void UpdateTarget2()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestEnemy = enemy;
                tempEnemy = nearestEnemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= range)
        {
            if (target != null)
            {
                ITargetable oldEnemy = target.GetComponent<ITargetable>();
                oldEnemy.loseTarget();
            }
            target = nearestEnemy.transform;
            ITargetable newEnemy = target.GetComponent<ITargetable>();
            newEnemy.becomeTarget();

        }
        else
        {
            target = null;
            if (tempEnemy != null)
            { 
                tempEnemy.GetComponent<ITargetable>().loseTarget();
            }
        }
    }


    void UpdateTarget()
    {
        Collider2D[] collidersInRange = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), range, targetable);
        List<Collider2D> targets = new List<Collider2D>(collidersInRange);
        List<Collider2D> notTargets = new List<Collider2D>();
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;
        foreach (Collider2D target in targets)
        {
            if (target.gameObject.GetComponent<ITargetable>() != null)
            {
                float distance = Vector3.Distance(transform.position, target.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestEnemy = target.gameObject;
                    tempEnemy = nearestEnemy;
                }
            }
        }/*else
            {
                notTargets.Add(target);
            }          
        }
        foreach(Collider2D nontarget in notTargets)
        {
            targets.Remove(nontarget);
        }*/

        if (nearestEnemy != null && shortestDistance <= range)
        {
            if (target != null)
            {
                ITargetable oldEnemy = target.GetComponent<ITargetable>();
                oldEnemy.loseTarget();
            }
            target = nearestEnemy.transform;
            ITargetable newEnemy = target.GetComponent<ITargetable>();
            newEnemy.becomeTarget();

        }
        else
        {
            target = null;
            if (tempEnemy != null)
            {
                tempEnemy.GetComponent<ITargetable>().loseTarget();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}

