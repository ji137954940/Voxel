using System;
using UnityEngine;
/// <summary>
/// 封装一层PlayerPrefs来防止未来出BUG的时候不好查的问题
/// @author Ollydbg
/// @date 2018-2-9
/// </summary>
public static class LancherPrefs
{
    /// <summary>
    /// 存储字符串
    /// </summary>
    /// <param name="_key"></param>
    /// <param name="_value"></param>
    public static void SetString(string _key, string _value)
    {
        PlayerPrefs.SetString(_key, _value);
    }

    /// <summary>
    /// 存储Int
    /// </summary>
    /// <param name="_key"></param>
    /// <param name="_value"></param>
    public static void SetInt(string _key, int _value)
    {
        PlayerPrefs.SetInt(_key, _value);
    }

    /// <summary>
    /// 检查是否存在某一个Key
    /// </summary>
    /// <param name="_key"></param>
    /// <returns></returns>
    public static bool HasKey(string _key)
    {
        return PlayerPrefs.HasKey(_key);
    }

    /// <summary>
    /// 获取字符串
    /// </summary>
    /// <param name="_key"></param>
    /// <returns></returns>
    public static string GetString(string _key)
    {
        return PlayerPrefs.GetString(_key);
    }

    /// <summary>
    /// 存储起来
    /// </summary>
    internal static void Save()
    {
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 获取Int
    /// </summary>
    /// <param name="_key"></param>
    /// <returns></returns>
    public static int GetInt(string _key)
    {
        return PlayerPrefs.GetInt(_key);
    }

    /// <summary>
    /// 删除Key
    /// </summary>
    /// <param name="_key"></param>
    public static void DeleteKey(string _key)
    {
        PlayerPrefs.DeleteKey(_key);
    }

    /// <summary>
    /// 删除全部
    /// </summary>
    public static void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }
}
