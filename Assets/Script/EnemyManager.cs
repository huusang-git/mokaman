using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public GameObject enemyPrefab; // Prefab của kẻ địch (Gán trong Inspector)
    public Transform[] spawnPoints; // Danh sách vị trí spawn

    private List<GameObject> enemies = new List<GameObject>();

    void Start()
    {
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.SetPatrolOrigin(spawnPoint.position); // Cập nhật vị trí tuần tra ban đầu
            }
            enemies.Add(enemy);
        }
    }
}
