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


    bool isTarget = false;

    public TargetingManager targetingManager;

    //Temp sprite renderer colour change
    public SpriteRenderer spriteColour;

    public int Health
    {
        set { health = value; }
        get { return health; }
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
            FindObjectOfType<SoundManager>().Play("enemyDeath");
            Destroy(gameObject);
            return;
        }
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
    }
}
