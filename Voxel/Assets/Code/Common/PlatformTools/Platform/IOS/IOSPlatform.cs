using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;


/// <summary>
/// Ios 平台操作
/// </summary>
public class IOSPlatform : IPlatformBase
{
    /// <summary>
    /// 保存图片
    /// </summary>
    /// <param name="str"></param>
    [DllImport("__Internal")]
    static extern void _SavePhoto(string str);

    /// <summary>
    /// 保存视频
    /// </summary>
    /// <returns></returns>
    [DllImport("__Internal")]
    static extern string _SaveVideo(string str);

    /// <summary>
    /// 保存图片
    /// </summary>
    /// <param name="srcpath"></param>
    public void SaveImg(string srcpath)
    {
        _SavePhoto(srcpath);
    }

    /// <summary>
    /// 保存视频
    /// </summary>
    /// <param name="srcpath"></param>
    public void SaveVideo(string srcpath)
    {
        _SaveVideo(srcpath);
    }
}

