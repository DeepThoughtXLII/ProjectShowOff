using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyPathing : MonoBehaviour
{
    public enum pathingType { SIMPLE, SHADOW, SMART}
    [SerializeField] private pathingType pathing = pathingType.SIMPLE;

    public string playerTag = "player";
    Player player = null;

    SpriteRenderer rend;

    public float speed = 3f;
    private CircleCollider2D collisionBox;
    [Header("Shadow Pathing")]
    public float EmergeSpeed;
    public int EmergeDamage;
    public float MeleeRange;
    public float TimeBeforeDissapear;
    

    bool _emerging = false;

    public TargetingManager targetingManager;

    Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();
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
            if (player.State != Player.PlayerState.REVIVING)
            {
                if (pathing == pathingType.SIMPLE)
                {
                    walkTowardsPlayer();
                }
                else if (pathing == pathingType.SHADOW)
                {
                    if (_emerging == false)
                    {
                        walkTowardsPlayer();
                        if (Vector2.Distance(player.transform.position, transform.position) < 0.1)
                        {
                            _emerging = true;
                            rend.color = Color.yellow;
                        }

                    }
                    else if (_emerging == true)
                    {
                        StartCoroutine(Emerging());
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

    public void AttackPlayer()
    {
        if (Vector2.Distance(player.transform.position, transform.position) < MeleeRange)
        {
            IDamageable playerDam = player.GetComponent<IDamageable>();
            playerDam.takeDamage(EmergeDamage);
        }
    }

    public IEnumerator Emerging()
    {
        yield return new WaitForSeconds(EmergeSpeed);
        AttackPlayer();
        collisionBox.enabled = true;
        yield return new WaitForSeconds(TimeBeforeDissapear);
        collisionBox.enabled = false;
        _emerging = false;
        rend.color = Color.gray;

    }

    void walkTowardsPlayer()
    {
        Vector2 direction = player.transform.position - transform.position;
        //transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
    }
}
