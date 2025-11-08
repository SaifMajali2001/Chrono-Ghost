using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletLife = 1f;
    public float speed = 1f;
    public float rotationSpeed = 0f;

    private Vector2 spawnPoint;
    private float timer = 0f;
    void Start()
    {
        spawnPoint = new Vector2(transform.position.x, transform.position.y);
    }
    void Update()
    {
        if (timer > bulletLife)
        {
            Destroy(gameObject);
        }
        timer += Time.deltaTime;
        transform.position = Movement(timer);
    }

    private Vector2 Movement(float timer)
    {
        float x = timer * speed * transform.right.x;
        float y = timer * speed * transform.right.y;
        return new Vector2(x+spawnPoint.x, y+spawnPoint.y);
    }
}
