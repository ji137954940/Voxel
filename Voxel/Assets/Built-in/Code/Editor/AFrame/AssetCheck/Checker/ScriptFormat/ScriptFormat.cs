using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AFrame.AssetCheck
{
    /// <summary>
    /// 检查脚本编码格式 UTF-8
    /// @author LiuLeiLei
    /// @data 3/2/2018
    /// @desc 
    /// </summary>
    [AssetCheck(checkType = CheckEnumType.Script, option = CheckOption.Script_Format,desc = "检查脚本编码格式 UTF-8")]
    public class ScriptFormat : BaseCheck
    {
        private string basePath = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length);

        private System.Text.Encoding encode;

        private string encodePath;

        public override bool OnCheck(Object _obj, string _path, AssetImporter _importer)
        {
            if(_path.EndsWith(".cs"))
            {
                encodePath = string.Format("{0}{1}", basePath,_path);
                
                using (System.IO.FileStream fs = new System.IO.FileStream(encodePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    encode = GetFileEncodeType(fs);
                }

                if (encode != System.Text.Encoding.UTF8)
                {
                    return false;
                }
            }

            return base.OnCheck(_obj, _path, _importer);
        }

        public override bool OnFormat(Object _obj, string _path, AssetImporter _importer)
        {
            string str = System.IO.File.ReadAllText(_path, System.Text.Encoding.Default);
            System.IO.File.WriteAllText(encodePath,str, System.Text.Encoding.UTF8);

            return base.OnFormat(_obj, _path, _importer);
        }

        public override string OnCheckMessage()
        {
            return "非UTF-8编码格式";
        }

        /// <summary>
        /// 判断配置文件的编码格式是不是utf-8
        /// </summary>
        /// <returns>The file encode type.</returns>
        /// <param name="filename">文件全路径.</param>
        /// 代码中没判断内容是不是空
        private System.Text.Encoding GetFileEncodeType(System.IO.FileStream fs)
        {
            System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
            byte[] buffer = br.ReadBytes(2);

            if (buffer[0] >= 0xEF)
            {
                if (buffer[0] == 0xEF && buffer[1] == 0xBB)
                {
                    return System.Text.Encoding.UTF8;
                }
                else if (buffer[0] == 0xFE && buffer[1] == 0xFF)
                {
                    return System.Text.Encoding.BigEndianUnicode;
                }
                else if (buffer[0] == 0xFF && buffer[1] == 0xFE)
                {
                    return System.Text.Encoding.Unicode;
                }
                else
                {
                    return System.Text.Encoding.Default;
                }
            }
            else
            {
                return System.Text.Encoding.Default;
            }
        }
    }
}
