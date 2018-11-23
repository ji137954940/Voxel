using AFrame.Core;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AFrame.AssetCheck
{
    /// <summary>
    /// 资源检查界面检查类型GUI
    /// @author LiuLeiLei
    /// @data 3/6/2018
    /// @desc 
    /// </summary>
    public class AssetCheckWinItem : BreadCrumbElement
    {
        /// <summary>
        /// window
        /// </summary>
        private AssetCheckWindow checkWindow;
        /// <summary>
        /// 检查项数据list
        /// </summary>
        private List<AssetItemWinData> winDataList = new List<AssetItemWinData>();
 
        private Vector2 itemScrollPos;

        public AssetCheckWinItem(AssetCheckWindow _window, object _target, string _name = ""):base(_target,_name)
        {
            checkWindow = _window;
            winDataList = target as List<AssetItemWinData>;
        }

        public void OnGUI(Rect _rect)
        {
            GUILayout.BeginArea(_rect/*, new GUIContent(name), checkWindow.checkStyle.Window*/);

            GUILayout.BeginVertical();

            GUILayout.Label("Options:");

            itemScrollPos = GUILayout.BeginScrollView(itemScrollPos);

            int winDataCount = winDataList.Count;
            for(int i = 0;i < winDataCount;i ++)
            {
                EditorGUI.BeginChangeCheck();
                var winData = winDataList[i];
                winData.select = GUILayout.Toggle(winData.select, new GUIContent(winData.desc));
                if (EditorGUI.EndChangeCheck())
                {
                    checkWindow.SetOptionList(winData.checkOption, winData.select);
                }
            }

            GUILayout.EndScrollView();

            GUILayout.EndVertical();

            GUILayout.EndArea();
        }
	}
}