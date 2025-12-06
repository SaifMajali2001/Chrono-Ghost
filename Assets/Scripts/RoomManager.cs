using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class RoomManager : MonoBehaviour
{
    [System.Serializable]
    public class SpawnPoint
    {
        public Transform position;
        public GameObject enemyPrefab;
    }

    [Header("Spawn Settings")]
    [SerializeField] private List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
    [SerializeField] private bool spawnOnce = true;
    [SerializeField] private int roomID = 0;
    [SerializeField] private GameObject upgradePickupPrefab;

    [Header("Trigger Settings")]
    [SerializeField] private Vector2 roomSize = new Vector2(10f, 10f);
    [SerializeField] private bool showGizmos = true;

    private bool hasSpawned = false;
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"RoomManager (ID={roomID}) OnTriggerEnter2D by '{other.name}' (tag='{other.tag}'). hasSpawned={hasSpawned}, spawnOnce={spawnOnce}");
        if (!other.CompareTag("Player")) return;

        if (!hasSpawned || !spawnOnce)
        {
            SpawnEnemies();
            hasSpawned = true;
        }
    }

    private void SpawnEnemies()
    {
        ClearEnemies();

        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogWarning($"RoomManager (ID={roomID}): No spawnPoints configured.");
        }

        int created = 0;
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            var point = spawnPoints[i];
            if (point.position == null)
            {
                Debug.LogWarning($"RoomManager (ID={roomID}): spawnPoints[{i}].position is null.");
                continue;
            }
            if (point.enemyPrefab == null)
            {
                Debug.LogWarning($"RoomManager (ID={roomID}): spawnPoints[{i}].enemyPrefab is null.");
                continue;
            }

            GameObject enemy = Instantiate(point.enemyPrefab,
                point.position.position,
                point.position.rotation);
            spawnedEnemies.Add(enemy);
            created++;
            Debug.Log($"RoomManager (ID={roomID}): Spawned '{point.enemyPrefab.name}' at spawnPoints[{i}] ({point.position.position}).");
        }
        if (created == 0)
        {
            Debug.LogWarning($"RoomManager (ID={roomID}): No enemies were spawned (check spawn point assignments).\nTotal spawnPoints={spawnPoints.Count}.");
        }

        // Start monitoring for room clear when enemies are spawned
        Debug.Log($"RoomManager (ID={roomID}) started with {spawnedEnemies.Count} spawned enemies.");
        StartCoroutine(CheckForClear());
    }

    public void ClearEnemies()
    {
        foreach (var enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        spawnedEnemies.Clear();
    }

    private IEnumerator CheckForClear()
    {
        while (true)
        {
            // Remove destroyed/null enemies from the list
            spawnedEnemies.RemoveAll(e => e == null);
            Debug.Log($"RoomManager (ID={roomID}) checking clear: {spawnedEnemies.Count} enemies remaining.");
            if (spawnedEnemies.Count == 0)
            {
                RoomCleared();
                yield break;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void RoomCleared()
    {
        Debug.Log($"RoomManager (ID={roomID}) cleared.");
        if (roomID == 2)
        {
            if (upgradePickupPrefab == null)
            {
                Debug.LogWarning($"RoomManager: upgradePickupPrefab is not assigned on Room ID {roomID}.");
                return;
            }

            // Spawn a bit above the room center so it's visible
            Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
            GameObject go = Instantiate(upgradePickupPrefab, spawnPos, Quaternion.identity);
            var pickup = go.GetComponent<UpgradePickup>();
            if (pickup != null)
            {
                pickup.SetRandomType();
                Debug.Log($"Spawned UpgradePickup of type {pickup} at {spawnPos} for Room {roomID}.");
            }
            else
            {
                Debug.LogWarning("Spawned upgrade prefab does not contain UpgradePickup component.");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(roomSize.x, roomSize.y, 1f));

        Gizmos.color = Color.red;
        foreach (var point in spawnPoints)
        {
            if (point.position != null)
            {
                Gizmos.DrawWireSphere(point.position.position, 0.5f);
            }
        }
    }
}