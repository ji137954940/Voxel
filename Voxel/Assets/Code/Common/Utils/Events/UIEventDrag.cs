using Color.Number.Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ZLib;

/// <summary>
/// 拖拽事件
/// </summary>
public class UIEventDrag : MonoBehaviour, IEventSystemHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public object parameter;

    public UIEventListener.VoidDelegate1 onBeginDrag;
    public UIEventListener.VoidDelegate1 onDrag;
    public UIEventListener.VoidDelegate1 onEndDrag;

    //toggle 状态改变
    public UIEventListener.VoidDelegate3 onToggleValueChanged;

    //滚动区域对象
    public ScrollRect sr;

    #region GameObject 回调

    UIPlaySound _uiPlaySound = null;//音效

    void Start()
    {
        _uiPlaySound = GetComponent<UIPlaySound>();
    }

    /// <summary>
    /// 开始拖拽
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (sr != null)
            sr.OnBeginDrag(eventData);

        if (onBeginDrag != null)
        {
            onBeginDrag(gameObject, eventData, parameter);
        }
        if (null != _uiPlaySound && UIPlaySound.Trigger.BeginDrag == _uiPlaySound.trigger)
        {
            PlaySound();
        }
    }

    /// <summary>
    /// 拖拽
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        if (sr != null)
            sr.OnDrag(eventData);

        if (onDrag != null)
        {
            onDrag(gameObject, eventData, parameter);
        }
        if (null != _uiPlaySound && UIPlaySound.Trigger.Drag == _uiPlaySound.trigger)
        {
            PlaySound();
        }
    }

    /// <summary>
    /// 拖拽结束
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        if (sr != null)
            sr.OnEndDrag(eventData);

        if (onEndDrag != null)
        {
            onEndDrag(gameObject, eventData, parameter);
        }
        if (null != _uiPlaySound && UIPlaySound.Trigger.EndDrag == _uiPlaySound.trigger)
        {
            PlaySound();
        }
    }

    #endregion

    /// <summary>
    /// 播放声音
    /// </summary>
    void PlaySound()
    {
        if (_uiPlaySound.allowPlay)
        {
            if (_uiPlaySound.soundEventId != 0)
            {
                Button button = GetComponent<Button>();
                if (null != button)
                {
                    if (_uiPlaySound.buttonDisablePlay)
                    {
                        AudioManager.instance.PlayUISound(_uiPlaySound.soundEventId, this);
                    }
                    else if (button.interactable)
                    {//button可用时播放
                        AudioManager.instance.PlayUISound(_uiPlaySound.soundEventId, this);
                    }
                }
                else
                {
                    AudioManager.instance.PlayUISound(_uiPlaySound.soundEventId, this);
                }
            }
            else
            {
                Debug.LogError(GetComponentInParent<BaseUIWindowData>() + " --> " + gameObject.name + " UIPlaySound audioClip is null");
                //#if UNITY_EDITOR
                //                Debug.LogError(GetComponentInParent<BaseUIWindowData>() + " --> " + UnityEditor.AnimationUtility.CalculateTransformPath(transform, GetComponentInParent<BaseUIWindowData>().transform) + " UIPlaySound audioClip is null");
                //#else
                //#endif
            }
        }
    }

}

