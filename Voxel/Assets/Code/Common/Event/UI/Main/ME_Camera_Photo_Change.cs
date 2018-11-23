using UnityEngine;
using ZFrame;
using Color.Number.File;

namespace Color.Number.Event
{
    /// <summary>
    /// 刷新 Texture 显示内容
    /// </summary>
	public class ME_Camera_Photo_Change : ModuleEvent
    {
		/// <summary>
		/// cpi 信息
		/// </summary>
		public CameraPhotoInfo cpi;

        /// <summary>
        /// 是否添加
        /// </summary>
		public bool IsAdd;
    }
}
