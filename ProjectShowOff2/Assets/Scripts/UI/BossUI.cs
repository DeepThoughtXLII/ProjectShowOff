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
        if (player.MaxHealth > currentMaxHealth)
        {
            MaxHpRecalc();
        }
        int hp = player.Health;
        healthBar.fillAmount = healthUnit * hp;
        healthText.text = "Health: " + hp;
    }

    public void MaxHpRecalc()
    {
        currentMaxHealth = player.MaxHealth;
        float maxHp = player.MaxHealth;
        healthUnit = 1f / maxHp;
    }




}
