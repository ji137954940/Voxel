using System.Collections;
using System.Collections.Generic;
using Tgame.Game.Table;
using UnityEngine;
using Color = UnityEngine.Color;

/// <summary>
/// 游戏内部的常量数据配置
/// </summary>
public class ConstantConfig
{

    #region 资源版本数据

    //安装包资源组组名
    public static string GROUP_SETUP = "setup";
    //更新资源组组名
    public static string GROUP_UPDATE = "update";
    //实时加载资源组组名
    public static string GROUP_LOAD = "load";

    ///// <summary>
    ///// 资源服务器地址     //_remoteUrl
    ///// </summary>
    //public static string RESSERVERURL = "http://188.188.0.52:80/xy/XY_DEV/PC/CN_Pro";

#if UNITY_ANDROID

    /// <summary>
    /// 资源服务器地址     //_remoteUrl
    /// </summary>
    public static string RESSERVERURL = "http://krlyv1.res.ts100.com/krly/res/M5B-Business/Business/Business.2018-07-19/ANDROID";

#elif UNITY_IOS
    /// <summary>
    /// 资源服务器地址     //_remoteUrl
    /// </summary>
    public static string RESSERVERURL = "http://krlyv1.res.ts100.com/krly/res/M5B-Business/Business/Business.2018-07-19/IOS";
#else
    /// <summary>
    /// 资源服务器地址     //_remoteUrl
    /// </summary>
    public static string RESSERVERURL = "http://krlyv1.res.ts100.com/krly/res/M5B-Business/Business/Business.2018-07-19/PC";
#endif

    /// <summary>
    /// add Wade 7.19+ cdn地址
    /// </summary>
    public static string _cdnUrl;

#if UNITY_ANDROID

    //Android 路径比较特殊

    //本地路径
    public static string LOCAL_URL = Application.streamingAssetsPath;
    public static string _buildinUrl = string.Format("{0}!assets", Application.dataPath);       // streamingAssets LoadFromFile 加载(用于加载ab资源)
    //缓存路径（url WWW使用）
    public static string CACHE_URL = string.Format("{0}/vercache", Application.persistentDataPath);

#else

    //本地路径（url WWW使用）
    public static string LOCAL_URL = string.Format("file://{0}", Application.streamingAssetsPath);
    public static string _buildinUrl = Application.streamingAssetsPath;       // streamingAssets LoadFromFile 加载(用于加载ab资源)
    //缓存路径（url WWW使用）  此url 在 windows 及 WP IOS  可以使用
    //IOS 下这个路径用cache，避免备份到Icloud
    public static string CACHE_URL = string.Format("file://{0}/vercache", Application.temporaryCachePath);

#endif

#if UNITY_IOS

    //缓存路径（缓存使用 IO 使用）
    public static string CACHE_PATH = System.IO.Path.Combine(Application.temporaryCachePath, "vercache");

#else

    //缓存路径（缓存使用 IO 使用）
    public static string CACHE_PATH = System.IO.Path.Combine(Application.persistentDataPath, "vercache");

#endif

#endregion

#region 资源后缀名

#if UNITY_ANDROID

        //资源后缀名
        public static string RES_EXTENSION = ".aresources";

#elif UNITY_IOS

        //资源后缀名
        public static string RES_EXTENSION = ".iresources";

#else

    //资源后缀名
    public static string RES_EXTENSION = ".tresources";

#endif

    //资源manifest 后缀名
    public static string MANIFEST_EXTENSION = "";
    //配表配置后缀名
    public static string CONFIG_EXTENSION = ".bytes";
    //UGE 配置后缀
    public static string UGE_CONFIG_TXT = ".txt";
    //UGE 地图二进制后缀
    public static string UGE_CONFIG_BIN = ".bin";
    //加载的文件后缀名
    public static string FILESUFF = ".P";

#endregion

#region 资源管理

    //资源没有引用的时候缓存时长（秒）
    public static float RES_CACHE_TIME = 30;

    //资源保存管理配置名字
    public static string RES_MANI_FEST_NAME = "AssetBundleManifest";
    //资源保存管理配置位置
    public static string RES_MANI_FEST_URL = "Data.txt";
    //资源加载是否使用异步加载方式
    public static bool RES_LOAD_ASYNC = false;

#endregion

#region 最大加载的资源个数

    //同时最大加载的资源个数
    public static int MAX_LOAD_RES_COUNT = 20;

#endregion

#region 线程池数据

    //线程池中最大线程个数
    public static int MAX_THREAD_COUNT = 8;

    //线程池总最大异步线程个数
    public static int MAX_ASYNC_THREAD_COUNT = 8;

#endregion

#region UI资源路径

    //UI资源路径
    public static string UI_RES_PATH = "Res/UI/Window/";
    //UI资源图集路径
    public static string UI_ATLAS_PATH = "Res/UI/PackerImg/";

#endregion

    #region Camera

    #region 相机拍照

    /// <summary>
    /// ios 相机截图存放路径
    /// </summary>
    public static string CAMERA_SCREEN_SHOTS_PATH_IOS = string.Format("{0}/Screenshots/", Application.persistentDataPath);

    /// <summary>
    /// pc 相机截图存放路径
    /// </summary>
    public static string CAMERA_SCREEN_SHOTS_PATH_PC = string.Format("{0}/Screenshots/", Application.persistentDataPath);

    #endregion

    #endregion

    #region 视频数据

    /// <summary>
    /// 录屏截图保存目录
    /// </summary>
    public static string FFMPEG_REC_CASH_DIR = System.IO.Path.Combine(Application.temporaryCachePath, "RecordingCash");

    #endregion



    #region 游戏数据常量(服务器，客户端同时使用的)

    /// <summary>
    /// 游戏数据配置字典 float
    /// </summary>
    private static Dictionary<string, float> _gameConfigFloatDic = new Dictionary<string, float>();

    /// <summary>
    /// 游戏数据配置字典 int
    /// </summary>
    private static Dictionary<string, int> _gameConfigIntDic = new Dictionary<string, int>();

    /// <summary>
    /// 游戏数据配置字典  string
    /// </summary>
    private static Dictionary<string, string> _gameConfigStringDic = new Dictionary<string, string>();

    /// <summary>
    /// 游戏数据配置字典  vector3
    /// </summary>
    private static Dictionary<string, Vector3> _gameConfigVector3Dic = new Dictionary<string, Vector3>();

    /// <summary>
    /// 游戏数据配置字典  color
    /// </summary>
    private static Dictionary<string, UnityEngine.Color> _gameConfigColorDic = new Dictionary<string, UnityEngine.Color>();

    /// <summary>
    /// 获取游戏数据配置
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static float GetGameConfigFloat(string key)
    {
        float f = 0;
        if (_gameConfigFloatDic.TryGetValue(key, out f))
            return f;

        Table_Game_Config config = Table_Game_Config.GetPrimary(key);
        if (config != null)
        {
            f = ConvertUtils.GetFloatFromString(config.data);
            SetGameConfigFloat(key, f);
            return f;
        }

        return 0;
    }

    /// <summary>
    /// 获取游戏数据配置
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static int GetGameConfigInt(string key)
    {
        int i = 0;
        if (_gameConfigIntDic.TryGetValue(key, out i))
            return i;

        Table_Game_Config config = Table_Game_Config.GetPrimary(key);
        if (config != null)
        {
            i = ConvertUtils.GetIntFromString(config.data);
            SetGameConfigInt(key, i);
            return i;
        }

        return 0;
    }

    /// <summary>
    /// 获取游戏数据配置
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void SetGameConfigFloat(string key, float value)
    {
        _gameConfigFloatDic[key] = value;
    }

    /// <summary>
    /// 获取游戏数据配置
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void SetGameConfigInt(string key, int value)
    {
        _gameConfigIntDic[key] = value;
    }

    /// <summary>
    /// 获取字符串配置
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string GetGameConfigString(string key)
    {
        string str = null;
        if (_gameConfigStringDic.TryGetValue(key, out str))
            return str;

        Table_Game_Config table = Table_Game_Config.GetPrimary(key);
        if (table != null)
        {
            str = table.data;
            _gameConfigStringDic[key] = table.data;
        }

        return str;
    }

    /// <summary>
    /// 获取 Vector3 数据配置
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static Vector3 GetGameConfigVector3(string key)
    {
        Vector3 v;
        if (_gameConfigVector3Dic.TryGetValue(key, out v))
            return v;

        Table_Game_Config table = Table_Game_Config.GetPrimary(key);
        if (table != null)
        {
            v = ConvertUtils.GetVector3FromString(table.data);
            _gameConfigVector3Dic[key] = v;
        }

        return v;
    }

    /// <summary>
    /// 获取 Vector3 数据配置
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static UnityEngine.Color GetGameConfigColor(string key)
    {
        UnityEngine.Color c;
        if (_gameConfigColorDic.TryGetValue(key, out c))
            return c;

        Table_Game_Config table = Table_Game_Config.GetPrimary(key);
        if (table != null)
        {
            c = ConvertUtils.GetColorFromString(table.data);
            _gameConfigColorDic[key] = c;
        }

        return c;
    }

    #endregion
}
