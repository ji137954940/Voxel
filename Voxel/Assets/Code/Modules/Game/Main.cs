using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Color.Number.GameInfo;
using Color.Number.Grid;
using Color.Number.Input;
using Color.Number.Scene;
using FFmpeg;
using UnityEngine;
using UnityEngine.Tilemaps;
using ZFrame;
using ZLib;
using ZTool.Res;
using ZTool.Table;
using ThreadPriority = UnityEngine.ThreadPriority;

/// <summary>
/// 游戏主入口
/// </summary>
public class Main : MonoBehaviour
{

    #region 数据对象

    /// <summary>
    /// 场景中 tile map 对象引用
    /// </summary>
    public TilemapInfo tileMapInfo;

    /// <summary>
    /// 场景中 voxel 信息引用
    /// </summary>
    public VoxelShowInfo voxelShowInfo;

    private IEnumerator currentExecute;

    #endregion

    #region 初始化

    //// Use this for initialization
    //void Start ()
    //{

    //    OnInit();

    //}

    /// <summary>
    /// 数据初始化
    /// </summary>
    public void OnInit()
    {
        Debug.EnableLog = true;

        GameInstance.Main = this;

        CodeBridgeTool.instance.showLoginWindow = ShowLoginWindow;

        //初始化迭代器
        OnInitCoroutine();

        //设置Table和Res信息数据
        OnTableAndResInfo();

        //初始化UGE
        OnInitUGE();

        //初始化 FFmpeg
        OnInitFFmpeg();

        //游戏模块初始化
        var moduleList = new ModuleList();

        //初始化输入操作
        InputManager.instance.Init();

        if (currentExecute == null)
        {
            currentExecute = StartGameState();
            CoroutineManager.instance.StartCoroutine(currentExecute);
        }

        //设置最大线程个数
        ThreadPool.SetMaxThreads(ConstantConfig.MAX_THREAD_COUNT, ConstantConfig.MAX_ASYNC_THREAD_COUNT);
    }

    /// <summary>
    /// 初始化 Coroutine
    /// </summary>
    private void OnInitCoroutine()
    {
        //开始游戏设置
        //设置游戏运行状态为配置阶段
        if (gameObject.GetComponent<CoroutineManager>() == null)
        {
            CoroutineManager.Init(gameObject);
        }
    }

    /// <summary>
    ///     设置Table和Res信息数据
    /// </summary>
    private void OnTableAndResInfo()
    {
        OnSetTableInfo();
        OnSetLoadResInfo();
    }

    /// <summary>
    ///     设置Table管理信息数据资源
    /// </summary>
    private void OnSetTableInfo()
    {
        var tableNameSpace = "Tgame.Game.Table";
        TableManager.SetTableAssemblyAndNameSpace(Assembly.GetExecutingAssembly(), tableNameSpace);
    }

    /// <summary>
    ///     设置资源加载配置信息
    /// </summary>
    private void OnSetLoadResInfo()
    {
        ResConfig.instance.OnSetResExtension(ConstantConfig.RES_EXTENSION, ConstantConfig.MANIFEST_EXTENSION,
            ConstantConfig.CONFIG_EXTENSION);
        ResConfig.instance.OnSetUGEResExtension(ConstantConfig.UGE_CONFIG_TXT, ConstantConfig.UGE_CONFIG_BIN,
            ConstantConfig.FILESUFF);
        ResConfig.instance.OnSetResGroupInfo(ConstantConfig.GROUP_SETUP, ConstantConfig.GROUP_UPDATE, ConstantConfig.GROUP_LOAD);
        ResConfig.instance.OnSetResSaveInfo(ConstantConfig.RES_CACHE_TIME, ConstantConfig.RES_MANI_FEST_NAME,
            ConstantConfig.RES_MANI_FEST_URL, ConstantConfig.RES_LOAD_ASYNC);
        ResConfig.instance.OnSetResUrlInfo(ConstantConfig.RESSERVERURL, ConstantConfig.LOCAL_URL, ConstantConfig.CACHE_URL,
            ConstantConfig.CACHE_PATH);
        ResConfig.instance.OnSetResLoadMaxCount(ConstantConfig.MAX_LOAD_RES_COUNT);
        //ResConfig.instance.SetRecoveryScriptTimeMargin(ConstantConfig.SCRIPT_RECOVERY_TIME_MARGIN);
        //ResConfig.instance.SetCheckScriptTimeMargin(ConstantConfig.CHECK_SCRIPT_RECOVERY_TIME_MARGIN);
        //设置资源加载优先级最高
        ResConfig.instance.SetResLoadThreadPriority(ThreadPriority.High);

        //ResourcesLoad.instance.SetLocalResConfig(GamePreLoadData.instance.Local_Res);
    }

    /// <summary>
    ///     从Lancher那边调用过来的函数 想要打开 main window
    /// </summary>
    private void ShowLoginWindow()
    {
        UIWindowManager.instance.Open<MainWindowModule>();
    }

    /// <summary>
    /// 初始化游戏内部各种数据状态
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartGameState()
    {
        ResVerManager.instance.OnResVerInit(null);

        yield return new WaitResInitComplete();

        CodeBridgeTool.instance.SignTaskProgress(TaskID.ResVersion, 1);

        UGEStart.Init();

        //加载本地资源列表
        if (!string.IsNullOrEmpty(CodeBridgeTool.instance.localResText))
            GamePreLoadData.instance.SetLoadLocalResConfig(CodeBridgeTool.instance.localResText);

        //设置到加载模块
        ResourcesLoad.instance.SetLocalResConfig(GamePreLoadData.instance.Local_Res);

        //初始化资源依赖文件
        ResManifest.instance.Init();

        yield return new WaitResManifestInitComplete();

        CodeBridgeTool.instance.SignTaskProgress(TaskID.ResManifest, 1);

        //初始化所有的shader
        InitShader();

        //派发加载配置表的消息
        Frame.DispatchEvent(ME.New<ME_Init_Config>());

        yield return new WaitResLoadComplete();

        ////重置状态，开始预加载资源
        //CodeBridgeTool.instance.GamePreLoadComplete = false;

        SceneManager.instance.InitSceneInfo();

        //设置手指长按时间
        InputManager.instance.SetEasyTouchLongTapTime();

        //重置状态，开始预加载资源
        CodeBridgeTool.instance.GamePreLoadComplete = true;

    }

    /// <summary>
    ///     初始化UGE
    /// </summary>
    private void OnInitUGE()
    {
        //初始化UGE数据
        var uge = GameObject.Find("Engine");
        if (uge == null)
            uge = new GameObject("Engine");

        if (uge.GetComponent<UGE>() == null) uge.AddComponent<UGE>();
    }

    /// <summary>
    /// 初始化FFmpeg数据
    /// </summary>
    private void OnInitFFmpeg()
    {
        //初始化FFmpeg数据
        var ffmpeg = GameObject.Find("FFmpeg");
        if (ffmpeg == null)
        {
            ffmpeg = new GameObject("FFmpeg");
            ffmpeg.AddComponent<FFmpegWrapper>();
            ffmpeg.AddComponent<UnityThread>();
        }

        var rec = ffmpeg.GetComponent<FFmpegREC>();
        if(rec == null)
        {
            ffmpeg.AddComponent<FFmpegREC>();
            var go = GameObject.Find("ScreenshotCamera");
            var cam = go.GetComponent<Camera>();

            rec.screenshotCam = cam;
        }

        GameInstance.FFmpegREC = rec;

        //var go = Camera.main.gameObject;
        //var rec = go.GetComponent<FFmpegREC>();
        //if (rec == null)
        //    rec = go.AddComponent<FFmpegREC>();

        //GameInstance.FFmpegREC = rec;
    }

    /// <summary>
    ///     预加载Shader
    ///     至于预热不预热的问题 主要看咱们预热要花多长的时间 目前是没有WarmUp的
    /// </summary>
    private void InitShader()
    {
        //var url = string.Format("res/shader{0}", ConstantConfig.RES_EXTENSION);
        //ResourcesLoad.instance.LoadAssetBundle(url, OnShaderLoadDone, null, null, false, true, false);
    }

    /// <summary>
    ///     预加载完毕Shader
    /// </summary>
    /// <param name="path"></param>
    /// <param name="obj"></param>
    /// <param name="parameter"></param>
    private void OnShaderLoadDone(string path, Object obj, object parameter)
    {
        if (string.IsNullOrEmpty(path) || obj == null)
        {
            Debug.LogError(" Shader 预热加载失败！ ");
            return;
        }

        var bundle = obj as AssetBundle;
        if (bundle == null)
        {
            Debug.LogError(" Shader 预热 Bundle 为null ");
            return;
        }

        var shader = bundle.LoadAllAssets<Shader>();

        CodeBridgeTool.instance.SignTaskProgress(TaskID.LoadShader, 1);

        //TODO 考虑 shader的Warmup
        //Shader.WarmupAllShaders();
    }


    private void OnPreLoadComplete(PreLoadResource.PreLoadEnum obj, bool isBroken)
    {
        if (isBroken)
        {
            CodeBridgeTool.instance.NotifyTaskError(TaskID.PreLoad, "预加载失败");

            //PreLoadResource.instance.UnLoad(PreLoadResource.PreLoadEnum.PRE_LOAD_CREATE_ROLE_TYPE, true);
            PreLoadResource.instance.UnLoad((PreLoadResource.PreLoadEnum)obj, true);

        }
        else
        {
            CodeBridgeTool.instance.GamePreLoadComplete = true;
        }
    }

    /// <summary>
    /// 预加载的进度
    /// </summary>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    private void PreLoadOnProgress(int progress, int allCount)
    {
        CodeBridgeTool.instance.SignTaskProgress(TaskID.PreLoad, progress / (float)allCount);
    }

    #endregion

    #region 主线程刷新


    // Update is called once per frame
    void Update ()
    {
        Tick.Update();

        HandleKey();
    }


    void HandleKey()
    {
        if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.Z))
        {
            GridManager.instance.GameLevelGridInfo.AutoFillColor();
        }
    }

    #endregion
}
