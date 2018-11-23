using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Tgame.Game.Table;
using ZTool.Table;

public class BaseUIWindowData : MonoBehaviour
{
    //image路径(文件名)
    public List<string> ImageDependenciesPathName = new List<string>();
    //image OverrideSprite
    public List<string> ImageDependenciesOverrideSpriteName = new List<string>();
    //image Go
    public List<Image> ImageDependenciesImage = new List<Image>();
    //RawImage路径(文件名)
    public List<string> RawImageDependenciesPathName = new List<string>();
    //RawImage Go
    public List<RawImage> RawImageDependenciesImage = new List<RawImage>();



    /// <summary>
    /// 面板对象
    /// </summary>
    public GameObject Panel;
    /// <summary>
    /// 是否为全屏界面
    /// </summary>
    public bool IsFullScreen;
    /// <summary>
    /// 模板列表数据
    /// </summary>
    public List<GameObject> ModelList;



    /// <summary>
    /// 初始化方法
    /// </summary>
    public virtual void Init() { InitSound(); }

    /// <summary>
    /// 播放窗体上挂的播放声音脚本
    /// </summary>
    void InitSound()
    {
        var sounds = GetComponentsInChildren<FMOD_UI_PlaySound>();
        for (int i = 0; i < sounds.Length; i++)
        {
            var sound = sounds[i];
            if (sound != null)
            {
                //table data
                var tableSound = Table_Client_Sound.GetPrimary(sound.EventID);
                if (tableSound != null)
                {
                    //var emmiter = sound.gameObject.AddComponent<StudioEventEmitter>();
                    //emmiter.Event = table_sound.event_str;
                    //emmiter.PlayEvent = (EmitterGameEvent)((int)sound.PlayEvent);
                    //emmiter.StopEvent = (EmitterGameEvent)((int)sound.StopEvent);

                    //if (emmiter.PlayEvent == EmitterGameEvent.ObjectEnable)
                    //{
                    //    emmiter.Play();
                    //}
                }
            }
        }
    }
}

public class ImageDependenciesInfo
{
    public Image Go;
    public string Path;
}
