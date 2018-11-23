using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tgame.AssetBundle
{
    /// <summary>
    /// 资源的依赖关系
    /// 为了不太妨碍资源的导入速度 这里只是做了标记
    /// </summary>
    [System.Serializable]
    public class AssetsDependenct : SerializableDictionary<string, DependenctList>
    {

    }

}



