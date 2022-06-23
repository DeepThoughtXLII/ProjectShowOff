using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyScript : MonoBehaviour, IDamageable, ITargetable
{
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     FIELDS
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    [SerializeField]
    private int health = 0;

    public string damageSound;
    SpriteRenderer rend;

    Color defColor;
    public Color targetColor = Color.white;
    public ParticleSystem enemyDeath;

    bool isTarget = false;

    public TargetingManager targetingManager;


    //Temp sprite renderer colour change
    public SpriteRenderer spriteColour;

    public int Health
    {
        set { health = value; }
        get { return health; }
    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                    TAKE DAMAGE
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
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
            Instantiate(enemyDeath, transform.position, transform.rotation);
            Destroy(enemyDeath);
            Destroy(gameObject);
            return;
        }
    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     ON DESTROY
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void OnDestroy()
    {
        waveSpawner.EnemiesAlive--;
        //Console.WriteLine("" + waveSpawner.EnemiesAlive);
    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     BECOME TARGET
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void becomeTarget()
    {
        if (isTarget == false)
        {
            rend.color = targetColor;
            isTarget = true;
        }

    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     LOSE TARGET
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void loseTarget()
    {
        rend.color = defColor;
        isTarget = false;
    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     AWAKE
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
        defColor = rend.color;
    }
}
