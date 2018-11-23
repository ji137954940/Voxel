using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tgame.AssetBundle
{

    /// <summary>
    /// 文件类型映射
    /// </summary>
    public static class AssetFileType
    {

        /// <summary>
        /// 获取搜索的后缀
        /// </summary>
        /// <returns></returns>
        public static string[] GetSearchPattern()
        {

            var ls = new List<string>();

            var rs = ResourcesExtension;

            var rb = ResourcesCheck;

            for (int i = 0; i < rs.Length; i++)
            {
                if (rb[i])
                {
                    ls.Add("*" + rs[i]);
                }
            }

            return ls.ToArray();
        }

        /// <summary>
        /// 是不是打包允许的后缀
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static bool IsPermissionExtension(string extension)
        {
            var AssetBundleBuildType = GetAssetBundleBuildType(null, extension);

            var AssetBundleBuildTypeIndex = ResourceTypeMaping.IndexOf(AssetBundleBuildType);

            if (AssetBundleBuildTypeIndex != -1)
            {
                return ResourcesCheck[AssetBundleBuildTypeIndex];
            }
            return false;
        }

        /// <summary>
        /// 是否为需要排除的资源数据
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsExcludeResDependenct(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            for (int i = 0; i < ExcludeResDependenct.Length; i++)
            {
                if (path.Contains(ExcludeResDependenct[i]))
                    return true;
            }
            
            return false;
        }

        /// <summary>
        /// 是否为需要排除的资源数据
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsExclude(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            for (int i = 0; i < ExcludeRes.Length; i++)
            {
                if (path.Contains(ExcludeRes[i]))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 需要排除的资源引用路径数据
        /// </summary>
        public static readonly string[] ExcludeResDependenct =
        {
            "LightingData.asset"
        };

        /// <summary>
        /// 需要排除的路径数据
        /// </summary>
        public static readonly string[] ExcludeRes =
        {
            "Assets/Resources/Retain/"
        };

        /// <summary>
        /// 是否包含进入打包内容
        /// </summary>
        public static readonly bool[] ResourcesCheck = { true,true,true,
                                                   true,true,true,true,true,true,true,true,true,true,
                                                   false,true,true,true,true,true,
                                                   true,true,true,true,true,true,true,
                                                   true,true,true,
                                                   true,
                                                   true,true,
                                                   true,true,
                                                   true,
                                                   false,false,false};

        /// <summary>
        /// 文件的后缀分类
        /// </summary>
        public static readonly string[] ResourcesExtension = { ".prefab", ".fbx",".obj",//GameObject
													 ".tif",".png", ".jpg", ".jpeg",".dds", ".gif", ".psd", ".tga", ".bmp",".exr",//Texture
													 ".txt", ".bytes", ".xml", ".csv", ".json",".asset",//TextAsset
													 ".controller",".overrideController", ".shader", ".anim", ".unity", ".mat", ".cubemap",//UnitySupportType
													 ".wav", ".mp3", ".ogg",//Audio System
													 ".shadervariants",//new KeyWord
													 ".ttf",".otf",
                                                     ".flare",".renderTexture",
                                                     ".mask",//动作遮罩
                                                     ".cs",".js",".dll"
                                                    };

        /// <summary>
        /// 对应的Unity类型
        /// </summary>
        public static readonly Type[] ResourcesType = { typeof(GameObject), typeof(GameObject),typeof(GameObject),
                                                        typeof(Texture),typeof(Texture),typeof(Texture), typeof(Texture), typeof(Texture), typeof(Texture), typeof(Texture), typeof(Texture), typeof(Texture),typeof(Texture),
                                                        typeof(TextAsset), typeof(TextAsset), typeof(TextAsset), typeof(TextAsset), typeof(TextAsset),typeof(ScriptableObject),
                                                        typeof(UnityEngine.Object),typeof(AnimatorOverrideController), typeof(Shader), typeof(AnimationClip), null, typeof(Material), typeof(Cubemap),
                                                        typeof(AudioClip), typeof(AudioClip), typeof(AudioClip),
                                                        typeof(ShaderVariantCollection),
                                                        typeof(Flare),typeof(RenderTexture),
                                                        typeof(AvatarMask),
                                                        typeof(Font),typeof(Font)
                                                  };

        /// <summary>
        /// 文件类型分类
        /// </summary>
        public static readonly AssetBundleBuildType[] ResourceTypeMaping = {
                                                                 AssetBundleBuildType.Prefab,AssetBundleBuildType.FBX,AssetBundleBuildType.FBX,
                                                                 AssetBundleBuildType.Texture, AssetBundleBuildType.Texture,AssetBundleBuildType.Texture,AssetBundleBuildType.Texture,AssetBundleBuildType.Texture,AssetBundleBuildType.Texture,AssetBundleBuildType.Texture,AssetBundleBuildType.Texture,AssetBundleBuildType.Texture,AssetBundleBuildType.Texture,
                                                                 AssetBundleBuildType.Text,AssetBundleBuildType.Text,AssetBundleBuildType.Text,AssetBundleBuildType.Text,AssetBundleBuildType.Text,AssetBundleBuildType.ScriptAbleObject,
                                                                 AssetBundleBuildType.Controller,AssetBundleBuildType.OverrideController,AssetBundleBuildType.Shader,AssetBundleBuildType.Animation,AssetBundleBuildType.Scene,AssetBundleBuildType.Material,AssetBundleBuildType.Cubemap,
                                                                 AssetBundleBuildType.Audio,AssetBundleBuildType.Audio,AssetBundleBuildType.Audio,
                                                                 AssetBundleBuildType.ShaderVariants,
                                                                 AssetBundleBuildType.Font,AssetBundleBuildType.Font,
                                                                 AssetBundleBuildType.Flare,AssetBundleBuildType.RenderTexture,
                                                                 AssetBundleBuildType.Mask,
                                                                 AssetBundleBuildType.Script,AssetBundleBuildType.Script,AssetBundleBuildType.Script
                                                             };

        /// <summary>
        /// 获取一个文件类型
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static AssetBundleBuildType GetAssetBundleBuildType(string path, string extension)
        {
            extension = extension.ToLower();

            var index = AssetFileType.ResourcesExtension.IndexOf(extension);

            if (index != -1)
            {
                if (AssetFileType.ResourceTypeMaping.Length > index)
                {
                    AssetBundleBuildType type = AssetFileType.ResourceTypeMaping[index];
                    type = IsDirectoryPackage(path, type);
                    return type;
                }
                else
                {
                    return AssetBundleBuildType.Unkown;
                }
            }

            return AssetBundleBuildType.Unkown;
        }

        /// <summary>
        /// 是否为目录打包
        /// </summary>
        /// <param name="url"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        static AssetBundleBuildType IsDirectoryPackage(string url, AssetBundleBuildType type)
        {
            if (string.IsNullOrEmpty(url))
                return type;

            if (type == AssetBundleBuildType.FBX)
            {
                if (AssetPathRule.IsDirectoryBuild(url))
                {
                    return AssetBundleBuildType.Animation_FBX;
                }

                return type;
            }
            else
            {
                return type;
            }
        }
    }
}
