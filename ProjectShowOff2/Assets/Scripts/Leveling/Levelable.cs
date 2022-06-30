using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Levelable : MonoBehaviour, ILevelable
{

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     ACTIONS
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public static event Action onUpgrade;
    public static event Action onUpgradeChosen;
    public static event Action onLevelUp;
    public ParticleSystem levelup;


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     FIELDS
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    [SerializeField]private Level level;

    [SerializeField] private int xp = 0;

    [SerializeField] private int nextLevelAt = 0;

    private List<Upgrade> UpgradesToTake;
    private List<Upgrade> UpgradesTaken;

    private LevelManager lvlManager;

    private Player player;
    private playerShooting playerShot;


    public bool upgradesAvailable;

    PlayerUI _pUI;


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     START()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void Start()
    {
        _pUI = GetComponent<PlayerUI>();
        player = GetComponent<Player>();
        playerShot = GetComponent<playerShooting>();
        UpgradesToTake = new List<Upgrade>();
        UpgradesTaken = new List<Upgrade>();

        lvlManager = transform.parent.GetComponentInChildren<LevelManager>();//GetComponentInSibling<LevelManager>();
        level = lvlManager.GetLevel(1);
        nextLevelAt = level.xpNeeded;
    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     GET() AND SET()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public Level Level
    {
        set { level = value; }
        get { return level; }
    }

    public int Xp
    {
        get { return xp; }
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     LEVEL UP()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void LevelUp(Level lvl)
    {
        level = lvl;
        nextLevelAt = lvl.xpNeeded;
        FindObjectOfType<SoundManager>().Play("playerLevelUp");
        levelup.Play();

    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     GAIN XP()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void GainXP(int pXp)
    {
        xp += pXp;
        if(xp >= nextLevelAt)
        {
            //Debug.Log("LEVEL UP YAY!");
           
            LevelUp(lvlManager.GetNextLevel(level));
            if (level.hasUpgrade && player.GetPlayerMovement().IsUsingInput == Controls.ONLINE)
            {
                _pUI.p_UI.sprite = _pUI.yeslvlup;
                UpgradesToTake.Add(level.upgrade);
                upgradesAvailable = true;
                Debug.Log("upgrade booyaaa");
                onUpgrade();
            }
            onLevelUp();
        }
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     APPLY UPGRADE()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void ApplyUpgrade(Upgrade upgrade)
    {
        switch (upgrade.typeChosen)
        {
            case Upgrade.UpgradeType.ATTACK:
                playerShot.dmg += upgrade.AttackValue;
                Debug.Log("dmg upgrade");
                break;
            case Upgrade.UpgradeType.HEALTH:
                Debug.Log("health before: "+player.GetPlayerHealth().Health + "max health before:  " + player.GetPlayerHealth().MaxHealth + "upgrade Value: "+upgrade.HealthValue);
                player.GetPlayerHealth().Health += upgrade.HealthValue;
                player.GetPlayerHealth().MaxHealth += upgrade.HealthValue;
                Debug.Log("health after: " + player.GetPlayerHealth().Health + "max health after:  " + player.GetPlayerHealth().MaxHealth);
                break;
            case Upgrade.UpgradeType.SPEED:
                player.GetPlayerMovement().speed += upgrade.SpeedValue;
                Debug.Log("speed upgrade");
                break;
            case Upgrade.UpgradeType.UNDEFINED:
                
                break;
        }
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     CHOOSE UPGRADE()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void ChooseUpgrade(Upgrade.UpgradeType type)
    {
        if(UpgradesToTake.Count > 0)
        {
            UpgradesToTake[0].ChoseType(type);
            ApplyUpgrade(UpgradesToTake[0]);
            UpgradesToTake.RemoveAt(0);
        }

    }

}
