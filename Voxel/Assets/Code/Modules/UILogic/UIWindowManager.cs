using System.Collections;
using System.Collections.Generic;
using Color.Number.GameInfo;
using UnityEngine;
using ZFrame;
using ZLib;
using ZTool.Res;
using System;

public class UIWindowManager : Singleton<UIWindowManager>
{

    /// <summary>
    /// 主界面显隐操作
    /// </summary>
    /// <param name="active"></param>
    public void ShowMainWindow(bool active)
    {
        var go = GameObject.Find("MainWindow");
        if(go != null)
            go.SetActive(active);
    }


    //存储当前已经打开的窗口模块
    private List<BaseUIModule> m_lOpenWindowModule = new List<BaseUIModule>();

    //特殊的窗口类型数据
    private List<Type> m_lSpecialWindowModule;

    /// <summary>
    /// 打开一个window
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void Open<T>() where T : BaseUIModule, new()
    {
        Open<T>(null, null);
    }

    /// <summary>
    /// 打开一个window
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="callback"></param>
    /// <param name="Inparameter">传给调用者回调的参数对像</param>
    /// <param name="toParameter">传给将要开启的面板参数对像</param>
    /// <param name="isLoadDependencies">是否加载依赖关系，false只加载window框，true是原来加载全部window和图集</param>
    public void Open<T>(Action<object> callback, object Inparameter = null, object toParameter = null, bool isLoadDependencies = true) where T : BaseUIModule, new()
    {

        //单独加载window框的时候
        T t = Frame.instance.GetModule<T>();
        if (t == null)
        {
            //如果当前类型还没有注册，那么就创建一个，然后注册
            t = new T();
            Frame.instance.RegisterModule(t);
        }

        //当前已经有加载请求，或者资源正在显示，那么就不需要重新创建请求
        if (t.IsOpened)
        {
            if (!t.IsVisible)
            {
                ////全屏打开时候的判断要打开面板是否有CanShowFullScreen，
                //if (IsHaveOpenFullScreen() && !t.CanShowFullScreen)
                //{
                //    t.Visible(false);
                //}
                //else
                {
                    t.Visible(true);
                }

                if (callback != null)
                    callback(Inparameter);

            }
            return;
        }

        if (isLoadDependencies == true)
        {
            //添加界面展示开启记录
            if (!m_lOpenWindowModule.Contains(t))
                m_lOpenWindowModule.Add(t);
        }

        t.Open(callback, Inparameter, toParameter, isLoadDependencies);
    }

    /// <summary>
    /// 关闭界面模块，并且移除监听数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void Close<T>() where T : BaseUIModule
    {
        T t = Frame.instance.GetModule<T>();
        if (t != null)
        {
            //移除界面展示开启记录
            if (m_lOpenWindowModule != null && m_lOpenWindowModule.Count > 0)
                m_lOpenWindowModule.Remove(t);

            //当前模块已经注册，那么就关闭界面并且移除模块监听
            t.Close();
            //移除注册的数据模块
            Frame.instance.RemoveModule<T>();

            //加载不依赖引用关系window---测试
            //if (t is AreaMapModule)
            //{
            //   Open<AreaMapModule>(null, null, null, false);
            //}
        }
    }

    /// <summary>
    /// 关闭界面模块，并且移除监听数据
    /// </summary>
    /// <param name="type"></param>
    public void Close(Type type)
    {
        BaseUIModule m = Frame.instance.GetModule(type) as BaseUIModule;
        if (m != null)
        {
            //当前模块已经注册，那么就关闭界面并且移除模块监听
            m.Close();

            //移除界面展示开启记录
            if (m_lOpenWindowModule != null && m_lOpenWindowModule.Count > 0)
                m_lOpenWindowModule.Remove(m);

            //移除注册的数据模块
            Frame.instance.RemoveModule(type);

            //加载不依赖引用关系window---测试
            //if (m is AreaMapModule)
            //{
            //    Open<AreaMapModule>(null, null, null, false);
            //}
        }
    }


    /// <summary>
    /// 加载资源数据
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <param name="callback">资源回调函数</param>
    /// <param name="parameter">资源回调函数参数</param>
    /// <param name="isLoadDependencies">是否加载依赖关系</param>
    public void LoadRes(string path, ResHelper.OnResNodeLoadOver callback, object parameter, bool isLoadDependencies = true)
    {
        string url = string.Format("{0}{1}", ConstantConfig.UI_RES_PATH, path);
        ResourcesLoad.instance.LoadAssetBundle(url, callback, parameter, null, true, false, isLoadDependencies);

    }

    /// <summary>
    /// 资源卸载
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <param name="callback">资源回调函数</param>
    public void UnLoadRes(string path, ResHelper.OnResNodeLoadOver callback)
    {
        string url = string.Format("{0}{1}", ConstantConfig.UI_RES_PATH, path);
        ResourcesLoad.instance.UnLoadAssetBundle(url, callback);
    }

    /// <summary>
    /// 获取一个面板是否开启（有可能没有显示）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public bool IsOpen<T>() where T : BaseUIModule
    {
        T t = Frame.instance.GetModule<T>();
        if (t == null)
            return false;
        return m_lOpenWindowModule.Contains(t);
    }

    /// <summary>
    /// 设置UI显示隐藏
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="visible"></param>
    public void Visible<T>(bool visible) where T : BaseUIModule
    {
        T t = Frame.instance.GetModule<T>();
        if (t != null)
        {
            //if (t.IsVisible != visible && visible == true)
            //{
            //    //全屏打开时候的判断要打开面板是否有CanShowFullScreen，
            //    if (IsHaveOpenFullScreen() && !t.CanShowFullScreen)
            //    {
            //        t.Visible(false);
            //    }
            //    else
            //    {
            //        t.Visible(true);
            //    }
            //}
            //else
            {
                t.Visible(visible);
            }

        }
    }

    /// <summary>
    /// 关闭当前所有面板数据
    /// </summary>
    public void CloseAll(params BaseUIModule[] specialWindowArr)
    {
        if (m_lOpenWindowModule == null
            || m_lOpenWindowModule.Count == 0)
            return;

        for (int i = 0; i < m_lOpenWindowModule.Count;)
        {
            if (m_lOpenWindowModule[i] != null)
            {
                if (IsContainsType(specialWindowArr, m_lOpenWindowModule[i]))
                {
                    i++;
                    continue;
                }
                Close(m_lOpenWindowModule[i].GetType());
            }
            else
            {
                i++;
            }
        }
        m_lOpenWindowModule.Clear();
    }

    /// <summary>
    /// 数组中是否包含某一类型
    /// </summary>
    /// <param name="arr"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    bool IsContainsType(BaseUIModule[] arr, BaseUIModule t)
    {
        if (arr == null
            || arr.Length == 0
            || t == null)
            return false;

        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] != null && arr[i] == t)
                return true;
        }

        return false;
    }

}
