using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZLib;


/// <summary>
///  平台管理代码
/// </summary>
public class PlatformManager : Singleton<PlatformManager>
{


    private IPlatformBase _platform;


    public PlatformManager()
    {

#if UNITY_IOS
        SetIosPlatform();
#elif UNITY_ANDROID
        SetAndroidPlatform();
#else
        SetDefaultPlatform();
#endif

    }

    #region 平台信息设置

    /// <summary>
    /// 设置Android平台信息
    /// </summary>
    private void SetAndroidPlatform()
    {
        _platform = new AndroidPlatform();
    }

    /// <summary>
    /// 设置ios平台信息
    /// </summary>
    private void SetIosPlatform()
    {
        _platform = new IOSPlatform();
    }

    /// <summary>
    /// 设置默认平台信息
    /// </summary>
    private void SetDefaultPlatform()
    {
        _platform = new DefaultPlatform();
    }

    #endregion


    #region 工具方法

    /// <summary>
    /// 刷新游戏保存的图片到相册
    /// </summary>
    /// <param name="str"></param>
    public void SaveImageToAlbum(string str)
    {
        if(_platform == null)
            return;
        
        _platform.SaveImg(str);
    }

    /// <summary>
    /// 刷新游戏保存的视频到相册
    /// </summary>
    /// <param name="str"></param>
    public void SaveVideoToAlbum(string str)
    {
        if(_platform == null)
            return;
        
        _platform.SaveVideo(str);
    }

    #endregion


}

