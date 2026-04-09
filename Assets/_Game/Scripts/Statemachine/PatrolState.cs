
using UnityEngine;

public class PatrolState : IState
{
    float timer   ; 
    float randomTime ; 
    public void OnEnter(Enemy enemy)
    {
        randomTime = Random.Range(3f, 6f);
        timer = 0f  ; 
    }

    public void OnExecute(Enemy enemy)
    {
        timer += Time.deltaTime; 
        if(timer <  randomTime){
            enemy.Moving() ;    
        } 
        else{
            enemy.ChangeState(new IdleState());
        } 
    }

    public void OnExit(Enemy enemy)
    {
        
    }
}

