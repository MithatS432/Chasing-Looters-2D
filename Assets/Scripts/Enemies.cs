using UnityEngine;

public class Enemies : MonoBehaviour
{
    public float health;
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
            Destroy(gameObject, 1f);
        }
    }
}
