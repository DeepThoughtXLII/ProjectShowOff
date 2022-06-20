using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    Player player;
    string playerName;

    Material p_Mat;

    [SerializeField] Image health;
    [SerializeField] TextMeshProUGUI healthText;
     float healthUnit;
    int currentMaxHealth;

    [SerializeField] Image xp;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI xpText;
    float xpUnit;

    [SerializeField] TextMeshProUGUI nameText;

    PlayerHealth.PlayerState oldState = PlayerHealth.PlayerState.ALIVE;

    ILevelable levelable;
    [SerializeField] private int displayedLevel;

    [SerializeField] public Image p_UI;

    [SerializeField] public Sprite nolvlup;
    [SerializeField] public Sprite yeslvlup;


    private void Start()
    {
        p_UI = this.GetComponent<Image>();
        p_Mat = this.GetComponent<Image>().material;
        health = transform.GetChild(1).transform.GetChild(0).GetComponent<Image>();
        healthText = transform.GetChild(1).transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        nameText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        xp = transform.GetChild(2).transform.GetChild(0).GetComponent<Image>();
        xpText = transform.GetChild(2).transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        levelText = transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        levelable = player.gameObject.GetComponent<ILevelable>();
        displayedLevel = levelable.Level.id;
        levelText.text = "" + displayedLevel;
        nameText.text = player.name;
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
        if(player.GetPlayerHealth().MaxHealth > currentMaxHealth)
        {
            p_Mat.color = Color.red;
            MaxHpRecalc();
        }
        int hp = player.GetPlayerHealth().Health;
        health.fillAmount = healthUnit * hp;
        healthText.text = "Health: " + hp;
    }

    public void MaxHpRecalc()
    {
        currentMaxHealth = player.GetPlayerHealth().MaxHealth;
        float maxHp = player.GetPlayerHealth().MaxHealth;
        healthUnit = 1f / maxHp;
    }

    public void UpdateName()
    {
        nameText.text = playerName;
    }

    public void SetReviveMode()
    {
        p_Mat.color = Color.green;
        healthUnit = 1 / player.GetPlayerHealth().ReviveCooldown;
        health.color = Color.green;
        UpdateReviveBar();
    }

    public void SetAliveMode()
    {
        MaxHpRecalc();
        p_Mat.color = Color.white;
        health.color = Color.white;
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
        healthText.text = "reviving in... " + Mathf.RoundToInt(player.GetPlayerHealth().ReviveTimer) + "secs";
        health.fillAmount = healthUnit * player.GetPlayerHealth().ReviveTimer;
    }

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

    private void levelUpdate()
    {
        if (levelable.Level.id > displayedLevel)
        {
            displayedLevel = levelable.Level.id;
            levelText.text = ""+displayedLevel;
            xpText.text = "XP: " + levelable.Xp;
            p_UI.sprite = yeslvlup;
        }else {
            p_UI.sprite = nolvlup;
        }
        xp.fillAmount = levelable.Xp/levelable.Level.xpNeeded;
    }
}
