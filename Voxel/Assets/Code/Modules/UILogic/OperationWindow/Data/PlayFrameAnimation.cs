using System.Collections.Generic;
using Color.Number.Animation;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 播放帧动画
/// </summary>
public class PlayFrameAnimation : MonoBehaviour
{
    /// <summary>
    /// 动画对象
    /// </summary>
    public GameObject go;

    /// <summary>
    /// 播放帧动画的主体
    /// </summary>
    public RawImage sprite;

    /// <summary>
    /// 动画帧率
    /// </summary>
    public int fps = 12;

    /// <summary>
    /// sprite 序列帧
    /// </summary>
    public List<GifAnimation.GifTexture> list;

    /// <summary>
    /// 是否正在播放
    /// </summary>
    private bool isPlaying = false;

    /// <summary>
    /// 当前播放的帧动画的 id
    /// </summary>
    private int indexId = 0;

    /// <summary>
    /// 当前sprite 动画的 总帧数
    /// </summary>
    private int textureCount = 0;

    /// <summary>
    /// 当前帧时间
    /// </summary>
    private float time;

    /// <summary>
    /// 是否需要屏幕录像
    /// </summary>
    private bool isREC = false;

    /// <summary>
    /// 屏幕录像 的indexId
    /// </summary>
    private int recTextureIndexId = -1;

    /// <summary>
    /// 屏幕录像截屏回调
    /// </summary>
    private Action<int, int> recAction;

    /// <summary>
    /// 开始播放帧动画
    /// </summary>
    /// <param name="list"></param>
    /// <param name="recAction"></param>
    /// <param name="isRec"></param>
    /// <param name="isPlay"></param>
    public void PlayAnimation(List<GifAnimation.GifTexture> list, Action<int, int> recAction, bool isRec, bool isPlay = false)
    {
        if(list == null || list.Count == 0)
            return;

        this.list = list;
        this.isREC = isRec;
        this.recAction = recAction;
        if (isPlay)
            PlayAnimation(isRec);
    }

    /// <summary>
    /// 开始播放帧动画
    /// </summary>
    public void PlayAnimation(bool isRec)
    {
        if (list != null)
        { 
            textureCount = list.Count;
            //临时设置 1秒钟播放完成
            fps = textureCount;
            indexId = 0;
            isPlaying = true;
            recTextureIndexId = -1;
            this.isREC = isRec;
        }
    }

    /// <summary>
    /// 停止播放帧动画
    /// </summary>
    /// <param name="isClear"></param>
    public void StopAnimation(bool isClear)
    {
        isPlaying = false;
        if(isClear)
            Clear();
    }

    private void Update()
    {
        if (isPlaying)
        {
            //time += Time.deltaTime;
            //indexId = (int)(time * fps) % textureCount;

            time += Time.deltaTime;
            if (time >= list[indexId].DelaySec)
            {
                indexId = (indexId + 1) % textureCount;
                time = 0;

            }

            sprite.texture = list[indexId].Texture2D;

            if(isREC
                && recTextureIndexId != indexId
                && recTextureIndexId <= textureCount)
            {
                recTextureIndexId = indexId;
                recAction(indexId, textureCount);
                if (recTextureIndexId == textureCount - 1)
                    isREC = false;
            }
        }
    }

    /// <summary>
    /// 清理信息数据
    /// </summary>
    public void Clear()
    {
        if (list != null)
        {
            list.Clear();
            list = null;
        }

        if (recAction != null)
            recAction = null;

        sprite.texture = null;
        textureCount = 0;
        indexId = 0;
        isPlaying = false;
        isREC = false;
        recTextureIndexId = -1;
    }
}
