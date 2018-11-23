using System;
using UnityEngine;

/// <summary>
/// 等策划配置表加载完毕
/// </summary>
public class WaitResLoadComplete : CustomYieldInstruction
{
    private float startTime;

    public WaitResLoadComplete()
    {
        startTime = Time.time;
    }

    public override bool keepWaiting
    {
        get
        {
            if (Time.time - startTime > 20)
            {
                throw new Exception("加载配置文件超时!!!!");
            }

            return !GameData.instance.ConfigReady;
        }
    }
}
