using UnityEngine;
using System.Collections;

public class ForPeasent : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private AudioSource deathSound;
    private SpriteRenderer[] spriteRenderers;

    public float health = 50f;
    public GameObject arrowPrefab;
    public Transform firePoint;
    public float attackCooldown = 1.5f;
    private float attackTimer = 0f;

    public float detectionRange = 15f;
    public float attackRange = 10f;
    public float speed = 3f;
    public float followDistance = 1.5f;

    private Transform target;
    private Transform player;
    private Transform mainHouse;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        deathSound = GetComponent<AudioSource>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        mainHouse = GameObject.FindGameObjectWithTag("Main House").transform;
    }

    void Update()
    {
        if (health <= 0) return;
        attackTimer -= Time.deltaTime;

        FindTarget();

        if (target != null)
        {
            float distance = Vector2.Distance(transform.position, target.position);

            foreach (var sr in spriteRenderers)
                sr.flipX = target.position.x < transform.position.x;

            if (distance > attackRange && distance <= detectionRange)
            {
                Vector2 dir = (target.position - transform.position).normalized;
                rb.linearVelocity = dir * speed;
                anim.SetFloat("Speed", rb.linearVelocity.magnitude);
            }
            else if (distance <= attackRange)
            {
                rb.linearVelocity = Vector2.zero;
                anim.SetFloat("Speed", 0);

                if (attackTimer <= 0f)
                {
                    Attack();
                    attackTimer = attackCooldown;
                }

                if (distance < attackRange * 0.5f)
                {
                    Vector2 dir = (transform.position - target.position).normalized;
                    rb.linearVelocity = dir * speed;
                    anim.SetFloat("Speed", rb.linearVelocity.magnitude);
                }
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
                anim.SetFloat("Speed", 0);
            }
        }
        else
        {
            MoveToDefaultTarget();
        }
    }

    void MoveToDefaultTarget()
    {
        Transform targetFollow = (Vector2.Distance(transform.position, player.position) > followDistance) ? player : mainHouse;
        Vector2 dir = (targetFollow.position - transform.position).normalized;
        rb.linearVelocity = dir * speed;
        anim.SetFloat("Speed", rb.linearVelocity.magnitude);

        foreach (var sr in spriteRenderers)
            sr.flipX = targetFollow.position.x < transform.position.x;
    }

    void Attack()
    {
        if (health <= 0) return;

        anim.SetTrigger("Attack");

        StartCoroutine(ShootArrowWithDelay(0.5f));
    }
    private IEnumerator ShootArrowWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (arrowPrefab != null && firePoint != null && target != null && health > 0)
        {
            GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
            arrow.GetComponent<Arrow>().Initialize(target);
        }
    }


    void FindTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float minDist = Mathf.Infinity;
        Transform closest = null;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist < minDist && dist <= detectionRange)
            {
                minDist = dist;
                closest = enemy.transform;
            }
        }

        target = closest;
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
