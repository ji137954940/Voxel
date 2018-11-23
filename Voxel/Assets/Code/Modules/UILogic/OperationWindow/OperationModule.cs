using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZFrame;
using ZLib;
using Object = UnityEngine.Object;


/// <summary>
/// 操作界面模块
/// </summary>
public class OperationModule : BaseUIModule
{

    protected override List<Processor> ListProcessors()
    {
        return new List<Processor>()
        {
            new OperationWindow(this)
        };
    }

    protected override void OnInit()
    {
        base.OnInit();
        this.Path = "OperationWindow";
    }

    /// <summary>
    /// 资源加载完成回调
    /// </summary>
    /// <param name="path"></param>
    /// <param name="obj"></param>
    /// <param name="parameter"></param>
    protected override void OnResLoadOver(string path, Object obj, object parameter)
    {
        base.OnResLoadOver(path, obj, parameter);


        var data = UIObject.AddAndCreateConnection<OperationWindowData>(go);
        var window = GetProcessor<OperationWindow>();
        window.Init(data);
    }

    public override void Close()
    {
        base.Close();
    }
}

