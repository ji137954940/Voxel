using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Tgame.AssetBundle
{
    /// <summary>
    /// 资源引用关系树（构建成树形结构）
    /// </summary>
    public class AssetDependenctTree : EditorWindow
    {

        #region 参数数据

        /// <summary>
        /// 现在的资源依赖关系图
        /// </summary>
        public static AssetDependenctGraph resourceGraph;

        /// <summary>
        /// 所有的prefab资源数据
        /// </summary>
        private static string[] prefabfiles;

        /// <summary>
        /// 当前选中的资源对象
        /// </summary>
        private Object currentSelection;

        /// <summary>
        /// 当前绘制出的结点图（GUI模式显示）
        /// </summary>
        private AssetNodeGraph newGraph;

        /// <summary>
        /// 滚动条数值
        /// </summary>
        private Vector2 scrollValue;

        /// <summary>
        /// 当前资源数据构建状态
        /// </summary>
        public static AssetDependenctTreeBuildState state = AssetDependenctTreeBuildState.Init;


        #endregion

        #region 窗口显示

        [MenuItem("Tools/AssetBundle/资源依赖图", priority = 100)]
        public static void Build()
        {
            var window = EditorWindow.GetWindow<AssetDependenctTree>();

            window.titleContent.text = "资源依赖查询";

            window.Show();
        }

        void OnGUI()
        {
            if (EditorPrefs.GetBool(AssetsDependenctGenerateEditor.ResourceDirty))
            {
                EditorPrefs.SetBool(AssetsDependenctGenerateEditor.ResourceDirty, false);

                state = AssetDependenctTreeBuildState.Init;
            }

            switch (state)
            {
                case AssetDependenctTreeBuildState.Init:

                    if (GUILayout.Button("生成资源图GUI"))
                    {
                        BuildAssetDependenctGraph();
                    }
                    break;
                case AssetDependenctTreeBuildState.NodeGraph:
                    DrawDependencyGUI();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 画依赖的GUI
        /// </summary>
        private void DrawDependencyGUI()
        {
            if (resourceGraph.result.Count > 0)
            {
                EditorGUILayout.BeginHorizontal();

                currentSelection = EditorGUILayout.ObjectField(currentSelection, typeof(Object), false);

                EditorGUILayout.EndHorizontal();

                if (currentSelection)
                {
                    var currentSelectionPath = AssetDatabase.GetAssetPath(currentSelection);

                    currentSelectionPath = BuildUtils.GetUnityPath(currentSelectionPath);

                    if (currentSelectionPath.Contains("Resources"))
                    {
                        AssetDependenctGraph.Node graph;

                        if (resourceGraph.result.TryGetValue(currentSelectionPath, out graph))
                        {
                            EditorGUILayout.BeginHorizontal();

                            var maxHeight = newGraph != null ? newGraph.maxHeight : 5000;

                            scrollValue = GUI.BeginScrollView(new Rect(0, 20, position.width, position.height), scrollValue, new Rect(0, 0, position.width, maxHeight), true, true);

                            BeginWindows();

                            if (graph.Parents != null)
                            {
                                if (newGraph == null || newGraph.tree != graph)
                                {
                                    newGraph = new AssetNodeGraph(resourceGraph.result, graph);
                                }

                                newGraph.Draw();
                            }

                            EndWindows();

                            GUI.EndScrollView();

                            EditorGUILayout.EndHorizontal();
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 当编辑器内更改了选择
        /// </summary>
        void OnSelectionChange()
        {
            currentSelection = Selection.activeObject;

            this.Repaint();
        }

        #endregion

        #region 数据结构构建

        /// <summary>
        /// 构建资源关系图
        /// </summary>
        public static void BuildAssetDependenctGraph()
        {
            var asset = BuildUtils.BuildAndSave();

            var dic = new Dictionary<string, string[]>();

            if (asset != null)
            {
                Debug.Log("This way can fast analysis resources relationship.");
                using (new ExecuteTimer("Compute cache file dependencies"))
                {
                    var resourceDependenct = asset.data.GetDic();

                    var res = resourceDependenct.GetEnumerator();

                    while (res.MoveNext())
                    {
                        dic.Add(res.Current.Key, res.Current.Value.dependencies);
                    }
                }
            }
            else
            {
                Debug.Log("This way need some time analysis resources relationship.");

                string rpath = "Assets/Resources/";
                using (new ExecuteTimer("Compute which file that need analysis"))
                {

                    //var searchPattern = AssetFileType.GetSearchPattern();

                    //var filePaths = searchPattern.SelectMany(i => Directory.GetFiles(rpath, i, SearchOption.AllDirectories)).Distinct().ToArray();

                    //prefabfiles = filePaths;

                    prefabfiles = AssetsDependenctGenerateEditor.GetAllFile();
                }

                using (new ExecuteTimer("Compute Resource dependencies."))
                {
                    for (int i = 0; i < prefabfiles.Length; i++)
                    {
                        var assetPath = prefabfiles[i] = BuildUtils.GetUnityPath(prefabfiles[i]);

                        var assetsDependencies = BuildUtils.GetDependencies(assetPath);

                        assetsDependencies = BuildUtils.ExceptScriptAndDll(assetsDependencies);

                        dic.Add(assetPath, assetsDependencies);
                    }
                }
            }

            using (new ExecuteTimer("Compute Resouce Relationship" + dic.Count))
            {
                //根据当前的资源引用关系数据，刷新构建图形数据结构
                resourceGraph = new AssetDependenctGraph(dic);

                state = AssetDependenctTreeBuildState.NodeGraph;
            }
        }

        #endregion
    }
}


