using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//[System.Serializable]
//[ExecuteInEditMode]
public class spawnChances : MonoBehaviour
{
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     FIELDS
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public waveSpawner _waveSpawner;
    public List<GameObject> enemyPrefabList;
    private List<int> enemyDangerLevelList;
    private List<int> EDLNoDupe;
    public spawnChancesVariables[] DLSpawnChances;

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     AWAKE
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void Awake()
    {
        enemyPrefabList = new List<GameObject>();
        enemyDangerLevelList = new List<int>();
        EDLNoDupe = new List<int>();
    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     UPDATE
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void Update()
    {
        getDangerLevels();
        setDLValues();
    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                    GET DANGER LEVELS
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void getDangerLevels()
    {

        foreach (waveEnemy enemies in _waveSpawner.waveEnemies)
        {
            if (!enemyPrefabList.Contains(enemies.enemyPrefab))
            {
                enemyPrefabList.Add(enemies.enemyPrefab);
                enemyDangerLevelList.Add(enemies.enemyDangerLevel);
            }
        }

        EDLNoDupe = enemyDangerLevelList.Distinct().ToList();
    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     SET DL VALUES
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void setDLValues()
    {
        DLSpawnChances = new spawnChancesVariables[EDLNoDupe.Count()];
        for(int i = 0; i < DLSpawnChances.Length;)
        {
            DLSpawnChances[i].dangerLevel = EDLNoDupe[i];
            Console.WriteLine("" + EDLNoDupe[i]);
            i++;
        }

    }
}


