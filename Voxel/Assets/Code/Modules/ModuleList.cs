using System.Collections.Generic;
using ZFrame;

/// <summary>
/// 注册模块
/// </summary>
public class ModuleList : Frame
{
    protected override List<Module> ListModules()
    {
        return new List<Module>()
        {

            //加载配置文件模块
            new LoadResModule(),
            
        };
    }
}
