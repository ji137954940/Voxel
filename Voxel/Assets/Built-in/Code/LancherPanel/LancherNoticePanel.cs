using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ZTool.Json;

/// <summary>
/// 游戏进入前的公告界面
/// @author Ollydbg
/// @date 2018-2-10
/// </summary>
public class LancherNoticePanel : LancherBasePanel
{
    private AnnouncementWindowPre window;

    public override void Hide()
    {
        base.Hide();
    }

    public override void Init()
    {
        base.Init();
        this.panelType = CodeBridgeTool.EnumPreWindow.Before_game;
    }

    public override void OnDestory()
    {
        SaveWhenDestory();

        window.errorNetWorkObj.btnExit.onClick.RemoveAllListeners();

        var button = window.AnnouncementObj.exit.GetComponent<Button>();

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
        }

        base.OnDestory();
        window = null;

    }

    /// <summary>
    /// 在退出的时候保存一些信息
    /// </summary>
    private void SaveWhenDestory()
    {
        int count = passedNotice.Count;
        var sb = new StringBuilder();
        if (count > 0)
        {
            sb.Append(passedNotice[0]);

        }
        for (int i = 1; i < count; i++)
        {
            sb.Append("_");
            sb.Append(passedNotice[i]);
        }
        LancherPrefs.SetString(LancherPrefsConst.NoticePrefs, sb.ToString());
        LancherPrefs.Save();
    }

    public override void Show()
    {
        base.Show();

        if (!window)
        {
            window = container.GetComponent<AnnouncementWindowPre>();
        }
        //提示面板需要在打开以后请求网络 更新界面上的显示内容
    }

    private LancherNoticeShowType showType;

    /// <summary>
    /// 打开某一个类别的面板
    /// </summary>
    /// <param name="_type"></param>
    public void ShowType(LancherNoticeShowType _type)
    {
        this.showType = _type;

        Show();

        switch (_type)
        {
            case LancherNoticeShowType.ErrorNet:
                ShowErrorNet();
                break;
            case LancherNoticeShowType.Notice:
                ShowNotice();
                break;
            case LancherNoticeShowType.TryConnectServer:
                ShowConnectServer();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 显示连接服务器的状态
    /// </summary>
    private void ShowConnectServer()
    {
        if (window == null)
        {
            Debug.LogWarning("这里不应该为空");
            return;
        }

        window.errorNetWorkObj.ToggleActive(false);
        window.AnnouncementObj.ToggleActive(false);
        window.LoadingObj.ToggleActive(true);
    }

    /// <summary>
    /// 显示Notice
    /// </summary>
    private void ShowNotice()
    {

        if (window == null)
        {
            Debug.LogWarning("这里不应该为空");
            return;
        }

        window.errorNetWorkObj.ToggleActive(false);
        window.AnnouncementObj.ToggleActive(true);
        window.LoadingObj.ToggleActive(false);

        string path = string.Format("{0}{1}",  /*"192.168.17.63:8080"*/context.serverInfo.loginServerUrl, "//login-notice/list");

        ReadGameFile();

        LancherLoadUtils.Load(path, 10)
            .Then(NoticeRepose)
            .Catch(ExceptionHandler);

        var button = window.AnnouncementObj.exit.GetComponent<Button>();

        if (button != null)
        {
            button.onClick.AddListener(ClickCloseHandler);
        }
    }

    private void ClickCloseHandler()
    {

        machine.ChangeState<EnterGameState>();//进入游戏内部

        var button = window.AnnouncementObj.exit.GetComponent<Button>();

        if (button != null)
        {
            button.onClick.RemoveListener(ClickCloseHandler);
        }
        //防止立即执行导致的BUG
        if (!CodeBridgeTool.instance.LoginPanelOpen)
        {
            LancherCoroutine.instance.StartCoroutine(WaitForLoginOpenThenClose());
        }
        else
        {
            this.Hide();

            if (context.callDispose != null)
                context.callDispose();
        }
    }

    /// <summary>
    /// 总是等登录面板打开后 才会关闭提示面板 销毁整个内容
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForLoginOpenThenClose()
    {
        yield return new WaitLoginPanelOpenCloseNotice();

        this.Hide();

        if (context.callDispose != null)
            context.callDispose();
    }

    private class WaitLoginPanelOpenCloseNotice : CustomYieldInstruction
    {
        public override bool keepWaiting
        {
            get
            {
                return !CodeBridgeTool.instance.LoginPanelOpen;
            }
        }
    }

    /// <summary>
    /// 判断是否是new的逻辑
    /// </summary>
    private void ReadGameFile()
    {
        if (LancherPrefs.HasKey(LancherPrefsConst.NoticePrefs) == false) return;
        string gamefile = LancherPrefs.GetString(LancherPrefsConst.NoticePrefs);
        string[] gamefiles = gamefile.Split('_');
        if (passedNotice == null) passedNotice = new List<string>();
        passedNotice.AddRange(gamefiles);
    }

    /// <summary>
    /// 加载Notice
    /// </summary>
    /// <param name="e"></param>
    private void ExceptionHandler(Exception e)
    {
        throw e;
    }

    private IPromise<LancherLoadData> NoticeRepose(LancherLoadData arg1)
    {
        var promise = new Promise<LancherLoadData>();

        var text = LancherEncodingUtils.GetString(arg1.bytes);

        this.ResolveJson(text);

        ReSortList();

        promise.Resolve(arg1);

        return promise;
    }
    private List<AnnouncementListItem> itemList = new List<AnnouncementListItem>();

    private List<string> passedNotice = new List<string>();

    private void ResolveJson(string json)
    {
        var arr = (ArrayList)JsonResolve.JsonDecode(json);
        for (int i = 0; i < arr.Count; i++)
        {
            var item = (Hashtable)arr[i];
            var ac = new AnnouncementWindowPre.AnnouncementContent();
            //object o = item["id"];

            ac.id = item["id"].ToString();

            ac.priority = item["priority"].ToString();
            ac.type = item["type"].ToString();
            ac.title = item["title"].ToString();
            ac.content = item["content"].ToString();

            AnnouncementListItem go = GameObject.Instantiate<AnnouncementListItem>(window.AnnouncementObj.prefab);
            go.gameObject.SetActive(true);
            go.transform.SetParent(window.AnnouncementObj.listContaniner);
            go.transform.localScale = Vector3.one;
            go.UpdateContent(ac);

            itemList.Add(go);

            var btn = go.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() =>
                {
                    window.AnnouncementObj.SetContent(ac);
                    go.HighLight(true);
                    itemList.ForEach(f =>
                    {
                        if (f != go)
                        {
                            f.HighLight(false);
                        }
                    });
                    if (passedNotice.Contains(ac.id) == false)
                    {
                        passedNotice.Add(ac.id);
                    }
                    go.UpdateNewState(!passedNotice.Contains(ac.id));
                });
            }

            go.UpdateNewState(!passedNotice.Contains(ac.id));
        }

        if (itemList.Count > 0)
        {
            itemList[0].GetComponent<Button>().onClick.Invoke();
        }
    }

    /// <summary>
    /// 重新列表排序，根据 priority排列
    /// </summary>
    private void ReSortList()
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            var item = itemList[i];
            item.ChangePriority();
        }
    }



    /// <summary>
    /// 显示连接错误的状态
    /// </summary>
    private void ShowErrorNet()
    {
        if (window == null)
        {
            Debug.LogWarning("这里不应该为空");
            return;
        }

        window.errorNetWorkObj.ToggleActive(true);
        window.AnnouncementObj.ToggleActive(false);
        window.LoadingObj.ToggleActive(false);

        window.errorNetWorkObj.btnExit.onClick.AddListener(ErrorNetClickHandler);
    }

    /// <summary>
    /// 错误点击退出游戏
    /// </summary>
    private void ErrorNetClickHandler()
    {
        LancherApplicationUtils.QuitApplication();
    }
}

/// <summary>
/// 打开面板的类型
/// </summary>
public enum LancherNoticeShowType
{
    /// <summary>
    /// 打开网络未连接
    /// </summary>
    ErrorNet,
    /// <summary>
    /// 公告
    /// </summary>
    Notice,
    /// <summary>
    /// 连接服务器的状态
    /// </summary>
    TryConnectServer
}
