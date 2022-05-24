using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class waveSpawner : MonoBehaviour
{
    public static int EnemiesAlive = 0;

    public Wave[] waves;
    public waveEnemy[] waveEnemies;
    private int waveIndex = 0;
    private int currentWaveDangerLevel;
    private bool spawning = false;


    private List<int> dangerLevels;

    private List<Transform> spawnpoints;
    private int spawnPointAmount = 0;

    private int rand;
    private Transform currentSpawn;

    public float TimeBetweenWaves = 0;
    private float timeTillNextWave = 0;

    private int differentEnemies = 0;
    private waveEnemy currentEnemy;

    [ExecuteInEditMode]
    private void Awake()
    {
        dangerLevels = new List<int>();
        spawnpoints = new List<Transform>();
        foreach(waveEnemy enemy in waveEnemies)
        {
            differentEnemies++;
        }

        foreach(Transform child in transform)
        {
            spawnPointAmount++;
            spawnpoints.Add(child);
        }

    }

    void Start()
    {
        timeTillNextWave = TimeBetweenWaves;
    }

    void Update()
    {
        if (EnemiesAlive > 0 || spawning == true)
        {
            return;
        }

        if (timeTillNextWave < 0)
        {
            StartCoroutine(spawnWave());
            timeTillNextWave = TimeBetweenWaves;
        }

        timeTillNextWave -= Time.deltaTime;
    }


    IEnumerator spawnWave()
    {
        Console.WriteLine("Wave Started");
        currentWaveDangerLevel = waves[waveIndex].waveDangerLevel;
        while (currentWaveDangerLevel > 0)
        {
            spawning = true;
            bool repeat = true;
            while(repeat)
            {
                try
                {
                    randomEnemy();
                    if (currentWaveDangerLevel - currentEnemy.enemyDangerLevel >= 0)
                    {
                        currentWaveDangerLevel -= currentEnemy.enemyDangerLevel;
                        Console.WriteLine("Current Remaining Danger Level:" + currentWaveDangerLevel);
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                    
                    repeat = false;
                }
                catch(Exception e)
                {
                    Console.WriteLine("Enemy Danger Level Too High, Rolling again");
                    repeat = true;
                }
            }

            SpawnEnemies(currentEnemy.enemyPrefab);
            EnemiesAlive++;
            Console.WriteLine("Current Enemies Alive: " +EnemiesAlive);
            yield return new WaitForSeconds(waves[waveIndex].spawnRate);
        }
        
        waveIndex++;
        spawning = false;
    }
    void SpawnEnemies(GameObject enemyPrefab)
    {

        rand = UnityEngine.Random.Range(0, spawnPointAmount);
        currentSpawn = spawnpoints[rand].transform;
        Instantiate(enemyPrefab, currentSpawn);
    }

    void randomEnemy()
    {
        int randnum = UnityEngine.Random.Range(0, differentEnemies);
        currentEnemy = waveEnemies[randnum];
        Console.WriteLine("Random number generated: " + randnum);
    }

    
    void enemyChance()
    {
        foreach(waveEnemy enemy in waveEnemies)
        {
            dangerLevels.Add(enemy.enemyDangerLevel);
        }


    }
}


