using UnityEngine;
using UnityEditor;
using AFrame.EditorCommon;

namespace AFrame.Table
{
    /// <summary>
    /// 界面元素表现类
    /// </summary>
    public class TableViewStyle
    {
        /// <summary>
        /// 行间距
        /// </summary>
        private float _lineHeight = 25;
        public float LineHeight
        {
            get { return _lineHeight; }
            set { _lineHeight = value; }
        }
        
        /// <summary>
        /// 标题栏Style
        /// </summary>
        private GUIStyle titleStyle;
        /// <summary>
        /// 普通背景
        /// </summary>
        private Texture2D titleOridinary;
        /// <summary>
        /// 选中背景
        /// </summary>
        private Texture2D titleSelected;

        /// <summary>
        /// 行Style
        /// </summary>
        private GUIStyle rowStyle;
        /// <summary>
        /// 选中的表格行风格
        /// </summary>
        private GUIStyle rowSelectedStyle;
        /// <summary>
        /// 选中的单个表格风格
        /// </summary>
        private GUIStyle singleSelectedStyle;
        

        /// <summary>
        /// 升降序排列标记
        /// </summary>
        /// <param name="_select"></param>
        /// <returns></returns>
        public string SetSortMark(bool _select)
        {
            return _select ? " ▼" : " ▲";
        }
        
        /// <summary>
        /// 表格标题栏风格
        /// </summary>
        /// <param name="_selected"></param>
        /// <returns></returns>
        public GUIStyle TitleStyle(bool _select)
        {
            if(titleStyle == null || titleOridinary == null || titleOridinary == null)
            {
                titleStyle = new GUIStyle(EditorStyles.whiteBoldLabel)
                {
                    alignment = TextAnchor.MiddleCenter
                };
                //设置未选中状态下的贴图
                titleOridinary = EditorUtil.GetColorTexture(EditorConst.TitleColor);
                //设置选中状态下的贴图
                titleSelected = EditorUtil.GetColorTexture(EditorConst.TitleColorSelected);
            }

            titleStyle.normal.background = _select ? titleSelected : titleOridinary;
            titleStyle.normal.textColor = _select ? UnityEngine.Color.cyan : UnityEngine.Color.white;
            
            return titleStyle;
        }

        /// <summary>
        /// 表格行风格
        /// </summary>
        /// <param name="_alt"></param>
        /// <returns></returns>
        public GUIStyle RowStyle(bool _alt = false)
        {
            if (rowStyle == null)
                rowStyle = new GUIStyle(EditorStyles.whiteLabel);

            rowStyle.normal.background = EditorUtil.GetColorTexture(new UnityEngine.Color(0.7f, 0.7f, 0.7f, _alt ? 0.3f : 0.1f));
            rowStyle.normal.textColor = UnityEngine.Color.white;

            return rowStyle;
        }

        /// <summary>
        /// 选中表格行风格
        /// </summary>
        public GUIStyle RowSelectedStyle()
        {
            if (rowSelectedStyle == null)
            {
                rowSelectedStyle = new GUIStyle(EditorStyles.whiteLabel);
                rowSelectedStyle.normal.background = EditorUtil.GetColorTexture(EditorConst.SelectionColor);
                rowSelectedStyle.normal.textColor = UnityEngine.Color.white;
            }

            return rowSelectedStyle;
        }

        /// <summary>
        /// 单个表格选中风格
        /// </summary>
        public GUIStyle SingleSelectedStyle()
        {
            if (singleSelectedStyle == null)
            {
                singleSelectedStyle = new GUIStyle(EditorStyles.whiteBoldLabel);
                singleSelectedStyle.normal.background = EditorUtil.GetColorTexture(EditorConst.SelectionColorDark);
                singleSelectedStyle.normal.textColor = UnityEngine.Color.cyan;
            }

            return singleSelectedStyle;
        }
    }
}
