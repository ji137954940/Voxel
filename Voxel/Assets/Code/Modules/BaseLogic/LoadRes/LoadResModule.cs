using System.Collections.Generic;
using ZFrame;

/// <summary>
/// 加载资源模块
/// </summary>
public class LoadResModule : Module
{
    protected override List<Processor> ListProcessors()
    {
        return new List<Processor>() {

            new LoadResProcessor(this),
        };
    }
}
