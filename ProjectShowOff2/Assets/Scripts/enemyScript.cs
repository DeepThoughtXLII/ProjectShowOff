using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyScript : MonoBehaviour, IDamageable, ITargetable
{


    [SerializeField]
    private int health = 0;

    public float attackSpeed = 0;
    public int damage = 0;
    public float attackRange = 0;

    private float passedTime = 0;

    SpriteRenderer rend;

    Color defColor;
    public Color targetColor = Color.white;

    public string playerTag = "player";
    Transform player = null;

    public float speed = 3f;

    bool isTarget = false;

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
            waveSpawner.EnemiesAlive--;
            Console.WriteLine("" + waveSpawner.EnemiesAlive);
            Destroy(gameObject);
            return;
        }
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
        player = GameObject.FindGameObjectWithTag(playerTag).transform;
        speed = Random.Range(speed - 1.2f, speed);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        if (player != null)
        {
            walkTowardsPlayer();
            inRangeOfPlayer();
        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    void inRangeOfPlayer()
    {
        Vector2 dir = player.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if(dir.magnitude <= attackRange)
        {
            if(passedTime <= 0)
            {
                AttackPlayer();
                passedTime = attackSpeed;
            }
        }

        passedTime -= Time.deltaTime;
    }

    public void AttackPlayer()
    {
        IDamageable playerDam = player.GetComponent<IDamageable>();
        playerDam.takeDamage(damage);
    }

    void walkTowardsPlayer()
    {
        Vector3 direction = player.position - transform.position;
        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
