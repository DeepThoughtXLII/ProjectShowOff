using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, IDamageable
{

    [SerializeField]
    private int health = 0;

    private int id;

    PlayerControls controls;
    

    Vector2 move;

    public PlayerInput pi;

    public float speed = 5f;

    public SpriteRenderer playerColour;
    public Gradient playerGradient;
    public Color revivalColor;

    private int maxHealth = 0;

    public bool isUsingController = false;

    public bool testingwithKeyboard = true;

    public enum PlayerState { ALIVE, REVIVING, INVINCIBLE}
    [SerializeField] private PlayerState state = PlayerState.ALIVE;
    [SerializeField] private int revivingRange = 10;
    [SerializeField] private float reviveCooldown = 30f;
    [SerializeField] private float reviveTimer = 0;
    [SerializeField] private float reviveMultiplyer = 3f;
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private bool OtherPlayerIsClose = false;
    [SerializeField] private float invincibilityInSec = 1f;
    


    public bool IsUsingController
    {
        set { isUsingController = value; }
        get { return isUsingController; }
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
        set { maxHealth = value; }
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

    public void takeDamage(int damage)
    {
        if (state == PlayerState.ALIVE)
        {
            health -= damage;
            if(health <= 0)
            {
                manageRevivalState();
            }
        }
    }

    IEnumerator Invincible()
    {
        state = PlayerState.INVINCIBLE;
        manageRevivalState();
        yield return new WaitForSeconds(invincibilityInSec);
        state = PlayerState.ALIVE;
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
        foreach(Collider2D coll in colliders)
        {
            if (coll.gameObject.CompareTag("Player"))
            {
                playerCount++;
            }
        }
        if(playerCount >= 1)
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
        if(reviveTimer > 0)
        {
            
            if (OtherPlayerIsClose)
            {
                
                reviveTimer -= Time.deltaTime * reviveMultiplyer;
            }
            else
            {
                
                reviveTimer -= Time.deltaTime;
            }
            
        } else
        {
            Debug.Log("PLAYER ALIVE AGAIN");
            StartCoroutine(Invincible());
        }
    }


    private void Awake()
    {



        //controls = new PlayerControls();

        //myActionMap.devices.
        //controls.Gameplay.move.;
        //controls.Gameplay.move.performed += ctx => move = ctx.ReadValue<Vector2>();
        //controls.Gameplay.move.canceled += ctx => move = Vector2.zero;

        //InputSystem.GetDeviceById(id)

        maxHealth = Health;
    }

    void Start()
    {
        pi = GetComponentInChildren<PlayerInput>();
        if (pi != null)
        {
            Debug.Log("subscribed");
            pi.onActionTriggered += OnAction;
        }

    }

    private void test(InputAction.CallbackContext ctx)
    {
        Debug.Log("Action");
    }

    

    private void OnAction(InputAction.CallbackContext ctx)
    {
        Debug.Log("Action");
        if (state != PlayerState.REVIVING)
        {
            if (ctx.action.name == "move")
            {

                if (ctx.action.phase == InputActionPhase.Performed)
                {
                    move = ctx.ReadValue<Vector2>();
                }
                else if (ctx.action.phase == InputActionPhase.Canceled)
                {
                    move = Vector2.zero;
                }

            }
        }
        //Debug.Log(ctx);
    }

    void FixedUpdate()
    {
        if (!isUsingController)
        {
            Move(move);
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

    private void OnEnable()
    {
        
        //controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        //controls.Gameplay.Disable();
    }


    public void Remove()
    {
        Destroy(this);
    }

    public void Move(Vector2 direction)
    {
        Vector2 m = new Vector2(direction.x, direction.y) * speed * Time.deltaTime;
        transform.Translate(m, Space.World);
        SetColour(health);
    }

    public void Move2(InputAction.CallbackContext context)
    {
        Vector2 move = Vector2.zero;
        if (context.performed)
        {
            move = context.ReadValue<Vector2>() * speed * Time.deltaTime;

        }
        transform.Translate(move, Space.World);
        SetColour(health);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, revivingRange);
    }
}