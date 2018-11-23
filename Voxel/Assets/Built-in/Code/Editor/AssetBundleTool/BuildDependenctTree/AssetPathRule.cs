using System.Text.RegularExpressions;

namespace Tgame.AssetBundle
{

    /// <summary>
    /// 资源路径规则
    /// </summary>
    public class AssetPathRule
    {
        /// <summary>
        /// 包含路径
        /// </summary>
        public static string[] IncludePath = new string[] { "Assets/Resources/"
        , "Assets/StreamingAssets/" };

        /// <summary>
        /// 排除路径
        /// </summary>
        public static string[] ExclusivePath;

        /// <summary>
        /// 必须单独打包
        /// </summary>
        public static string[] MustBuild = new string[] {
		//@"Assets/Resources/Res/UI/UIRes/Prefab/CommonAtlas/.+\.prefab",//打包CommonAtlas
		//@"Assets/Resources/Res/UI/UIRes/Textures/UI/Manual/.+\.png",//打包图片
		//@".+/Lightmap/.+",//打包所有的光照贴图
		//@".+Atlas.+\.prefab",//所有的Atlas
		//@"Assets/Resources/Res/UI/UIRes/Textures/UI/LoadingScene/.+\.png",
        @"Assets/Resources/Res/UI/PackerImg/.+\.png",   //UI图集图片
        @"Assets/Resources/Res/NavMesh/.+\.asset",   //UI图集图片
    };


        /// <summary>
        /// 目录打包
        /// </summary>
        public static string[] MustDirectoryBuild = new string[] {
            @"Assets/Resources/Res/Character/.+/Animations/.+\.FBX",                //打包成一个目录
        };

        static int directory_build_path_start = "Assets/Resources/".Length;

        /// <summary>
        /// 是否是必须单独打包的.
        /// </summary>
        /// <param name="path"></param>
        public static bool IsMustBuild(string path)
        {
            for (int i = 0; i < MustBuild.Length; i++)
            {
                if (Regex.IsMatch(path, MustBuild[i]))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 是否是必须单独打包的.
        /// </summary>
        /// <param name="path"></param>
        public static bool IsDirectoryBuild(string path)
        {
            for (int i = 0; i < MustDirectoryBuild.Length; i++)
            {
                if (Regex.IsMatch(path, MustDirectoryBuild[i]))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取目录打包的资源路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetDirectoryBuildBundlePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            return path.Remove(0, directory_build_path_start).ToLower();
        }
    }
}
