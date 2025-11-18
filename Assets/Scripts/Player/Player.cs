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

    float maxHealth = 500f;
    public float currentHealth;
    public int coinCount = 0;

    [Header("Player UI Settings")]
    public GameObject[] healthBars;
    public TextMeshProUGUI coinText;


    [Header("Player Audio Settings")]
    public AudioClip[] playerSounds;
    private float stepTimer = 0f;
    public float stepInterval = 0.35f;


    void Start()
    {
        prb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    void Update()
    {
        GetDamage(1f);
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
            anim.SetTrigger("Tumble");

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
        UpdateHealthUI();
    }
    void UpdateHealthUI()
    {
        float healthFraction = currentHealth / maxHealth;

        foreach (GameObject barObj in healthBars)
        {
            Transform bar = barObj.transform;

            bool full = healthFraction > 0.5f;
            bool half = healthFraction > 0.2f && healthFraction <= 0.5f;

            bar.GetChild(0).gameObject.SetActive(full);
            bar.GetChild(1).gameObject.SetActive(half);
            bar.GetChild(2).gameObject.SetActive(!full && !half);
        }
    }

}
