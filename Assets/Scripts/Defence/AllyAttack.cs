using UnityEngine;

public class AllyAttack : MonoBehaviour
{
    public Collider2D attackCollider;
    public float damage = 15f;
    private void Start()
    {
        attackCollider.enabled = false;
    }

    public void EnableForSeconds(float time)
    {
        attackCollider.enabled = true;
        CancelInvoke(nameof(DisableCollider));
        Invoke(nameof(DisableCollider), time);
    }

    private void DisableCollider()
    {
        attackCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Enemies enemy = other.GetComponent<Enemies>();
        if (enemy != null)
        {
            enemy.GetDamage(damage);
        }
    }
}
