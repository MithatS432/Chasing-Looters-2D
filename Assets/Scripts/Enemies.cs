using System;
using UnityEngine;

public class Enemies : MonoBehaviour
{
    public float health;

    public event Action onDeath;

    void Start()
    {

    }

    void Update()
    {

    }

    public void GetDamage(float damage)
    {
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
