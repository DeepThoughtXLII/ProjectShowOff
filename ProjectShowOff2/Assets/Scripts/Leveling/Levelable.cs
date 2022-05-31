using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Levelable : MonoBehaviour, ILevelable
{
    [SerializeField]private Level level;

    [SerializeField] private int xp = 0;

    [SerializeField] private int nextLevelAt = 0;

    private LevelManager lvlManager;

    private void Start()
    {
        lvlManager = transform.parent.GetComponentInChildren<LevelManager>();//GetComponentInSibling<LevelManager>();
        level = lvlManager.GetLevel(1);
        nextLevelAt = level.xpNeeded;
    }


    public Level Level
    {
        set { level = value; }
        get { return level; }
    }

    public int Xp
    {
        get { return xp; }
    }

    public void LevelUp(Level lvl)
    {
        level = lvl;
        nextLevelAt = lvl.xpNeeded;
    }

    public void GainXP(int pXp)
    {
        xp += pXp;
        if(xp >= nextLevelAt)
        {
            Debug.Log("LEVEL UP YAY!");
            LevelUp(lvlManager.GetNextLevel(level));
        }
    }

}
