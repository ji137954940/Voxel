using System;
using System.Text.RegularExpressions;
using UnityEngine;
using ZLib;
/// <summary>
/// 加载HTTP服务器上配置的状态
/// @author Ollydbg
/// @date 2018-2-10
/// </summary>
public class LoadServerConfigState : IState<LancherContext>
{
    public LancherContext context
    {
        get;
        private set;
    }

    private StateMachine<LancherContext> sm;

    public LoadServerConfigState(StateMachine<LancherContext> sm, LancherContext context)
    {
        this.context = context;
        this.sm = sm;
    }

    public void OnDestroy()
    {

    }

    public void OnEnter()
    {
        LancherLoadUtils.Load(context.serverURL, 5, context.loadingServerConfigProgress)
            .Then(LoadServerJson)
            .Done(StateComplete)
            .Catch(CatchException);
    }

    /// <summary>
    /// 捕获异常
    /// </summary>
    /// <param name="obj"></param>
    private void CatchException(Exception e)
    {
        //如果是网络问题
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            sm.ChangeState<NetErrorState>();
            return;
        }
        else
        {
            sm.ChangeState<LoadErrorState>();
        }
        throw e;
    }

    private void StateComplete(LancherLoadData obj)
    {
        CodeBridgeTool.instance.SignTaskProgress(TaskID.LoadServerConfig, 1);
        //加载决定是远程还是本地的配置文件
        if (!context.needUpdateGame)
            sm.ChangeState<LoadResConfigState>();
    }

    /// <summary>
    /// 加载服务器Json后
    /// </summary>
    /// <param name="_www"></param>
    /// <returns></returns>
    private Promise<LancherLoadData> LoadServerJson(LancherLoadData _www)
    {
        var promise = new Promise<LancherLoadData>();

        context.serverInfo = JsonUtility.FromJson<TgameSvrInfo>(LancherEncodingUtils.GetString(_www.bytes));

        if (context.serverInfo != null)
        {
            //成功获取到服务器的信息
            LancherPrefs.SetInt(LancherPrefsConst.Need_Code, context.serverInfo.needCode ? 1 : 0);

            LancherPrefs.SetString(LancherPrefsConst.Channel, context.serverInfo.opChannel);

            promise.Resolve(_www);

            CompareGameVersion();
        }
        else
        {
            promise.Reject(new Exception(LancherConstTable.ParseServerFileError));
        }

        return promise;
    }

    public void OnExecute()
    {
    }

    public void OnExit()
    {
    }

    public void OnUpdate()
    {
    }

    /// <summary>
    /// 对比游戏版本
    /// </summary>
    private void CompareGameVersion()
    {
        context.needUpdateGame = false;

        if (string.IsNullOrEmpty(context.localGameVersion) || string.IsNullOrEmpty(context.serverInfo.gameVersion))
            return;

        var serVersion = context.serverInfo.gameVersion.Split('.');
        var locVersion = context.localGameVersion.Split('.');

        int locNumber = 0;
        int serNumber = 0;

        for (int i = 0; i < serVersion.Length; i++)
        {
            if (i < locVersion.Length)
            {
                if (int.TryParse(locVersion[i], out locNumber) && int.TryParse(serVersion[i], out serNumber))
                {
                    if (locNumber < serNumber)
                    {
                        context.needUpdateGame = true;
                        return;
                    }
                }
                else
                {
                    Debug.LogError("版本号设置了非数字内容，请检查！！");
                    return;
                }
            }
            else
            {
                Debug.LogError("config文件中，版本号设置的长度有问题");
                return;
            }

        }
    }
}
