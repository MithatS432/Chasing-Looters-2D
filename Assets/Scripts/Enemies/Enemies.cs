using System;
using UnityEngine;

public class Enemies : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private AudioSource deathSound;
    private SpriteRenderer sp;

    public float health;
    public float speed;

    [Header("Attack Ranges")]
    public float attackRange = 3f;        // Player
    public float allyAttackRange = 2.5f;  // Ally
    public float towerAttackRange = 4f;   // Tower
    public float houseAttackRange = 50f;  // Main House

    public float detectionRange = 5f;

    public float attackCooldown = 1f;
    private float attackTimer = 0f;
    private Transform target;

    public event Action onDeath;
    private EnemyAttack enemyAttack;
    public int coinReward;

    public GameObject bloodEffectPrefab;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        deathSound = GetComponent<AudioSource>();
        sp = GetComponent<SpriteRenderer>();
        enemyAttack = GetComponentInChildren<EnemyAttack>();

        if (enemyAttack == null)
            Debug.LogWarning($"{name} has no EnemyAttack component in children!");

        FindMainHouse();
    }

    void Update()
    {
        if (health <= 0) return;

        DetectNearbyEnemies();

        if (target != null)
        {
            MoveTowardsTarget();

            attackTimer += Time.deltaTime;

            // Hedef tipine göre attackRange belirle
            float currentAttackRange = attackRange;

            if (target.CompareTag("Ally"))
                currentAttackRange = allyAttackRange;
            else if (target.CompareTag("Tower"))
                currentAttackRange = towerAttackRange;
            else if (target.CompareTag("Main House"))
                currentAttackRange = houseAttackRange;

            if (Vector2.Distance(transform.position, target.position) <= currentAttackRange && attackTimer >= attackCooldown)
            {
                Attack();
                attackTimer = 0f;
            }
        }
    }

    void DetectNearbyEnemies()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRange);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player") || hit.CompareTag("Ally") || hit.CompareTag("Tower"))
            {
                target = hit.transform;
                return;
            }
        }

        FindMainHouse();
    }

    void FindMainHouse()
    {
        GameObject mainHouse = GameObject.FindGameObjectWithTag("Main House");
        if (mainHouse != null)
        {
            target = mainHouse.transform;
        }
    }

    void MoveTowardsTarget()
    {
        Vector2 direction = (target.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;

        if (target.position.x > transform.position.x)
        {
            sp.flipX = false;
        }
        else if (target.position.x < transform.position.x)
        {
            sp.flipX = true;
        }
    }


    void Attack()
    {
        anim.SetTrigger("Attack");
        if (enemyAttack != null)
            enemyAttack.EnableForSeconds(0.3f);
    }

    public void GetDamage(float damage)
    {
        if (damage <= 0) return;
        health -= damage;

        if (bloodEffectPrefab != null)
        {
            GameObject blood = Instantiate(bloodEffectPrefab, transform.position, Quaternion.identity);
            Destroy(blood, 1f);
        }

        if (health > 0)
        {

        }
        else
        {
            anim.SetTrigger("Die");
            if (deathSound != null) deathSound.Play();
            Die(); // Ödül ve event tetikleme
        }
    }


    private void Die()
    {
        Player player = UnityEngine.Object.FindAnyObjectByType<Player>();
        if (player != null)
        {
            player.GatherCoin(coinReward);
        }

        onDeath?.Invoke();
        Destroy(gameObject, 1f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
