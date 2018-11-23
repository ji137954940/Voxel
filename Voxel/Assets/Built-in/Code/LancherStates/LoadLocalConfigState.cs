using System;
using UnityEngine;
using ZLib;
/// <summary>
/// 加载本地config文件的状态
/// @author Ollydbg
/// @date 2018-2-10
/// </summary>
public class LoadLocalConfigState : IState<LancherContext>
{
    public LancherContext context
    {
        get;
        private set;
    }

    private StateMachine<LancherContext> sm;

    public LoadLocalConfigState(StateMachine<LancherContext> sm, LancherContext context)
    {
        this.context = context;
        this.sm = sm;
    }

    public void OnDestroy()
    {
    }

    public void OnEnter()
    {
        var localURLPath = LancherPathUtils.GetWWWStreamingPath(LancherPathConst.ConfigPath);

        LancherLoadUtils.Load(localURLPath, 3)
            .Then(ParseWWWPrepareServerPath)
            .Done(JumpToLoadHttpServerConfig)
            .Catch(CatchException);
    }

    /// <summary>
    /// 捕获到异常 需要通知面板显示 至少要把异常打印
    /// </summary>
    /// <param name="obj"></param>
    private void CatchException(Exception obj)
    {
        //TODO 异常处理
        //throw obj;
        //改变到网络改变的状态
        sm.ChangeState<LoadErrorState>();

        throw obj;
    }

    /// <summary>
    /// 开始加载服务器的配置文件
    /// </summary>
    /// <param name="obj"></param>
    private void JumpToLoadHttpServerConfig(LancherLoadData obj)
    {
        CodeBridgeTool.instance.SignTaskProgress(TaskID.LoadLocalConfig, 1);

        sm.ChangeState<LoadServerConfigState>();
    }

    /// <summary>
    /// 解析WWW读的文件 然后准备HTTP服务器地址
    /// </summary>
    /// <param name="_www"></param>
    /// <returns></returns>
    private Promise<LancherLoadData> ParseWWWPrepareServerPath(LancherLoadData _www)
    {
        var promise = new Promise<LancherLoadData>();

        var text = LancherEncodingUtils.GetString(_www.bytes);

        if (text.Length == 0)
        {
            promise.Reject(new Exception(LancherConstTable.LoadConfigError));
            return promise;
        }

        var infoArray = text.Split(';');

        if (infoArray.Length > 5)
        {
            var channelStr = string.Format("{0}-{1}-{2}-{3}", infoArray[1], infoArray[2], infoArray[3], infoArray[4]).Trim();

            if (infoArray.Length >= 6)//
            {
                CodeBridgeTool.instance.SetUpdateBulidNum(infoArray[5]);
            }

            // 记录当前客户端的 游戏版本号
            if (infoArray.Length >= 8)
                context.localGameVersion = infoArray[7];

            context.serverURL = string.Format("{0}?op_channel={1}", WWW.UnEscapeURL(infoArray[0].ToString()), channelStr);

            promise.Resolve(_www);

            return promise;
        }
        else
        {
            promise.Reject(new Exception(LancherConstTable.ParseConfigFileError));

            return promise;
        }
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
}