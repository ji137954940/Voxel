using ZLib;
/// <summary>
/// 显示公告信息的状态
/// @author Ollydbg
/// @date 2018-2-11
/// </summary>
public class ShowNoticeState : IState<LancherContext>
{
    public LancherContext context
    {
        get;
        private set;
    }

    private StateMachine<LancherContext> sm;

    public ShowNoticeState(StateMachine<LancherContext> sm, LancherContext context)
    {
        this.context = context;

        this.sm = sm;
    }


    public void OnDestroy()
    {
    }

    public void OnEnter()
    {
        //显示面板
        context.noticePanel.ShowType(LancherNoticeShowType.Notice);
        
        //播放背景音乐
        CodeBridgeTool.instance.PlayEnterBGM(303);
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