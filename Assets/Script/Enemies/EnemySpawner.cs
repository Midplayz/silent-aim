using System.IO;
using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public PathDefiner[] paths;
    public Transform[] spawnPoints;
    public float spawnInterval = 2f;

    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            for (int i = 0; i < spawnPoints.Length; i++)
            {
                GameObject enemy = Instantiate(enemyPrefab, spawnPoints[i].position, Quaternion.identity);

                Enemy enemyScript = enemy.GetComponent<Enemy>();
                enemyScript.path = paths[i];
            }
        }
    }
}
