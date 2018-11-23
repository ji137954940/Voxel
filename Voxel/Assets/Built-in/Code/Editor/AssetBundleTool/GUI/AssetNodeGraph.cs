using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tgame.AssetBundle
{

    public class AssetNodeGraph
    {
        /// <summary>
        /// 当前结构
        /// </summary>
        public AssetDependenctGraph.Node tree;

        /// <summary>
        /// 总的结构
        /// </summary>
        public Dictionary<string, AssetDependenctGraph.Node> trees;

        public const int NodeWidth = 200;

        public const int NodeHeight = 130;

        public const int CellWidth = NodeWidth + 20;

        public const int CellHeight = NodeHeight + 20;

        public int CellOffsetY = 100;

        public int CellOffsetX = 100;

        public Rect[] rects;

        public int id = 0;

        public int maxHeight = 0;

        public AssetNodeGraph(Dictionary<string, AssetDependenctGraph.Node> roots, AssetDependenctGraph.Node current)
        {
            trees = roots;

            tree = current;

            rects = new Rect[tree.Parents.Count + tree.AllChildren.Length + 1];
        }

        /// <summary>
        /// 绘制
        /// </summary>
        public void Draw()
        {
            id = 0;

            //按照哪个算的问题
            var count = tree.Parents.Count > tree.AllChildren.Length ? tree.Parents.Count : tree.AllChildren.Length;

            //var maxWidth = CellOffsetX + CellWidth * 3;

            var maxHeight = CellOffsetY + count * CellHeight;

            this.maxHeight = maxHeight;

            var parentOffsetY = CellOffsetY + ((count - tree.Parents.Count) * CellHeight) >> 1;

            //当前资源的位置
            var selfOffsetY = CellOffsetY + ((count - 1) * CellHeight) >> 1;

            var selfRect = rects[tree.Parents.Count + tree.AllChildren.Length] = new Rect(CellOffsetX + CellWidth, selfOffsetY, NodeWidth, NodeHeight + 50);

            //首先绘制线 然后绘制父亲的框
            for (int i = 0; i < tree.Parents.Count; i++)
            {
                var tempid = i;

                if (rects[tempid].center == Vector2.zero)
                    rects[tempid] = new Rect(CellOffsetX, parentOffsetY + (i * CellHeight), NodeWidth, NodeHeight);

                DrawNodeCurve(rects[tempid], selfRect);

                var index = i;

                rects[tempid] = GUI.Window(i, rects[tempid], winid =>
                {
                    GUILayout.Label(tree.Parents[index]);

                    var asset = AssetDatabase.LoadMainAssetAtPath(tree.Parents[index]);

                    if (asset != null)
                    {
                        EditorGUILayout.ObjectField(asset, asset.GetType(), false);
                    }
                    else
                    {
                        Debug.LogError(tree.Parents[index]);
                    }

                    GUI.DragWindow();

                }, new GUIContent("parent", tree.Parents[index]));
            }

            var childOffsetY = CellOffsetY + ((count - tree.AllChildren.Length) * CellHeight) >> 1;

            id += tree.Parents.Count;

            for (int i = 0; i < tree.AllChildren.Length; i++)
            {
                var tempid = id + i;

                if (rects[tempid].center == Vector2.zero)

                    rects[tempid] = new Rect(CellOffsetX + CellWidth * 2, childOffsetY + (i * CellHeight), NodeWidth, NodeHeight);

                DrawNodeCurve(selfRect, rects[tempid]);

                var index = i;

                rects[tempid] = GUI.Window(id + i, rects[tempid], winid =>
                {
                    GUILayout.Label(tree.AllChildren[index]);

                    var asset = AssetDatabase.LoadMainAssetAtPath(tree.AllChildren[index]);
                    if (asset != null)
                    {
                        EditorGUILayout.ObjectField(asset, asset.GetType(), false);
                    }
                    else
                    {
                        Debug.LogError(tree.AllChildren[index]);
                    }

                    GUI.DragWindow();

                }, new GUIContent("child", tree.AllChildren[index]));

            }

            var selfid = id + tree.AllChildren.Length;

            if (rects[selfid].center != Vector2.zero)
            {
                rects[selfid] = GUI.Window(selfid, rects[selfid], winid =>
                {
                    GUILayout.Label("当前深度为" + this.tree.Depth + " " + this.tree.PositiveDepth + "  最大深度为  " + AssetDependenctGraph.Node.MaxDepth);

                    GUILayout.Label(this.tree.NodePath);

                    var asset = AssetDatabase.LoadMainAssetAtPath(this.tree.NodePath);

                    if (asset != null)
                    {
                        EditorGUILayout.ObjectField(asset, asset.GetType(), false);
                    }
                    else
                    {
                        Debug.LogError(this.tree.NodePath);
                    }
                    GUI.DragWindow();

                }, new GUIContent("self", this.tree.NodePath));
            }
        }


        /// <summary>
        /// 画贝塞尔曲线
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        internal static void DrawNodeCurve(Rect start, Rect end)
        {
            DrawNodeCurve(start, end, new Vector2(0.5f, 0.5f), new Vector2(0.0f, 0.5f));
        }

        /// <summary>
        /// 画贝塞尔曲线
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="vStartPercentage"></param>
        /// <param name="vEndPercentage"></param>
        internal static void DrawNodeCurve(Rect start, Rect end, Vector2 vStartPercentage, Vector2 vEndPercentage)
        {
            Vector3 startPos = new Vector3(start.x + start.width * vStartPercentage.x, start.y + start.height * vStartPercentage.y, 0);

            Vector3 endPos = new Vector3(end.x + end.width * vEndPercentage.x, end.y + end.height * vEndPercentage.y, 0);

            Vector3 startTan = startPos + Vector3.right * (-50 + 100 * vStartPercentage.x) + Vector3.up * (-50 + 100 * vStartPercentage.y);

            Vector3 endTan = endPos + Vector3.right * (-50 + 100 * vEndPercentage.x) + Vector3.up * (-50 + 100 * vEndPercentage.y);

            UnityEngine.Color shadowCol = new UnityEngine.Color(0, 0, 0, 0.06f);

            for (int i = 0; i < 3; i++) // Draw a shadow

                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);

            Handles.DrawBezier(startPos, endPos, startTan, endTan, UnityEngine.Color.black, null, 2);

        }
    }
}