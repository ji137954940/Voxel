using System.Text;

/// <summary>
/// Encoding的帮助类 我们在Lancher中统一使用Ascii的编码
/// 回头方便统一换掉
/// </summary>
public static class LancherEncodingUtils
{
    public static string GetString(byte[] bytes)
    {
        return Encoding.UTF8.GetString(bytes);
    }
}
