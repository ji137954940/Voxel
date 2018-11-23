
/// <summary>
/// 打包的策略:
/// 一个资源如果被多次引用 这个多次引用主要是说的他的parent的个数 还有他parent的parent的个数的和
/// 而且 这个资源占用很大的空间 这里的空间主要是说存为文件格式的时候的整体大小.
/// 这两项的乘积 和人类的干预 决定了Alone的属性
/// </summary>

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Tgame.AssetBundle
{

    /// <summary>
    /// 打出来的包的信息
    /// </summary>
    public class AssetBundleInfo
    {
        public const int threshold = 1024 * 1000;

        /// <summary>
        /// 把所有的内容打到一个包里面的类型
        /// </summary>
        public static AssetBundleBuildType[] OnePackageTypes = new AssetBundleBuildType[] { AssetBundleBuildType.Shader };

        /// <summary>
        /// 按照目录进行打包
        /// </summary>
        public static AssetBundleBuildType[] DirectoryTypes = new AssetBundleBuildType[] { AssetBundleBuildType.Animation_FBX };

        /// <summary>
        /// 必须单独打包的资源
        /// </summary>
		public static AssetBundleBuildType[] mustAlone = new AssetBundleBuildType[] { AssetBundleBuildType.Prefab, AssetBundleBuildType.Audio, AssetBundleBuildType.Text, AssetBundleBuildType.Controller, AssetBundleBuildType.OverrideController, AssetBundleBuildType.Scene ,AssetBundleBuildType.Font};

        /// <summary>
        /// 必须依赖别人的资源
        /// </summary>
        public static AssetBundleBuildType[] mustDependency = new AssetBundleBuildType[] { AssetBundleBuildType.Script };

        /// <summary>
        /// 当前资源所在的结点
        /// </summary>
        public AssetDependenctGraph.Node selfNode;

        /// <summary>
        /// 当前资源的GUID
        /// </summary>
        public string guid;

        /// <summary>
        /// 资源类型 
        /// 如果想要让某一个类型的资源打成一个包 譬如 所有的shader都打成一个包.（这个功能只支持在TreeType为Single的模式）
        /// 那么需要计算深度 按照最深度的那个来.
        /// </summary>
        public AssetBundleBuildType type;

        /// <summary>
        /// 是否单独打包
        /// 如果一个资源不被单独打包 
        /// 深度就对于他来说 没有意义
        /// 因为push pop的时候 也不会处理
        /// </summary>
        public bool Alone = false;

        /// <summary>
        /// 人类干预的打包流程
        /// </summary>
        public bool alone_set = false;

        /// <summary>
        /// 是不是前面已经处理过了
        /// </summary>
        public bool isAlreadyProcess = false;

        /// <summary>
        /// 整个资源的引用树
        /// </summary>
        Dictionary<string, AssetDependenctGraph.Node> Tree;

        /// <summary>
        /// 被依赖的次数
        /// </summary>
        public int bedependencyCount = 0;

        /// <summary>
        /// 被别的资源包包含的次数
        /// </summary>
        public int alreadyBuildCount = 0;

        /// <summary>
        /// 资源大小
        /// </summary>
        public long size;

        /// <summary>
        /// 打包之后的资源位置
        /// </summary>
        //public string buildPath;

        /// <summary>
        /// Bundle的名字
        /// </summary>
        public string bundleName;

        /// <summary>
        /// 最后的修改时间
        /// </summary>
        public DateTime lastWriteTime;

        private SortedList<string, AssetBundleInfo> sortedBundleTree;

        /// <summary>
        /// Bundle的信息
        /// </summary>
        /// <param name="node"></param>
        /// <param name="allTree"></param>
        public AssetBundleInfo(AssetDependenctGraph.Node node, Dictionary<string, AssetDependenctGraph.Node> allTree, SortedList<string, AssetBundleInfo> __sortedBundleTree)
        {
            this.selfNode = node;

            Tree = allTree;

            sortedBundleTree = __sortedBundleTree;

            var info = new FileInfo(node.NodePath);

            //检测是否必须单独进行打包
            if (AssetPathRule.IsMustBuild(node.NodePath))
            {
                alone_set = true;
            }

            size = info.Length;

            lastWriteTime = info.LastWriteTime;

            //查询构建被引用了多少次
            BuildBeDependenctCount(this.selfNode);

            //this.guid = AssetDatabase.AssetPathToGUID(BuildUtils.GetAssetRelativePath(this.selfNode.NodePath));

            //guid = AssetDatabase.AssetPathToGUID(this.selftree.assetPath);
            //var extension = Path.GetExtension(this.selfNode.NodePath);

            //获取资源打包的类型数据
            type = AssetFileType.GetAssetBundleBuildType(this.selfNode.NodePath, Path.GetExtension(this.selfNode.NodePath));
        }

        /// <summary>
        /// 是否已经标记为被打包设置
        /// </summary>
        private bool markAsBuild = false;

        public void MarkAsBuild()
        {
            markAsBuild = true;
        }

        public bool AlreadyBuild()
        {
            return markAsBuild;
        }

        /// <summary>
        /// 查找自己被使用了几次
        /// </summary>
        public void BuildAlreadyBuildCount()
        {
            this.BuildAlreadyBuildCount(this.selfNode);
        }

        /// <summary>
        /// 查找自己被使用了几次.
        /// </summary>
        /// <param name="tempNode"></param>
        private void BuildAlreadyBuildCount(AssetDependenctGraph.Node tempNode)
        {
            for (int i = 0; i < tempNode.Parents.Count; i++)
            {
                AssetDependenctGraph.Node node;

                if (Tree.TryGetValue(tempNode.Parents[i], out node))
                {
                    AssetBundleInfo bundles;

                    var bundleKey = BuildUtils.GetPathDirWithoutExtension(tempNode.Parents[i]);

                    if (sortedBundleTree.TryGetValue(bundleKey, out bundles))
                    {
                        if (bundles.AlreadyBuild())// && Mathf.Abs(bundles.selfNode.Depth - this.selfNode.Depth) == 1)
                        {
                            alreadyBuildCount++;
                            continue;
                        }
                    }
                    if (node.Parents != null && node.Parents.Count > 0)
                    {
                        //继续上走
                        BuildAlreadyBuildCount(node);
                    }
                }
            }
        }


        /// <summary>
        /// 查找自己被使用了几次.
        /// </summary>
        /// <param name="tempNode"></param>
        public void BuildBeDependenctCount(AssetDependenctGraph.Node tempNode)
        {
            for (int i = 0; i < tempNode.Parents.Count; i++)
            {
                AssetDependenctGraph.Node node;

                if (Tree.TryGetValue(tempNode.Parents[i], out node))
                {
                    if (node.Parents != null && node.Parents.Count > 0)
                    {
                        //继续上走
                        BuildBeDependenctCount(node);
                        bedependencyCount++;
                    }
                    else
                    {
                        bedependencyCount++;
                        //统计+1
                    }
                }
            }
        }

        /// <summary>
        /// 是否会被单独打包
        /// </summary>
        /// <returns></returns>
        public bool BuildAlone()
        {
            //资源单独打包设置
            if (alone_set)
                return true;

            //是否为目录资源打包
            if (IsDirectoryBuild(type))
                return false;

            //单独打包的情况（多个打成一个包）
            if (IndexOf(OnePackageTypes, type) != -1)
            {
                return false;
            }

            //必须要依赖别人的资源 自己的话 宁可不打
            if (IndexOf(mustDependency, type) != -1)
                return false;

            ////是否为必须单独进行打包的类型，如果是那么就进行单独打包
            //var mustAloneIndex = IndexOf(mustAlone, this.type);
            //if (mustAloneIndex != -1 && this.selfNode.Parents.Count == 0)
            //{
            //    return true;
            //}
            //else if (mustAloneIndex != -1)
            //{
            //    return true;
            //}

            //是否为必须单独进行打包的类型，如果是那么就进行单独打包
            var mustAloneIndex = IndexOf(mustAlone, this.type);
            if (mustAloneIndex != -1)
            {
                return true;
            }

            //如果一个资源没有父结点，是最上层，那么也需要进行单独打包
            if (this.selfNode.Parents.Count == 0)
                return true;

            //资源被多次引用，那么也进行单独打包
            if (alreadyBuildCount > 1)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 构建其他Child包 
        /// </summary>
        /// <param name="withSubAssets">是否在每个资源中包含依赖资源</param>
        internal void BuildAllUnusedAssets(bool withSubAssets = true)
        {
            if (withSubAssets)
            {
                return;//TODO 允许资源重复打包
            }
            for (int i = 0; i < selfNode.AllChildren.Length; ++i)
            {
                string subFileName = selfNode.AllChildren[i];

                if (!string.IsNullOrEmpty(subFileName))
                {
                    AssetImporter importer = AssetImporter.GetAtPath(subFileName);

                    BuildUtils.SetAssetBundleName(importer, this.bundleName);
                }
            }
        }

        public static int IndexOf(AssetBundleBuildType[] bundles, AssetBundleBuildType type)
        {
            for (int i = 0; i < bundles.Length; i++)
            {
                if (bundles[i] == type)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// 是否为目录资源打包
        /// </summary>
        /// <returns></returns>
        public static bool IsDirectoryBuild(AssetBundleBuildType type)
        {
            for (int i = 0; i < DirectoryTypes.Length; i++)
            {
                if (DirectoryTypes[i] == type)
                    return true;
            }

            return false;
        }
    }
}