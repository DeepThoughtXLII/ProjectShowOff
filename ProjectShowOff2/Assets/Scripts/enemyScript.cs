using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyScript : MonoBehaviour, IDamageable, ITargetable
{


    [SerializeField]
    private int health = 0;

    SpriteRenderer rend;

    Color defColor;
    public Color targetColor = Color.white;

    public string playerTag = "player";
    Player player = null;

    public float speed = 3f;

    bool isTarget = false;

    public TargetingManager targetingManager;

    Rigidbody2D rb;

    public int Health
    {
        set { health = value; }
        get { return health; }
    }

    public void takeDamage(int damage)
    {
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
    }

    // Start is called before the first frame update
    void Start()
    {
        player = targetingManager.GetTarget(transform);
    }

    void FixedUpdate()
    {
        if (player != null)
        {
            if(player.State != Player.PlayerState.REVIVING)
            {
                walkTowardsPlayer();
                //inRangeOfPlayer();
            } else
            {
                player = targetingManager.GetTarget(transform);
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    //public void AttackPlayer()
    //{
    //    IDamageable playerDam = player.GetComponent<IDamageable>();
    //    playerDam.takeDamage(damage);
    //}

    void walkTowardsPlayer()
    {
        Vector2 direction = player.transform.position - transform.position;
        //transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
    }

}
