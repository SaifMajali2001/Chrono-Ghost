using UnityEngine;

public class FlashEffect : MonoBehaviour
{
    private SpriteRenderer[] spriteRenderers;
    private Color[] originalColors;
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashDuration = 0.1f;

    void Awake()
    {
        // Collect all SpriteRenderers on this GameObject and its children
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
        if (spriteRenderers != null && spriteRenderers.Length > 0)
        {
            originalColors = new Color[spriteRenderers.Length];
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                originalColors[i] = spriteRenderers[i].color;
            }
        }
    }

    public void Flash()
    {
        Flash(flashDuration);
    }

    public void Flash(float duration)
    {
        if (spriteRenderers != null && spriteRenderers.Length > 0)
        {
            StopAllCoroutines();
            StartCoroutine(FlashRoutine(duration));
        }
    }

    private System.Collections.IEnumerator FlashRoutine(float duration)
    {
        // Set all renderers to flash color
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] != null)
                spriteRenderers[i].color = flashColor;
        }

        yield return new WaitForSecondsRealtime(duration);

        // Restore original colors
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] != null && originalColors != null && i < originalColors.Length)
                spriteRenderers[i].color = originalColors[i];
        }
    }
}