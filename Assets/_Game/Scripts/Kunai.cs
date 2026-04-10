using UnityEngine;

public class Kunai : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Rigidbody2D rb;
    public float speed = 10f;
    public float damage;
    public float lifetime;
    private Vector2 direction;
    public Transform player;
    void Start()
    {
        rb.linearVelocity = transform.right * speed;
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetDirection()
    {
        direction = player.transform.right;
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Kunai collided with: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.OnHit(damage);
            }
            Destroy(gameObject);
        }



    }

}
