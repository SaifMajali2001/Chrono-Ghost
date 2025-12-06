using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeNotifier : MonoBehaviour
{
    private static UpgradeNotifier instance;
    public static UpgradeNotifier Instance
    {
        get
        {
            if (instance == null)
            {
                var existing = FindFirstObjectByType<UpgradeNotifier>();
                if (existing != null) instance = existing;
                else
                {
                    var go = new GameObject("UpgradeNotifier");
                    instance = go.AddComponent<UpgradeNotifier>();
                }
            }
            return instance;
        }
    }

    private Canvas canvas;
    private RectTransform container;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        CreateCanvasIfNeeded();
    }

    private void CreateCanvasIfNeeded()
    {
        if (canvas != null) return;

        var canvasGO = new GameObject("UpgradeNotifierCanvas");
        canvasGO.transform.SetParent(transform);
        canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10000;

        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasGO.AddComponent<GraphicRaycaster>();

        container = new GameObject("NotifierContainer").AddComponent<RectTransform>();
        container.SetParent(canvas.transform, false);
        container.anchorMin = new Vector2(0.5f, 1f);
        container.anchorMax = new Vector2(0.5f, 1f);
        container.pivot = new Vector2(0.5f, 1f);
        container.anchoredPosition = new Vector2(0f, -80f);
        container.sizeDelta = new Vector2(800f, 100f);
    }

    public void Show(string message, float duration = 2f)
    {
        CreateCanvasIfNeeded();
        StartCoroutine(ShowCoroutine(message, duration));
    }

    private IEnumerator ShowCoroutine(string message, float duration)
    {
        var go = new GameObject("UpgradeMessage");
        go.transform.SetParent(container, false);
        var rt = go.AddComponent<RectTransform>();
        // Make the background wide enough for most messages and slightly taller
        rt.sizeDelta = new Vector2(1000f, 80f);

        var image = go.AddComponent<Image>();
        image.color = new Color(0f, 0f, 0f, 0.75f);

        var textGO = new GameObject("Text");
        textGO.transform.SetParent(go.transform, false);
        var text = textGO.AddComponent<Text>();
        // Make text stretch to fill the background with padding
        var textRT = textGO.GetComponent<RectTransform>();
        textRT.anchorMin = new Vector2(0f, 0f);
        textRT.anchorMax = new Vector2(1f, 1f);
        textRT.offsetMin = new Vector2(18f, 8f); // left, bottom padding
        textRT.offsetMax = new Vector2(-18f, -8f); // right, top padding
        text.alignment = TextAnchor.MiddleCenter;
        // Use current Unity built-in font; LegacyRuntime.ttf is the supported runtime font
        Font builtin = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (builtin == null)
        {
            Debug.LogWarning("UpgradeNotifier: LegacyRuntime.ttf not found, falling back to default font.");
            // fallback to whatever Unity assigns by default
        }
        else
        {
            text.font = builtin;
        }
        text.text = message;
        text.color = Color.white;
        // Increase default font size and allow best-fit with larger max
        text.fontSize = 36;
        text.resizeTextForBestFit = true;
        text.resizeTextMinSize = 18;
        text.resizeTextMaxSize = 72;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Truncate;

        var canvasGroup = go.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;

        // Fade in
        float t = 0f;
        while (t < 0.15f)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Clamp01(t / 0.15f);
            yield return null;
        }

        // Wait
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        // Fade out
        t = 0f;
        while (t < 0.3f)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = 1f - Mathf.Clamp01(t / 0.3f);
            yield return null;
        }

        Destroy(go);
    }
}
