using ZLib;

/// <summary>
/// 加载Error的错误状态
/// @author Ollydbg
/// @date 2018-2-11
/// </summary>
public class LoadErrorState : IState<LancherContext>
{
    public LancherContext context
    {
        get;
        private set;
    }

    private StateMachine<LancherContext> sm;

    public LoadErrorState(StateMachine<LancherContext> sm, LancherContext context)
    {
        this.context = context;
        this.sm = sm;
    }

    public void OnDestroy()
    {
    }

    public void OnEnter()
    {
        context.netErrorPanel.ShowWithType(LancherErrorType.NetSlow);
    }

    public void OnExecute()
    {
    }

    public void OnExit()
    {
    }

    public void OnUpdate()
    {
    }
}
