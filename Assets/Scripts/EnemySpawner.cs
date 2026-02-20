// using UnityEngine;
// using System.Collections;

// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;

// public class EnemySpawner : MonoBehaviour
// {
//     [System.Serializable]
// public class EnemySpawnEntry
// {
//     public GameObject enemyPrefab;

//     [Range(0f, 1f)]
//     public float spawnChance = 1f;
// }


//     [Header("Spawn Points")]
//     [SerializeField] private Transform[] spawnPoints;

//     [Header("Enemies")]
//     [SerializeField] private List<EnemySpawnEntry> enemyList = new();

//     [Header("Timing")]
//     [SerializeField] private float spawnInterval = 3f;

//     private float totalSpawnChance;

//     private void Awake()
//     {
//         CalculateTotalChance();
//     }

//     private void Start()
//     {
//         StartCoroutine(SpawnLoop());
//     }

//     private IEnumerator SpawnLoop()
//     {
//         while (true)
//         {
//             SpawnEnemy();
//             yield return new WaitForSeconds(spawnInterval);
//         }
//     }

//     private void SpawnEnemy()
//     {
//         if (spawnPoints.Length == 0 || enemyList.Count == 0)
//             return;

//         Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
//         GameObject enemyPrefab = GetRandomEnemy();

//         if (enemyPrefab != null)
//         {
//             Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
//         }
//     }

//     private GameObject GetRandomEnemy()
//     {
//         float randomValue = Random.Range(0f, totalSpawnChance);
//         float cumulative = 0f;

//         foreach (EnemySpawnEntry entry in enemyList)
//         {
//             cumulative += entry.spawnChance;

//             if (randomValue <= cumulative)
//             {
//                 return entry.enemyPrefab;
//             }
//         }

//         return enemyList[0].enemyPrefab;
//     }

//     private void CalculateTotalChance()
//     {
//         totalSpawnChance = 0f;

//         foreach (EnemySpawnEntry entry in enemyList)
//         {
//             totalSpawnChance += entry.spawnChance;
//         }
//     }
// }


// Gwen Proposal:
// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;

// public class EnemySpawner : MonoBehaviour
// {
//     [System.Serializable]
//     public class EnemySpawnEntry
//     {
//         public GameObject enemyPrefab;

//         [Range(0f, 1f)]
//         public float spawnChance = 1f;
//     }

//     [Header("Spawn Points")]
//     [SerializeField] private Transform[] spawnPoints;

//     [Header("Enemies")]
//     [SerializeField] private List<EnemySpawnEntry> enemyList = new();

//     [Header("Timing")]
//     [SerializeField] private float minSpawnInterval = 1f;
//     [SerializeField] private float maxSpawnInterval = 5f;

//     private float totalSpawnChance;

//     private void Awake()
//     {
//         CalculateTotalChance();
//     }

//     private void Start()
//     {
//         StartCoroutine(SpawnLoop());
//     }

//     private IEnumerator SpawnLoop()
//     {
//         while (true)
//         {
//             SpawnEnemy();
//             float randomInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
//             yield return new WaitForSeconds(randomInterval);
//         }
//     }

//     private void SpawnEnemy()
//     {
//         if (spawnPoints.Length == 0 || enemyList.Count == 0)
//             return;

//         Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
//         GameObject enemyPrefab = GetRandomEnemy();

//         if (enemyPrefab != null)
//         {
//             Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
//         }
//     }

//     private GameObject GetRandomEnemy()
//     {
//         if (totalSpawnChance <= 0f) return null;

//         float randomValue = Random.Range(0f, totalSpawnChance);
//         float cumulative = 0f;

//         foreach (EnemySpawnEntry entry in enemyList)
//         {
//             cumulative += entry.spawnChance;
//             if (randomValue <= cumulative)
//             {
//                 return entry.enemyPrefab;
//             }
//         }

//         return enemyList[0].enemyPrefab;
//     }

//     private void CalculateTotalChance()
//     {
//         totalSpawnChance = 0f;
//         foreach (EnemySpawnEntry entry in enemyList)
//         {
//             totalSpawnChance += entry.spawnChance;
//         }
//     }
// }

//Chat gpt proposal 

using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public Transform[] spawnPoints;
    public GameObject[] enemyPrefabs;
    public float spawnDelay = 1f;

    public void SpawnWave(int enemyCount)
    {
        StartCoroutine(SpawnRoutine(enemyCount));
    }

    private IEnumerator SpawnRoutine(int enemyCount)
    {
        for (int i = 0; i < enemyCount; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void SpawnEnemy()
    {
        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        Instantiate(enemy, point.position, point.rotation);
    }
}
