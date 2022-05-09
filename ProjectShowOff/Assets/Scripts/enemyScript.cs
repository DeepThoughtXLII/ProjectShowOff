using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyScript : MonoBehaviour, IDamageable, ITargetable
{


    [SerializeField]
    private int health = 0;

    SpriteRenderer rend;

    Color defColor;
    Color targetColor = Color.white;

    public string playerTag = "player";
    Transform player = null;

    public float speed = 3f;

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
            return;
        }
    }

    public void becomeTarget()
    {
        rend.color = targetColor;
    }

    public void loseTarget()
    {
        rend.color = defColor;
    }

    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
        defColor = rend.color;
        player = GameObject.FindGameObjectWithTag(playerTag).transform;
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(player != null)
        {
            walkTowardsPlayer();
        }
    }


    void walkTowardsPlayer()
    {
        Vector3 direction = player.position - transform.position;
        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
    }
}
