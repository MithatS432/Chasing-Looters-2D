using UnityEngine;

public class Tower : MonoBehaviour
{
    public float towerHealth;
    public float detectionRadius;
    public float fireRate;
    public GameObject arrowPrefab;

    private float fireTimer;

    public AudioClip destroySound;

    void Update()
    {
        fireTimer += Time.deltaTime;

        Collider2D target = FindClosestEnemy();
        if (target != null && fireTimer >= fireRate)
        {
            FireArrow(target.transform);
            fireTimer = 0f;
        }
        if (towerHealth <= 0)
        {
            Destroy(gameObject);
            AudioSource.PlayClipAtPoint(destroySound, transform.position);
        }
    }

    Collider2D FindClosestEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        Collider2D closest = null;
        float minDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                float d = Vector2.Distance(transform.position, hit.transform.position);
                if (d < minDist)
                {
                    minDist = d;
                    closest = hit;
                }
            }
        }

        return closest;
    }

    void FireArrow(Transform target)
    {
        GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
        arrow.GetComponent<Arrow>().Initialize(target);
    }
}
