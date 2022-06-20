using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyPathing : MonoBehaviour
{
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     FIELDS
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public enum pathingType { SIMPLE, SHADOW, SMART }
    [SerializeField] private pathingType pathing = pathingType.SIMPLE;


    public string playerTag = "player";
    Player player = null;

    public SpriteRenderer rend;

    public float speed = 3f;

    [Header("Shadow Pathing")]

    public TargetingManager targetingManager;

    enemyShooting _enemyShooting;

    IsometricCharacterRenderer isoRenderer;

    Rigidbody2D rb;

    public bool isMoving;



    public Vector2 movement;

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     AWAKE
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void Awake()
    {
        isMoving = false;
        rb = gameObject.GetComponent<Rigidbody2D>();
        rend = gameObject.GetComponent<SpriteRenderer>();
        targetingManager = GameObject.FindGameObjectWithTag("targetManager").GetComponent<TargetingManager>();
        _enemyShooting = gameObject.GetComponent<enemyShooting>();
        isoRenderer = GetComponent<IsometricCharacterRenderer>();
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     UPDATE
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void Update()
    {
        player = targetingManager.GetTarget(transform);
    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     FIXEDUPDATE
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void FixedUpdate()
    {
        if (player != null)
        {
            if (player.GetPlayerHealth().State != PlayerHealth.PlayerState.REVIVING)
            {
                pathingSimple();
                pathingShadow();
                pathingSmart();
                isMoving = true;

            }

        }
        else
        {
            player = targetingManager.GetTarget(transform);
            isMoving = false;

        }

    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     PATHING SIMPLE
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void pathingSimple()
    {
        if (pathing == pathingType.SIMPLE)
        {
            if(_enemyShooting.charging == false)
            {
                walkTowardsPlayer();
            }
        }

    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     PATHING SHADOW
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void pathingShadow()
    {
        if (pathing == pathingType.SHADOW)
        {
            if (_enemyShooting.emerging == false)
            {
                walkTowardsPlayer();
                if (Vector2.Distance(player.transform.position, transform.position) < 0.1)
                {
                    _enemyShooting.emerging = true;
                    rend.color = Color.yellow;
                }

            }
            else if (_enemyShooting.emerging == true)
            {
                StartCoroutine(_enemyShooting.Emerging());
            }
        }


    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     PATHING SMART (WIP)
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void pathingSmart()
    {
        if (pathing == pathingType.SMART)
        {
            // INSERT PATHFINDING
        }
    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     WALK TOWARDS PLAYER
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void walkTowardsPlayer()
    {
        Vector2 direction = player.transform.position - transform.position;
        direction.Normalize();
        Vector2 inputVector = Vector2.ClampMagnitude(direction, 1);
        movement = inputVector * speed;
        //isoRenderer.SetDirection(movement);
        //transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
        rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);
    }
}
