using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AFrame.AssetCheck
{
	/// <summary>
	/// 检查贴图压缩格式
	/// @author LiuLeiLei
	/// @data 3/2/2018
	/// @desc 
	/// </summary>
    [AssetCheck(checkType = CheckEnumType.Texture,option = CheckOption.Texture_Format,desc = "检查贴图压缩格式")]
	public class TextureFormat : BaseCheck
    {
        private StringBuilder sb = new StringBuilder();

        public override bool OnCheck(Object _obj, string _path, AssetImporter _importer)
        {
            sb.Remove(0, sb.Length);

            var texImporter = _importer as TextureImporter;
            if(texImporter)
            {
                TextureImporterPlatformSettings android = texImporter.GetPlatformTextureSettings("Android");
                TextureImporterPlatformSettings ios = texImporter.GetPlatformTextureSettings("iPhone");

                //是否带alpha通道
                bool isAlpha = texImporter.DoesSourceTextureHaveAlpha();

                if(isAlpha)
                {
                    if (android.format != TextureImporterFormat.ETC2_RGBA8)
                    {
                        sb.Append("Android 格式不是 ETC2_RGBA8;");
                    }

                    if(ios.format != TextureImporterFormat.PVRTC_RGBA4)
                    {
                        sb.Append("iOS 格式不是 PVRTC_RGBA4;");
                    }
                }
                else
                {
                    if (android.format != TextureImporterFormat.ETC_RGB4)
                    {
                        sb.Append("Android 格式不是 ETC_RGB4;");
                    }

                    if (ios.format != TextureImporterFormat.PVRTC_RGB4)
                    {
                        sb.Append("iOS 格式不是 PVRTC_RGB4;");
                    }
                }

                if (sb.Length > 0)
                    return false;
            }

            return base.OnCheck(_obj, _path, _importer);
        }

        public override bool OnFormat(Object _obj, string _path, AssetImporter _importer)
        {
            var texImporter = _importer as TextureImporter;
            if (texImporter)
            {
                TextureImporterPlatformSettings android = texImporter.GetPlatformTextureSettings("Android");
                TextureImporterPlatformSettings ios = texImporter.GetPlatformTextureSettings("iPhone");

                //是否带alpha通道
                bool isAlpha = texImporter.DoesSourceTextureHaveAlpha();
                android.format = isAlpha ? TextureImporterFormat.ETC2_RGBA8 : TextureImporterFormat.ETC_RGB4;
                ios.format = isAlpha ? TextureImporterFormat.PVRTC_RGBA4 : TextureImporterFormat.PVRTC_RGB4;

                texImporter.SaveAndReimport();
            }

            return base.OnFormat(_obj, _path, _importer);
        }

        public override string OnCheckMessage()
        {
            return sb.ToString();
        }
    }
}