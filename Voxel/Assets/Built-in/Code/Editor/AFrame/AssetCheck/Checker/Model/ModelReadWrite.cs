using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AFrame.AssetCheck
{
	/// <summary>
	/// 模型 Read/Write 检查
	/// @author LiuLeiLei
	/// @data 3/2/2018
	/// @desc 
	/// </summary>
    [AssetCheck(checkType = CheckEnumType.Model, option = CheckOption.Model_ReadWrite,desc = "模型 Read/Write 检查")]
	public class ModelReadWrite : BaseCheck
	{
        public override bool OnCheck(Object _obj, string _path, AssetImporter _importer)
        {
            var modelImporter = _importer as ModelImporter;
            if (modelImporter)
            {
                if (modelImporter.isReadable)
                    return false;
            }

            return base.OnCheck(_obj, _path, _importer);
        }

        public override bool OnFormat(Object _obj, string _path, AssetImporter _importer)
        {
            var modelImporter = _importer as ModelImporter;
            if (modelImporter)
            {
                modelImporter.isReadable = false;
                modelImporter.SaveAndReimport();
            }

            return base.OnFormat(_obj, _path, _importer);
        }

        public override string OnCheckMessage()
        {
            return "Read/Write 应关闭，因为游戏运行时会产生双倍内存";
        }
    }
}