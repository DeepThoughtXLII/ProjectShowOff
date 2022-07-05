using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyPathing : MonoBehaviour
{
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     FIELDS
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public enum pathingType { SIMPLE, SHADOW, SMART, DISTANCE }
    [SerializeField] private pathingType pathing = pathingType.SIMPLE;


    public string playerTag = "player";
    Player player = null;

    public SpriteRenderer rend;

    public float speed = 3f;

    [Header("Shadow Pathing")]

    public TargetingManager targetingManager;

    enemyShooting _enemyShooting;

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
                pathingDistance();
                isMoving = true;
            }

        }
        else
        {
            player = targetingManager.GetTarget(transform);
            isMoving = false;

        }
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     PATHING SIMPLE
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void pathingSimple()
    {
        if (pathing == pathingType.SIMPLE)
        {
            if (_enemyShooting.charging == false)
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
                if (Vector2.Distance(player.transform.position, transform.position) < 0.5)
                {
                    _enemyShooting.emerging = true;
                    rend.color = Color.yellow;
                    StartCoroutine(_enemyShooting.Emerging());
                }

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

    void pathingDistance()
    {
        if (pathing == pathingType.DISTANCE)
        {
            walkWithDistance();
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
        //transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
        rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);
    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     STAY DISTANCE TOWARDS PLAYER
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    void walkWithDistance()
    {
        if (_enemyShooting.charging == false)
        {
            Vector2 direction = player.transform.position - transform.position;
            Vector2 attackMinus = new Vector2(_enemyShooting.range - 1, 0);
            attackMinus = rotateVector(attackMinus, angleBetweenVectors((Vector2)player.transform.position, (Vector2)gameObject.transform.position));
            //Console.WriteLine("Attack angle: " + angleBetweenVectors((Vector2)player.transform.position, (Vector2)gameObject.transform.position));
            direction += attackMinus;
            direction.Normalize();
            Vector2 inputVector = Vector2.ClampMagnitude(direction, 1);
            movement = inputVector * speed;
            //transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
            rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);
        }

    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     ROTATE VECTOR
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public Vector2 rotateVector(Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    public float angleBetweenVectors(Vector2 a, Vector2 b)
    {
        float x = b.x - a.x;
        float y = b.y - a.y;
        return Mathf.Atan2(y, x) * (180 / Mathf.PI);
    }
}
