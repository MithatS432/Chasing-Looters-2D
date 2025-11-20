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

    [Header("Player Settings")]
    [SerializeField] private float speed;
    private float attackIndex = 0f;
    private float tumbleForce = 5500f;

    float maxHealth = 500f;
    public float currentHealth;
    public int coinCount = 0;

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


    [Header("Player Audio Settings")]
    public AudioClip[] playerSounds;
    private float stepTimer = 0f;
    public float stepInterval = 0.35f;
    public bool inWater = false;


    void Start()
    {
        Time.timeScale = 1f;
        prb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        currentHealth = maxHealth;
        UpdateHealthUI();
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
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            anim.SetTrigger("Tumble");
            float dirx = spriteRenderer.flipX ? -1f : 1f;
            prb.AddForce(new Vector2(dirx, 0) * tumbleForce);
        }

    }
    private void HandleFootsteps()
    {
        bool isMoving = prb.linearVelocity.magnitude > 0.1f;

        if (isMoving)
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
        if (inWater)
        {
            audioSource.PlayOneShot(playerSounds[5]);
        }
    }
    private void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector2 move = new Vector2(x, y);
        prb.linearVelocity = move * speed;
        anim.SetFloat("Speed", Mathf.Abs(x) + Mathf.Abs(y));
    }

    public void TakeDamage(float damage)
    {

    }

    public void GetDamage(float damage)
    {
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
    void UpdateHealthUI()
    {
        float healthFraction = currentHealth / maxHealth;
        healthBar.fillAmount = healthFraction;

        if (healthFraction > 0.5f)
        {
            healthBar.color = new Color(0f, 1f, 0f);
        }
        else if (healthFraction > 0.2f)
        {
            healthBar.color = new Color(1f, 1f, 0f);
        }
        else
        {
            healthBar.color = new Color(1f, 0f, 0f);
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
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Water"))
        {
            inWater = false;
        }
    }
}