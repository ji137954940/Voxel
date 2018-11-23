using UnityEngine;
using ZFrame;


namespace Color.Number.Event
{
    /// <summary>
    /// 刷新 Texture 显示内容
    /// </summary>
    public class ME_Texture_Change : ModuleEvent
    {
        /// <summary>
        /// Texture typeId
        /// </summary>
        public int TextureTypeId;

        /// <summary>
        /// Texture Id
        /// </summary>
        public int TextureId;

        /// <summary>
        /// Texture
        /// </summary>
        public Texture2D Texture;
    }
}
