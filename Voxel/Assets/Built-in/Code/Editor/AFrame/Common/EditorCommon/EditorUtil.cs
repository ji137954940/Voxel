using UnityEngine;
using System.Collections;
using System.Reflection;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace AFrame.EditorCommon
{
    public class EditorUtil
    {
        /// <summary>
        /// 不同颜色的贴图
        /// </summary>
        private static Dictionary<UnityEngine.Color, Texture2D> colorTexDic = new Dictionary<UnityEngine.Color, Texture2D>();
        
        public static string FieldToString(object _obj,FieldInfo _info,string _fmt)
        {
            object value = FieldValue(_obj, _info);
            
            if (value == null) return "";

            if (_fmt == EditorConst.BytesFormatter)
                return EditorUtility.FormatBytes((int)value);

            if (value is float)
                return ((float)value).ToString(_fmt);

            if (value is double)
                return ((double)value).ToString(_fmt);

            return value.ToString();
        }

        public static string FieldToString(object _obj, PropertyInfo _info, string _fmt)
        {
            object value = FieldValue(_obj, _info);

            if (value == null) return "";

            if (_fmt == EditorConst.BytesFormatter)
                return EditorUtility.FormatBytes((int)value);

            if (value is float)
                return ((float)value).ToString(_fmt);

            if (value is double)
                return ((double)value).ToString(_fmt);

            return value.ToString();
        }

        public static object FieldValue(object _obj, FieldInfo _info)
        {
            if (_obj == null) return "";

            if (_info == null) return "";
            //Debug.Log("__info == " + __info);
            return _info.GetValue(_obj);
        }

        public static object FieldValue(object _obj, PropertyInfo _info)
        {
            if (_obj == null) return "";

            if (_info == null) return "";
            //Debug.Log("__info == " + __info);
            return _info.GetValue(_obj, null);
        }

        public static string GetRandomString()
        {
            string path = Path.GetRandomFileName();
            path = path.Replace(".", ""); // 去掉句号
            return path;
        }

        /// <summary>
        /// 获得带颜色的贴图
        /// </summary>
        /// <param name="__c"></param>
        /// <returns></returns>
        public static Texture2D GetColorTexture(UnityEngine.Color _c)
        {
            Texture2D tex = null;

            colorTexDic.TryGetValue(_c, out tex);

            if(tex == null)
            {
                tex = new Texture2D(1,1,TextureFormat.RGBA32,false);
                //设置坐标（x，y）处的像素颜色
                tex.SetPixel(0, 0, _c);
                tex.Apply();

                colorTexDic[_c] = tex;
            }

            return tex;
        }

        /// <summary>
        /// 绘制一个标签
        /// </summary>
        /// <param name="__content"></param>
        /// <param name="__style"></param>
        public static void DrawLabel(string _content,GUIStyle _style)
        {
            EditorGUILayout.BeginHorizontal();

            //插入一个灵活的空间元素。
            GUILayout.FlexibleSpace();

            //制作一个标签栏（用于显示只读信息）
            //_style.CalcSize : 如果使用此样式呈现某些内容，需要计算其大小。
            EditorGUILayout.LabelField(_content, _style, GUILayout.Width(_style.CalcSize(new GUIContent(_content)).x + 3));

            EditorGUILayout.EndHorizontal();
        }
    }

}
