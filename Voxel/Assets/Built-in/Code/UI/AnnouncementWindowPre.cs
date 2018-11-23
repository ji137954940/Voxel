using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 如果有网络
/// </summary>
public class AnnouncementWindowPre : MonoBehaviour
{
    public ErrorNetRef errorNetWorkObj;
    public AnnouncementRef AnnouncementObj;
    public LodingRef LoadingObj;


    [System.Serializable]
    public class ABRef
    {
        public GameObject main;

        public void ToggleActive(bool show)
        {
            if (main && main.activeInHierarchy != show)
            {
                main.SetActive(show);
            }
        }
    }

    [System.Serializable]
    public class ErrorNetRef : ABRef
    {
        public Text title;
        public Button btnExit;
    }
    [System.Serializable]
    public class AnnouncementRef : ABRef
    {
        public Text title;
        public Text innerTitle;
        public Text content;
        public GameObject exit;
        public Transform listContaniner;

        public AnnouncementListItem prefab;
        internal void SetContent(AnnouncementContent ac)
        {
            this.innerTitle.text = ac.title;
            string content = ac.content;
            content = content.Replace("\\n", "\n");
            this.content.text = content;
        }
    }


    [System.Serializable]
    public class LodingRef : ABRef
    {

    }

    public class AnnouncementContent
    {
        public string id;
        public string priority;
        public string type;
        public string title;
        public string content;
    }

}
