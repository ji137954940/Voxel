using Color.Number.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ZLib;
using UnityEngine;
/// <summary>
/// 点击事件
/// </summary>
public class UIEventPointerT<T> : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IUpdateSelectedHandler, ISelectHandler
{
    public object parameter;

    public T paramT;

    /// <summary>
    /// 长按时间间隔
    /// </summary>
    public float longPressTimeMargin = 1;
    public Toggle toggle;

    public UIEventListener.VoidDelegate4<T> onClick;

    public UIEventListener.VoidDelegate1 onDown;
    public UIEventListener.VoidDelegate1 onEnter;
    public UIEventListener.VoidDelegate1 onExit;
    public UIEventListener.VoidDelegate1 onUp;
    public UIEventListener.VoidDelegate1 onDrag;
    public UIEventListener.VoidDelegate1 onEndDrag;
    public UIEventListener.VoidDelegate2 onSelect;
    public UIEventListener.VoidDelegate2 onUpdateSelect;

    //toggle 状态改变
    public UIEventListener.VoidDelegate3 onToggleValueChanged;
    /// <summary>
    /// 长按事件
    /// </summary>
    public UIEventListener.VoidDelegate1 onLongPreseeEvent = null;
    /// <summary>
    /// 记录长按的时间
    /// </summary>
    private float markPressTime = 0;

    #region GameObject 回调

    UIPlaySound _uiPlaySound = null;//音效

    void Start()
    {
        _uiPlaySound = GetComponent<UIPlaySound>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //长按事件为空或者 没有达到长按事件的触发时长，则执行正常的点击事件，同时禁用点长按
        if (onLongPreseeEvent == null || Time.time - markPressTime < longPressTimeMargin)
        {
            if (null != _uiPlaySound && UIPlaySound.Trigger.PointerClick == _uiPlaySound.trigger)
            {
                PlaySound();
            }
            if (onClick != null)
            {
                onClick(gameObject, eventData, paramT);
            }
            markPressTime = Time.time;
        }
    }

    //按下
    public void OnPointerDown(PointerEventData eventData)
    {
        if (onDown != null)
        {
            onDown(gameObject, eventData, parameter);
        }
        if (null != _uiPlaySound && UIPlaySound.Trigger.PointerDown == _uiPlaySound.trigger)
        {
            PlaySound();
        }

        if (onLongPreseeEvent != null)
        {
            //如果长按事件存在，那么点下的时候开始计时
            markPressTime = Time.time;
            Tick.AddCallback(CheckLongPress, eventData, longPressTimeMargin);
        }
    }

    private void CheckLongPress(PointerEventData eventData)
    {
        if (onLongPreseeEvent != null && Time.time - markPressTime > longPressTimeMargin)
        {
            onLongPreseeEvent(gameObject, eventData, parameter);
        }
    }

    //进入
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (onEnter != null)
        {
            onEnter(gameObject, eventData, parameter);
        }
        if (null != _uiPlaySound && UIPlaySound.Trigger.PointerEnter == _uiPlaySound.trigger)
        {
            PlaySound();
        }
    }

    //退出
    public void OnPointerExit(PointerEventData eventData)
    {
        markPressTime = Time.time;
        if (onLongPreseeEvent != null && onUp != null)
        {
            onUp(gameObject, eventData, parameter);
        }
        if (onExit != null)
        {
            onExit(gameObject, eventData, parameter);
        }
        if (null != _uiPlaySound && UIPlaySound.Trigger.PointerExit == _uiPlaySound.trigger)
        {
            PlaySound();
        }
    }

    //抬起
    public void OnPointerUp(PointerEventData eventData)
    {
        if (onUp != null)
        {
            onUp(gameObject, eventData, parameter);
        }
        if (null != _uiPlaySound && UIPlaySound.Trigger.PointerUp == _uiPlaySound.trigger)
        {
            PlaySound();
        }
    }

    //选中
    public void OnSelect(BaseEventData eventData)
    {
        if (onSelect != null)
        {
            onSelect(eventData, parameter);
        }
        if (null != _uiPlaySound && UIPlaySound.Trigger.OnSelect == _uiPlaySound.trigger)
        {
            PlaySound();
        }
    }

    //更新选中状态
    public void OnUpdateSelected(BaseEventData eventData)
    {
        if (onUpdateSelect != null)
            onUpdateSelect(eventData, parameter);
    }

    #endregion

    #region UGUI 回调

    /// <summary>
    /// toggle 状态改变
    /// </summary>
    /// <param name="isOn"></param>
    public void OnToggleValueChanged(bool isOn)
    {
        if (onToggleValueChanged != null)
            onToggleValueChanged(toggle, isOn, parameter);
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
            }
        }
    }
}
  