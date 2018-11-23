using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Tgame.AssetBundle
{
    /// <summary>
    /// 资源引用数据生成
    /// </summary>
    public class AssetsDependenctGenerateEditor : EditorWindow
    {
        /// <summary>
        /// 检查资源是否脏了
        /// </summary>
        public const string ResourceDirty = "CheckResourceDirty";

        /// <summary>
        /// 是否检测资源数据
        /// </summary>
        public const string CheckResource = "CheckResource";

        /// <summary>
        /// 生成的资源引用数据
        /// </summary>
        public const string EditorGenerate = "Assets/Built-in/Code/Editor/AssetBundleTool/AssetsDependenct.asset";

        #region 窗口界面数据刷新


        [MenuItem("Tools/AssetBundle/资源依赖关系缓存", priority = 100)]
        public static void Execute()
        {
            var editor = EditorWindow.CreateInstance<AssetsDependenctGenerateEditor>();

            editor.titleContent.text = "预计算资源依赖";

            editor.Show();
        }

        private static int index = 0;

        private static bool isPermission = false;

        private static string[] files;

        public void OnGUI()
        {
            if (GUILayout.Button("遍历文件，生成资源的依赖关系"))
            {
                files = GetAllFile();

                index = 0;

                isPermission = true;

                data = ScriptableObject.CreateInstance<AssetsDependenctData>();

                data.data = new AssetsDependenct();
            }

            var checkResource = EditorPrefs.GetBool(CheckResource);

            EditorGUI.BeginChangeCheck();

            checkResource = GUILayout.Toggle(checkResource, new GUIContent("每当导入资源时，检查资源依赖变更"));

            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool(CheckResource, checkResource);
            }
        }

        public void Update()
        {
            for (int fast = 0; fast < 10; fast++)
            {
                if (isPermission && files != null)
                {
                    if (index < files.Length)
                    {
                        var assetPath = BuildUtils.GetUnityPath(files[index]);

                        var denpendenctList = new DependenctList();

                        denpendenctList.dependencies = BuildUtils.ExceptScriptAndDll(BuildUtils.GetDependencies(assetPath));

                        denpendenctList.isdirty = false;

                        data.data.Add(assetPath, denpendenctList);

                        EditorUtility.DisplayProgressBar("正在生成资源依赖图", "进度:" + (index / (float)files.Length) * 100 + "%", ++index / (float)files.Length);
                    }
                    else
                    {
                        Debug.Log(data.data.Count + "HashCode:::" + data.data.GetHashCode());

                        isPermission = false;

                        EditorUtility.ClearProgressBar();

                        BuildUtils.RemoveAsset(EditorGenerate);

                        BuildUtils.SaveAsset(data, EditorGenerate);

                        EditorPrefs.SetBool(ResourceDirty, true);

                        data = null;
                    }
                }
            }
        }

        #endregion

        #region 检测所有资源的数据引用

        /// <summary>
        /// 资源引用存储数据
        /// </summary>
        static AssetsDependenctData data;

        /// <summary>
        /// 获取所有的资源引用数据信息
        /// </summary>
        public static void UpdateAssetDependenct()
        {
            using (new ExecuteTimer("UpdateAssetDependenct"))
            {

                //获取所有的资源引用数据信息
                data = GetAssetDependenctData();

                //刷新存储资源引用数据
                BuildUtils.RemoveAsset(EditorGenerate);

                BuildUtils.SaveAsset(data, EditorGenerate);

                EditorPrefs.SetBool(ResourceDirty, true);
            }
        }

        /// <summary>
        /// 获取当前所有的资源引用数据
        /// </summary>
        /// <returns></returns>
        static AssetsDependenctData GetAssetDependenctData()
        {
            var allFile = GetAllFile();

            AssetsDependenctData data = ScriptableObject.CreateInstance<AssetsDependenctData>();

            data.data = new AssetsDependenct();

            for (int i = 0; i < allFile.Length; i++)
            {
                var assetPath = BuildUtils.GetUnityPath(allFile[i]);

                var list = new DependenctList();

                list.dependencies = BuildUtils.ExceptScriptAndDll(BuildUtils.GetDependencies(assetPath));

                list.isdirty = false;

                data.data.Add(assetPath, list);
            }

            return data;
        }

        /// <summary>
        /// 获取所有的目录合适的文件
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public static string[] GetAllFile(string root = "Assets/Resources/Res")
        {
            var searchPattern = AssetFileType.GetSearchPattern();

            //return searchPattern.SelectMany(i => Directory.GetFiles(root, i, SearchOption.AllDirectories)).Distinct().ToArray();

            return searchPattern.SelectMany(i => SelectFile(root, i)).Distinct().ToArray();

        }

        /// <summary>
        /// 查找需要进行的文件数据
        /// </summary>
        /// <param name="root"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        static string[] SelectFile(string root, string pattern)
        {
            string[] str = Directory.GetFiles(root, pattern, SearchOption.AllDirectories);
            List<string> list = new List<string>(str);
            for (int i = 0; i < list.Count;)
            {
                if (AssetFileType.IsExclude(list[i]))
                {
                    list.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

            return list.ToArray();
        }

        #endregion
    }
}


