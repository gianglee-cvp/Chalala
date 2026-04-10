using UnityEngine;

public class Character : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private  float hp  ; 
    public bool IsDead => hp <= 0 ;
    [SerializeField] private Animator anim  ;   
    protected string currentAnimName ; 
    [SerializeField] protected AttackArea attackArea ;
    public virtual void OnInit(){
        hp = 100 ; 
        
    }
    public virtual void OnDespawn(){

    }
    protected virtual void OnDeath(){
        Debug.Log("Character died") ;
    }
    public void OnHit(float damage  )
    {
        if(!IsDead)
        {
            hp -= damage ;
            if(IsDead)
            {
                hp = 0 ;
                OnDeath() ;
            }
        }
        Debug.Log($"OnHit {damage} damage, hp left {hp}") ;
    }

    protected void ChangeAnim(string animName)
    {
       // Debug.Log($"ChangeAnim from {currentAnimName} to {animName}") ;
        if(currentAnimName != animName)
        {
            anim.ResetTrigger(animName) ;
            currentAnimName = animName ; 

            anim.SetTrigger(currentAnimName) ;
        }
    }
}
