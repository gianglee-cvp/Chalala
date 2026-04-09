using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float speed = 5f;
    
    private Transform currentTarget;
    private Vector3 direction;

    private void Start()
    {
        transform.position = pointA.position ; 
        if (pointA == null || pointB == null)
        {
            Debug.LogError("Point A and Point B must be assigned!");
            return;
        }
        
        currentTarget = pointB;
    }

    private void Update()
    {
        if (pointA == null || pointB == null) return;

        // Move towards current target
        transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);

        // Switch target when reached
   //     Debug.Log(Vector3.Distance(transform.position, currentTarget.position));
        if (Vector3.Distance(transform.position, currentTarget.position) < 0.1f)
        {
            currentTarget = (currentTarget == pointA) ? pointB : pointA;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}