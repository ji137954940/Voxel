using AFrame.EditorCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AFrame.AssetCheck
{
	/// <summary>
	/// 资源检查管理类
	/// @author LiuLeiLei
	/// @data 3/2/2018
	/// @desc 
	/// </summary>
	public class AssetCheckManager 
	{
        /// <summary>
        /// 资源检查结束回调
        /// </summary>
        public System.Action onCheckEndCallBack;
        /// <summary>
        /// 资源处理结束回调
        /// </summary>
        public System.Action onFormatEndCallBack;
        
        /// <summary>
        /// 所有检查类型字典
        /// </summary>
        private Dictionary<CheckEnumType, List<BaseCheck>> checkDic = new Dictionary<CheckEnumType, List<BaseCheck>>();

        /// <summary>
        /// 检测出的资源数据字典
        /// </summary>
        private Dictionary<CheckEnumType, List<AssetCheckData>> checkDataDic = new Dictionary<CheckEnumType, List<AssetCheckData>>();

        /// <summary>
        /// 所有的固定枚举类型数组
        /// </summary>
        public CheckEnumType[] allEnumArr;
        /// <summary>
        /// 所有的具体检查项枚举数组
        /// </summary>
        public CheckOption[] allOptionArr;

        /// <summary>
        /// 包含的枚举项list
        /// </summary>
        private List<CheckEnumType> includeEnumList = new List<CheckEnumType>();
        /// <summary>
        /// 包含的具体检查项list
        /// </summary>
        private List<CheckOption> includeOptionList = new List<CheckOption>();

        /// <summary>
        /// 处理失败的个数
        /// </summary>
        private List<AssetCheckData> formatFailList = new List<AssetCheckData>();

        /// <summary>
        /// 资源检查输出log
        /// </summary>
        private StringBuilder checkLogSb = new StringBuilder();
        /// <summary>
        /// 资源文件夹路径
        /// </summary>
        private string filePath;
        /// <summary>
        /// 资源路径list
        /// </summary>
        private List<string> getPathList = new List<string>();

        /// <summary>
        /// 共检查的文件个数
        /// </summary>
        public int checkCount { private set; get; }

        #region OnEnable
        /// <summary>
        /// 第一次初始化数据
        /// </summary>
        public void OnEnable()
        {
            //获取全部 BaseCheck type list
            List<System.Type> typeList = new List<System.Type>();
            var typeArr = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();

            int typeCount = typeArr.Length;
            for(int i = 0;i < typeCount;i ++)
            {
                var type = typeArr[i];
                if (type.IsSubclassOf(typeof(BaseCheck)))
                    typeList.Add(type);
            }

            //筛选不同类型的 BaseCheck list
            typeCount = typeList.Count;
            int attributeCount = 0;

            for(int i = 0;i < typeCount;i ++)
            {
                var type = typeList[i];

                var attributeArr = type.GetCustomAttributes(typeof(AssetCheckAttribute), false) as AssetCheckAttribute[];

                attributeCount = attributeArr.Length;
                for(int j = 0;j < attributeCount;j ++)
                {
                    var attribute = attributeArr[j];

                    List<BaseCheck> checkTypeList;
                    if (!checkDic.TryGetValue(attribute.checkType, out checkTypeList))
                    {
                        checkTypeList = new List<BaseCheck>();
                        checkDic.Add(attribute.checkType, checkTypeList);
                    }

                    var check = Activator.CreateInstance(type) as BaseCheck;
                    checkTypeList.Add(check);
                }
            }

            //获取所有的枚举类型数组
            allEnumArr = Enum.GetValues(typeof(CheckEnumType)) as CheckEnumType[];
            //获取所有具体检查项枚举类型数组
            allOptionArr = Enum.GetValues(typeof(CheckOption)) as CheckOption[];
        }
        #endregion

        #region Ready Data

        /// <summary>
        /// 设置检查类型list
        /// </summary>
        /// <param name="_list"></param>
        public void SetIncludeCheckTypeList(List<CheckEnumType> _list)
        {
            includeEnumList = _list;
        }

        /// <summary>
        /// 设置检查项list
        /// </summary>
        /// <param name="_list"></param>
        public void SetIncludeOptionList(List<CheckOption> _list)
        {
            includeOptionList = _list;
        }

        /// <summary>
        /// 获得某个文件夹下的所有资源路径
        /// </summary>
        /// <param name="_filePath"></param>
        public void SetFilePath(string _filePath)
        {
            filePath = _filePath;
        }

        /// <summary>
        /// 获得资源路径list
        /// </summary>
        public bool GetPathListByFilePath()
        {
            //check
            if (!CanCheck()) return false;

            //默认路径为Resources路径
            if (string.IsNullOrEmpty(filePath))
                filePath = "Assets/Resources";

            //获取指定路径下的资源路径,不包含.meta文件
            List<string> totalPathList = new List<string>();
            List<string> pathList = new List<string>();

            EditorPathMatch.ScanDirectoryFile(filePath, true, totalPathList);

            int pathCount = totalPathList.Count;
            for (int i = 0; i < pathCount; i++)
            {
                var path = totalPathList[i];

                EditorUtility.DisplayProgressBar("Get Asset Path", path, (float)(i) / pathCount);

                //路径格式化
                path = EditorPathMatch.FormatAssetPath(path);

                //去掉.meta文件
                if (path.ToLower() != ".meta")
                    pathList.Add(path);
            }
            EditorUtility.ClearProgressBar();

            //指定资源路径列表
            SetPathList(pathList);

            return true;
        }

        /// <summary>
        /// 设定资源路径list
        /// </summary>
        /// <param name="_pathList"></param>
        public void SetPathList(List<string> _pathList)
        {
            getPathList = _pathList;
        }

        #endregion

        #region 资源检查

        /// <summary>
        /// 资源检查
        /// </summary>
        public void OnCheck()
        {
            //reset info
            ResetInfo();

            //checkLogSb.Append("Asset Check Result:\r\n");

            int pathCount = getPathList.Count;
            for(int i = 0;i < pathCount;i ++)
            {
                var path = getPathList[i];

                EditorUtility.DisplayProgressBar("Asset Check", path, (float)(i) / pathCount);

                OnEnumTypeCheck(path);
            }
            EditorUtility.ClearProgressBar();

            //输出资源检查log
            if(checkLogSb.Length > 0)
            {
                checkLogSb.Insert(0,"Asset Check Result:\r\n");
            }
            else
            {
                checkLogSb.Append("没有不合理资源");
            }
            Debug.LogError(checkLogSb.ToString());

            checkCount = pathCount;

            //资源检查结束，执行回调方法
            if (onCheckEndCallBack != null)
                onCheckEndCallBack();
        }

        /// <summary>
        /// 各种类型的检查
        /// </summary>
        /// <param name="_path"></param>
        private void OnEnumTypeCheck(string _path)
        {
            int includeEnumCount = includeEnumList.Count;
            int baseCheckCount = 0;

            for(int i = 0;i < includeEnumCount;i ++)
            {
                var enumType = includeEnumList[i];
                if(checkDic.ContainsKey(enumType))
                {
                    var baseCheckList = checkDic[enumType];
                    baseCheckCount = baseCheckList.Count;

                    for(int j = 0;j < baseCheckCount;j ++)
                    {
                        var baseCheck = baseCheckList[j];

                        //判断是否包含具体检查项,不包含则直接跳过
                        if (!ContainsOption(baseCheck))
                            continue;

                        UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(_path,typeof(UnityEngine.Object));
                        var assetImporter = AssetImporter.GetAtPath(_path);

                        //检测不通过
                        if (!baseCheck.OnCheck(obj,_path,assetImporter))
                        {
                            //打印log
                            //Debug.Log(string.Format("资源路径：{0}，CheckFailMsg：{1}",_path,baseCheck.OnCheckMessage()));

                            AddInLog(enumType, _path, baseCheck.OnCheckMessage());

                            //未通过的资源信息存储起来用于处理
                            AddInCheckDataList(enumType, obj, _path, assetImporter, baseCheck.OnCheckMessage());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 是否可以检查
        /// </summary>
        /// <returns></returns>
        private bool CanCheck()
        {
            if (includeEnumList.Count == 0)
            {
                Debug.LogError("没有选择任何检查类型");
                return false;
            }

            if (includeOptionList.Count == 0)
            {
                Debug.LogError("没有选择任何检查项");
                return false;
            }

            return true;
        }

        #endregion

        #region 资源处理

        /// <summary>
        /// 资源处理
        /// </summary>
        public void OnFormat()
        {
            formatFailList.Clear();

            //暂定所有不符合检查的资源都通过手动检查
            Debug.LogError("暂定所有不符合检查的资源都通过手动修复，规则需要再议");
            return;

            //指定类型需要处理的资源个数
            int dataListCount = 0;
            //指定类型的检查项个数
            int baseCheckCount = 0;
            
            for (var item = checkDataDic.GetEnumerator();item.MoveNext();)
            {
                var list = item.Current.Value;
                dataListCount = list.Count;

                //获得当前类型的所有检查项
                var baseCheckList = checkDic[item.Current.Key];
                baseCheckCount = baseCheckList.Count;

                for(int i = 0;i < dataListCount;i ++)
                {
                    var checkData = list[i];
                    //处理资源
                    for(int j = 0;j < baseCheckCount;j ++)
                    {
                        var baseCheck = baseCheckList[j];

                        if(!baseCheck.OnFormat(checkData.obj,checkData.resourcePath,checkData.importer))
                        {
                            Debug.LogError(string.Format("资源路径：{0}，FormatFailMsg：{1}",checkData.resourcePath,baseCheck.OnFormatMessage()));

                            formatFailList.Add(checkData);
                        }
                    }
                }
            }

            //把处理成功的从字典中剔除出去，留下处理失败的显示提醒
            int failCount = formatFailList.Count;
            for(var item = checkDataDic.GetEnumerator();item.MoveNext();)
            {
                var list = item.Current.Value;
                if(failCount == 0)
                {
                    list.Clear();
                    continue;
                }

                for(int i = list.Count;i >= 0;i --)
                {
                    for(int j = 0;j < failCount; j ++)
                    {
                        if (list[i].resourcePath != formatFailList[j].resourcePath)
                            list.RemoveAt(i);
                    }
                }
            }

            //资源处理结束，执行回调方法
            if (onFormatEndCallBack != null)
                onFormatEndCallBack();
        }

        #endregion

        /// <summary>
        /// 是否包含某项
        /// </summary>
        /// <param name="_target">目标枚举类型转换为int类型</param>
        /// <param name="_selectMix">所有选中的枚举值</param>
        /// <returns></returns>
        public bool IsSelect(int _target,int _allSelectMix)
        {
            //if ((_allSelectMix & _target) == _target)
            //    return true;
            //1 左移
            int index = 1 << _target;
            //按位 与
            if ((_allSelectMix & index) == index)
                return true;

            return false;
        }

        #region OtptionCheck

        /// <summary>
        /// 是否包含具体检查项
        /// </summary>
        /// <param name="_check"></param>
        /// <returns></returns>
        private bool ContainsOption(BaseCheck _check)
        {
            var attributeArr = _check.GetType().GetCustomAttributes(typeof(AssetCheckAttribute), false) as AssetCheckAttribute[];
            int attributeLen = attributeArr.Length;
            for(int i = 0;i < attributeLen;i ++)
            {
                var attribute = attributeArr[i];
                //UnKnow 为必须检查项
                if (attribute.checkType == CheckEnumType.UnKnow)
                    return true;

                return includeOptionList.Contains(attribute.option);
            }

            return false;
        }
        #endregion

        #region Reset Info
        /// <summary>
        /// 重置一些数据
        /// </summary>
        public void ResetInfo()
        {
            for (var item = checkDataDic.GetEnumerator(); item.MoveNext();)
            {
                item.Current.Value.Clear();
            }

            checkLogSb.Remove(0, checkLogSb.Length);
        }

        #endregion

        #region Log
        /// <summary>
        /// 添加打印信息
        /// </summary>
        /// <param name="_enumType"></param>
        /// <param name="_path"></param>
        /// <param name="_checkMsg"></param>
        private void AddInLog(CheckEnumType _enumType,string _path,string _checkMsg)
        {
            checkLogSb.Append(_enumType);
            checkLogSb.Append("|");
            checkLogSb.Append(_path);
            checkLogSb.Append("|");
            checkLogSb.Append(_checkMsg);
            checkLogSb.Append("\r\n");
        }
        #endregion

        #region CheckData List
        /// <summary>
        /// 添加到未通过检查的缓存数据list里
        /// </summary>
        /// <param name="_enumType"></param>
        /// <param name="_obj"></param>
        /// <param name="_path"></param>
        /// <param name="_importer"></param>
        /// <param name="_checkMsg"></param>
        private void AddInCheckDataList(CheckEnumType _enumType,UnityEngine.Object _obj,string _path,AssetImporter _importer,string _checkMsg)
        {
            //未通过的资源信息存储起来用于处理
            AssetCheckData checkData = new AssetCheckData();
            checkData.resourcePath = _path;
            checkData.obj = _obj;
            checkData.importer = _importer;
            checkData.logMsg = _checkMsg;

            List<AssetCheckData> checkDataList;
            if (!checkDataDic.TryGetValue(_enumType, out checkDataList))
            {
                checkDataList = new List<AssetCheckData>();
                checkDataDic.Add(_enumType, checkDataList);
            }

            checkDataList.Add(checkData);
        }

        /// <summary>
        /// 获得未通过检测的资源数据列表
        /// </summary>
        /// <returns></returns>
        public List<AssetCheckData> GetCheckDataList()
        {
            List<AssetCheckData> getList = new List<AssetCheckData>();

            int dataCount = 0;

            for (var item = checkDataDic.GetEnumerator(); item.MoveNext();)
            {
                var dataList = item.Current.Value;

                dataCount = dataList.Count;
                for (int i = 0; i < dataCount; i++)
                    getList.Add(dataList[i]);
            }

            return getList;
        }

        /// <summary>
        /// 获得检查输出log
        /// </summary>
        /// <returns></returns>
        public string GetLog()
        {
            return checkLogSb.ToString();
        }

        #endregion
    }
}