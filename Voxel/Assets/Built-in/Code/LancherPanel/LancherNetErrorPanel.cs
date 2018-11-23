/// <summary>
/// 网络错误的时候需要弹出来的界面
/// @author Ollydbg
/// @date 2018-2-10
/// </summary>
public class LancherNetErrorPanel : LancherBasePanel
{
    private NetErrorWindow window;

    public override void Hide()
    {
        base.Hide();

        window.data.net_error_exit_btn.onClick.RemoveAllListeners();

        window.data.net_slow_exit_btn.onClick.RemoveAllListeners();
    }

    public override void Init()
    {
        base.Init();
        this.panelType = CodeBridgeTool.EnumPreWindow.NetError;
    }

    public override void OnDestory()
    {
        base.OnDestory();
        window = null;
    }

    public override void Show()
    {
        base.Show();

        if (!window)
        {
            window = container.GetComponent<NetErrorWindow>();
        }
    }

    /// <summary>
    /// 显示具体的错误类型
    /// </summary>
    /// <param name="_type"></param>
    public void ShowWithType(LancherErrorType _type)
    {
        this.Show();

        switch (_type)
        {
            case LancherErrorType.NetError:
                ShowNetError();
                break;
            case LancherErrorType.NetSlow:
                ShowNetSlow();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 显示网络错误的面板
    /// </summary>
    private void ShowNetError()
    {
        if (window == null)
        {
            Debug.LogWarning("按照预想这里不应该为空.");
            return;
        }

        window.data.net_error_obj.SetActive(true);
        window.data.net_slow_obj.SetActive(false);

        //添加按钮监听
        if (window.data.net_error_exit_btn != null)
        {
            window.data.net_error_exit_btn.onClick.AddListener(OnClickExitBtn);
        }
    }

    /// <summary>
    /// 显示重新开始更新的面板
    /// </summary>
    private void ShowNetSlow()
    {
        if (window == null)
        {
            Debug.LogWarning("按照预想这里不应该为空.");

            return;
        }

        window.data.net_error_obj.SetActive(false);
        window.data.net_slow_obj.SetActive(true);

        //添加按钮监听
        if (window.data.net_slow_exit_btn != null)
        {
            window.data.net_slow_exit_btn.onClick.AddListener(OnClickSlowBtn);
        }
    }

    /// <summary>
    /// 重新开始
    /// </summary>
    private void OnClickSlowBtn()
    {
        this.Hide();

        this.context.ResetAll();
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    private void OnClickExitBtn()
    {
        LancherApplicationUtils.QuitApplication();
    }
}

/// <summary>
/// 错误类型
/// </summary>
public enum LancherErrorType
{
    /// <summary>
    /// 网络错误
    /// </summary>
    NetError,
    /// <summary>
    /// 网络太慢了
    /// </summary>
    NetSlow,
}
