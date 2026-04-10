using UnityEngine;

public class EnemySight : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Enemy enemy;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Debug.Log("Player Detected") ; 
            enemy.SetTarget(collision.GetComponent<Character>()) ;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player Lost") ; 
            enemy.SetTarget(null) ;
        }
    }
}
