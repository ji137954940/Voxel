namespace AFrame.AssetCheck
{
    /// <summary>
	/// 资源检查类型
	/// @author LiuLeiLei
	/// @data 3/1/2018
	/// @desc 
	/// </summary>
    public enum CheckEnumType
    {
        /// <summary>
        /// 必要的（一定会查找的）
        /// </summary>
        UnKnow = 0,
        /// <summary>
        /// 贴图
        /// </summary>
        Texture,
        /// <summary>
        /// 粒子
        /// </summary>
        Particle,
        /// <summary>
        /// 材质
        /// </summary>
        Material,
        /// <summary>
        /// 模型
        /// </summary>
        Model,
        /// <summary>
        /// 投影
        /// </summary>
        Projector,
        /// <summary>
        /// 动画
        /// </summary>
        Animation,
        /// <summary>
        /// 脚本
        /// </summary>
        Script,
    }

    /// <summary>
    /// 所有展开的类型
    /// </summary>
    public enum CheckOption
    {
        None = 0,

        #region UnKnow
        /// <summary>
        /// 路径含中文字符
        /// </summary>
        ResourcePath,

        #endregion

        #region Texture
        /// <summary>
        /// 修改图片压缩品质
        /// </summary>
        Texture_Compression,
        /// <summary>
        /// 检查贴图压缩格式
        /// </summary>
        Texture_Format,
        /// <summary>
        /// 检查不符合规定尺寸的贴图
        /// </summary>
        Texture_MaxSize,
        /// <summary>
        /// 贴图Mipmap检查
        /// </summary>
        Texture_Mipmap,
        /// <summary>
        /// 贴图Overridden选项检查
        /// </summary>
        Texture_Overridden,
        /// <summary>
        /// 贴图是否是2的次幂
        /// </summary>
        Texture_PowerOf2,
        /// <summary>
        /// 贴图Read/Write选项检查
        /// </summary>
        Texture_ReadWrite,
        #endregion

        #region Particle
        /// <summary>
        /// 粒子最大数量检查(超出10)
        /// </summary>
        Particle_MaxSize,
        #endregion

        #region Material
        /// <summary>
        /// 检测选中目录下材质球Shader
        /// </summary>
        Material_Shader,
        /// <summary>
        /// 查找出全部的 StandardShader Material
        /// </summary>
        Material_GetAllStandardShader,
        #endregion

        #region Model
        /// <summary>
        /// 模型 AnimationCompression 检查
        /// </summary>
        Model_AnimationCompression,
        /// <summary>
        /// 设置 Npc LOD--1层
        /// </summary>
        Model_LOD,
        /// <summary>
        /// 模型 MeshCompression 检查
        /// </summary>
        Model_MeshCompression,
        /// <summary>
        /// 模型 OptimizeGameObjects 开启检查
        /// </summary>
        Model_OptimizeGameObjects,
        /// <summary>
        /// 模型 Read/Write 检查
        /// </summary>
        Model_ReadWrite,
        #endregion

        #region Projector
        /// <summary>
        /// 投影组件检查
        /// </summary>
        Projector_Check,
        #endregion

        #region Animation
        /// <summary>
        /// 替换震屏参数改成id
        /// </summary>
        Animation_CameraShakeID,
        #endregion

        #region Script
        /// <summary>
        /// 检查脚本编码格式 UTF-8
        /// </summary>
        Script_Format,
        #endregion
    }
}
