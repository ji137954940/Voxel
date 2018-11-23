using UnityEngine.UI;
using UnityEngine;

public class AnnouncementListItem : MonoBehaviour
{
    AnnouncementWindowPre.AnnouncementContent data;
    [SerializeField]
    Image[] kind;
    [SerializeField]
    Text content;
    [SerializeField]
    Image newMsg;
    [SerializeField]
    Image normalBg;
    [SerializeField]
    Image highLightBg;

    internal void UpdateContent(AnnouncementWindowPre.AnnouncementContent item)
    {
        if (item != null)
        {
            this.data = item;
            if (item == null) return;

            content.text = item.title;
            UpdateType(int.Parse(item.type));
        }
    }

    public UnityEngine.Color hightlight_color = new UnityEngine.Color((float)255 / 255, (float)255 / 255, (float)255 / 255);
    public UnityEngine.Color normal_color = new UnityEngine.Color((float)160 / 255, (float)200 / 255, (float)249 / 255);

    public void HighLight(bool hightlight)
    {
        normalBg.gameObject.SetActive(!hightlight);
        highLightBg.gameObject.SetActive(hightlight);
        content.color = hightlight ? hightlight_color : normal_color;
    }

    /// <summary>
    /// 类型显示
    /// </summary>
    /// <param name="type"></param>
    public void UpdateType(int type)
    {
        for (int i = 0; i < kind.Length; i++)
        {
            var item = kind[i];
            if (item != null)
            {
                item.gameObject.SetActive(i == (type - 1));
            }
        }
    }


    string[] typeStr = { "公告", "活动" };
    private string GetTypeStr(int type)
    {
        return typeStr[type - 1];
    }

    /// <summary>
    /// 是否显示 new  标识
    /// </summary>
    public void UpdateNewState(bool isnew)
    {
        if (newMsg)
        {
            newMsg.gameObject.SetActive(isnew);
        }
    }

    internal void ChangePriority()
    {
        transform.SetSiblingIndex(int.Parse(data.priority));
    }
}