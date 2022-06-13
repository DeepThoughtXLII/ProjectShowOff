using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyScript : MonoBehaviour, IDamageable, ITargetable
{


    [SerializeField]
    private int health = 0;

    public string damageSound;
    public enum pathingType { SIMPLE, SHADOW, SMART }
    [SerializeField] private pathingType pathing = pathingType.SIMPLE;

    SpriteRenderer rend;

    Color defColor;
    public Color targetColor = Color.white;

    public string playerTag = "player";
    Player player = null;

    public float speed = 3f;
    private CircleCollider2D collisionBox;
    [Header("Shadow Pathing")]
    public float EmergeSpeed;
    public int EmergeDamage;
    public float MeleeRange;
    public float TimeBeforeDissapear;

    bool isTarget = false;

    bool _emerging = false;

    public TargetingManager targetingManager;

    Rigidbody2D rb;


    //Temp sprite renderer colour change
    public SpriteRenderer spriteColour;

    public int Health
    {
        set { health = value; }
        get { return health; }
    }

    public pathingType State
    {
        set { pathing = value; }
        get { return pathing; }
    }

    public void takeDamage(int damage)
    {
        if (damageSound != null && damageSound != "")
        {
            FindObjectOfType<SoundManager>().Play(damageSound);
        }
      
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
            FindObjectOfType<SoundManager>().Play("enemyDeath");
            return;
        }
        player = targetingManager.GetTarget(transform);
    }

    void OnDestroy()
    {
        waveSpawner.EnemiesAlive--;
        //Console.WriteLine("" + waveSpawner.EnemiesAlive);
    }


    public void becomeTarget()
    {
        if (isTarget == false)
        {
            rend.color = targetColor;
            isTarget = true;
        }

    }

    public void loseTarget()
    {
        rend.color = defColor;
        isTarget = false;
    }

    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
        defColor = rend.color;
        //player = GameObject.FindGameObjectWithTag(playerTag).transform;
        speed = Random.Range(0.3f, speed);
        rb = GetComponent<Rigidbody2D>();
        targetingManager = GameObject.FindGameObjectWithTag("targetManager").GetComponent<TargetingManager>();
        collisionBox = gameObject.GetComponent<CircleCollider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        spriteColour = rend;
        player = targetingManager.GetTarget(transform);
        spriteColour.color = Color.gray;
        if (pathing == pathingType.SHADOW)
        {
            collisionBox.enabled = false;
        }
    }

    void FixedUpdate()
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
                            spriteColour.color = Color.yellow;
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

    // Update is called once per frame
    void Update()
    {

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
        spriteColour.color = Color.gray;

    }

    void walkTowardsPlayer()
    {
        Vector2 direction = player.transform.position - transform.position;
        //transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
    }

}
