using ZLib;

/// <summary>
/// 真正进入游戏的状态 
/// 这个状态负责清理Lancher
/// 想法是把整个Lancher的数据全部清除掉
/// 后面游戏里面如果继续需要 就再写一个 省掉交互的过程
/// 这个过程还是很容易产生BUG的 增加很多认知上的困难！
/// @author Ollydbg
/// @date 2018-2-11
/// </summary>
public class EnterGameState : IState<LancherContext>
{
    public LancherContext context
    {
        get;
        private set;
    }

    private StateMachine<LancherContext> sm;

    public EnterGameState(StateMachine<LancherContext> sm, LancherContext context)
    {
        this.context = context;
        this.sm = sm;
    }

    public void OnDestroy()
    {
    }

    public void OnEnter()
    {
        if (CodeBridgeTool.instance.showLoginWindow != null)
        {
            CodeBridgeTool.instance.showLoginWindow();
        }
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
