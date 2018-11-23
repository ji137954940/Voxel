using UnityEngine;
using ZTool.Res;

/// <summary>
/// 等资源加载完成
/// </summary>
public class WaitResInitComplete : CustomYieldInstruction
{
    public override bool keepWaiting
    {
        get
        {
            return !ResVerManager.instance.IsDone;
        }
    }
}
