using ZLib;

/// <summary>
/// 手机不是我们支持的型号的状态
/// @author Ollydbg
/// @date 2018-2-11
/// </summary>
public class PhoneErrorState : IState<LancherContext>
{
    public LancherContext context
    {
        get;
        private set;
    }

    private StateMachine<LancherContext> sm;

    public PhoneErrorState(StateMachine<LancherContext> sm, LancherContext context)
    {
        this.context = context;
        this.sm = sm;
    }

    public void OnDestroy()
    {
    }

    public void OnEnter()
    {
        this.context.phoneErrorPanel.Show();
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
