using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyPathing : MonoBehaviour
{
    public enum pathingType { SIMPLE, SHADOW, SMART}
    [SerializeField] private pathingType pathing = pathingType.SIMPLE;

    public string playerTag = "player";
    Player player = null;

    public SpriteRenderer rend;

    public float speed = 3f;
    
    [Header("Shadow Pathing")]

    public TargetingManager targetingManager;

    enemyShooting _enemyShooting;

    Rigidbody2D rb;
    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        rend = gameObject.GetComponent<SpriteRenderer>();
        targetingManager = GameObject.FindGameObjectWithTag("targetManager").GetComponent<TargetingManager>();
        _enemyShooting = gameObject.GetComponent<enemyShooting>();
    }

    void Start()
    {
        player = targetingManager.GetTarget(transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            if (player.GetPlayerHealth().State != PlayerHealth.PlayerState.REVIVING)
            {
                if (pathing == pathingType.SIMPLE)
                {
                    walkTowardsPlayer();
                }
                else if (pathing == pathingType.SHADOW)
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
                        StartCoroutine(_enemyShooting.Emerging(player));
                    }

                }
                else if (pathing == pathingType.SMART)
                {

                }
                //inRangeOfPlayer();
            }
            else
            {
                player = targetingManager.GetTarget(transform);
            }

        }
    }

    void walkTowardsPlayer()
    {
        Vector2 direction = player.transform.position - transform.position;
        direction.Normalize();
        //transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
    }
}
