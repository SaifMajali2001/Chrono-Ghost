using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [Header("Boss Spiral Settings")]
    [SerializeField] private bool isBoss = false;
    [SerializeField] private GameObject spiralBulletPrefab;
    [SerializeField] private Transform spiralFirePoint;
    [SerializeField] private int bulletsPerWave = 12;
    [SerializeField] private float timeBetweenBullets = 0.05f;
    [SerializeField] private int bulletsPerBurst = 36;
    [SerializeField] private float spiralSpinPerBullet = 5f;
    [SerializeField] private float burstCooldown = 3f;

    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = false;

    private Transform player;
    private GameObject playerGameObject;
    private float nextShotTime;
    private Enemy enemyComponent;
    private Rigidbody2D rb;
    private Coroutine bossSpiralCoroutine = null;
    private bool playerAcquired = false;

    private void Awake()
    {
        enemyComponent = GetComponent<Enemy>();
        rb = GetComponent<Rigidbody2D>();
        
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        if (spiralFirePoint == null)
        {
            spiralFirePoint = firePoint;
        }

        // Register for scene loaded events to handle scene transitions
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Unregister from scene loaded events
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Deferred initialization - waits one frame to ensure all objects are spawned
    private IEnumerator Start()
    {
        // Wait one frame for all objects in the scene to initialize
        yield return null;
        
        AcquirePlayer();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Re-acquire player when scene loads (handles scene transitions)
        playerAcquired = false;
        player = null;
        playerGameObject = null;
        AcquirePlayer();
    }

    private void AcquirePlayer()
    {
        playerGameObject = GameObject.FindGameObjectWithTag("Player");
        
        if (playerGameObject != null)
        {
            player = playerGameObject.transform;
            playerAcquired = true;
            
            if (enableDebugLogs)
            {
                Debug.Log($"[Frame {Time.frameCount}][EnemyAI-{gameObject.name}] Successfully found player: {playerGameObject.name} at position {player.position}");
            }
        }
        else
        {
            Debug.LogError($"[Frame {Time.frameCount}][EnemyAI-{gameObject.name}] CRITICAL: Player not found! Scene: {gameObject.scene.name}. Make sure player has 'Player' tag.");
            
            // Log all tagged objects for debugging
            if (enableDebugLogs)
            {
                LogAllTaggedObjects();
            }
        }
    }

    private void LogAllTaggedObjects()
    {
        Debug.Log($"[EnemyAI-{gameObject.name}] Listing all tagged objects in scene:");
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (!string.IsNullOrEmpty(obj.tag) && obj.tag != "Untagged")
            {
                Debug.Log($"  - {obj.name} has tag: '{obj.tag}'");
            }
        }
    }

    private void Update()
    {
        // Defensive player reference check with re-acquisition
        if (player == null || playerGameObject == null)
        {
            if (!playerAcquired)
            {
                AcquirePlayer();
            }
            
            // If still null after attempt, exit early
            if (player == null)
            {
                return;
            }
        }

        // Check if enemy is dead
        if (enemyComponent != null && enemyComponent.Health <= 0f)
        {
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Movement logic
        if (distanceToPlayer > stoppingDistance)
        {
            MoveTowardsPlayer();
        }
        else if (distanceToPlayer < retreatDistance)
        {
            MoveAwayFromPlayer();
        }

        LookAtPlayer();

        // Combat logic
        if (distanceToPlayer <= shootingRange)
        {
            if (enemyComponent == null || enemyComponent.Health > 0f)
            {
                if (isBoss)
                {
                    if (bossSpiralCoroutine == null && spiralBulletPrefab != null && spiralFirePoint != null)
                    {
                        bossSpiralCoroutine = StartCoroutine(BossSpiralBurst());
                    }
                }
                else
                {
                    if (Time.time >= nextShotTime)
                    {
                        Shoot();
                        nextShotTime = Time.time + timeBetweenShots;
                    }
                }
            }
        }
    }

    private void MoveTowardsPlayer()
    {
        if (player == null) return;

        Vector2 current = transform.position;
        Vector2 target = player.position;
        Vector2 newPos = Vector2.MoveTowards(current, target, moveSpeed * Time.deltaTime);
        
        if (rb != null)
        {
            rb.MovePosition(newPos);
        }
        else
        {
            transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
        }
    }

    private void MoveAwayFromPlayer()
    {
        if (player == null) return;

        Vector2 dir = ((Vector2)transform.position - (Vector2)player.position).normalized;
        Vector2 newPos = (Vector2)transform.position + dir * moveSpeed * Time.deltaTime;
        
        if (rb != null)
        {
            rb.MovePosition(newPos);
        }
        else
        {
            transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
        }
    }

    private void LookAtPlayer()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = direction.x < 0f;
        }
    }

    private void Shoot()
    {
        if (player == null)
        {
            Debug.LogWarning($"[EnemyAI-{gameObject.name}] Cannot shoot, player reference is null!");
            return;
        }

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
            Debug.LogWarning($"[EnemyAI-{gameObject.name}] Bullet prefab or fire point not assigned!");
        }
    }

    private IEnumerator BossSpiralBurst()
    {
        float currentAngle = Random.Range(0f, 360f);
        float baseStep = 360f / Mathf.Max(1, bulletsPerWave);

        for (int i = 0; i < bulletsPerBurst; i++)
        {
            float angle = currentAngle + (i % bulletsPerWave) * baseStep + i * spiralSpinPerBullet;
            SpawnSpiralBullet(angle);
            yield return new WaitForSecondsRealtime(timeBetweenBullets);
        }

        yield return new WaitForSecondsRealtime(burstCooldown);

        bossSpiralCoroutine = null;
    }

    private void SpawnSpiralBullet(float angle)
    {
        if (spiralBulletPrefab == null || spiralFirePoint == null) return;

        Quaternion rot = Quaternion.Euler(0f, 0f, angle);
        GameObject b = Instantiate(spiralBulletPrefab, spiralFirePoint.position, rot);
        b.layer = gameObject.layer;
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