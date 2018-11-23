using System;
using System.Reflection;
using UnityEngine;

namespace AFrame.Table
{
    /// <summary>
    /// 表格列数据
    /// </summary>
    public class TableViewColum
    {
        /// <summary>
        /// 属性名字
        /// </summary>
        public string PropertyName;
        /// <summary>
        /// 标题名
        /// </summary>
        public string TitleText;
        /// <summary>
        /// 格式化
        /// </summary>
        public string Format;
        /// <summary>
        /// 对齐方式
        /// </summary>
        public TextAnchor Alignment;
        /// <summary>
        /// 宽度占比
        /// </summary>
        public float WidthInPercent;

        public FieldInfo FieldInfo;

        public PropertyInfo PropertyInfo;

        /// <summary>
        /// 格式化成字符串
        /// </summary>
        /// <param name="__obj"></param>
        /// <returns></returns>
        public string FormatObject(object __obj)
        {
            if(this.FieldInfo == null)
            {
                return EditorCommon.EditorUtil.FieldToString(__obj, this.PropertyInfo, Format);
            }
            else
            {
                return EditorCommon.EditorUtil.FieldToString(__obj, this.FieldInfo, Format);
            }

        }

        public int Compare(object __obj1,object __obj2)
        {
            object fv1;
            object fv2;

            if(this.FieldInfo == null)
            {
                fv1 = EditorCommon.EditorUtil.FieldValue(__obj1, this.PropertyInfo);
                fv2 = EditorCommon.EditorUtil.FieldValue(__obj2, this.PropertyInfo);
            }
            else
            {
                fv1 = EditorCommon.EditorUtil.FieldValue(__obj1, this.FieldInfo);
                fv2 = EditorCommon.EditorUtil.FieldValue(__obj2, this.FieldInfo);
            }


            IComparable fc1 = fv1 as IComparable;
            IComparable fc2 = fv2 as IComparable;
            
            if (fc1 != null && fc2 != null)
                return fv1.ToString().CompareTo(fv2.ToString());

            return 0/*fc1.CompareTo(fc2)*/;
        }
    }
}
