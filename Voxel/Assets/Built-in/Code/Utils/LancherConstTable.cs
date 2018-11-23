/// <summary>
/// Lancher的错误常量
/// 主要是预留给未来做多语言
/// @author Ollydbg
/// @date 2018-2-11
/// </summary>
public static class LancherConstTable
{
    public const string LoadConfigError = "读取Config文件出错!";

    public const string ParseConfigFileError = "读取配置文件的数据不完整";

    public const string LoadServerConfigFileError = "加载文件出错了!";

    public const string ParseServerFileError = "解析服务器配置文件出错";

    public const string LoadDLLError = "DLL加载出错了!";

    public const string CanNotFoundAssembly = "没有找到合适的入口程序集!";

    public const string CanNotFoundClass = "在程序集中没有找到合适的入口类!";

    public const string AddToComponentError = "在GameObject上添加入口类失败了,需要检查是否继承自Monobehavior";

    public const string CanNotFoundEntryFunction = "在入口实例对象上未找到入口方法OnGameStart";

    public const string CanNotExecuteEntryFunction = "在执行入口方法的时候出了错误,可能是参数个数传递错误等原因.";
}
