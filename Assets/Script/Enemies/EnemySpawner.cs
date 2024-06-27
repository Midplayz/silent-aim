using System.Collections;
using System.IO;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public PathDefiner[] paths;
    public Transform[] spawnPoints;
    public Transform playerTransform;
    public float spawnInterval = 2f;
    public int maxEnemies = 10;
    public int numberOfSkins = 50;

    private int currentEnemyCount = 0;

    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            float timeToWait = Random.Range(spawnInterval - 1, spawnInterval + 1);
            yield return new WaitForSeconds(timeToWait);

            if (currentEnemyCount < maxEnemies)
            {
                for (int i = 0; i < spawnPoints.Length; i++)
                {
                    if (currentEnemyCount < maxEnemies)
                    {
                        GameObject enemy = Instantiate(enemyPrefab, spawnPoints[i].position, Quaternion.identity);

                        int skinToUse = Random.Range(0, numberOfSkins);
                        enemy.transform.GetChild(skinToUse).gameObject.SetActive(true);

                        Enemy enemyScript = enemy.GetComponent<Enemy>();
                        enemyScript.player = playerTransform;
                        enemyScript.path = paths[i];
                        enemyScript.spawner = this;

                        currentEnemyCount++;
                    }
                }
            }
        }
    }

    public void DecreaseEnemyCount()
    {
        currentEnemyCount--;
    }
}
