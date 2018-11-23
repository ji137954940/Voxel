using System;
using System.Collections.Generic;
using ZLib;

/// <summary>
/// code 代码和 built-in 代码桥接工具
/// </summary>
public class CodeBridgeTool : Singleton<CodeBridgeTool>
{
    #region 资源预加载

    /// <summary>
    /// dll加载回调设置
    /// </summary>
    Action dll_res_load_action;
    public void SetDllResLoadAction(Action action)
    {
        dll_res_load_action = action;
    }

    /// <summary>
    /// dll资源加载数据更新
    /// </summary>

    public void UpdateDllResLoadAction()
    {
        if (dll_res_load_action != null)
            dll_res_load_action();
    }

    /// <summary>
    /// 配表资源加载解析进度设置回调
    /// </summary>
    Action<float> table_config_load_action;
    public void SetTableConfigLoadAction(Action<float> action)
    {
        table_config_load_action = action;
    }

    /// <summary>
    /// 更新配表资源加载解析进度
    /// </summary>
    /// <param name="p"></param>
    public void UpdateTableConfigLoadAction(float p)
    {
        if (table_config_load_action != null)
            table_config_load_action(p);
    }

    /// <summary>
    /// 角色界面资源加载回调
    /// </summary>
    Action<float> role_view_res_load_action;
    public void SetRoleViewResLoadAction(Action<float> action)
    {
        role_view_res_load_action = action;
    }

    /// <summary>
    /// 角色界面资源加载数据更新
    /// </summary>
    /// <param name="p"></param>
    public void UpdateRoleViewResLoadAction(float p)
    {
        if (role_view_res_load_action != null)
            role_view_res_load_action(p);
    }

    //资源预加载类型
    public enum PRE_LOAD_ENUM
    {
        CONFIG,                             //配置数据
        ROLE_RES,                           //角色界面资源
        SCENE,                              //场景资源数据
    }

    #endregion

    #region Loading 数据更新

    //load信息
    public enum LoadMessageTypeEnum
    {
        None = 0,
        Config = 1,                                             //配置数据，资源预加载
        Scene = 2,                                              //场景切换
        Return_Role_View = 3,                                   //返回角色列表
    }

    /// <summary>
    /// 设置loading进度回调（主要是完成的时候）
    /// </summary>
    Action<LoadMessageTypeEnum> load_progress_action;
    public void SetLoadProgressAction(Action<LoadMessageTypeEnum> action)
    {
        this.load_progress_action = action;
    }

    /// <summary>
    /// 更新loading进度回调（主要是完成的时候）
    /// </summary>
    /// <param name="obj"></param>
    public void UpdateLoadProgressAction(LoadMessageTypeEnum type)
    {
        if (load_progress_action != null)
            load_progress_action(type);
    }

    /// <summary>
    /// 设置loading 界面进度刷新回调
    /// </summary>
    Action<float> loading_view_progress_action;
    public void SetLoadingViewProgressAction(Action<float> action)
    {
        loading_view_progress_action = action;
    }

    /// <summary>
    /// loading 界面进度刷新
    /// </summary>
    /// <param name="f"></param>
    public void UpdateLoadingViewProgressAction(float f)
    {
        if (loading_view_progress_action != null)
            loading_view_progress_action(f);
    }

    /// <summary>
    /// 设置loading 界面进度刷新回调
    /// </summary>
    Action<bool, bool> loading_view_restart_action;
    public void SetLoadingViewRestartAction(Action<bool, bool> action)
    {
        loading_view_restart_action = action;
    }

    /// <summary>
    /// loading 界面进度刷新
    /// </summary>
    /// <param name="f"></param>
    public void UpdateLoadingViewRestartAction(bool real_start, bool is_restart_load)
    {
        if (loading_view_restart_action != null)
            loading_view_restart_action(real_start, is_restart_load);
    }

    //loading界面是否处于打开状态
    Func<bool> loading_is_open_func;

    public void SetLoadingIsOpenFunc(Func<bool> func)
    {
        loading_is_open_func = func;
    }

    //打开网络错误异常界面
    Action<bool> show_net_error_window;
    public void SetShowNetErrorWindow(Action<bool> action)
    {
        show_net_error_window = action;
    }

    public void ShowNetErrorWindow(bool is_net_error)
    {
        if (show_net_error_window != null)
        {
            show_net_error_window(is_net_error);
        }
    }

    //关闭网络错误异常界面
    Action<bool, bool> close_net_error_window;
    public void SetCloseNetErrorWindow(Action<bool, bool> action)
    {
        close_net_error_window = action;
    }

    public void CloseNetErrorWindow(bool check_net_type, bool is_net_error)
    {
        if (close_net_error_window != null)
        {
            close_net_error_window(check_net_type, is_net_error);
        }
    }


    public bool LoadingIsOpen()
    {
        if (loading_is_open_func != null)
            return loading_is_open_func();

        return false;
    }

    #endregion

    #region 打开界面信息数据

    /// <summary>
    /// 窗口类型
    /// </summary>
    public enum EnumPreWindow
    {
        Logo = 0,                                       //logo
        Loading = 1,                                    //loading
        Before_game,                                    //公告
        PhoneType_Filter,                               //手机限制提示
        NetError,                                       //网络错误
        Update,                                         //更新客户端
    }

    #endregion

    #region 发布版本号

    //发布版本号
    string build_num;

    /// <summary>
    /// 更新发布版本号数据
    /// </summary>
    /// <param name="str"></param>
    public void SetUpdateBulidNum(string str)
    {
        if (string.IsNullOrEmpty(str))
            return;

        build_num = str;
        //int.TryParse(str, out build_num);
    }

    /// <summary>
    /// 获取发布版本号
    /// </summary>
    /// <returns></returns>
    public string GetBuildNum()
    {
        return build_num;
    }

    #endregion


    #region API桥接
    /// <summary>
    /// 显示登录界面 在执行入口后 会被注入实现，然后就可以在Lancher随便调用了
    /// </summary>
    public Action showLoginWindow;

    /// <summary>
    /// 在UGE引擎部分初始化以后回调回来的.这部分以后 Lancher就可以被销毁了.
    /// </summary>
    public Action UGECallBack;

    /// <summary>
    /// 屏幕还原原始分辨率
    /// </summary>
    public Action ScreenSetting_revertToAct = null;

    #endregion

    /// <summary>
    /// 需要读取的本地资源列表
    /// 在调用Main的开始以前 这里面应该被填充上内容
    /// </summary>
    public string localResText;

    /// <summary>
    /// 游戏预先加载是否完毕
    /// </summary>
    public bool GamePreLoadComplete { get; set; }

    /// <summary>
    /// 登录界面打开了 会这是这个状态为true 关闭了 会设置为false 
    /// 是给Lancher用的 防止那边的界面过早的关闭
    /// </summary>
    public bool LoginPanelOpen;

    Action<string, Action, bool> play_video_action;

    /// <summary>
    /// 设置播放视频信息数据
    /// </summary>
    /// <param name="play_video_action"></param>
    public void SetPlayVideoInfo(Action<string, Action, bool> play_video_action)
    {
        this.play_video_action = play_video_action;
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="_id"></param>
    public void PlayEnterBGM(int _id)
    {
        if (playEnterBGM != null)
        {
            playEnterBGM(_id);
        }
        else
        {
            Debug.LogError("并没有执行成功");
        }
    }

    Action<int> playEnterBGM;

    /// <summary>
    /// 设置播放背景音乐的委托
    /// </summary>
    /// <param name="_playBGMAction"></param>
    public void SetPlayBGMAction(Action<int> _playBGMAction)
    {
        playEnterBGM = _playBGMAction;
    }

    /// <summary>
    /// 播放视频数据
    /// </summary>
    /// <param name="path"></param>
    /// <param name="action"></param>
    /// <param name="is_skip"></param>
    public void PlayVideo(string path, Action action, bool is_skip)
    {
        if (play_video_action != null)
            play_video_action(path, action, is_skip);
    }

    /// <summary>
    /// 进度提示
    /// </summary>
    /// <param name="currentCount"></param>
    /// <param name="allCount"></param>
    public void OnProgress(int currentCount, int allCount)
    {

    }

    /// <summary>
    /// 任务进度
    /// </summary>
    private Dictionary<TaskID, TaskInfo> tasks = new Dictionary<TaskID, TaskInfo>();

    /// <summary>
    /// 标记任务进度
    /// </summary>
    /// <param name="id"></param>
    /// <param name="progress"></param>
    public void SignTaskProgress(TaskID id, float progress)
    {
        if (tasks.Count == 0)
        {
            foreach (TaskID item in Enum.GetValues(typeof(TaskID)))
            {
                tasks.Add(item, new TaskInfo()
                {
                    id = item,
                    progress = 0,
                    maxProgress = item.GetRadio()
                });
            }
        }

        TaskInfo tinfo;

        if (tasks.TryGetValue(id, out tinfo))
        {
            tinfo.progress = progress;
        }
    }

    /// <summary>
    /// 移除全部任务
    /// </summary>
    public void RemoveAll()
    {
        tasks.Clear();
    }

    private Action<TaskID, string> callBack;


    public void SetNotifyTaskErrorCallBack(Action<TaskID, string> callBack)
    {
        this.callBack = callBack;
    }
    /// <summary>
    /// 通知一个任务错误
    /// </summary>
    public void NotifyTaskError(TaskID id, string error)
    {
        if (callBack != null)
        {
            callBack(id, error);
        }
        else
        {
            Debug.LogError("没有订阅加载进度消息");
        }
    }

    /// <summary>
    /// 查询总的任务的进度 0-1的一个值
    /// </summary>
    /// <returns></returns>
    public float QueryTaskProgress()
    {
        var progress = 0f;

        var maxProgress = 0f;

        foreach (var m in tasks)
        {
            maxProgress += m.Value.maxProgress;
        }

        foreach (var m in tasks)
        {
            progress += m.Value.progress * (m.Value.maxProgress / maxProgress);
        }

        return progress;
    }


}

/// <summary>
/// 为枚举写的扩展方法
/// </summary>
public static class EnumExtend
{
    /// <summary>
    /// 在枚举上获取RadioAttribute上面的值
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static float GetRadio(this TaskID value)
    {
        Type type = value.GetType();
        string name = Enum.GetName(type, value);

        if (string.IsNullOrEmpty(name))
        {
            throw new Exception("未知原因没找到类型");
        }

        var field = type.GetField(name);
        var attribute = Attribute.GetCustomAttribute(field, typeof(RadioAttribute)) as RadioAttribute;

        if (attribute == null)
        {
            return RadioAttribute.defaultValue;
        }
        return attribute.radio;
    }
}
/// <summary>
/// 比例Attribute
/// </summary>
public class RadioAttribute : Attribute
{
    internal static float defaultValue = 1;

    public float radio;

    public RadioAttribute(float radio)
    {
        this.radio = radio;
    }
}
/// <summary>
/// 前置需要做的任务
/// </summary>
public enum TaskID
{
    /// <summary>
    /// 加载本地配置文件
    /// </summary>
    LoadLocalConfig,

    /// <summary>
    /// 加载服务器配置文件
    /// </summary>
    LoadServerConfig,

    /// <summary>
    /// 加载决定读取本地或者远程的配置文件
    /// </summary>
    LoadResConfig,

    /// <summary>
    /// 加载logo视频
    /// </summary>
    LoadLogoVideo,

    /// <summary>
    /// 加载DLL
    /// </summary>
    LoadDll,
    /// <summary>
    /// 加载ResVersion
    /// </summary>
    ResVersion,
    /// <summary>
    /// 加载ResManifest
    /// </summary>
    ResManifest,
    /// <summary>
    /// 加载主摄像机
    /// </summary>
    LoadMainCamera,

    /// <summary>
    /// 预热Shader
    /// </summary>
    LoadShader,
    [Radio(50)]
    /// <summary>
    /// 加载静态配置表
    /// </summary>
    LoadStaticConfig,
    /// <summary>
    /// 初始化FMod
    /// </summary>
    FMODInit,

    [Radio(50)]
    /// <summary>
    /// 预加载资源
    /// </summary>
    PreLoad,
}

/// <summary>
/// 任务信息
/// </summary>
class TaskInfo
{
    public TaskID id;

    public float progress;

    public float maxProgress;
}