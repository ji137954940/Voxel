using System;
using UnityEngine;

/// <summary>
/// 设置屏幕分辨率的实现
/// @author Ollydbg
/// @date 2018-2-7
/// </summary>
public static class ScreenSetting
{
    //屏幕分辨率使用比例
    static float resolution_ratio = 0.75f;
    //基础分辨率高度 1080（当前市场上的大部分机器）
    static float base_height = 1080f;

    private const int GAME_QUALITY_LOW = 1;
    /// <summary>
    /// 初始化的时候原始屏幕高
    /// </summary>
    static int act_Height = 0;
    /// <summary>
    /// 初始化的时候原始屏幕宽
    /// </summary>
    static int act_Width = 0;

    /// <summary>
    /// 设置分辨率
    /// </summary>
    public static void ScreenInch(GameObject go, bool fullscreen)
    {
        SetScreenResolution(750, 1334, true);
        return;

        //dpi = Screen.dpi;
        ////////////
        //获取当前屏幕分辨率
        float w = act_Width;
        float h = act_Height;

        float tempw = 0;
        float temph = 0;
        int _gameQuality;
        //float _gameQuality;
        //注意设置完分辨率后，ui面板要延迟显示，否则会有ui面板抖动
        //分辨率剩下是在当前帧结束的时候
        var device_aspect = Mathf.RoundToInt((w / h) * 10) / 10f;
        //device_aspect = float.Parse(string.Format("{0:F1}", w / h));//这种写法有些问题

        //如果当前分辨率大于 1080 那么就先设定成 1080
        if (h >= base_height)
        {
            float f = base_height / h;
            w *= f;
            h *= f;
        }
        //游戏品质
        if (!PlayerPrefs.HasKey("GameLocalQulity"))
        {
            //_gameQuality = go.GetComponent<MobileHierarchicalControlShowInfo>().GameQuality;
            _gameQuality = 4;
            PlayerPrefs.SetInt("GameLocalQulity", (int)_gameQuality);
        }
        else
            _gameQuality = PlayerPrefs.GetInt("GameLocalQulity");

        Debug.LogError("_gameQuality_____________" + _gameQuality);
        //GameData.instance.GameQuality = (int)_gameQuality;//设备设置

        if (LancherApplicationUtils.FloatEqual(device_aspect, 1.7f) || LancherApplicationUtils.FloatEqual(device_aspect, 1.8f))
        {
            if (w > 1334)
            {
                tempw = w * resolution_ratio;
                temph = h * resolution_ratio;
                SetScreenResolution((int)(tempw), (int)(temph), fullscreen);
            }
            //低端机默认的分辨率 最终降低到960*640级别
            if (_gameQuality == GAME_QUALITY_LOW)
            {
                if (tempw > 1080)
                {
                    var tempw2 = tempw * resolution_ratio;
                    var temph2 = temph * resolution_ratio;
                    if (tempw2 > 1080)
                    {
                        SetScreenResolution((int)(tempw2 * resolution_ratio), (int)(temph2 * resolution_ratio), fullscreen);
                    }
                    else
                        SetScreenResolution((int)(tempw2), (int)(temph2), fullscreen);
                }
            }
        }
        else if (LancherApplicationUtils.FloatEqual(device_aspect, 1.3f) || LancherApplicationUtils.FloatEqual(device_aspect, 1.5f))
        {
            if (w > 1280)
            {
                tempw = w * resolution_ratio;
                temph = h * resolution_ratio;
                SetScreenResolution((int)(tempw), (int)(temph), fullscreen);
            }
            //低端机默认的分辨率 最终降低到960*640级别
            if (_gameQuality == GAME_QUALITY_LOW)
            {
                if (tempw > 960)
                {
                    SetScreenResolution((int)(tempw * resolution_ratio), (int)(temph * resolution_ratio), fullscreen);
                }
            }
        }
        else
        {
            SetScreenResolution((int)(w * resolution_ratio), (int)(h * resolution_ratio), fullscreen);
            //Screen.SetResolution((int)(w * resolution_ratio), (int)(h * resolution_ratio), fullscreen);
        }
    }
    /// <summary>
    /// 汇总成方法，便于log，调试
    /// </summary>
    /// <param name="w"></param>
    /// <param name="h"></param>
    /// <param name="full"></param>
    static void SetScreenResolution(int w, int h, bool full)
    {
        Screen.SetResolution(w, h, full);
        //string str = string.Format("w={0},h={1}", w, h);
        //Debug.AddLog(str, true, true);
    }

    public static void RevertToActResolution()
    {
        var fullscreen = true;
        if (Application.isMobilePlatform)
        {

            fullscreen = true;
        }
        else
        {
            fullscreen = Screen.fullScreen;
        }
        SetScreenResolution(act_Width, act_Height, fullscreen);
    }

    /// <summary>
    /// 防止被剥离
    /// </summary>
    internal static void Init()
    {
        act_Width = Screen.width;
        act_Height = Screen.height;
        CodeBridgeTool.instance.ScreenSetting_revertToAct = RevertToActResolution;
    }
}
