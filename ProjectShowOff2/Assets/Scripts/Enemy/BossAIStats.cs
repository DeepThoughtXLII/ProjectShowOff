using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAIStats : MonoBehaviour, IDamageable
{
    [SerializeField]
    private int health = 0;

    public void takeDamage(int damage)
    { 
        health -= damage;
        if (health <= 0)
        {
            FindObjectOfType<SoundManager>().Play("enemyDeath");
            Destroy(gameObject);
            return;
        }
    }

    public int Health
    {
        set { health = value; }
        get { return health; }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
