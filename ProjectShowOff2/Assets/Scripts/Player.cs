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

    public enum PlayerState { ALIVE, DEAD, REVIVING }
    [SerializeField] private PlayerState state = PlayerState.ALIVE;
    [SerializeField] private int revivingRange = 10;
    [SerializeField] private float reviveCooldown = 30f;
    [SerializeField] private float reviveTimer = 0;

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
            reviveTimer = Time.time + reviveCooldown;
            transform.GetComponent<BoxCollider2D>().enabled = false;
            transform.GetComponent<playerShooting>().enabled = false;
            playerColour.color = revivalColor;
        }
        else
        {
            transform.GetComponent<BoxCollider2D>().enabled = true;
            transform.GetComponent<playerShooting>().enabled = true;
            health = maxHealth;
            SetColour(health);
        }
    }

    void Reviving()
    {

        if(Time.time > reviveTimer)
        {
            reviveTimer -= Time.deltaTime;
        } else
        {
            state = PlayerState.ALIVE;
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

    private void OnAction(InputAction.CallbackContext ctx)
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

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, revivingRange);
    }
}