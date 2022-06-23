using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Boss : MonoBehaviour, ITargetable
{
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     FIELDS
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public bool isTarget = false;

    PlayerInput pi;

    SpriteRenderer rend;

    public bool isPlayer = true;



    Color defColor;
    public Color targetColor = Color.white;

    Player player;

    public GameObject bulletPrefab;



    public float range = 1.5f;

    public float speed = 3f;


    public Transform firepoint;

    public float firerate = 0.2f;

    bool readyToShoot = true;

    public int bulletDamage = 1;

    public int circularCount = 8;

    public TargetingManager targetingManager;

    Rigidbody2D rb;

    public Vector2 movement;

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     START()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void Start()
    {
        if (isPlayer == true)
        {
            player = GetComponent<Player>();
            pi = GetComponentInChildren<PlayerInput>();
            if (pi != null)
            {
                pi.onActionTriggered += OnAction;
            }
        }

        if (isPlayer == false)
        {
            targetingManager = GameObject.FindGameObjectWithTag("targetManager").GetComponent<TargetingManager>();
            player = targetingManager.GetTarget(transform);
            rb = gameObject.GetComponent<Rigidbody2D>();
        }

        rend = GetComponent<SpriteRenderer>();
        defColor = rend.color;


        gameObject.layer = LayerMask.NameToLayer("targetable");
        firepoint = transform.GetChild(0);

        firepoint.position = transform.position;

    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     ON ACTION()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void OnAction(InputAction.CallbackContext ctx)
    {
        if (player.GetPlayerHealth().State != PlayerHealth.PlayerState.REVIVING)
        {
            if (ctx.action.name == "shoot" && ctx.action.phase == InputActionPhase.Performed)
            {
                CanShoot();
                FindObjectOfType<SoundManager>().Play("playerShoot");
            }
        }

    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     BECOME TARGET()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void becomeTarget()
    {
        if (isTarget == false)
        {
            rend.color = targetColor;
            isTarget = true;
        }

    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     LOSE TARGET()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void loseTarget()
    {
        rend.color = defColor;
        isTarget = false;
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     CAN SHOOT()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public void CanShoot()
    {
        if (readyToShoot)
        {
            StartCoroutine(Shoot());
        }
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     SHOOT()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    IEnumerator Shoot()
    {
        readyToShoot = false;
        Vector3 beginDir = new Vector3(1, 0, 0);
        float angle = 360 / circularCount;
        for (int i = 0; i < circularCount; i++)
        {
            Quaternion rotate = Quaternion.AngleAxis(angle * i, Vector3.forward);
            Vector3 rotateDir = rotate * beginDir;
            GameObject newProjectile = (GameObject)Instantiate(bulletPrefab, firepoint.position, firepoint.rotation);
            IProjectile projectile = newProjectile.GetComponent<IProjectile>();
            projectile.ReceiveDirection(rotateDir, bulletDamage);
            newProjectile.transform.position = firepoint.position + rotateDir * 2;
        }

        yield return new WaitForSeconds(firerate);
        readyToShoot = true;
    }

    void FixedUpdate()
    {
        if(isPlayer == false)
        {
            player = targetingManager.GetTarget(transform);
            BossAI();
        }
    }

    void BossAI()
    {
        //pathing
        AIPathing();
        //shooting
        AIShooting();

    }

    void AIPathing()
    {
        Vector2 direction = player.transform.position - transform.position;
        direction.Normalize();
        Vector2 inputVector = Vector2.ClampMagnitude(direction, 1);
        movement = inputVector * speed;
        rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);

    }

    void AIShooting()
    {
        if(readyToShoot)
        {
            StartCoroutine(Shoot());
        }
    }




    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     GIZMOS
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

}

