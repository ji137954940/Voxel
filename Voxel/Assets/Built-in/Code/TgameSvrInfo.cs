/// <summary>
/// 服务器信息
/// </summary>
public class TgameSvrInfo
{
    /// <summary>
    /// 游戏版本
    /// </summary>
    public string gameVersion = null;
    public string hotReloadUrl = null;
    public string hotReloadVersion = null;
    public int id = 0;
    /// <summary>
    /// 登录服务器地址
    /// </summary>
    public string loginServerUrl = null;
    /// <summary>
    /// 渠道名
    /// </summary>
    public string name = null;
    /// <summary>
    /// 版本标记
    /// </summary>
    public string remark = null;
    /// <summary>
    /// 资源服务器地址
    /// </summary>
    public string resServerUrl = null;
    ///// <summary>
    ///// Log服务器地址
    ///// </summary>
    public string logServerUrl = null;    // todo  json 解析需要知道他的  位置，  JsonUtitlty
    /// <summary>
    /// 资源版本
    /// </summary>
    public int resVersion = 0;
    /// <summary>
    /// 是否需要激活码
    /// </summary>
    public bool needCode = false;
    /// <summary>
    /// 状态
    /// </summary>
    public string success = null;

    //渠道设置
    public string opChannel = null;
}