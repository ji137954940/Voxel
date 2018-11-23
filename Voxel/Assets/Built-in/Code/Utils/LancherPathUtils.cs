using UnityEngine;
/// <summary>
/// 主要处理Lancher上的各种加载路径问题
/// @author Ollydbg
/// @date 2018-2-9
/// </summary>
public static class LancherPathUtils
{
    public const char PathSeparator = '/';

    /// <summary>
    /// StreamingAsset的路径
    /// </summary>
    public const string StreamingPath = "Assets/StreamingAssets";

    /// <summary>
    /// 很多平台的前缀
    /// </summary>
    public const string FilePrefix = "file://";

    /// <summary>
    /// 负责包装Path到StreamingPath
    /// 只支持WWW加载的模式 并不支持AssetBundle.CreateFromFile
    /// </summary>
    /// <param name="_filename">约定好了 只有相对路径名字 前面不需要写其他路径区分符号</param>
    /// <returns></returns>
    public static string GetWWWStreamingPath(string _filename)
    {
        if (string.IsNullOrEmpty(_filename))
        {
            throw new System.Exception("GetWWWStreamingPath这个函数不允许有穿空参数的情况！");
        }

        var ret = _filename;

        switch (Application.platform)
        {
            case RuntimePlatform.OSXEditor:
                return FilePrefix + Application.streamingAssetsPath + PathSeparator + ret;
            case RuntimePlatform.OSXPlayer:
                return FilePrefix + Application.streamingAssetsPath + PathSeparator + ret;
            case RuntimePlatform.WindowsPlayer:
                return FilePrefix + Application.streamingAssetsPath + PathSeparator + ret;
            case RuntimePlatform.WindowsEditor:
                return FilePrefix + Application.streamingAssetsPath + PathSeparator + ret;
            case RuntimePlatform.IPhonePlayer:
                return FilePrefix + Application.streamingAssetsPath + PathSeparator + ret;
            case RuntimePlatform.Android:
                return Application.streamingAssetsPath.Replace("/jar:file:/", "jar:file:///") + PathSeparator + ret;
            default:
                throw new System.NotImplementedException("平台未实现!");
                break;
        }
    }
}
