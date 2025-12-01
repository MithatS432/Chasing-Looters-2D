using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Player : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D prb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    public AttackColliderAnim attackCollider;


    [Header("Player Settings")]
    [SerializeField] private float speed;
    private float attackIndex = 0f;
    private float tumbleForce = 5500f;

    public float maxHealth = 500f;
    public float currentHealth;
    bool isTakeDamage = false;
    public int totalCoint = 0;

    public GameObject waterEffectPrefab;
    public GameObject[] alliePrefabs;
    public Transform allySpawnPoint;
    public GameObject[] towerPrefabs;
    public Transform[] towerSpawnPoints;
    private bool[] towerSlotsFilled;
    private int nextSpawnIndex = 0;
    private bool isFull = false;
    public Image superPowerImage;
    public GameObject superPowerPrefab;
    public bool isUseSuperPower = false;



    [Header("Player UI Settings")]
    public Image healthBar;
    public Image lessHealthWarning;
    public bool isLessHealthWarningActive = false;
    public TextMeshProUGUI coinText;

    public Button continueButton;
    public Button quitGameButton;
    public Button pauseButton;

    public Button restartButton;
    public Button exitButton;
    public GameObject deathPanel;
    public GameObject deathImageCredits;

    public bool isDead = false;
    public bool isAlive = true;

    public GameObject shopPanel;
    public bool isShopping = false;
    public bool isNearShop = false;

    [HideInInspector] public float allyBehaviorMode = 0f;
    public TextMeshProUGUI allyModeText;



    [Header("Player Audio Settings")]
    public AudioClip[] playerSounds;
    private float stepTimer = 0f;
    public float stepInterval = 0.35f;

    public bool inWater = false;
    private float waterTimer = 0f;
    public float waterInterval = 0.5f;


    void Start()
    {
        Time.timeScale = 1f;
        prb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        currentHealth = maxHealth;
        UpdateHealthUI();
        GatherCoin(totalCoint);
        towerSlotsFilled = new bool[towerSpawnPoints.Length];
        restartButton.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().name));
        exitButton.onClick.AddListener(() =>
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        });
        continueButton.onClick.AddListener(() => Time.timeScale = 1f);
        quitGameButton.onClick.AddListener(() =>
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        });
        pauseButton.onClick.AddListener(() => Time.timeScale = 0f);
    }

    void Update()
    {
        if (!isAlive) return;
        speed = Input.GetKey(KeyCode.LeftShift) ? 18f : 12f;
        HandleFootsteps();

        if (prb.linearVelocity.x > 0)
            spriteRenderer.flipX = false;
        else if (prb.linearVelocity.x < 0)
            spriteRenderer.flipX = true;


        if (Input.GetMouseButtonDown(0))
        {
            attackCollider.EnableForSeconds(0.3f);
            anim.SetFloat("attackIndex", attackIndex);
            anim.SetTrigger("Attack");
            audioSource.PlayOneShot(playerSounds[1]);

            attackIndex++;
            if (attackIndex > 2f) attackIndex = 0f;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            audioSource.PlayOneShot(playerSounds[2]);
            anim.SetTrigger("Shield");
            StartCoroutine(ShieldRoutine());
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            anim.SetTrigger("Tumble");
            float dirx = spriteRenderer.flipX ? -1f : 1f;
            prb.AddForce(new Vector2(dirx, 0) * tumbleForce);
        }

        if (isNearShop && Input.GetKeyDown(KeyCode.E))
        {
            isShopping = !isShopping;
            shopPanel.SetActive(isShopping);
            Time.timeScale = isShopping ? 0f : 1f;
        }
    }
    private IEnumerator ShieldRoutine()
    {
        isTakeDamage = true;
        yield return new WaitForSeconds(2f);
        isTakeDamage = false;
    }
    private void HandleFootsteps()
    {
        bool isMoving = prb.linearVelocity.magnitude > 0.1f;

        // Normal ayak sesleri
        if (isMoving && !inWater)
        {
            stepTimer += Time.deltaTime;

            if (stepTimer >= stepInterval)
            {
                audioSource.PlayOneShot(playerSounds[0]);
                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = 0f;
        }

        // Su ayak sesleri
        if (isMoving && inWater)
        {
            waterTimer += Time.deltaTime;

            if (waterTimer >= waterInterval)
            {
                audioSource.PlayOneShot(playerSounds[5]);
                GameObject waterEffect = Instantiate(waterEffectPrefab, transform.position, Quaternion.identity);
                Destroy(waterEffect, 1f);
                waterTimer = 0f;
            }
        }
        else
        {
            waterTimer = 0f;
        }
    }

    private void FixedUpdate()
    {
        if (!isAlive)
        {
            prb.linearVelocity = Vector2.zero;
            return;
        }
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector2 move = new Vector2(x, y);
        prb.linearVelocity = move * speed;
        anim.SetFloat("Speed", Mathf.Abs(x) + Mathf.Abs(y));
    }



    public void GetDamage(float damage)
    {
        if (isTakeDamage) return;
        currentHealth -= damage;
        anim.SetTrigger("Hurt");
        UpdateHealthUI();
        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            isAlive = false;
            anim.SetTrigger("Death");
            audioSource.PlayOneShot(playerSounds[4]);
            Invoke("DeathScreen", 1.5f);
        }
    }
    public void UpdateHealthUI()
    {
        float healthFraction = currentHealth / maxHealth;
        healthBar.fillAmount = healthFraction;

        if (healthFraction > 0.5f)
        {
            healthBar.color = Color.green;
            lessHealthWarning.gameObject.SetActive(false); 
            isLessHealthWarningActive = false;
        }
        else if (healthFraction > 0.2f)
        {
            healthBar.color = Color.yellow;
            lessHealthWarning.gameObject.SetActive(false);
            isLessHealthWarningActive = false;
        }
        else
        {
            healthBar.color = Color.red;
            if (!isLessHealthWarningActive)
            {
                audioSource.PlayOneShot(playerSounds[3]);
                isLessHealthWarningActive = true;
            }
            lessHealthWarning.gameObject.SetActive(true);
        }
    }

    void DeathScreen()
    {
        deathPanel.SetActive(true);
        deathImageCredits.SetActive(true);
        restartButton.gameObject.SetActive(true);
        exitButton.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Water"))
        {
            inWater = true;
        }
        if (other.CompareTag("Shop"))
        {
            isNearShop = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Water"))
        {
            inWater = false;
        }
        if (other.CompareTag("Shop"))
        {
            isNearShop = false;
            isShopping = false;
            shopPanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    public void GatherCoin(int coinCount)
    {
        totalCoint += coinCount;
        coinText.SetText(totalCoint.ToString());
    }

    public void Soldier()
    {
        if (totalCoint >= 25)
        {
            totalCoint -= 25;
            coinText.text = totalCoint.ToString();
            audioSource.PlayOneShot(playerSounds[6]);
            Instantiate(alliePrefabs[0], allySpawnPoint.position, Quaternion.identity);
        }
    }
    public void Peasent()
    {
        if (totalCoint >= 50)
        {
            totalCoint -= 50;
            coinText.text = totalCoint.ToString();
            audioSource.PlayOneShot(playerSounds[6]);
            Instantiate(alliePrefabs[1], allySpawnPoint.position, Quaternion.identity);
        }
    }
    public void Priest()
    {
        if (totalCoint >= 40)
        {
            totalCoint -= 40;
            coinText.text = totalCoint.ToString();
            audioSource.PlayOneShot(playerSounds[6]);
            Instantiate(alliePrefabs[2], allySpawnPoint.position, Quaternion.identity);
        }
    }
    public void Knight()
    {
        if (totalCoint >= 100)
        {
            totalCoint -= 100;
            coinText.text = totalCoint.ToString();
            audioSource.PlayOneShot(playerSounds[6]);
            Instantiate(alliePrefabs[3], allySpawnPoint.position, Quaternion.identity);
        }
    }
    public void Thief()
    {
        if (totalCoint >= 60)
        {
            totalCoint -= 60;
            coinText.text = totalCoint.ToString();
            audioSource.PlayOneShot(playerSounds[6]);
            Instantiate(alliePrefabs[4], allySpawnPoint.position, Quaternion.identity);
        }
    }
    public void AllyBoss()
    {
        if (totalCoint >= 500)
        {
            totalCoint -= 500;
            coinText.text = totalCoint.ToString();
            audioSource.PlayOneShot(playerSounds[6]);
            Instantiate(alliePrefabs[5], allySpawnPoint.position, Quaternion.identity);
        }
    }

    public void Tower1() => BuyTower(300, 0);
    public void Tower2() => BuyTower(400, 1);
    public void Tower3() => BuyTower(500, 2);
    public void Tower4() => BuyTower(800, 3);
    public void Tower5() => BuyTower(900, 4);
    public void Tower6() => BuyTower(1000, 5);

    public void BuyTower(int price, int towerIndex)
    {
        if (isFull)
            return;

        if (totalCoint < price)
            return;

        totalCoint -= price;
        coinText.text = totalCoint.ToString();
        audioSource.PlayOneShot(playerSounds[6]);

        while (nextSpawnIndex < towerSpawnPoints.Length && towerSlotsFilled[nextSpawnIndex])
            nextSpawnIndex++;

        if (nextSpawnIndex >= towerSpawnPoints.Length)
        {
            isFull = true;
            return;
        }

        Instantiate(
            towerPrefabs[towerIndex],
            towerSpawnPoints[nextSpawnIndex].position,
            Quaternion.identity
        );

        towerSlotsFilled[nextSpawnIndex] = true;
        nextSpawnIndex++;

        if (nextSpawnIndex >= towerSpawnPoints.Length)
            isFull = true;
    }


    public void SetAllyBehavior(float value)
    {
        allyBehaviorMode = value;

        if (allyModeText != null)
        {
            if (value == 0)
                allyModeText.text = "FOLLOW PLAYER";
            else
                allyModeText.text = "PROTECT VILLAGE";
        }

        Allies[] allAllies = FindObjectsByType<Allies>(FindObjectsSortMode.None);
        foreach (Allies ally in allAllies)
        {
            ally.currentMode = allyBehaviorMode;
        }
    }

}