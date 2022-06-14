using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour, IDamageable
{

    public static event Action onBossDeath;


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


    void Awake()
    {
        health = maxHealth;
    }

    void Start()
    {
        playerMovement = GetComponent<Player>().GetPlayerMovement();
    }

    void Update()
    {
        StateCheck();
    }


    public void takeDamage(int damage)
    {
        if (state == PlayerState.ALIVE || state == PlayerState.BOSS)
        {
            Debug.Log("DAMAGEEEEEEE");
            health -= damage;
            FindObjectOfType<SoundManager>().Play("playerDamageVoice");

            if (health <= 0)
            {
                if (state != PlayerState.BOSS)
                {
                    manageRevivalState();
                    FindObjectOfType<SoundManager>().Play("playerDeath");
                }
                else
                {
                    onBossDeath();
                }

            }
        }
    }



    IEnumerator Invincible()
    {
        state = PlayerState.INVINCIBLE;
        manageRevivalState();
        yield return new WaitForSeconds(invincibilityInSec);
        state = PlayerState.ALIVE;
        FindObjectOfType<SoundManager>().Play("playerRevive");
    }



    public void StateCheck()
    {
        if (state == PlayerState.REVIVING)
        {
            Reviving();
        }
    }

    void manageRevivalState()
    {
        if (state == PlayerState.ALIVE)
        {
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
            //SetColour(health);
        }
    }



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
