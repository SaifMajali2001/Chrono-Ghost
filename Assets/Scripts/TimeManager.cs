using UnityEngine;
using System.Collections;

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

    [Header("Hitstop Settings")]
    [Tooltip("How long the hitstop lasts (seconds, real-time)")]
    [SerializeField] private float defaultHitstopDuration = 0.1f;

    [Tooltip("Time scale to set during hitstop (0 = freeze, 1 = normal)")]
    [SerializeField] [Range(0f, 1f)] private float hitstopTimeScale = 0.05f;

    /// <summary>
    /// Triggers a hitstop. If duration is <= 0 the serialized default is used.
    /// </summary>
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
}