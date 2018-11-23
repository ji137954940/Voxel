using System.IO;
using UnityEngine;
using ZLib;

/// <summary>
/// 显示Logo的状态
/// @author Ollydbg
/// @date 2018-2-9
/// </summary>
public class ShowLogoState : IState<LancherContext>
{
    public LancherContext context
    {
        get;
        private set;
    }

    private StateMachine<LancherContext> sm;

    public ShowLogoState(StateMachine<LancherContext> sm, LancherContext context)
    {
        this.context = context;
        this.sm = sm;
    }

    public void OnDestroy()
    {
    }

    public void OnEnter()
    {
        if (CheckPhoneType())
        {
            context.logoPanel.Show();

            ////TODO 临时方案 每次进去都会清理缓存
            //string path = string.Format("{0}/{1}", Application.persistentDataPath, "vercache");

            //if (Directory.Exists(path))
            //    Directory.Delete(path, true);
        }
        else
        {
            this.sm.ChangeState<PhoneErrorState>();
        }
    }

    /// <summary>
    /// 检查IOS的类型
    /// </summary>
    /// <returns></returns>
    public bool CheckPhoneType()
    {
        return true;

//#if UNITY_IOS
//        //ios平台只允许6s以上平台
//        switch (UnityEngine.iOS.Device.generation)
//        {
//            case UnityEngine.iOS.DeviceGeneration.iPhone6S:
//            case UnityEngine.iOS.DeviceGeneration.iPhone6SPlus:
//            case UnityEngine.iOS.DeviceGeneration.iPhone7:
//            case UnityEngine.iOS.DeviceGeneration.iPhone7Plus:
//            case UnityEngine.iOS.DeviceGeneration.iPhoneSE1Gen:
//            case UnityEngine.iOS.DeviceGeneration.iPhoneUnknown:
//                return true;
//            default:
//                return false;
//        }
//#else
//        //其他平台的暂时都可以
//        return true;
//#endif
    }

    public void OnExecute()
    {
    }

    public void OnExit()
    {
    }

    public void OnUpdate()
    {
    }
}
