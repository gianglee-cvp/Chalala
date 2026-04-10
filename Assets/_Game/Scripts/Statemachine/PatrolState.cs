
using UnityEngine;

public class PatrolState : IState
{
    float timer   ; 
    float randomTime ; 
    public void OnEnter(Enemy enemy)
    {
        randomTime = Random.Range(2f, 5f);
        timer = 0f  ; 
    }

    public void OnExecute(Enemy enemy)
    {
        timer += Time.deltaTime; 
        if(enemy.Target != null && enemy.IsTargetInRange()){
            Debug.Log("a") ; 
            enemy.ChangeDirection(enemy.Target.transform) ;
            enemy.Moving() ;

        }
        else
        {
            if(timer <  randomTime){
                enemy.Moving() ;    
            } 
            else{
                enemy.ChangeState(new IdleState());
            }            
        }

    }   

    public void OnExit(Enemy enemy)
    {
        
    }
}

