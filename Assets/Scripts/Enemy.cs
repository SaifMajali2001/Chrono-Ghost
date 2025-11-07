using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class Enemy : MonoBehaviour
{
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

    void Die()
    {
        Destroy(gameObject);
    }
    
}
