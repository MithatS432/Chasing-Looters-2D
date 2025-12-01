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
    public float attackRange;

    public float attackCooldown = 1f;
    private float attackTimer = 0f;
    public event Action onDeath;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        deathSound = GetComponent<AudioSource>();
        sp = GetComponent<SpriteRenderer>();

    }

    void Update()
    {

    }

    public void GetDamage(float damage)
    {
        if (damage <= 0) return;
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }


    private void Die()
    {
        onDeath?.Invoke();
        Destroy(gameObject, 1f);
    }
}
