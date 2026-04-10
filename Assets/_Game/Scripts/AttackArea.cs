using NUnit.Framework;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private bool debugTriggerLogs = true;
    private Collider2D attackCollider;
    public bool IsTriggered  = false ; 
    public bool IsAttackAreaTriggered = false ;
    Enemy enemy;
    Player player;

    private void Start()
    {
        attackCollider = GetComponent<Collider2D>();
        attackCollider.isTrigger = true;
    }
    void Update()
    {
        // Có thể thêm logic để kích hoạt hoặc vô hiệu hóa vùng tấn công dựa trên trạng thái của nhân vật
        // Ví dụ: IsAttackAreaTriggered = true khi nhân vật đang tấn công, và false khi không tấn công
        if(IsTriggered && IsAttackAreaTriggered && enemy != null)
        {
           // Debug.Log($"[AttackArea] Dealing damage to {enemy.name} (id {enemy.GetInstanceID()}) tag={enemy.tag} time={Time.time:0.000}");
            enemy.OnHit(damageAmount);
            IsAttackAreaTriggered = false ; // Đảm bảo
        }
        if(IsTriggered && IsAttackAreaTriggered && player != null)
        {
            player.OnHit(damageAmount);
            IsAttackAreaTriggered = false ; // Đảm bảo
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") || collision.CompareTag("Enemy"))
        {
            IsTriggered = true ;
            enemy = collision.GetComponent<Enemy>();
            player = collision.GetComponent<Player>();

            // if(enemy != null && IsAttackAreaTriggered)
            // {
            //     Debug.Log($"[AttackArea] Dealing damage to {collision.name} (id {collision.GetInstanceID()}) tag={collision.tag} time={Time.time:0.000}");
            //     enemy.OnHit(damageAmount);
            //     Debug.Log($"Dealt {damageAmount} damage to {collision.name}");
            // }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (debugTriggerLogs)
        {
            Debug.Log($"[AttackArea] Exit: {collision.name} (id {collision.GetInstanceID()}) tag={collision.tag} time={Time.time:0.000}");
        }
        // Có thể thêm logic khi đối tượng rời khỏi vùng tấn công nếu cần
        if(collision.CompareTag("Player") || collision.CompareTag("Enemy"))
        {
            IsTriggered = false ;
        }
    }
}


