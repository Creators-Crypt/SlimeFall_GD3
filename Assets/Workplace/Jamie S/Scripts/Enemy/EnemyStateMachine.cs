public class EnemyStateMachine
{
    public IEnemyState currentState;

    public void Initialize(IEnemyState startingState)
    {
        currentState = startingState;
        currentState.Enter();
    }

    public void ChangeState(IEnemyState newState)
    {
        if (newState == currentState) return;
        if(currentState != null)
        {
            currentState.Exit();
        }
        currentState = newState;
        currentState.Enter();
    }

    public void Tick()
    {
        if(currentState != null)
        {
            currentState.Tick();
        }
    }  
}