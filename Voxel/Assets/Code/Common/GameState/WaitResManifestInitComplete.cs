using UnityEngine;
using ZTool.Res;

/// <summary>
/// 等待ResMainfest加载完毕
/// </summary>
public class WaitResManifestInitComplete : CustomYieldInstruction
{
    public override bool keepWaiting
    {
        get
        {
            return !ResManifest.instance.IsDone;
        }
    }
}
