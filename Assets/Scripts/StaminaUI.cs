using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Image fillImage;
    [SerializeField] private LayoutElement layoutElement;

    [Header("Color Settings")]
    [SerializeField] private Color fullStaminaColor = Color.green;
    [SerializeField] private Color emptyStaminaColor = Color.red;

    [Header("Scaling")]
    [SerializeField] private float baseWidth = 200f;
    [SerializeField] private float maxWidth = 400f;

    private TimeManager timeManager;
    private float initialMaxStamina = 100f;

    private void Start()
    {
        timeManager = TimeManager.Instance;
        
        if (staminaSlider != null)
        {
            staminaSlider.value = 1f;
            UpdateFillColor(1f);
            initialMaxStamina = timeManager != null ? timeManager.GetMaxStamina() : 100f;
        }

        if (layoutElement == null)
        {
            layoutElement = GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = gameObject.AddComponent<LayoutElement>();
            }
        }
        layoutElement.preferredWidth = baseWidth;
    }

    private void Update()
    {
        if (timeManager != null && staminaSlider != null)
        {
            float staminaPercentage = timeManager.GetStaminaPercentage();
            staminaSlider.value = staminaPercentage;
            UpdateFillColor(staminaPercentage);

            // Scale bar based on current max stamina
            float currentMaxStamina = timeManager.GetMaxStamina();
            float staminaRatio = currentMaxStamina / initialMaxStamina;
            float newWidth = Mathf.Lerp(baseWidth, maxWidth, staminaRatio - 1f);
            layoutElement.preferredWidth = Mathf.Max(baseWidth, newWidth);
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