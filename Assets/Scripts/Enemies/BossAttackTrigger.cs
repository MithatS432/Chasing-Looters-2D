using UnityEngine;

public class BossAttackTrigger : MonoBehaviour
{
    public Collider2D attackCollider;
    private float damage = 20f; // Boss saldırı hasarı
    public float damageInterval = 0.5f; // Her 0.5 saniyede bir hasar

    private void Start()
    {
        if (attackCollider != null)
            attackCollider.enabled = false;
    }

    public void EnableForSeconds(float time)
    {
        if (attackCollider == null) return;

        attackCollider.enabled = true;
        CancelInvoke(nameof(DisableCollider));
        Invoke(nameof(DisableCollider), time);
    }

    private void DisableCollider()
    {
        if (attackCollider != null)
            attackCollider.enabled = false;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!isInvoking)
        {
            isInvoking = true;
            StartCoroutine(DealDamageOverTime(other));
        }
    }

    private bool isInvoking = false;

    private System.Collections.IEnumerator DealDamageOverTime(Collider2D target)
    {
        while (attackCollider.enabled && target != null)
        {
            Player player = target.GetComponent<Player>();
            if (player != null) player.GetDamage(damage);

            Allies allies = target.GetComponent<Allies>();
            if (allies != null) allies.GetDamage(damage);

            MainHous mainHous = target.GetComponent<MainHous>();
            if (mainHous != null) mainHous.GetDamage(damage);

            Tower tower = target.GetComponent<Tower>();
            if (tower != null) tower.GetDamage(damage);

            yield return new WaitForSeconds(damageInterval);
        }
        isInvoking = false;
    }
}
