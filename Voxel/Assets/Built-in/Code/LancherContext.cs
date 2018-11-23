using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using ZLib;
/// <summary>
/// 启动器上下文
/// 
/// 这里面需要在协调状态与UI之间的交互
/// 在这里没有必要开启框架来解决问题
/// 也存储了一些在状态里产生的数据
/// @author Ollydbg
/// @date 2018-2-9
/// </summary>
public class LancherContext
{
    /// <summary>
    /// 生成的Tgame的server信息
    /// </summary>
    public TgameSvrInfo serverInfo;

    /// <summary>
    /// 当前游戏版本号
    /// </summary>
    public string localGameVersion;

    /// <summary>
    /// 是否需要更新客户端
    /// </summary>
    public bool needUpdateGame;


    public List<LancherBasePanel> lancherUIList;

    /// <summary>
    /// 读取本地配置文件读到的HTTP服务器的地址
    /// 需要从这里下载配置文件
    /// </summary>
    internal string serverURL;

    /// <summary>
    /// 附着的GameObject
    /// </summary>
    internal GameObject gameObject;

    #region 加载进度信息 每个都对应着一个加载进度

    /// <summary>
    /// 加载服务器配置信息的进度
    /// </summary>
    public LancherProgressHolder loadingServerConfigProgress;

    /// <summary>
    /// 加载DLL的进度
    /// </summary>
    public LancherProgressHolder loadingDLLProgress;

    /// <summary>
    /// 加载资源配置的进度
    /// </summary>
    public LancherProgressHolder loadingResourcesConfigProgress;

    /// <summary>
    /// 加载 Logo 视频进度信息
    /// </summary>
    public LancherProgressHolder loadingLogoVideoProgress;

    private StateMachine<LancherContext> machine;

    internal Assembly nocodeAssembly;

    #endregion
    /// <summary>
    /// 
    /// </summary>
    public void Init(StateMachine<LancherContext> _machine)
    {
        CodeBridgeTool.instance.SetNotifyTaskErrorCallBack(TaskErrorCallBack);

        machine = _machine;

        //加载进度把控 每次都是需要初始化最新的
        loadingServerConfigProgress = new LancherProgressHolder(LancherProgressType.LoadServerConfig);

        loadingDLLProgress = new LancherProgressHolder(LancherProgressType.LoadDLL);

        loadingResourcesConfigProgress = new LancherProgressHolder(LancherProgressType.LoadResConfig);

        loadingLogoVideoProgress = new LancherProgressHolder(LancherProgressType.LoadLogoVido);

        lancherUIList = new List<LancherBasePanel>();

        lancherUIList.Add(new LancherLogoPanel());

        lancherUIList.Add(new LancherLoadingPanel());

        lancherUIList.Add(new LancherNoticePanel());

        lancherUIList.Add(new LancherPhoneErrorPanel());

        lancherUIList.Add(new LancherNetErrorPanel());

        lancherUIList.Add(new LancherUpdatePanel());

        for (int i = 0; i < lancherUIList.Count; i++)
        {
            lancherUIList[i].machine = _machine;

            lancherUIList[i].context = this;

            lancherUIList[i].Init();
        }
    }

    /// <summary>
    /// 加载异常出现后的处理
    /// 显示NetSlow面板 暂停Loading条
    /// </summary>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    private void TaskErrorCallBack(TaskID arg1, string arg2)
    {
        if (loadingPanel.isOpen)
        {

            loadingPanel.PauseProgress = true;

            loadingPanel.Hide();
        }

        netErrorPanel.ShowWithType(LancherErrorType.NetSlow);
    }

    public LancherLogoPanel logoPanel
    {
        get
        {
            return lancherUIList[(int)CodeBridgeTool.EnumPreWindow.Logo] as LancherLogoPanel;
        }
    }

    /// <summary>
    /// 重置全部
    /// 然后重新开始
    /// </summary>
    internal void ResetAll()
    {
        this.machine.ChangeState<ShowLogoState>();
    }

    public LancherLoadingPanel loadingPanel
    {
        get
        {
            return lancherUIList[(int)CodeBridgeTool.EnumPreWindow.Loading] as LancherLoadingPanel;
        }
    }

    public LancherNoticePanel noticePanel
    {
        get
        {
            return lancherUIList[(int)CodeBridgeTool.EnumPreWindow.Before_game] as LancherNoticePanel;
        }
    }

    public LancherPhoneErrorPanel phoneErrorPanel
    {
        get
        {
            return lancherUIList[(int)CodeBridgeTool.EnumPreWindow.PhoneType_Filter] as LancherPhoneErrorPanel;
        }
    }

    public LancherNetErrorPanel netErrorPanel
    {
        get
        {
            return lancherUIList[(int)CodeBridgeTool.EnumPreWindow.NetError] as LancherNetErrorPanel;
        }
    }

    public LancherUpdatePanel updatePanel
    {
        get
        {
            return lancherUIList[(int)CodeBridgeTool.EnumPreWindow.Update] as LancherUpdatePanel;
        }
    }

    /// <summary>
    /// 销毁Lancher的回调
    /// </summary>
    internal Action callDispose;

    /// <summary>
    /// 销毁掉
    /// </summary>
    public void Dispose()
    {
        for (int i = 0; i < lancherUIList.Count; i++)
        {
            lancherUIList[i].OnDestory();
        }

        lancherUIList.Clear();

        lancherUIList = null;
    }
}
