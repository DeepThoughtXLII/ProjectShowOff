using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     FIELDS
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    Player player;
    string playerName;


    [SerializeField] Image health;
    [SerializeField] TextMeshProUGUI healthText;
     float healthUnit;
    int currentMaxHealth;

    [SerializeField] Image xp_img;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI xpText;
    float xpUnit;

    [SerializeField] TextMeshProUGUI nameText;

    PlayerHealth.PlayerState oldState = PlayerHealth.PlayerState.ALIVE;

    ILevelable levelable;
    [SerializeField] private int displayedLevel;


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     START()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void Start()
    {

        health = transform.GetChild(1).transform.GetChild(0).GetComponent<Image>();
        healthText = transform.GetChild(1).transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        nameText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        xp_img = transform.GetChild(2).transform.GetChild(0).GetComponent<Image>();
        xpText = transform.GetChild(2).transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        levelText = transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        levelable = player.gameObject.GetComponent<ILevelable>();
        displayedLevel = levelable.Level.id;
        levelText.text = "" + displayedLevel;
        nameText.text = player.name;
        MaxHpRecalc();
    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     GET() AND SET()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
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


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     UPDATE HEALTH BAR()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void UpdateHealthBar()
    {
        if(player.GetPlayerHealth().MaxHealth > currentMaxHealth)
        {
            MaxHpRecalc();
        }
        int hp = player.GetPlayerHealth().Health;
        health.fillAmount = healthUnit * hp;
        healthText.text = "Health: " + hp;
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     MAX HP RECALC()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void MaxHpRecalc()
    {
        currentMaxHealth = player.GetPlayerHealth().MaxHealth;
        float maxHp = player.GetPlayerHealth().MaxHealth;
        healthUnit = 1f / maxHp;
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     UPDATE NAME()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void UpdateName()
    {
        nameText.text = playerName;
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     SET REVIVE MODE()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void SetReviveMode()
    {
        healthUnit = 1 / player.GetPlayerHealth().ReviveCooldown;
        health.color = Color.green;
        UpdateReviveBar();
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     SET ALIVE MODE()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void SetAliveMode()
    {
        MaxHpRecalc();
        health.color = Color.red;
        UpdateHealthBar();
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     SET INVINCIBLE MODE()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void SetInvincibleMode()
    {
        health.fillAmount = 1f;
        health.color = Color.white;
        healthText.text = "INVINCIBLE";
        healthText.color = Color.black;
    }



    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     UPDAT ERVEIVE BAR()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void UpdateReviveBar()
    {
        healthText.text = "reviving in... " + Mathf.RoundToInt(player.GetPlayerHealth().ReviveTimer) + "secs";
        health.fillAmount = healthUnit * player.GetPlayerHealth().ReviveTimer;
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     STATE CHANGE()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void StateChange()
    {
        if(oldState != player.GetPlayerHealth().State)
        {
            oldState = player.GetPlayerHealth().State;
            healthText.color = Color.white;
            if (player.GetPlayerHealth().State == PlayerHealth.PlayerState.ALIVE)
            {
                SetAliveMode();
            }else if(player.GetPlayerHealth().State == PlayerHealth.PlayerState.INVINCIBLE)
            {
                SetInvincibleMode();
            }else if(player.GetPlayerHealth().State == PlayerHealth.PlayerState.REVIVING)
            {
                SetReviveMode();
            }
        }
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     UODATE()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void Update()
    {
        StateChange();
        if(player.GetPlayerHealth().State == PlayerHealth.PlayerState.ALIVE)
        {
            UpdateHealthBar();
        }
        else if(player.GetPlayerHealth().State == PlayerHealth.PlayerState.REVIVING)
        {
            UpdateReviveBar();
        }
        levelUpdate();
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     LEVEL UPDATE()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void levelUpdate()
    {
        if (levelable.Level.id > displayedLevel)
        {
            displayedLevel = levelable.Level.id;
            levelText.text = ""+displayedLevel;
            xpText.text = "XP: " + levelable.Xp;
            xp_img.fillAmount = levelable.Xp / levelable.Level.xpNeeded;

        }
    }
}
