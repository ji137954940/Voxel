using UnityEngine;

/// <summary>
/// 网络连接失败提示
/// </summary>
public class NetErrorWindow : MonoBehaviour
{
    public NetErrorData data;

    //是否为网络错误
    public bool is_net_error = false;

    private Action SlowBtnCallBack;

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(bool is_net_error, Action slowBtnCallBack)
    {
        if (data == null)
            return;

        //当前是否为网络断开
        this.is_net_error = is_net_error;

        SlowBtnCallBack = slowBtnCallBack;

        if (is_net_error)
        {
            ShowNetError();
        }
        else
        {
            ShowNetSlow();
        }

    }

    /// <summary>
    /// 显示网络错误（断开）
    /// </summary>
    void ShowNetError()
    {
        if (data == null)
            return;

        data.net_error_obj.SetActive(true);
        data.net_slow_obj.SetActive(false);

        //添加按钮监听
        if (data.net_error_exit_btn != null)
        {
            data.net_error_exit_btn.onClick.AddListener(OnClickExitBtn);
        }
    }

    /// <summary>
    /// 显示网络异常
    /// </summary>
    void ShowNetSlow()
    {
        if (data == null)
            return;

        data.net_error_obj.SetActive(false);
        data.net_slow_obj.SetActive(true);

        //添加按钮监听
        if (data.net_slow_exit_btn != null)
        {
            data.net_slow_exit_btn.onClick.AddListener(OnClickSlowBtn);
        }
    }

    /// <summary>
    /// 点击按钮操作
    /// </summary>
    void OnClickExitBtn()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// 点击按钮退出当前界面
    /// </summary>
    void OnClickSlowBtn()
    {
        if (data != null)
        {
            if (SlowBtnCallBack != null)
            {
                SlowBtnCallBack.Invoke();
            }

            //更新进度信息
            //PreUIWindowManager.instance.UpdateLoadingView();
            //PreUIWindowManager.instance.Close(CodeBridgeTool.EnumPreWindow.NetError);
        }
    }

}
