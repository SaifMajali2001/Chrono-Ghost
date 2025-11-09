using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class TimeManager : MonoBehaviour
{
    private static TimeManager instance;
    public static TimeManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Object.FindAnyObjectByType<TimeManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("TimeManager");
                    instance = go.AddComponent<TimeManager>();
                }
            }
            return instance;
        }
    }

    [Header("Time Slow Settings")]
    [SerializeField] [Range(0.1f, 1f)] private float slowMotionTimeScale = 0.5f;
    
    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaDrainRate = 30f;
    [SerializeField] private float staminaRechargeRate = 20f;
    [SerializeField] private float staminaRechargeDelay = 1f;

    private float currentStamina;
    private bool isSlowMotionActive;
    private float lastSlowMotionTime;

    [Header("Hitstop Settings")]
    [SerializeField] private float defaultHitstopDuration = 0.1f;

    [SerializeField] [Range(0f, 1f)] private float hitstopTimeScale = 0.05f;

    public void DoHitstop(float duration = -1f)
    {
        if (duration <= 0f)
            duration = defaultHitstopDuration;

        StartCoroutine(Hitstop(duration));
    }

    private IEnumerator Hitstop(float duration)
    {
        float previousTimeScale = Time.timeScale;
        Time.timeScale = hitstopTimeScale;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = previousTimeScale;
    }

    private void Awake()
    {
        currentStamina = maxStamina;
    }

    private void Update()
    {
        bool slowMotionInput = Keyboard.current.leftShiftKey.isPressed;
        
        if (slowMotionInput && currentStamina > 0)
        {
            ActivateSlowMotion();
        }
        else
        {
            DeactivateSlowMotion();
        }

        UpdateStamina();
    }

    private void ActivateSlowMotion()
    {
        if (!isSlowMotionActive)
        {
            Time.timeScale = slowMotionTimeScale;
            isSlowMotionActive = true;
        }
    }

    private void DeactivateSlowMotion()
    {
        if (isSlowMotionActive)
        {
            Time.timeScale = 1f;
            isSlowMotionActive = false;
            lastSlowMotionTime = Time.unscaledTime;
        }
    }

    private void UpdateStamina()
    {
        if (isSlowMotionActive)
        {
            currentStamina = Mathf.Max(0f, currentStamina - staminaDrainRate * Time.unscaledDeltaTime);
            
            if (currentStamina <= 0f)
            {
                DeactivateSlowMotion();
            }
        }
        else if (Time.unscaledTime >= lastSlowMotionTime + staminaRechargeDelay)
        {
            currentStamina = Mathf.Min(maxStamina, currentStamina + staminaRechargeRate * Time.unscaledDeltaTime);
        }
    }

    public float GetStaminaPercentage()
    {
        return currentStamina / maxStamina;
    }
}