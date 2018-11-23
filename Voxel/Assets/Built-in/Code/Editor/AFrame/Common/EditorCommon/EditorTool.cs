using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using LitJson;
using AFrame.Core;

namespace AFrame.EditorCommon
{
    public static class EditorTool
    {
        /// <summary>
        /// 计算图片字节大小
        /// </summary>
        /// <param name="_tex"></param>
        /// <param name="_format"></param>
        /// <returns></returns>
        public static int CalculateTextureSizeBytes(Texture _tex,TextureImporterFormat _format)
        {
            var texWidth = _tex.width;
            var texHeight = _tex.height;

            if(_tex is Texture2D)
            {
                var tex2D = _tex as Texture2D;
                var bitsPerPixel = GetBitsPerPixel(_format);
                var mipmapCount = tex2D.mipmapCount;
                var mipLevel = 1;
                var texSize = 0;

                while(mipLevel <= mipmapCount)
                {
                    texSize += texWidth * texHeight * bitsPerPixel / 8;
                    texWidth = texWidth / 2;
                    texHeight = texHeight / 2;
                    mipLevel++;
                }
                return texSize;
            }

            if(_tex is Cubemap)
            {
                var bitsPerPixel = GetBitsPerPixel(_format);
                return texWidth * texHeight * 6 * bitsPerPixel / 8;
            }

            return 0;
        }

        /// <summary>
        /// 计算模型大小
        /// </summary>
        /// <param name="_path"></param>
        /// <returns></returns>
        public static int CalculateModelSizeBytes(string _path)
        {
            int size = 0;

            Object[] objsArr = AssetDatabase.LoadAllAssetsAtPath(_path);

            int objsArrLen = objsArr.Length;
            for(int i = 0;i < objsArrLen; i ++)
            {
                var obj = objsArr[i];
                if (obj is Mesh)
                    size += GetRuntimeMemorySize(obj);

                if ((!(obj is GameObject)) && (!(obj is Component)))
                    Resources.UnloadAsset(obj);
            }

            return size;
        }

        /// <summary>
        /// 计算动画大小
        /// </summary>
        /// <param name="_path"></param>
        /// <returns></returns>
        public static int CalculateAnimationSizeBytes(string _path)
        {
            int size = 0;
            Object[] objsArr = AssetDatabase.LoadAllAssetsAtPath(_path);

            int objsArrLen = objsArr.Length;
            for(int i = 0;i < objsArrLen; i ++)
            {
                var obj = objsArr[i];
                if ((obj is AnimationClip) && obj.name != EditorConst.EDITOR_ANICLIP_NAME)
                    size += GetRuntimeMemorySize(obj);

                if ((!(obj is GameObject)) && (!(obj is Component)))
                    Resources.UnloadAsset(obj);
            }

            return size;
        }

        /// <summary>
        /// 获得像素位
        /// </summary>
        /// <param name="_format"></param>
        /// <returns></returns>
        public static int GetBitsPerPixel(TextureImporterFormat _format)
        {
            switch(_format)
            {
                case TextureImporterFormat.Alpha8:          //只包含alpha通道
                    return 8;
                case TextureImporterFormat.RGB24:           //A color
                    return 24;
                case TextureImporterFormat.RGBA32:          //带alpha通道的颜色
                    return 32;
                case TextureImporterFormat.ARGB32:          //同上
                    return 32;
                case TextureImporterFormat.DXT1:            //压缩纹理格式
                    return 4;
                case TextureImporterFormat.DXT5:            //带alpha通道的纹理压缩格式
                    return 8;
                case TextureImporterFormat.PVRTC_RGB2:      //PowerVR（IOS）2位/像素压缩纹理格式
                    return 2;
                case TextureImporterFormat.PVRTC_RGBA2:     //PowerVR（IOS）2位/像素的alpha通道的纹理压缩格式
                    return 2;
                case TextureImporterFormat.PVRTC_RGB4:      //PowerVR（IOS）4位/像素压缩纹理格式
                    return 4;
                case TextureImporterFormat.PVRTC_RGBA4:     //PowerVR（IOS）4位/像素的alpha通道的纹理压缩格式
                    return 4;
                case TextureImporterFormat.ETC_RGB4:        //（gles2.0）4位/像素的RGB纹理压缩格式
                    return 4;
                case TextureImporterFormat.ETC2_RGB4:       //（gles3.0）4位/像素的RGB纹理压缩格式
                    return 4;
                case TextureImporterFormat.ETC2_RGBA8:      //（gles3.0）8位/像素的alpha通道的纹理压缩格式
                    return 8;
                case TextureImporterFormat.ATC_RGB4:        //ATC（atitc）4位/像素的RGB纹理压缩格式
                    return 4;
                case TextureImporterFormat.ATC_RGBA8:       //ATC（atitc）8位/像素的RGB纹理压缩格式
                    return 8;
#pragma warning disable 0618
                case TextureImporterFormat.AutomaticCompressed:     //适合当前平台的压缩格式
                    return 4;
                case TextureImporterFormat.AutomaticTruecolor:      //真彩色（无压缩）
                    return 32;
                default:
                    return 32;
#pragma warning restore 0618
            }
        }

        /// <summary>
        /// 获得运行时所占内存大小
        /// </summary>
        /// <param name="_obj"></param>
        /// <returns></returns>
        public static int GetRuntimeMemorySize(Object _obj)
        {
#pragma warning disable 0618
            return UnityEngine.Profiling.Profiler.GetRuntimeMemorySize(_obj);
#pragma warning restore 0618
        }

        /// <summary>
        /// 图片是否是2的次幂
        /// </summary>
        /// <param name="_path"></param>
        /// <returns></returns>
        public static bool IsPowerOf2(string _path)
        {
            Texture texTarget = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(_path) as Texture;

            //var texType = Types.GetType("UnityEditor.TextureUtil", "UnityEditor.dll");
            
            var assem = System.AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetTypes().Any(t => t.Name == "TextureUtil"));
            if (assem != null)
            {
                var type = assem.GetTypes().FirstOrDefault(t => t.Name == "TextureUtil");
                MethodInfo info = type.GetMethod("IsNonPowerOfTwo", BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);

                string isNon = info.Invoke(null, new object[] { texTarget }).ToString();

                return isNon.Contains("true");
            }

            return false;
        }

        /// <summary>
        /// 图片是否是2的次幂
        /// </summary>
        /// <param name="_w"></param>
        /// <param name="_h"></param>
        /// <returns></returns>
        public static bool IsPowerOf2(int _w,int _h)
        {
            if (_w < 1 || _h < 1) return false;
            
            bool wOf2 = (_w & _w - 1) == 0;
            bool hOf2 = (_h & _h - 1) == 0;

            return wOf2 && hOf2;
        }

        /// <summary>
        /// 转换成object list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_dataList"></param>
        /// <returns></returns>
        public static List<object> ToObjectList<T>(List<T> _dataList)
        {
            if (_dataList == null) return null;
            List<object> objs = new List<object>();
            _dataList.ForEach((data) => { objs.Add(data); });

            return objs;
        }

        /// <summary>
        /// 进度条进度显示
        /// </summary>
        /// <param name="_i"></param>
        /// <param name="_count"></param>
        /// <returns></returns>
        public static float GetProgress(int _i,int _count)
        {
            if(_count <= 0)
            {
                Debug.LogError("Count can not be <= 0 >>> Count == " + _count);
                return 0;
            }

            return (_i * 1.0f) / _count;
        }

        #region Json数据存取
        /// <summary>
        /// load json data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="__path"></param>
        /// <returns></returns>
        public static T LoadJsonData<T>(string _path)
        {
            try
            {
                if (!File.Exists(_path))
                {
                    return default(T);
                }
                string str = File.ReadAllText(_path);
                if (string.IsNullOrEmpty(str))
                {
                    return default(T);
                }
                T data = JsonMapper.ToObject<T>(str);
                if (data == null)
                {
                    Debug.LogError("Can't read json data from " + _path);
                }

                return data;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return default(T);
            }
        }

        /// <summary>
        /// 保存json格式数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_data"></param>
        /// <param name="_path"></param>
        public static void SaveJsonData<T>(T _data, string _path)
        {
            //Debug.Log("__path == " + __path);
            CreateDirectory(_path);

            string jsonStr = JsonFormatter.PrettyPrint(JsonMapper.ToJson(_data));
            //Debug.Log(jsonStr);
            File.WriteAllText(_path, jsonStr);
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="_path"></param>
        public static void CreateDirectory(string _path)
        {
            if (string.IsNullOrEmpty(_path))
            {
                return;
            }

            string dir = Path.GetDirectoryName(_path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
        #endregion

        #region 提示信息

        /// <summary>
        /// 消息提示窗口
        /// </summary>
        /// <param name="_text"></param>
        /// <param name="_doubleBtn"></param>
        /// <returns></returns>
        public static bool ShowTips(string _text,bool _doubleBtn = false)
        {
            if(!_doubleBtn)
                return ShowTips("Hello World", _text, "俺晓得嘞");
            else
                return ShowTips("Hello World", _text, "否","是");
        }

        /// <summary>
        /// 消息提示窗口
        /// </summary>
        /// <param name="_titleText"></param>
        /// <param name="_text"></param>
        /// <param name="_okText"></param>
        /// <param name="_cancelText"></param>
        public static bool ShowTips(string _titleText,string _text,string _okText = "OK",string _cancelText = "")
        {
            if(string.IsNullOrEmpty(_cancelText))
                return EditorUtility.DisplayDialog(_titleText, _text, _okText);
            else
                return EditorUtility.DisplayDialog(_titleText, _text, _okText, _cancelText);
        }

        #endregion

        #region BreadElement

        /// <summary>
        /// 添加页签
        /// </summary>
        /// <param name="_breadList"></param>
        /// <param name="_target"></param>
        /// <param name="_name"></param>
        /// <returns></returns>
        public static object AddBreadCrumb(List<BreadCrumbElement> _breadList, object _target, string _name)
        {
            //先看有没有当前要跳转的页面，有的话直接切到目标页签
            BreadCrumbElement target = _breadList.Find((BreadCrumbElement item) => item.name == _name);
            //Debug.Log("target == " + target);
            if (target != null)
            {
                return GoToTargetGUI(_breadList,target.target);
            }
            else
            {
                _breadList.Add(new BreadCrumbElement(_target, _name));

                return _breadList[_breadList.Count - 1].target;
            }
        }

        /// <summary>
        /// 跳转到指定页签
        /// </summary>
        /// <param name="_breadList"></param>
        /// <param name="_target"></param>
        /// <returns></returns>
        public static object GoToTargetGUI(List<BreadCrumbElement> _breadList,object _target)
        {
            int num = _breadList.FindIndex((BreadCrumbElement item) => item.target == _target);
            while (_breadList.Count > num + 1)
            {
                _breadList.RemoveAt(num + 1);
            }

            return _breadList[_breadList.Count - 1].target;
        }

        #endregion
    }

    /// <summary>
    /// 提示弹窗窗体
    /// </summary>
    public class EditorTipsWin : EditorWindow
    {
        public static void CreateTipsWin(string __titleTipStr = "")
        {
            //EditorTipsWin tipsWin = GetWindow<EditorTipsWin>(false,__titleTipStr,true);
            //tipsWin.wantsMouseEnterLeaveWindow = true;

            EditorUtility.DisplayDialog(__titleTipStr,"Hello World","OK","GO");
        }
    }
}
