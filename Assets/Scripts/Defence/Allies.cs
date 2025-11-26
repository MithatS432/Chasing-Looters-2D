using UnityEngine;

public class Allies : MonoBehaviour
{
    private Rigidbody2D arb;
    private Animator aa;
    private SpriteRenderer[] allSprites;
    private AudioSource deathSound;
    private SpriteRenderer spriteRenderer;

    public float health;
    public float speed;
    public float attackRange;

    public float currentMode = 0f;
    private Transform player;
    private Transform mainHouse;

    void Start()
    {
        arb = GetComponent<Rigidbody2D>();
        aa = GetComponent<Animator>();
        deathSound = GetComponent<AudioSource>();
        allSprites = GetComponentsInChildren<SpriteRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        mainHouse = GameObject.FindGameObjectWithTag("Main House").transform;
    }

    void Update()
    {
        if (currentMode == 0)
        {
            FollowTarget(player);
        }
        else
        {
            FollowTarget(mainHouse);
        }
    }

    void FollowTarget(Transform target)
    {
        if (target == null) return;

        Vector2 direction = (target.position - transform.position).normalized;
        arb.linearVelocity = direction * speed;

        if (direction.x > 0)
            spriteRenderer.flipX = false;
        else if (direction.x < 0)
            spriteRenderer.flipX = true;
    }
}