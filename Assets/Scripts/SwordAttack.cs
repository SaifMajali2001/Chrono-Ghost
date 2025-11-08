using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    Collider2D swordCollider;
    Vector2 rightAttackOffset;
    public float damage = 3;
    [Header("Attack Timing")]
    [Tooltip("Time from attack start to when the hit window opens (seconds, real-time)")]
    [SerializeField] private float hitDelay = 0.12f;
    [Tooltip("How long the hit collider stays active (seconds, real-time)")]
    [SerializeField] private float activeWindow = 0.08f;
    [Tooltip("Optional delay between collision and applying hit effects (seconds, real-time)")]
    [SerializeField] private float effectDelay = 0f;
    private Coroutine attackCoroutine;

    public void Awake()
    {
        swordCollider = GetComponent<Collider2D>();
        if (swordCollider != null)
            swordCollider.enabled = false;

        rightAttackOffset = transform.localPosition;
    }

    public void AttackRight()
    {
        StartAttack(true);
    }

    public void AttackLeft()
    {
        StartAttack(false);
    }

    public void StopAttack()
    {
        // Stop any running attack coroutine and ensure collider is off
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
        if (swordCollider != null)
            swordCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Apply hit effects after an optional effectDelay (real-time)
                StartCoroutine(DelayedApplyHitEffects(enemy));
            }
        }
    }

    private System.Collections.IEnumerator DelayedApplyHitEffects(Enemy enemy)
    {
        if (effectDelay > 0f)
            yield return new WaitForSecondsRealtime(effectDelay);

        // Apply damage
        enemy.Health -= damage;

        // Hitstop
        TimeManager.Instance.DoHitstop();

        // Flash
        FlashEffect flashEffect = enemy.GetComponent<FlashEffect>();
        if (flashEffect == null)
            flashEffect = enemy.gameObject.AddComponent<FlashEffect>();
        flashEffect.Flash();
    }

    private void StartAttack(bool toRight)
    {
        // Stop any previous attack
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }
        attackCoroutine = StartCoroutine(PerformAttackRoutine(toRight));
    }

    private System.Collections.IEnumerator PerformAttackRoutine(bool toRight)
    {
        // Position the sword for the attack (immediate so animation can align)
        if (toRight)
            transform.localPosition = rightAttackOffset;
        else
            transform.localPosition = new Vector2(-rightAttackOffset.x, rightAttackOffset.y);

        // Wait for wind-up before enabling collider
        if (hitDelay > 0f)
            yield return new WaitForSecondsRealtime(hitDelay);

        if (swordCollider != null)
            swordCollider.enabled = true;

        // Keep collider active for the attack window
        if (activeWindow > 0f)
            yield return new WaitForSecondsRealtime(activeWindow);

        if (swordCollider != null)
            swordCollider.enabled = false;

        attackCoroutine = null;
    }
}