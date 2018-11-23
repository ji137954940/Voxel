using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZLib;


public class GameData : Singleton<GameData>
{
    /// <summary>
    /// 配置加载完成
    /// </summary>
    public bool ConfigReady;

    /// <summary>
    /// 预加载数据是否加载完毕
    /// </summary>
    public bool IsPreResourcesLoadOver { get; set; }

    /// <summary>
    /// 是否显示相机最大的距离
    /// </summary>
    public bool IsShowCameraMaxDis { get; set; }
}
