using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace AFrame.AssetCheck
{
	/// <summary>
	/// 资源路径规范
	/// @author LiuLeiLei
	/// @data 3/6/2018
	/// @desc 
	/// </summary>
    [AssetCheck(checkType = CheckEnumType.UnKnow,option = CheckOption.ResourcePath, desc = "资源路径规范")]
	public class ResourcePath : BaseCheck
	{
        public override bool OnCheck(Object _obj, string _path, AssetImporter _importer)
        {
            //是否包含中文字符
            bool hasChinese = Regex.IsMatch(_path, @"[\u4e00-\u9fa5]");
            //是否包含特殊字符
            bool hasSpecial = Regex.IsMatch(_path, "[^/[/]/?/*]+");
            //是否包含空格
            bool hasSpace = _path.IndexOf(" ") != -1;

            return !hasChinese && !hasSpecial && !hasSpace;
        }

        public override string OnCheckMessage()
        {
            return "路径中不允许存在中文字符、空格或其它特殊字符";
        }
    }
}