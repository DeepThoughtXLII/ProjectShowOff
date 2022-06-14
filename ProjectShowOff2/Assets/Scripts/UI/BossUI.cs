using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossUI : MonoBehaviour
{

    Player player;

    Image healthBar;
    TextMeshProUGUI healthText;
    float healthUnit;
    int currentMaxHealth;

    public Player Player
    {
        set { player = value; }
        get { return player; }
    }

    // Start is called before the first frame update
    void Start()
    {
       healthBar =  transform.GetChild(1).GetComponent<Image>();
        healthText = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        MaxHpRecalc();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealthBar();
    }



    public void UpdateHealthBar()
    {
        if (player.GetPlayerHealth().MaxHealth > currentMaxHealth)
        {
            MaxHpRecalc();
        }
        int hp = player.GetPlayerHealth().Health;
        healthBar.fillAmount = healthUnit * hp;
        healthText.text = "Health: " + hp;
    }

    public void MaxHpRecalc()
    {
        currentMaxHealth = player.GetPlayerHealth().MaxHealth;
        float maxHp = player.GetPlayerHealth().MaxHealth;
        healthUnit = 1f / maxHp;
    }




}
