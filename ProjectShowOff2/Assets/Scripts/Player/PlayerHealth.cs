using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     EVENTS
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public static event Action onBossDeath;

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     FIELDS
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    [SerializeField]
    private int health = 0;
    [SerializeField] private int maxHealth = 0;

    //public SpriteRenderer playerColour;
    public Color revivalColor;

    public enum PlayerState { ALIVE, REVIVING, INVINCIBLE, BOSS }
    [SerializeField] private PlayerState state = PlayerState.ALIVE;
    [SerializeField] private int revivingRange = 10;
    [SerializeField] private float reviveCooldown = 30f;
    [SerializeField] private float reviveTimer = 0;
    [SerializeField] private float reviveMultiplyer = 3f;
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private bool OtherPlayerIsClose = false;
    [SerializeField] private float invincibilityInSec = 1f;
    public ParticleSystem revivalState;
    public ParticleSystem resurrection;
    private ParticleSystem clonedRevivalState;



    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     GET() AND SET()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public int Health
    {
        set { health = value; }
        get { return health; }
    }

    public int MaxHealth
    {
        set
        {
            maxHealth = value;
        }
        get { return maxHealth; }
    }

    public PlayerState State
    {
        set { state = value; }
        get { return state; }
    }

    public float ReviveTimer
    {
        get { return reviveTimer; }
    }

    public float ReviveCooldown
    {
        get { return reviveCooldown; }
    }

    PlayerMovement playerMovement;

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     AWAKE()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void Awake()
    {
        health = maxHealth;
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     START()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void Start()
    {
        playerMovement = GetComponent<Player>().GetPlayerMovement();
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     UPDATE()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void Update()
    {
        StateCheck();
    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     TAKE DAMAGE()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void takeDamage(int damage)
    {
        if (state == PlayerState.ALIVE || state == PlayerState.BOSS)
        {
            Debug.Log("DAMAGEEEEEEE");
            health -= damage;
            if (health <= 0)
            {
                if (state != PlayerState.BOSS)
                {
                    manageRevivalState();
                    FindObjectOfType<SoundManager>().Play("playerDeath");
                    FindObjectOfType<SoundManager>().Play("playerDiesVO");
                    
                }
                else
                {
                    onBossDeath();
                }

            }
            else if (health == 20 && state != PlayerState.BOSS)
            {
                FindObjectOfType<SoundManager>().Play("playerLowHpVO");
            }
            else if (state != PlayerState.BOSS)
            {
                FindObjectOfType<SoundManager>().Play("playerDamageVoice");
            }
        }
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     INVINCIBLE
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    /////used for invincibility frames after reviving
    IEnumerator Invincible()
    {
        state = PlayerState.INVINCIBLE;
        
        manageRevivalState();
        yield return new WaitForSeconds(invincibilityInSec); 
        state = PlayerState.ALIVE;                              //im not invincible anymore
        FindObjectOfType<SoundManager>().Play("playerRevive");
        
        Instantiate(resurrection, transform.position, transform.rotation);
        
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     STATECHECK()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///checks all states and calls responsible functions
    public void StateCheck()
    {
        if (state == PlayerState.REVIVING)
        {
            Reviving();
        }
    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     MANAGE REVIVAL STATE()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///sets the revival process into motion/ ends the revival process
    void manageRevivalState()
    {
        if (state == PlayerState.ALIVE)
        {
            clonedRevivalState = Instantiate(revivalState, transform.position, transform.rotation);
            state = PlayerState.REVIVING;
            reviveTimer = reviveCooldown;
            transform.GetComponent<BoxCollider2D>().enabled = false;
           
            //playerColour.color = revivalColor;
            playerMovement.ResetMovement();
        }
        else
        {
            transform.GetComponent<BoxCollider2D>().enabled = true;
            health = maxHealth;
            Destroy(clonedRevivalState, 1.0f);
            //SetColour(health);
        }
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     REVIVE()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///triggers revival 
    public void Revive()
    {
        if(state == PlayerState.REVIVING) 
        {
            manageRevivalState();
        }
        
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     PLAYER PROXIMITY CHECKS()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void playerProximityCheck()
    {
        int playerCount = 0;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, revivingRange);
        foreach (Collider2D coll in colliders)
        {
            if (coll.gameObject.CompareTag("Player"))
            {
                playerCount++;
            }
        }
        if (playerCount >= 1)
        {
            OtherPlayerIsClose = true;
        }
        else
        {
            OtherPlayerIsClose = false;
        }
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     REVIVING()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///manages revive timer if other players are in range and starts invincibility if revive timer runs out
    void Reviving()
    {
        playerProximityCheck();
        if (reviveTimer > 0)
        {

            if (OtherPlayerIsClose)
            {

                reviveTimer -= Time.deltaTime * reviveMultiplyer;
            }
            else
            {

                reviveTimer -= Time.deltaTime;
            }

        }
        else
        {
            Debug.Log("PLAYER ALIVE AGAIN");
            StartCoroutine(Invincible());
        }
    }




}
