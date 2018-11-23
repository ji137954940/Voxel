using System.Text;
using UnityEditor;
using UnityEngine;

namespace AFrame.AssetCheck
{
    /// <summary>
    /// 修改图片压缩品质为 best
    /// @author LiuLeiLei
    /// @data 3/2/2018
    /// @desc 
    /// </summary>
    [AssetCheck(checkType = CheckEnumType.Texture,option = CheckOption.Texture_Compression, desc = "检查图片压缩品质是否为 best")]
    public class TextureCompression : BaseCheck
	{
        private StringBuilder sb = new StringBuilder();

        public override bool OnCheck(Object _obj, string _path, AssetImporter _importer)
        {
            sb.Remove(0,sb.Length);

            var texImporter = _importer as TextureImporter;
            if(texImporter)
            {
                if (texImporter.textureType == TextureImporterType.Lightmap)
                    return true;

                TextureImporterPlatformSettings android = texImporter.GetPlatformTextureSettings("Android");
                TextureImporterPlatformSettings ios = texImporter.GetPlatformTextureSettings("iPhone");
                
                if(android.compressionQuality != (int)TextureCompressionQuality.Best)
                {
                    sb.Append("Android TextureCompressionQuality 不是 best;");
                }
                if (ios.compressionQuality != (int)TextureCompressionQuality.Best)
                {
                    sb.Append("Android TextureCompressionQuality 不是 best;");
                }
            }

            return base.OnCheck(_obj, _path, _importer);
        }

        public override bool OnFormat(Object _obj, string _path, AssetImporter _importer)
        {
            var texImporter = _importer as TextureImporter;
            if (texImporter)
            {
                if (texImporter.textureType == TextureImporterType.Lightmap)
                    return true;

                TextureImporterPlatformSettings android = texImporter.GetPlatformTextureSettings("Android");
                TextureImporterPlatformSettings ios = texImporter.GetPlatformTextureSettings("iPhone");

                android.compressionQuality = (int)TextureCompressionQuality.Best;
                ios.compressionQuality = (int)TextureCompressionQuality.Best;

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