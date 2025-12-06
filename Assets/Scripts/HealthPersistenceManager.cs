using UnityEngine;

public class HealthPersistenceManager : MonoBehaviour
{
    private static HealthPersistenceManager _instance;

    public static HealthPersistenceManager Instance {
        get { return _instance; }
    }

    // Stored health state
    public float storedCurrentHealth { get; set; }
    public float storedMaxHealth { get; set; }
    public int storedShields { get; set; }

    private void Awake()
    {
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize with defaults only if not already set
        if (storedMaxHealth == 0f) storedMaxHealth = 100f;
        if (storedCurrentHealth == 0f) storedCurrentHealth = 100f;
        // Shields can be zero by default
    }

    private void OnDestroy()
    {
        // Clean up singleton reference when destroyed
        if (_instance == this)
        {
            _instance = null;
        }
    }

    public void SaveHealth(float currentHealth, float maxHealth, int shields)
    {
        storedCurrentHealth = currentHealth;
        storedMaxHealth = maxHealth;
        storedShields = shields;
        Debug.Log($"HealthPersistenceManager: Saved health={currentHealth}/{maxHealth}, shields={shields}");
    }

    public void RestoreHealth(PlayerHealth playerHealth)
    {
        if (playerHealth != null)
        {
            playerHealth.SetCurrentHealth(storedCurrentHealth);
            playerHealth.shieldCount = storedShields;
            Debug.Log($"HealthPersistenceManager: Restored health={storedCurrentHealth}, shields={storedShields}");
        }
    }
}