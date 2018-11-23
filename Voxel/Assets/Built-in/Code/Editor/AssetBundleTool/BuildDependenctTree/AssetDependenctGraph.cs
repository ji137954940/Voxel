using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tgame.AssetBundle
{

    /// <summary>
    /// 资源依赖关系分析图
    /// </summary>
    public class AssetDependenctGraph
    {
        /// <summary>
        /// 所有的未经过处理的资源引用数据
        /// </summary>
        private Dictionary<string, string[]> asset_node;


        /// <summary>
        /// 正向关系 临时数据（从树干到树叶）
        /// </summary>
        private Dictionary<string, List<string>> positiveInclusion;

        /// <summary>
        /// 反向关系 临时数据（从树叶到树干）
        /// </summary>
        private Dictionary<string, List<string>> reverseInclusion;

        /// <summary>
        /// 分析之后结果数据
        /// </summary>
        public Dictionary<string, Node> result;

        /// <summary>
        /// 构建数据结构
        /// </summary>
        /// <param name="asset_node"></param>
        public AssetDependenctGraph(Dictionary<string, string[]> asset_node)
        {
            this.asset_node = asset_node;

            BuildPositiveAndReverseInclusion();

            BuildTree();

            MarkSureParent();

            BuildDepth();
        }

        /// <summary>
        /// 构建深度
        /// 之前构建的是反向深度 现在修正为正向的
        /// </summary>
        private void BuildDepth()
        {
            var ie = result.GetEnumerator();

            while (ie.MoveNext())
            {
                if (ie.Current.Value.Parents.Count == 0)
                {
                    ie.Current.Value.BuildDepth();
                }
            }
        }

        /// <summary>
        /// 确定真正的父节点数据
        /// </summary>
        private void MarkSureParent()
        {
            var ie = result.GetEnumerator();

            while (ie.MoveNext())
            {
                ie.Current.Value.MarkSureParent();
            }
        }

        /// <summary>
        /// 构建正向和反向的数据关系
        /// </summary>
        private void BuildPositiveAndReverseInclusion()
        {
            positiveInclusion = new Dictionary<string, List<string>>();

            reverseInclusion = new Dictionary<string, List<string>>();

            var ie = asset_node.GetEnumerator();

            while (ie.MoveNext())
            {
                //存储正向引用关系
                positiveInclusion.Add(ie.Current.Key, new List<string>(ie.Current.Value));

                //存储反向引用关系
                for (int i = 0; i < ie.Current.Value.Length; i++)
                {
                    if (!reverseInclusion.ContainsKey(ie.Current.Value[i]))
                    {
                        reverseInclusion.Add(ie.Current.Value[i], new List<string> { ie.Current.Key });
                    }
                    else
                    {
                        reverseInclusion[ie.Current.Value[i]].Add(ie.Current.Key);
                    }
                }
            }
        }

        /// <summary>
        /// 砍成树形结构
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, Node> BuildTree()
        {
            //构建之后的结果
            //result = result ?? new Dictionary<string, Node>();
            if (result == null)
                result = new Dictionary<string, Node>();

            //叶子堆栈
            Stack<string> leafStack = new Stack<string>();

            //结点深度
            int depth = 0;

            ////开始计算时间
            //var beginTime = DateTime.Now;

            //while (reverseInclusion.Count > 0 && DateTime.Now - beginTime < TimeSpan.FromSeconds(1))
            while (reverseInclusion.Count > 0)
            {
                //Debug.Log(reverseInclusion.Count);

                var positiveIe = positiveInclusion.GetEnumerator();

                string leaf = string.Empty;

                depth++;

                while (positiveIe.MoveNext()) //正向遍历
                {
                    if (positiveIe.Current.Value.Count == 1)//叶子节点
                    {
                        leaf = positiveIe.Current.Key;

                        List<string> guessParents;

                        if (reverseInclusion.TryGetValue(leaf, out guessParents))
                        {
                            for (int i = 0; i < guessParents.Count; i++)
                            {
                                Node node;

                                var guessParent = guessParents[i];

                                if ((guessParents.Count == 1) || (guessParents.Count > 1 && guessParent != leaf))
                                {
                                    if (!result.TryGetValue(leaf, out node))
                                    {
                                        node = new Node(leaf, result);

                                        node.Depth = depth;

                                        node.PositiveDepth = depth;

                                        Node.MaxDepth = Mathf.Max(node.Depth, Node.MaxDepth);

                                        node.SetChildren(asset_node[leaf]);

                                        result.Add(leaf, node);
                                    }

                                    node.AddGuessParent(guessParent);
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("This is impossible in theory");
                        }

                        leafStack.Push(leaf);
                    }
                }

                while (leafStack.Count > 0) //剪掉叶子
                {
                    List<string> containParents1;

                    leaf = leafStack.Pop();

                    if (reverseInclusion.TryGetValue(leaf, out containParents1))
                    {
                        for (int i = 0; i < containParents1.Count; i++)
                        {
                            var containParent = containParents1[i];

                            positiveInclusion[containParent].Remove(leaf);

                            if (positiveInclusion[containParent].Count == 0)
                            {
                                positiveInclusion.Remove(containParent);
                            }
                        }

                        reverseInclusion.Remove(leaf);
                    }
                }
            }

            if (reverseInclusion.Count > 0)
            {
                Debug.Log("Some Resources did not deleted.");

                foreach (var item in reverseInclusion)
                {
                    for (int i = 0; i < item.Value.Count; i++)
                    {
                        Debug.LogFormat("reverseInclusion __Current Key: {0},Count:{1} who was reference by: {2}", item.Key, item.Value.Count, item.Value[i]);
                    }
                }

                foreach (var item in positiveInclusion)
                {
                    for (int i = 0; i < item.Value.Count; i++)
                    {
                        Debug.LogFormat("positiveInclusion __Current Key: {0},Count:{1} reference:{2}", item.Key, item.Value.Count, item.Value[i]);
                    }
                }
            }

            foreach (var asset in asset_node)
            {
                Node node = null;

                if (result.TryGetValue(asset.Key, out node))
                {
                    node.RemoveParentAndChildSelf();
                }
            }

            return result;
        }

        /// <summary>
        /// 资源节点
        /// </summary>
        public class Node
        {
            public static int MaxDepth;
            /// <summary>
            /// 当前资源的path
            /// </summary>
            private string currentNodePath;

            private Dictionary<string, Node> globalNode;

            private int positiveDepth;

            /// <summary>
            /// 正向深度
            /// </summary>
            public int PositiveDepth
            {
                set { positiveDepth = value; }
                get { return positiveDepth; }
            }

            /// <summary>
            /// 深度
            /// </summary>
            private int depth;
            public int Depth
            {
                set { depth = value; }
                get { return depth; }
            }

            /// <summary>
            /// NodePath
            /// </summary>
            public string NodePath { get { return currentNodePath; } }

            /// <summary>
            /// Parent
            /// </summary>
            private List<string> parents;
            public List<string> Parents { get { return parents; } }

            /// <summary>
            /// 所有引用的数据
            /// </summary>
            private string[] allChildren;
            public string[] AllChildren { get { return allChildren; } }

            /// <summary>
            /// 引用的资源数据
            /// </summary>
            private HashSet<string> childrenSet;

            /// <summary>
            /// 被一用的资源数据
            /// </summary>
            private HashSet<string> parentSet;

            public Node(string __current, Dictionary<string, Node> __globalNode)
            {
                this.currentNodePath = __current;

                parents = new List<string>();

                globalNode = __globalNode;
            }

            /// <summary>
            /// 检测当前文件是否为shader文件
            /// </summary>
            /// <returns></returns>
            public bool IsShader()
            {
                if (string.IsNullOrEmpty(NodePath))
                    return false;

                string extension = BuildUtils.GetPathExtension(NodePath);

                if (AssetFileType.GetAssetBundleBuildType(NodePath, extension) == AssetBundleBuildType.Shader)
                    return true;
                return false;
            }

            public void SetChildren(string[] __children)
            {
                this.allChildren = __children;

                childrenSet = new HashSet<string>();

                for (int i = 0; i < allChildren.Length; i++)
                {
                    childrenSet.Add(allChildren[i]);
                }
            }

            public void AddGuessParent(string __parent)
            {
                parents.Add(__parent);

                if (parentSet == null)
                {
                    parentSet = new HashSet<string>();
                }

                parentSet.Add(__parent);
            }

            /// <summary>
            /// 检查他的parent是否在其他的parent的children里面
            /// 如果存在 那么需要在parent中删除其他的parent
            /// </summary>
            public void MarkSureParent()
            {
                for (int j = parents.Count - 1; j >= 0; j--)
                {
                    var parentj = parents[j];

                    for (int z = parents.Count - 1; z >= 0; z--)
                    {
                        if (j != z)
                        {
                            var parentz = parents[z];

                            Node node;

                            if (globalNode.TryGetValue(parentz, out node))
                            {
                                if (node.ContainsChildren(parentj))
                                {
                                    //需要从parents中删除parentz
                                    parentSet.Remove(parentz);
                                }
                            }
                        }
                    }
                }

                parents = new List<string>(parentSet.Count);

                foreach (var item in parentSet)
                {
                    parents.Add(item);
                }
            }

            /// <summary>
            /// 是否包含某一个child
            /// </summary>
            /// <param name="parentj"></param>
            /// <returns></returns>
            private bool ContainsChildren(string child)
            {
                if (childrenSet.Contains(child))
                {
                    return true;
                }
                return false;
            }

            /// <summary>
            /// 构建组织深度
            /// </summary>
            internal void BuildDepth()
            { 
                var maxNodeDepth = 0;

                for (int i = 0; i < allChildren.Length; i++)
                {
                    var assetPath = allChildren[i];

                    Node childNode;

                    if (globalNode.TryGetValue(assetPath, out childNode))
                    {
                        maxNodeDepth = Mathf.Max(maxNodeDepth, childNode.Depth);
                    }
                }

                for (int i = 0; i < allChildren.Length; i++)
                {
                    var assetPath = allChildren[i];

                    Node childNode;

                    if (assetPath != currentNodePath && globalNode.TryGetValue(assetPath, out childNode))
                    {
                        childNode.Depth = Mathf.Max(childNode.Depth, maxNodeDepth - childNode.Depth);
                    }
                }

                MaxDepth = Mathf.Max(MaxDepth, maxNodeDepth);
            }

            /// <summary>
            /// 在parent和children中移除自己
            /// </summary>
            internal void RemoveParentAndChildSelf()
            {
                parents.Remove(currentNodePath);

                parentSet.Remove(currentNodePath);

                allChildren = ArrayTools.Remove(allChildren, currentNodePath);
            }
        }
    }
}
