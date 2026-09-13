using UnityEngine;

public class BansheeHiddenState : IEnemyState
{
    BansheeAI enemy;
    private float delay;

    public BansheeHiddenState(BansheeAI _enemy)
    {
        enemy = _enemy;
    }

    public void WaitFor(float _delay)
    {
        delay = Time.time + _delay;
    }


    public void Enter()
    {
        enemy.StopScream();
        enemy.SetVisible(false);
    }
    public void Tick()
    {
        if (enemy.IsDead || Time.time < delay) return;
        if (enemy.CanSeePlayer())
        {
            enemy.stateMachine.ChangeState(enemy.approachState);
        }


    }
    public void Exit()
    {
        
    }

  
}

public class BansheeAppearState : IEnemyState
{
    BansheeAI enemy;
    private float screamAt;
    public BansheeAppearState(BansheeAI _enemy)
    {
        enemy = _enemy;
    }
    public void Enter()
    {
        enemy.SetVisible(true);
        enemy.FacePlayer();
        screamAt = Time.time + enemy.stats.appearDelay;

    }
    public void Tick()
    {
       if(enemy.playerTarget == null)
        {
            enemy.GoHidden(enemy.stats.scareCooldown);
            return;
        }
        enemy.FacePlayer();
        if(Time.time >= screamAt)
        {
            enemy.stateMachine.ChangeState(enemy.screamState);
        }
    }
    public void Exit()
    {
        
    }

  
}

public class BansheeScreamState : IEnemyState
{
    BansheeAI enemy;
    private float disappearAt;
    public BansheeScreamState(BansheeAI _enemy)
    {
        enemy = _enemy;
    }
    public void Enter()
    {
        float duration = enemy.PlayScream();
        disappearAt = Time.time + duration + enemy.stats.lingerAfterScream;
    }
    public void Tick()
    {
        if(enemy.playerTarget == null)
        {
            enemy.GoHidden(enemy.stats.scareCooldown);
            return;
        }
        enemy.FacePlayer();
        if(Time.time >= disappearAt)
        {
            enemy.stateMachine.ChangeState(enemy.fleeState);
        }
    }

    public void Exit()
    {
        enemy.StopScream();
    }

   
}
public class BansheeFleeState : IEnemyState
{
    private BansheeAI enemy;
    private float disappearAt;
    private float nextPathTime;

    public BansheeFleeState(BansheeAI _enemy)
    {
        enemy = _enemy;
    }
    public void Enter()
    {
        disappearAt = Time.time + enemy.stats.fleeDuration;
        nextPathTime = Time.time + .5f;
        enemy.RunAway();
    }
    public void Tick()
    {
        if(enemy.playerTarget == null || Time.time >= disappearAt)
        {
            enemy.GoHidden(enemy.stats.scareCooldown);
            return;
        }

        if(Time.time >= nextPathTime)
        {
            enemy.RunAway();
            nextPathTime = Time.time +.5f;
        }
    }
    public void Exit()
    {
        enemy.StopMoving();
    }

   
}
public class BansheeApproachState : IEnemyState
{
    private BansheeAI enemy;
    private float nextPathTime;
    private float previousStoppingDistance;
    

    public BansheeApproachState(BansheeAI _enemy)
    {
        enemy = _enemy;
    }
    public void Enter()
    {
        enemy.SetVisible(false);
        enemy.timeSinceLastSawPlayer = 0f;
        nextPathTime = Time.time;
        if(enemy.agent != null)
        {
            previousStoppingDistance = enemy.agent.stoppingDistance;
            enemy.agent.stoppingDistance = enemy.stats.attackRange * .5f;
        }
    }
    public void Tick()
    {
        if(enemy.playerTarget ==  null || enemy.agent == null)
        {
            enemy.GoHidden(enemy.stats.scareCooldown);
            return;
        }

        bool canSee = enemy.CanSeePlayer();
        if(canSee&& enemy.IsPlayerInAttackRange())
        {
            enemy.stateMachine.ChangeState(enemy.appearState);
            return;
        }

        if(canSee == false)
        {
            enemy.AddSightLossTime();
            if(enemy.timeSinceLastSawPlayer >= enemy.stats.lostSightTime)
            {
                enemy.GoHidden(enemy.stats.scareCooldown);
                return;
            }
        }

        if(Time.time >= nextPathTime)
        {
            enemy.agent.speed = enemy.stats.chaseSpeed;
            enemy.agent.isStopped = false;
            bool foundDestination = enemy.agent.SetDestination(enemy.playerTarget.position);
            nextPathTime = Time.time + .5f;
            if(foundDestination == false)
            {
                enemy.GoHidden(enemy.stats.scareCooldown);
            }
        }
    }
    public void Exit()
    {
        enemy.StopMoving();
        if(enemy.agent != null )
        {
            enemy.agent.stoppingDistance = previousStoppingDistance;
        }
    }

    
}

public class BansheeDefeatedState : IEnemyState
{
    BansheeAI enemy;
    private float reviveAt;
    public BansheeDefeatedState(BansheeAI _enemy)
    {
        enemy = _enemy;
    }
    public void Enter()
    {
        enemy.StopScream();
        enemy.SetVisible(false);
        reviveAt = Time.time + enemy.stats.returnDelay;
    }
    public void Tick()
    {
        if(Time.time >= reviveAt)
        {
            enemy.Revive();
        }
    }
    public void Exit()
    {
        
    }

    
}