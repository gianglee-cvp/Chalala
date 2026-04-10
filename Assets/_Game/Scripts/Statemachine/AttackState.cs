using UnityEngine;

public class AttackState : IState
{
    float timer ; 
    public void OnEnter(Enemy enemy)
    {
        enemy.StopMoving() ;
        // Quay mặt về phía player
        if(enemy.Target != null){
            enemy.ChangeDirection(enemy.Target.transform) ;
            enemy.Attack() ;
        }
        timer = 0f ;
    }

    public void OnExecute(Enemy enemy)
    {
        timer += Time.deltaTime ;
        if(timer >= 1f){
            enemy.Attack() ;
            timer = 0f ;
        }

    }

    public void OnExit(Enemy enemy)
    {
        
    }
}
