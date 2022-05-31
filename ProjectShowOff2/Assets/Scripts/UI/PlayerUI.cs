using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    Player player;
    string playerName;


    [SerializeField] Image health;
    [SerializeField] TextMeshProUGUI healthText;
     float healthUnit;
    [SerializeField] TextMeshProUGUI levelText;

    [SerializeField] TextMeshProUGUI nameText;

    Player.PlayerState oldState = Player.PlayerState.ALIVE;

    ILevelable levelable;
    [SerializeField] private int displayedLevel;

    private void Start()
    {
        health = transform.GetChild(1).transform.GetChild(0).GetComponent<Image>();
        healthText = transform.GetChild(1).transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        nameText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        levelText = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        levelable = player.gameObject.GetComponent<ILevelable>();
        displayedLevel = levelable.Level.id + 1;
        levelText.text = "level: " + displayedLevel;
        MaxHpRecalc();
    }


    public Player Player
    {
        set { player = value; }
        get { return player; }
    }

    public string PlayerName
    {
        set { playerName = value; }
        get { return playerName; }
    }

    public void UpdateHealthBar()
    {
        int hp = player.Health;      
        health.fillAmount = healthUnit * hp;
        healthText.text = "Health: " + hp;
    }

    public void MaxHpRecalc()
    {
        float maxHp = player.MaxHealth;
        healthUnit = 1f / maxHp;
    }

    public void UpdateName()
    {
        nameText.text = playerName;
    }

    public void SetReviveMode()
    {
        healthUnit = 1 / player.ReviveCooldown;
        health.color = Color.green;
        UpdateReviveBar();
    }

    public void SetAliveMode()
    {
        MaxHpRecalc();
        health.color = Color.red;
        UpdateHealthBar();
    }

    public void SetInvincibleMode()
    {
        health.fillAmount = 1f;
        health.color = Color.white;
        healthText.text = "INVINCIBLE";
        healthText.color = Color.black;
    }

    public void UpdateReviveBar()
    {
        healthText.text = "reviving in... " + Mathf.RoundToInt(player.ReviveTimer) + "secs";
        health.fillAmount = healthUnit * player.ReviveTimer;
    }

    public void StateChange()
    {
        if(oldState != player.State)
        {
            oldState = player.State;
            healthText.color = Color.white;
            if (player.State == Player.PlayerState.ALIVE)
            {
                SetAliveMode();
            }else if(player.State == Player.PlayerState.INVINCIBLE)
            {
                SetInvincibleMode();
            }else if(player.State == Player.PlayerState.REVIVING)
            {
                SetReviveMode();
            }
        }
    }

    private void Update()
    {
        StateChange();
        if(player.State == Player.PlayerState.ALIVE)
        {
            UpdateHealthBar();
        }
        else if(player.State == Player.PlayerState.REVIVING)
        {
            UpdateReviveBar();
        }
        levelUpdate();
    }

    private void levelUpdate()
    {
        if (levelable.Level.id+1 > displayedLevel)
        {
            displayedLevel = levelable.Level.id + 1;
            levelText.text = "level: " + displayedLevel;
        }
    }


}
