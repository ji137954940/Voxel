using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Tgame.AssetBundle
{

    /// <summary>
    /// 资源打包工具类
    /// </summary>
    public static class BuildUtils
    {

        /// <summary>
        /// 优化打包速度
        /// 每次设置AssetName都是很慢的一件事
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="assetName"></param>
        public static void SetAssetBundleName(AssetImporter importer, string assetName)
        {
            if (importer != null)
            {
                if (!importer.assetBundleName.Equals(assetName))
                {
                    importer.assetBundleName = assetName;
                }
            }
        }

        /// <summary>
        /// 比对两个Manifest的差异
        /// </summary>
        public static void BuildVersionMergeInfomation(string oldbasePath, string newbasePath, AssetBundleManifest baseManifest, AssetBundleManifest afterManifest, out string[] addResource, out string[] deleteResource, out string[] updateResource)
        {
            var allAssetBundle = baseManifest.GetAllAssetBundles();

            var afterAllAssetBundle = afterManifest.GetAllAssetBundles();

            //新增加的资源列表
            addResource = afterAllAssetBundle.Except(allAssetBundle).ToArray();

            //addResource.Each(s => Debug.Log("Update::::::::::::::::::" + s));

            //删除的资源列表
            deleteResource = allAssetBundle.Except(afterAllAssetBundle).ToArray();

            //deleteResource.Each(s => Debug.Log("Update::::::::::::::::::" + s));

            //两边都存在的资源
            var unionResource = allAssetBundle.Intersect(afterAllAssetBundle).ToArray();

            //更新的资源列表
            var updateResourceList = new List<string>();

            for (int i = 0; i < unionResource.Length; i++)
            {
                if (baseManifest.GetAssetBundleHash(unionResource[i]) != afterManifest.GetAssetBundleHash(unionResource[i]))
                {
                    updateResourceList.Add(unionResource[i]);
                }
                else
                {
                    //Fixed Unity Script Not Change BUG
                    //相等的情况下比对MD5 查看是否需要更新脚本 当脚本需要更新的时候 需要打大版本
                    var oldFilePath = Path.Combine(oldbasePath, unionResource[i]);

                    var newFilePath = Path.Combine(newbasePath, unionResource[i]);

                    var md5 = BuildUtils.Md5(oldFilePath, false);

                    var newMD5 = BuildUtils.Md5(newFilePath, false);

                    //Debug.Log(oldFilePath + " " + newFilePath + " MD5：" + md5 + " new MD5" + newMD5);

                    if (md5 != newMD5)
                    {
                        updateResourceList.Add(unionResource[i]);

                        //Debug.LogFormat("Changed!!!:{0},oldFilePath:{1},NewFilePath:{2}", unionResource[i], oldFilePath, newFilePath);

                        //Debug.Log("Changed!!! " + unionResource[i]);

                        Debug.LogError("We need Build a Big Version " + unionResource[i] + " is modify.");
                    }
                    else
                    {
                        //Debug.Log("No change" + unionResource[i]);
                    }
                }
            }

            updateResource = updateResourceList.ToArray();

            //updateResource.Each(s => Debug.Log("Update::::::::::::::::::" + s));
        }

        public static AssetsDependenctData BuildAndSave()
        {
            var resource = BuildUtils.GetAsset<AssetsDependenctData>(AssetsDependenctGenerateEditor.EditorGenerate);

            resource = UnityEngine.Object.Instantiate<AssetsDependenctData>(resource);

            resource.FixedDirty();

            return resource;
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static T GetAsset<T>(string assetPath) where T : UnityEngine.Object
        {
            return AssetDatabase.LoadAssetAtPath<T>(assetPath);
        }
        /// <summary>
        /// 移除Asset
        /// </summary>
        /// <param name="assetPath"></param>
        public static void RemoveAsset(string assetPath)
        {
            AssetDatabase.MoveAssetToTrash(assetPath);
        }
        /// <summary>
        /// 保存资源Asset
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="assetPathAndName"></param>
        public static void SaveAsset(UnityEngine.Object asset, string assetPathAndName)
        {

            Debug.Log("Save? " + assetPathAndName);

            var assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPathAndName);

            AssetDatabase.CreateAsset(asset, assetPath);

            EditorUtility.SetDirty(asset);

            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// 获取依赖关系
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static string[] GetDependencies(string assetPath)
        {
            //如果一个资源是需要剔除的资源，那么就不获取它的引用数据（.unity文件的烘焙数据会引用 .unity 文件 造成相互引用）
            if (AssetFileType.IsExcludeResDependenct(assetPath))
                return new string[1] { assetPath };

            var assetDependencies = AssetDatabase.GetDependencies(new string[] { assetPath });

            return assetDependencies;
        }


        /// <summary>
        /// 获取Unity的路径格式
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetUnityPath(string path)
        {
            string newstring = path.Replace("\\", "/");

            return newstring;
        }


        /// <summary>
        /// 获取一个没有后缀名的全路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetPathDirWithoutExtension(string path)
        {
            path = path.ToLower();
            return path;
        }

        /// <summary>
        /// 获取一个资源的后缀名
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetPathExtension(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            return Path.GetExtension(path);
        }

        /// <summary>
        /// 清理所有打包过程中产生的AssetBundleName
        /// </summary>
        public static void ClearAllAssetNames()
        {
            var assetBundleNames = AssetDatabase.GetAllAssetBundleNames();

            if (assetBundleNames == null || assetBundleNames.Length <= 0)
                return;

            for (int i = 0; i < assetBundleNames.Length; ++i)
            {
                float process = ((float)i) / ((float)assetBundleNames.Length);

                EditorUtility.DisplayProgressBar("清理AssetBundleName中...", assetBundleNames[i], process);

                AssetDatabase.RemoveAssetBundleName(assetBundleNames[i], true);

                EditorUtility.UnloadUnusedAssetsImmediate();
            }

            EditorUtility.ClearProgressBar();
        }


        /// <summary>
        /// 版本号的文件地址
        /// </summary>
        public const string buildVersionCFG = "buildVersion.cfg";

        /// <summary>
        /// 初始的build版本号
        /// </summary>
        public const string initBuildVersion = "1.000.000";

        /// <summary>
        /// 获取当前的打包版本号
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentPackageVersion()
        {
            var buildVersion = string.Empty;

            if (string.IsNullOrEmpty(buildVersion))
            {
                string versionFile = buildVersionCFG;
                if (!File.Exists(versionFile))
                    buildVersion = initBuildVersion;
                else
                {
                    FileStream stream = new FileStream(versionFile, FileMode.Open, FileAccess.Read);
                    try
                    {
                        if (stream.Length <= 0)
                        {
                            buildVersion = initBuildVersion;
                        }
                        else
                        {
                            byte[] src = new byte[stream.Length];
                            stream.Read(src, 0, src.Length);
                            string ver = Encoding.ASCII.GetString(src);
                            ver = ver.Trim();
                            if (string.IsNullOrEmpty(ver))
                                buildVersion = initBuildVersion;
                            else
                                buildVersion = ver;
                        }
                    }
                    finally
                    {
                        stream.Close();
                        stream.Dispose();
                    }
                }
            }
            return buildVersion;
        }

        /// <summary>
        /// 获取对应平台的bundleName
        /// </summary>
        /// <param name="platform"></param>
        /// <returns></returns>
        public static string GetAssetBundleName(BuildTarget platform)
        {
            switch (platform)
            {
                case BuildTarget.Android:
                    return "Android";
                case BuildTarget.StandaloneWindows:
                    return "Windows";
                case BuildTarget.StandaloneWindows64:
                    return "Windows64";
                case BuildTarget.iOS:
                    return "iOS";
                case BuildTarget.tvOS:
                    return "iOS";
                case BuildTarget.StandaloneOSX:
                    return "Mac";
            }
            Debug.LogError("Error! platform");
            return string.Empty;
        }

        /// <summary>
        /// 创建对应平台的导出文件
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="exportDir"></param>
        /// <returns></returns>
        public static string CreateAssetBundleDir(BuildTarget platform, string exportDir)
        {
            string outPath;
            bool isExternal = false;
            if (!string.IsNullOrEmpty(exportDir))
            {
                isExternal = true;
                outPath = exportDir;
            }
            else
                outPath = "Assets/StreamingAssets";

            if (!Directory.Exists(outPath))
            {
                if (isExternal)
                    Directory.CreateDirectory(outPath);
                else
                    AssetDatabase.CreateFolder("Assets", "StreamingAssets");
            }

            var platformName = GetAssetBundleName(platform);
            switch (platform)
            {
                case BuildTarget.Android:
                    {

                        outPath += "/" + platformName;
                        if (!Directory.Exists(outPath))
                        {
                            if (isExternal)
                                Directory.CreateDirectory(outPath);
                            else
                                AssetDatabase.CreateFolder("Assets/StreamingAssets", platformName);
                        }
                        break;
                    }

                case BuildTarget.StandaloneWindows:
                    {
                        outPath += "/" + platformName;
                        if (!Directory.Exists(outPath))
                        {
                            if (isExternal)
                                Directory.CreateDirectory(outPath);
                            else
                                AssetDatabase.CreateFolder("Assets/StreamingAssets", platformName);
                        }
                        break;
                    }
                case BuildTarget.StandaloneOSX:
                    {
                        outPath += "/" + platformName;
                        if (!Directory.Exists(outPath))
                        {
                            if (isExternal)
                                Directory.CreateDirectory(outPath);
                            else
                                AssetDatabase.CreateFolder("Assets/StreamingAssets", platformName);
                        }
                        break;
                    }
                case BuildTarget.iOS:
                    {
                        outPath += "/" + platformName;
                        if (!Directory.Exists(outPath))
                        {
                            if (isExternal)
                                Directory.CreateDirectory(outPath);
                            else
                                AssetDatabase.CreateFolder("Assets/StreamingAssets", platformName);
                        }
                        break;
                    }
                default:
                    return string.Empty;
            }

            return outPath;
        }

        /// <summary>
        /// 在编辑器下清理数据
        /// </summary>
        public static void CleanResources()
        {
            AssetDatabase.RemoveUnusedAssetBundleNames();
            EditorUtility.UnloadUnusedAssetsImmediate();
        }

        /// <summary>
        /// 清理MD5 Cache
        /// </summary>
        public static void ClearMd5FileMap()
        {
            fileNameMd5CacheMap.Clear();
            fileContentMd5CacheMap.Clear();
        }

        private static Dictionary<string, string> fileNameMd5CacheMap
            = new Dictionary<string, string>();

        private static Dictionary<string, string> fileContentMd5CacheMap
            = new Dictionary<string, string>();

        /// <summary>
        /// MD5计算器
        /// </summary>
        private static MD5 md5Calculator = new MD5CryptoServiceProvider();

        /// <summary>
        /// 返回文件名的MD5
        /// </summary>
        /// <param name="filePath">路径名字</param>
        /// <param name="isOnlyUseFileName">是否只有路径MD5 还是整个文件md5</param>
        /// <returns></returns>
        public static string Md5(string filePath, bool isOnlyUseFileName = true)
        {
            if (string.IsNullOrEmpty(filePath))
                return string.Empty;

            if (!File.Exists(filePath))
                return string.Empty;

            string ret;
            if (isOnlyUseFileName)
            {
                if (fileNameMd5CacheMap.TryGetValue(filePath, out ret))
                    return ret;
            }
            else
            {
                if (fileContentMd5CacheMap.TryGetValue(filePath, out ret))
                    return ret;
            }

            ret = string.Empty;

            if (isOnlyUseFileName)
            {
                //string fileName = Path.GetFileNameWithoutExtension(filePath);
                byte[] src = Encoding.ASCII.GetBytes(filePath);
                byte[] hash = md5Calculator.ComputeHash(src);
                for (int i = 0; i < hash.Length; i++)
                {
                    ret += hash[i].ToString("X").PadLeft(2, '0');
                }

                ret = ret.ToLower();

                fileNameMd5CacheMap.Add(filePath, ret);
            }
            else
            {
                FileStream stream = new FileStream(filePath, FileMode.Open);
                try
                {
                    if (stream.Length <= 0)
                        return string.Empty;
                    byte[] src = new byte[stream.Length];
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.Read(src, 0, src.Length);
                    byte[] hash = md5Calculator.ComputeHash(src);

                    for (int i = 0; i < hash.Length; i++)
                    {
                        ret += hash[i].ToString("X").PadLeft(2, '0');
                    }

                    ret = ret.ToLower();

                    fileContentMd5CacheMap.Add(filePath, ret);
                }
                finally
                {
                    stream.Close();
                    stream.Dispose();
                }
            }

            return ret;
        }


        /// <summary>
        /// 移除打包之后Unity生成的文件
        /// </summary>
        /// <param name="outPath"></param>
        public static void RemoveManifestFiles(string outPath)
        {
            var files = Directory.GetFiles(outPath, "*.manifest", SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; i++)
            {
                File.Delete(files[i]);
            }
        }

        /// <summary>
        /// 获取相对于项目的路径
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static string GetAssetRelativePath(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
                return string.Empty;

            fullPath = fullPath.Replace("\\", "/");

            var lowerFullPath = fullPath.ToLower();

            var index = lowerFullPath.IndexOf("assets/");

            if (index < 0)
                return fullPath;

            return fullPath.Substring(index);
        }

        /// <summary>
        /// 刷新资源
        /// </summary>
        public static void Refresh()
        {
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 获取上一个版本的AssetBundleManifest
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        internal static AssetBundleManifest GetStaticAssetBundleManifest(string path, string manifestFileName)
        {
            var manifestFilePath = path + Path.DirectorySeparatorChar + manifestFileName;

            var manifestBytes = File.ReadAllBytes(manifestFilePath);

            var assetBundle = UnityEngine.AssetBundle.LoadFromMemory(manifestBytes);

            var assets = assetBundle.LoadAllAssets<AssetBundleManifest>();

            if (assets.Length == 1)
            {
                return assets[0];
            }
            return null;
        }


        ///// <summary>
        ///// 获取上一个版本的AssetBundleManifest
        ///// </summary>
        ///// <param name="version"></param>
        ///// <returns></returns>
        //internal static AssetBundleManifest GetLastBundleManifest(string versionPath, string version, string manifestFileName)
        //{
        //    int major, minor, build;

        //    const char versionSplitChar = '.';

        //    VersionJson.GetResVersion(version, out major, out minor, out build);

        //    if (build > 0)
        //    {
        //        build--;

        //        var lastVersion = GetVersion(major, minor, build);// major.ToString() + versionSplitChar + string.Format("{0:D" + VersionJson.VersionCount + "}", minor) + versionSplitChar + string.Format("{0:D" + VersionJson.VersionCount + "}", build);

        //        var manifestFilePath = Path.Combine(versionPath, lastVersion) + Path.DirectorySeparatorChar + manifestFileName;

        //        var manifestBytes = File.ReadAllBytes(manifestFilePath);

        //        var assetBundle = UnityEngine.AssetBundle.LoadFromMemory(manifestBytes);

        //        var assets = assetBundle.LoadAllAssets<AssetBundleManifest>();

        //        if (assets.Length == 1)
        //        {
        //            return assets[0];
        //        }
        //    }
        //    else
        //    {
        //        Debug.LogError("Error minor version" + build);
        //    }

        //    return null;
        //}


        //public static string GetVersion(int major, int minor, int build)
        //{
        //    return string.Format("{0}.{1}.{2}", major.ToString(), minor.ToString().PadLeft(VersionJson.VersionCount, '0'), build.ToString().PadLeft(VersionJson.VersionCount, '0'));
        //}

        ///// <summary>
        ///// 获取上一个版本的AssetBundlePath
        ///// </summary>
        ///// <param name="version"></param>
        ///// <returns></returns>
        //internal static string GetBaseBundlePath(string versionPath, string version)
        //{
        //    int major, minor, build;

        //    const char versionSplitChar = '.';

        //    VersionJson.GetResVersion(version, out major, out minor, out build);

        //    //GetVersion(version, out major, out minor, out mLen);

        //    if (build > 0)
        //    {
        //        build = 0;

        //        var lastVersion = GetVersion(major, minor, build);// major.ToString() + versionSplitChar + minor.ToString().PadLeft(VersionJson.VersionCount, '0') + versionSplitChar + build.ToString().PadLeft(VersionJson.VersionCount, '0');

        //        var manifestFilePath = Path.Combine(versionPath, lastVersion);

        //        return manifestFilePath;
        //    }
        //    else
        //    {
        //        Debug.LogError("Error minor version" + minor);
        //    }

        //    return null;
        //}

        ///// <summary>
        ///// 获取上一个版本的AssetBundlePath
        ///// </summary>
        ///// <param name="version"></param>
        ///// <returns></returns>
        //internal static string GetLastBundlePath(string versionPath, string version)
        //{
        //    int major, minor, build;

        //    const char versionSplitChar = '.';

        //    VersionJson.GetResVersion(version, out major, out minor, out build);

        //    //GetVersion(version, out major, out minor, out mLen);

        //    if (build > 0)
        //    {
        //        build--;

        //        var lastVersion = GetVersion(major, minor, build);// major.ToString() + versionSplitChar + string.Format("{0:D" + VersionJson.VersionCount + "}", minor) + versionSplitChar + string.Format("{0:D" + VersionJson.VersionCount + "}", build);

        //        var manifestFilePath = Path.Combine(versionPath, lastVersion);

        //        return manifestFilePath;
        //    }
        //    else
        //    {
        //        Debug.LogError("Error minor version" + minor);
        //    }

        //    return null;
        //}


        /// <summary>
        /// 获取版本目录
        /// </summary>
        /// <param name="versionPath"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        internal static string GenVersionPath(string versionPath, string version)
        {
            return Path.Combine(versionPath, version);
        }

        public static List<string> assets = new List<string>(2 * 2 * 2 * 2);
        internal static string[] ExceptScriptAndDll(string[] assetsDependencies)
        {
            assets.Clear();

            for (int i = 0; i < assetsDependencies.Length; i++)
            {
                if (!assetsDependencies[i].EndsWith(".cs") && !assetsDependencies[i].EndsWith(".dll"))
                {
                    assets.Add(assetsDependencies[i]);
                }
            }

            return assets.ToArray();
        }

        internal static List<T> FindAll<T>(Dictionary<string, T> dictionary, Predicate<T> predicate)
        {
            var list = new List<T>();

            foreach (var single in dictionary)
            {
                if (predicate(single.Value))
                {
                    list.Add(single.Value);
                }
            }

            return list;
        }
    }
}
