using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILevelable 
{

    public Level Level { get; set; }

    public int Xp { get; }

    void LevelUp(Level lvl);

    void GainXP(int xp);
}
