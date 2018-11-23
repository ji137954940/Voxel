using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AFrame.AssetCheck
{
    /// <summary>
    /// 投影组件检查
    /// @author LiuLeiLei
    /// @data 3/2/2018
    /// @desc 
    /// </summary>
    [AssetCheck(checkType = CheckEnumType.Projector, option = CheckOption.Projector_Check,desc = "投影组件检查")]
	public class ProjectorCheck : BaseCheck
	{
        public override bool OnCheck(Object _obj, string _path, AssetImporter _importer)
        {
            GameObject gameObj = _obj as GameObject;
            if (gameObj != null)
            {
                Projector[] projectorArr = gameObj.GetComponentsInChildren<Projector>();
                if (projectorArr != null && projectorArr.Length > 0)
                {
                    return false;
                }
            }

            return base.OnCheck(_obj, _path, _importer);
        }

        public override string OnCheckMessage()
        {
            return "Projector 组件应去掉";
        }
    }
}