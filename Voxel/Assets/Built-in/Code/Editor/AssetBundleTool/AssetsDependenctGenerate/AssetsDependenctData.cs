using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Tgame.AssetBundle
{
    /// <summary>
    /// 资源引用关系数据
    /// </summary>
    public class AssetsDependenctData : ScriptableObject
    {

        public AssetsDependenct data;

        /// <summary>
        /// 修复所有的关联关系
        /// </summary>
        public void FixedDirty()
        {
            foreach (var i in data.GetDic())
            {
                if (i.Value.isdirty)
                {
                    if (File.Exists(i.Key))
                    {
                        i.Value.dependencies = BuildUtils.ExceptScriptAndDll(BuildUtils.GetDependencies(i.Key));

                        i.Value.isdirty = false;
                    }
                    else
                    {
                        Debug.Log(i.Key + " is delete!");
                    }
                }
            }

            EditorUtility.SetDirty(this);
        }
    }
}