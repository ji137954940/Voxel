/// <summary>
/// 在写Lancher的时候需要扩展的数组操作方法
/// @author Ollydbg
/// @date 2018-2-10
/// </summary>
public static class LancherArrayTools
{
    /// <summary>
    /// 扩展循环的处理
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="fn"></param>
    public static void Each<T>(this T[] source, Action<T, int> fn)
    {
        for (int i = 0, len = source.Length; i < len; i++)
        {
            var item = source[i];
            fn(item, i);
        }
    }
}
