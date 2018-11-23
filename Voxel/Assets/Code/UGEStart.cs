using System.Collections.Generic;
using UnityEngine;
using ZTool.Res;

public class UGEStart
{
    public static void Init()
    {
        //设置资源加载相关的常量
        RuntimePlatform platform = RuntimePlatform.WindowsPlayer;

#if UNITY_ANDROID
        ConstantConfig.FILESUFF = ".d";
        platform = RuntimePlatform.Android;
#elif UNITY_IOS
        ConstantConfig.FILESUFF = ".i";
        platform = RuntimePlatform.IPhonePlayer;
#else
        ConstantConfig.FILESUFF = ".p";
        platform = RuntimePlatform.WindowsPlayer;
#endif

#if QA || NET || CLOUD

        //关闭日志输出
        TTrace.Enable = false;
        TTrace.ErrorEnable = false;
        //帧率显示控制
        UGE.loadMgr.isDebug = false;
#else
        //关闭日志输出
        TTrace.Enable = false;
        TTrace.ErrorEnable = true;
        //帧率显示控制
        UGE.loadMgr.isDebug = false;
#endif

        //01 -- 开启map load过程中的所有 加载内容
        TResConfig.isSaveMapLoadInfo = true;
        //不加载依赖项的prefab

        //缓存池存储时间
        UGE.poolMgr.life_sec = 30f;
        //销毁的时候，判断玩家距离，判断时间间隔
        UGE.poolMgr.distance_sec = 30f;
        //距离目标多少距离之后销毁
        UGE.poolMgr.target_distance = 30f;
        //AB延迟卸载时间
        UGE.delayDistroy = 10;
        //UGE.mapMgr.bigMapContentLoader.bigMapConfig.isFixFrameLoadSurf = false;
        UGE.mapMgr.bigMapContentLoader.bigMapConfig.isFadeInOut = false;
        //取消队列加载排序
        UGE.mapMgr.bigMapContentLoader.bigMapConfig.isQueueLoadMap = false;
        //同一帧同一地块同时加载的草的数量
        UGE.mapMgr.bigMapContentLoader.bigMapConfig.frame = 0;
        //队列加载个数
        UGE.mapMgr.loadQueueMgr.loadMaxCount = 1;

        //设置资源使用的时候在加载
        UGE.mapMgr.bigMapContentLoader.bigMapConfig.isDownLoadMapData = false;
        //UGE.mapMgr.bigMapContentLoader.bigMapConfig.terrainLayer = (int)SpriteLayerEnum.Ground;
        //UGE.mapMgr.bigMapContentLoader.bigMapConfig.isOnlyLoadTerrain = true;
        //UGE.mapMgr.bigMapContentLoader.bigMapConfig.circleNum = 3;
        //设置不同层级的资源加载不同的圈数

        ////设置场景资源数据加载范围
        //SceneObjectLoadManager.instance.SetSceneObjetLoadFromQuality();

        //设置地形加载完成就可以进入游戏
        UGE.mapMgr.bigMapContentLoader.bigMapConfig.isNeedSurfaceLoadOver = false;
        UGE.mapMgr.isSaveToHeiht = false;
        UGE.mapMgr.bigMapContentLoader.bigMapConfig.isDestroyEff = false;

        //是否替换材质加载
        UGE.mapMgr.bigMapContentLoader.bigMapConfig.isChangeMaterial = true;
        //是否在加载完所有地表之后在统一替换材质， false 加载单块地表之后替换 true 加载完所有地表在替换
        UGE.mapMgr.bigMapContentLoader.bigMapConfig.isChangeNear = true;
        //在替换附近材质的基础上，设置替换几圈
        UGE.mapMgr.bigMapContentLoader.bigMapConfig.changeNearCircle = 1;

        //Add Wade 7.19+
        UGE.runtimePlatform = platform;

        //设置路径
        //remote
        TResConfig._remoteUrl = ConstantConfig.RESSERVERURL;
        //streamingAssets
        TResConfig._buildinWwwUrl = ConstantConfig.LOCAL_URL;
        TResConfig._buildinUrl = ConstantConfig._buildinUrl;
        //disk
        TResConfig._diskDir = ConstantConfig.CACHE_PATH;

        Debug.Log("UGE STARTUP....\t  " +
            "TResConfig._remoteUrl=" + TResConfig._remoteUrl + "/t" +
            "TResConfig._buildinWwwUrl=" + TResConfig._buildinWwwUrl + "/t" +
            "TResConfig._buildinUrl=" + TResConfig._buildinUrl + "/t" +
            "TResConfig._diskDir=" + TResConfig._diskDir + "/t"
        );

        //设置ver中介
        UGE.loadMgr.SetRevealAction(ResLoadManager.instance.GetResStateType1, SaveVer);
        //设置依赖关系中介
        UGE.loadMgr.SetRevealDependentAction(ResManifest.instance.GetAllDependencies);
        //设置 加载卸载 中介
        UGE.loadMgr.SetLoaderAction(RequestLoaderEquals);

        //设置 UGE 启动的各种启动提示
        UGE.loadMgr.SetStartupCallbackAction(OnChangeLoadingContent, OnDownProgress, OnLocalProgress);
        //加载错误 提示
        UGE.loadMgr.SetFailCallbackAction(OnLoaderFailCallback);

        //设置，是否下载(提示用户wifi等网络情况)
        //UGE.loadMgr.SetIsContinueDownloadFunc(OnIsContinueDownload);

        TResConfig.isOpenProgress = true;

        //uge简易启动

        //UGE.Startup(
        //    OnStartupComplete,
        //    new List<string>() { string.Format("game/map/map_allshader{0}", ConstantConfig.FILESUFF) }
        //);

        UGE.Startup(OnStartupComplete, null);
    }

    private static void SaveVer(FileVer fileVer)
    {
        //???  只传path就行  ???

        //Debug.LogError(" update file info " + fileVer.name + "       " + fileVer.state);

        ResVerManager.instance.UpdateFileInfo(fileVer.name);
    }

    private static void OnStartupComplete()
    {

    }
    /// <summary>
    /// 下载 是否提示判断       //Add Wade 1.25+
    /// </summary>
    private static bool OnIsContinueDownload()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {//todo jh UI
            //提示用户连网 -- 弄个统一方法(心跳 那个也可以用)
            Debug.Log("请链接网络");
            //LocalAlertManager.GetInstance().Show("请链接网络继续下载", true, "重 试", OnClickOk);
        }
        else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
        {//todo jh UI
            //wifi / 有线网络   //不需要弹面板，直接下载 即可
            return true;
        }
        else
        {//todo jh UI
            //dll+ 2017/9/19 暂时用Alert
            // Alert.ShowNewAlert("没有连接wifi，将通过数据流量下载，是否下载？", ContinueDownloadNoWifi);

            //网络可通过载波数据网络连接。
            //--提示用户，是否继续下载
            //确认后 调用方法 -》 UGE.loadMgr.ContinueDownload();
            return true;
        }
        return false;
    }

    /// <summary>
    /// 加载模块 报错 回调          //Add Wade 1.25+ Update 修改流程
    /// </summary>
    private static void OnLoaderFailCallback(TLoaderFailInfo failInfo)
    {
        //是否联网，如果根本没有联网，那就直接报网络             //放入Update，及时检测
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            //无法连接到网络	-	Network unreachable
            Debug.LogError("---------- Loader Error. 无法连接到网络    Network unreachable");
            /**
             * 应该弹出提示，让用户联网。
             **/
            //todo dll UI
            //dll+ 2017/9/19 暂时用Alert
            //  Alert.ShowNewAlert("无法连接到网络");
            //LocalAlertManager.GetInstance().Show("请链接网络继续下载", true, "重 试", OnClickOk);

            return;
        }
        if (UGE.loadMgr.isStartupComplete == false)
        {
            //加载模块，启动失败。 就继续启动，因为网络没有问题。如果网络有问题已经在上面方法停下来了。
            //  UGE.Restartup();
            //  return;
        }
        //联网了，获取不到服务器 -》如果停服了，应该是从服务器获取路径都出问题了，从那里就知道停服了
        if (failInfo.responseHeadersCount == 200)
        {
            //下面就是判断程序返回的了
            switch (failInfo.type)
            {
                case 1:			//1 下载404	- source				-	基本不会出现 - 服务器ver和file没对上	-	让用户重新登录
                    break;
                case 2:			//2 hash错误	- source			-	基本不会出现 - 解决方案，删除本地ver，让其重新登录游戏
                    break;
                case 3:			//3 本地无此文件 -> 只是用于ver		-	基本不可能	 - 客户端streamingAssets中ver丢失了!这个不可能!
                    break;
                case 4:			//4 buildin无此文件	disk无此文件-> local	//需要重新整理本地ver	//运行时	//提示重新登录，修复bug
                    SetLocalVerify(1);

                    /* *
                     * -》弹UI，告知用户，本地资源有误，重新启动
                     * 确定后-》
                     *      重新让用户登录
                     *        3D
                     *          清理Map
                     *          清理sprite
                     *          清理effect
                     *          清理剧情
                     *          清理声音
                     *        2D
                     *          UIManager-》UI
                     *          显示loading
                     *          UGE.重启
                    * */
                    //todo dll socket重连
                    //dll+ 2017/9/19 只重启UGE，全部清理会出现问题
                    //注释掉，导致无限循环
                    // UGE.Restartup();

                    break;
            }
            //		Debug.Log("GameSavePrefs.GetLocalVerify()=" + GameSavePrefs.GetLocalVerify());
            Debug.LogError("---------- Loader Error. failInfo=" + failInfo.path + "|" + failInfo.type + "|" + failInfo.info + "|" + failInfo.responseHeadersCount);
            return;
        }
        else
        {
            //无法连接到服务器	-	Connection refused  //0无法连接服务器 | 404找不到文件 | 501等等错误
            Debug.LogError("---------- Loader Error. 无法连接到服务器    Connection refused    failInfo=" + failInfo.path + "|" + failInfo.type + "|" + failInfo.info + "|" + failInfo.responseHeadersCount);
            //todo dll UI
            //dll+ 2017/9/19 暂时用Alert
            //   Alert.ShowNewAlert("无法连接到服务器");
        }
    }

    const string LOCAL_VERIFY = "LOCAL_VERIFY";
    /// <summary>
    /// 设置本地需要验证disk
    /// </summary>
    /// <param name="need">大于0，是需要； 小于等于0都是不需要</param>
    public static void SetLocalVerify(int need)
    {
        PlayerPrefs.SetInt(LOCAL_VERIFY, need);
    }

    private static void OnChangeLoadingContent(TLoaderStartupEnum startupEnum)
    {
        string title = "";
        //string tip = "不会参数任何流量";
        switch (startupEnum)
        {
            case TLoaderStartupEnum.VER_START:
                title = "验证资源...";
                break;
            case TLoaderStartupEnum.VER_END:
                title = "验证资源完成";
                break;
            case TLoaderStartupEnum.LOCAL_VERIFY_START:
                title = "修正本地资源错误...";
                break;
            case TLoaderStartupEnum.LOCAL_VERIFY_END:
                title = "修正本地资源错误完成";
                break;
            case TLoaderStartupEnum.DOWNLOAD_START:
                title = "下载资源中...";
                break;
            case TLoaderStartupEnum.DOWNLOAD_END:
                title = "下载资源完成";
                break;
            case TLoaderStartupEnum.DEPENDENT_START:
                title = "加载依赖文件...";
                break;
            case TLoaderStartupEnum.DEPENDENT_END:
                title = "加载依赖文件完成";
                break;
            case TLoaderStartupEnum.PREHEAD_SHADER_START:
                title = "预热shader...";
                break;
            case TLoaderStartupEnum.PREHEAD_SHADER_END:
                title = "预热shader完成";
                break;
        }
        //			LoadingManager.GetInstance().UpdateTextProgress(title);	//"正在加载资源......"
        //			LoadingManager.GetInstance().UpdateTextTip(tip);	//"Tip===============..."
        Debug.Log("title=" + title);
    }

    /// <summary>
    /// 资源加载进度的回调
    /// </summary>
    private static void OnDownProgress(int curSize, int needSize, int current, int total)
    {
        //		switch(startupEnum){
        //		case TLoaderStartupEnum.DOWNLOAD_PROGRESS:
        //			break;
        //		}
        //			LoadingManager.GetInstance().UpdateProgress((float)current / total);
        var tip = string.Format("{0}/{1}", curSize, needSize);
        //			LoadingManager.GetInstance().UpdateTextTip(tip);
        Debug.Log("tip=" + tip);
    }
    private static void OnLocalProgress(TLoaderStartupEnum startupEnum, int current, int total)
    {
        //		switch(startupEnum){
        //		case TLoaderStartupEnum.DEPENDENT_PROGRESS:
        //		case TLoaderStartupEnum.PREHEAD_SHADER_PROGRESS:
        //			break;
        //		}
        //			LoadingManager.GetInstance().UpdateProgress((float)current / total);
        var tip = string.Format("{0}/{1}", current, total);
        //Debug.Log("tip=" + tip);
    }

    /// <summary>
    /// </summary>
    /// <param name="tLoaderRequest"></param>
    /// <param name="obj">tgame中间的回调方法</param>
    /// <returns></returns>
    public static bool RequestLoaderEquals(TLoaderRequest tLoaderRequest, object obj)
    {
        ResourcesLoad.RequestLoadData data = tLoaderRequest.param as ResourcesLoad.RequestLoadData;
        if (data == null)
        {
            //Debug.Log("UGE====Main.RequestLoaderEquals, data为null，说明是grouploader，此时就直接判断obj是否为方法即可  path=" + tLoaderRequest.path);
            //data为null，说明是grouploader，此时就直接判断obj是否为方法即可
            if (tLoaderRequest.onLoad.Equals(obj))
            {
                return true;
            }
            return false;
        }
        object callback = null;
        if (data.callbackOne != null)
        {
            callback = data.callbackOne;
        }
        else if (data.callbackAll != null)
        {
            callback = data.callbackAll;
        }
        if (callback == null)
        {
            //Debug.Log("UGE====Main.RequestLoaderEquals, callback为null, path=" + tLoaderRequest.path);
            return false;
        }
        if (callback.Equals(obj))
        {
            return true;
        }
        return false;
    }
}
