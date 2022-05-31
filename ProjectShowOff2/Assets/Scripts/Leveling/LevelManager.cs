using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    public List<Level> levels = new List<Level>();

    public void AddLevel(int xpNeeded, bool hasUpgrade = false)
    {
        levels.Add(new Level(xpNeeded, hasUpgrade, levels.Count));
    }

    public Level AddLevel()
    {
        Level level = new Level();       
        level.id = levels.Count+1;     
        levels.Add(level);
        return level;
    }

    public void checkXP(ILevelable levelable)
    {
        if(levelable.Level.xpNeeded <= levelable.Xp)
        {
            levelable.LevelUp(levels[levelable.Level.id]);
        }
    }

    public Level GetNextLevel(Level currentLevel)
    {
        return levels[currentLevel.id];
    }

    public Level GetLevel(int id)
    {
        return levels[id-1];
    }

}


[System.Serializable]
public class Level
{
    public int id;
    public int xpNeeded;
    public bool hasUpgrade;
    public Upgrade upgrade = null;



    public Level(int pXpNeeded = 0, bool pHasUpgrade = false, int pId = -1)
    {
        xpNeeded = pXpNeeded;
        hasUpgrade = pHasUpgrade;
        id = pId;
    }


}

[System.Serializable]
public class Upgrade
{


}