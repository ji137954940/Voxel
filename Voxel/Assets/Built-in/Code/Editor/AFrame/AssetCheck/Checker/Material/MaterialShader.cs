using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AFrame.AssetCheck
{
    /// <summary>
    /// 检测选中目录下材质球Shader
    /// @author LiuLeiLei
    /// @data 3/2/2018
    /// @desc 
    /// </summary>
    [AssetCheck(checkType = CheckEnumType.Material, option = CheckOption.Material_Shader,desc = "检测选中目录下材质球Shader")]
    public class MaterialShader : BaseCheck
	{
        private string shaderName;

        public override bool OnCheck(Object _obj, string _path, AssetImporter _importer)
        {
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(_path);
            if (mat == null)
                return true;

            bool contains = AssetCheckTool.ShaderNameEquils(mat.shader.name);

            shaderName = mat.shader.name;

            return !contains;
        }

        public override bool OnFormat(Object _obj, string _path, AssetImporter _importer)
        {
            return false;
        }

        public override string OnCheckMessage()
        {
            return string.Format("ShaderName is {0}", shaderName);
        }

        public override string OnFormatMessage()
        {
            return "Shader 需手动修改";
        }
    }
}