using System;

/// <summary>
/// 手机不适配需要给予玩家提示
/// @author Ollydbg
/// @date 2018-2-10
/// </summary>
public class LancherPhoneErrorPanel : LancherBasePanel
{
    public override void Hide()
    {
        base.Hide();
    }

    public override void Init()
    {
        base.Init();
        this.panelType = CodeBridgeTool.EnumPreWindow.PhoneType_Filter;
    }

    public override void OnDestory()
    {
        base.OnDestory();
    }

    public override void Show()
    {
        base.Show();
    }
}
