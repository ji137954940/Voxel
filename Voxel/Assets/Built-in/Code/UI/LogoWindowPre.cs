using UnityEngine;
//using System.Collections;
using UnityEngine.UI;
//using System.Collections.Generic;

public class LogoWindowPre : MonoBehaviour
{
    public RawImage logoImage;
    public RawImage movieRawImage;

    //    private string[] sprites = {
    //        "Res/UI/PackerImg/splash","Res/UI/PackerImg/DengLu_BeiJing"
    //    };
    //    List<Texture> textures;
    //    private void Start()
    //    {
    //        if (this.logoImage != null)
    //        {
    //            this.logoImage.gameObject.SetActive(true);
    //        }
    //        textures = new List<Texture>();

    //        for (int i = 0; i < sprites.Length; i++)
    //        {
    //            textures.Add(Resources.Load<Texture>(sprites[i]));
    //        }

    //        //显示Loading 界面 heping
    //        LancherCoroutine.instance.StartCoroutine(OnTickOver());

    //        // 统计
    //        GameDataStatisticalAnalysisManager.instance.OnEvent(StatisticalAnalysisEnum.Logo);
    //    }
    //    private IEnumerator OnTickOver()
    //    {
    //        //for (int i = 0; i < sprites.Length; i++)
    //        //{
    //        //    this.logoImage.texture = textures[i];
    //        //}
    //        this.logoImage.texture = textures[0];
    //        //yield return new WaitForEndOfFrame();
    //        if (PreUIWindowManager.instance.OnProcessorComplete != null)
    //        {
    //            PreUIWindowManager.instance.OnProcessorComplete();
    //            PreUIWindowManager.instance.OnProcessorComplete = null;
    //        }

    //        yield return new WaitForSeconds(3f);
    //        //this.logoImage.CrossFadeAlpha(0, 0.5f, true);
    //        //yield return new WaitForSeconds(0.5f);
    //        //if (Application.internetReachability == NetworkReachability.NotReachable)
    //        //{
    //        //    Debug.LogError("   网络不可达NotReachable ， 不继续往下走");
    //        //    yield break;
    //        //}

    //        //GameUtils.DispatchEvent<ME_Play_Video>(ConstantConfig.Logo_Movie_Path, (MediaPlayerCtrl.VideoEnd)OnPlayOver);
    //#if UNITY_ANDROID || UNITY_IOS || UNITY_IPHONE
    //        Handheld.PlayFullScreenMovie("TG_Logo_Movie.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput);
    //#endif

    //        //this.logoImage.CrossFadeAlpha(0, 0.5f, true);
    //        //yield return new WaitForSeconds(0.5f);

    //        yield return new WaitForEndOfFrame();
    //        this.logoImage.texture = textures[1];
    //        yield return new WaitForEndOfFrame();

    //        OnPlayOver();
    //    }

    //    private void OnPlayOver()
    //    {
    //        Clear();

    //        //if (PreUIWindowManager.instance.OnProcessorComplete != null)
    //        //{
    //        //    PreUIWindowManager.instance.OnProcessorComplete();
    //        //}
    //        //else
    //        //{
    //        //    Tick.AddCallback(AnnouncementWindowPre.instance.delayShowLoding, 1);
    //        //}

    //        //var errorWin = GameObject.FindObjectOfType<NetErrorWindow>();
    //        //if (errorWin && errorWin.gameObject.activeInHierarchy)
    //        //{
    //        //    return;
    //        //}

    //        PreUIWindowManager.instance.OpenWindow(CodeBridgeTool.EnumPreWindow.Loading, CodeBridgeTool.LoadMessageTypeEnum.Config);
    //        PreUIWindowManager.instance.UpdateNetErrorHierarchy();
    //        PreUIWindowManager.instance.Close(CodeBridgeTool.EnumPreWindow.Logo);

    //        //Tick.AddCallback(AnnouncementWindowPre.instance.delayShowLoding, 1);


    //        //PreUIWindowManager.instance.Open(EnumPreWindow.Before_game);
    //        //UIWindowManager.instance.Close<LogoModule>();
    //    }

    //    public void Clear()
    //    {
    //        //临时处理，第二张图为公告界面背景图，如果卸载那么公告界面背景图就被卸载掉了
    //        //for (int i = 0; i < sprites.Length; i++)
    //        for (int i = 0; i < 1; i++)
    //        {
    //            Resources.UnloadAsset(textures[i]);

    //            textures[i] = null;
    //        }
    //        this.logoImage.texture = null;
    //    }
}
