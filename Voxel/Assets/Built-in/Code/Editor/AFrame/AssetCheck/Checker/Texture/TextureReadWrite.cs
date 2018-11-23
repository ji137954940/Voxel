using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AFrame.AssetCheck
{
	/// <summary>
	/// 贴图Read/Write选项检查
	/// @author LiuLeiLei
	/// @data 3/2/2018
	/// @desc 
	/// </summary>
    [AssetCheck(checkType = CheckEnumType.Texture,option = CheckOption.Texture_ReadWrite, desc = "贴图Read/Write选项检查")]
	public class TextureReadWrite : BaseCheck
	{
        public override bool OnCheck(Object _obj, string _path, AssetImporter _importer)
        {
            var texImporter = _importer as TextureImporter;
            if (texImporter)
            {
                if (texImporter.textureType == TextureImporterType.Lightmap)
                    return true;

                if (texImporter.isReadable)
                    return false;
            }

            return base.OnCheck(_obj, _path, _importer);
        }

        public override bool OnFormat(Object _obj, string _path, AssetImporter _importer)
        {
            var texImporter = _importer as TextureImporter;
            if (texImporter)
            {
                if (texImporter.textureType == TextureImporterType.Lightmap)
                    return true;

                texImporter.isReadable = false;
                texImporter.SaveAndReimport();
            }

            return base.OnFormat(_obj, _path, _importer);
        }

        public override string OnCheckMessage()
        {
            return "Read/Write Enable";
        }
    }
}