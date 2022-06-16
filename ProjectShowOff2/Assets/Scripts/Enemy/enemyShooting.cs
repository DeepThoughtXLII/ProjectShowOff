using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class enemyShooting : MonoBehaviour
{
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     FIELDS
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public enum shootingType { BASIC, SHADOW, ARROWHAIL }
    [SerializeField] private shootingType shooting = shootingType.BASIC;

    public Transform target = null;
    public float range = 1.5f;

    public GameObject bulletPrefab;
    public Transform firepoint;

    public float firerate = 0.2f;
    public int damage = 1;

    bool readyToShoot = true;
    public bool charging = false;

    List<GameObject> activeArrowHails;

    public int bulletDamage;

    TargetingManager targetingManager;

    enemyPathing _enemyPathing;

    hailOfArrows _hailOfArrows;

    public float MeleeRange;

    [Header("Shadow Attacks")]
    public int EmergeDamage;
    public float EmergeSpeed;
    public float TimeBeforeDissapear;

    private CircleCollider2D collisionBox;
    public bool emerging = false;

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     START
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void Start()
    {
        collisionBox = gameObject.GetComponent<CircleCollider2D>();
        _enemyPathing = gameObject.GetComponent<enemyPathing>();
        // players = GameObject.FindGameObjectsWithTag("Player");
        targetingManager = GameObject.FindGameObjectWithTag("targetManager").GetComponent<TargetingManager>();
        activeArrowHails = new List<GameObject>();
        getTarget();
    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     UPDATE
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void Update()
    {
        getTarget();
        if (target != null)
        {
            if (readyToShoot && shooting == shootingType.BASIC)
            {
                StartCoroutine(Shoot());
            }

            if (readyToShoot && shooting == shootingType.SHADOW)
            {
                StartCoroutine(Emerging());
            }

            if (readyToShoot && shooting == shootingType.ARROWHAIL)
            {
                StartCoroutine(ArrowHailAttack());
            }
        }
    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     UPDATE TARGET
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void updateTarget()
    {
        if (Vector3.Distance(transform.position, target.position) > range)
        {
            target = null;
        }
    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     GET TARGET
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void getTarget()
    {
        Player p = targetingManager.GetTargetInShootingRange(transform, range);
        if (p != null)
        {
            target = p.transform;

        }
        if (target != null)
        {
            updateTarget();
        }
    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     SHOOT
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
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

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     EMERGING
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public IEnumerator Emerging()
    {
        readyToShoot = false;
        yield return new WaitForSeconds(EmergeSpeed);
        meleeAttack();
        collisionBox.enabled = true;
        
        yield return new WaitForSeconds(TimeBeforeDissapear);
        collisionBox.enabled = false;
        emerging = false;
        _enemyPathing.rend.color = Color.gray;
        readyToShoot = true;

    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     ARROW HAIL ATTACK
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    IEnumerator ArrowHailAttack()
    {
        readyToShoot = false;
        GameObject newProjectile = (GameObject)Instantiate(bulletPrefab, firepoint.position, firepoint.rotation);
        activeArrowHails.Add(newProjectile);
        _hailOfArrows = newProjectile.GetComponent<hailOfArrows>();
        _hailOfArrows.ReceiveTarget(target);
        _hailOfArrows.setAttackLocation(gameObject.transform);
        _hailOfArrows.highlightArea.SetActive(true);
        charging = true;
        yield return new WaitForSeconds(_hailOfArrows.chargeTime);
        charging = false;
        _hailOfArrows.highlightArea.SetActive(false);
        _hailOfArrows.attackArea.SetActive(true);
        yield return new WaitForSeconds(_hailOfArrows.attackDuration);
        activeArrowHails.Remove(newProjectile);
        Destroy(newProjectile);
        readyToShoot = true;

    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     MELEE ATTACK
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void meleeAttack()
    {
        if (target != null)
        {
            if (Vector2.Distance(target.position, transform.position) < MeleeRange)
            {
                IDamageable playerDam = target.GetComponent<IDamageable>();
                playerDam.takeDamage(EmergeDamage);
            }

        }
    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     GIZMO SELECTED
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     ON DESTROY
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void OnDestroy()
    {
        foreach(GameObject gameObject in activeArrowHails)
        {
            Destroy(gameObject);
        }
    }
}
