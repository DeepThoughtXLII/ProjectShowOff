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

    public float speed = 5f;

    public SpriteRenderer playerColour;
    public Gradient playerGradient;

    private int maxHealth = 0;

    bool isUsingController = false;

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
        health -= damage;
    }

 

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Gameplay.move.performed += ctx => move = ctx.ReadValue<Vector2>();
        controls.Gameplay.move.canceled += ctx => move = Vector2.zero;

        maxHealth = Health;
    }

    void Update()
    {
        if (isUsingController)
        {
            Move(move);
        }
    }

    void SetColour(int heat)
    {
        playerColour.color = playerGradient.Evaluate((float)heat / maxHealth);
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
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
}
