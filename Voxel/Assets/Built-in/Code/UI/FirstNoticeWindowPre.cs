using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 机型检测，拒绝不符合的机型进入游戏
/// </summary>
public class FirstNoticeWindowPre : MonoBehaviour
{
    public Button CloseGameBtn;

    void Start()
    {
        if (CloseGameBtn != null)
        {
            CloseGameBtn.onClick.AddListener(() =>
            {
                Application.Quit();
            });
        }
    }

}
