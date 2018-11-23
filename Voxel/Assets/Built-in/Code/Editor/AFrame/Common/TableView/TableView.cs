using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AFrame.Table
{
    
    public delegate void SelectionHandler(object _select, int _row);

    /// <summary>
    /// 视图表格 
    /// </summary>
    public class TableView
    {
        #region 点击回调
        /// <summary>
        /// 单击选择回调
        /// </summary>
        public event SelectionHandler onSelected;

        /// <summary>
        /// 双击选择回调
        /// </summary>
        public event SelectionHandler onDoubleSelected;

        /// <summary>
        /// 右键选择回调
        /// </summary>
        public event SelectionHandler onRightSelected;
        #endregion

        /// <summary>
        /// 界面表现类（字体风格、颜色）
        /// </summary>
        private TableViewStyle tableViewStyle;

        /// <summary>
        /// 主体视窗
        /// </summary>
        private EditorWindow hostWindow = null;
        /// <summary>
        /// 数据类型
        /// </summary>
        private Type dataType = null;
        /// <summary>
        /// 列集合
        /// </summary>
        private List<TableViewColum> columList;
        /// <summary>
        /// 行对象 list
        /// </summary>
        private List<object> rowObjList;

        /// <summary>
        /// 行 GUIStyle
        /// </summary>
        private GUIStyle rowStyle;
       
        /// <summary>
        /// 需要特殊显示的格子
        /// </summary>
        private Dictionary<object, UnityEngine.Color> colorObjDic;

        /// <summary>
        /// 滑动列表位置
        /// </summary>
        private Vector2 scrollPos = Vector2.zero;

        /// <summary>
        /// 当前点击的行对象
        /// </summary>
        private object selectObj = null;

        /// <summary>
        /// 当前列
        /// </summary>
        private int curColum = 0;
        
        /// <summary>
        /// 选中的当前列
        /// </summary>
        private int curSelectColum = -1;

        /// <summary>
        /// 当前选中的行下标
        /// </summary>
        private int curSelectIndex = -1;
        /// <summary>
        /// 上次选中的行下标
        /// </summary>
        private int lastSelectIndex = -1;

        //当前点击的行
        public int CurSelectIndex
        {
            get
            {
                return curSelectIndex;
            }
        }

        /// <summary>
        /// 是否降序排列
        /// </summary>
        private bool descending = true;

        #region 初始化表格数据
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="_window"></param>
        /// <param name="_type"></param>
        public TableView(EditorWindow _window,Type _type)
        {
            hostWindow = _window;
            dataType = _type;

            InItView();
        }

        /// <summary>
        /// 初始化表格数据
        /// </summary>
        private void InItView()
        {
            tableViewStyle = new TableViewStyle();

            columList = new List<TableViewColum>();

            rowObjList = new List<object>();

            rowStyle = new GUIStyle();
        }
        #endregion

        #region 添加列属性
        /// <summary>
        /// 添加资源条目
        /// </summary>
        /// <param name="_propertyName">属性名</param>
        /// <param name="_titleText">标题</param>
        /// <param name="_widthPercent">宽度百分比</param>
        /// <param name="_alignment">对齐方式</param>
        /// <param name="_fmt"></param>
        public bool AddColum(string _propertyName,string _titleText,float _widthPercent,TextAnchor _alignment = TextAnchor.MiddleCenter,string _fmt = "")
        {
            TableViewColum colum = new TableViewColum
            {
                PropertyName = _propertyName,
                TitleText = _titleText,
                Alignment = _alignment,
                WidthInPercent = _widthPercent,
                Format = string.IsNullOrEmpty(_fmt) ? null : _fmt,
                FieldInfo = dataType.GetField(_propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField),
                PropertyInfo = dataType.GetProperty(_propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty)
            };
            //Debug.Log("__propertyName == " + __propertyName + " >>> FieldInfo == " + colum.FieldInfo);
            //Debug.Log("__titleText == " + __titleText + " >>> Format == " + colum.Format);
            if (colum.FieldInfo == null && colum.PropertyInfo == null)
            {
                Debug.LogWarningFormat("Field And Property '{0}' accessing failed.", colum.PropertyName);
                return false;
            }

            columList.Add(colum);
            return true;
        }
        #endregion

        #region 更新表格数据
        /// <summary>
        /// 刷新数据
        /// </summary>
        /// <param name="_entries">载入的资源数据</param>
        /// <param name="_colorObjDic">特殊显示的对象字典</param>
        public void RefreshData(List<object> _entries,Dictionary<object,UnityEngine.Color> _colorObjDic = null)
        {
            rowObjList.Clear();

            if(_entries != null && _entries.Count > 0)
            {
                //将载入的资源数据list添加到资源对象list末尾
                rowObjList.AddRange(_entries);

                //如果非0，则使用此样式呈现的任何GUI元素将具有此处指定的高度
                rowStyle.fixedHeight = tableViewStyle.LineHeight * (rowObjList.Count + 1);
                //横向延伸
                rowStyle.stretchWidth = true;

                //排序
                SortData();
            }

            colorObjDic = _colorObjDic;
        }
        #endregion

        #region 绘制表格
        /// <summary>
        /// 绘制表格
        /// </summary>
        /// <param name="_rect"></param>
        public void Draw(Rect _rect)
        {
            GUILayout.BeginArea(_rect);

            //绘制标题栏
            DrawTitle(_rect.width);

            //绘制表格
            Rect scrollRect = new Rect(0, tableViewStyle.LineHeight, _rect.width, _rect.height - tableViewStyle.LineHeight);
            GUILayout.BeginArea(scrollRect);
            {
                scrollPos = GUILayout.BeginScrollView(scrollPos, GUIStyle.none, GUI.skin.verticalScrollbar);
                {
                    Rect tableRect = EditorGUILayout.BeginVertical(rowStyle);
                    {
                        // this silly line (empty label) is required by Unity to ensure the scroll bar appear as expected.
                        // Unity需要这个愚蠢的行（空标签），以确保滚动条按预期显示。
                        EditorCommon.EditorUtil.DrawLabel("", tableViewStyle.RowStyle());

                        //绘制标题栏
                        //DrawTitle(_rect.width);

                        // these first/last calculatings are for smart clipping 
                        // 计算用于智能剪辑
                        int firstLine = Mathf.Max((int)(scrollPos.y / tableViewStyle.LineHeight) - 1, 0);
                        //显示的行数
                        int showLineCount = (int)(scrollRect.height / tableViewStyle.LineHeight) + 2;
                        int lastLine = Mathf.Min(firstLine + showLineCount, rowObjList.Count);

                        for (int i = firstLine; i < lastLine; i++)
                        {
                            //绘制每一行
                            DrawLine(i, rowObjList[i], tableRect.width);
                        }
                    }
                    EditorGUILayout.EndVertical();
                }

                GUILayout.EndScrollView();
            }
            GUILayout.EndArea();
            
            GUILayout.EndArea();
        }
        
        /// <summary>
        /// 绘制表格标题栏
        /// </summary>
        /// <param name="_width"></param>
        private void DrawTitle(float _width)
        {
            int columCount = columList.Count;
            for(int i = 0;i < columCount; i ++)
            {
                var colum = columList[i];

                Rect rect = LabelRect(_width, i,0);
                bool selected = curColum == i;
                GUI.Label(rect, colum.TitleText + (selected ? tableViewStyle.SetSortMark(descending) : ""), tableViewStyle.TitleStyle(selected));

                // 事件类型
                // && 如果点的x和y分量是此矩形内的一个点，则返回true。
                if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
                {
                    //如果点击的是当前列，切换排列方式，如果不是，设置点击的列为当前列
                    if (curColum == i)
                        descending = !descending;
                    else
                        curColum = i;

                    //排序
                    SortData();
                    hostWindow.Repaint();
                }
            }
        }

        /// <summary>
        /// 绘制行
        /// </summary>
        /// <param name="_pos"></param>
        /// <param name="__obj"></param>
        /// <param name="__width"></param>
        private void DrawLine(int _pos,object _obj,float _width)
        {
            //设定单一表格绘制区域
            Rect rect = new Rect(0, _pos * tableViewStyle.LineHeight, _width, tableViewStyle.LineHeight);
            //是否点击了当前行
            bool selectionHappens = Event.current.type == EventType.MouseUp && rect.Contains(Event.current.mousePosition);

            //默认表格行风格
            GUIStyle style = tableViewStyle.RowStyle(_pos % 2 != 0);

            if (selectionHappens)
            {
                selectObj = _obj;
                curSelectIndex = _pos;
            }

            // note that the 'selected-style' assignment below should be isolated from the if-conditional statement above
            // since the above if is a one-time event, on the contrary, the 'selected-style' assignment below should be done every time in the drawing process
            //注意下面的“选择样式”赋值应该与上面的if-conditional语句隔离开来
            //因为上述if是一次性事件，相反，下面的“选择样式”赋值应该在绘图过程中每次都完成
            if (selectObj == _obj)
            {
                //选中的表格行风格变换
                style = tableViewStyle.RowSelectedStyle();
            }
            else
            {
                UnityEngine.Color specialColor;
                if (colorObjDic != null && colorObjDic.TryGetValue(_obj, out specialColor))
                    style.normal.textColor = specialColor;
            }

            int columCount = columList.Count;
            for (int i = 0;i < columCount; i ++)
            {
                //绘制行中的每一个格子
                DrawLineCol(_pos, i, _width, _obj, style,selectionHappens);
            }

            if (selectionHappens && Event.current.button == 1)
            {
                if (curSelectIndex == lastSelectIndex)
                {
                    if (onRightSelected != null)
                        onRightSelected(_obj, curSelectColum);
                }
            }

            if (curSelectIndex != lastSelectIndex)
            {
                lastSelectIndex = curSelectIndex;
            }
        }

        /// <summary>
        /// 绘制表格行颜色
        /// </summary>
        /// <param name="_pos"></param>
        /// <param name="_colum"></param>
        /// <param name="_width"></param>
        /// <param name="_obj"></param>
        /// <param name="_style"></param>
        /// <param name="_selectionHappens"></param>
        private void DrawLineCol(int _pos,int _colum,float _width,object _obj,GUIStyle _style,bool _selectionHappens = false)
        {
            Rect rect = LabelRect(_width, _colum, _pos);
            //如果当前的行已被选中
            if(_selectionHappens && rect.Contains(Event.current.mousePosition))
            {
                curSelectColum = _colum;

                if (Event.current.clickCount == 1)
                {
                    if (onSelected != null)
                        onSelected(_obj, _colum);
                }
                else
                {
                    if (onDoubleSelected != null)
                        onDoubleSelected(_obj, _colum);
                }

                hostWindow.Repaint();
            }

            //获得当前列
            var colum = columList[_colum];
            var text = colum.FormatObject(_obj);

            // note that the 'selected-style' assignment below should be isolated from the if-conditional statement above
            // since the above if is a one-time event, on the contrary, the 'selected-style' assignment below should be done every time in the drawing process
            //注意下面的“选择样式”赋值应该与上面的if-conditional语句隔离开来
            //因为上述if是一次性事件，相反，下面的“选择样式”赋值应该在绘图过程中每次都完成
            if (curSelectColum == _colum && selectObj == _obj)
                _style = tableViewStyle.SingleSelectedStyle();

            _style.alignment = colum.Alignment;
            GUI.Label(rect, new GUIContent(text,text), _style);
        }

        /// <summary>
        /// 表格框区域
        /// </summary>
        /// <param name="_width"></param>
        /// <param name="_slot"></param>
        /// <param name="_pos"></param>
        /// <returns></returns>
        private Rect LabelRect(float _width,int _slot,int _pos)
        {
            //累积比例
            float accumPercent = 0.0f;
            int count = Mathf.Min(_slot, columList.Count);
            for(int i = 0;i < count;i ++)
            {
                accumPercent += columList[i].WidthInPercent;
            }
            return new Rect(_width * accumPercent, _pos * tableViewStyle.LineHeight, _width * columList[_slot].WidthInPercent, tableViewStyle.LineHeight);
        }

        #endregion

        /// <summary>
        /// 排序
        /// </summary>
        private void SortData()
        {
            rowObjList.Sort((s1, s2) =>
            {
                if (curColum >= columList.Count)
                    return 0;

                return columList[curColum].Compare(s1, s2) * (descending ? -1 : 1);
            });
        }

        /// <summary>
        /// 设置列排序参数
        /// </summary>
        /// <param name="_curColum"></param>
        /// <param name="_descending"></param>
        public void SetSortParams(int _curColum,bool _descending)
        {
            curColum = _curColum;
            descending = _descending;
        }

        /// <summary>
        /// 设置选中对象
        /// </summary>
        /// <param name="_obj"></param>
        public void SetSelected(object _obj)
        {
            selectObj = _obj;

            if (onSelected != null)
                onSelected(_obj, 0);
        }

        /// <summary>
        /// 设置双击选中对象
        /// </summary>
        /// <param name="_obj"></param>
        public void SetDoubleSelected(object _obj)
        {
            selectObj = _obj;

            if (onDoubleSelected != null)
                onDoubleSelected(_obj, 0);
        }

        #region 清理
        /// <summary>
        /// 清理所有的列
        /// </summary>
        public void ClearColums()
        {
            columList.Clear();
        }
        #endregion
    }
}
