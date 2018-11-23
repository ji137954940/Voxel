using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AFrame.AssetCheck
{
	/// <summary>
	/// 贴图Mipmap检查
	/// @author LiuLeiLei
	/// @data 3/2/2018
	/// @desc 
	/// </summary>
    [AssetCheck(checkType = CheckEnumType.Texture,option = CheckOption.Texture_Mipmap,desc = "贴图Mipmap检查")]
	public class TextureMipmap : BaseCheck
	{
        public override bool OnCheck(Object _obj, string _path, AssetImporter _importer)
        {
            TextureImporter texImporter = _importer as TextureImporter;
            if (texImporter)
            {
                if (texImporter.mipmapEnabled)
                    return false;
            }
            
            return base.OnCheck(_obj, _path, _importer);
        }

        public override bool OnFormat(Object _obj, string _path, AssetImporter _importer)
        {
            TextureImporter texImporter = _importer as TextureImporter;
            if (texImporter)
            {
                texImporter.mipmapEnabled = false;
                texImporter.SaveAndReimport();
            }
            
            return base.OnFormat(_obj, _path, _importer);
        }

        public override string OnCheckMessage()
        {
            return "Mipmap Enable";
        }
    }
}