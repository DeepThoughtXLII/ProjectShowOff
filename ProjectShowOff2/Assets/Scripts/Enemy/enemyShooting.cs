                                                                                                                                                       using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class enemyShooting : MonoBehaviour
{
    GameObject[] players;
    List<Transform> targetsInRange = new List<Transform>();
    public Transform target = null;
    public float range = 1.5f;

    public GameObject bulletPrefab;
    public Transform firepoint;

    public float firerate = 0.2f;
    public int damage = 1;

    bool readyToShoot = true;

    public int bulletDamage;

    TargetingManager targetingManager;

    enemyPathing _enemyPathing;

    public float MeleeRange;

    [Header("Shadow Attacks")]
    public int EmergeDamage;
    public float EmergeSpeed;
    public float TimeBeforeDissapear;

    private CircleCollider2D collisionBox;
    public bool emerging = false;


    private void Start()
    {
        collisionBox = gameObject.GetComponent<CircleCollider2D>();
        _enemyPathing = gameObject.GetComponent<enemyPathing>();
        // players = GameObject.FindGameObjectsWithTag("Player");
        targetingManager = GameObject.FindGameObjectWithTag("targetManager").GetComponent<TargetingManager>();
    }


    private void Update()
    {
        getTarget();
        if (target != null && readyToShoot)
        {
            StartCoroutine(Shoot());
        }
    }

    void updateTarget()
    {
        if(Vector3.Distance(transform.position, target.position) > range)
        {
            target = null;
        }
    }

    void getTarget()
    {
        Player p = targetingManager.GetTargetInShootingRange(transform, range);
        if(p != null)
        {
            target = p.transform;

        }
        if (target != null)
        {
            updateTarget();
        }
    }

    IEnumerator Shoot()
    {
        if (target != null)
        {
            GameObject newProjectile = (GameObject)Instantiate(bulletPrefab, firepoint.position, firepoint.rotation);
            IProjectile projectile = newProjectile.GetComponent<IProjectile>();
            if (projectile != null)
            {
                projectile.ReceiveTarget(target, bulletDamage);
            }
        }
        readyToShoot = false;
        yield return new WaitForSeconds(firerate);
        readyToShoot = true;
    }

    public void meleeAttack(Player player)
    {
        if (Vector2.Distance(player.transform.position, transform.position) < MeleeRange)
        {
            IDamageable playerDam = player.GetComponent<IDamageable>();
            playerDam.takeDamage(EmergeDamage);
        }
    }

    public IEnumerator Emerging(Player player)
    {
        yield return new WaitForSeconds(EmergeSpeed);
        meleeAttack(player);
        collisionBox.enabled = true;
        yield return new WaitForSeconds(TimeBeforeDissapear);
        collisionBox.enabled = false;
        emerging = false;
        _enemyPathing.rend.color = Color.gray;

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

}
