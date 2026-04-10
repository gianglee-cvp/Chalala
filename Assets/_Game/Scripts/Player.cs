// using System.Numerics;
using System;
using System.Data.Common;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Character
{
    [SerializeField] private Rigidbody2D rb ; 
    [SerializeField] private LayerMask groundLayer ;

    [SerializeField] private float speed = 5f   ;
    [SerializeField] private float jumpForce = 350f   ; 
    [SerializeField] private GameObject kunai ; 

    private int coin = 0 ; 
    private bool isGrounded = true  ; 
    private bool isJumping = false;    
    private bool isAttack  = false; 
    private bool isDead = false;

    private float horizontal ; 
   // private string currentAnimName ; 
    
    // Input buffers to capture inputs in Update and execute them in FixedUpdate
    private bool jumpInput;
    private bool attackInput;
    private bool throwInput;

    private Vector3 savePoint ; 

    // private float vertical ;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        savePoint = transform.position ;
        OnInit() ;
    }
    public override void OnInit()
    {
        base.OnInit() ;
        ChangeAnim("idle")   ;
        coin = 0 ; 
        isGrounded = true  ;    
        isJumping = false;    
        isAttack  = false; 
        isDead = false;
        horizontal = 0f ; 
        currentAnimName = null ;
        transform.position = savePoint ; 
    }
    protected override void OnDeath()
    {
        Debug.Log("Player died") ;
        base.OnDeath();
        ChangeAnim("die")   ;
        isDead = true ;
        Invoke(nameof(OnInit), 1f) ; // Reset game after 1 second
    }

    // Update is called once per frame
    void Update()
    {
        // lấy input từ bàn phím trong Update để không bị delay
        horizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space)) jumpInput = true;
        if (Input.GetKeyDown(KeyCode.C)) attackInput = true;
        if (Input.GetKeyDown(KeyCode.V)) throwInput = true;
    }

    void FixedUpdate()
    {
        if(isDead) return; // Nếu đã chết, không thực hiện các hành động tiếp theo
        isGrounded = CheckGrounded() ;  

        if (isGrounded)
        {
            if(jumpInput && !isAttack)
            {
                Jump() ; 
            }
            if(Math.Abs(horizontal)  > 0.1f)
            {
                // linearVelocity là vận tốc tuyến tính của Rigidbody2D, đã là đơn vị m/s nên có thể không cần nhân với Time.deltaTime
                if(isAttack) 
                {
                    // Nếu đang attack thì bỏ qua di chuyển và reset input
                    jumpInput = false;
                    attackInput = false;
                    throwInput = false;
                    return ;
                }
                ChangeAnim("run")   ; 
            }
            //attack
            if (attackInput && !isAttack)
            {
                Attack() ; 
            }
            //throw
            if (throwInput && !isAttack)
            {
                Throw() ;
            }
            else
            {
                if(!isAttack && Math.Abs(horizontal)  <= 0.1f)
                {
                    ChangeAnim("idle")   ;
                }
            }
        }


        if(!isGrounded && rb.linearVelocity.y < 0.01f)
        {
            ChangeAnim("fall")   ;
        }
        //moving
        if(Math.Abs(horizontal)  > 0.1f && !isAttack)
        {
            // linearVelocity là vận tốc tuyến tính của Rigidbody2D, đã là đơn vị m/s nên có thể không cần nhân với Time.deltaTime
            rb.linearVelocity = new Vector2(horizontal * speed   , rb.linearVelocity.y) ; 
            transform.rotation = horizontal > 0 ? Quaternion.identity : Quaternion.Euler(0, 180, 0) ;
            // identity là một phép quay không có sự thay đổi, nghĩa là đối tượng sẽ không bị xoay. Còn Quaternion.Euler(0, 180, 0) sẽ xoay đối tượng 180 độ quanh trục Y, làm cho nó quay ngược lại.

        }
        else if(isGrounded)
        {
            ChangeAnim("idle")   ;
            rb.linearVelocity = Vector2.zero ;
        }
        
        // Reset input buffers sau khi đã xử lý trong FixedUpdate
        jumpInput = false;
        attackInput = false;
        throwInput = false;
    }
    private bool CheckGrounded() 
    {
        // If moving upward, don't treat one-way platforms as grounded
        if (rb.linearVelocity.y > 0.01f)
        {
            return false;
        }
        Debug.DrawLine(transform.position, transform.position + Vector3.down * 1.1f, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundLayer);
        return hit.collider != null;
    }
    private void Attack()
    {
        isAttack = true ;
        ChangeAnim("attack")   ;     
        if(attackArea != null)
        {
            attackArea.IsAttackAreaTriggered = true  ; // Cho tấn công kích hoạt vùng tấn công
        }
        Invoke(nameof(ResetAttack), 0.5f) ;
    }
    private void ResetAttack()
    {
        //ChangeAnim("idle")   ;
        isAttack = false ;
        if(attackArea != null)
        {
            attackArea.IsAttackAreaTriggered = false  ; // Reset trạng thái sau khi tấn công
        }
    }
    private void Throw()
    {
        isAttack = true ;
        ChangeAnim("throw") ;
        Instantiate(kunai, transform.position + transform.right * 0.5f, transform.rotation) ;
        Invoke(nameof(ResetAttack), 0.5f) ;
    }
    private void Jump()
    {
        isJumping = true ;
        ChangeAnim("jump")   ;
        rb.AddForce(jumpForce * Vector2.up) ; 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Coin"))
        {
            coin++  ; 
            Destroy(collision.gameObject) ;
        }
        if(collision.CompareTag("DeathZone"))
        {
            OnDeath() ;
        }
        if(collision.CompareTag("SavePoint"))
        {
            savePoint = collision.transform.position ;
        }
    }
}

