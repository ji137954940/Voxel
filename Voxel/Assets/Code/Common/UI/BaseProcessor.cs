
using ZFrame;

/// <summary>
/// 基本Processor（扩展，增加网络通讯消息处理的功能）
/// </summary>
public abstract class BaseProcessor : Processor
{
    /// <summary>
    /// 重载构造函数
    /// </summary>
    /// <param name="_module"></param>
	public BaseProcessor(Module _module)
        : base(_module)
    {

        ////创建消息号与消息CLASS对照表（这样就可以使用从服务端发来的消息号，找到对应的解析类
        //var list = ListenModuleEvents();
        //if (list != null && list.Count > 0)
        //{
        //}
    }

    /// <summary>
    /// 获取当前的Module 类型数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    protected T GetMoule<T>() where T : Module
    {
        var rModule = (T)module;

        if (rModule != null)
            return rModule;
        return default(T);
    }


    #region 事件处理

    /// <summary>
    /// 处理事件方法
    /// </summary>
    /// <param name="__key">事件唯一标识</param>
    /// <param name="__data">数据</param>
    protected sealed override void ReceivedModuleEvent(int __key, object __data)
    {
        //if(__data is IExtensible)
        //{
        //    ReceivedMessage(__data as IExtensible);
        //}

        //if (__data is Message)
        //{
        //    ReceivedMessage(__data as Message);
        //}
    }

    /// <summary>
    /// 处理网络事件消息方法
    /// </summary>
    /// <param name="__msg"></param>
    protected virtual void ReceivedMessage(object __msg) { }

    #endregion

}
