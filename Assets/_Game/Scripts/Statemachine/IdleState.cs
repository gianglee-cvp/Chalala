using UnityEngine;

public class IdleState : IState
{
    float randomtime ; 
    float timer ; 
    public void OnEnter(Enemy enemy)
    {
        enemy.StopMoving();
        randomtime = Random.Range(2f, 4f);
        timer = 0f  ; 
    }

    public void OnExecute(Enemy enemy)
    {
        timer += Time.deltaTime;
        if(timer >= randomtime){
            enemy.ChangeState(new PatrolState());
        }
    }

    public void OnExit(Enemy enemy)
    {
        
    }
}

