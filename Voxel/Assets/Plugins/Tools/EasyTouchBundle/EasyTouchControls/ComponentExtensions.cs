using UnityEngine;
using System.Collections;

public static class ComponentExtensions{
	public static RectTransform rectTransform(this Component cp){
		return cp.transform as RectTransform;
	}

	public static float Remap (this float value, float from1, float to1, float from2, float to2) {
		return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
	}



    //public static void SetLog(this string s, ColorEnum level)
    //{
    //    return;
    //    string log = "";
    //    switch (level)
    //    {
    //        case ColorEnum.black:
    //            log =  "<color=#000000ff>" + s + "</color>";
    //            break;
    //        case ColorEnum.green:
    //            log =  "<color=#008000ff>" + s + "</color>";
    //            break;
    //        case ColorEnum.yellow:
    //            log =  "<color=#ffff00ff>" + s + "</color>";
    //            break;
    //        case ColorEnum.red:
    //            log =  "<color=#ff0000ff>" + s + "</color>";
    //            break;
    //        case ColorEnum.white:
    //            log =  "<color=#ffffffff>" + s + "</color>";
    //            break;
    //        case ColorEnum.silver:
    //            log =  "<color=#c0c0c0ff>" + s + "</color>";
    //            break;
    //    }
    //    UnityEngine.Debug.Log(log);
    //}
}
/// <summary>
/// 颜色
/// </summary>
public enum ColorEnum
{
    /// <summary>
    /// black
    /// </summary>
    black,
    /// <summary>
    /// green
    /// </summary>
    green,
    /// <summary>
    /// red
    /// </summary>
    red,
    /// <summary>
    /// silver
    /// </summary>
    silver,
    /// <summary>
    /// white
    /// </summary>
    white,
    /// <summary>
    /// yellow
    /// </summary>
    yellow
}
