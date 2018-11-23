using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AFrame.Core
{
	/// <summary>
	/// 页签
	/// @author LiuLeiLei
	/// @data 2018.3.12
	/// @desc 
	/// </summary>
	public class BreadCrumbElement 
	{
        public object target { get;private set; }

        public string name { get; private set; }

        public BreadCrumbElement(object _target, string _name = "")
        {
            this.target = _target;
            this.name = _name;
        }
    }
}