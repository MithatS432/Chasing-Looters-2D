using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public Collider2D attackCollider;
    private float damage = 10f;
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
        Allies allies = other.GetComponent<Allies>();
        if (allies != null)
        {
            allies.GetDamage(damage);
        }

        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.GetDamage(damage);
        }
    }
}
