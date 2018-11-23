using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AFrame.AssetCheck
{
	/// <summary>
	/// 贴图是否是2的次幂
	/// @author LiuLeiLei
	/// @data 3/2/2018
	/// @desc 
	/// </summary>
    [AssetCheck(checkType = CheckEnumType.Texture,option = CheckOption.Texture_PowerOf2,desc = "贴图是否是2的次幂")]
	public class TexturePowerOf2 : BaseCheck
	{
        public override bool OnCheck(Object _obj, string _path, AssetImporter _importer)
        {
            var texture = _obj as Texture;
            if (texture)
            {
                if (!PowOf2(texture.width) || !PowOf2(texture.height))
                    return false;
            }

            return base.OnCheck(_obj, _path, _importer);
        }

        public override bool OnFormat(Object _obj, string _path, AssetImporter _importer)
        {
            var texImporter = _importer as TextureImporter;
            if (texImporter)
            {
                texImporter.npotScale = TextureImporterNPOTScale.ToNearest;
                texImporter.SaveAndReimport();
            }

            return base.OnFormat(_obj, _path, _importer);
        }

        public override string OnCheckMessage()
        {
            return "贴图不是2的n次幂";
        }

        /// <summary>
        /// 是否为2的次幂
        /// </summary>
        /// <param name="_size"></param>
        /// <returns></returns>
        private bool PowOf2(int _size)
        {
            return (_size > 0) && ((_size & (_size - 1)) == 0);
        }
    }
}