using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    Collider2D swordCollider;
    Vector2 rightAttackOffset;
    public float damage = 3;

    public void Awake()
    {
        swordCollider = GetComponent<Collider2D>();
        if (swordCollider != null)
            swordCollider.enabled = false;

        rightAttackOffset = transform.localPosition;
    }

    public void AttackRight()
    {
        if (swordCollider != null)
            swordCollider.enabled = true;

        transform.localPosition = rightAttackOffset;
    }

    public void AttackLeft()
    {
        if (swordCollider != null)
            swordCollider.enabled = true;

        transform.localPosition = new Vector2(-rightAttackOffset.x, rightAttackOffset.y);
    }

    public void StopAttack()
    {
        swordCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Health -= damage;
            }

        }
    }
}