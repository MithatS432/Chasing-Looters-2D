using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && other.TryGetComponent<Enemies>(out var enemies))
        {
            enemies.GetDamage(damage);
        }
    }

}
