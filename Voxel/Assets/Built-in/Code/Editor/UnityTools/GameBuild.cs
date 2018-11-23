using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System;
using UnityEngine.Rendering;

/// <summary>
/// 发布客户端版本数据
/// </summary>
public class GameBuild
{

    #region 发布游戏版本

    /// <summary>
    /// 发布游戏版本
    /// </summary>
    public static void BuildGame()
    {
        commandList = GetCommandLineArgs().Split(","[0]);
        switch (commandList[0])
        {
            case "PC":
                BuildPC();
                break;
            case "Android":
                BuildAndroid();
                break;
            case "IOS":
                BuildIOS();
                break;
            case "WP8":
                break;
        }
    }

    #endregion


    #region 发布PC版本

    /// <summary>
    /// 发布PC版本数据
    /// </summary>
    [MenuItem("Tools/Build PC")]
    public static void BuildPC()
    {
        //切换版本平台
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneWindows);
        GetBuildInfo();
        string str = commandList[1];
        //设置在iOS和Andriod平台共享的应用程序包版本。
        PlayerSettings.bundleVersion = "1.0.0";
        //获得本地文件存储数据信息
        GetAllFiles("1.0.0");

        //发布版本
        //PlayerSettings.productName = "凯瑞利亚" + str;
        //if (string.IsNullOrEmpty(str) || str.Contains("DEV"))
        //{
        //    PlayerSettings.productName = "凯瑞利亚DEV";
        //}
        //else
        //{
        //    PlayerSettings.productName = "凯瑞利亚" + str;
        //}

        if (commandList[1].Contains("NET")
                || commandList[1].Contains("OnLine")
                || commandList[1].Contains("CLOUD")
                || commandList[1].Contains("cloud"))
        {
            PlayerSettings.productName = "凯瑞利亚";
        }
        else
        {
            PlayerSettings.productName = string.Format("凯瑞利亚_{0}_{1}", commandList[1], commandList[5]);
        }

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, GetScriptingDefineSymbolsForGroup());

        //更新设置修改，包名
        SetApplicationIdentifier(commandList[7]);
        //设置出包版本号
        SetApplicationVersion(commandList[8], commandList[9]);

        //string folder = "../build";
        string folder = Directory.GetParent(Directory.GetCurrentDirectory()).FullName + "/build";
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        options = BuildOptions.Development;
        options = commandList[2].Contains("Debug") ? BuildOptions.Development : BuildOptions.None;

        EditorUserBuildSettings.development = commandList[2].Contains("Debug") ? true : false;

        var errorString = BuildPipeline.BuildPlayer(GetScenes(), folder + "/client.exe", BuildTarget.StandaloneWindows, options);

        if (!string.IsNullOrEmpty(errorString))
        {
            throw new Exception("Build PC Failed!" + errorString);
        }
    }



    #endregion

    #region 发布Android版本

    /// <summary>
    /// 发布Android版本数据
    /// </summary>
    public static void BuildAndroid()
    {

        //切换版本平台
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
        GetBuildInfo();

        //设置数据配置
        //EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ETC;
        //PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new GraphicsDeviceType[] { GraphicsDeviceType.OpenGL2, GraphicsDeviceType.OpenGLES2, GraphicsDeviceType.OpenGLES3 });
        //PlayerSettings.strippingLevel = StrippingLevel.StripByteCode;
        PlayerSettings.strippingLevel = StrippingLevel.Disabled;

        //设置在iOS和Andriod平台共享的应用程序包版本。
        PlayerSettings.bundleVersion = "1.0.0";
        PlayerSettings.Android.bundleVersionCode = 100;//android 上的versioncode

        //获得本地文件存储数据信息
        GetAllFiles("1.0.0", commandList[3]);


        if (commandList[2].Contains("Debug"))
        {
            EditorUserBuildSettings.development = true;
            PlayerSettings.productName = string.Format("凯瑞利亚_{0}_{1}", commandList[1], commandList[5]);
            options = BuildOptions.Development;
        }
        else
        {
            EditorUserBuildSettings.development = false;
            options = BuildOptions.None;

            if (commandList[1].Contains("NET")
                || commandList[1].Contains("OnLine")
                || commandList[1].Contains("CLOUD")
                || commandList[1].Contains("cloud"))
            {
                PlayerSettings.productName = "凯瑞利亚";
            }
            else
            {
                PlayerSettings.productName = string.Format("凯瑞利亚_{0}_{1}", commandList[1], commandList[5]);
            }

        }

        //发布版本
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, GetScriptingDefineSymbolsForGroup());

        //更新设置修改，包名
        SetApplicationIdentifier(commandList[7]);
        //设置出包版本号
        SetApplicationVersion(commandList[8], commandList[9]);

        //string folder = "../build";
        string folder = Directory.GetParent(Directory.GetCurrentDirectory()).FullName + "/build";
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        var errorString = BuildPipeline.BuildPlayer(GetScenes(), folder + "/krly.apk", BuildTarget.Android, options);

        if (!string.IsNullOrEmpty(errorString))
        {
            throw new Exception("Build Android Failed!" + errorString);
        }
    }

    #endregion


    #region 发布IOS版本

    /// <summary>
    /// 发布Ios版本数据
    /// </summary>
    [MenuItem("Tools/Build IOS")]
    public static void BuildIOS()
    {
        //切换版本平台
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iOS);
        GetBuildInfo();

        //设置数据配置
        //EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ETC;
        //PlayerSettings.SetGraphicsAPIs(BuildTarget.iOS, new GraphicsDeviceType[] { GraphicsDeviceType.OpenGL2, GraphicsDeviceType.OpenGLES2, GraphicsDeviceType.OpenGLES3 });
        //PlayerSettings.strippingLevel = StrippingLevel.StripByteCode;
        //PlayerSettings.SetGraphicsAPIs(BuildTarget.iOS, new GraphicsDeviceType[] { SystemInfo.graphicsDeviceType });//new GraphicsDeviceType[] { GraphicsDeviceType.Metal, GraphicsDeviceType.OpenGLES2 });
        PlayerSettings.strippingLevel = StrippingLevel.Disabled;

        //设置在iOS和Andriod平台共享的应用程序包版本。
        PlayerSettings.bundleVersion = "1.0.0";
        PlayerSettings.Android.bundleVersionCode = 100;//android 上的versioncode

        //获得本地文件存储数据信息
        //GetAllFiles("1.0.0", commandList[3]);



        if (commandList[2].Contains("Debug"))
        {
            PlayerSettings.productName = string.Format("凯瑞利亚_{0}_{1}", commandList[1], commandList[5]);
            options = BuildOptions.Development;
            EditorUserBuildSettings.development = true;
            PlayerSettings.iOS.scriptCallOptimization = ScriptCallOptimizationLevel.SlowAndSafe;
        }
        else
        {
            options = BuildOptions.None;
            EditorUserBuildSettings.development = false;
            PlayerSettings.iOS.scriptCallOptimization = ScriptCallOptimizationLevel.SlowAndSafe;

            if (commandList[1].Contains("NET")
                || commandList[1].Contains("OnLine")
                || commandList[1].Contains("CLOUD")
                || commandList[1].Contains("cloud"))
            {
                PlayerSettings.productName = "凯瑞利亚";
            }
            else
            {
                PlayerSettings.productName = string.Format("凯瑞利亚_{0}_{1}", commandList[1], commandList[5]);
            }

        }

        //发布版本
        PlayerSettings.iOS.targetOSVersion = iOSTargetOSVersion.iOS_8_0;
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, GetScriptingDefineSymbolsForGroup());

        //更新设置修改，包名
        SetApplicationIdentifier(commandList[7]);

        //设置出包版本号
        SetApplicationVersion(commandList[8], commandList[9]);

        //string folder = "../build";
        string folder = Directory.GetParent(Directory.GetCurrentDirectory()).FullName + "/build";
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        var errorString = BuildPipeline.BuildPlayer(GetScenes(), folder + "/XCodeBuild", BuildTarget.iOS, options);

        if (!string.IsNullOrEmpty(errorString))
        {
            throw new Exception("Build IOS Failed!" + errorString);
        }
    }

    #endregion

    #region 设置数据信息

    //命令传入参数保存（平台, 版本（运行平台,DEV,QA）, 是否可调试, 版本号，编译条件（发行平台）, "脚本宏定义", "语言版本", "Version发布版本 NORMAL平时开发测试，AUDIT（IOS TestFlight 测试）", "Version", "PC Build IOS Build, Android BundleVersionCode"）
    static string[] commandList = new string[10];
    //资源打包时的操作处理（移动平台）
    static BuildOptions options = BuildOptions.None;

    /// <summary>
    /// 获得Build数据信息
    /// </summary>
    static void GetBuildInfo()
    {

        //创建生成本地资源数据配置
        GameTools.OnCreateLocalLoadResConfig();

        ////添加版本文件数据
        //VerInfoTools.GetVerInfo();

        ////添加空场景数据
        //EditorBuildSettingsScene scene = new EditorBuildSettingsScene("Assets/emptyscene.unity", true);
        //List<EditorBuildSettingsScene> temp = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        //if (!temp.Contains(scene))
        //{
        //    temp.Add(scene);
        //    EditorBuildSettings.scenes = temp.ToArray();
        //}


#if UNITY_ANDROID || UNITY_IOS

        //启用垂直同步数据(移动平台开启垂直同步)
        QualitySettings.vSyncCount = 0;
#endif

        //commandList = GetCommandLineArgs().Split(","[0]);

    }

    /// <summary>
    /// 获取命令行信息数据
    /// </summary>
    /// <returns></returns>
    static string GetCommandLineArgs()
    {
        foreach (string arg in System.Environment.GetCommandLineArgs())
        {
            Debug.LogError(arg + "    00000000000000000000000000000000000000000000000000  ");
            if (arg.StartsWith("Ver"))
            {
                Debug.Log("获取命令行参数：" + arg.Split("="[0])[1]);
                return arg.Split("="[0])[1];
            }
        }
        return "";
    }

    /// <summary>
    /// 更新设置包名
    /// </summary>
    static void SetApplicationIdentifier(string str)
    {
        if (str == "AUDIT")
        {
            //ios TestFlight 修改包名
            PlayerSettings.applicationIdentifier = GameBundleName.TEST_FLIGTH_IDENTIFIER;
        }
        else
        {
            //重新设置包名
            PlayerSettings.applicationIdentifier = GameBundleName.INTERNAL_TEST_IDENTIFIER;
        }

        //更改了包名和宏编译，尝试刷新配置数据
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 设置发布版本号
    /// </summary>
    /// <param name="str"></param>
    [MenuItem("Tools/Test")]
    static void SetApplicationVersion(string bundleVersion, string buildNumber)
    {
        if (!string.IsNullOrEmpty(bundleVersion))
        {
            PlayerSettings.bundleVersion = bundleVersion;
        }

        if (!string.IsNullOrEmpty(buildNumber))
        {
            PlayerSettings.macOS.buildNumber = buildNumber;
            PlayerSettings.iOS.buildNumber = buildNumber;
            PlayerSettings.Android.bundleVersionCode = Convert.ToInt32((buildNumber.Split('.')[0]).ToString());
        }
    }

    /// <summary>
    /// 获得本地文件配置信息数据
    /// </summary>
    /// <param name="bundleversion"></param>
    /// <param name="buildVer"></param>
    static void GetAllFiles(string bundleversion, string buildVer = "0")
    {
#if DEV
        string[] arr = AssetDatabase.GetAllAssetPaths();
#else
        string[] arr = AssetDatabase.FindAssets("t:scene t:Prefab t:Material t:font t:AudioClip", null);
#endif
        string configPath = "Assets/StreamingAssets/Resconfig.txt";
        if (File.Exists(configPath))//已经有的文件要删除
        {
            File.Delete(configPath);
        }

        //        StringBuilder sb = new StringBuilder();
        //        sb.AppendFormat("{0}\n{1}\n", bundleversion, buildVer);

        //        for (int i = 0; i < arr.Length; i++)
        //        {
        //#if DEV
        //			string s = arr[i];
        //#else
        //            string s = AssetDatabase.GUIDToAssetPath(arr[i]);
        //#endif
        //            sb.AppendFormat("{0}\n", s.Replace("Assets/Resources/Res/", "").Replace('\\', '/').Replace("Assets/Resources/Config", "Config"));
        //        }

        //        //数据存数起来
        //        byte[] b = Encoding.UTF8.GetBytes(sb.ToString());
        //        if (!Directory.Exists(Path.GetDirectoryName(configPath)))
        //            Directory.CreateDirectory(Path.GetDirectoryName(configPath));
        //        using (var s = File.Create(configPath))
        //        {
        //            s.Write(b, 0, b.Length);
        //        }
        //        Debug.Log("**********************************************************************************");
        //        Debug.Log(sb);
        //        Debug.Log("本地配置文件生成完毕！！！");

        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("{0}\n{1}\n", bundleversion, buildVer);

        for (int i = 0; i < arr.Length; i++)
        {

            if (!(arr[i].Contains("Resources/Res") || arr[i].Contains("Resources/Config")))
                continue;

            string s = Path.GetExtension(arr[i]);
            if (s.Equals(".cs") || s.Equals(".dll") || s.Equals(".asset") || s.Equals(".xml") || s.Equals(".prefs") || string.IsNullOrEmpty(s))
                continue;

            sb.Append(arr[i].Replace("Assets/Resources/", "").Replace("Assets/Built-in/Resources/", "").Replace('\\', '/'));
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
    }

    /// <summary>
    /// 获得当前需要打包进入的场景信息数据
    /// </summary>
    /// <returns></returns>
    static string[] GetScenes()
    {
        string[] scenes = { "Assets/Main.unity" };
        return scenes;
    }

    /// <summary>
    /// 获取编译标签数据
    /// </summary>
    /// <returns></returns>
    static string GetScriptingDefineSymbolsForGroup()
    {
        if (commandList == null || commandList.Length == 0)
            return null;

        if (commandList.Length >= 5)
        {
            int count = commandList.Length;
            StringBuilder sb = new StringBuilder();
            for (int i = 4; i < count; i++)
            {
                if (string.IsNullOrEmpty(commandList[i]))
                    continue;

                if (commandList[i].Contains("cloud"))
                    sb.Append("CLOUD");
                else
                    sb.Append(commandList[i]);
                if (i < count - 1)
                    sb.Append(";");
            }

            return sb.ToString();
        }

        return null;
    }


    #endregion

    #region 版本文件信息数据

    /// <summary>
    /// 设置当前的版本文件信息数据
    /// </summary>
    class VerInfoTools
    {
        //根目录
        static string root = "./";

        /// <summary>
        /// 获得设置当前版本文件信息数据
        /// </summary>
        public static void GetVerInfo()
        {
            string temp = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(Application.streamingAssetsPath);
            InitVerAndPath();
            CheckVer();
            GetVer();
            Directory.SetCurrentDirectory(temp);
        }

        //文件版本管理
        static VerManager ver = null;
        static VerManager verNew = null;

        /// <summary>
        /// 初始化版本号和文件路径信息
        /// </summary>
        static void InitVerAndPath()
        {
            //读取文件分组版本信息
            ver = VerManager.Read(root);
            if (ver == null)
            {
                //如果当前路径下面没有此版本文件组信息
                //初始化一个版本文件组信息

                ver = new VerManager();
                //获得根目录下面的文件夹名称
                string[] groups = Directory.GetDirectories(root);
                foreach (string g in groups)
                {
                    //移除根目录
                    string path = g.Replace(root, "").ToLower();
                    if (path.IndexOf("path") == 0 || path.Contains(".svn"))
                        continue;

                    VerInfo info = new VerInfo(path);
                    if (ver.Groups.ContainsKey(path))
                        ver.Groups[path] = info;
                    else
                        ver.Groups.Add(path, info);
                }

                //设置文件版本号
                ver.Ver = 0;
            }
        }

        /// <summary>
        /// 检测文件版本信息
        /// </summary>
        static void CheckVer()
        {
            verNew = new VerManager();
            string[] groups = Directory.GetDirectories(root);

            //输出当前文件信息里面不包含的文件路径
            foreach (string g in groups)
            {
                //string path = g.Substring(2).ToLower();
                string path = g.Replace(root, "").ToLower();
                if (path.IndexOf("path") == 0 || path.Contains(".svn"))
                    continue;

                if (!ver.Groups.ContainsKey(path.Replace(root, "")))
                {
                    //如果文件数据里面不包含此文件组信息
                    Debug.LogError("目录未包含:" + path.Replace(root, "") + " 增加此数据 ");

                    ver.Groups.Add(path, new VerInfo(path));
                }
            }

            //标识出来各种操作的文件个数
            int delcount = 0;
            int updatecount = 0;
            int addcount = 0;

            foreach (var g in ver.Groups)
            {
                //存储获得文件组里所有文件的hash
                verNew.Groups[g.Key] = new VerInfo(g.Key);
                verNew.Groups[g.Key].GetHash();
                foreach (var f in g.Value.FileHash)
                {
                    //检测如果新的文件组hash表中没有 之前读取配置文件里面的文件hash,那么就认为此文件被删除掉了
                    if (!verNew.Groups[g.Key].FileHash.ContainsKey(f.Key))
                    {
                        Debug.LogError("文件被删除：" + g.Key + ":" + f.Key);
                        delcount++;
                    }
                    else
                    {
                        //如果文件的hash值不相同 更新文件hash值
                        string hash = verNew.Groups[g.Key].FileHash[f.Key];
                        string oldHash = g.Value.FileHash[f.Key];

                        if (!string.Equals(hash, oldHash))
                        {
                            Debug.LogError("文件更新：" + g.Key + ":" + f.Key);
                            updatecount++;
                        }
                    }
                }

                //循环遍历 新获得的文件hash表数据，检测添加的文件个数
                foreach (var f in verNew.Groups[g.Key].FileHash)
                {
                    //如果原hash表不包含此文件数据,那么次数据就是新添加的数据
                    if (!g.Value.FileHash.ContainsKey(f.Key))
                    {
                        Debug.LogError("文件增加：" + g.Key + ":" + f.Key);
                        addcount++;
                    }
                }
            }

            if (addcount == 0 && delcount == 0 && updatecount == 0)
            {
                verNew.Ver = ver.Ver;
                Debug.LogError("无变化 ver=" + verNew.Ver);
            }
            else
            {
                //改变文件版本号，如果有文件数据改变
                verNew.Ver = ver.Ver + 1;
                Debug.LogError("检查变化结果 add:" + addcount + " remove:" + delcount + " update:" + updatecount);
                Debug.LogError("版本号变为:" + verNew.Ver);
            }

        }

        //获取保存版本文件数据
        static void GetVer()
        {
            if (verNew == null)
            {
                Debug.LogError("先检查一下版本再生成");
                return;
            }
            if (verNew.Ver == ver.Ver)
            {
                Debug.LogError("版本无变化");
                return;
            }

            //保存数据
            verNew.SaveToPath(root);
            Debug.LogError("生成OK Ver:" + verNew.Ver);

            ver = verNew;
        }
    }

    /// <summary>
    /// 版本管理类
    /// </summary>
    class VerManager
    {

        //版本文件号
        int m_iVer;
        public int Ver
        {
            get { return m_iVer; }
            set { m_iVer = value; }
        }

        //版本文件组数据
        Dictionary<string, VerInfo> m_dGroups;
        public Dictionary<string, VerInfo> Groups
        {
            get
            {
                if (m_dGroups == null)
                    m_dGroups = new Dictionary<string, VerInfo>();
                return m_dGroups;
            }
            set { m_dGroups = value; }
        }

        /// <summary>
        /// 通过路径，存数数据
        /// </summary>
        public void SaveToPath(string path)
        {
            Dictionary<string, string> grouphash = new Dictionary<string, string>();
            foreach (var i in Groups.Values)
            {
                //存储文件Hash数据
                grouphash[i.Group] = i.SaveToPath(this.Ver, path);
            }

            //获取文件标头数据，文件版本号
            string outstr = string.Format("Ver:{0}\n", this.Ver);
            StringBuilder sb = new StringBuilder();
            foreach (var g in grouphash)
            {
                sb.Append(g.Key + "|" + g.Value + "|" + Groups[g.Key].FileHash.Count + "\n");
            }
            outstr = string.Format("{0}{1}", outstr, sb.ToString());

            File.WriteAllText(Path.Combine(path, "allver.ver.txt"), outstr, Encoding.UTF8);
        }

        /// <summary>
        /// 通过路径，读取版本文件数据
        /// </summary>
        /// <param name="path"></param>
        public static VerManager Read(string path)
        {
            string p = Path.Combine(path, "allver.ver.txt");
            if (!File.Exists(p))
                return null;

            //有文本数据
            string txt = File.ReadAllText(p, Encoding.UTF8);
            string[] lines = txt.Split(new string[] { "\n", "\r" }, System.StringSplitOptions.RemoveEmptyEntries);
            VerManager ver = new VerManager();
            foreach (string l in lines)
            {
                if (l.IndexOf("Ver:") == 0)
                {
                    //取得文件版本号
                    ver.Ver = int.Parse(l.Substring(4));
                }
                else
                {
                    //解析获得文件内容信息    组信息，hash,文件个数
                    string[] sp = l.Split('|');
                    VerInfo info = new VerInfo(sp[0]);
                    info.Read(ver.Ver, sp[1], int.Parse(sp[2]), path);

                    if (ver.Groups.ContainsKey(sp[0]))
                        ver.Groups[sp[0]] = info;
                    else
                        ver.Groups.Add(sp[0], info);
                }
            }

            return ver;
        }

        //输出内容数据
        public override string ToString()
        {
            int useful = 0;
            int filecount = 0;
            int filematch = 0;
            foreach (var i in Groups)
            {
                if (i.Value.Match > 0) useful++;
                filematch += i.Value.Match;
                filecount += i.Value.FileHash.Count;
            }
            return "ver=" + Ver + " group=(" + useful + "/" + Groups.Count + ") file=(" + filematch + "/" + filecount + ")";
        }
    }

    /// <summary>
    /// 版本文件数据
    /// </summary>
    class VerInfo
    {
        public VerInfo(string group)
        {
            this.Group = group;
        }

        //文件分组标识
        string m_sGroup;
        public string Group
        {
            get { return m_sGroup; }
            set { m_sGroup = value; }
        }


        int m_iMatch;
        public int Match
        {
            get { return m_iMatch; }
            set { m_iMatch = value; }
        }

        //计算哈希值
        SHA1CryptoServiceProvider m_cSha1;
        public SHA1CryptoServiceProvider Sha1
        {
            get
            {
                if (m_cSha1 == null)
                    m_cSha1 = new SHA1CryptoServiceProvider();
                return m_cSha1;
            }
            set { m_cSha1 = value; }
        }

        //文件Hash列表
        Dictionary<string, string> m_dFileHash;
        public Dictionary<string, string> FileHash
        {
            get
            {
                if (m_dFileHash == null)
                    m_dFileHash = new Dictionary<string, string>();
                return m_dFileHash;
            }
            set { m_dFileHash = value; }
        }

        /// <summary>
        /// 获得Hash
        /// </summary>
        public void GetHash()
        {
            if (!Directory.Exists(this.Group))
                return;
            string[] files = Directory.GetFiles(this.Group, "*.*", SearchOption.AllDirectories);
            foreach (var f in files)
            {
                if (f.IndexOf(".crc.txt") >= 0
                    || f.IndexOf(".meta") >= 0
                    || f.IndexOf(".db") >= 0
                    )
                    continue;
                GetHashOne(f);
            }
        }

        /// <summary>
        /// 获得文件唯一Hash值
        /// </summary>
        void GetHashOne(string filename)
        {
            using (Stream s = File.OpenRead(filename))
            {
                var hash = Sha1.ComputeHash(s);
                var shash = string.Format("{0}|{1}", Convert.ToBase64String(hash), s.Length);
                //去除文件组表头，只有文件名字信息
                filename = filename.Substring(Group.Length + 1);

                filename = filename.Replace('\\', '/');
                FileHash[filename] = shash;
            }
        }

        /// <summary>
        /// 存储版本文件路径
        /// </summary>
        /// <param name="ver"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public string SaveToPath(int ver, string path)
        {
            //存储文件标头信息
            string outstr = string.Format("Ver:{0}|FileCount:{1}\n", ver, this.FileHash.Count);

            StringBuilder sb = new StringBuilder();
            foreach (var f in FileHash)
            {
                //把文件名和文件的hash值添加到输出字符串里
                sb.Append(f.Key + "|" + f.Value + "\n");
            }

            outstr = string.Format("{0}{1}", outstr, sb.ToString());

            //获得输出文件
            string g = this.Group.Replace('/', '_');
            string outfile = Path.Combine(path, string.Format("{0}.ver.txt", g));

            //存储文件
            File.WriteAllText(outfile, outstr, Encoding.UTF8);

            //获取新的版本文件的hash值
            using (Stream s = File.OpenRead(outfile))
            {
                var hash = Sha1.ComputeHash(s);
                var shash = Convert.ToBase64String(hash);
                return shash;
            }
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="ver"></param>
        /// <param name="hash"></param>
        /// <param name="filecount"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool Read(int ver, string hash, int filecount, string path)
        {
            //获得文件存储路径
            string g = this.Group.Replace('/', '_');
            string file = Path.Combine(path, string.Format("{0}.ver.txt", g));

            //文件不存在
            if (!File.Exists(file))
                return false;

            //获取版本文件的hash值
            using (Stream s = File.OpenRead(file))
            {
                var chash = Sha1.ComputeHash(s);
                var shash = Convert.ToBase64String(chash);

                //文件Hash值不匹配
                if (!string.Equals(shash, hash))
                    return false;
            }

            //获得文件内容
            string txt = File.ReadAllText(file, Encoding.UTF8);

            //对文件内容进行剪切
            string[] lines = txt.Split(new string[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
            //循环遍历数据
            foreach (var l in lines)
            {
                if (l.IndexOf("Ver:") == 0)
                {
                    //如果是文件标头内容
                    var sp = l.Split(new string[] { "Ver:", "|FileCount:" }, StringSplitOptions.RemoveEmptyEntries);
                    int vercount = int.Parse(sp[0]);
                    int count = int.Parse(sp[1]);

                    //如果版本号不匹配
                    if (vercount != ver) return false;
                    //如果文件个数不匹配
                    if (count != filecount) return false;
                }
                else
                {
                    //如果是文件普通内容, 文件匹配那么更新存储数据
                    var sp = l.Split('|');
                    FileHash[sp[0]] = string.Format("{0}|{1}", sp[1], sp[2]);
                }
            }
            return true;
        }
    }

    #endregion
}

public class GameBundleName
{
    /// <summary>
    /// Test Flight 测试包名
    /// </summary>
    public const string TEST_FLIGTH_IDENTIFIER = "com.zeus.awesome.krly";

    /// <summary>
    /// 公司内部测试包名
    /// </summary>
    public const string INTERNAL_TEST_IDENTIFIER = "com.tianshen.shuguang.tgame";

}