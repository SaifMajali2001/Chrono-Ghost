using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    Collider2D swordCollider;
    Vector2 rightAttackOffset;
    public float damage = 3;
    [Header("Attack Timing")]
    [SerializeField] private float hitDelay = 0.12f;
    [SerializeField] private float activeWindow = 0.08f;
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
                StartCoroutine(DelayedApplyHitEffects(enemy));
            }
        }
    }

    private System.Collections.IEnumerator DelayedApplyHitEffects(Enemy enemy)
    {
        if (effectDelay > 0f)
            yield return new WaitForSecondsRealtime(effectDelay);

        enemy.Health -= damage;

        TimeManager.Instance.DoHitstop();

        FlashEffect flashEffect = enemy.GetComponent<FlashEffect>();
        if (flashEffect == null)
            flashEffect = enemy.gameObject.AddComponent<FlashEffect>();
        flashEffect.Flash();
    }

    private void StartAttack(bool toRight)
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }
        attackCoroutine = StartCoroutine(PerformAttackRoutine(toRight));
    }

    private System.Collections.IEnumerator PerformAttackRoutine(bool toRight)
    {
        if (toRight)
            transform.localPosition = rightAttackOffset;
        else
            transform.localPosition = new Vector2(-rightAttackOffset.x, rightAttackOffset.y);

        if (hitDelay > 0f)
            yield return new WaitForSecondsRealtime(hitDelay);

        if (swordCollider != null)
            swordCollider.enabled = true;

        if (activeWindow > 0f)
            yield return new WaitForSecondsRealtime(activeWindow);

        if (swordCollider != null)
            swordCollider.enabled = false;

        attackCoroutine = null;
    }
}