using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
/// <summary>
/// Lancher加载工具类
/// @author Ollydbg
/// @date 2018-2-10
/// </summary>
public class LancherLoadUtils
{
    /// <summary>
    /// 开始加载一个URL
    /// </summary>
    /// <param name="_url"></param>
    /// <returns></returns>
    public static Promise<LancherLoadData> Load(string _url, float _timeout)
    {
        var promise = new Promise<LancherLoadData>();

        LancherCoroutine.instance.StartCoroutine(Load(_url, promise, _timeout, null));

        return promise;
    }

    /// <summary>
    /// 设计一个支持异步
    /// 支持timeout
    /// 支持进度反馈
    /// 的加载函数
    /// </summary>
    /// <param name="_url"></param>
    /// <param name="_timeout"></param>
    /// <param name="_progress"></param>
    /// <returns></returns>
    public static Promise<LancherLoadData> Load(string _url, float _timeout, LancherProgressHolder _progress)
    {
        var promise = new Promise<LancherLoadData>();

        LancherCoroutine.instance.StartCoroutine(Load(_url, promise, _timeout, _progress));

        return promise;
    }

    /// <summary>
    /// 修复WWW isDone的BUG
    /// </summary>
    /// <param name="www"></param>
    /// <returns></returns>
    public static bool IsWWWDone(UnityWebRequest www)
    {
        return www.isDone || www.downloadProgress >= 1f;
    }

    /// <summary>
    /// 想要实现一个能支持Timeout的加载函数 并且能不断反馈的进度
    /// </summary>
    /// <param name="_url"></param>
    /// <param name="_promise"></param>
    /// <param name="_timeout"></param>
    /// <param name="_progress"></param>
    /// <returns></returns>
    private static IEnumerator Load(string _url, Promise<LancherLoadData> _promise, float _timeout, LancherProgressHolder _progress)
    {
        var www = UnityWebRequest.Get(_url);

        bool failed = false;

        float loadtime = Time.time;

        www.Send();

        while (!IsWWWDone(www))
        {
            if (loadtime - Time.time > _timeout)
            {
                failed = true;
                //打断加载yield
                yield break;
            }
            if (_progress != null)
                _progress.progress = www.downloadProgress;

            yield return null;
        }

        if (_progress != null)
            // 只要坚持走到这里 都设置进度为1
            _progress.progress = 1;

        if (failed || !string.IsNullOrEmpty(www.error))
        {
            if (failed)
            {
                _promise.Reject(new System.Exception("加载:" + _url + "出错!" + "加载超时"));
            }
            else
            {
                _promise.Reject(new System.Exception("加载:" + _url + "出错!" + www.error));
            }

            www.Dispose();
        }
        else
        {
            _promise.Resolve(new LancherLoadData() { url = www.url, bytes = www.downloadHandler.data });

            www.Dispose();
        }
    }

    /// <summary>
    /// 支持加载进度
    /// 超时检查
    /// 多重加载
    /// </summary>
    /// <param name="_dllNameArray"></param>
    /// <param name="_timeout"></param>
    /// <param name="_holders"></param>
    /// <returns></returns>
    public static IPromise<LancherLoadData[]> LoadArray(string[] _dllNameArray, float _timeout, LancherProgressHolder[] _holders)
    {
        var promiseArray = new Promise<LancherLoadData>[_dllNameArray.Length];

        for (int i = 0; i < _dllNameArray.Length; i++)
        {
            promiseArray[i] = Load(_dllNameArray[i], _timeout, _holders[i]);
        }
        return Promise<LancherLoadData>.All(promiseArray);
    }
}

/// <summary>
/// 加载的内容
/// </summary>
public class LancherLoadData
{
    /// <summary>
    /// 网址
    /// </summary>
    public string url;

    /// <summary>
    /// 内容
    /// </summary>
    public byte[] bytes;
}