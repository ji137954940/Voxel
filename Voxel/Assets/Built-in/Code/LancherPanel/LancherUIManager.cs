using System;
using System.Collections.Generic;
using UnityEngine;
using ZLib;
using Object = UnityEngine.Object;

/// <summary>
/// 不同的UI需要显示在不同的层上
/// @author Ollydbg
/// @date 2018-2-10
/// </summary>
public enum LancherLayer
{
    Bottom,
    Top,
}

/// <summary>
/// 这里想要管理好Lancher阶段的UI
/// 包括Logo
/// Loading
/// 还有错误提示
/// 一些游戏进入的公告
/// @author Ollydbg
/// @date 2018-2-9
/// </summary>
public class LancherUIManager : Singleton<LancherUIManager>
{
    /// <summary>
    /// 面板类型映射
    /// </summary>
    private Dictionary<CodeBridgeTool.EnumPreWindow, string> lancherUIPathDic = new Dictionary<CodeBridgeTool.EnumPreWindow, string>()
    {
        {CodeBridgeTool.EnumPreWindow.Logo,"Res/UI/Window/LogoWindowPre" },
        {CodeBridgeTool.EnumPreWindow.Loading,"Res/UI/Window/Panel_Loading" },
        {CodeBridgeTool.EnumPreWindow.Before_game,"Res/UI/Window/Panel_Notice" },
        {CodeBridgeTool.EnumPreWindow.PhoneType_Filter,"Res/UI/Window/Panel_FirstNotice" },
        {CodeBridgeTool.EnumPreWindow.NetError,"Res/UI/Window/Panel_ErrorNetwork" },
        {CodeBridgeTool.EnumPreWindow.Update,"Res/UI/Window/Panel_UpDataNotice" }
    };


    /// <summary>
    /// 面板层次映射
    /// </summary>
    private Dictionary<CodeBridgeTool.EnumPreWindow, LancherLayer> lancherUILayerDic = new Dictionary<CodeBridgeTool.EnumPreWindow, LancherLayer>()
    {
        {CodeBridgeTool.EnumPreWindow.Logo,LancherLayer.Top },
        {CodeBridgeTool.EnumPreWindow.Loading,LancherLayer.Top },
        {CodeBridgeTool.EnumPreWindow.Before_game,LancherLayer.Bottom },
        {CodeBridgeTool.EnumPreWindow.PhoneType_Filter,LancherLayer.Top },
        {CodeBridgeTool.EnumPreWindow.NetError,LancherLayer.Top },
        {CodeBridgeTool.EnumPreWindow.Update,LancherLayer.Top }
    };

    /// <summary>
    /// 当前打开的面板
    /// </summary>
    private Dictionary<CodeBridgeTool.EnumPreWindow, GameObject> opendWindow = new Dictionary<CodeBridgeTool.EnumPreWindow, GameObject>();

    /// <summary>
    /// 按照配置打开界面
    /// 并且按照配置设置层级
    /// </summary>
    public GameObject Open(CodeBridgeTool.EnumPreWindow _lancherUIType)
    {
        GameObject window;

        if (!opendWindow.TryGetValue(_lancherUIType, out window))
        {
            window = LoadWindow(_lancherUIType);

            var layer = LancherLayer.Top;

            if (this.lancherUILayerDic.TryGetValue(_lancherUIType, out layer))
            {
                var uiTransform = window.transform as RectTransform;

                switch (layer)
                {
                    case LancherLayer.Bottom:
                        uiTransform.SetParent(CanvasBottomTransform);
                        break;
                    case LancherLayer.Top:
                        uiTransform.SetParent(CanvasTopTransform);
                        uiTransform.SetAsLastSibling();
                        break;
                    default:
                        break;
                }

                uiTransform.sizeDelta = Vector2.zero;
                uiTransform.localPosition = Vector3.zero;
                uiTransform.localScale = Vector3.one;

                opendWindow.Add(_lancherUIType, window);
            }
            else
            {
                throw new Exception("需要配置lancherUI的层级");
            }
        }
        //TODO 打开和关闭可能太消耗性能 这里是未来的性能优化点
        window.SetActive(true);

        return window;
    }


    /// <summary>
    /// 关闭UI
    /// </summary>
    /// <param name="_lancherUIType"></param>
    public void Close(CodeBridgeTool.EnumPreWindow _lancherUIType)
    {
        GameObject window;

        if (opendWindow.TryGetValue(_lancherUIType, out window))
        {
            //TODO 打开和关闭可能太消耗性能 这里是未来的性能优化点
            window.SetActive(false);
        }
        else
        {
            Debug.LogWarning("尝试关闭一个从未打开的面板！" + _lancherUIType);
        }
    }

    /// <summary>
    /// 通过Resource加载Lancher中的UI
    /// </summary>
    /// <param name="_lancherUIType"></param>
    /// <returns></returns>
    private GameObject LoadWindow(CodeBridgeTool.EnumPreWindow _lancherUIType)
    {
        string loadPath;

        if (lancherUIPathDic.TryGetValue(_lancherUIType, out loadPath))
        {
            if (string.IsNullOrEmpty(loadPath))
            {
                throw new Exception("加载路径获取到的为空" + _lancherUIType);
            }

            var prefab = Resources.Load(loadPath) as GameObject;

            if (prefab != null)
            {
                var result = Object.Instantiate(prefab);

                if (result)
                {
                    return result;
                }
                else
                {
                    throw new Exception("神奇的问题，加载成功但是竟然未能初始化成功!" + loadPath);
                }
            }
            else
            {
                throw new Exception("在Resource中未能成功加载" + loadPath);
            }
        }
        else
        {
            throw new Exception("在Lancher中加载未发现合适的路径配置!" + _lancherUIType);
        }
    }


    private static Transform _canvasTopTransform;

    /// <summary>
    /// 游戏Canvas对象 transform---底部
    /// </summary>
    public static Transform CanvasTopTransform
    {
        get
        {
            if (_canvasTopTransform == null)
            {
                GameObject ob = GameObject.Find("CanvasContent/Canvas");
                if (ob == null) throw new Exception("获取CanvasContent/Canvas信息失败");
                _canvasTopTransform = ob.transform;
            }
            return _canvasTopTransform;
        }
    }

    private static Transform _canvasBottomTransform;

    /// <summary>
    /// 游戏Canvas对象 transform---底部
    /// </summary>
    public static Transform CanvasBottomTransform
    {
        get
        {
            if (_canvasBottomTransform == null)
            {
                GameObject ob = GameObject.Find("CanvasContent/Canvas");
                if (ob == null) throw new Exception("获取CanvasContent/Canvas信息失败");
                _canvasBottomTransform = ob.transform;
            }
            return _canvasBottomTransform;
        }
    }

    /// <summary>
    /// 在进入游戏以后，最好直接走游戏内部的
    /// </summary>
    public override void Dispose()
    {
        opendWindow.Clear();

        Resources.UnloadUnusedAssets();

        _canvasBottomTransform = null;

        _canvasTopTransform = null;

        base.Dispose();
    }
}