using UnityEngine;

/// <summary>
/// 更新客户端界面逻辑
/// @author 王岳
/// @data 2018/6/5
/// @codereview 
/// </summary>
public class LancherUpdatePanel : LancherBasePanel {

    private UpdateNoticeWindowPre window;

    public override void Init()
    {
        base.Init();
        this.panelType = CodeBridgeTool.EnumPreWindow.Update;
    }

    public override void Show()
    {
        base.Show();

        if (window == null)
            window = container.GetComponent<UpdateNoticeWindowPre>();
        
        window.versionNumber_txt.text = string.Format("当前版本为 V{0}", context.localGameVersion);
        window.updateNow_btn.onClick.AddListener(OnClickUpdateClient);
        window.updateLater_btn.onClick.AddListener(OnClickExit);
    }

    private void OnClickUpdateClient()
    {
        // 打开更新链接
        Application.OpenURL("http://krlyv1.res.ts100.com/krly/release/TgameIosRelease.html");
        OnClickExit();
    }

    private void OnClickExit()
    {
        LancherApplicationUtils.QuitApplication();
    }

    public override void Hide()
    {
        base.Hide();
    }



}
