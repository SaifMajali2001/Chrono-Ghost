using UnityEngine;

public class FlashEffect : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashDuration = 0.1f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    public void Flash()
    {
        Flash(flashDuration);
    }

    public void Flash(float duration)
    {
        if (spriteRenderer != null)
        {
            StopAllCoroutines();
            StartCoroutine(FlashRoutine(duration));
        }
    }

    private System.Collections.IEnumerator FlashRoutine(float duration)
    {
        spriteRenderer.color = flashColor;
        yield return new WaitForSecondsRealtime(duration);
        spriteRenderer.color = originalColor;
    }
}