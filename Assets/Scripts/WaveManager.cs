// using UnityEngine;
// using System.Collections;

// public class WaveManager : MonoBehaviour
// {
//     [Header("Wave Settings")]
//     public int baseEnemies = 10;
//     public int enemiesPerWaveIncrease = 2;
//     public float timeBetweenWaves = 10f;

//     public int CurrentWave { get; private set; } = 1;
//     public int EnemiesRemaining { get; private set; }

//     private EnemySpawner spawner;

//     private void Awake()
//     {
//         spawner = FindObjectOfType<EnemySpawner>();
//     }

//     private void Start()
//     {
//         StartWave();
//     }

//     private void StartWave()
//     {
//         int enemiesToSpawn =
//             baseEnemies + (CurrentWave - 1) * enemiesPerWaveIncrease;

//         EnemiesRemaining = enemiesToSpawn;

//         Debug.Log($"🌊 Wave {CurrentWave} started!");
//         Debug.Log($"👾 Enemies remaining: {EnemiesRemaining}");

//         spawner.SpawnWave(enemiesToSpawn);
//     }

//     // Called by enemies when they die
//     public void OnEnemyKilled()
//     {
//         EnemiesRemaining--;

//         Debug.Log($"👾 Enemies remaining: {EnemiesRemaining}");

//         if (EnemiesRemaining <= 0)
//         {
//             Debug.Log($"✅ Wave {CurrentWave} cleared!");
//             StartCoroutine(NextWaveCountdown());
//         }
//     }

//     private IEnumerator NextWaveCountdown()
//     {
//         Debug.Log($"⏳ Next wave in {timeBetweenWaves} seconds...");

//         yield return new WaitForSeconds(timeBetweenWaves);

//         CurrentWave++;
//         StartWave();
//     }
// }

//Emprover Version :
using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private int baseEnemies = 10;
    [SerializeField] private int enemiesPerWaveIncrease = 2;
    [SerializeField] private float timeBetweenWaves = 10f;

    public int CurrentWave { get; private set; } = 1;
    public int EnemiesRemaining { get; private set; }

    private EnemySpawner spawner;
    private Coroutine nextWaveCoroutine;
    private bool waveEnding;

    private void Awake()
    {
        spawner = FindObjectOfType<EnemySpawner>();

        if (spawner == null)
            Debug.LogError("WaveManager: No EnemySpawner found in the scene!");
    }

    private void Start()
    {
        if (spawner != null)
            StartWave();
    }

    private void StartWave()
    {
        waveEnding = false;

        int enemiesToSpawn = baseEnemies + (CurrentWave - 1) * enemiesPerWaveIncrease;
        EnemiesRemaining = enemiesToSpawn;

        Debug.Log($"🌊 Wave {CurrentWave} started!");
        Debug.Log($"👾 Enemies remaining: {EnemiesRemaining}");

        spawner.SpawnWave(enemiesToSpawn);
    }

    public void OnEnemyKilled()
    {
        if (waveEnding) return; // prevents extra calls while waiting for next wave

        EnemiesRemaining--;
        Debug.Log($"👾 Enemies remaining: {EnemiesRemaining}");

        if (EnemiesRemaining <= 0)
        {
            waveEnding = true;
            Debug.Log($"✅ Wave {CurrentWave} cleared!");

            if (nextWaveCoroutine != null)
                StopCoroutine(nextWaveCoroutine);

            nextWaveCoroutine = StartCoroutine(NextWaveCountdown());
        }
    }

    private IEnumerator NextWaveCountdown()
    {
        Debug.Log($"⏳ Next wave in {timeBetweenWaves} seconds...");
        yield return new WaitForSeconds(timeBetweenWaves);

        CurrentWave++;
        StartWave();
        nextWaveCoroutine = null;
    }
}
