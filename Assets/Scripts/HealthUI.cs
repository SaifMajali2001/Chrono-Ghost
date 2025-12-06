using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image fillImage;
    [SerializeField] private Transform shieldIconContainer;

    [Header("Colors")]
    [SerializeField] private Color fullHealthColor = Color.green;
    [SerializeField] private Color emptyHealthColor = Color.red;

    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = false;

    private PlayerHealth playerHealth;
    private int lastShieldCount = -1;
    private float lastHealthPercentage = -1f;
    private bool playerAcquired = false;

    private void Awake()
    {
        // Register for scene loaded events
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Deferred initialization - waits one frame to ensure player exists
    private IEnumerator Start()
    {
        // Wait one frame for all objects in the scene to initialize
        yield return null;
        
        AcquirePlayer();
        InitializeSlider();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Re-acquire player when scene loads
        playerAcquired = false;
        playerHealth = null;
        lastShieldCount = -1;
        lastHealthPercentage = -1f;
        
        StartCoroutine(DelayedAcquirePlayer());
    }

    private IEnumerator DelayedAcquirePlayer()
    {
        yield return null;
        AcquirePlayer();
        InitializeSlider();
    }

    private void AcquirePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
            
            if (playerHealth != null)
            {
                playerAcquired = true;
                
                if (enableDebugLogs)
                {
                    Debug.Log($"[HealthUI] Successfully found player and PlayerHealth component");
                }
            }
            else
            {
                Debug.LogError($"[HealthUI] Player found but has no PlayerHealth component!");
            }
        }
        else
        {
            Debug.LogError($"[HealthUI] Player not found! Make sure player has 'Player' tag.");
        }
    }

    private void InitializeSlider()
    {
        if (healthSlider != null)
        {
            healthSlider.minValue = 0f;
            healthSlider.maxValue = 1f;
            healthSlider.value = 1f;
            UpdateFillColor(1f);
            
            // Force canvas update for builds
            Canvas.ForceUpdateCanvases();
            
            if (enableDebugLogs)
            {
                Debug.Log($"[HealthUI] Slider initialized");
            }
        }

        UpdateShieldDisplay();
    }

    private void Update()
    {
        // Defensive player reference check with re-acquisition
        if (playerHealth == null)
        {
            if (!playerAcquired)
            {
                AcquirePlayer();
            }
            
            if (playerHealth == null || healthSlider == null)
            {
                return;
            }
        }

        // Get current health percentage
        float currentPct = Mathf.Clamp01(playerHealth.GetHealthPercentage());

        // Only update if health percentage changed (avoid unnecessary updates)
        if (Mathf.Abs(currentPct - lastHealthPercentage) > 0.001f)
        {
            healthSlider.value = currentPct;
            UpdateFillColor(currentPct);
            
            // CRITICAL: Force canvas update for builds
            Canvas.ForceUpdateCanvases();
            
            lastHealthPercentage = currentPct;
            
            if (enableDebugLogs)
            {
                Debug.Log($"[HealthUI] Health updated: {currentPct:P0}");
            }
        }

        // Update shield icons if shield count changed
        if (playerHealth.shieldCount != lastShieldCount)
        {
            UpdateShieldDisplay();
            lastShieldCount = playerHealth.shieldCount;
            
            if (enableDebugLogs)
            {
                Debug.Log($"[HealthUI] Shield count updated: {playerHealth.shieldCount}");
            }
        }
    }

    private void UpdateFillColor(float pct)
    {
        if (fillImage != null)
        {
            fillImage.color = Color.Lerp(emptyHealthColor, fullHealthColor, pct);
        }
    }

    private void UpdateShieldDisplay()
    {
        if (playerHealth == null) return;

        if (shieldIconContainer == null)
        {
            // Auto-create container if not assigned
            GameObject go = new GameObject("ShieldIcons");
            go.transform.SetParent(healthSlider?.transform.parent);
            
            if (healthSlider != null)
            {
                go.transform.SetSiblingIndex(healthSlider.transform.GetSiblingIndex() + 1);
            }
            
            shieldIconContainer = go.transform;
            
            // Add layout component for proper positioning
            var layoutGroup = go.AddComponent<HorizontalLayoutGroup>();
            layoutGroup.spacing = 10f;
            layoutGroup.childAlignment = TextAnchor.MiddleLeft;
            layoutGroup.childControlWidth = false;
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childForceExpandHeight = false;
        }

        // Clear existing shield icons
        foreach (Transform child in shieldIconContainer)
        {
            Destroy(child.gameObject);
        }

        // Create one icon per shield
        for (int i = 0; i < playerHealth.shieldCount; i++)
        {
            GameObject iconGO = new GameObject($"ShieldIcon_{i}");
            iconGO.transform.SetParent(shieldIconContainer);
            
            RectTransform rt = iconGO.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(50f, 50f);
            rt.anchoredPosition = new Vector2(i * 60f, 0f);

            Image image = iconGO.AddComponent<Image>();
            image.color = new Color(0.2f, 0.8f, 1f, 1f);

            Outline outline = iconGO.AddComponent<Outline>();
            outline.effectColor = new Color(0.0f, 0.6f, 1f, 1f);
            outline.effectDistance = new Vector2(3f, -3f);
        }

        // CRITICAL: Force layout rebuild and canvas update for dynamically created UI
        if (shieldIconContainer != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(shieldIconContainer.GetComponent<RectTransform>());
        }
        
        Canvas.ForceUpdateCanvases();
    }

    // Public method to force an immediate health update (useful for testing)
    public void ForceHealthUpdate()
    {
        if (playerHealth != null && healthSlider != null)
        {
            float pct = Mathf.Clamp01(playerHealth.GetHealthPercentage());
            healthSlider.value = pct;
            UpdateFillColor(pct);
            Canvas.ForceUpdateCanvases();
            
            if (enableDebugLogs)
            {
                Debug.Log($"[HealthUI] Forced health update: {pct:P0}");
            }
        }
    }
}