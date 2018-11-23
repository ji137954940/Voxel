using System.Text;
using UnityEditor;

namespace AFrame.AssetCheck
{
    /// <summary>
	/// 所有检查项的基类
	/// @author LiuLeiLei
	/// @data 3/1/2018
	/// @desc 
	/// </summary>
    public class BaseCheck
    {
        /// <summary>
        /// 资源检查
        /// </summary>
        /// <param name="_obj"></param>
        /// <param name="_path"></param>
        /// <param name="_importer"></param>
        /// <returns>有问题的资源返回false</returns>
        public virtual bool OnCheck(UnityEngine.Object _obj,string _path, AssetImporter _importer)
        {
            return true;
        }

        /// <summary>
        /// 资源处理
        /// </summary>
        /// <param name="_obj"></param>
        /// <param name="_path"></param>
        /// <param name="_importer"></param>
        /// <returns>处理失败的资源返回false</returns>
        public virtual bool OnFormat(UnityEngine.Object _obj,string _path, AssetImporter _importer)
        {
            return true;
        }

        /// <summary>
        /// 资源检查log
        /// </summary>
        /// <returns></returns>
        public virtual string OnCheckMessage()
        {
            return string.Empty;
        }

        /// <summary>
        /// 资源处理log
        /// </summary>
        /// <returns></returns>
        public virtual string OnFormatMessage()
        {
            return string.Empty;
        }
    }
}
