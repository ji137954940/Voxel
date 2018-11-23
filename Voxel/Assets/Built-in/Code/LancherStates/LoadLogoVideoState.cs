using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZLib;

/// <summary>
/// 准备数据 把Logo视频加载，放入缓存中存储
/// 用于后面进行视频合成时使用
/// </summary>
public class LoadLogoVideoState : IState<LancherContext>
{

    public LancherContext context
    {
        get;
        private set;
    }

    private StateMachine<LancherContext> sm;

    public LoadLogoVideoState(StateMachine<LancherContext> sm, LancherContext context)
    {
        this.context = context;
        this.sm = sm;
    }

    public void OnDestroy()
    {
    }

    public void OnEnter()
    {
        if(!IsHaveLogoVideoInCache())
        {
            var localURLPath = LancherPathUtils.GetWWWStreamingPath(LancherPathConst.ResourceLogoVideoPath);

            LancherLoadUtils.Load(localURLPath, 10, context.loadingLogoVideoProgress)
                .Then(SaveLogoVideoToCache)
                .Done(LoadLogoVideoComplete)
                .Catch(CatchException);
        }
        else
        {
            LoadLogoVideoComplete(null);
        }
    }

    /// <summary>
    /// 加载Logo视频完成，去往下一个状态
    /// </summary>
    /// <param name="obj"></param>
    private void LoadLogoVideoComplete(LancherLoadData obj)
    {
        CodeBridgeTool.instance.SignTaskProgress(TaskID.LoadLogoVideo, 1f);

        sm.ChangeState<ResolveDLLState>();
    }

    /// <summary>
    /// 即使加载失败，也一样继续往下走
    /// </summary>
    /// <param name="obj"></param>
    private void CatchException(Exception obj)
    {
        sm.ChangeState<ResolveDLLState>();
    }

    /// <summary>
    /// 存储Logo视频到本地缓存中
    /// </summary>
    /// <param name="www"></param>
    /// <returns></returns>
    private Promise<LancherLoadData> SaveLogoVideoToCache(LancherLoadData www)
    {
        var promise = new Promise<LancherLoadData>();

        var path = GetLogoVideoCachePath();

        System.IO.File.WriteAllBytes(path, www.bytes);

        promise.Resolve(www);

        return promise;
    }

    /// <summary>
    /// 在缓存中是否已经存在 Logo 视频
    /// </summary>
    /// <returns></returns>
    private bool IsHaveLogoVideoInCache()
    { 
        return System.IO.File.Exists(GetLogoVideoCachePath());
    }

    /// <summary>
    /// 获取Logo视频缓存地址
    /// </summary>
    /// <returns></returns>
    private string GetLogoVideoCachePath()
    {
        return string.Format("{0}/{1}/{2}",
                LancherPathConst.CACHE_PATH,
                LancherPathConst.GROUP_SETUP,
                LancherPathConst.ResourceLogoVideoPath);
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

