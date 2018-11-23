using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// manifest 引用关系数据管理
/// </summary>
public class ManifestTools
{

    /// <summary>
    /// 存储Manifest 文件信息
    /// </summary>
    /// <param name="manifest"></param>
    public static void OnSaveManifest(string url, AssetBundleManifest manifest)
    {
        if (manifest != null)
        {
            Dictionary<string, List<string>> dependencies = new Dictionary<string, List<string>>();

            string[] str = manifest.GetAllAssetBundles();

            int count = str.Length;
            for (int i = 0; i < count; i++)
            {
                if (string.IsNullOrEmpty(str[i]))
                    continue;

                string[] allDependencies = manifest.GetAllDependencies(str[i]);
                List<string> list = new List<string>();
                if (allDependencies != null && allDependencies.Length > 0)
                {
                    //获得数据，然后存储
                    list.AddRange(allDependencies);
                    if (string.IsNullOrEmpty(list[0]))
                        list.RemoveAt(0);
                }

                list.Reverse();
                dependencies[str[i]] = list;
            }

            OnSaveManifest(url, dependencies);
        }
    }

    /// <summary>
    /// 开始存储manifest 文件数据信息
    /// </summary>
    /// <param name="dic"></param>
    public static void OnSaveManifest(string url, Dictionary<string, List<string>> dic)
    {
        if (dic == null || dic.Count == 0)
            return;

        url = string.Format("{0}.txt", url);

        //Dictionary<string, Table_Res_Manifest> resDic = OnReadLocalManifest(url);


        //OnManifestCompare(url, resDic, dic);

        Dictionary<string, Table_Res_Manifest> newResManifest = OnGetManifestInfo(dic);
        if (newResManifest == null || newResManifest.Count == 0)
            return;

        OnSaveManifestInfo(url, newResManifest);
    }

    #region 读取本地 Manifest 文件数据信息

    /// <summary>
    /// 读取本地的 存储的manifest数据信息
    /// </summary>
    /// <param name="url"></param>
    static Dictionary<string, Table_Res_Manifest> OnReadLocalManifest(string url)
    {

        if (!File.Exists(url))
        {
            //如果文件不存在
            return new Dictionary<string, Table_Res_Manifest>();
        }

        byte[] data;
        using (Stream s = File.OpenRead(url))
        {
            //读取数据信息
            data = new byte[s.Length];
            s.Read(data, 0, (int)s.Length);
            s.Dispose();
        }

        if (data == null || data.Length == 0)
            return null;

        string text = BytesToString(data);

        Dictionary<string, Table_Res_Manifest> manifest; 
        TextRead(text, out manifest);

        return manifest;
    }

    /// <summary>
    /// Text 文本数据解析
    /// </summary>
    /// <param name="text"></param>
    static void TextRead(string text, out Dictionary<string, Table_Res_Manifest> manifest)
    {
        manifest = new Dictionary<string, Table_Res_Manifest>();
        string[] array2 = text.Split(new char[]
        {
                '\n'
        });

        int count = array2.Length;
        for (int i = 0; i < count; i++)
        {
            if (string.IsNullOrEmpty(array2[i]))
                continue;

            object obj = JsonUtility.FromJson(array2[i], typeof(Table_Res_Manifest));
            if (obj == null)
            {
                Debug.LogError("Table Create Failure~ ");
                continue;
            }

            Table_Res_Manifest tc = (Table_Res_Manifest)obj;
            if (tc == null)
                continue;

            manifest[tc.res] = tc;
        }
    }



    #endregion

    #region 数据对比存储

    ///// <summary>
    ///// manifest 文件数据进行对比
    ///// </summary>
    ///// <param name="resDic"></param>
    ///// <param name="dic"></param>
    //static void OnManifestCompare(string url, Dictionary<string, Table_Res_Manifest> resDic, Dictionary<string, List<string>> dic)
    //{

    //    Dictionary<string, Table_Res_Manifest> newResManifest = OnGetManifestInfo(dic);
    //    if (newResManifest == null || newResManifest.Count == 0)
    //        return;

    //    foreach (var item in newResManifest)
    //    {
    //        resDic[item.Key] = item.Value;
    //    }

    //    OnSaveManifestInfo(url, resDic);
    //}

    /// <summary>
    /// 获取引用关系，关联数据
    /// </summary>
    /// <param name="dic"></param>
    /// <returns></returns>
    static Dictionary<string, Table_Res_Manifest> OnGetManifestInfo(Dictionary<string, List<string>> dic)
    {

        if (dic == null || dic.Count == 0)
            return null;

        Dictionary<string, Table_Res_Manifest> tableDic = new Dictionary<string, Table_Res_Manifest>();

        foreach (var item in dic)
        {
            Table_Res_Manifest info = new Table_Res_Manifest();
            info.res = item.Key;

            List<string> list = item.Value;
            int count = list.Count;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                if (string.IsNullOrEmpty(list[i]))
                    continue;

                if (i > 0 && i <= list.Count - 1)
                    sb.Append(";");
                sb.Append(list[i]);
            }

            info.dependencies = sb.ToString();
            tableDic[item.Key] = info;
        }

        return tableDic;
    }

    #endregion

    #region 保存配置信息数据

    /// <summary>
    /// 保存配置表信息数据
    /// </summary>
    /// <param name="url"></param>
    /// <param name="resDic"></param>
    static void OnSaveManifestInfo(string url, Dictionary<string, Table_Res_Manifest> resDic)
    {
        if (string.IsNullOrEmpty(url) || resDic == null || resDic.Count == 0)
            return;

        StringBuilder sb = new StringBuilder();
        foreach (var item in resDic)
        {
            sb.Append(JsonUtility.ToJson(item.Value));
            sb.Append("\n");
        }

        //判断文件是否存在，如果存在那么删除（因为有可能为老版本文件）
        if (File.Exists(url))
            File.Delete(url);

        //写文件
        using (FileStream fs = File.Create(url))
        {
            //fs.Write(Data, 0, Data.Length);
            byte[] data = StringToBytes(sb.ToString());
            fs.Write(data, 0, data.Length);
            fs.Dispose();
        }

        Debug.LogError("  文件保存完成  ");
    }

    #endregion


    /// <summary>
    /// byte[] 转换为string
    /// </summary>
    /// <param name="b"></param>
    /// <returns></returns>
    public static string BytesToString(byte[] b)
    {
        if (b == null || b.Length == 0)
            return null;

        return Encoding.UTF8.GetString(b);
    }

    /// <summary>
    /// string 转换成 byte[]
    /// </summary>
    /// <param name="str">需要转换的string数据</param>
    /// <returns></returns>
    public static byte[] StringToBytes(string str)
    {
        if (string.IsNullOrEmpty(str))
            return null;

        return Encoding.UTF8.GetBytes(str);
    }
}

/// <summary>
/// 资源引用关系配置数据信息
/// </summary>
public class Table_Res_Manifest
{
    public string res;
    public string dependencies;
}
