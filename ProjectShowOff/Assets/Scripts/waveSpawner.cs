using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waveSpawner : MonoBehaviour
{
    public Transform Spawn1;
    public Transform Spawn2;

    private int rand;
    private Transform currentSpawn;

    public GameObject enemyPrefab;

    public float TimeBetweenSpawns = 0;
    private float timeTillSpawn = 0;

    private GameObject _enemyPrefab;
    // Start is called before the first frame update
    void Start()
    {
        _enemyPrefab = enemyPrefab;
        timeTillSpawn = TimeBetweenSpawns;
    }

    // Update is called once per frame
    void Update()
    {
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        timeTillSpawn -= Time.deltaTime;
        if(timeTillSpawn < 0)
        {
            rand = Random.Range(1, 3);

            if(rand == 1)
            {
                currentSpawn = Spawn1;
            }
            else
            {
                currentSpawn = Spawn2;
            }

            Instantiate(enemyPrefab, currentSpawn);
            timeTillSpawn = TimeBetweenSpawns;
        }
    }
}


