using UnityEngine;

public class MummyBandageState : IEnemyState
{
    private MummyAI enemy;
    private float fireAt;
    private float doneAt;
    private bool fired;

    public MummyBandageState(MummyAI _enemy)
    {
        enemy = _enemy;
    }
    public void Enter()
    {
        enemy.StopAgent();
        fired = false;
        fireAt = Time.time + enemy.mummyStats.bandageWindupTime;
        doneAt = 0f;

        enemy.PlayVFXandSFX(enemy.mummyStats.bandageWindup, enemy.firePoint.position);
    }

    public void Tick()
    {
        if(enemy.playerTarget == null)
        {
            enemy.ReturnToChase();
            return;
        }

        enemy.FacePlayer();

        if(fired == false)
        {
            if(Time.time >=  fireAt)
            {
                enemy.FireBandage(); ;
                fired = true;
                doneAt = Time.time + enemy.mummyStats.bandageRecovery;
            }
            return;
        }

        if (Time.time >= doneAt) enemy.ReturnToChase();
    }

    public void Exit()
    {
      
    }

  
}
public class MummyQuicksandState : IEnemyState
{
    private MummyAI enemy;
    private Vector3 point;
    private float spawnAt;
    private float doneAt;
    private bool spawned;

    public MummyQuicksandState(MummyAI _enemy)
    {
        enemy = _enemy;
    }
    public void Enter()
    {
        enemy.StopAgent();
        enemy.MarkQuicksandUsed();
        spawned = false;
        doneAt = 0;

        point = enemy.PickQuicksandPoint();
        enemy.SpawnQuicksandTelegraph(point);
        enemy.PlayVFXandSFX(enemy.mummyStats.quicksandWindup, enemy.transform.position);

        spawnAt = Time.time + enemy.mummyStats.quicksandWarningTime;
    }
    public void Tick()
    {
        if (enemy.playerTarget != null) enemy.FacePlayer();

        if(spawned == false)
        {
            if(Time.time >= spawnAt)
            {
                enemy.SpawnQuicksandPool(point);
                spawned = true;
                doneAt = Time.time + enemy.mummyStats.quicksandRecovery;
            }
            return;
        }
        if (Time.time >= doneAt) enemy.ReturnToChase();
    }
    public void Exit()
    {
        
    }

   
}

public class MummyBurrowState : IEnemyState
{
    private enum Phase { Sinking, Travelling, Rising, Recovering}

    private MummyAI enemy;
    private Phase phase;
    private float timer;
    private Vector3 sinkStart;
    private Vector3 exitPoint;
    private Vector3 riseStart;
    private float travelSpeed;


    public MummyBurrowState(MummyAI _enemy)
    {
        enemy = _enemy;
    }
    public void Enter()
    {
       enemy.MarkBurrowUsed();
        enemy.StopAgent();

        enemy.SetAgentEnabled(false);

        enemy.PlayVFXandSFX(enemy.mummyStats.burrowDown, enemy.transform.position);

        sinkStart = enemy.transform.position;
        phase = Phase.Sinking;
        timer = 0f;
    }
    public void Tick()
    {
        timer += Time.deltaTime;
        MummStatsSO stats = enemy.mummyStats;

        if(phase == Phase.Sinking)
        {
            float amount = Mathf.Clamp01(timer / stats.burrowDigTime);
            Vector3 under = sinkStart + Vector3.down * stats.burrowDepth;
            enemy.transform.position = Vector3.Lerp(sinkStart,under, amount);

            if(amount > 1f)
            {
                enemy.SetBurrowed(true);
                enemy.PlayVFXandSFX(stats.burrowtrail, enemy.transform.position);

                enemy.SpawnBurrowMound();

                exitPoint = enemy.PickBurrowExit();

                float gap = Vector3.Distance(enemy.transform.position, exitPoint);
                travelSpeed = gap / stats.burrowTravelTime;

                phase = Phase.Travelling;
                timer = 0f;
            }
            return;
        }
        if(phase == Phase.Travelling)
        {
            if (enemy.playerTarget != null) exitPoint = enemy.PickBurrowExit();

            Vector3 target = exitPoint + Vector3.down * stats.burrowDepth;
            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, target,travelSpeed*Time.deltaTime);

            bool arrived = Vector3.Distance(enemy.transform.position, target) <= .2f;
            if (arrived || timer >= stats.burrowTravelTime) StartRising();

            return;
        }
        if(phase == Phase.Rising)
        {
            float amount = Mathf.Clamp01(timer/stats.burrowRiseTime);
            enemy.transform.position = Vector3.Lerp(riseStart, exitPoint, amount);

            if(amount >= 1)
            {
                enemy.SetAgentEnabled(true);
                enemy.WarpTo(exitPoint);

                phase = Phase.Recovering;
                timer = 0f;
            }
            return;
        }

        if (timer >= stats.burrowRecovery) enemy.ReturnToChase();
    }
    public void Exit()
    {
        enemy.ClearBurrowMound();
        enemy.SetBurrowed(false);
        enemy.SetAgentEnabled(true);
    }

   private void StartRising()
    {
        MummStatsSO stats = enemy.mummyStats;

        riseStart = exitPoint + Vector3.down * stats.burrowDepth;
        enemy.transform.position = riseStart;

        enemy.ClearBurrowMound();
        enemy.SetBurrowed(false);

        if(enemy.playerTarget != null)
        {
            enemy.QuickFace(enemy.playerTarget.position);
        }

        enemy.DoEmergeBurst(exitPoint);

        phase= Phase.Rising;
        timer =0f;
    }
}
