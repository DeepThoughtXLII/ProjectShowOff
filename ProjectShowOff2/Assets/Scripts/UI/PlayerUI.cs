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
    [SerializeField] private int oldXp;
    [SerializeField] private int displayedXpBarMax;
    [SerializeField] private int displayedXpBarMin;
    [SerializeField] private int xpBarMax;
    [SerializeField] private int xpBarMin;
    [SerializeField] private int displayedXp;


    [SerializeField] public Image p_UI;

    [SerializeField] public Sprite nolvlup;
    [SerializeField] public Sprite yeslvlup;

    public GameObject particles;
    public ParticleSystem lvlEffect;


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
        xp.fillAmount = 0;
        reassignXpValues();
        p_Mat.color = Color.white;
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
        if (player.GetPlayerHealth().MaxHealth > currentMaxHealth)
        {
            p_Mat.color = Color.white;
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
        p_Mat.color = Color.grey;
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
        if (oldState != player.GetPlayerHealth().State)
        {
            oldState = player.GetPlayerHealth().State;
            healthText.color = Color.white;
            if (player.GetPlayerHealth().State == PlayerHealth.PlayerState.ALIVE)
            {
                SetAliveMode();
            }
            else if (player.GetPlayerHealth().State == PlayerHealth.PlayerState.INVINCIBLE)
            {
                SetInvincibleMode();
            }
            else if (player.GetPlayerHealth().State == PlayerHealth.PlayerState.REVIVING)
            {
                SetReviveMode();
            }
        }
    }

    private void Update()
    {
        StateChange();
        if (player.GetPlayerHealth().State == PlayerHealth.PlayerState.ALIVE)
        {
            UpdateHealthBar();
        }
        else if (player.GetPlayerHealth().State == PlayerHealth.PlayerState.REVIVING)
        {
            UpdateReviveBar();
        }
        levelUpdate();
    }

    private void xpRecalc()
    {
        Debug.Log("reclaculate xp unit");
        xpUnit = 1f / displayedXpBarMax;
        xpUpdate();
    }

    private void xpUpdate()
    {
        Debug.Log("update xp");
        if (oldXp != levelable.Xp)
        {
            displayedXp = levelable.Xp - xpBarMin;
            xp.fillAmount = displayedXp * xpUnit;
            xpText.text = "XP: " + displayedXp;
            Debug.Log($"new xp {displayedXp} because we calculated {oldXp} + {levelable.Xp - oldXp}");
            oldXp = levelable.Xp;
        }

    }

    private void reassignXpValues()
    {
        Debug.Log("reassign xp values");
        xpBarMax = levelable.Level.xpNeeded; //actual xp needed for next level
        xpBarMin = levelable.Xp; //actual xp that you have currently;
        displayedXpBarMax = xpBarMax - xpBarMin;
        displayedXpBarMin = 0;
        displayedXp = 0;
        xpRecalc();
    }


    private void levelUpdate()
    {
        if (levelable.Level.id > displayedLevel)//if you leveled up
        {
            displayedLevel = levelable.Level.id;
            levelText.text = "" + displayedLevel;
            reassignXpValues();

            if (displayedLevel == 3 || displayedLevel == 7)
            {
                if (lvlEffect != null)
                {
                    p_UI.sprite = yeslvlup;
                    StartCoroutine(LevelUpEffect());
                }
            }
            else
            {
                p_UI.sprite = nolvlup;
            }
        }
        xpUpdate();
        //p_UI.sprite = nolvlup;
    }



    IEnumerator LevelUpEffect()
    {
        particles.gameObject.SetActive(true);
        lvlEffect.Play();
        yield return new WaitForSeconds(lvlEffect.main.duration);
        particles.gameObject.SetActive(false);

    }

}