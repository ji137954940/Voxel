using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AFrame.AssetCheck
{
	/// <summary>
	/// 检查不符合规定尺寸的贴图
	/// @author LiuLeiLei
	/// @data 3/2/2018
	/// @desc 
	/// </summary>
    [AssetCheck(checkType = CheckEnumType.Texture,option = CheckOption.Texture_MaxSize,desc = "检查不符合规定尺寸的贴图")]
	public class TextureMaxSize : BaseCheck
	{
        private int texMaxSize = 1024;

        public override bool OnCheck(Object _obj, string _path, AssetImporter _importer)
        {
            TextureImporter texImporter = _importer as TextureImporter;
            if (texImporter)
            {
                //TODO 尺寸应该走配置
                if (texImporter.maxTextureSize > texMaxSize)
                    return false;
            }
            
            return base.OnCheck(_obj, _path, _importer);
        }

        public override bool OnFormat(Object _obj, string _path, AssetImporter _importer)
        {
            return false;
        }

        public override string OnCheckMessage()
        {
            return string.Format("贴图尺寸大于{0};", texMaxSize);
        }

        public override string OnFormatMessage()
        {
            return "贴图尺寸需要手动修改";
        }
    }
}