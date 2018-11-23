using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

public class SVNUtils
{
    private static List<string> drives = new List<string>() { "c:", "d:", "e:", "f:" };
    private static string svnPath = @"\Program Files\TortoiseSVN\bin\";
    private static string svnProc = @"TortoiseProc.exe";
    private static string svnProcPath = "";

    [MenuItem("Assets/SVN更新全部 %&e", false, 2000)]
    public static void UpdateFromSVN()
    {
        //更新项目代码
        if (string.IsNullOrEmpty(svnProcPath))
            svnProcPath = GetSvnProcPath();

        var dir = new DirectoryInfo(Application.dataPath);

        var process = UpdatePath(dir.Parent.FullName);

        process.WaitForExit();

        if (process.HasExited)
        {
            var path = "Assets/Resources";

            var resourcePath = SymbolicLink.GetTarget(Application.dataPath + "/Resources");

            path = ReplacePath(path, resourcePath);

            //更新Resource
            var dir2 = new DirectoryInfo(path);

            var process2 = UpdatePath(dir2.FullName);

            process2.WaitForExit();

            if (process2.HasExited)
            {
                //var path2 = "Assets/Resources/Config";

                var resourcePath2 = SymbolicLink.GetTarget(Application.dataPath + "/Resources/Config");

                //path2 = ReplacePath(path2, resourcePath2);

                //UnityEngine.Debug.Log(path2);

                //更新Resource
                var dir3 = new DirectoryInfo(resourcePath2);

                UpdatePath(dir3.FullName);
            }
        }
    }

    private static Process UpdatePath(string path)
    {
        if (string.IsNullOrEmpty(svnProcPath))
            svnProcPath = GetSvnProcPath();

        var path2 = path.Replace('/', '\\');
        var para2 = "/command:update /path:\"" + path2 + "\" /closeonend:0";
        return Process.Start(svnProcPath, para2);
    }

    [MenuItem("Assets/SVN更新当前选择", false, 2000)]
    public static void UpdateLocalSVN()
    {
        if (Selection.activeObject != null)
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());

            var resourcePath = SymbolicLink.GetTarget(Application.dataPath + "/Resources");

            path = ReplacePath(path, resourcePath);

            if (path.Length > 0)
            {
                UpdatePath(path);
            }
        }
    }

    [MenuItem("Assets/SVN提交当前选择关联 %&c", false, 2000)]
    public static void CommitToSVNSpcialSelect()
    {
        if (string.IsNullOrEmpty(svnProcPath))
        {
            svnProcPath = GetSvnProcPath();
        }

        var resourcePath = SymbolicLink.GetTarget(Application.dataPath + "/Resources");

        //首先需要过滤只提交Assets 
        var selectionObjs = FilterAssetsInProject(Selection.objects);

        UnityEngine.Debug.Log(Selection.objects.Length + " " + Selection.instanceIDs.Length + "  " + selectionObjs.Length);

        var selectionPaths = new List<string>();

        var dependencies = new List<string>();

        for (int i = 0; i < selectionObjs.Length; i++)
        {
            var path = AssetDatabase.GetAssetPath(selectionObjs[i].GetInstanceID());

            var dependenciest = AssetDatabase.GetDependencies(path);

            dependencies.AddRange(dependenciest);
        }

        for (int i = dependencies.Count - 1; i >= 0; i--)
        {
            dependencies.Add(dependencies[i] + ".meta");
        }

        string cpath = "";

        for (int i = 0; i < dependencies.Count; i++)
        {
            cpath += ReplacePath((dependencies[i]) + (i != dependencies.Count - 1 ? "*" : ""), resourcePath);
        }

        var param = "/command:commit /path:\"" + cpath;

        Process.Start(svnProcPath, param);
    }

    /// <summary>
    /// 过滤资源
    /// </summary>
    /// <param name="objects"></param>
    /// <returns></returns>
    private static UnityEngine.Object[] FilterAssetsInProject(UnityEngine.Object[] objects)
    {
        var list = new List<UnityEngine.Object>();

        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] is GameObject)
            {
                var go = objects[i] as GameObject;

                if (!go.scene.isLoaded)
                {
                    list.Add(go);
                }
            }
            else
            {
                list.Add(objects[i]);
            }
        }
        return list.ToArray();
    }

    /// <summary>
    /// 变更路径到映射路径
    /// </summary>
    /// <param name="path"></param>
    /// <param name="resourceBasePath"></param>
    /// <returns></returns>
    public static string ReplacePath(string path, string resourceBasePath)
    {
        path = path.Replace('/', '\\');

        if (path.Contains("Assets\\Resources"))
        {
            path = path.Replace("Assets\\Resources", resourceBasePath);
        }
        return path;
    }
    /// <summary>
    /// 变更路径为SVN的合法路径
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string ReplacePath(string path)
    {
        path = path.Replace('/', '\\');

        if (path.Contains("Assets\\Resources"))
        {
            var mnn = SymbolicLink.GetRealPath(Application.dataPath + "/Resources/Res.meta");
            var mm = SymbolicLink.GetTarget(Application.dataPath + "/Resources");

            UnityEngine.Debug.Log(mm);
        }
        return path;
    }

    [MenuItem("Assets/SVN提交 %&r", false, 2000)]
    public static void CommitToSVN()
    {
        if (string.IsNullOrEmpty(svnProcPath))
            svnProcPath = GetSvnProcPath();
        var path = Application.dataPath.Replace('/', '\\');
        var para = "/command:commit /path:\"" + path + "\"";
        Process.Start(svnProcPath, para);
    }



    //防止被滥用
    //[MenuItem("Assets/回滚本地修改（更新时看到红色字请点我！） %&t", false, 2000)]
    //public static void RevertFromSVN()
    //{
    //	if (string.IsNullOrEmpty(svnProcPath))
    //		svnProcPath = GetSvnProcPath();
    //	var path = Application.dataPath.Replace('/', '\\');
    //	var para = "/command:revert /path:\"" + path + "\"";
    //	System.Diagnostics.Process.Start(svnProcPath, para);
    //}

    //[MenuItem("Assets/SVN更新策划数据 %&i", false, 2000)]
    //public static void UpdateDataFromSVN()
    //{
    //    if (string.IsNullOrEmpty(svnProcPath))
    //        svnProcPath = GetSvnProcPath();
    //    var path = (Application.dataPath + "/StreamingAssets/Config").Replace('/', '\\');
    //    var para = "/command:update /path:\"" + path + "\" /closeonend:0";
    //    Process.Start(svnProcPath, para);
    //}

    //[MenuItem("Assets/SVN添加 %&u", false, 2000)]
    //public static void AddToSVN()
    //{
    //    if (string.IsNullOrEmpty(svnProcPath))
    //        svnProcPath = GetSvnProcPath();
    //    var path = Application.dataPath.Replace('/', '\\');
    //    var para = "/command:add /path:\"" + path + "\"";
    //    Process.Start(svnProcPath, para);
    //}

    [MenuItem("Assets/清理SVN %&y", false, 2000)]
    public static void CleanUpFromSVN()
    {
        if (string.IsNullOrEmpty(svnProcPath))
            svnProcPath = GetSvnProcPath();
        var path = Application.dataPath.Replace('/', '\\');
        var para = "/command:cleanup /path:\"" + path + "\"";
        Process.Start(svnProcPath, para);
    }

    private static string GetSvnProcPath()
    {

        foreach (var item in drives)
        {
            var path = string.Concat(item, svnPath, svnProc);
            if (File.Exists(path))
                return path;
        }

        return svnProc;

        return EditorUtility.OpenFilePanel("Select TortoiseProc.exe", "c:\\", "exe");
    }

    ///<summary>
    ///
    ///</summary>
    //[MenuItem("Assets/GetVersion")]
    //public static void GenerateSVN()
    //{
    //	RunCmd2("subwcrev.exe", Directory.GetCurrentDirectory() + " " + "/Assets/Editor/CodeVersion.cs.template" + " " + "/Assets/Editor/CodeVersion.cs");
    //}


    /// <summary>
    /// 运行cmd命令
    /// 会显示命令窗口
    /// </summary>
    /// <param name="cmdExe">指定应用程序的完整路径</param>
    /// <param name="cmdStr">执行命令行参数</param>
    static bool RunCmd(string cmdExe, string cmdStr)
    {
        bool result = false;
        try
        {
            using (Process myPro = new Process())
            {
                //指定启动进程是调用的应用程序和命令行参数
                ProcessStartInfo psi = new ProcessStartInfo(cmdExe, cmdStr);
                myPro.StartInfo = psi;
                myPro.Start();
                myPro.WaitForExit();
                result = true;
            }
        }
        catch
        {

        }
        return result;
    }

    /// <summary>
    /// 运行cmd命令
    /// 不显示命令窗口
    /// </summary>
    /// <param name="cmdExe">指定应用程序的完整路径</param>
    /// <param name="cmdStr">执行命令行参数</param>
    static bool RunCmd2(string cmdExe, string cmdStr)
    {
        bool result = false;
        try
        {
            using (Process myPro = new Process())
            {
                myPro.StartInfo.FileName = "cmd.exe";
                myPro.StartInfo.UseShellExecute = false;
                myPro.StartInfo.RedirectStandardInput = true;
                myPro.StartInfo.RedirectStandardOutput = true;
                myPro.StartInfo.RedirectStandardError = true;
                myPro.StartInfo.CreateNoWindow = true;
                myPro.Start();
                //如果调用程序路径中有空格时，cmd命令执行失败，可以用双引号括起来 ，在这里两个引号表示一个引号（转义）
                string str = string.Format(@"{0} {1}", cmdExe, cmdStr);

                myPro.StandardInput.WriteLine(str);
                myPro.StandardInput.AutoFlush = true;
                myPro.WaitForExit();

                result = true;
            }
        }
        catch
        {

        }
        return result;
    }
}