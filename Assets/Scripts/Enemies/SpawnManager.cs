using System.Collections;
using UnityEngine;
using TMPro; // Eğer timerCountText TextMeshProUGUI ise

public class SpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    public Transform[] spawnPoints;
    public GameObject[] enemies;
    public int enemiesPerWave = 5;
    public float spawnInterval = 1f;

    [Header("Wave Settings")]
    public float prepTime = 30f;
    public GameObject nextWaveUI;
    public TextMeshProUGUI timerCountText;

    private int currentWave = 0;

    private int aliveEnemies;
    public int startingEnemies = 2;// 1. dalgada gelecek düşman sayısı
    public int enemyIncreasePerWave = 2;

    void Start()
    {
        StartCoroutine(WaveCycle());
    }

    IEnumerator WaveCycle()
    {
        enemiesPerWave = startingEnemies;

        while (true)
        {
            float prepTimer = prepTime;
            while (prepTimer > 0f)
            {
                prepTimer -= Time.deltaTime;
                int displayTime = Mathf.CeilToInt(prepTimer);
                if (timerCountText != null)
                    timerCountText.text = displayTime.ToString();

                yield return null;
            }

            if (nextWaveUI != null)
                nextWaveUI.SetActive(true);
            Debug.Log("Next Wave! Dalga " + (currentWave + 1));
            yield return new WaitForSeconds(3f);
            if (nextWaveUI != null)
                nextWaveUI.SetActive(false);

            currentWave++;
            aliveEnemies = enemiesPerWave;
            int spawnedThisWave = 0;
            while (spawnedThisWave < enemiesPerWave)
            {
                Transform spawnPoint = spawnPoints[spawnedThisWave % spawnPoints.Length];
                GameObject enemyPrefab = enemies[Random.Range(0, enemies.Length)];

                GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
                Enemies enemyScript = spawnedEnemy.GetComponent<Enemies>();
                if (enemyScript != null)
                    enemyScript.onDeath += OnEnemyDeath;

                spawnedThisWave++;
                yield return new WaitForSeconds(spawnInterval);
            }

            yield return new WaitUntil(() => aliveEnemies <= 0);

            Debug.Log("Dalga " + currentWave + " tamamlandı!");

            enemiesPerWave += enemyIncreasePerWave;
        }
    }

    private void OnEnemyDeath()
    {
        aliveEnemies--;
    }
}
