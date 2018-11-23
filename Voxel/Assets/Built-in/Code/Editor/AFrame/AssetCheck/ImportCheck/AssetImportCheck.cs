using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AFrame.AssetCheck
{
	/// <summary>
	/// 资源导入检查
	/// @author LiuLeiLei
	/// @data 3/8/2018
	/// @desc 
	/// </summary>
	public class AssetImportCheck : AssetPostprocessor
    {
        /// <summary>
        /// 资源导入后处理
        /// </summary>
        /// <param name="importedAssets"></param>
        /// <param name="deletedAssets"></param>
        /// <param name="movedAssets"></param>
        /// <param name="movedFromAssetPaths"></param>
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            bool checkOpen = AssetCheckTool.OwnCheckOpen();
            if (!checkOpen)
                return;

            List<string> pathList = new List<string>();

            pathList.AddRange(importedAssets);
            pathList.AddRange(movedAssets);
            
            int pathCount = pathList.Count;
            if (pathCount > 0)
            {
                AssetCheckManager manager = new AssetCheckManager();
                manager.OnEnable();
                
                manager.SetPathList(pathList);
                manager.SetIncludeCheckTypeList(manager.allEnumArr.ToList());
                manager.SetIncludeOptionList(manager.allOptionArr.ToList());
                manager.onCheckEndCallBack = (() =>
                {
                    int dataCount = manager.GetCheckDataList().Count;
                    if(dataCount > 0)
                    {
                        LogTableViewWindow.OpenTableViewWindow(manager.GetCheckDataList());
                    }
                });
                manager.OnCheck();
            }
        }
    }
}