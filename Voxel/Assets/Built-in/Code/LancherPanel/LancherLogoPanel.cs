using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Logo界面逻辑
/// </summary>
public class LancherLogoPanel : LancherBasePanel
{
    private LogoWindowPre window;

    private List<Texture> textures;

    private string[] sprites = {
        "Res/UI/PackerImg/splash","Res/UI/PackerImg/DengLu_BeiJing"
    };

    public override void Hide()
    {
        base.Hide();
    }

    public override void Init()
    {
        base.Init();
        this.panelType = CodeBridgeTool.EnumPreWindow.Logo;
    }

    public override void OnDestory()
    {
        base.OnDestory();
        window = null;
    }

    public override void Show()
    {
        base.Show();

        if (!window)
            window = container.GetComponent<LogoWindowPre>();

        if (window.logoImage != null)
        {
            window.logoImage.gameObject.SetActive(true);
        }
        textures = new List<Texture>();

        for (int i = 0; i < sprites.Length; i++)
        {
            textures.Add(Resources.Load<Texture>(sprites[i]));
        }

        //显示Loading 界面 heping
        LancherCoroutine.instance.StartCoroutine(OnTickOver());
    }

    /// <summary>
    /// @codereview 
    /// </summary>
    /// <returns></returns>
    private IEnumerator OnTickOver()
    {
        window.logoImage.texture = textures[0];

        yield return new WaitForSeconds(3f);

#if UNITY_ANDROID || UNITY_IOS || UNITY_IPHONE
        //Handheld.PlayFullScreenMovie("TG_Logo_Movie.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput);
#endif

        //this.logoImage.CrossFadeAlpha(0, 0.5f, true);
        //yield return new WaitForSeconds(0.5f);

        yield return new WaitForEndOfFrame();
        window.logoImage.texture = textures[1];
        yield return new WaitForEndOfFrame();

        OnPlayOver();
    }

    private void OnPlayOver()
    {
        //this.machine.ChangeState<LoadLocalConfigState>();

        //加载决定是远程还是本地的配置文件
        this.machine.ChangeState<LoadResConfigState>();

        Clear();

        if (!context.needUpdateGame)
            //TODO 考虑这里是否需要封装 在检查到网络问题的情况下 怎么办
            this.context.loadingPanel.Show();
        else
            this.context.updatePanel.Show();

        this.Hide();

        //if (PreUIWindowManager.instance.OnProcessorComplete != null)
        //{
        //    PreUIWindowManager.instance.OnProcessorComplete();
        //}
        //else
        //{
        //    Tick.AddCallback(AnnouncementWindowPre.instance.delayShowLoding, 1);
        //}

        //var errorWin = GameObject.FindObjectOfType<NetErrorWindow>();
        //if (errorWin && errorWin.gameObject.activeInHierarchy)
        //{
        //    return;
        //}

        //PreUIWindowManager.instance.OpenWindow(CodeBridgeTool.EnumPreWindow.Loading, CodeBridgeTool.LoadMessageTypeEnum.Config);
        //PreUIWindowManager.instance.UpdateNetErrorHierarchy();
        //PreUIWindowManager.instance.Close(CodeBridgeTool.EnumPreWindow.Logo);

        //Tick.AddCallback(AnnouncementWindowPre.instance.delayShowLoding, 1);


        //PreUIWindowManager.instance.Open(EnumPreWindow.Before_game);
        //UIWindowManager.instance.Close<LogoModule>();
    }

    public void Clear()
    {
        //临时处理，第二张图为公告界面背景图，如果卸载那么公告界面背景图就被卸载掉了
        //for (int i = 0; i < sprites.Length; i++)
        for (int i = 0; i < 1; i++)
        {
            Resources.UnloadAsset(textures[i]);

            textures[i] = null;
        }
        window.logoImage.texture = null;
    }
}
