using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    enum SpawnerType { Straight, Spin }

    [Header("Bullet Attributes")]
    public GameObject bulletPrefab;
    public float bulletLife = 1f;
    public float Speed = 1f;

    [Header("Spawner Attributes")]
    [SerializeField] private SpawnerType spawnerType;
    [SerializeField] private float fireRate = 1f;

    private GameObject spawnedBullet;
    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;
        if (spawnerType == SpawnerType.Spin)
        {
            transform.eulerAngles = new Vector3(0f, 0f, transform.eulerAngles.z + 1f);
        }
        if (timer >= fireRate)
        {
            Fire();
            timer = 0;
        }

    }
    
    private void Fire()
    {
        if(bulletPrefab)
        {
            // Instantiate using the spawner's rotation so the prefab's art/forward matches
            spawnedBullet = Instantiate(bulletPrefab, transform.position, transform.rotation);

            // Preserve prefab's local scale (in case instantiating changed it)
            spawnedBullet.transform.localScale = bulletPrefab.transform.localScale;

            // Set bullet properties on the Bullet component if it exists
            Bullet bulletComp = spawnedBullet.GetComponent<Bullet>();
            if (bulletComp != null)
            {
                bulletComp.speed = Speed;
                bulletComp.bulletLife = bulletLife;
            }
        }
    }
}
