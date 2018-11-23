using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZFrame;
using ZLib;
using Object = UnityEngine.Object;


/// <summary>
/// 主控界面模块
/// </summary>
public class MainWindowModule : BaseUIModule
{

    protected override List<Processor> ListProcessors()
    {
        return new List<Processor>()
        {
            new MainWindow(this)
        };
    }

    protected override void OnInit()
    {
        base.OnInit();
        this.Path = "MainWindow";
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


        var data = UIObject.AddAndCreateConnection<MainWindowData>(go);
        var window = GetProcessor<MainWindow>();
        window.Init(data);
    }

    public override void Close()
    {
        base.Close();
    }
}

