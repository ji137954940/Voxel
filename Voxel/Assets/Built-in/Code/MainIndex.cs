using System;
using UnityEngine;
using ZLib;

/// <summary>
/// GAME 启动入口
/// 主要是发送get请求 获取登录服务器和资源服务器地址信息,并设置设备分辨率和UI分辨率
/// </summary>
public partial class MainIndex : MonoBehaviour
{
    /// <summary>
    /// 启动状态机
    /// </summary>
    private StateMachine<LancherContext> stateMachine;

    /// <summary>
    /// 启动器上下文
    /// </summary>
    private LancherContext context;

    private void Awake()
    {
        //屏幕旋转的限制
        Screen.orientation = ScreenOrientation.AutoRotation;

        Screen.autorotateToLandscapeLeft = false;

        Screen.autorotateToLandscapeRight = false;

        Screen.autorotateToPortrait = true;

        Screen.autorotateToPortraitUpsideDown = false;

        //如果是移动平台,初始化bugly
#if !UNITY_STANDALONE
        //InitBugly();
#endif
        InitStateMachine();
    }

    private void Start()
    {
        InitUWA();

        OnAwakeScreen();//修改获取设备分级后再做屏幕修改
    }


    /// <summary>
    /// 初始化状态机
    /// </summary>
    public void InitStateMachine()
    {
        context = new LancherContext();

        context.gameObject = this.gameObject;

        context.gameObject.AddComponent<LancherCoroutine>();

        stateMachine = new StateMachine<LancherContext>(context);

        stateMachine.changeType = StateChangeType.Immediately;

        context.Init(stateMachine);

        context.callDispose = Dispose;

        stateMachine.AddState(new ShowLogoState(this.stateMachine, this.context));

        stateMachine.AddState(new EnterGameState(this.stateMachine, this.context));

        stateMachine.AddState(new NetErrorState(this.stateMachine, this.context));

        stateMachine.AddState(new PhoneErrorState(this.stateMachine, this.context));

        stateMachine.AddState(new LoadLocalConfigState(this.stateMachine, this.context));

        stateMachine.AddState(new LoadServerConfigState(stateMachine, context));

        stateMachine.AddState(new LoadResConfigState(stateMachine, context));

        stateMachine.AddState(new LoadLogoVideoState(stateMachine, context));

        stateMachine.AddState(new ResolveDLLState(this.stateMachine, this.context));

        stateMachine.AddState(new ShowNoticeState(this.stateMachine, this.context));

        stateMachine.AddState(new LoadErrorState(this.stateMachine, this.context));

        stateMachine.ChangeState<ShowLogoState>();
    }

    public void StateMachineUpdate()
    {
        stateMachine.Update();
    }

    public void Update()
    {
        this.StateMachineUpdate();
    }

    public void Dispose()
    {
        LancherCoroutine.instance.Dispose();

        stateMachine.Destroy();

        context.Dispose();

        LancherUIManager.instance.Dispose();

        UnityEngine.Object.Destroy(this);
    }


    #region 设置屏幕分辨率 涉及方面
    /// <summary>
    /// 如果是移动平台,根据屏幕设置分辨率
    /// </summary>
    private void OnAwakeScreen()
    {
        bool fullscreen = false;

        if (Application.isMobilePlatform)
            fullscreen = true;
        else
            fullscreen = Screen.fullScreen;
        ScreenSetting.Init();
#if UNITY_IPHONE || UNITY_IOS || UNITY_ANDROID
        ScreenSetting.ScreenInch(this.gameObject, fullscreen);
#endif
    }
    #endregion

    #region 初始化Bugly


//    /// <summary>
//    /// 初始化Bugly引擎
//    /// </summary>
//    private void InitBugly()
//    {
//#if QA || DEV
//        //Debug.Log(LogType.Log, "bugly init");
//        //控制是否开启SDK的调试信息打印，接入调试时可设置为true，正式发布版本请设置为false或注释掉
//        BuglyAgent.ConfigDebugMode(false);
//        BuglyAgent.ConfigAutoQuitApplication(false);
//#if QA

//#if UNITY_IPHONE || UNITY_IOS
//    	    BuglyAgent.InitWithAppId ("567d571ee4");                    //qa ios
//#elif UNITY_ANDROID
//            BuglyAgent.InitWithAppId("88bf4ce78c");                     //qa android
//#endif

//#elif NET || Business || OnLine || OnLine_Test

//#if UNITY_IPHONE || UNITY_IOS
//    	    BuglyAgent.InitWithAppId ("29d1ff2f8c");                    //net ios
//#elif UNITY_ANDROID
//            BuglyAgent.InitWithAppId("4478407b61");                     //net android
//#endif

//#else

//#if UNITY_IPHONE || UNITY_IOS
//    	    BuglyAgent.InitWithAppId ("29d1ff2f8c");                     //dev ios
//#elif UNITY_ANDROID
//            BuglyAgent.InitWithAppId("4478407b61");                      //dev android
//#endif
//#endif

//        // 如果你确认已在对应的iOS工程或Android工程中初始化SDK，那么在脚本中只需启动C#异常捕获上报功能即可
//        BuglyAgent.EnableExceptionHandler();
//#if UNITY_ANDROID
//        this.gameObject.AddComponent<WeTest.U3DAutomation.U3DAutomationBehaviour>();
//        BuglyAgent.RegisterLogCallback(WeTest.U3DAutomation.CrashMonitor._OnLogCallbackHandler);
//        this.gameObject.AddComponent<Wetest_FPS>();
//#endif
//#endif
//    }
    
    
    
    #endregion

    #region UWA

    /// <summary>
    /// 初始化 UWA 插件信息数据
    /// </summary>
    private void InitUWA()
    {
#if UNITY_ANDROID && UWA
        GameObject go = new GameObject("UWA");
        go.AddComponent<UWA.GUIWrapper>();
#endif
    }

    #endregion
}

