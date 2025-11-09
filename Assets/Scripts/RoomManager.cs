using UnityEngine;
using System.Collections.Generic;

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

    [Header("Trigger Settings")]
    [SerializeField] private Vector2 roomSize = new Vector2(10f, 10f);
    [SerializeField] private bool showGizmos = true;

    private bool hasSpawned = false;
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    private void OnTriggerEnter2D(Collider2D other)
    {
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

        foreach (var point in spawnPoints)
        {
            if (point.position != null && point.enemyPrefab != null)
            {
                GameObject enemy = Instantiate(point.enemyPrefab, 
                    point.position.position, 
                    point.position.rotation);
                spawnedEnemies.Add(enemy);
            }
        }
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