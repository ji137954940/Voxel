using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AFrame.AssetCheck
{
	/// <summary>
	/// 贴图Overridden选项检查
	/// @author LiuLeiLei
	/// @data 3/2/2018
	/// @desc 
	/// </summary>
    [AssetCheck(checkType = CheckEnumType.Texture,option = CheckOption.Texture_Overridden,desc = "贴图Overridden选项检查")]
	public class TextureOverridden : BaseCheck
    {
        public override bool OnCheck(Object _obj, string _path, AssetImporter _importer)
        {
            var texImporter = _importer as TextureImporter;
            if (texImporter)
            {
                if (texImporter.textureType == TextureImporterType.Lightmap)
                    return true;

                TextureImporterPlatformSettings android = texImporter.GetPlatformTextureSettings("Android");
                TextureImporterPlatformSettings ios = texImporter.GetPlatformTextureSettings("iPhone");

                if(!android.overridden || !ios.overridden)
                {
                    return false;
                }
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

                TextureImporterPlatformSettings android = texImporter.GetPlatformTextureSettings("Android");
                TextureImporterPlatformSettings ios = texImporter.GetPlatformTextureSettings("iPhone");

                android.overridden = true;
                ios.overridden = true;
            }

            return base.OnFormat(_obj, _path, _importer);
        }

        public override string OnCheckMessage()
        {
            return "贴图Overridden选项未开启";
        }
    }
}