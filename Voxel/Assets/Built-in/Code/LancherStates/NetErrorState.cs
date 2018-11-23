using ZLib;
/// <summary>
/// 网络错误的状态
/// @author Ollydbg
/// @date 2018-2-11
/// </summary>
public class NetErrorState : IState<LancherContext>
{
    public LancherContext context
    {
        get;
        private set;
    }

    private StateMachine<LancherContext> sm;

    public NetErrorState(StateMachine<LancherContext> sm, LancherContext context)
    {
        this.context = context;
        this.sm = sm;
    }
    public void OnDestroy()
    {
    }

    public void OnEnter()
    {
        context.netErrorPanel.ShowWithType(LancherErrorType.NetError);
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
