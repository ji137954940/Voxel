using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AFrame.AssetCheck
{
    /// <summary>
    /// 设置 Npc LOD--1层
    /// @author LiuLeiLei
    /// @data 3/2/2018
    /// @desc 
    /// </summary>
    [AssetCheck(checkType = CheckEnumType.Model, option = CheckOption.Model_LOD,desc = "设置 Npc LOD--1层")]
    public class ModelLOD : BaseCheck
	{
        public override bool OnFormat(Object _obj, string _path, AssetImporter _importer)
        {
            var modelImporter = _importer as ModelImporter;
            if (modelImporter)
            {
                var prefabObj = _obj as GameObject;
                var bodyctrl = prefabObj.transform.Find("bodyctrl");

                LODGroup lodGroup = bodyctrl.GetComponent<LODGroup>();
                if (lodGroup == null)
                    lodGroup = bodyctrl.gameObject.AddComponent<LODGroup>();

                LOD[] lodArr = new LOD[1];
                lodArr[0].screenRelativeTransitionHeight = 0;
                lodArr[0].renderers = bodyctrl.GetComponentsInChildren<Renderer>();
                lodGroup.SetLODs(lodArr);

                modelImporter.SaveAndReimport();
            }

            return base.OnFormat(_obj, _path, _importer);
        }
    }
}