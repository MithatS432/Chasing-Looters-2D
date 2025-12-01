using System;
using UnityEngine;

public class ForEnemyBoss : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private AudioSource deathSound;
    private SpriteRenderer sp;

    public float health = 300f;
    public float speed = 3f;

    public float attackCooldown = 1.5f;
    private float attackTimer = 0f;
    private Transform target;

    public event Action onDeath;
    private BossAttackTrigger bossAttack;

    public GameObject bloodEffectPrefab;

    public float attackRange = 3f;
    private bool isAttacking = false;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        deathSound = GetComponent<AudioSource>();
        sp = GetComponent<SpriteRenderer>();
        bossAttack = GetComponentInChildren<BossAttackTrigger>(); // <-- Düzeltilen kısım

        if (bossAttack == null)
            Debug.LogWarning($"{name} has no BossAttackTrigger component in children!");

        FindTarget();
    }

    void Update()
    {
        if (health <= 0) return;

        attackTimer += Time.deltaTime;

        if (target == null)
        {
            FindTarget();
            return;
        }

        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        if (distanceToTarget <= attackRange && attackTimer >= attackCooldown)
        {
            Attack();
            attackTimer = 0f;
        }

        MoveTowardsTarget();
    }


    void FindTarget()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject ally = GameObject.FindGameObjectWithTag("Ally");
        GameObject tower = GameObject.FindGameObjectWithTag("Tower");
        GameObject mainHouse = GameObject.FindGameObjectWithTag("Main House");

        if (player != null) target = player.transform;
        else if (ally != null) target = ally.transform;
        else if (tower != null) target = tower.transform;
        else if (mainHouse != null) target = mainHouse.transform;
    }

    void MoveTowardsTarget()
    {
        if (isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 direction = (target.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;

        sp.flipX = target.position.x < transform.position.x;
    }


    void Attack()
    {
        isAttacking = true;
        anim.SetTrigger("Attack");

        if (bossAttack != null)
            bossAttack.EnableForSeconds(0.5f);

        Invoke(nameof(ResetAttack), 0.5f);
    }

    void ResetAttack()
    {
        isAttacking = false;
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

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        anim.SetTrigger("Die");
        if (deathSound != null) deathSound.Play();
        onDeath?.Invoke();
        rb.linearVelocity = Vector2.zero; // Ölürken hareketi durdur
        if (bossAttack != null) bossAttack.gameObject.SetActive(false); // Collider kapat
        Destroy(gameObject, 1f);
    }
}
