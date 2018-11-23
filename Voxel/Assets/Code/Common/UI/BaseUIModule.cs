using Tgame.Game.Icon;
using UnityEngine;
using UnityEngine.UI;
using ZFrame;

/// <summary>
/// 基本 Module 扩展 增加UI相关处理
/// </summary>
public class BaseUIModule : Module
{

    //资源路径
    public string Path { get; protected set; }

    //当前是否已经打开（不代表正在显示）
    public bool IsOpened { get; private set; }

    //当前是否正在显示
    public bool IsVisible { get; private set; }

    //资源对象
    protected GameObject go { get; private set; }

    //资源对象的transform
    public RectTransform transform { get; private set; }


    //资源回调数据
    Action<object> OnResCallBack;
    object toparameter;
    /// <summary>
    /// 开启时由调用方传入的数据对像
    /// </summary>
    protected object inParameter;

    /// <summary>
    /// 是否加载依赖关系
    /// </summary>
    public bool isLoadDependencies = true;
    /// <summary>
    /// 打开窗口界面
    /// </summary>
    public void Open()
    {
        Open(null);
    }

    /// <summary>
    /// 打开窗口界面
    /// </summary>
    /// <param name="callback"></param>
    /// <param name="inParameter"></param>
    /// <param name="toparameter"></param>
    /// <param name="isLoadDependencies"></param>是否加载依赖关系（空window框架）

    public void Open(Action<object> callback, object inParameter = null, object toparameter = null, bool isLoadDependencies = true)
    {
        this.OnResCallBack = callback;
        this.inParameter = inParameter;
        this.toparameter = toparameter;
        this.isLoadDependencies = isLoadDependencies;

        IsOpened = true;
        UIWindowManager.instance.LoadRes(Path, OnResLoadOver, toparameter, isLoadDependencies);

    }

    /// <summary>
    /// 窗口界面资源数据加载完成回调
    /// </summary>
    /// <param name="path">资源数据路径</param>
    /// <param name="obj">加载资源数据</param>
    /// <param name="parameter">资源数据回调参数</param>
    protected virtual void OnResLoadOver(string path, UnityEngine.Object obj, object parameter)
    {
        if (obj == null)
        {
            Debug.LogError("资源 " + path + " 加载失败，获取对象为null！");
            return;
        }
        //Debug.LogError("完成回调" + Path + "    " + Time.time);

        //SettingManager.instance.StopCost(Path);

        //转换成 GameObject对象
        go = obj as GameObject;
        transform = (RectTransform)go.transform;
        transform.name = this.Path;
        //初始化资源数据

        //设置对象layer
        //LayerUtils.SetLayerFromParent(go);

        //设置不同层级,血条在里面单独设置layer
        //setOriginParent(transform);

        transform.SetParent(GameInstance.CanvasTransform);
        transform.sizeDelta = Vector2.zero;
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;

        //单独加载window框的时候隐藏
        if (isLoadDependencies == false)
        {
            Visible(false);
            return;
        }

        //OnResLoadOver2(parameter);

    }

   

    /// <summary>
    /// 资源加载完成之后的回调函数
    /// </summary>
    protected virtual void OnResLoadCallBack()
    {
        if (OnResCallBack != null)
        {
            OnResCallBack(toparameter);
            OnResCallBack = null;
            toparameter = null;
        }
    }

    /// <summary>
    /// 设置UI显隐
    /// </summary>
    public virtual void Visible(bool visible)
    {
        if (go != null)
        {
            IsVisible = visible;
            //List<Processor> list = GetAllRegisterProcessor();
            //if (list != null && list.Count > 0)
            //{
            //    int count = list.Count;
            //    for (int i = 0; i < count; i++)
            //    {
            //        BaseProcessor processor = (BaseProcessor)list[i];
            //        if (processor != null)
            //            processor.IsVisible = IsVisible;
            //    }
            //}

            {
                go.SetActive(IsVisible);
            }
            //}
        }
    }

    #region 数据刷新

    /// <summary>
    /// 是否可以进行数据刷新
    /// </summary>
    /// <returns></returns>
    public override bool IsCanUpdate()
    {
        if (!IsVisible
            || go == null
            || !go.activeSelf)
            return false;

        return base.IsCanUpdate();
    }

    /// <summary>
    /// 数据刷新
    /// </summary>
    protected override bool OnUpdate()
    {
        if (!IsCanUpdate())
            return false;

        return base.OnUpdate();
    }

    /// <summary>
    /// 数据刷新
    /// </summary>
    protected override bool OnLateUpdate()
    {
        if (!IsCanUpdate())
            return false;

        return base.OnLateUpdate();
    }

    #endregion

    /// <summary>
    /// 清理数据
    /// </summary>
    public virtual void Clear()
    {
        //List<Processor> list = GetAllRegisterProcessor();
        //int count = list.Count;
        //for (int i = 0; i < count; i++)
        //{
        //    if (list[i] != null)
        //    {
        //        ((BaseProcessor)list[i]).Clear();
        //    }
        //}
        if (go != null)
            GameObject.DestroyImmediate(go, true);
        go = null;
        IsOpened = false;
        IsVisible = false;
        transform = null;
        OnResCallBack = null;
        toparameter = null;
        inParameter = null;
        isLoadDependencies = true;

    }

    /// <summary>
    /// 界面关闭
    /// </summary>
    public virtual void Close()
    {
        //首先清理数据
        Clear();

        //资源卸载
        UIWindowManager.instance.UnLoadRes(Path, OnResLoadOver);
        Path = null;
    }

    #region 切换图片

    /// <summary>
    /// 设置Icon数据
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="typeId"></param>
    /// <param name="id"></param>
    /// <param name="callback"></param>
    /// <param name="isGrey"></param>
    /// <param name="isAutoUse"></param>
    public void SetIcon(Image sprite, int typeId, int id, Action<int, int, Object> callback, bool isGrey = false, bool isAutoUse = true)
    {
        IconManager.instance.SetIcon(go, sprite, typeId, id, callback, isGrey, isAutoUse);
    }

    /// <summary>
    /// 设置Icon数据
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="typeId"></param>
    /// <param name="id"></param>
    /// <param name="callback"></param>
    /// <param name="isGrey"></param>
    /// <param name="isAutoUse"></param>
    public void SetIcon(RawImage sprite, int typeId, int id, Action<int, int, Object> callback, bool isGrey = false, bool isAutoUse = true)
    {
        IconManager.instance.SetIcon(go, sprite, typeId, id, callback, isGrey, isAutoUse);
    }

    /// <summary>
    /// 设置Icon数据
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="typeId"></param>
    /// <param name="id"></param>
    /// <param name="path"></param>
    /// <param name="callback"></param>
    /// <param name="isGrey"></param>
    /// <param name="isAutoUse"></param>
    public void SetIcon(RawImage sprite, int typeId, int id, string path, Action<int, int, Object> callback, bool isGrey = false, bool isAutoUse = true)
    {
        IconManager.instance.SetIcon(go, sprite, typeId, id, path, callback, isGrey, isAutoUse);
    }

    #endregion
}

