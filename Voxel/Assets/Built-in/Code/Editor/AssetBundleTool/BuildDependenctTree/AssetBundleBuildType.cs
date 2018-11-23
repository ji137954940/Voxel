
namespace Tgame.AssetBundle
{

    /// <summary>
    /// 进行资源打包的文件类型
    /// </summary>
    [System.Flags]
    public enum AssetBundleBuildType
    {
        None = 0,
        /// <summary>
        /// 贴图
        /// </summary>
        Texture = 1 << 1,
        /// <summary>
        /// 动画控制器
        /// </summary>
        Controller = 1 << 2,
        /// <summary>
        /// 重载动画控制器
        /// </summary>
        OverrideController = 1 << 3,
        /// <summary>
        /// 预制件
        /// </summary>
        Prefab = 1 << 4,
        /// <summary>
        /// 美术素材
        /// </summary>
        FBX = 1 << 5,
        /// <summary>
        /// 材质球
        /// </summary>
        Material = 1 << 6,
        /// <summary>
        /// shader
        /// </summary>
        Shader = 1 << 7,
        /// <summary>
        /// 音乐
        /// </summary>
        Audio = 1 << 8,
        /// <summary>
        /// 字体文件
        /// </summary>
        Font = 1 << 9,
        /// <summary>
        /// 脚本文件
        /// </summary>
        Script = 1 << 10,

        /// <summary>
        /// 文本
        /// </summary>
        Text = 1 << 11,

        /// <summary>
        /// 动画
        /// </summary>
        Animation = 1 << 12,

        /// <summary>
        /// 场景文件
        /// </summary>
        Scene = 1 << 13,

        ScriptAbleObject = 1 << 14,
        /// <summary>
        /// Shader的Variants
        /// </summary>
        ShaderVariants = 1 << 15,

        Flare = 1 << 16,
        /// <summary>
        /// Animator mask
        /// </summary>
        Mask = 1 << 17,
        /// <summary>
        /// RT
        /// </summary>
        RenderTexture = 1 << 18,

        //动画 FBX文件
        Animation_FBX = 1 << 19,

        //场景 cubemap 文件
        Cubemap = 1 << 20,

        /// <summary>
        /// 未知
        /// </summary>
        Unkown = 1 << 21,
    }

}