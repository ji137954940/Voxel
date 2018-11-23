using System.Collections;
/// <summary>
/// Loading面板的实现
/// 
/// 下面有三种状态 
/// 加载DLL
/// 加载配置文件
/// @author Ollydbg
/// @date 2018-2-10
/// </summary>
public class LancherLoadingPanel : LancherBasePanel
{
    private LoadingWindow1 window;

    public override void Hide()
    {
        base.Hide();
    }

    public override void Init()
    {
        base.Init();
        this.panelType = CodeBridgeTool.EnumPreWindow.Loading;
    }

    public override void OnDestory()
    {
        base.OnDestory();

        window = null;
    }

    public override void Show()
    {
        base.Show();

        PauseProgress = true;

        if (!window)
        {
            window = container.GetComponent<LoadingWindow1>();
        }
        LancherCoroutine.instance.StartCoroutine(CheckAllComplete());
    }

    private bool pauseProgress = false;
    /// <summary>
    /// 暂停进度
    /// </summary>
    public bool PauseProgress
    {
        set
        {
            pauseProgress = value;

            if (window)
                window.SetLoadingUpdate(!pauseProgress);
        }
        get
        {
            return pauseProgress;
        }
    }

    /// <summary>
    /// 检查是否加载结束
    /// </summary>
    /// <returns></returns>
    public IEnumerator CheckAllComplete()
    {
        //检查预加载是否完毕
        do
        {
            yield return null;
        } while (!CodeBridgeTool.instance.GamePreLoadComplete);

        //等Loading差不多走完
        do
        {
            yield return null;
        } while (window.currentProgress < 0.9f);

        OnLoadingComplete();
    }

    /// <summary>
    /// 可以在Panel中检查到所有的加载都完毕以后
    /// </summary>
    private void OnLoadingComplete()
    {

        CodeBridgeTool.instance.RemoveAll();

        //this.machine.ChangeState<ShowNoticeState>();

        this.machine.ChangeState<EnterGameState>();//进入游戏内部

        this.Hide();
    }
}
