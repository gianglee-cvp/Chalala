using System.Collections;
using System.Collections.Generic;
using UnityEngine ; 

public class Enemy : Character
{
    [SerializeField] private float movespeed = 5f   ; 
    [SerializeField] private float attackRange = 1f   ; 
    [SerializeField] private float attackRate = 1f   ; 
    [SerializeField] private float attackDamage = 10f   ; 
    [SerializeField] private float attackSpeed = 1f   ; 

    [SerializeField] private Transform target ;
    [SerializeField] private Rigidbody2D rb ; 

    private bool isRight = true ; 



    private IState currentState;

    private void Start()
    {
        OnInit();
    }

    void Update(){
        if(currentState != null){
            currentState.OnExecute(this);
        }
    }

    public override void OnInit(){
        base.OnInit();
        ChangeState(new IdleState());
    }
    public override void OnDespawn(){
        base.OnDespawn();
    }
    public void Moving(){
        rb.linearVelocity = new Vector2(transform.right.x * movespeed, rb.linearVelocity.y);
        Debug.Log("Moving") ;
        ChangeAnim("run")  ; 

    }
    public void StopMoving(){
        ChangeAnim("idle")  ; 
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }
    public void Attack(){

    }
    public bool IsTargerInRange(){
        return false ; 
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
    
}