using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Invincibility")]
    public float invincibilityDuration = 1f;
    public float invincibilityBlinkRate = 0.1f;
    
    [Header("Death / Feedback")]
    public Animator animator;
    public string hubSceneName = "Hub";
    public float restartDelay = 2f;

    private bool isDead = false;
    private bool isInvincible = false;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        currentHealth = maxHealth;
        if (animator == null)
            animator = GetComponent<Animator>();
        
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(float amount)
    {
        if (isDead || isInvincible) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(0f, currentHealth);


        if (currentHealth <= 0f)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityFrames());
        }
    }

    private IEnumerator InvincibilityFrames()
    {
        isInvincible = true;

        if (spriteRenderer != null)
        {
            float endTime = Time.time + invincibilityDuration;
            while (Time.time < endTime)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
                yield return new WaitForSeconds(invincibilityBlinkRate);
            }
            spriteRenderer.enabled = true;
        }
        else
        {
            yield return new WaitForSeconds(invincibilityDuration);
        }

        isInvincible = false;
    }

    public void Heal(float amount)
    {
        if (isDead) return;
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        var pc = GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.enabled = false;
        }

        var col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        StartCoroutine(RestartSequence());
    }

    private IEnumerator RestartSequence()
    {
        yield return new WaitForSeconds(restartDelay);

        SceneManager.LoadScene(hubSceneName);
    }

    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }

    public float CurrentHealth => currentHealth;
}
