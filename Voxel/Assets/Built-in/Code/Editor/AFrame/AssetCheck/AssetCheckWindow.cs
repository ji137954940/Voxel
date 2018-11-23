using AFrame.EditorCommon;
using AFrame.Table;
using System.Collections.Generic;
using System.IO;
using Tgame.AssetBundle;
using UnityEditor;
using UnityEngine;

namespace AFrame.AssetCheck
{
    public class AssetCheckWindow : EditorWindow
    {
        #region GUI Check

        [MenuItem("AFrame/资源检查器", priority = 100)]
        public static void OpenCheckWindow()
        {
            var window = GetWindow<AssetCheckWindow>("资源检查器");
            window.Show();
        }

        /// <summary>
        /// 资源检查界面GUI
        /// </summary>
        public class CheckStyle
        {
            public readonly GUIStyle ToolBar = "ToolBar";

            public readonly GUIStyle TE_ToolBarBtn = "TE toolbarbutton";

            public readonly GUIStyle WindowBackground = "WindowBackground";

            public readonly GUIStyle Window = "window";
        }

        /// <summary>
        /// 资源检查管理类
        /// </summary>
        private AssetCheckManager checkManager;

        /// <summary>
        /// GUI 界面风格
        /// </summary>
        public CheckStyle checkStyle;
        /// <summary>
        /// 数据显示用的表格
        /// </summary>
        private TableView table;
        
        private const string titleStr = "选择检查类型";

        /// <summary>
        /// GUI显示所占百分比
        /// </summary>
        private float percet = 0.2f;
        /// <summary>
        /// y方向偏移值
        /// </summary>
        private float offset = 25;
        /// <summary>
        /// 距离边界值
        /// </summary>
        private float border = 10;
        
        /// <summary>
        /// 检查项GUI与边界的距离
        /// </summary>
        private float itemWinBorder = 0;
        /// <summary>
        /// 检查项GUI显示高度
        /// </summary>
        private float itemWinHeight = 150;
        /// <summary>
        /// 检查类型GUI列表
        /// </summary>
        private Vector2 scrollview;

        /// <summary>
        /// 检查项类型字典
        /// </summary>
        private Dictionary<CheckEnumType, List<AssetItemWinData>> checkOptionDic = new Dictionary<CheckEnumType, List<AssetItemWinData>>();

        /// <summary>
        /// 资源文件夹路径
        /// </summary>
        private string filePath = "Assets/Resources/Res";
        /// <summary>
        /// 检查类型
        /// </summary>
        private CheckEnumType checkTypeMix;
        /// <summary>
        /// 检查类型list
        /// </summary>
        private List<CheckEnumType> checkTypeList = new List<CheckEnumType>();
        /// <summary>
        /// 检查项list
        /// </summary>
        private List<CheckOption> checkOptionList = new List<CheckOption>();

        /// <summary>
        /// 检查项Item list
        /// </summary>
        private List<AssetCheckWinItem> checkWinItemList = new List<AssetCheckWinItem>();

        private void OnEnable()
        {
            checkManager = new AssetCheckManager();
            checkManager.OnEnable();
            checkManager.onCheckEndCallBack = OnCheckEndCallBack;
            checkManager.onFormatEndCallBack = OnFormatEndCallBack;
            checkManager.SetFilePath(filePath);

            table = new TableView(this,typeof(AssetCheckData));
            table.AddColum("resourcePath","Path",0.5f,TextAnchor.MiddleLeft);
            table.AddColum("logMsg", "CheckFailMsg",0.5f,TextAnchor.MiddleLeft);
            table.onSelected += TableOnSelected;

            //checkWinItemList.Add(new AssetCheckWinItem(this,this,"Texture"));
            //checkWinItemList.Add(new AssetCheckWinItem(this, this, "Model"));

            //对Option检查项做区分
            var typeArr = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
            int typeCount = typeArr.Length;
            int attributeArrLen = 0;
            for (int i = 0; i < typeCount; i++)
            {
                var type = typeArr[i];
                var attributeArr = type.GetCustomAttributes(typeof(AssetCheckAttribute), false) as AssetCheckAttribute[];
                attributeArrLen = attributeArr.Length;
                for(int j = 0;j < attributeArrLen;j ++)
                {
                    var attribute = attributeArr[j];
                    List<AssetItemWinData> optionList;
                    if(!checkOptionDic.TryGetValue(attribute.checkType,out optionList))
                    {
                        optionList = new List<AssetItemWinData>();
                        checkOptionDic.Add(attribute.checkType, optionList);
                    }
                    AssetItemWinData itemData = new AssetItemWinData();
                    itemData.checkOption = attribute.option;
                    itemData.desc = attribute.desc;
                    itemData.select = false;
                    optionList.Add(itemData);
                }
            }
        }

        /// <summary>
        /// 表格条目点击回调
        /// </summary>
        /// <param name="_obj"></param>
        /// <param name="_col"></param>
        private void TableOnSelected(object _obj,int _col)
        {
            if (_obj != null)
            {
                AssetCheckData checkData = _obj as AssetCheckData;

                EditorGUIUtility.PingObject(checkData.obj);

                Selection.activeObject = checkData.obj;
            }
        }
        
        private void OnGUI()
        {
            //Unable to use a named GUIStyle without a current skin. 
            //Most likely you need to move your GUIStyle initialization code to OnGUI
            //GUI风格元素需要在OnGUI方法里初始化，否则会报以上错误，界面也会发生显示错误
            if (checkStyle == null)
                checkStyle = new CheckStyle();

            GUILayout.BeginHorizontal(checkStyle.ToolBar,new GUILayoutOption[] { GUILayout.Width(this.position.width) });
            {
                EditorGUI.BeginChangeCheck();

                checkTypeMix = (CheckEnumType)EditorGUILayout.EnumMaskPopup(new GUIContent(titleStr), checkTypeMix, checkStyle.TE_ToolBarBtn,new GUILayoutOption[] { GUILayout.Width(600) });

                if(EditorGUI.EndChangeCheck())
                {
                    //检查类型分类
                    OnCheckTypeSelect(checkTypeMix);
                }

                EditorGUI.BeginChangeCheck();

                if (GUILayout.Button(string.Format("当前目录{0}", filePath),checkStyle.TE_ToolBarBtn))
                {
                    filePath = EditorUtility.OpenFolderPanel("选择筛选目录{0}",Application.dataPath,string.Empty);

                    var currentDir = BuildUtils.GetUnityPath(Path.GetFullPath(Directory.GetCurrentDirectory()));

                    filePath = filePath.Replace(string.Format("{0}/", currentDir), "");
                }
                if (EditorGUI.EndChangeCheck())
                {
                    checkManager.SetFilePath(filePath);

                    Debug.Log(string.Format("当前选择目录:{0}", filePath));
                }
            }
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Check"))
                {
                    //检查
                    if(checkManager.GetPathListByFilePath())
                        checkManager.OnCheck();
                }

                if (GUILayout.Button("Format"))
                {
                    //处理
                    checkManager.OnFormat();
                }
            }
            GUILayout.EndHorizontal();
            
            #region 表格GUI绘制
            //表格 GUI 绘制
            Rect viewRect = new Rect(border * 2 + this.position.width * percet, offset * 2, this.position.width * (1 - percet) - border * 3, this.position.height - offset * 2 - border);

            if (table != null)
                table.Draw(viewRect);
            #endregion

            #region 检查项GUI绘制
            
            //检查项 itemlist 绘制
            Rect itemViewRect = new Rect(border,offset * 2,this.position.width * percet, this.position.height - offset * 2 - border);

            GUILayout.BeginArea(itemViewRect,checkStyle.WindowBackground);
            
            scrollview = GUILayout.BeginScrollView(scrollview, GUIStyle.none, GUI.skin.verticalScrollbar);

            DrawItemWind(itemViewRect);

            GUILayout.EndScrollView();

            GUILayout.EndArea();
            
            #endregion
        }

        private void DrawItemWind(Rect _rect)
        {
            int itemCount = checkWinItemList.Count;
            for (int i = 0; i < itemCount; i++)
            {
                var item = checkWinItemList[i];

                Rect itemRect = new Rect(0, i * itemWinHeight + itemWinBorder * (i + 1), _rect.width - itemWinBorder * 4, itemWinHeight);

                GUILayout.Box(new GUIContent(/*item.name*/)/*, checkStyle.Window*/, GUIStyle.none, GUILayout.Width(itemRect.width), GUILayout.Height(itemRect.height));

                if (Event.current.type == EventType.Repaint)
                {
                    checkStyle.Window.Draw(itemRect, item.name, false, false, false, false);
                }
                item.OnGUI(itemRect);
            }
        }

        /// <summary>
        /// 资源检查结束回调方法
        /// </summary>
        private void OnCheckEndCallBack()
        {
            TableRefresh();
        }

        /// <summary>
        /// 刷新表格
        /// </summary>
        public void TableRefresh()
        {
            //显示未通过检查的资源列表
            var dataList = checkManager.GetCheckDataList();
            table.RefreshData(EditorTool.ToObjectList(dataList));
        }

        /// <summary>
        /// 资源处理结束回调方法
        /// </summary>
        private void OnFormatEndCallBack()
        {
            TableRefresh();
        }

        /// <summary>
        /// 检查类型选择
        /// </summary>
        /// <param name="_checkTypeMix"></param>
        private void OnCheckTypeSelect(CheckEnumType _checkTypeMix)
        {
            var checkEnumArr = checkManager.allEnumArr;
            int checkEnumLen = checkEnumArr.Length;
            for(int i = 0;i < checkEnumLen;i ++)
            {
                var checkType = checkEnumArr[i];

                if(checkManager.IsSelect((int)checkType,(int)checkTypeMix))
                {
                    if(!checkTypeList.Contains(checkType))
                    {
                        checkTypeList.Add(checkType);
                        //添加当前类型的Option GUI
                        SetItemWinList(checkType);
                    }
                }
                else
                {
                    if (checkTypeList.Contains(checkType))
                    {
                        checkTypeList.Remove(checkType);
                        //移除当前类型的Option GUI
                        SetItemWinList(checkType,false);
                    }
                }
            }

            checkManager.SetIncludeCheckTypeList(checkTypeList);

            //foreach(var item in checkTypeList)
            //{
            //    Debug.Log(item);
            //}
        }

        /// <summary>
        /// 添加或删除检查类型
        /// </summary>
        /// <param name="_checkType"></param>
        /// <param name="_add"></param>
        private void SetItemWinList(CheckEnumType _checkType,bool _add = true)
        {
            AssetCheckWinItem itemWin = this.checkWinItemList.Find((item) => item.name == _checkType.ToString());
            if (_add)
            {
                if(itemWin == null)
                {
                    AssetCheckWinItem winItem = new AssetCheckWinItem(this, checkOptionDic[_checkType], _checkType.ToString());
                    checkWinItemList.Add(winItem);
                }
            }
            else
            {
                if(itemWin != null)
                    this.checkWinItemList.Remove(itemWin);
            }
        }
        
        /// <summary>
        /// 添加或删除检查项
        /// </summary>
        /// <param name="_option"></param>
        /// <param name="_add"></param>
        public void SetOptionList(CheckOption _option,bool _add = true)
        {
            bool contains = checkOptionList.Contains(_option);
            if (_add)
            {
                if (!contains)
                    checkOptionList.Add(_option);
            }
            else
            {
                if (contains)
                    checkOptionList.Remove(_option);
            }

            checkManager.SetIncludeOptionList(checkOptionList);
        }

        #endregion
    }
}
