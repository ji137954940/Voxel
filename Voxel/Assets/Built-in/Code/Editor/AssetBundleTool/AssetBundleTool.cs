using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Tgame.AssetBundle
{
    /// <summary>
    /// 资源进行 AssetBundle 打包创建
    /// </summary>
    public class AssetBundleTool : Editor
    {

        #region 资源分析处理

        /// <summary>
        /// 资源打包（全部资源）
        /// </summary>
        public static void BuildRes()
        {

            //注册日志接受信息
            Application.logMessageReceived += Command.LogMessageReceived;
            //获取命令行信息数据
            string platform = Command.GetCommandLineArgs();

            BuildTarget target = BuildTarget.StandaloneWindows;

            //设置平台数据
            if (platform.StartsWith("PC"))
            {
                target = BuildTarget.StandaloneWindows;
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);
            }
            else if (platform.StartsWith("Android"))
            {
                target = BuildTarget.Android;
                EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ETC;
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            }
            else if (platform.StartsWith("IOS"))
            {
                target = BuildTarget.iOS;
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
            }

            AssetDatabase.Refresh();

            //开始构建 AssetBundle
            BuildAssetBundle(target, 2, false, null);

        }

        /// <summary>
        /// 资源打包（从配置中读取）
        /// </summary>
        public static void BuildResFromConfig()
        {

            //注册日志接受信息
            Application.logMessageReceived += Command.LogMessageReceived;
            //获取命令行信息数据
            string platform = Command.GetCommandLineArgs();

            BuildTarget target = BuildTarget.StandaloneWindows;

            //设置平台数据
            if (platform.StartsWith("PC"))
            {
                target = BuildTarget.StandaloneWindows;
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);
            }
            else if (platform.StartsWith("Android"))
            {
                target = BuildTarget.Android;
                EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ETC;
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            }
            else if (platform.StartsWith("IOS"))
            {
                target = BuildTarget.iOS;
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
            }

            AssetDatabase.Refresh();

            //获取需要打包的资源数据
            string[] arr = GetNeedBuildAsset();

            //开始构建 AssetBundle
            BuildAssetBundle(target, 2, false, arr);

        }

        /// <summary>
        /// 获取AssetBundle输出目录
        /// </summary>
        /// <returns></returns>
        static string GetAssetBundleOutPath()
        {
            //获得打包资源根目录
            string path = Application.dataPath;

            string dataPath = string.Format("{0}Data/", path.Remove(path.LastIndexOf("Assets")));

            //如果目录不存在，那么创建
            if (!Directory.Exists(dataPath))
                Directory.CreateDirectory(dataPath);

            return dataPath;
        }

        /// <summary>
        /// Bundle PC
        /// </summary>
        [MenuItem("Tools/AssetBundle/打包PC资源")]
        static void BuildPC()
        {
            BuildTarget target = BuildTarget.StandaloneWindows;

            //string[] arr = new string[]
            //    {
            //        "Assets/Resources/Res/UI/PackerImg/BD-BJ-Wu.png",
            //        "Assets/Resources/Res/UI/Window/TG_MainWindow2.prefab",
            //        "Assets/Resources/Res/Effect/TG_TX/Public/Prefab/mob/M_yingyao_001/M_yingyao_skill6_new.prefab",
            //        "Assets/Resources/Res/SoundBank/Scene.bytes",
            //        "Assets/Resources/Res/SoundBank/Players.bytes",
            //        "Assets/Resources/Res/Effect/TG_TX/Public/Material/path/path_10023_mask_path_TG_948.mat"
            //    };

            //开始构建 AssetBundle
            BuildAssetBundle(target, 2, false, null);
        }

        /// <summary>
        /// Build Android
        /// </summary>
        [MenuItem("Tools/AssetBundle/打包Android资源")]
        static void BuildAndroid()
        {
            BuildTarget target = BuildTarget.Android;

            //开始构建 AssetBundle
            BuildAssetBundle(target, 2, false, null);
        }

        /// <summary>
        /// Build IOS
        /// </summary>
        [MenuItem("Tools/AssetBundle/打包IOS资源")]
        static void BuildIOS()
        {
            BuildTarget target = BuildTarget.iOS;

            //开始构建 AssetBundle
            BuildAssetBundle(target, 2, false, null);
        }

        /// <summary>
        /// 获取，需要打包的资源数据
        /// </summary>
        /// <returns></returns>
        static string[] GetNeedBuildAsset()
        {
            string url = string.Format("{0}/Config.txt", Application.streamingAssetsPath);
            //不存在文件数据
            if (!File.Exists(url))
            {
                Debug.LogError(" 不存在Config.txt文件 ");
                return null;
            }

            using (StreamReader sr = new StreamReader(url))
            {
                string line;
                List<string> strList = new List<string>();
                while ((line = sr.ReadLine()) != null)
                {
                    Debug.Log("需要进行打包的资源数据有 " + line);

                    strList.Add(string.Format("Assets/Resources/{0}", line));
                }

                Debug.Log(" Res 资源数据获取完成 " + strList.Count);

                return strList.ToArray();
            }
        }

        /// <summary>
        /// Bundle PC
        /// </summary>
        [MenuItem("Tools/AssetBundle/打包PC资源2")]
        static void BuildPC2()
        {
            BuildTarget target = BuildTarget.StandaloneWindows;

            //string[] arr = new string[]
            //    {
            //        "Assets/Resources/Res/UI/PackerImg/BD-BJ-Wu.png",
            //        "Assets/Resources/Res/UI/Window/TG_MainWindow2.prefab",
            //        "Assets/Resources/Res/Effect/TG_TX/Public/Prefab/mob/M_yingyao_001/M_yingyao_skill6_new.prefab",
            //        "Assets/Resources/Res/SoundBank/Scene.bytes",
            //        "Assets/Resources/Res/SoundBank/Players.bytes",
            //        "Assets/Resources/Res/Effect/TG_TX/Public/Material/path/path_10023_mask_path_TG_948.mat"
            //    };

            //开始构建 AssetBundle
            BuildAssetBundle2(target, 2, false, null);
        }
        [MenuItem("Tools/AssetBundle/打包PC资源3")]
        static void BuildPC3()
        {
            string out_path = GetAssetBundleOutPath();

            BuildAssetInternal(out_path, 2, BuildTarget.StandaloneWindows, false);
        }

        /// <summary>
        /// 真实的打包资源数据
        /// </summary>
        /// <param name="exportDir"></param>
        /// <param name="compressType"></param>
        /// <param name="target"></param>
        /// <param name="isReBuild"></param>
        /// <param name="builds"></param>
        /// <returns></returns>
        public static AssetBundleManifest BuildAssetInternal(string exportDir, int compressType, BuildTarget target, bool isReBuild = true)
        {
            BuildAssetBundleOptions buildOpts = BuildAssetBundleOptions.DeterministicAssetBundle;

            if (isReBuild)
                buildOpts |= BuildAssetBundleOptions.ForceRebuildAssetBundle;
            if (compressType == 0)
                buildOpts |= BuildAssetBundleOptions.UncompressedAssetBundle;
            //#if UNITY_5_3_OR_NEWER
            //        else if (compressType == 2)
            //            buildOpts |= BuildAssetBundleOptions.ChunkBasedCompression;
            //#endif

            AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(exportDir, buildOpts, target);

            return manifest;
        }
        static void BuildAssetBundle2(BuildTarget buildTarget, int compre_type, bool is_force_build, string[] build_asset_arr)
        {
            BuildUtils.ClearAllAssetNames();

            //生成资源的依赖关系
            AssetsDependenctGenerateEditor.UpdateAssetDependenct();

            //构建引用关系图形
            AssetDependenctTree.BuildAssetDependenctGraph();

            //更新资源打包后缀
            UpAssetExtension(buildTarget);

            //获取所有需要打包的资源数据引用
            var dic = AllNeedBuildAssetDependenct(build_asset_arr);

            //获取资源进行单独打包的列表
            var singles = BuildSinglePackage(dic);

            //获取目录资源打包
            //var directory = BuildDirectoryPackage(dic);

            //获取资源进行除单独打包的列表
            var resources = BuildAllResourceEx(dic);

            //获取总共需要进行打包的资源数据
            var builds = ArrayTools.Concat(singles, resources);

            //var exportDir = BuildUtils.CreateAssetBundleDir(buildTarget, out_path);//构建不同平台的子目录

            //var fullversion = BuildUtils.GetCurrentPackageVersion();

            //var bundleName = BuildUtils.GetAssetBundleName(buildTarget);

            //int major, minor, build;

            //VersionJson.GetResVersion(fullversion, out major, out minor, out build);

            //var version = build.ToString().PadLeft(VersionJson.VersionCount, '0');

            //var versionPath = BuildUtils.GenVersionPath(__versionPath, version);

            //AssetBundleManifest assetBundlemainfest;

            //if (build > 0)//小版本的情况
            //{
            //    Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!build version" + version);

            //    //如果某一个版本的文件存在 就全部删除掉.
            //    if (Directory.Exists(versionPath))
            //    {
            //        Directory.Delete(versionPath, true);
            //    }

            //    //不强制打包 小幅度增加版本号,以上一个版本的数据比较变动
            //    assetBundlemainfest = BuildBundlesUnity5(exportDir, __compressType, __buildTarget, false, builds);

            //    if (assetBundlemainfest != null)
            //    {
            //        var staticManifest = BuildUtils.GetStaticAssetBundleManifest(lastVersionPath, bundleName);

            //        var filePath = lastVersionPath;

            //        if (staticManifest != null)
            //        {
            //            string[] addResource, deleteResource, updateResource;

            //            BuildUtils.BuildVersionMergeInfomation(filePath, exportDir, staticManifest, assetBundlemainfest, out addResource, out deleteResource, out updateResource);

            //            CopyResourceToVersionFile(addResource, updateResource, exportDir, versionPath);

            //            CopyAssetBundleManifestFile(exportDir, versionPath, __buildTarget);

            //            CreateVersionFile(versionPath, fullversion, __buildTarget);

            //            //生成信息中需要包含新增资源 删除资源 更新资源
            //            GenLittleVersionChange(addResource, updateResource, deleteResource, versionPath);
            //        }
            //        else
            //        {
            //            Debug.LogError("Error!!,Last Version Is Empty!" + version);
            //        }
            //    }
            //    else
            //    {
            //        Debug.LogError("Error! AssetBundleMainfest is Null.");
            //    }
            //}
            //else//大版本的情况
            //{
            //    if (Directory.Exists(versionPath))
            //    {
            //        Directory.Delete(versionPath, true);
            //    }

            //    Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!build version" + version);

            //    //强制重新生成,全部拷贝到一个新的目录 更新大版本 生成到StreamingPath
            //    assetBundlemainfest = BuildBundlesUnity5(exportDir, __compressType, __buildTarget, true, builds);

            //    if (assetBundlemainfest != null)
            //    {
            //        //到version目录 1 2 3
            //        CopyAssetBundleManifestFile(exportDir, versionPath, __buildTarget);

            //        //备份文件到Version文件版本库中 虽然以后不用使用 但是需要提取版本变化文件
            //        CopyResourceToVersionFile(assetBundlemainfest.GetAllAssetBundles(), null, exportDir, versionPath);

            //        CreateVersionFile(versionPath, fullversion, __buildTarget);

            //        //到streaming目录 1 2 3
            //        CopyResouceToStreamingAssets(assetBundlemainfest, exportDir, __buildTarget);

            //        CopyVersionToStreamingAssets(versionPath, __buildTarget);

            //        CopyAssetBundleManifestToStreamingAssets(exportDir, __buildTarget);
            //        //拷贝到游戏里面 供下一个打包流程使用
            //    }
            //    else
            //    {
            //        Debug.LogError("Error! AssetBundleMainfest is Null.");
            //    }
            //}

            ////比对文件
            //if (__withMD5 && assetBundlemainfest != null)
            //{
            //    var bundlexList = CreateBundleMD5(assetBundlemainfest, exportDir);

            //    ProcessVersion(exportDir, bundlexList);

            //    //处理StreamingAsset里面的资源
            //    ChangeBundleNameToMd5(exportDir, bundlexList);
            //}

            //获取AssetBundle输出目录
            string out_path = GetAssetBundleOutPath();

            AssetBundleManifest assetBundlemainfest;

            //不强制打包 小幅度增加版本号,以上一个版本的数据比较变动
            assetBundlemainfest = BuildAssetBundle(out_path, compre_type, buildTarget, is_force_build, builds);

            if (assetBundlemainfest != null)
            {
                Debug.Log("资源数据打包完成  ~~~ ");

                //存储引用数据记录
                ManifestTools.OnSaveManifest(out_path + "/Data", assetBundlemainfest);

                //var staticManifest = BuildUtils.GetStaticAssetBundleManifest(lastVersionPath, bundleName);

                //var filePath = lastVersionPath;

                //if (staticManifest != null)
                //{
                //    string[] addResource, deleteResource, updateResource;

                //    BuildUtils.BuildVersionMergeInfomation(filePath, exportDir, staticManifest, assetBundlemainfest, out addResource, out deleteResource, out updateResource);

                //    CopyResourceToVersionFile(addResource, updateResource, exportDir, versionPath);

                //    CopyAssetBundleManifestFile(exportDir, versionPath, __buildTarget);

                //    CreateVersionFile(versionPath, fullversion, __buildTarget);

                //    //生成信息中需要包含新增资源 删除资源 更新资源
                //    GenLittleVersionChange(addResource, updateResource, deleteResource, versionPath);
                //}
                //else
                //{
                //    Debug.LogError("Error!!,Last Version Is Empty!" + version);
                //}
            }
            else
            {
                Debug.LogError("Error! AssetBundleMainfest is Null.");
            }

            //清理环境
            EditorUtility.UnloadUnusedAssetsImmediate();

            //更新资源列表
            BuildUtils.Refresh();
        }

        /// <summary>
        /// 开始构建 AssetBundle
        /// </summary>
        /// <param name="target"></param>
        static void BuildAssetBundle(BuildTarget buildTarget, int compre_type, bool is_force_build, string[] build_asset_arr)
        {

            //生成资源的依赖关系
            AssetsDependenctGenerateEditor.UpdateAssetDependenct();

            //构建引用关系图形
            AssetDependenctTree.BuildAssetDependenctGraph();

            //更新资源打包后缀
            UpAssetExtension(buildTarget);

            //获取所有需要打包的资源数据引用
            var dic = AllNeedBuildAssetDependenct(build_asset_arr);

            //获取资源进行单独打包的列表
            var singles = BuildSinglePackage(dic);

            //获取目录资源打包
            var directory = BuildDirectoryPackage(dic);

            //获取资源进行除单独打包的列表
            var resources = BuildAllResourceEx(dic);

            //获取总共需要进行打包的资源数据
            var builds = ArrayTools.Concat(singles, directory, resources);

            //var exportDir = BuildUtils.CreateAssetBundleDir(buildTarget, out_path);//构建不同平台的子目录

            //var fullversion = BuildUtils.GetCurrentPackageVersion();

            //var bundleName = BuildUtils.GetAssetBundleName(buildTarget);

            //int major, minor, build;

            //VersionJson.GetResVersion(fullversion, out major, out minor, out build);

            //var version = build.ToString().PadLeft(VersionJson.VersionCount, '0');

            //var versionPath = BuildUtils.GenVersionPath(__versionPath, version);

            //AssetBundleManifest assetBundlemainfest;

            //if (build > 0)//小版本的情况
            //{
            //    Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!build version" + version);

            //    //如果某一个版本的文件存在 就全部删除掉.
            //    if (Directory.Exists(versionPath))
            //    {
            //        Directory.Delete(versionPath, true);
            //    }

            //    //不强制打包 小幅度增加版本号,以上一个版本的数据比较变动
            //    assetBundlemainfest = BuildBundlesUnity5(exportDir, __compressType, __buildTarget, false, builds);

            //    if (assetBundlemainfest != null)
            //    {
            //        var staticManifest = BuildUtils.GetStaticAssetBundleManifest(lastVersionPath, bundleName);

            //        var filePath = lastVersionPath;

            //        if (staticManifest != null)
            //        {
            //            string[] addResource, deleteResource, updateResource;

            //            BuildUtils.BuildVersionMergeInfomation(filePath, exportDir, staticManifest, assetBundlemainfest, out addResource, out deleteResource, out updateResource);

            //            CopyResourceToVersionFile(addResource, updateResource, exportDir, versionPath);

            //            CopyAssetBundleManifestFile(exportDir, versionPath, __buildTarget);

            //            CreateVersionFile(versionPath, fullversion, __buildTarget);

            //            //生成信息中需要包含新增资源 删除资源 更新资源
            //            GenLittleVersionChange(addResource, updateResource, deleteResource, versionPath);
            //        }
            //        else
            //        {
            //            Debug.LogError("Error!!,Last Version Is Empty!" + version);
            //        }
            //    }
            //    else
            //    {
            //        Debug.LogError("Error! AssetBundleMainfest is Null.");
            //    }
            //}
            //else//大版本的情况
            //{
            //    if (Directory.Exists(versionPath))
            //    {
            //        Directory.Delete(versionPath, true);
            //    }

            //    Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!build version" + version);

            //    //强制重新生成,全部拷贝到一个新的目录 更新大版本 生成到StreamingPath
            //    assetBundlemainfest = BuildBundlesUnity5(exportDir, __compressType, __buildTarget, true, builds);

            //    if (assetBundlemainfest != null)
            //    {
            //        //到version目录 1 2 3
            //        CopyAssetBundleManifestFile(exportDir, versionPath, __buildTarget);

            //        //备份文件到Version文件版本库中 虽然以后不用使用 但是需要提取版本变化文件
            //        CopyResourceToVersionFile(assetBundlemainfest.GetAllAssetBundles(), null, exportDir, versionPath);

            //        CreateVersionFile(versionPath, fullversion, __buildTarget);

            //        //到streaming目录 1 2 3
            //        CopyResouceToStreamingAssets(assetBundlemainfest, exportDir, __buildTarget);

            //        CopyVersionToStreamingAssets(versionPath, __buildTarget);

            //        CopyAssetBundleManifestToStreamingAssets(exportDir, __buildTarget);
            //        //拷贝到游戏里面 供下一个打包流程使用
            //    }
            //    else
            //    {
            //        Debug.LogError("Error! AssetBundleMainfest is Null.");
            //    }
            //}

            ////比对文件
            //if (__withMD5 && assetBundlemainfest != null)
            //{
            //    var bundlexList = CreateBundleMD5(assetBundlemainfest, exportDir);

            //    ProcessVersion(exportDir, bundlexList);

            //    //处理StreamingAsset里面的资源
            //    ChangeBundleNameToMd5(exportDir, bundlexList);
            //}

            //获取AssetBundle输出目录
            string out_path = GetAssetBundleOutPath();

            AssetBundleManifest assetBundlemainfest;

            //不强制打包 小幅度增加版本号,以上一个版本的数据比较变动
            assetBundlemainfest = BuildAssetBundle(out_path, compre_type, buildTarget, is_force_build, builds);

            if (assetBundlemainfest != null)
            {
                Debug.Log("资源数据打包完成  ~~~ ");

                //存储引用数据记录
                ManifestTools.OnSaveManifest(out_path + "/Data", assetBundlemainfest);

                //var staticManifest = BuildUtils.GetStaticAssetBundleManifest(lastVersionPath, bundleName);

                //var filePath = lastVersionPath;

                //if (staticManifest != null)
                //{
                //    string[] addResource, deleteResource, updateResource;

                //    BuildUtils.BuildVersionMergeInfomation(filePath, exportDir, staticManifest, assetBundlemainfest, out addResource, out deleteResource, out updateResource);

                //    CopyResourceToVersionFile(addResource, updateResource, exportDir, versionPath);

                //    CopyAssetBundleManifestFile(exportDir, versionPath, __buildTarget);

                //    CreateVersionFile(versionPath, fullversion, __buildTarget);

                //    //生成信息中需要包含新增资源 删除资源 更新资源
                //    GenLittleVersionChange(addResource, updateResource, deleteResource, versionPath);
                //}
                //else
                //{
                //    Debug.LogError("Error!!,Last Version Is Empty!" + version);
                //}
            }
            else
            {
                Debug.LogError("Error! AssetBundleMainfest is Null.");
            }

            //清理环境
            EditorUtility.UnloadUnusedAssetsImmediate();

            //更新资源列表
            BuildUtils.Refresh();
        }

        #endregion

        #region 资源打包之前的设置

        /// <summary>
        /// 存储资源打包的信息设置
        /// </summary>
        public static SortedList<string, AssetBundleInfo> bundleListEx = new SortedList<string, AssetBundleInfo>();

        static string extension = "";

        /// <summary>
        /// 根据平台，设置资源打包后缀
        /// </summary>
        /// <param name="buildTarget"></param>
        static void UpAssetExtension(BuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case BuildTarget.StandaloneWindows:
                    extension = ".tresources";
                    break;
                case BuildTarget.Android:
                    extension = ".aresources";
                    break;
                case BuildTarget.iOS:
                    extension = ".iresources";
                    break;
                default:
                    extension = ".tresources";
                    break;
            }
        }

        /// <summary>
        /// 获取所有需要进行打包的资源数据
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        static Dictionary<string, AssetDependenctGraph.Node> AllNeedBuildAssetDependenct(string[] arr)
        {
            if (arr == null || arr.Length == 0)
                return AssetDependenctTree.resourceGraph.result;

            int count = arr.Length;
            Dictionary<string, AssetDependenctGraph.Node> dic = new Dictionary<string, AssetDependenctGraph.Node>();
            for (int i = 0; i < count; i++)
            {
                if (string.IsNullOrEmpty(arr[i]))
                    continue;

                GetBuildAssetDependenct(AssetDependenctTree.resourceGraph.result, dic, arr[i]);
            }

            //检测是否包含shader文件
            dic = CheckHaveShaderFile(dic);

            return dic;
        }

        /// <summary>
        /// 获取 build 资源数据引用
        /// </summary>
        /// <param name="all"></param>
        /// <param name="dic"></param>
        /// <param name="file"></param>
        static void GetBuildAssetDependenct(Dictionary<string, AssetDependenctGraph.Node> all, Dictionary<string, AssetDependenctGraph.Node> dic, string file)
        {
            if (all == null
                || all.Count == 0
                || string.IsNullOrEmpty(file))
                return;

            if (all.ContainsKey(file))
            {
                if (!dic.ContainsKey(file))
                    dic.Add(file, all[file]);

                string[] child = all[file].AllChildren;
                if (child == null || child.Length == 0)
                    return;

                int count = child.Length;
                for (int i = 0; i < count; i++)
                {
                    if (string.Equals(child[i], file))
                        continue;

                    GetBuildAssetDependenct(all, dic, child[i]);
                }
            }
        }

        /// <summary>
        /// 检测可以打包的资源列表里面是否包含shader文件
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        static Dictionary<string, AssetDependenctGraph.Node> CheckHaveShaderFile(Dictionary<string, AssetDependenctGraph.Node> dic)
        {
            if (dic == null || dic.Count == 0)
                return dic;

            //检测是否包含shader文件，如果包含，那么所有shader需要全部打成一个包
            bool is_have_shader = false;
            foreach (var d in dic)
            {
                if (d.Value.IsShader())
                {
                    is_have_shader = true;
                    break;
                }
            }

            if (is_have_shader)
            {
                //如果有shader文件，那么就需要把所有的shader文件都添加到资源打包列表中
                Dictionary<string, AssetDependenctGraph.Node> all = AssetDependenctTree.resourceGraph.result;
                if (all == null || all.Count == 0)
                    return dic;

                foreach (var d in all)
                {
                    if (d.Value.IsShader())
                    {
                        if (!dic.ContainsKey(d.Key))
                            dic.Add(d.Key, d.Value);
                    }
                }
            }

            return dic;
        }

        ///// <summary>
        ///// 检测，设置构建单个包的数据资源
        ///// </summary>
        //static AssetBundleBuild[] BuildSinglePackage()
        //{
        //    var singleBuilds = new AssetBundleBuild[AssetBundleInfo.OnePackageTypes.Length];

        //    for (int oneIndex = 0; oneIndex < AssetBundleInfo.OnePackageTypes.Length; oneIndex++)
        //    {
        //        //进行单独打成一个包的资源数据
        //        var list = new List<AssetBundleInfo>();

        //        foreach (var result in AssetDependenctTree.resourceGraph.result)
        //        {
        //            var node = result.Value;

        //            AssetBundleInfo bundle;

        //            //获取一个没有后缀名的全路径
        //            var bundleKey = BuildUtils.GetPathDirWithoutExtension(node.NodePath);

        //            if (!bundleListEx.TryGetValue(bundleKey, out bundle))
        //            {
        //                bundle = new AssetBundleInfo(node, AssetDependenctTree.resourceGraph.result, bundleListEx);

        //                bundleListEx.Add(bundleKey, bundle);
        //            }

        //            //检测是否为当前同种类型
        //            if (AssetBundleInfo.OnePackageTypes[oneIndex] == bundle.type)
        //            {
        //                bundle.MarkAsBuild();

        //                list.Add(bundle);
        //            }
        //        }

        //        var singleBuild = new AssetBundleBuild();

        //        var assetPath = "res/" + AssetBundleInfo.OnePackageTypes[oneIndex].ToString() + extension;

        //        singleBuild.assetBundleName = assetPath;

        //        singleBuild.assetNames = new string[list.Count];

        //        for (int i = 0; i < list.Count; i++)
        //        {
        //            singleBuild.assetNames[i] = list[i].selfNode.NodePath;
        //        }

        //        singleBuilds[oneIndex] = singleBuild;
        //    }

        //    return singleBuilds;
        //}

        ///// <summary>
        ///// 构建除了单独打包之外的其他资源
        ///// </summary>
        ///// <param name="withSubAsset"></param>
        //private static AssetBundleBuild[] BuildAllResourceEx(bool withSubAsset = true)
        //{
        //    var assetBundlesList = new List<AssetBundleBuild>();

        //    //自底层分析出来 哪个资源需要单独拆出来打包
        //    int maxdepth = AssetDependenctGraph.Node.MaxDepth;

        //    //资源按照depth排序 计算出来最大的深度
        //    for (int i = maxdepth; i >= 0; i--)
        //    {
        //        var currentDepth = i;

        //        //获取某一 depth 的所有资源数据
        //        var alldepth = BuildUtils.FindAll(AssetDependenctTree.resourceGraph.result, tree => tree.Depth == currentDepth);

        //        for (int j = 0; j < alldepth.Count; j++)
        //        {
        //            AssetBundleInfo info;

        //            var bundleKey = BuildUtils.GetPathDirWithoutExtension(alldepth[j].NodePath);

        //            //如果不在资源打包列表，那么添加进来
        //            if (!bundleListEx.TryGetValue(bundleKey, out info))
        //            {
        //                info = new AssetBundleInfo(alldepth[j], AssetDependenctTree.resourceGraph.result, bundleListEx);

        //                Debug.Log(bundleKey);

        //                bundleListEx.Add(bundleKey, info);
        //            }

        //            //查找自己被使用了几次
        //            info.BuildAlreadyBuildCount();

        //            //检测是否需要单独进行打包
        //            if (info.BuildAlone())
        //            {
        //                //设置flag已经被放到打包列表里面了
        //                info.MarkAsBuild();

        //                var assetBundleBuild = new AssetBundleBuild();

        //                var assetBundleName = GetAssetNamePath(info) + extension;

        //                assetBundleBuild.assetNames = new string[1] { info.selfNode.NodePath };

        //                assetBundleBuild.assetBundleName = assetBundleName;

        //                assetBundlesList.Add(assetBundleBuild);
        //            }
        //            else//如果不需要单独打包 就不做任何处理
        //            {

        //            }
        //        }
        //    }

        //    return assetBundlesList.ToArray();
        //}

        /// <summary>
        /// 检测，设置构建单个包的数据资源
        /// </summary>
        static AssetBundleBuild[] BuildSinglePackage(Dictionary<string, AssetDependenctGraph.Node> dic)
        {
            var singleBuilds = new AssetBundleBuild[AssetBundleInfo.OnePackageTypes.Length];

            for (int oneIndex = 0; oneIndex < AssetBundleInfo.OnePackageTypes.Length; oneIndex++)
            {
                //进行单独打成一个包的资源数据
                var list = new List<AssetBundleInfo>();

                foreach (var result in dic)
                {
                    var node = result.Value;

                    AssetBundleInfo bundle;

                    //获取一个没有后缀名的全路径
                    var bundleKey = BuildUtils.GetPathDirWithoutExtension(node.NodePath);

                    if (!bundleListEx.TryGetValue(bundleKey, out bundle))
                    {
                        bundle = new AssetBundleInfo(node, dic, bundleListEx);

                        bundleListEx.Add(bundleKey, bundle);
                    }

                    //检测是否为当前同种类型
                    if (AssetBundleInfo.OnePackageTypes[oneIndex] == bundle.type)
                    {
                        bundle.MarkAsBuild();

                        list.Add(bundle);
                    }
                }

                var singleBuild = new AssetBundleBuild();

                var assetPath = "res/" + AssetBundleInfo.OnePackageTypes[oneIndex].ToString() + extension;

                singleBuild.assetBundleName = assetPath;

                singleBuild.assetNames = new string[list.Count];

                for (int i = 0; i < list.Count; i++)
                {
                    singleBuild.assetNames[i] = list[i].selfNode.NodePath;
                }

                singleBuilds[oneIndex] = singleBuild;
            }

            return singleBuilds;
        }

        /// <summary>
        /// 按照目录进行打包
        /// </summary>
        static AssetBundleBuild[] BuildDirectoryPackage(Dictionary<string, AssetDependenctGraph.Node> all)
        {
            List<AssetBundleBuild> singleBuilds = new List<AssetBundleBuild>();

            //需要进行目录打包的资源
            Dictionary<string, List<AssetBundleInfo>> dic = new Dictionary<string, List<AssetBundleInfo>>();

            for (int oneIndex = 0; oneIndex < AssetBundleInfo.DirectoryTypes.Length; oneIndex++)
            {
                foreach (var result in all)
                {
                    var node = result.Value;

                    AssetBundleInfo bundle;

                    //获取一个没有后缀名的全路径
                    var bundleKey = BuildUtils.GetPathDirWithoutExtension(node.NodePath);

                    if (!bundleListEx.TryGetValue(bundleKey, out bundle))
                    {
                        bundle = new AssetBundleInfo(node, all, bundleListEx);

                        bundleListEx.Add(bundleKey, bundle);
                    }

                    //检测是否为当前同种类型
                    if (AssetBundleInfo.DirectoryTypes[oneIndex] == bundle.type)
                    {
                        bundle.MarkAsBuild();

                        string url = Path.GetDirectoryName(node.NodePath);

                        if (dic.ContainsKey(url))
                        {
                            dic[url].Add(bundle);
                        }
                        else
                        {
                            List<AssetBundleInfo> list = new List<AssetBundleInfo>();
                            list.Add(bundle);
                            dic[url] = list;
                        }
                    }
                }


                foreach (var item in dic)
                {
                    var singleBuild = new AssetBundleBuild();

                    var assetPath = AssetPathRule.GetDirectoryBuildBundlePath(item.Key) + extension;

                    singleBuild.assetBundleName = assetPath;

                    var list = item.Value;

                    singleBuild.assetNames = new string[list.Count];

                    for (int i = 0; i < list.Count; i++)
                    {
                        singleBuild.assetNames[i] = list[i].selfNode.NodePath;
                    }

                    singleBuilds.Add(singleBuild);
                }
            }

            return singleBuilds.ToArray();
        }

        /// <summary>
        /// 构建除了单独打包之外的其他资源
        /// </summary>
        /// <param name="withSubAsset"></param>
        private static AssetBundleBuild[] BuildAllResourceEx(Dictionary<string, AssetDependenctGraph.Node> dic, bool withSubAsset = true)
        {
            var assetBundlesList = new List<AssetBundleBuild>();

            //自底层分析出来 哪个资源需要单独拆出来打包
            int maxdepth = AssetDependenctGraph.Node.MaxDepth;

            //资源按照depth排序 计算出来最大的深度
            for (int i = maxdepth; i >= 0; i--)
            {
                var currentDepth = i;

                //获取某一 depth 的所有资源数据
                var alldepth = BuildUtils.FindAll(dic, tree => tree.PositiveDepth == currentDepth);

                for (int j = 0; j < alldepth.Count; j++)
                {
                    AssetBundleInfo info;

                    var bundleKey = BuildUtils.GetPathDirWithoutExtension(alldepth[j].NodePath);

                    //如果不在资源打包列表，那么添加进来
                    if (!bundleListEx.TryGetValue(bundleKey, out info))
                    {
                        info = new AssetBundleInfo(alldepth[j], dic, bundleListEx);

                        Debug.Log(bundleKey);

                        bundleListEx.Add(bundleKey, info);
                    }

                    //查找自己被使用了几次
                    info.BuildAlreadyBuildCount();

                    //检测是否需要单独进行打包
                    if (info.BuildAlone())
                    {
                        //设置flag已经被放到打包列表里面了
                        info.MarkAsBuild();

                        var assetBundleBuild = new AssetBundleBuild();

                        var assetBundleName = GetAssetNamePath(info) + extension;

                        assetBundleBuild.assetNames = new string[1] { info.selfNode.NodePath };

                        assetBundleBuild.assetBundleName = assetBundleName;

                        assetBundlesList.Add(assetBundleBuild);
                    }
                    else//如果不需要单独打包 就不做任何处理
                    {

                    }
                }
            }

            return assetBundlesList.ToArray();
        }

        /// <summary>
        /// 资源所在目录名字默认为 Resouces
        /// </summary>
        public static string Resouces = "Resources";

        /// <summary>
        /// 获取资源的路径信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private static string GetAssetNamePath(AssetBundleInfo info)
        {
            var path = Path.GetDirectoryName(info.selfNode.NodePath) + "/" + Path.GetFileNameWithoutExtension(info.selfNode.NodePath);

            var index = path.IndexOf(Resouces);

            return path.Substring(index + Resouces.Length + 1);
        }

        #endregion

        #region 资源打包

        /// <summary>
        /// 真实的打包资源数据
        /// </summary>
        /// <param name="exportDir"></param>
        /// <param name="compressType"></param>
        /// <param name="target"></param>
        /// <param name="isReBuild"></param>
        /// <param name="builds"></param>
        /// <returns></returns>
        public static AssetBundleManifest BuildAssetBundle(string exportDir, int compressType, BuildTarget target, bool isReBuild = true, AssetBundleBuild[] builds = null)
        {
            BuildAssetBundleOptions buildOpts = BuildAssetBundleOptions.DeterministicAssetBundle;

            if (isReBuild)
                buildOpts |= BuildAssetBundleOptions.ForceRebuildAssetBundle;
            if (compressType == 0)
                buildOpts |= BuildAssetBundleOptions.UncompressedAssetBundle;
            //#if UNITY_5_3_OR_NEWER
            //        else if (compressType == 2)
            //            buildOpts |= BuildAssetBundleOptions.ChunkBasedCompression;
            //#endif

            AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(exportDir, builds, buildOpts, target);

            return manifest;
        }

        #endregion

    }

}



