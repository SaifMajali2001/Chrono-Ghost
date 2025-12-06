using UnityEngine;

public enum UpgradeType
{
    Shield,
    SlowMotionSpeed,
    ExtraStamina
}

[RequireComponent(typeof(Collider2D))]
public class UpgradePickup : MonoBehaviour
{
    [SerializeField] private UpgradeType upgradeType;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float floatHeight = 0.5f;
    [SerializeField] private float floatSpeed = 2f;

    private Vector3 startPos;
    private bool hasBeenPickedUp = false;

    private void Start()
    {
        startPos = transform.position;
        var col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
    }

    private void Update()
    {
        // Float animation
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasBeenPickedUp) return;
        if (!other.CompareTag("Player")) return;

        hasBeenPickedUp = true;
        ApplyUpgrade(other.gameObject);
        Destroy(gameObject);
    }

    public void SetRandomType()
    {
        int randomType = Random.Range(0, 3);
        upgradeType = (UpgradeType)randomType;
    }

    private void ApplyUpgrade(GameObject player)
    {
        switch (upgradeType)
        {
            case UpgradeType.Shield:
                ApplyShieldUpgrade(player);
                break;
            case UpgradeType.SlowMotionSpeed:
                ApplySlowMotionSpeedUpgrade(player);
                break;
            case UpgradeType.ExtraStamina:
                ApplyExtraStaminaUpgrade(player);
                break;
        }
    }

    private void ApplyShieldUpgrade(GameObject player)
    {
        var playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.AddShields(1);
            Debug.Log("Player gained 1 Shield!");
            UpgradeNotifier.Instance?.Show("Upgrade: Shield");
        }
    }

    private void ApplySlowMotionSpeedUpgrade(GameObject player)
    {
        var timeManager = TimeManager.Instance;
        if (timeManager != null)
        {
            // Increase the move speed multiplier while in slow motion (default is 1.25)
            float currentMultiplier = timeManager.SlowMotionMoveMultiplier;
            timeManager.SetSlowMotionMoveMultiplier(currentMultiplier + 0.5f);
            Debug.Log("Player slow motion move speed increased! New multiplier: " + (currentMultiplier + 0.5f));
            UpgradeNotifier.Instance?.Show("Upgrade: Slow-Motion Speed");
        }
    }

    private void ApplyExtraStaminaUpgrade(GameObject player)
    {
        var timeManager = TimeManager.Instance;
        if (timeManager != null)
        {
            timeManager.AddMaxStamina(30f);
            Debug.Log("Player gained +30 max stamina for slow motion!");
            UpgradeNotifier.Instance?.Show("Upgrade: +Stamina");
        }
    }
}
