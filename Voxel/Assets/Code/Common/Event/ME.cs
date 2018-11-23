using ZFrame;

/// <summary>
/// 统一消息的构造 给未来在性能上优化消息的时候做基础
/// </summary>
public static class ME
{
    /// <summary>
    /// 构造ModuleEvent
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T New<T>() where T : ModuleEvent, new()
    {
        return new T();
    }

    /// <summary>
    /// 通过参数构造
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_init"></param>
    /// <returns></returns>
    public static T New<T>(Action<T> _init) where T : ModuleEvent, new()
    {
        var me = new T();

        if (_init != null)
        {
            _init(me);
        }

        return me;
    }
}
