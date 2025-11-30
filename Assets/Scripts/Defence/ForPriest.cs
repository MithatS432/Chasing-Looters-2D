using UnityEngine;

public class ForPriest : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private AudioSource deathSound;
    private SpriteRenderer[] spriteRenderers;

    [Header("Stats")]
    public float health = 30f;
    public float speed = 2f;

    [Header("Healing")]
    public float healRange = 5f;
    public float minHealDistance = 2f;
    public float healAmount = 5f;
    public float healCooldown = 1.5f;
    private float healTimer = 0f;

    private Transform player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        deathSound = GetComponent<AudioSource>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        healTimer -= Time.deltaTime;

        if (player != null)
        {
            // Player'ı her zaman takip et
            MoveTowards(player.position);

            Player playerScript = player.GetComponent<Player>();
            if (playerScript != null && playerScript.currentHealth < playerScript.maxHealth)
            {
                float distance = Vector2.Distance(transform.position, player.position);
                if (distance <= healRange && distance >= minHealDistance && healTimer <= 0f)
                {
                    anim.SetTrigger("Attack");
                    Heal(player);
                    healTimer = healCooldown;
                }
            }
            else
            {
                // Player full ise en düşük canlı ally'yi iyileştir
                HealLowestAlly();
            }
        }
    }

    void MoveTowards(Vector2 targetPos)
    {
        Vector2 dir = (targetPos - (Vector2)transform.position).normalized;
        rb.linearVelocity = dir * speed;

        if (dir.x != 0)
        {
            // Mevcut X scale'i bozmadan yön değiştir
            Vector3 localScale = transform.localScale;
            localScale.x = Mathf.Abs(localScale.x) * (dir.x > 0 ? 1 : -1);
            transform.localScale = localScale;

            // Tüm child sprite'ları da ters çevir
            foreach (SpriteRenderer sr in spriteRenderers)
            {
                sr.flipX = dir.x < 0;
            }
        }

        if (anim != null)
            anim.SetFloat("Speed", rb.linearVelocity.magnitude);
    }

    void HealLowestAlly()
    {
        GameObject[] allies = GameObject.FindGameObjectsWithTag("Ally");
        Transform lowestAlly = null;
        float minHealth = Mathf.Infinity;

        foreach (GameObject ally in allies)
        {
            Allies allyScript = ally.GetComponent<Allies>();
            if (allyScript != null && allyScript.health < allyScript.maxHealth)
            {
                if (allyScript.health < minHealth)
                {
                    minHealth = allyScript.health;
                    lowestAlly = ally.transform;
                }
            }
        }

        if (lowestAlly != null)
        {
            float distance = Vector2.Distance(transform.position, lowestAlly.position);
            if (distance <= healRange && distance >= minHealDistance && healTimer <= 0f)
            {
                anim.SetTrigger("Attack");
                Heal(lowestAlly);
                healTimer = healCooldown;
            }
        }
    }

    void Heal(Transform t)
    {
        if (t == null) return;

        Player playerScript = t.GetComponent<Player>();
        if (playerScript != null)
        {
            playerScript.currentHealth += healAmount;
            if (playerScript.currentHealth > playerScript.maxHealth)
                playerScript.currentHealth = playerScript.maxHealth;

            playerScript.UpdateHealthUI();
            return;
        }

        Allies allyScript = t.GetComponent<Allies>();
        if (allyScript != null)
        {
            allyScript.health += healAmount;
            if (allyScript.health > allyScript.maxHealth)
                allyScript.health = allyScript.maxHealth;
        }
    }

    public void GetDamage(float dmg)
    {
        health -= dmg;

        if (health > 0)
        {
            anim.SetTrigger("Hurt");
        }
        else
        {
            anim.SetTrigger("Die");
            if (deathSound != null) deathSound.Play();
            Destroy(gameObject, 0.5f);
        }
    }
}
