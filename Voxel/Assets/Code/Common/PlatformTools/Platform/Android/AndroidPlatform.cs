using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


/// <summary>
/// Android 平台操作
/// </summary>
public class AndroidPlatform : IPlatformBase
{

    /// <summary>
    /// 保存图片
    /// </summary>
    /// <param name="srcpath"></param>
    public void SaveImg(string srcpath)
    {
        AndroidPlugin.SaveImageToGallery(srcpath);
    }

    /// <summary>
    /// 保存视频
    /// </summary>
    /// <param name="srcpath"></param>
    public void SaveVideo(string srcpath)
    {
        throw new NotImplementedException();
    }
}

