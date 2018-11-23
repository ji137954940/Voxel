using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 主要用于获取GO上的UIEventPointer，UIEventDrag 没有就添加
/// </summary>
public class UIEventListener
{

    #region 添加点击监听

    /// <summary>
    /// 添加监听脚本
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    static public UIEventPointer GetPointer(GameObject go, object p = null)
    {
        if (go == null)
        {
            return null;
        }
        UIEventPointer listener = go.GetComponent<UIEventPointer>();
        if (listener == null) listener = go.AddComponent<UIEventPointer>();
        listener.parameter = p;
        return listener;
    }

    /// <summary>
    /// 只为长按添加一个 长按时间间隔
    /// </summary>
    /// <param name="longMarginTime">长按的时间间隔</param>
    /// <returns></returns>
    static public UIEventPointer GetLongPointer(GameObject go, object p = null, float longMarginTime = 1f)
    {
        UIEventPointer listener = go.GetComponent<UIEventPointer>();
        if (listener == null) listener = go.AddComponent<UIEventPointer>();
        listener.parameter = p;
        listener.longPressTimeMargin = longMarginTime;
        return listener;
    }

    /// <summary>
    /// 添加Toggle监听脚本
    /// </summary>
    /// <param name="toggle"></param>
    /// <param name="p"></param>
    /// <returns></returns>
    static public UIEventPointer GetPointer(Toggle toggle, object p = null)
    {
        UIEventPointer listener = toggle.GetComponent<UIEventPointer>();
        if (listener == null) listener = toggle.gameObject.AddComponent<UIEventPointer>();
        listener.parameter = p;
        listener.toggle = toggle;
        toggle.onValueChanged.AddListener(listener.OnToggleValueChanged);
        return listener;
    }

    #endregion

    #region 添加拖拽监听

    /// <summary>
    /// 添加监听脚本
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    static public UIEventDrag GetDrag(GameObject go, object p = null)
    {
        UIEventDrag listener = go.GetComponent<UIEventDrag>();
        if (listener == null) listener = go.AddComponent<UIEventDrag>();
        listener.parameter = p;
        return listener;
    }

    /// <summary>
    /// 添加监听脚本
    /// </summary>
    /// <param name="sr"></param>
    /// <param name="p"></param>
    /// <returns></returns>
    static public UIEventDrag GetDrag(ScrollRect sr, object p = null)
    {
        UIEventDrag listener = sr.GetComponent<UIEventDrag>();
        if (listener == null) listener = sr.gameObject.AddComponent<UIEventDrag>();
        listener.parameter = p;
        listener.sr = sr;
        return listener;
    }

    #endregion

    #region 添加点击监听泛型参数

    /// <summary>
    /// 添加监听脚本
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public static UIEventPointerT<T> GetPointerT<T>(GameObject go, T t = default(T))
    {
        if (go == null)
        {
            return null;
        }
        UIEventPointerT<T> listener = go.GetComponent<UIEventPointerT<T>>();
        if (listener == null) listener = go.AddComponent<UIEventPointerT<T>>();
        listener.parameter = t;
        return listener;
    }

    public static UIEventPointerT<T> GetPointerT<T>(GameObject go)
    {
        if (go == null)
        {
            return null;
        }
        UIEventPointerT<T> listener = go.GetComponent<UIEventPointerT<T>>();
        if (listener == null) listener = go.AddComponent<UIEventPointerT<T>>();
        return listener;
    }

    #endregion

    //事件回调数据
    public delegate void VoidDelegate1(GameObject go, PointerEventData eventData, object parameter);
    public delegate void VoidDelegate2(BaseEventData eventData, object parameter);
    public delegate void VoidDelegate3(Toggle toggle, bool isOn, object paramter);
    //object parameter;
    //Toggle toggle;

    public delegate void VoidDelegate4<T>(GameObject go, PointerEventData eventData, T t = default(T));

}
