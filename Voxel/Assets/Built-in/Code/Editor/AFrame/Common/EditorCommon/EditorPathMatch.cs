using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace AFrame.EditorCommon
{
    public static class EditorPathMatch
    {
        /// <summary>
        /// 是否是图片
        /// </summary>
        /// <param name="_path"></param>
        /// <returns></returns>
        public static bool IsTexture(string _path)
        {
            return PathEndWithExt(_path, EditorConst.TextureExts);
        }

        /// <summary>
        /// 是否是模型
        /// </summary>
        /// <param name="_path"></param>
        /// <returns></returns>
        public static bool IsModel(string _path)
        {
            return PathEndWithExt(_path, EditorConst.ModelExts);
        }

        /// <summary>
        /// 是否是遮罩
        /// </summary>
        /// <param name="_path"></param>
        /// <returns></returns>
        public static bool IsMask(string _path)
        {
            return PathEndWithExt(_path,EditorConst.MaskExts);
        }

        /// <summary>
        /// 是否是动画文件
        /// </summary>
        /// <param name="__path"></param>
        /// <returns></returns>
        public static bool IsAnimation(string _path)
        {
            if(PathEndWithExt(_path,EditorConst.ModelExts))
            {
                string assetPath = FormatAssetPath(_path);
                ModelImporter mImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;
                if (mImporter != null && mImporter.importAnimation)
                    return true;

                return false;
            }

            return PathEndWithExt(_path, EditorConst.AnimationExts);
        }

        /// <summary>
        /// 是否是Controller文件
        /// </summary>
        /// <param name="_path"></param>
        /// <returns></returns>
        public static bool IsController(string _path)
        {
            return PathEndWithExt(_path, EditorConst.ControllerExts);
        }

        /// <summary>
        /// 是否是配置文件
        /// </summary>
        /// <param name="_path"></param>
        /// <returns></returns>
        public static bool IsConfigText(string _path)
        {
            return PathEndWithExt(_path, EditorConst.TextExts);
        }

        /// <summary>
        /// 是否是材质球 
        /// </summary>
        /// <param name="_path"></param>
        /// <returns></returns>
        public static bool IsMaterial(string _path)
        {
            return PathEndWithExt(_path, EditorConst.MaterialExts);
        }

        /// <summary>
        /// 是否是prefab
        /// </summary>
        /// <param name="_path"></param>
        /// <returns></returns>
        public static bool IsPrefab(string _path)
        {
            return PathEndWithExt(_path, EditorConst.PrefabExts);
        }

        /// <summary>
        /// 字符串比较
        /// </summary>
        /// <param name="_path"></param>
        /// <param name="_ext"></param>
        /// <returns></returns>
        public static bool PathEndWithExt(string _path, string[] _ext)
        {
            for (int i = 0; i < _ext.Length; i++)
            {
                if (_path.EndsWith(_ext[i], System.StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 扫描目录文件夹
        /// </summary>
        /// <param name="_root">路径</param>
        /// <param name="_deep">是否深层次扫描</param>
        /// <param name="_list">结果list</param>
        public static void ScanDirectoryFile(string _root,bool _deep,List<string> _list)
        {
            if(string.IsNullOrEmpty(_root) || !Directory.Exists(_root))
            {
                Debug.LogWarning("Scan directory file failed! >>> " + _root);

                EditorTool.ShowTips("当前目录下不包含要查找的资源！！！");

                return;
            }

            DirectoryInfo dirInfo = new DirectoryInfo(_root);
            //得到一个包含 '.' 符号的路径数组
            FileInfo[] fileArr = dirInfo.GetFiles("*.*");
            int fileArrLen = fileArr.Length;
            for(int i = 0;i < fileArrLen;i ++)
            {
                //Debug.LogError("files[" + i + "] == " + files[i]);
                _list.Add(fileArr[i].FullName);
            }

            if(_deep)
            {
                //得到一个包含 '.' 符号的目录路径数组
                DirectoryInfo[] dirInfoArr = dirInfo.GetDirectories("*.*");
                int dirInfoArrLen = dirInfoArr.Length;
                for(int i = 0;i < dirInfoArrLen; i ++)
                {
                    //Debug.LogError("dirInfos[" + i + "] == " + dirInfos[i] + " >>> FullName == " + dirInfos[i].FullName);
                    ScanDirectoryFile(dirInfoArr[i].FullName, _deep, _list);
                }
            }
        }

        /// <summary>
        /// 得到根目录下所有文件夹的路径
        /// </summary>
        /// <param name="_rootPath">指定根目录</param>
        /// <param name="_deep">是否深层次扫描</param>
        /// <param name="_pathList">输出路径数组</param>
        public static void ScanDirectoryToGetFiles(string _rootPath,bool _deep,List<string> _pathList)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(_rootPath);
            DirectoryInfo[] dirInfoArr = dirInfo.GetDirectories("*.*");
            int dirInfoArrLen = dirInfoArr.Length;
            for (int i = 0; i < dirInfoArrLen; i++)
            {
                //Debug.LogError("dirInfos[" + i + "] == " + dirInfos[i] + " >>> FullName == " + dirInfos[i].FullName);
                _pathList.Add(dirInfoArr[i].FullName);
                if(_deep)
                    ScanDirectoryToGetFiles(dirInfoArr[i].FullName, _deep, _pathList);
            }
        }

        /// <summary>
        /// 资源路径格式化
        /// </summary>
        /// <param name="_path"></param>
        /// <returns></returns>
        public static string FormatAssetPath(string _path)
        {
            int index = _path.IndexOf("Assets");
            if (index != -1)
                _path = _path.Substring(index);

            return NormalizePathSplash(_path);
        }

        /// <summary>
        /// 规范路径
        /// </summary>
        /// <param name="_path"></param>
        /// <returns></returns>
        public static string NormalizePathSplash(string _path)
        {
            return _path.Replace('\\', '/');
        }

        /// <summary>
        /// 获得全部文件路径list
        /// </summary>
        /// <param name="_rootPath"></param>
        /// <returns></returns>
        public static List<string> GetAssetPathList(string _rootPath)
        {
            List<string> list = new List<string>();

            ScanDirectoryFile(_rootPath, true, list);

            int count = list.Count;
            for(int i = 0;i < count; i ++)
            {
                list[i] = FormatAssetPath(list[i]);
            }

            return list;
        }
    }
}


