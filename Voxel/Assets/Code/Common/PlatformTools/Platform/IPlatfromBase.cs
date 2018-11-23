using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// 平台抽象层
/// </summary>
interface IPlatformBase
{

    /// <summary>
    /// 保存图片信息数据
    /// </summary>
    /// <param name="srcpath"></param>
    void SaveImg(string srcpath);

    /// <summary>
    /// 保存视频信息数据
    /// </summary>
    /// <param name="srcpath"></param>
    void SaveVideo(string srcpath);

}

