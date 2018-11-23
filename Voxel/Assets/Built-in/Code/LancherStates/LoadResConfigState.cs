using System;
using ZLib;

/// <summary>
/// 前置读取了本地的文件 
/// 看看哪些文件是需要在Resource里面加载的 
/// 哪些需要在AssetBundle加载的
/// 准备这些数据给游戏内部使用
/// @author Ollydbg 
/// @date 2018-2-10
/// </summary>
public class LoadResConfigState : IState<LancherContext>
{
    public LancherContext context
    {
        get;
        private set;
    }

    private StateMachine<LancherContext> sm;

    public LoadResConfigState(StateMachine<LancherContext> sm, LancherContext context)
    {
        this.context = context;
        this.sm = sm;
    }

    public void OnDestroy()
    {
    }

    public void OnEnter()
    {
        var localURLPath = LancherPathUtils.GetWWWStreamingPath(LancherPathConst.ResourceConfigPath);

        LancherLoadUtils.Load(localURLPath, 10, context.loadingResourcesConfigProgress)
            .Then(SaveTextToBridge)
            .Done(LoadResConfigComplete)
            .Catch(CatchException);
    }

    /// <summary>
    /// 加载本地的配置文件完毕
    /// 去往下一个状态
    /// </summary>
    /// <param name="obj"></param>
    private void LoadResConfigComplete(LancherLoadData obj)
    {
        CodeBridgeTool.instance.SignTaskProgress(TaskID.LoadResConfig, 1f);

        //sm.ChangeState<ResolveDLLState>();
        sm.ChangeState<LoadLogoVideoState>();
    }

    /// <summary>
    /// 即便加载不成功ResConfig 也一样往后走
    /// </summary>
    /// <param name="obj"></param>
    private void CatchException(Exception obj)
    {
        //sm.ChangeState<LoadErrorState>();
        ////TODO 异常处理
        //throw obj;

        //sm.ChangeState<ResolveDLLState>();
        sm.ChangeState<LoadLogoVideoState>();
    }

    /// <summary>
    /// 保存文本到Bridge
    /// </summary>
    /// <param name="arg1"></param>
    /// <returns></returns>
    private Promise<LancherLoadData> SaveTextToBridge(LancherLoadData www)
    {
        var promise = new Promise<LancherLoadData>();

        CodeBridgeTool.instance.localResText = LancherEncodingUtils.GetString(www.bytes);

        promise.Resolve(www);

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
}
