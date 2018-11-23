using System;
using System.Reflection;
using UnityEngine;
using ZLib;

/// <summary>
/// 解决DLL从哪里开始的问题
/// 小心由于宏编译导致的错误
/// @author Ollydbg
/// @date 2018-2-9
/// </summary>
public class ResolveDLLState : IState<LancherContext>
{
    public LancherContext context
    {
        get;
        private set;
    }

    private StateMachine<LancherContext> sm;

    /// <summary>
    /// 这里是需要加载的程序集地址
    /// 有可能需求加载多个程序集
    /// </summary>
    private string[] dllNameArray;

    public ResolveDLLState(StateMachine<LancherContext> sm, LancherContext context)
    {
        this.context = context;
        this.sm = sm;
    }

    public void OnDestroy()
    {
    }

    public void OnEnter()
    {
        LancherPrefs.SetString(LancherPrefsConst.AssemblyName, Assembly.GetExecutingAssembly().FullName);

#if NO_CODE
        //在没有代码的情况下 需要IOS和其他平台分开处理
        dllNameArray = LancherPathConst.DLLPath;
#else
        context.loadingDLLProgress.progress = 1;

        //在有代码的情况下直接添加入口类即可
        var entry = context.gameObject.GetComponent<Main>();

        if (entry == null)
        {
            entry = context.gameObject.AddComponent<Main>();
        }

        //entry.OnGameStart(context.serverInfo.resServerUrl, context.serverInfo.loginServerUrl, context.serverInfo.opChannel);

        entry.OnInit();
#endif
        if (dllNameArray != null && dllNameArray.Length > 0)
        {
            LoadDLL();
        }
        else
        {

        }
    }

    private void LoadDLL()
    {
        if (context.nocodeAssembly != null)
        {
            ExecuteMain(context.nocodeAssembly);
        }
        else
        {

            for (int i = 0; i < dllNameArray.Length; i++)
            {
                dllNameArray[i] = string.Format("{0}/{1}", context.serverInfo.resServerUrl, dllNameArray[i]);
            }
            //Loading那边一直在检查加载进度 直到完毕后 才会跳转下一个状态
            LancherLoadUtils
                .LoadArray(dllNameArray, 20, new LancherProgressHolder[] { context.loadingDLLProgress })
                .Then(LoadDLLComplete)
                .Catch(LoadDLLError);
        }
    }

    /// <summary>
    /// 加载DLL报错了.
    /// </summary>
    /// <param name="obj"></param>
    private void LoadDLLError(Exception obj)
    {
        //TODO 未处理的DLL加载异常
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            sm.ChangeState<NetErrorState>();
        }
        else
        {
            sm.ChangeState<LoadErrorState>();
        }
        throw obj;
    }

    /// <summary>
    /// 加载DLL群组结束
    /// 先加载全部的程序集 防止出现问题
    /// 然后再执行入口程序集中的对应函数
    /// </summary>
    /// <param name="_wwws"></param>
    /// <returns></returns>
    private IPromise<LancherLoadData[]> LoadDLLComplete(LancherLoadData[] _wwws)
    {
        var promise = new Promise<LancherLoadData[]>();

        //入口程序集
        Assembly gameEntry = null;

        //载入程序集
        for (int i = 0; i < _wwws.Length; i++)
        {
            var currentWWW = _wwws[i];

            if (currentWWW == null)
            {
                throw new Exception(LancherConstTable.LoadDLLError + currentWWW.url);
            }

            var assembly = Assembly.Load(currentWWW.bytes);

            //如果是入口 就需要保存下 在所有的前置DLL都Load完毕后 在去开始处理这个DLL
            if (currentWWW.url.Contains(LancherPathConst.DLLNAME))
            {
                gameEntry = assembly;
            }
        }

        if (gameEntry != null)//处理入口的问题
        {
            context.nocodeAssembly = gameEntry;

            try
            {
                ExecuteMain(context.nocodeAssembly);

                promise.Resolve(_wwws);
            }
            catch (Exception e)
            {
                promise.Reject(e);
            }
        }
        else
        {
            promise.Reject(new Exception(LancherConstTable.CanNotFoundAssembly));
        }

        CodeBridgeTool.instance.UpdateDllResLoadAction();

        return promise;
    }

    /// <summary>
    /// 执行Assembly的方法
    /// </summary>
    /// <param name="gameEntry"></param>
    public void ExecuteMain(Assembly gameEntry)
    {
        var currentEntryClass = gameEntry.GetType(LancherPathConst.MainClassName);//获取Main入口

        if (currentEntryClass != null)
        {
            var entryComponent = context.gameObject.GetComponent(currentEntryClass);

            if (entryComponent == null)
            {
                entryComponent = context.gameObject.AddComponent(currentEntryClass);
            }

            if (entryComponent != null)
            {
                var entryMethod = currentEntryClass.GetMethod(LancherPathConst.MainEntryFunctioName);//获取入口函数

                if (entryMethod != null)
                {
                    try
                    {
                        entryMethod.Invoke(entryComponent, new object[] { context.serverInfo.resServerUrl, context.serverInfo.loginServerUrl, context.serverInfo.opChannel });
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                else
                {
                    throw new Exception(LancherConstTable.CanNotFoundEntryFunction);
                }
            }
            else
            {
                throw new Exception(LancherConstTable.AddToComponentError);
            }
        }
        else
        {
            throw new Exception(LancherConstTable.CanNotFoundClass);
        }
    }

    public void OnExecute()
    {

    }

    public void OnExit()
    {
        dllNameArray = null;
    }

    public void OnUpdate()
    {
    }
}
