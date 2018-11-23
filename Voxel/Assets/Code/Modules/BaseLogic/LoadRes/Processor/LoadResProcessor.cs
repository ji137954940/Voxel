using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using ZFrame;
using ZTool.Res;
using ZTool.Table;

/// <summary>
/// 加载策划静态配置信息
/// @author Ollydbg
/// @date 2018-2-28
/// </summary>
public class LoadResProcessor : BaseProcessor
{
    public LoadResProcessor(Module _module) : base(_module)
    {
    }

    protected override List<Type> ListenModuleEvents()
    {
        return new List<Type>()
        {
            typeof(ME_Init_Config),
        };
    }

    protected override void ReceivedModuleEvent(ModuleEvent __me)
    {
        if (__me.IsInstance<ME_Init_Config>())
        {
            ReceivedLoadRes(__me as ME_Init_Config);
        }
    }

    private void ReceivedLoadRes(ME_Init_Config mE_Init_Config)
    {
        OnLoadConfig(GamePreLoadData.instance.Pre_Load_Tables);
    }

    //已经加载的配置资源
    private int m_iCount;

    private int m_loadCount;
    //所有的资源配置数量
    private int m_iAllConfigCount;

    static public string ShowProgress = "0/0";
    static public double costTime;
    static public DateTime lastttt;

    private void OnLoadConfig(string[] config)
    {
        if (config == null || config.Length == 0)
            return;
        GameData.instance.ConfigReady = false;
        //lastttt = DateTime.Now;

        m_iCount = 0;

        m_loadCount = 0;
        m_iAllConfigCount = config.Length;
        for (int i = 0; i < config.Length; i++)
        {
            //if (i == m_iAllConfigCount - 1)
            //{
            //    //如果最后一个数据为null
            //    if (string.IsNullOrEmpty(config[i]))
            //    {
            //        m_iAllConfigCount--;
            //        OnAllConfigLoadOver();
            //        return;
            //    }
            //}
            //这个try在配置文件加载本地（Resources.Load）的时候是同步执行的，包括回调也是同步执行
            //如果出现异常回调和catch都会吧计数+1，
            //这就造成了m_iCount >= m_iAllConfigCount这个表达式会早于预期成立
            try
            {
                string name = config[i].TrimEnd(new char[] { '\r' });
                //string name = config[i];

                if (OnCheckConfig(name))
                    continue;

                string url = string.Format("Config/{0}", name);
                System.Text.Encoding enco = null;
                if (GamePreLoadData.instance.IsHaveRes(url))
                {
                    enco = System.Text.Encoding.UTF8;
                }
                //Debug.LogError(url);
                //加载配置资源
                ResourcesLoad.instance.LoadBytes(url, OnLoadConfigOver, name, true, enco);
            }
            catch (Exception ex)
            {
                Debug.LogError("配置资源 " + config[i] + " 加载出问题\n " + ex.ToString() + "\n" + ex.StackTrace);

                //跳过此配置的加载
                m_iCount++;

                OnAllConfigLoadOver();
            }
        }
    }

    private void OnLoadConfigOver(string path, object obj, object parameter)
    {
        if (obj == null)
        {
            //Debug.LogError("sss: " + path + "_____null"); 
            m_iCount += 1;

            m_loadCount++;

            //PlatformManager.instance.Log("Load Fail xxxxxxxxxxxxxxxxxxxxx" + m_loadCount + " /" + m_iAllConfigCount);

            OnAllConfigLoadOver();
            return;
        }
        m_loadCount++;

        CodeBridgeTool.instance.SignTaskProgress(TaskID.LoadStaticConfig, m_loadCount / (float)m_iAllConfigCount);

        var name = (string)parameter;

        //PlatformManager.instance.Log("Load Name xxxxxxxxxxxxxxxxxxxxx" + m_loadCount + " /" + m_iAllConfigCount + " " + name);

        var type = Type.GetType(string.Format("Tgame.Game.Table.Table_{0}", TableManager.ToTitleCase(name)));
        if (type != null)
        {
            //string str = GameUtils.BytesToString((byte[])obj);
            //TableManager.LoadTable(str, name, type);
            var objs = new object[]
            {
                obj,name,type
            };
            //HandlerDataInWorkItem(objs);
            //启动线程池 操作数据解析存储
            ThreadPool.QueueUserWorkItem(HandlerDataInWorkItem, objs);
        }
        else
        {
            OnAllConfigLoadOver();
            throw new Exception("table没有找到" + name);
        }

    }
    //object lockOOO = new object();
    /// <summary>
    /// 非主线程解析数据
    /// </summary>
    /// <param name="parmsArr"></param>
    private void HandlerDataInWorkItem(object parmsArr)
    {
        //注意，这里写法为了安全降低了执行效率，如需优化需关闭lock，代码需要进一步验证 ，
        //已发现问题 多线程会出现问题，问题暂未定位
        //lock (lockOOO)
        //{
        lastttt = DateTime.Now;

        var parms = parmsArr as object[];

        if (parms != null && parms.Length >= 3)
        {
            var obj = parms[0] as byte[];
            var name = parms[1] as string;
            var type = parms[2] as Type;
            try
            {
                TableManager.LoadTable(obj, name, type);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString() + " " + name);
            }
        }

        Interlocked.Increment(ref m_iCount);

        //m_iCount += 1;

        costTime += ((TimeSpan)(DateTime.Now - lastttt)).TotalMilliseconds;

        OnAllConfigLoadOver();
        //}
    }

    private void OnAllConfigLoadOver()
    {
        //float pro = (float)m_iCount / m_iAllConfigCount * 0.95f + 0.05f;
        float pro = (float)m_iCount / m_iAllConfigCount * 0.95f;

        ShowProgress = string.Format("{0}/{1}", m_iCount, m_iAllConfigCount);
        if (m_iCount >= m_iAllConfigCount)

        {
            pro = 1.0f;
            //清理需要加载的table表数据
            //GamePreLoadData.GetInst().ClearPreLoadTables();

            //Debug.LogError("  config load done  ");
            //costTime = ((TimeSpan)(DateTime.Now - lastttt)).TotalMilliseconds;
            //costTime = UnityEngine.Time.realtimeSinceStartup - costTime;
            Debug.Log("load config and extuce Over!!!" + m_iCount + "/" + m_iAllConfigCount);
            GameData.instance.ConfigReady = true;

            //更新配表数据加载进度
            CodeBridgeTool.instance.UpdateTableConfigLoadAction(1f);

            //Debug.Log("HowManyTableTotal:" + TableManager.HowManyTableTotal);
        }

        ////更新配表数据加载进度
        //CodeBridgeTool.instance.UpdateTableConfigLoadAction(pro);

        //通知外部资源加载进度
        //lm.SetConfigProgress(pro);
        //TODO 通过消息派发
    }

    /// <summary>
    /// 检测配置数据
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private bool OnCheckConfig(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            m_iAllConfigCount--;
            OnAllConfigLoadOver();
            return true;
        }
        return false;
    }
}
