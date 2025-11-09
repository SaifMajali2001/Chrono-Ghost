using UnityEngine;

// 2D / top-down Enemy AI
public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stoppingDistance = 5f;
    [SerializeField] private float retreatDistance = 3f;

    [Header("Combat")]
    [SerializeField] private float shootingRange = 10f;
    [SerializeField] private float timeBetweenShots = 2f;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    private Transform player;
    private float nextShotTime;
    private Enemy enemyComponent;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogWarning("Player not found! Make sure the player has the 'Player' tag.");
        }

        enemyComponent = GetComponent<Enemy>();
    }

    private void Update()
    {
        if (player == null) return;

        if (enemyComponent != null && enemyComponent.Health <= 0f)
        {
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > stoppingDistance)
        {
            MoveTowardsPlayer();
        }
        else if (distanceToPlayer < retreatDistance)
        {
            MoveAwayFromPlayer();
        }

        LookAtPlayer();

        if (distanceToPlayer <= shootingRange && Time.time >= nextShotTime)
        {
            if (enemyComponent == null || enemyComponent.Health > 0f)
            {
                Shoot();
                nextShotTime = Time.time + timeBetweenShots;
            }
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector2 current = transform.position;
        Vector2 target = player.position;
        Vector2 newPos = Vector2.MoveTowards(current, target, moveSpeed * Time.deltaTime);
        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
    }

    private void MoveAwayFromPlayer()
    {
        Vector2 dir = ((Vector2)transform.position - (Vector2)player.position).normalized;
        Vector2 newPos = (Vector2)transform.position + dir * moveSpeed * Time.deltaTime;
        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
    }

    private void LookAtPlayer()
    {
        Vector2 direction = (player.position - transform.position);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            Vector2 dir = (player.position - firePoint.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion rot = Quaternion.Euler(0f, 0f, angle);

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, rot);
            bullet.layer = gameObject.layer;
        }
        else
        {
            Debug.LogWarning("Bullet prefab or fire point not assigned!");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootingRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, retreatDistance);
    }
}