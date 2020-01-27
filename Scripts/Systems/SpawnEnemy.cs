using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public static SpawnEnemy instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Multiple instances of " + this + " found");
        }

        instance = this;
    }

    TerrainPlacer terrainPlacer;

    public GameObject buggyEnemy;
    public float minSpawnTime;
    public float maxSpawnTime;

    private void Start()
    {
        terrainPlacer = TerrainPlacer.instance;
    }

    public void StartSpawners()
    {
        //StartCoroutine(IEnemySpawner(buggyEnemy));
    }

    public void StopSpawners()
    {
        StopAllCoroutines();
    }

    IEnumerator IEnemySpawner(GameObject enemyType)
    {
        while (true)
        {
            Vector3 spawnPoint = GenerateRandomPoint();
            yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));
            Debug.Log(spawnPoint);
            Instantiate(enemyType, spawnPoint, Quaternion.identity);
        }
    }

    Vector3 GenerateRandomPoint(int layer = 0)
    {
        if (layer >= 100) // abort the program
        {
            return Vector3.up * (terrainPlacer.planetRadius + 5);
        }

        Vector3 randomPoint = new Vector3(Random.Range(-terrainPlacer.planetRadius, terrainPlacer.planetRadius), Random.Range(-terrainPlacer.planetRadius, terrainPlacer.planetRadius), Random.Range(-terrainPlacer.planetRadius, terrainPlacer.planetRadius));

        if (terrainPlacer.IsEmptyPoint(randomPoint))
        {
            return randomPoint;
        }
        else
        {
            return GenerateRandomPoint(layer++);
        }
    }
}
