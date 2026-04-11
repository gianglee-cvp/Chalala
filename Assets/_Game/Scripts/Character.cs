using UnityEngine;

public class Character : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private  float hp  ; 
    public bool IsDead => hp <= 0 ;
    [SerializeField] private Animator anim  ;   
    protected string currentAnimName ; 
    [SerializeField] protected AttackArea attackArea ;
    [SerializeField] protected HealthBar healthBar ;
    [SerializeField] protected GameObject combatTextPrefab ;
    public virtual void OnInit(){
        hp = 100 ; 
        
    }
    public virtual void OnDespawn(){

    }
    public float GetMaxHealth()
    {
        return 100f ; // Replace with actual max health value
    }
    protected virtual void OnDeath(){
        Debug.Log("Character died") ;
    }
    public void OnHit(float damage )
    {
        if(!IsDead)
        {
            hp -= damage ;
         //   Instantiate(combatText, transform.position + Vector3.up, Quaternion.identity).OnInit(damage) ;
            if(combatTextPrefab != null)
            {
                Instantiate(combatTextPrefab, transform.position + Vector3.up + (transform.rotation * Vector3.right), Quaternion.identity).GetComponent<CombatText>().OnInit(damage) ;
            }
            if(IsDead)
            {
                hp = 0 ;
                OnDeath() ;
            }
            else
            {
                if(healthBar != null)
                {
                    healthBar.SetHealth(hp) ;
                }
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
