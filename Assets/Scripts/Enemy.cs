using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class Enemy : MonoBehaviour
{
    Animator animator;
    public float health = 1;
    private bool isDying = false;
    [SerializeField] private float deathAnimationLength = 1f;
    public float Health
    {
        set
        {
            health = value;
            if (health <= 0 && !isDying)
            {
                Die();
            }
        }
        get
        {
            return health;
        }

    }
    public void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Die()
    {
        if (isDying) return;
        isDying = true;

        var ai = GetComponent<EnemyAI>();
        if (ai != null) ai.enabled = false;

        var collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;

        var rigidbody = GetComponent<Rigidbody2D>();
        if (rigidbody != null) rigidbody.simulated = false;

        if (animator != null)
        {
            animator.SetTrigger("Die");
            Invoke("RemoveEnemy", deathAnimationLength);
        }
        else
        {
            RemoveEnemy();
        }
    }

    public void RemoveEnemy()
    {
        CancelInvoke();
        Destroy(gameObject);
    }
    
}
