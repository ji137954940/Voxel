using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AFrame.AssetCheck
{
	/// <summary>
	/// 资源数据 Data类
	/// @author LiuLeiLei
	/// @data 3/2/2018
	/// @desc 
	/// </summary>
	public class AssetCheckData 
	{
        /// <summary>
        /// 资源路径
        /// </summary>
        public string resourcePath;

        /// <summary>
        /// 资源对象
        /// </summary>
        [System.NonSerialized]
        public UnityEngine.Object obj;

        [System.NonSerialized]
        public AssetImporter importer;

        /// <summary>
        /// 检测未通过的报错信息
        /// </summary>
        public string logMsg;

        /// <summary>
        /// 严重等级
        /// </summary>
        public int logLevel;
	}
}