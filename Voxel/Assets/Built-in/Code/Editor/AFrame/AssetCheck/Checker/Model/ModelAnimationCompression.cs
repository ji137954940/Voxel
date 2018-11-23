using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AFrame.AssetCheck
{
	/// <summary>
	/// 模型 AnimationCompression 检查
	/// @author LiuLeiLei
	/// @data 3/2/2018
	/// @desc 
	/// </summary>
    [AssetCheck(checkType = CheckEnumType.Model, option = CheckOption.Model_AnimationCompression,desc = "模型 AnimationCompression 检查")]
	public class ModelAnimationCompression : BaseCheck
	{
        private ModelImporterAnimationCompression animCompression = ModelImporterAnimationCompression.Optimal;

        public override bool OnCheck(Object _obj, string _path, AssetImporter _importer)
        {
            var modelImporter = _importer as ModelImporter;
            if (modelImporter)
            {
                //检查的都是带动画的文件
                if (modelImporter.importAnimation && modelImporter.animationCompression != animCompression)
                    return false;
            }

            return base.OnCheck(_obj, _path, _importer);
        }

        public override bool OnFormat(Object _obj, string _path, AssetImporter _importer)
        {
            var modelImporter = _importer as ModelImporter;
            if (modelImporter)
            {
                modelImporter.animationCompression = animCompression;
                modelImporter.SaveAndReimport();
            }

            return base.OnFormat(_obj, _path, _importer);
        }

        public override string OnCheckMessage()
        {
            return "模型 ModelImporterAnimationCompression 选项建议为 Optimal";
        }
    }
}