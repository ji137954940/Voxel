using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AFrame.AssetCheck
{
    /// <summary>
	/// Jenkins 资源检查对接
	/// @author LiuLeiLei
	/// @data 3/2/2018
	/// @desc 
	/// </summary>
    public class AssetJenkinsCheck
    {
        /// <summary>
        /// Jenkins 资源检查调用此接口
        /// </summary>
        public static void JenkinsCheck()
        {
            AssetCheckManager manager = new AssetCheckManager();
            manager.OnEnable();

            //检查类型list
            List<CheckEnumType> checkTypeList = new List<CheckEnumType>
            {
                CheckEnumType.UnKnow
            };
            //检查项list
            List<CheckOption> checkOptionList = new List<CheckOption>();

            //解析jenkins检查项
            //GetEnvironmentVariable("变量名")
            string[] checkTypeArr = GetOptionArr("AssetCheckType");
            if (checkTypeArr != null && checkTypeArr.Length != 0)
            {
                int arrLen = checkTypeArr.Length;
                for (int i = 0; i < arrLen; i++)
                {
                    CheckEnumType checkType = (CheckEnumType)System.Enum.Parse(typeof(CheckEnumType), checkTypeArr[i]);

                    checkTypeList.Add(checkType);
                }

                //收集具体检查项列表
                arrLen = checkTypeList.Count;
                for (int i = 0; i < arrLen; i++)
                {
                    var checkType = checkTypeList[i];

                    string[] strArr = GetOptionArr(checkType.ToString());

                    if (strArr == null || strArr.Length == 0)
                        continue;

                    GetOptionMix(manager, strArr, checkOptionList);
                }
            }

            //获得资源文件夹路径
            string filePath = System.Environment.GetEnvironmentVariable("AssetPath");
            
            manager.SetIncludeCheckTypeList(checkTypeList);
            manager.SetIncludeOptionList(checkOptionList);

            manager.SetFilePath(filePath);

            if(manager.GetPathListByFilePath())
                //Check
                manager.OnCheck();
        }

        /// <summary>
        /// 收集具体检查项
        /// </summary>
        /// <param name="_manager"></param>
        /// <param name="_strArr"></param>
        private static void GetOptionMix(AssetCheckManager _manager, string[] _strArr, List<CheckOption> _optionList)
        {
            if (_manager == null)
                return;

            int arrLen = _strArr.Length;
            for (int i = 0; i < arrLen; i++)
            {
                CheckOption option = (CheckOption)System.Enum.Parse(typeof(CheckOption), _strArr[i]);
                _optionList.Add(option);
            }
        }

        /// <summary>
        /// 获得检查项数组
        /// </summary>
        /// <param name="_str"></param>
        /// <returns></returns>
        private static string[] GetOptionArr(string _str)
        {
            string getStr = System.Environment.GetEnvironmentVariable(_str);
            if (string.IsNullOrEmpty(getStr))
                return null;

            getStr = FormatStr(getStr);

            string[] getStrArr = getStr.Split(',');
            return getStrArr;
        }

        /// <summary>
        /// 格式化字符串
        /// Jenkins会把返回的字符串加双引号，需要去掉双引号
        /// </summary>
        /// <param name="_str"></param>
        /// <returns></returns>
        private static string FormatStr(string _str)
        {
            //int index = _str.IndexOf('"');
            //if (index != -1)
            //    _str = _str.Remove(index,1);

            char remove = '"';
            return _str.TrimStart(remove).TrimEnd(remove);
        }
    }
}
