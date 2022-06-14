using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class Player : MonoBehaviour  //, IDamageable
{
    private int id;
    public int Id
    {
        set { id = value; }
        get { return id; }
    }

    [SerializeField] playerShooting ShootingScript;
    [SerializeField] PlayerMovement MovementScript;
    [SerializeField] PlayerHealth PlayerHealthScript;


    private void OnEnable()
    {
        ShootingScript = GetComponent<playerShooting>();
        MovementScript = GetComponent<PlayerMovement>();
        PlayerHealthScript = GetComponent<PlayerHealth>();
    }

    private void Awake()
    {
        ShootingScript = GetComponent<playerShooting>();
        MovementScript = GetComponent<PlayerMovement>();
        PlayerHealthScript = GetComponent<PlayerHealth>();
    }

    private void Start()
    {
        
    }

    public playerShooting GetPlayerShooting()
    {
        return ShootingScript;
    }

    public PlayerMovement GetPlayerMovement()
    {
        return MovementScript;
    }

    public PlayerHealth GetPlayerHealth()
    {
        return PlayerHealthScript;
    }

    public void Disconnected()
    {
        PlayerHealthScript.State = PlayerHealth.PlayerState.INVINCIBLE;
    }

    public void Reconnected()
    {
        PlayerHealthScript.State = PlayerHealth.PlayerState.ALIVE;
    }


    public void Remove()
    {
        Destroy(this);
    }


    public void bossMode()
    {
        Debug.Log("boosmOde");
        gameObject.tag = "enemy";
        PlayerHealthScript.State = PlayerHealth.PlayerState.BOSS;
        transform.localScale *= 2;
        PlayerHealthScript.MaxHealth *= 2;
        PlayerHealthScript.Health = PlayerHealthScript.MaxHealth;
        MovementScript.speed *= 1.5f;
    }





    /*
    public static event Action onBossDeath;


    [SerializeField]
    private int health = 0;

    private int id;

    PlayerControls controls;

    Vector2 spawn;


    Vector2 move;

    public PlayerInput pi;

    public float speed = 5f;

    public SpriteRenderer playerColour;
    public Gradient playerGradient;
    public Color revivalColor;


    [SerializeField] private int maxHealth = 0;

    public enum Input { KEYBOARD, GAMEPAD, ONLINE }
    public Input isUsingInput = Input.KEYBOARD;

    public bool isMoving = false;
    public Vector2 direction;
    private Vector2 lastDir;


    public enum PlayerState { ALIVE, REVIVING, INVINCIBLE, BOSS}
    [SerializeField] private PlayerState state = PlayerState.ALIVE;
    [SerializeField] private int revivingRange = 10;
    [SerializeField] private float reviveCooldown = 30f;
    [SerializeField] private float reviveTimer = 0;
    [SerializeField] private float reviveMultiplyer = 3f;
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private bool OtherPlayerIsClose = false;
    [SerializeField] private float invincibilityInSec = 1f;

    private Rigidbody2D rb;



    public Input IsUsingInput
    {
        set { isUsingInput = value; }
        get { return isUsingInput; }
    }

    public int Id
    {
        set { id = value; }
        get { return id; }
    }

    public int Health
    {
        set { health = value; }
        get { return health; }
    }

    public int MaxHealth
    {
        set {
            maxHealth = value; 
        }
        get { return maxHealth; }
    }

    public PlayerState State
    {
        set { state = value; }
        get { return state; }
    }

    public float ReviveTimer
    {
        get { return reviveTimer; }
    }

    public float ReviveCooldown
    {
        get { return reviveCooldown; }
    }

    public Vector2 Spawn
    {
        set { spawn = value; }
        get { return spawn; }
    }




    private void Awake()
    {



        //controls = new PlayerControls();

        //myActionMap.devices.
        //controls.Gameplay.move.;
        //controls.Gameplay.move.performed += ctx => move = ctx.ReadValue<Vector2>();
        //controls.Gameplay.move.canceled += ctx => move = Vector2.zero;

        //InputSystem.GetDeviceById(id)


    }

    void Start()
    {
        if (isUsingInput != Input.ONLINE)
        {
            pi = GetComponentInChildren<PlayerInput>();
            if (pi != null)
            {
                Debug.Log("subscribed");
                if (isUsingInput == Input.GAMEPAD)
                {
                    pi.onActionTriggered += OnAction2;
                }
                else
                {
                    pi.onActionTriggered += OnAction2;

                }
            }
        }


        rb = GetComponent<Rigidbody2D>();

        maxHealth = health;

    }


    public void takeDamage(int damage)
    {
        if (state == PlayerState.ALIVE || state == PlayerState.BOSS)
        {
            Debug.Log("DAMAGEEEEEEE");
            health -= damage;

            if (health <= 0)
            {
                if (state != PlayerState.BOSS)
                {
                    manageRevivalState();
                    FindObjectOfType<SoundManager>().Play("playerDeath");
                }else
                {
                    onBossDeath();
                }
                
            } 
        }
    }



    IEnumerator Invincible()
    {
        state = PlayerState.INVINCIBLE;
        manageRevivalState();
        yield return new WaitForSeconds(invincibilityInSec);
        state = PlayerState.ALIVE;
        FindObjectOfType<SoundManager>().Play("playerRevive");
    }

    public void StateCheck()
    {
        if (state == PlayerState.REVIVING)
        {
            Reviving();
        }
    }

    void manageRevivalState()
    {
        if (state == PlayerState.ALIVE)
        {
            state = PlayerState.REVIVING;
            reviveTimer = reviveCooldown;
            transform.GetComponent<BoxCollider2D>().enabled = false;
            move = Vector2.zero;
            playerColour.color = revivalColor;
            if (isUsingInput == Input.KEYBOARD)
            {
                direction = Vector2.zero;
            }
        }
        else
        {
            transform.GetComponent<BoxCollider2D>().enabled = true;
            health = maxHealth;
            SetColour(health);
        }
    }



    void playerProximityCheck()
    {
        int playerCount = 0;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, revivingRange);
        foreach (Collider2D coll in colliders)
        {
            if (coll.gameObject.CompareTag("Player"))
            {
                playerCount++;
            }
        }
        if (playerCount >= 1)
        {
            OtherPlayerIsClose = true;
        }
        else
        {
            OtherPlayerIsClose = false;
        }
    }

    void Reviving()
    {
        playerProximityCheck();
        if (reviveTimer > 0)
        {

            if (OtherPlayerIsClose)
            {

                reviveTimer -= Time.deltaTime * reviveMultiplyer;
            }
            else
            {

                reviveTimer -= Time.deltaTime;
            }

        }
        else
        {
            Debug.Log("PLAYER ALIVE AGAIN");
            StartCoroutine(Invincible());
        }
    }



    public void Disconnected()
    {
        state = PlayerState.INVINCIBLE;
    }

    public void Reconnected()
    {
        state = PlayerState.ALIVE;
    }

    private void OnAction(InputAction.CallbackContext ctx)
    {
        if (state != PlayerState.REVIVING)
        {
            if (ctx.action.name == "move")
            {
                if (ctx.action.phase == InputActionPhase.Performed)
                {
                    direction = ctx.ReadValue<Vector2>();
                }
                else if (ctx.action.phase == InputActionPhase.Canceled)
                {
                    //move = Vector2.zero;
                }
            }
        }
        //Debug.Log(ctx);
    }

    private void OnAction2(InputAction.CallbackContext ctx)
    {

        if (state != PlayerState.REVIVING)
        {
            if (ctx.action.name == "move")
            {
                if (ctx.action.phase == InputActionPhase.Performed)
                {
                    if (direction != ctx.ReadValue<Vector2>())
                    {
                        direction = ctx.ReadValue<Vector2>();
                    }
                    isMoving = true;
                }
                else if (ctx.action.phase == InputActionPhase.Canceled)
                {
                    isMoving = false;
                    direction = Vector2.zero;
                }
                //  Move(ctx.ReadValue<Vector2>());
            }
        }
        //Debug.Log(ctx);
    }


    void FixedUpdate()
    {
        if (state != PlayerState.REVIVING)
        {
            Move(direction);
            rb.MovePosition(move);
        }
    }

    void Update()
    {
        StateCheck();
    }

    void SetColour(int heat)
    {
        playerColour.color = playerGradient.Evaluate((float)heat / maxHealth);
    }


    public void Remove()
    {
        Destroy(this);
    }

    public void Move(Vector2 moveDirection)
    {
        if (state != PlayerState.REVIVING)
        {
            lastDir = direction;
            direction -= lastDir;
            direction += (moveDirection);
            direction.Normalize();
        }

        if (direction != Vector2.zero)
        {
            FindObjectOfType<SoundManager>().Play("playerWalk");

        } else
        {
            FindObjectOfType<SoundManager>().Play("playerWalk");
        }

        move = rb.position + direction * speed * Time.fixedDeltaTime;
        SetColour(health);
    }

    public void Move2(InputAction.CallbackContext context)
    {
        Vector2 move = Vector2.zero;
        if (context.performed)
        {
            move = context.ReadValue<Vector2>() * speed * Time.fixedDeltaTime;
        }
        transform.Translate(move, Space.World);
        SetColour(health);
    }

    public void bossMode()
    {
        Debug.Log("boosmOde");
        gameObject.tag = "enemy";
        state = PlayerState.BOSS;
        transform.localScale *= 2;
        maxHealth *= 2;
        health = maxHealth;
        speed *= 1.5f;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, revivingRange);
    }

    */
}