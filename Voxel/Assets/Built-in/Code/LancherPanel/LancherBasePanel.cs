using UnityEngine;
using ZLib;
/// <summary>
/// 简单的设计下启动器的父类
/// @author Ollydbg
/// @date 2018-2-10
/// </summary>
public abstract class LancherBasePanel
{
    /// <summary>
    /// 状态机 为什么要有呢 因为我们可能需要在UI中控制状态机
    /// </summary>
    public StateMachine<LancherContext> machine;

    /// <summary>
    /// 启动器上下文
    /// 原因是需要在上下文中同其他UI进行交互
    /// </summary>
    public LancherContext context;

    /// <summary>
    /// 获取本Panel对应类型
    /// </summary>
    protected CodeBridgeTool.EnumPreWindow panelType;

    /// <summary>
    /// 操纵的GameObject
    /// </summary>
    protected GameObject container;
    
    /// <summary>
    /// 当前是否打开了
    /// </summary>
    public bool isOpen;

    /// <summary>
    /// 在初始化的时候调用
    /// </summary>
    virtual public void Init()
    {

    }

    /// <summary>
    /// 显示
    /// </summary>
    virtual public void Show()
    {
        isOpen = true;
        //当container为null的时候会加载，实例化，但是不为空的时候 界面将再也无法显示，因为Close方法会Active(false)
        //if (container == null)
        container = LancherUIManager.instance.Open(panelType);
    }

    /// <summary>
    /// 隐藏
    /// </summary>
    virtual public void Hide()
    {
        if (container != null)
        {
            isOpen = false;

            LancherUIManager.instance.Close(panelType);
        }
    }

    /// <summary>
    /// 在销毁的时候的处理
    /// </summary>
    public virtual void OnDestory()
    {
        machine = null;

        context = null;

        if (container != null)
            Object.Destroy(container);

        container = null;
    }
}