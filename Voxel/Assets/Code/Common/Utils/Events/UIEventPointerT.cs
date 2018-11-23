using Color.Number.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ZLib;
using UnityEngine;
/// <summary>
/// ����¼�
/// </summary>
public class UIEventPointerT<T> : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IUpdateSelectedHandler, ISelectHandler
{
    public object parameter;

    public T paramT;

    /// <summary>
    /// ����ʱ����
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

    //toggle ״̬�ı�
    public UIEventListener.VoidDelegate3 onToggleValueChanged;
    /// <summary>
    /// �����¼�
    /// </summary>
    public UIEventListener.VoidDelegate1 onLongPreseeEvent = null;
    /// <summary>
    /// ��¼������ʱ��
    /// </summary>
    private float markPressTime = 0;

    #region GameObject �ص�

    UIPlaySound _uiPlaySound = null;//��Ч

    void Start()
    {
        _uiPlaySound = GetComponent<UIPlaySound>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //�����¼�Ϊ�ջ��� û�дﵽ�����¼��Ĵ���ʱ������ִ�������ĵ���¼���ͬʱ���õ㳤��
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

    //����
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
            //��������¼����ڣ���ô���µ�ʱ��ʼ��ʱ
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

    //����
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

    //�˳�
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

    //̧��
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

    //ѡ��
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

    //����ѡ��״̬
    public void OnUpdateSelected(BaseEventData eventData)
    {
        if (onUpdateSelect != null)
            onUpdateSelect(eventData, parameter);
    }

    #endregion

    #region UGUI �ص�

    /// <summary>
    /// toggle ״̬�ı�
    /// </summary>
    /// <param name="isOn"></param>
    public void OnToggleValueChanged(bool isOn)
    {
        if (onToggleValueChanged != null)
            onToggleValueChanged(toggle, isOn, parameter);
    }

    #endregion

    /// <summary>
    /// ��������
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
                    {//button����ʱ����
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
  