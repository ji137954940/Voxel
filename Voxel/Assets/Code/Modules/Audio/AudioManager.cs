using UnityEngine;
using ZLib;

namespace Color.Number.Audio
{
    public class AudioManager : Singleton<AudioManager>
    {
        /// <summary>
        /// 播放UI音效
        /// </summary>
        /// <param name="soundEventId"></param>
        /// <param name="c"></param>
        internal void PlayUISound(int soundEventId, Component c)
        {
            if (soundEventId != 0)
            {
                //if (!FMODManager.instance.PlayOneShot(soundEventId))
                //{
                //    Debug.LogWarning(c.GetComponentInParent<BaseUIWindowData>() + " 模块的" + c.gameObject.name + "指定的音效在sound_bank中找不到, id = " + soundEventId);
                //}
            }
        }
    }
}