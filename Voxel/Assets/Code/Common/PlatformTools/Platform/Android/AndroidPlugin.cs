using UnityEngine;
using System;
using System.Collections;
/// <summary>
/// 移植安卓插件管理类
/// </summary>
public class AndroidPlugin
{

    private static AndroidJavaObject _plugin;

    static AndroidPlugin()
    {
        if (Application.platform != RuntimePlatform.Android)
            return;

        using (var pluginClass = new AndroidJavaClass("com.tianshen.plugin.TSBasePlugin"))
            _plugin = pluginClass.CallStatic<AndroidJavaObject>("instance");
    }

    public static AndroidJavaObject GetAndroidPlugin()
    {
        return _plugin;
    }
    /// <summary>
    /// 设置游戏亮度
    /// </summary>
    /// <param name="f"></param>
    public static void setBrightness(int f)
    {
        _plugin.Call("setBrightness", f.ToString());
    }


    public static bool IsShowUserCenter()
    {
        return _plugin.Call<bool>("userCenterIsOpen");
    }

    public static bool IsShowSwitchAccount()
    {
        return _plugin.Call<bool>("switchAccountIsOpen");
    }

    public static void CreateBug()
    {
        _plugin.Call("createBug");
    }

    public static void tuiSong(int id, string msg)
    {
        _plugin.Call("registerLocalNotification", id, msg);
    }
    public static void unTuiSong()
    {
        _plugin.Call("unregisterAllLocalNotifications");
    }
    public static void copyTextToClipboard(string msg)
    {
        _plugin.Call("copyTextToClipboard", msg);
    }
    public static void SaveImageToGallery(string srcpath)
    {
        //_plugin.Call("SaveImageToGallery", srcpath);

        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = jc.GetStatic<AndroidJavaObject>("currentActivity");

        AndroidJavaObject javaObject = new AndroidJavaObject("com.unity.android.SaveImageTools");

        javaObject.Call("saveImageToGallery", activity, srcpath);
    }
    // 保存到相册

    //获取 Android可用总内存大小
    public static long GetMemTolal()
    {
        long totalMem = _plugin.Call<long>("GetMen_Tolal");
        return totalMem;
    }

    //军爷给的重启代码
    public static void restartApplication()
    {
        GetCurrentActivity().Call("restartApplicationSelf");
    }


    /// <summary>
    /// 显示更新版本对话框
    /// </summary>
    /// <param name="title">对话框标题</param>
    /// <param name="msg">对话框内容提示信息</param>
    /// <param name="url">新版本目标地址</param>
    public static void ShowUpdateVersionDialog(string title, string msg, string url)
    {
        _plugin.Call("showDownloadNewVersionDialog", title, msg, url);

    }

    public static void DownLoadApk(string msg, string title, string url)
    {
        _plugin.Call("downloadNewVersionApk", url, title, msg);
    }

    public static void OpenApk(string msg, string title)
    {
        _plugin.Call("openFile", msg, title);
    }

    public static bool IsExistenceAPK(string msg, string title)
    {
        bool isAPK = _plugin.Call<bool>("IsExistenceAPK", msg, title);
        return isAPK;
    }

    public static bool IsExistenceAPKOK(string msg, string title, string _MD5)
    {
        //bool isAPK = _plugin.Call<bool>("IsExistenceAPKOK", msg, title, _MD5);
        _plugin.Call("IsExistenceAPKOK", msg, title, _MD5);
        bool isAPK = false;
        return isAPK;
    }
    //public static void GetMacAddress (string modeName, string funName)
    //{
    //    if (Application.platform != RuntimePlatform.Android)
    //        return;

    //    Debug.Log ("GetMacAddress ");
    //    _plugin.Call ("getMac", modeName, funName);
    //}

    //public static void Android_DeviceInfo (string modeName, string funName)
    //{
    //    if (Application.platform != RuntimePlatform.Android)
    //        return;

    //    Debug.Log ("DeviceInfo  ");
    //    _plugin.Call ("DeviceInfo", modeName, funName);
    //}	

    //public static void GetSDCardPath (string modeName, string funName)
    //{
    //    if (Application.platform != RuntimePlatform.Android)
    //        return;

    //    _plugin.Call ("getSDCard", modeName, funName);
    //}



    public static AndroidJavaObject GetCurrentActivity()
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            return null;
        }

        AndroidJavaObject ajo = _plugin.Call<AndroidJavaObject>("getActivity");
        if (ajo == null)
        {
            //TDebug.LogError("TSAndroidPlugin.GetCurrentActivity is null");
        }
        return ajo;
    }
    /// <summary>
    /// 调用anndroid系统Toast
    /// </summary>
    /// <param name="toastMessage"></param>
    /// <param name="showType">1,长时间显示，其他为短时间</param>
    public static void MakeAndroidToast(string toastMessage, int showType)
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            return;
        }
        _plugin.Call("makeToast", toastMessage, Convert.ToString(showType));

    }

    public static AndroidJavaObject CreateJavaObject(string className)
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            return null;
        }
        AndroidJavaObject ajo = _plugin.Call<AndroidJavaObject>("createObject", className);
        return ajo;
    }

    public static AndroidJavaObject InvokeClazzStaticMethod(string className, string methodName)
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            return null;
        }
        AndroidJavaObject ajo = _plugin.Call<AndroidJavaObject>("invokeClazzStaticMethod", className, methodName);
        return ajo;
    }

    public static void StartActivity(string className)
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            return;
        }
        _plugin.Call("startActivity", className);
    }

    public static void StartActivity(String className, String flagStr, String param, params String[] datas)
    {
        {
            if (Application.platform != RuntimePlatform.Android)
            {
                return;
            }
            _plugin.Call("startActivity", className, flagStr, param, datas);
        }
    }
}

