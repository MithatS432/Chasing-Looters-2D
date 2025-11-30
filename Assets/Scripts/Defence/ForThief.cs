using UnityEngine;

public class ForThief : MonoBehaviour
{
    [SerializeField] private int stealCoinCount = 10;
    [SerializeField] private Player player;

    private float stealTime;
    private float timer = 0f;

    void Start()
    {
        SetNextStealTime();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= stealTime)
        {
            StealCoin();
            timer = 0f;
            SetNextStealTime();
        }
    }

    void StealCoin()
    {
        if (player != null)
        {
            if (player.totalCoint >= stealCoinCount)
            {
                player.totalCoint -= stealCoinCount;
                player.UpdateCoinUI();
                Debug.Log($"Hırsız {stealCoinCount} altın çaldı. Kalan: {player.totalCoint}");
            }
            else
            {
                Debug.Log("Yeterli altın yok, çalamıyor.");
            }
        }
    }

    void SetNextStealTime()
    {
        stealTime = Random.Range(5f, 10f);
    }
}
