using AFrame.Table;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Tgame.AssetBundle;
using UnityEditor;
using UnityEngine;

namespace AFrame.AssetCheck
{
    /// <summary>
    /// 资源导入检查Log窗口
    /// @author LiuLeiLei
    /// @data 3/8/2018
    /// @desc 
    /// </summary>
    public class LogTableViewWindow : EditorWindow
    {
        [MenuItem("AFrame/资源导入检查Log窗口", priority = 101)]
        public static void OpenWindow()
        {
            OpenTableViewWindow();
        }

        /// <summary>
        /// 打开log window
        /// </summary>
        /// <param name="_dataList"></param>
        public static void OpenTableViewWindow(List<AssetCheckData> _dataList = null)
        {
            dataList = _dataList;

            var window = GetWindow<LogTableViewWindow>("不合理资源列表");
            window.Show();

            window.OnTableRefresh(_dataList);
        }

        public class LogGUIStyle
        {
            public readonly GUIStyle ToolBar = "ToolBar";

            public readonly GUIStyle TE_ToolBarBtn = "TE toolbarbutton";
        }

        private static List<AssetCheckData> dataList;

        private TableView table;

        private LogGUIStyle guiStyle;

        private float offset = 25;

        /// <summary>
        /// 是否开启检查
        /// </summary>
        private bool openCheck;

        private void OnEnable()
        {
            table = new TableView(this, typeof(AssetCheckData));
            table.AddColum("resourcePath", "Path", 0.5f, TextAnchor.MiddleLeft);
            table.AddColum("logMsg", "CheckFailMsg", 0.5f, TextAnchor.MiddleLeft);
            table.onSelected += TableOnSelected;

            OnTableRefresh(dataList);
            
            openCheck = AssetCheckTool.OwnCheckOpen();
        }

        /// <summary>
        /// 表格条目点击回调
        /// </summary>
        /// <param name="_obj"></param>
        /// <param name="_col"></param>
        private void TableOnSelected(object _obj, int _col)
        {
            if (_obj != null)
            {
                AssetCheckData checkData = _obj as AssetCheckData;

                EditorGUIUtility.PingObject(checkData.obj);

                Selection.activeObject = checkData.obj;
            }
        }

        private void OnTableRefresh(List<AssetCheckData> _list)
        {
            if (table != null && _list != null)
                table.RefreshData(EditorCommon.EditorTool.ToObjectList(dataList));
        }

        private void OnGUI()
        {
            if(guiStyle == null)
                guiStyle = new LogGUIStyle();

            Rect rect = new Rect(0, 0, this.position.width, this.position.height);
            GUILayout.BeginArea(rect);

            GUILayout.BeginHorizontal(guiStyle.ToolBar,GUILayout.Width(rect.width));
            {
                GUILayout.Label(new GUIContent("资源导入检查开关:"),GUILayout.Width(200));

                EditorGUI.BeginChangeCheck();

                openCheck = GUILayout.Toggle(openCheck,openCheck ? "已开启" : "已关闭",new GUILayoutOption[] { GUILayout.Width(200) } );

                if (EditorGUI.EndChangeCheck())
                {
                    //Debug.Log("openCheck == " + openCheck);
                    AssetCheckTool.SetOwnCheckOpenState(openCheck);
                }
            }
            GUILayout.EndHorizontal();

            Rect tableRect = new Rect(0, offset,rect.width,rect.height - offset);
            table.Draw(tableRect);

            GUILayout.EndArea();
        }
    }
}