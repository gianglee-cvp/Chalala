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
    private float joystickHorizontal; // Lưu biến Joystick mềm riêng biệt
    private Vector2 lastAimDirection = Vector2.right; // Biến nhớ lưu hướng kéo Joystick để ném 360 độ
   // private string currentAnimName ;
    
    [Header("Glide Settings")]
    [SerializeField] private float glideStartHeight = 6f;
    [SerializeField] private float glideStopHeight = 3f;
    [SerializeField] private float maxGlideFallSpeed = 3f;
    [SerializeField] private float glideMoveSpeed = 3f;
    private bool isGliding = false;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    private bool isDashing;
    private bool canDash = true;
    private float originalGravity;
    [Header("Tele Setting")]
    public Transform telePoint1;
    private GameObject activeKunai;

    // Input buffers to capture inputs in Update and execute them in FixedUpdate
    private bool jumpInput;
    private bool attackInput;
    private bool throwInput;
    private bool dashInput;
    private bool teleportInput;

    private Vector3 savePoint ;

    // private float vertical ;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        savePoint = transform.position ;
        OnInit() ;
       // coin = PlayerPrefs.GetInt("coin", 0) ;
    }
    void Awake()
    {
        coin = PlayerPrefs.GetInt("coin", 0) ;
    }
    public override void OnInit()
    {
        base.OnInit() ;
        ChangeAnim("idle")   ;
        coin = PlayerPrefs.GetInt("coin", 0) ; 
        isGrounded = true  ;    
        isJumping = false;    
        isAttack  = false; 
        isDead = false;
        isDashing = false;
        canDash = true;
        isGliding = false;
        if (anim != null) anim.SetBool("Fly", false);
        if (rb != null) originalGravity = rb.gravityScale;
        horizontal = 0f ;
        currentAnimName = null ;
        healthBar.SetHealth(100f) ;
        transform.position = savePoint ; 
        UIManager.instance.SetCoinText(coin) ;

    }
    protected override void OnDeath()
    {
        healthBar.SetHealth(0f) ;
     //   Debug.Log("Player died") ;
        base.OnDeath();
        ChangeAnim("die")   ;
        isDead = true ;
        Invoke(nameof(OnInit), 1f) ; // Reset game after 1 second
    }

    // Update is called once per frame
    void Update()
    {
        // Bắt trọn 2 trục X Y của bàn phím:
        horizontal = Input.GetAxisRaw("Horizontal");
        Vector2 moveDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Math.Abs(joystickHorizontal) > 0.05f)
        {
            horizontal = joystickHorizontal;
            moveDir = new Vector2(joystickHorizontal, moveDir.y); // Hỗ trợ gán x tay
        }

        // Tự động quét và kéo dữ liệu X, Y từ Component On-Screen Stick 
        if (Gamepad.current != null && Gamepad.current.leftStick.ReadValue().magnitude > 0.05f)
        {
            Vector2 stickInput = Gamepad.current.leftStick.ReadValue();
            horizontal = stickInput.x;
            moveDir = stickInput;
        }
        else if (UnityEngine.InputSystem.Joystick.current != null && UnityEngine.InputSystem.Joystick.current.stick.ReadValue().magnitude > 0.05f)
        {
            Vector2 stickInput = UnityEngine.InputSystem.Joystick.current.stick.ReadValue();
            horizontal = stickInput.x;
            moveDir = stickInput;
        }

        // Bộ não ghi nhớ 360 độ: Cứ kéo joystick đi đâu, nó lưu hướng đó lại để lát lấy ra ném Kunai
        if (moveDir.magnitude > 0.05f)
        {
            lastAimDirection = moveDir.normalized;
        }

        if (Input.GetKeyDown(KeyCode.Space)) jumpInput = true;
        if (Input.GetKeyDown(KeyCode.C)) attackInput = true;
        if (Input.GetKeyDown(KeyCode.V)) throwInput = true;
        if (Input.GetKeyDown(KeyCode.LeftShift)) dashInput = true;
        if (Input.GetKeyDown(KeyCode.T)) teleportInput = true;

        string clipName = anim.GetCurrentAnimatorClipInfo(0).Length > 0 ? anim.GetCurrentAnimatorClipInfo(0)[0].clip.name : "none";
     //   Debug.Log("isGrounded: " + isGrounded + " | current anim: " + currentAnimName + " | playing clip: " + clipName);
        if(isGrounded && (clipName != "idle")){
            anim.SetBool("jumptoidle", true);
        }
        else{
            anim.SetBool("jumptoidle", false);
        }
    }

    void FixedUpdate()
    {
        if(isDead) return; // Nếu đã chết, không thực hiện các hành động tiếp theo

        if (teleportInput)
        {
            TeleportToKunai();
        }

        if (dashInput && canDash)
        {
            StartCoroutine(DashCoroutine());
        }

        if (isDashing)
        {
            jumpInput = false;
            attackInput = false;
            throwInput = false;
            dashInput = false;
            return;
        }

        isGrounded = CheckGrounded() ;  

        // --- Tính toán Gliding (Lơ lửng sải cánh) ---
        if (!isGrounded && rb.linearVelocity.y < 0f)
        {
            RaycastHit2D groundHit = Physics2D.Raycast(transform.position, Vector2.down, 20f, groundLayer);
            float groundDist = groundHit.collider != null ? groundHit.distance : 20f;

            if (!isGliding && groundDist >= glideStartHeight)
            {
                isGliding = true;
                anim.SetBool("Fly", true);
            }
            else if (isGliding && groundDist < glideStopHeight)
            {
                isGliding = false;
                anim.SetBool("Fly", false);
            }
        }
        else if (isGrounded && isGliding)
        {
            isGliding = false;
            anim.SetBool("Fly", false);
        }

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
             //   Debug.Log("Horizontal: " + horizontal);
                if(!isAttack && Math.Abs(horizontal)  <= 0.1f)
                {
                 //   Debug.Log("Changing to idle animation") ;
                    ChangeAnim("idle")   ;
                }
                
            }
        }


        if(!isGrounded && rb.linearVelocity.y < 0.01f)
        {
            if (!isGliding) ChangeAnim("fall")   ;
        }

        // Kìm hãm tốc độ rơi nếu đang Glide
        if (isGliding && rb.linearVelocity.y < -maxGlideFallSpeed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -maxGlideFallSpeed);
        }

        //moving
        if(Math.Abs(horizontal)  > 0.1f && !isAttack)
        {
            float targetSpeed = isGliding ? glideMoveSpeed : speed;
            rb.linearVelocity = new Vector2(horizontal * targetSpeed   , rb.linearVelocity.y) ; 
            transform.rotation = horizontal > 0 ? Quaternion.identity : Quaternion.Euler(0, 180, 0) ;
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
        dashInput = false;
        teleportInput = false;
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
     //   Debug.Log("Ground check hit: " + (hit.collider != null ? hit.collider.name : "None") + " - " + hit.collider != null);
        return hit.collider != null;
    }
    public void Attack()
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
    public void Throw()
    {
        isAttack = true ;
        ChangeAnim("throw") ;

        // Tính góc bay 360 độ (xoay theo trục Z) dựa trên lực kéo Joystick
        float angle = Mathf.Atan2(lastAimDirection.y, lastAimDirection.x) * Mathf.Rad2Deg;
        Quaternion aimRotation = Quaternion.Euler(0, 0, angle);

        // Sinh phi tiêu đẩy ra 0.5 mét theo chính cái hướng đó
        activeKunai = Instantiate(kunai, transform.position + (Vector3)(lastAimDirection * 0.5f), aimRotation) ;
        
        Invoke(nameof(ResetAttack), 0.5f) ;
    }

    public void TeleportToKunai()
    {
        // Unity tự động hiểu activeKunai == null nếu object đã bị Destroy (bay trúng quái hoặc hết lifetime)
        if (activeKunai != null)
        {
            transform.position = activeKunai.transform.position;
            rb.linearVelocity = Vector2.zero; // Reset quán tính rơi để nhân vật lơ lửng chờ phản ứng
            
            // Xoá phi tiêu ngay sau khi tele tới (nếu không xoá thì comment dòng dưới lại)
            Destroy(activeKunai);
            activeKunai = null;
        }
    }
    public void Jump()
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
            PlayerPrefs.SetInt("coin", coin) ;
            PlayerPrefs.Save() ;
            UIManager.instance.SetCoinText(coin);
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
    private System.Collections.IEnumerator DashCoroutine()
    {
        canDash = false;
        isDashing = true;

        // Lưu và tắt trọng lực để không bị rớt xuống khi lướt trên không
        originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        // Hướng lướt phụ thuộc vào hướng mặt hiển tại
        float dashDir = transform.right.x > 0 ? 1f : -1f;

        rb.linearVelocity = new Vector2(dashDir * dashSpeed, 0f);

        // Nếu bạn có animation dash, mở comment dòng dưới (cần thiết lập animation)
        // ChangeAnim("dash"); 

        yield return new WaitForSeconds(dashDuration);

        // Kết thúc lướt
        rb.gravityScale = originalGravity;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        isDashing = false;

        // Hồi chiêu
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    // Gọi hàm này từ OnScreenStick (Event OnDrag hoặc Update của UI Joystick)
    public void Setmove(float stickMoveX)
    {
        this.joystickHorizontal = stickMoveX ;
    }
    public void SetButton(string buttonName)
    {
        switch (buttonName)
        {
            case "Jump":
                jumpInput = true;
                break;
            case "Attack":
                attackInput = true;
                break;
            case "Throw":
                throwInput = true;  
                break;
            case "Dash":
                dashInput = true;
                break;
            case "Teleport":
                teleportInput = true;
                break;
        }
    }
}


