using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using ZEffect;
using System.Diagnostics;
using System.Threading;

/// <summary>
/// 游戏的一些简单工具
/// </summary>
public class GameTools : Editor
{
    /// <summary>
    /// 清理缓存数据信息
    /// </summary>
    [MenuItem("Tools/Clear Cache", priority = 1)]
    public static void OnClearCache()
    {
        string path = string.Format("{0}/{1}", Application.persistentDataPath, "vercache");

        if (Directory.Exists(path))
            Directory.Delete(path, true);

        path = string.Format("{0}/{1}", Application.temporaryCachePath, "vercache");
        if (Directory.Exists(path))
            Directory.Delete(path, true);

        PlayerPrefs.DeleteAll();

        Debug.Log("Cache cleared!!");
    }

	/// <summary>
	/// 打开缓存目录
	/// </summary>
	[MenuItem("Tools/Open Cache Dir", priority = 1)]
	public static void OnOpenCacheDir()
	{
		//string path = string.Format("{0}/{1}", Application.persistentDataPath, "vercache");
		string path = Application.persistentDataPath;
		//System.Diagnostics.Process.Start("explorer.exe", path);

		Debug.LogError (path);
		// 新开线程防止锁死
		Thread newThread = new Thread(new ParameterizedThreadStart(CmdOpenDirectory));
		newThread.Start(path);
	}

	/// <summary>
	/// 命令行打开 目录
	/// </summary>
	/// <param name="path">Path.</param>
	private static void CmdOpenDirectory(object path)
	{
		Process p = new Process();

		#if UNITY_EDITOR_WIN

		p.StartInfo.FileName = "cmd.exe";
		p.StartInfo.Arguments = "/c start " + path.ToString();

		#elif UNITY_EDITOR_OSX
		var str = path.ToString();
		int lastIndex = str.LastIndexOf("/");
		var shellPath = str.Substring(0, lastIndex) + "/Shell/";
		p.StartInfo.FileName = "bash";
		string shPath = shellPath + "openDir.sh";
		p.StartInfo.Arguments = shPath + " " + path.ToString();

		#endif

		//UnityEngine.Debug.Log(p.StartInfo.Arguments);
		p.StartInfo.UseShellExecute = false;
		p.StartInfo.RedirectStandardInput = true;
		p.StartInfo.RedirectStandardOutput = true;
		p.StartInfo.RedirectStandardError = true;
		p.StartInfo.CreateNoWindow = true;
		p.Start();
		p.WaitForExit();
		p.Close();
	}

    /// <summary>
    /// 创建 本地加载数据的配置文件
    /// </summary>
    [MenuItem("Tools/Create Local Load Res Config", priority = 2)]
    public static void OnCreateLocalLoadResConfig()
    {
        //获得当前所有的资源数据信息
        string[] str = AssetDatabase.GetAllAssetPaths();
        //string[] str = AssetDatabase.FindAssets("t:scene t:Prefab t:Material t:font t:AudioClip", null);

        string configPath = "Assets/StreamingAssets/Resconfig.txt";
        if (File.Exists(configPath))//已经有的文件要删除
        {
            File.Delete(configPath);
        }

        StringBuilder sb = new StringBuilder("1.0.0\n");

        for (int i = 0; i < str.Length; i++)
        {

            if (!(str[i].Contains("Resources/Res") || str[i].Contains("Resources/Config")))
                continue;

            string s = Path.GetExtension(str[i]);
            if (s.Equals(".cs") || s.Equals(".dll") || s.Equals(".xml") || s.Equals(".prefs") || string.IsNullOrEmpty(s))
                continue;

            sb.Append(str[i].Replace("Assets/Resources/", "").Replace("Assets/Built-in/Resources/", "").Replace('\\', '/'));
            sb.Append("\n");
        }
        byte[] b = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
        if (!Directory.Exists(Path.GetDirectoryName(configPath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(configPath));
        }
        using (var s = File.Create(configPath))
        {
            s.Write(b, 0, b.Length);
            s.Dispose();
        }
        Debug.LogError("本地配置文件生成完毕！！！");
    }

    /// <summary>
    /// 创建 本地加载数据的配置文件
    /// </summary>
    [MenuItem("Tools/Get ALL Res Config")]
    public static void OnGetAllResConfig()
    {
        //获得当前所有的资源数据信息
        string[] str = AssetDatabase.GetAllAssetPaths();
        //string[] str = AssetDatabase.FindAssets("t:scene t:Prefab t:Material t:font t:AudioClip", null);

        string configPath = "Assets/StreamingAssets/All_Resconfig.txt";
        if (File.Exists(configPath))//已经有的文件要删除
        {
            File.Delete(configPath);
        }

        StringBuilder sb = new StringBuilder("1.0.0\n");

        for (int i = 0; i < str.Length; i++)
        {

            if (!(str[i].Contains("Resources/Res") || str[i].Contains("Resources/Config")))
                continue;

            string s = Path.GetExtension(str[i]);
            //if (s.Equals(".cs") || s.Equals(".dll") || s.Equals(".asset") || s.Equals(".xml") || s.Equals(".prefs") || string.IsNullOrEmpty(s))
            //    continue;
            if (!s.Equals(".prefab") && !s.Equals(".mp3"))
                continue;

            sb.Append(str[i].Replace("Assets/Resources/", "").Replace("Assets/Built-in/Resources/", "").Replace('\\', '/'));
            sb.Append("\n");
        }
        byte[] b = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
        if (!Directory.Exists(Path.GetDirectoryName(configPath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(configPath));
        }
        using (var s = File.Create(configPath))
        {
            s.Write(b, 0, b.Length);
            s.Dispose();
        }
        Debug.LogError("本地配置文件生成完毕！！！");
    }

    /// <summary>
    /// 给选中的资源添加脚本数据
    /// </summary>
    [MenuItem("Tools/Add BecameVisiable Comp")]
    public static void OnAddBecameVisiable()
    {

        Object[] objs = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        for (int i = 0; i < objs.Length; i++)
        {
            var obj = objs[i];
            string path = AssetDatabase.GetAssetPath(obj);

            EditorUtility.DisplayProgressBar("添加检测 BecameVisiable 脚本", "添加 Component " + Path.GetFileName(path) + " 。。。" + i + " / " + objs.Length, 1f * i / objs.Length);

            if (string.IsNullOrEmpty(Path.GetExtension(path)) || Path.GetExtension(path) != ".prefab")
                continue;

            GameObject go = obj as GameObject;
            if (go != null)
            {
                Renderer renderer = go.GetComponentInChildren<Renderer>();
                if (renderer != null)
                {
                    if (renderer.GetComponent<BecameVisiable>() == null)
                    {
                        renderer.gameObject.AddComponent<BecameVisiable>();
                    }
                }
            }


            Debug.LogError("fdddddd " + path + "    fddd " + (obj as GameObject) + " iiii");
        }

        EditorUtility.ClearProgressBar();

        //刷新，存储数据
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();

    }

    /// <summary>
    /// 给选中的资源添加脚本数据
    /// </summary>
    [MenuItem("Tools/Get All Selected Prefab")]
    public static void OnGetAllSelectedPrefab()
    {

        Object[] objs = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < objs.Length; i++)
        {
            var obj = objs[i];
            string path = AssetDatabase.GetAssetPath(obj);

            EditorUtility.DisplayProgressBar("获取选中的所有Prefab", "Get Prefab " + Path.GetFileName(path) + " 。。。" + i + " / " + objs.Length, 1f * i / objs.Length);

            if (string.IsNullOrEmpty(Path.GetExtension(path)) || Path.GetExtension(path) != ".prefab")
                continue;

            sb.Append(path.Replace("Assets/Resources/", "") + "\n");

        }

        EditorUtility.ClearProgressBar();

        string configPath = "Assets/StreamingAssets/Selectd_Resconfig.txt";

        byte[] b = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
        if (!Directory.Exists(Path.GetDirectoryName(configPath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(configPath));
        }
        using (var s = File.Create(configPath))
        {
            s.Write(b, 0, b.Length);
            s.Dispose();
        }
    }

    [MenuItem("Tools/Replace Select Effect Script")]
    public static void OnRelpaceTSEffect()
    {
        Object[] objs = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        for (int i = 0; i < objs.Length; i++)
        {
            GameObject prefab = objs[i] as GameObject;
            if (prefab != null)
            {
                //TSEffect e = prefab.GetComponent<TSEffect>();
                //if(e != null)
                //{
                //    TSEffect e1 = prefab.GetComponent<TSEffect>();
                //    if (e1 == null)
                //        e1 = prefab.AddComponent<TSEffect>();

                //    e1.IsFollow = e.isFollow;
                //    e1.IsAutoDestory = e.isAutoDestory;
                //    e1.AutoDestoryTime = e.autoDestoryTime;
                //    e1.IsOnTargetPos = e.isOnTargetPos;
                //    e1.startMountPoint = e.mountPoint;
                //    e1.endMountPoint = e.mountPoint;
                //    e1.RelationEffectId = e.relationEffectId;
                //    e1.IsFly = e.isFly;
                //    e1.FlySpeed = e.flySpeed;

                //    //移除老的脚本
                //    DestroyImmediate(e, true);

                //    AssetDatabase.SaveAssets();
                //}
            }

            //string path = AssetDatabase.GetAssetPath(obj);

            //EditorUtility.DisplayProgressBar("获取选中的所有Prefab", "Get Prefab " + Path.GetFileName(path) + " 。。。" + i + " / " + objs.Length, 1f * i / objs.Length);

            //if (string.IsNullOrEmpty(Path.GetExtension(path)) || Path.GetExtension(path) != ".prefab")
            //    continue;

            //sb.Append(path.Replace("Assets/Resources/", "") + "\n");

        }

        EditorUtility.ClearProgressBar();

    }
}
