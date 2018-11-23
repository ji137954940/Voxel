using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AFrame.AssetCheck
{
    /// <summary>
    /// 模型 MeshCompression 检查
    /// @author LiuLeiLei
    /// @data 3/2/2018
    /// @desc 
    /// </summary>
    [AssetCheck(checkType = CheckEnumType.Model, option = CheckOption.Model_MeshCompression,desc = "模型 MeshCompression 检查")]
    public class ModelMeshCompression : BaseCheck
    {
        /// <summary>
        /// 模型压缩格式
        /// </summary>
        private ModelImporterMeshCompression meshCompression = ModelImporterMeshCompression.High;

        public override bool OnCheck(Object _obj, string _path, AssetImporter _importer)
        {
            var modelImporter = _importer as ModelImporter;
            if (modelImporter)
            {
                //TODO 这里是否有必要改为读取配置
                if (modelImporter.meshCompression != meshCompression)
                    return false;
            }

            return base.OnCheck(_obj, _path, _importer);
        }

        public override bool OnFormat(Object _obj, string _path, AssetImporter _importer)
        {
            var modelImporter = _importer as ModelImporter;
            if (modelImporter)
            {
                modelImporter.meshCompression = meshCompression;
                modelImporter.SaveAndReimport();
            }

            return base.OnFormat(_obj, _path, _importer);
        }

        public override string OnCheckMessage()
        {
            return "MeshCompression 应为 High";
        }
    }
}