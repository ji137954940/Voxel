using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AFrame.AssetCheck
{
	/// <summary>
	/// 检查类属性标签
	/// @author LiuLeiLei
	/// @data 3/1/2018
	/// @desc 
	/// </summary>
	public class AssetCheckAttribute : Attribute
	{
        /// <summary>
        /// 检查所属类型
        /// </summary>
        public CheckEnumType checkType;

        /// <summary>
        /// 具体类型
        /// </summary>
        public CheckOption option;

        /// <summary>
        /// 描述
        /// </summary>
        public string desc;
	}
}