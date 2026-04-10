using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine ;
using UnityEngine.UIElements;

public class Enemy : Character
{
    [SerializeField] private float movespeed = 5f   ; 
    [SerializeField] private float attackRange = 10f   ; 
    [SerializeField] public float attackRate = 1f   ; 
    [SerializeField] private float attackDamage = 10f   ; 
    [SerializeField] private float attackSpeed = 1f   ; 

    [SerializeField] private Rigidbody2D rb ; 
    [SerializeField] private Collider2D bodyCollider ;
    public Vector3 savePoint = new Vector3(40.5699997f,0.100000001f,0) ;

    private bool isRight = true ; 
    private Character target ; 
    public Character Target => target ;



    private IState currentState;

    private void Start()
    {
        OnInit();
        savePoint = transform.position ;
    }

    void Update(){
        if(currentState != null){
            currentState.OnExecute(this);
        }
    }

    public override void OnInit(){
        base.OnInit();
        gameObject.SetActive(true) ;
        transform.position = savePoint ; 
        ChangeState(new IdleState());


    }
    public override void OnDespawn(){
        base.OnDespawn();
    }
    protected override void OnDeath(){
        base.OnDeath();
        gameObject.SetActive(false) ;
        Invoke(nameof(OnInit), 3f) ;
        
    }
    public void Moving(){
        float direction = isRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(movespeed * direction, rb.linearVelocity.y);
        ChangeAnim("run")  ; 
    }
    public void StopMoving(){
        ChangeAnim("idle")  ; 
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }
    public void Attack(){
        Debug.Log("Attack") ;
        currentAnimName =  "" ; 
        ChangeAnim("attack")  ; 
    }
    public bool IsTargetInRange(){
        return Vector2.Distance(transform.position, target.transform.position) <= attackRange ; 
    }
    public void ChangeState(IState newState){
        Debug.Log("Change State " + newState.ToString()) ;
        if(currentState != null){
            currentState.OnExit(this);
        }
        currentState = newState;
        if(currentState != null){
            currentState.OnEnter(this);
        }
    }
    internal void SetTarget(Character character)
    {
        this.target = character; 
        if(target != null && IsTargetInRange()){
          //  ChangeState(new AttackState()) ;
        }
        else if(target != null){
            ChangeState(new PatrolState()) ;
        }
        else{
            ChangeState(new IdleState()) ;
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyWall"))
        {
            ChangeDirection();
            Debug.Log("Collided with EnemyWall, changing direction.");
         //   ChangeState(new IdleState()) ;
        }
        if(collision.CompareTag("DeathZone"))
        {
            Debug.Log("Enemy died") ;
            ChangeAnim("die") ;
            OnDeath() ;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Player") && bodyCollider.IsTouching(collision.collider))
        {
            ChangeState(new AttackState());
            Debug.Log("Player in range, switching to AttackState.");
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Player") && !bodyCollider.IsTouching(collision.collider))
        {
            ChangeState(new PatrolState());
            Debug.Log("Player out of range, switching to PatrolState.");
        }
    }
    // Toggle direction (dùng cho EnemyWall)
    public void ChangeDirection()
    {
        isRight = !isRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    // Quay mặt về phía target (dùng cho PatrolState)
    public void ChangeDirection(Transform target)
    {
        if (target == null) return;

        bool shouldFaceRight = target.position.x > transform.position.x;
        Debug.Log(shouldFaceRight + " abc" + isRight) ; 
        if (shouldFaceRight != isRight)
        {
            ChangeDirection(); 
        }
    }

}