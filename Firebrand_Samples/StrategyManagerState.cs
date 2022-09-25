using System.Linq;

public abstract class StrategyManagerState
{
    protected StrategyLayerManager _strategyLayerManager;
    protected StrategyManagerState (StrategyLayerManager strategyLayer)
    {
        _strategyLayerManager = strategyLayer;
    }

    /// <summary>
    /// Method is called on transitioning into a state.
    /// </summary>
    public virtual void OnStateEnter()
    {

    }

    /// <summary>
    /// Method is called on transitioning out of a state.
    /// </summary>
    public virtual void OnStateExit()
    {

    }
}
