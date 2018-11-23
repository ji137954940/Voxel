using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AFrame.AssetCheck
{
    /// <summary>
    /// 模型 OptimizeGameObjects 开启检查
    /// @author LiuLeiLei
    /// @data 3/2/2018
    /// @desc 开启 OptimizeGameObjects 选项会优化模型
    /// </summary>
    [AssetCheck(checkType = CheckEnumType.Model, option = CheckOption.Model_OptimizeGameObjects,desc = "模型 OptimizeGameObjects 开启检查")]
    public class ModelOptimizeGameObjects : BaseCheck
	{
        public override bool OnCheck(Object _obj, string _path, AssetImporter _importer)
        {
            var modelImporter = _importer as ModelImporter;
            if (modelImporter)
            {
                //检查的都是带动画的文件
                if (modelImporter.importAnimation && !modelImporter.optimizeGameObjects)
                    return false;
            }

            return base.OnCheck(_obj, _path, _importer);
        }

        public override bool OnFormat(Object _obj, string _path, AssetImporter _importer)
        {
            var modelImporter = _importer as ModelImporter;
            if (modelImporter)
            {
                modelImporter.optimizeGameObjects = true;
                modelImporter.SaveAndReimport();
            }

            return base.OnFormat(_obj, _path, _importer);
        }

        public override string OnCheckMessage()
        {
            return "未激活 OptimizeGameObjects 选项，建议勾选此项";
        }
    }
}