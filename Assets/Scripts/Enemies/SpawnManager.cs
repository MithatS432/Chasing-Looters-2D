using System.Collections;
using UnityEngine;
using TMPro;

public class SpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    public Transform[] spawnPoints;
    public GameObject[] enemies;
    public GameObject bossPrefab;
    public float spawnInterval = 1f;

    [Header("Wave Settings")]
    public int startingEnemies = 2;
    public int enemyIncreasePerWave = 2;
    public int enemiesPerWave = 5;
    public float prepTime = 30f;
    public int bossWaveNumber = 10; // Boss hangi dalgada gelsin
    public GameObject nextWaveUI;
    public TextMeshProUGUI timerCountText;

    [Header("Audio")]
    public AudioClip warningSound;
    public AudioClip waveWinSound;
    public AudioClip bossAppearSound;

    private int currentWave = 0;
    private int aliveEnemies = 0;
    private bool warningPlayed = false;

    void Start()
    {
        StartCoroutine(WaveCycle());
    }

    private void SpawnEnemy(GameObject enemyPrefab)
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Vector3 spawnPos = spawnPoint.position;
        spawnPos.z = 0f;

        GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        Enemies enemyScript = spawnedEnemy.GetComponent<Enemies>();
        if (enemyScript != null)
            enemyScript.onDeath += OnEnemyDeath;

        aliveEnemies++;
    }

    IEnumerator WaveCycle()
    {
        enemiesPerWave = startingEnemies;

        while (true)
        {
            float prepTimer = prepTime;
            warningPlayed = false;

            while (prepTimer > 0f)
            {
                prepTimer -= Time.deltaTime;
                int displayTime = Mathf.CeilToInt(prepTimer);
                if (timerCountText != null)
                    timerCountText.text = displayTime.ToString();

                if (!warningPlayed && displayTime <= 10)
                {
                    if (warningSound != null)
                        AudioSource.PlayClipAtPoint(warningSound, Camera.main.transform.position);
                    warningPlayed = true;
                }

                yield return null;
            }

            if (nextWaveUI != null)
                nextWaveUI.SetActive(true);

            Debug.Log("Next Wave! Dalga " + (currentWave + 1));
            yield return new WaitForSeconds(3f);

            if (nextWaveUI != null)
                nextWaveUI.SetActive(false);

            currentWave++;
            aliveEnemies = 0;

            bool isBossWave = currentWave == bossWaveNumber;

            if (isBossWave && bossPrefab != null)
            {
                AudioSource.PlayClipAtPoint(bossAppearSound, Camera.main.transform.position);
                SpawnEnemy(bossPrefab);
            }
            else
            {
                for (int i = 0; i < enemiesPerWave; i++)
                {
                    GameObject enemyPrefab = enemies[Random.Range(0, enemies.Length)];
                    SpawnEnemy(enemyPrefab);
                    yield return new WaitForSeconds(spawnInterval);
                }
            }


            yield return new WaitUntil(() => aliveEnemies <= 0);

            if (waveWinSound != null)
                AudioSource.PlayClipAtPoint(waveWinSound, Camera.main.transform.position);

            Debug.Log("Dalga " + currentWave + " tamamlandı!");

            enemiesPerWave += enemyIncreasePerWave;
        }
    }

    private void OnEnemyDeath()
    {
        aliveEnemies = Mathf.Max(0, aliveEnemies - 1);
    }
}
