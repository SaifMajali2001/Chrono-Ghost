using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class Enemy : MonoBehaviour
{
    Animator animator;
    public float health = 1;
    public float Health
    {
        set
        {
            health = value;
            if (health <= 0)
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
      animator.SetTrigger("Die");
    }

    public void RemoveEnemy()
    {
        Destroy(gameObject);
    }
    
}
