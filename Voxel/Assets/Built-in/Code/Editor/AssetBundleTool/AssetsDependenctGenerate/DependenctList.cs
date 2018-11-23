using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tgame.AssetBundle
{
    [System.Serializable]
    public class DependenctList
    {
        /// <summary>
        /// 引用数据列表
        /// </summary>
        public string[] dependencies;

        /// <summary>
        /// 如果在过程中有变动 就改变这个值为true 默认为false
        /// </summary>
        public bool isdirty;
    }
}


