using UnityEngine;

/// <summary>
/// 路径配置上的一些常量
/// @author Ollydbg
/// @date 2018-2-11
/// </summary>
public static class LancherPathConst
{
    /// <summary>
    /// 决定访问哪个HTTP服务器和什么版本的配置文件
    /// </summary>
    public const string ConfigPath = "config.txt";

    /// <summary>
    /// 决定资源从哪里加载的表
    /// </summary>
    public const string ResourceConfigPath = "Resconfig.txt";

    /// <summary>
    /// 视频文件加载
    /// </summary>
    public const string ResourceLogoVideoPath = "Logo.mp4";

    /// <summary>
    /// 包含这个字符的被认为应该有入口的
    /// </summary>
    public const string DLLNAME = "TgameCode.dll";

    /// <summary>
    /// GamePlay部分的入口类
    /// </summary>
    public const string MainClassName = "Main";

    /// <summary>
    /// GamePlay部分的入口函数
    /// </summary>
    public const string MainEntryFunctioName = "OnInit";

    /// <summary>
    /// 在HTTP服务器上加载DLL的相对路径
    /// 需要加前缀的
    /// </summary>
    public static string[] DLLPath = { "dll/" + DLLNAME };

#if UNITY_IOS

    /// <summary>
    /// 缓存路径（缓存使用 IO 使用）
    /// </summary>
    public static string CACHE_PATH = System.IO.Path.Combine(Application.temporaryCachePath, "vercache");

#else

    /// <summary>
    /// 缓存路径（缓存使用 IO 使用）
    /// </summary>
    public static string CACHE_PATH = System.IO.Path.Combine(Application.persistentDataPath, "vercache");

#endif

    /// <summary>
    /// 缓存资源存放路径
    /// </summary>
    public const string GROUP_SETUP = "setup";
}
