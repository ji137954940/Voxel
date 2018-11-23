/// <summary>
/// 帮助持有住当前进度的对象
/// @author Ollydbg
/// @date 2018-2-11
/// </summary>
public class LancherProgressHolder
{
    public float _progress;
    /// <summary>
    /// 当前进度
    /// 从0-1的进度
    /// </summary>
    public float progress
    {
        get
        {
            return _progress;
        }
        set
        {
            _progress = value;

            //UnityEngine.Debug.Log(type + " xxx" + _progress);
        }
    }

    public LancherProgressType type;

    public LancherProgressHolder(LancherProgressType _type)
    {
        this.type = _type;
    }
}

/// <summary>
/// 加载进度类型
/// </summary>
public enum LancherProgressType
{
    LoadResConfig,
    LoadLogoVido,
    LoadServerConfig,
    LoadDLL,
}