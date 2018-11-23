using System.Collections;
using UnityEngine;

/// <summary>
/// 在Lancher阶段使用的支持协程的类 只是封装一下
/// @author Ollydbg
/// @date 2018-2-9
/// </summary>
public class LancherCoroutine : MonoBehaviour
{
    private static LancherCoroutine _instance;

    /// <summary>
    /// 获取单例
    /// </summary>
    /// <returns></returns>
    public static LancherCoroutine instance
    {
        get
        {
            if (_instance == null)
            {
                throw new System.Exception("没有初始化LancherCoroutine");
            }

            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
    }

    /// <summary>
    /// 开启一个协程
    /// </summary>
    /// <param name="routine"></param>
    /// <returns></returns>
    public new Coroutine StartCoroutine(IEnumerator routine)
    {
        return base.StartCoroutine(routine);
    }

    /// <summary>
    /// 销毁掉自身
    /// </summary>
    public void Dispose()
    {
        _instance = null;

        Destroy(this);
    }
}
