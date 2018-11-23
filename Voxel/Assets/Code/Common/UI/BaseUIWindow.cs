using UnityEngine;
using UnityEngine.UI;
using ZFrame;
using ZTool.Res;

/// <summary>
/// window base 基类
/// </summary>
public class BaseUIWindow : BaseProcessor
{

    /// <summary>
    /// 重载构造函数
    /// </summary>
    /// <param name="_module"></param>
	public BaseUIWindow(Module _module)
        : base(_module)
    {

    }

    /// <summary>
    /// 初始化数据信息
    /// </summary>
    /// <param name="data">数据关联引用</param>
    public virtual void Init(BaseUIWindowData data)
    {
        Init(data, null);

    }

    /// <summary>
    /// 初始化数据信息
    /// </summary>
    /// <param name="data">数据关联引用</param>
    public virtual void Init(BaseUIWindowData data, object parameter)
    {
        //添加事件回调
        EventListener();
    }

    //添加事件处理
    protected virtual void EventListener() { }

    /// <summary>
    /// 切换图标
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="typeId"></param>
    /// <param name="id"></param>
    /// <param name="callback"></param>
    /// <param name="isGrey"></param>
    /// <param name="isAutoUse"></param>
    protected void SetIcon(Image sprite, int typeId, int id, Action<int, int, Object> callback, bool isGrey = false, bool isAutoUse = true)
    {
        var m = (BaseUIModule)module;
        if (m != null)
            m.SetIcon(sprite, typeId, id, callback, isGrey, isAutoUse);
    }

    /// <summary>
    /// 切换图标
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="typeId"></param>
    /// <param name="id"></param>
    /// <param name="callback"></param>
    /// <param name="isGrey"></param>
    /// <param name="isAutoUse"></param>
    protected void SetIcon(RawImage sprite, int typeId, int id, Action<int, int, Object> callback, bool isGrey = false, bool isAutoUse = true)
    {
        var m = (BaseUIModule)module;
        if (m != null)
            m.SetIcon(sprite, typeId, id, callback, isGrey, isAutoUse);
    }

    /// <summary>
    /// 切换图标
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="typeId"></param>
    /// <param name="id"></param>
    /// <param name="path"></param>
    /// <param name="callback"></param>
    /// <param name="isGrey"></param>
    /// <param name="isAutoUse"></param>
    protected void SetIcon(RawImage sprite, int typeId, int id, string path, Action<int, int, Object> callback, bool isGrey = false, bool isAutoUse = true)
    {
        var m = (BaseUIModule)module;
        if (m != null)
            m.SetIcon(sprite, typeId, id, path, callback, isGrey, isAutoUse);
    }

}

