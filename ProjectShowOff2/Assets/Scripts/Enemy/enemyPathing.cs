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

    Rigidbody2D rb;

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     AWAKE
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void Awake()
    {
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
            }

        }
        else
        {
            player = targetingManager.GetTarget(transform);
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
    void walkTowardsPlayer()
    {
        Vector2 direction = player.transform.position - transform.position;
        direction.Normalize();
        //transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
    }
}
