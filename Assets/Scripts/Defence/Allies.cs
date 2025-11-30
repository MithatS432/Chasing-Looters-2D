using UnityEngine;

public class Allies : MonoBehaviour
{
    private Rigidbody2D arb;
    private Animator aa;
    private SpriteRenderer[] allSprites;
    private AudioSource deathSound;
    private SpriteRenderer spriteRenderer;

    public float health;
    public float maxHealth;

    public float speed;
    public float attackRange;

    public float currentMode = 0f;
    private Transform player;
    private Transform mainHouse;
    public float followDistance = 1.5f;
    private bool isDead = false;
    private bool enemyInRange = false;
    public float attackCooldown = 1f;
    private float attackTimer = 0f;

    public float detectionRange = 15f;

    public AllyAttack allyAttack;



    void Start()
    {
        arb = GetComponent<Rigidbody2D>();
        aa = GetComponent<Animator>();
        deathSound = GetComponent<AudioSource>();
        allSprites = GetComponentsInChildren<SpriteRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        mainHouse = GameObject.FindGameObjectWithTag("Main House").transform;
        health = maxHealth;

    }

    void Update()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = GetClosestEnemy(enemies);

        if (closestEnemy != null)
        {
            float distanceToEnemy = Vector2.Distance(transform.position, closestEnemy.transform.position);

            if (distanceToEnemy <= detectionRange)
            {
                if (distanceToEnemy > followDistance)
                {
                    MoveTowards(closestEnemy.transform);
                }
                else
                {
                    arb.linearVelocity = Vector2.zero;
                    enemyInRange = true;
                }
            }
            else
            {
                enemyInRange = false;
                MoveToDefaultTarget();
            }
        }
        else
        {
            enemyInRange = false;
            MoveToDefaultTarget();
        }

        if (enemyInRange)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                aa.SetTrigger("Attack");
                allyAttack.EnableForSeconds(0.3f);
                attackTimer = attackCooldown;
            }
        }
    }


    GameObject GetClosestEnemy(GameObject[] enemies)
    {
        GameObject closest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = enemy;
            }
        }
        return closest;
    }

    void MoveTowards(Transform target)
    {
        float distance = Vector2.Distance(transform.position, target.position);

        if (distance <= followDistance)
        {
            arb.linearVelocity = Vector2.zero;
            aa.SetFloat("Speed", 0);
            return;
        }

        Vector2 direction = (target.position - transform.position).normalized;
        arb.linearVelocity = direction * speed;
        aa.SetFloat("Speed", arb.linearVelocity.magnitude);

        if (player != null)
        {
            bool flip = player.position.x < transform.position.x;
            foreach (var sr in allSprites)
                sr.flipX = flip;
        }
    }




    void MoveToDefaultTarget()
    {
        if (currentMode == 0)
            MoveTowards(player);
        else
            MoveTowards(mainHouse);
    }


    void FollowTarget(Transform target)
    {
        if (target == null) return;

        float distance = Vector2.Distance(transform.position, target.position);

        if (distance <= followDistance)
        {
            arb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 direction = (target.position - transform.position).normalized;
        arb.linearVelocity = direction * speed;
        aa.SetFloat("Speed", arb.linearVelocity.magnitude);

        if (player != null)
            spriteRenderer.flipX = player.position.x < transform.position.x;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
            enemyInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
            enemyInRange = false;
    }
    public void GetDamage(float dmg)
    {
        if (isDead)
            return;

        health -= dmg;

        if (health > 0)
        {
            aa.SetTrigger("Hurt");
            return;
        }

        isDead = true;
        aa.ResetTrigger("Hurt");
        aa.SetTrigger("Die");
        if (deathSound != null)
            deathSound.Play();

        Destroy(gameObject, 0.5f);
    }

}