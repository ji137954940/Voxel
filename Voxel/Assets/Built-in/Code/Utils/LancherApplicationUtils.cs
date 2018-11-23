using UnityEngine;
/// <summary>
/// 封装系统的一些函数
/// </summary>
public static class LancherApplicationUtils
{
    /// <summary>
    /// float比较相等
    /// </summary>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    /// <returns></returns>
    public static bool FloatEqual(float value1, float value2)
    {
        if (Mathf.Abs(value1 - value2) < float.Epsilon)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 退出Application
    /// </summary>
    public static void QuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
