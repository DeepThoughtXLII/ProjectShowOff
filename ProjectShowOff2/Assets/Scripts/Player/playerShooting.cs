using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerShooting : MonoBehaviour
{

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     FIELDS
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    PlayerInput pi;

    public Transform target;
    public float range = 1.5f;

    public GameObject bulletPrefab;
    public IProjectile bulletScriptPrefab;
    public Transform firepoint;

    public string enemyTag = "enemyTag";

    private GameObject tempEnemy = null;

    public LayerMask targetable;

    private Player player;

    PlayerHealth playerHealth;

    public int dmg = 0;

    public weaponAnimation weapon;


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     AWAKE()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void Awake()
    {
        //controls = new PlayerControls();
     
    }



    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     CHECK DAMAGE VALUES()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void checkDamageValues()
    {
        if(dmg != bulletScriptPrefab.Damage)
        {
            bulletScriptPrefab.Damage = dmg;
        }
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     START()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void Start()
    {

        player = GetComponent<Player>();
        playerHealth = player.GetPlayerHealth();
        //controls.Gameplay.shoot.performed += ctx => Shoot();
        bulletScriptPrefab = bulletPrefab.GetComponent<IProjectile>();
        //dmg = bulletScriptPrefab.Damage;
        weapon = GetComponentInChildren<weaponAnimation>();

        Levelable.onUpgradeChosen += checkDamageValues;
        pi = GetComponentInChildren<PlayerInput>();
        if (pi != null)
        {
            pi.onActionTriggered += OnAction;
        }
        //InvokeRepeating("UpdateTarget", 0f, 0.5f);
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     ON ACTION()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void OnAction(InputAction.CallbackContext ctx)
    {
        if(playerHealth.State != PlayerHealth.PlayerState.REVIVING)
        {
            if (ctx.action.name == "shoot" && ctx.action.phase == InputActionPhase.Performed)
            {
                StartCoroutine(Shoot());
                
            }
        }
        
    }



    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     UPDATE()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void Update()
    {
        UpdateTarget();
        if(target != null)
        {
            Vector2 direction = target.position - transform.position;
            weapon.faceDirection(direction.normalized);
        }
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     SHOOT()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public IEnumerator Shoot()
    {
        if (target != null)
        {
            weapon.playShootAnimation();
            yield return new WaitForSeconds(weapon.getAnimationLength());

            GameObject newProjectile = (GameObject)Instantiate(bulletPrefab, firepoint.position, firepoint.rotation);
            IProjectile projectile = newProjectile.GetComponent<IProjectile>();
            FindObjectOfType<SoundManager>().Play("playerShoot");
            if (projectile != null)
            {
                projectile.ReceiveTarget(target, dmg, player.Id);
            }
        }
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     UPDATE TARGET()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
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


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     UPDATE TARGET()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
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

