using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AFrame.AssetCheck
{
    /// <summary>
    /// 查找出全部的 StandardShader Material
    /// @author LiuLeiLei
    /// @data 3/2/2018
    /// @desc 
    /// </summary>
    [AssetCheck(checkType = CheckEnumType.Material, option = CheckOption.Material_GetAllStandardShader,desc = "查找出全部的 StandardShader Material")]
    public class MaterialGetAllStandardShader : BaseCheck
	{
        private string standardStr = "Standard";

        public override bool OnCheck(Object _obj, string _path, AssetImporter _importer)
        {
            bool equils = false;

            Material mat = AssetDatabase.LoadAssetAtPath<Material>(_path);
            if (mat)
            {
                equils = string.Equals(mat.shader.name, standardStr);
            }

            return !equils;
        }

        public override bool OnFormat(Object _obj, string _path, AssetImporter _importer)
        {
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(_path);
            if (mat)
            {
                AssetDatabase.DeleteAsset(_path);
            }

            return base.OnFormat(_obj, _path, _importer);
        }

        public override string OnCheckMessage()
        {
            return "Standard Shader should be deleted";
        }
    }
}