using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏实例对象缓存
/// </summary>
public static class GameInstance
{

    #region Main

    private static Main _main;

    /// <summary>
    /// Main 对象
    /// </summary>
    public static Main Main
    {
        get { return _main; }
        set
        {
            if (value != null && _main != value)
                _main = value;
        }
    }

    #endregion

    #region Camera

    private static Camera _mainCamera;

    /// <summary>
    /// MainCamera
    /// </summary>
    public static Camera MainCamera
    {
        get { return _mainCamera; }
        set
        {
            if (value != null && _mainCamera != value)
                _mainCamera = value;
        }
    }

    #endregion

    #region Video

    private static FFmpegREC _fFmpegRec;

    /// <summary>
    /// FFmpegREC
    /// </summary>
    public static FFmpegREC FFmpegREC
    {
        get { return _fFmpegRec; }
        set
        {
            if (value != null && _fFmpegRec != value)
                _fFmpegRec = value;
        }
    }

    #endregion

    #region Canvas

    //游戏Canvas对象 transform
    private static Transform _canvasTran;

    /// <summary>
    /// 游戏Canvas对象 transform
    /// </summary>
    public static Transform CanvasTransform
    {
        get
        {
            if (_canvasTran == null)
            {
                GameObject ob = GameObject.Find("CanvasContent/Canvas");
                if (ob == null) Debug.LogError("获取canvas信息失败");
                _canvasTran = ob.transform;
            }

            return _canvasTran;
        }
    }

    #endregion
}
