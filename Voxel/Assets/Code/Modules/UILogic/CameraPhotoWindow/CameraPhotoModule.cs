using System.Collections.Generic;
using ZFrame;
using ZLib;


/// <summary>
/// 相机拍照功能
/// </summary>
public class CameraPhotoModule : BaseUIModule
{
    protected override List<Processor> ListProcessors()
    {
        return new List<Processor>()
        {
            new CameraPhotoWindow(this)
        };
    }

    protected override void OnInit()
    {
        base.OnInit();
        this.Path = "CameraPhotoWindow";
    }

    /// <summary>
    /// 资源加载完成回调
    /// </summary>
    /// <param name="path"></param>
    /// <param name="obj"></param>
    /// <param name="parameter"></param>
    protected override void OnResLoadOver(string path, UnityEngine.Object obj, object parameter)
    {
        base.OnResLoadOver(path, obj, parameter);

        var data = UIObject.AddAndCreateConnection<CameraPhotoData>(go);
        var window = GetProcessor<CameraPhotoWindow>();
        window.Init(data);
    }

    public override void Close()
    {
        base.Close();
    }

}
