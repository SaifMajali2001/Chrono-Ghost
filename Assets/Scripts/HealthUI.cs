using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image fillImage;

    [Header("Colors")]
    [SerializeField] private Color fullHealthColor = Color.green;
    [SerializeField] private Color emptyHealthColor = Color.red;

    private PlayerHealth playerHealth;

    private void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }

        if (healthSlider != null)
        {
            healthSlider.minValue = 0f;
            healthSlider.maxValue = 1f;
            healthSlider.value = 1f;
            UpdateFillColor(1f);
        }
    }

    private void Update()
    {
        if (playerHealth == null || healthSlider == null) return;

        float pct = Mathf.Clamp01(playerHealth.GetHealthPercentage());
        healthSlider.value = pct;
        UpdateFillColor(pct);
    }

    private void UpdateFillColor(float pct)
    {
        if (fillImage != null)
            fillImage.color = Color.Lerp(emptyHealthColor, fullHealthColor, pct);
    }
}
