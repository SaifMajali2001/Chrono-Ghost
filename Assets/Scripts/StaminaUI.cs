using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Image fillImage;

    [Header("Color Settings")]
    [SerializeField] private Color fullStaminaColor = Color.green;
    [SerializeField] private Color emptyStaminaColor = Color.red;

    private TimeManager timeManager;

    private void Start()
    {
        timeManager = TimeManager.Instance;
        
        if (staminaSlider != null)
        {
            staminaSlider.value = 1f;
            UpdateFillColor(1f);
        }
    }

    private void Update()
    {
        if (timeManager != null && staminaSlider != null)
        {
            float staminaPercentage = timeManager.GetStaminaPercentage();
            staminaSlider.value = staminaPercentage;
            UpdateFillColor(staminaPercentage);
        }
    }

    private void UpdateFillColor(float percentage)
    {
        if (fillImage != null)
        {
            fillImage.color = Color.Lerp(emptyStaminaColor, fullStaminaColor, percentage);
        }
    }
}