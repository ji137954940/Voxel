using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Color.Number.File
{

    /// <summary>
    /// 保存信息数据
    /// </summary>
    public class FileInfo
    {
		/// <summary>
		/// 是否为 voxel 信息
		/// </summary>
		public bool IsVoxel;
        /// <summary>
        /// 颜色完成区域id 数据数组
        /// </summary>
        public int[] ColorCompleteAreaIdArr;

        /// <summary>
        /// 已经完成的颜色顺序数据数组
        /// </summary>
        public int[] ColorOrderArr;

        /// <summary>
        /// 原始颜色数组
        /// </summary>
        public UnityEngine.Color[] OriginalColorTypeArr;
    }

    /// <summary>
    /// 完成信息数据
    /// </summary>
    public class CompleteInfo
    {
        /// <summary>
        /// type Id
        /// </summary>
        public int typeId;
        /// <summary>
        /// 当前 type 中完成的 数据id数组
        /// </summary>
        public int[] completeArr;
    }

    /// <summary>
    /// 相机拍照信息数据
    /// </summary>
    public class CameraPhotoInfo
    {
        ///<summary>
        /// 类型id
        ///</summary>
        public int type_id;


        ///<summary>
        /// 文件id
        ///</summary>
        public int id;

        ///<summary>
        /// 名称
        ///</summary>
        public string name;

        ///<summary>
        /// 资源路径
        ///</summary>
        public string path;
    }
}


