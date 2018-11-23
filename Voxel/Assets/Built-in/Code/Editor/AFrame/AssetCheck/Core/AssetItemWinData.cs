using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AFrame.AssetCheck
{
	/// <summary>
	/// 检查项GUI界面显示数据
	/// @author LiuLeiLei
	/// @data 3/6/2018
	/// @desc 
	/// </summary>
	public class AssetItemWinData 
	{
        /// <summary>
        /// 检查项
        /// </summary>
        public CheckOption checkOption;

        /// <summary>
        /// 是否勾选
        /// </summary>
        public bool select;

        /// <summary>
        /// 检查项描述
        /// </summary>
        public string desc;
    }
}