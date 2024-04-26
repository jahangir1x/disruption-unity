using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject enemyPrefab;

    public List<EnemyHandler> enemyHandlers;

    private bool shouldSpawn = false;
    private int spawnCount = 1;
    private int needToSpawn;

    private void Start()
    {
        enemyHandlers = new List<EnemyHandler>();
    }

    private void Update()
    {
        Cleanup();
        checkIfshouldSpawn();

        if (shouldSpawn)
        {
            needToSpawn = spawnCount;
            for (int i = 0; i < needToSpawn; i++)
            {
                SpawnEnemy();
            }
            shouldSpawn = false;
        }

    }

    private void checkIfshouldSpawn()
    {
        if (enemyHandlers.Count < 2)
        {
            shouldSpawn = true;
        }
    }

    private void Cleanup()
    {
        enemyHandlers.RemoveAll(x => x.health <= 0);
    }

    public void DeleteAllEnemies()
    {
        foreach (EnemyHandler enemyHandler in enemyHandlers)
        {
            Destroy(enemyHandler.gameObject);
        }
    }

    private void SpawnEnemy()
    {
        Vector3 spawnPosition = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].position;

        float rotation = UnityEngine.Random.Range(0f, 360f);
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.Euler(0, rotation, 0));
        enemyHandlers.Add(enemy.GetComponentInChildren<EnemyHandler>());
        spawnCount += 1;
    }
}
