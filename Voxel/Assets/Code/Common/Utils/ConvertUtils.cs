
using UnityEngine;
/// <summary>
/// 类型转换Utils
/// </summary>
public static class ConvertUtils
{
    #region 数据类型转换

    /// <summary>
    /// 字符串转换为 byte
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static byte GetByteFromString(string str)
    {
        byte b = 0;
        if (byte.TryParse(str, out b))
            return b;

        return b;
    }

    /// <summary>
    /// 字符串转换为 sbyte
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static sbyte GetSbyteFromString(string str)
    {
        sbyte b = -1;
        if (sbyte.TryParse(str, out b))
            return b;

        return b;
    }

    /// <summary>
    /// 字符串转换为 short
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static short GetShortFromString(string str)
    {
        short s = -1;
        if (short.TryParse(str, out s))
            return s;

        return s;
    }

    /// <summary>
    /// 字符串转换为 float
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static float GetFloatFromString(string str)
    {
        float f = -1f;
        if (float.TryParse(str, out f))
            return f;

        return f;
    }

    /// <summary>
    /// 字符串转换为 double
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static double GetDoubleFromString(string str)
    {
        double d = -1;
        if (double.TryParse(str, out d))
            return d;

        return d;
    }

    /// <summary>
    /// 字符串转换为 int
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static int GetIntFromString(string str)
    {
        int i = -1;
        if (int.TryParse(str, out i))
            return i;

        return i;
    }

    /// <summary>
    /// 字符串转换为 uint
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static uint GetUIntFromString(string str)
    {
        uint i = 0;
        if (uint.TryParse(str, out i))
            return i;

        return i;
    }

    /// <summary>
    /// 字符串转换为 long
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static long GetLongFromString(string str)
    {
        long l = -1;
        if (long.TryParse(str, out l))
            return l;

        return l;
    }

    /// <summary>
    /// 字符串转换为 bool
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool GetBoolFromString(string str)
    {
        bool b = false;
        if (bool.TryParse(str, out b))
            return b;

        return b;
    }

    /// <summary>
    /// 字符串转换为 char
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static char GetCharFromString(string str)
    {
        char c = char.MinValue;
        if (char.TryParse(str, out c))
            return c;

        return c;
    }

    /// <summary>
    /// 字符串转换为 UInt16
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static ushort GetUInt16FromString(string str)
    {
        ushort i = 0;
        if (ushort.TryParse(str, out i))
            return i;

        return i;
    }

    /// <summary>
    /// 字符串转换为 UInt32
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static uint GetUInt32FromString(string str)
    {
        uint i = 0;
        if (uint.TryParse(str, out i))
            return i;

        return i;
    }

    /// <summary>
    /// 字符串转换为 UInt32
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static ulong GetUInt64FromString(string str)
    {
        ulong i = 0;
        if (ulong.TryParse(str, out i))
            return i;

        return i;
    }

    /// <summary>
    /// 字符串转换成 Vector3
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static Vector3 GetVector3FromString(string str)
    {
        if (string.IsNullOrEmpty(str))
            return Vector3.zero;

        var arr = str.Split(',');
        
        if(arr.Length >= 3)
        {
            Vector3 v = new Vector3(GetFloatFromString(arr[0]), GetFloatFromString(arr[1]), GetFloatFromString(arr[2]));
            return v;
        }

        return Vector3.zero;
    }

    /// <summary>
    /// 字符串转换成 Color
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static UnityEngine.Color GetColorFromString(string str)
    {
        if (string.IsNullOrEmpty(str))
            return UnityEngine.Color.white;

        var arr = str.Split(',');

        if(arr.Length >= 3)
        {
            if(arr.Length == 3)
                return new UnityEngine.Color(GetFloatFromString(arr[0]), GetFloatFromString(arr[1]), GetFloatFromString(arr[2]));
            else
                return new UnityEngine.Color(GetFloatFromString(arr[0]), GetFloatFromString(arr[1]), GetFloatFromString(arr[2]), GetFloatFromString(arr[3]));
        }

        return UnityEngine.Color.white;
    }

    #endregion
}
