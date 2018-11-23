using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


public class CameraPhotoData : BaseUIWindowData
{

    /// <summary>
    /// 拍摄中对象数据
    /// </summary>
    [System.Serializable]
    public class Shooting
    {
        /// <summary>
        /// panel
        /// </summary>
        public GameObject panel;

        /// <summary>
        /// 拍摄
        /// </summary>
        public GameObject cameraPhoto;

        /// <summary>
        /// 翻转相机
        /// </summary>
        public GameObject flipCamera;
    }

    /// <summary>
    /// 拍摄完成对象数据
    /// </summary>
    [System.Serializable]
    public class Filming
    {
        /// <summary>
        /// panel
        /// </summary>
        public GameObject panel;

        /// <summary>
        /// 重拍
        /// </summary>
        public GameObject remake;

        /// <summary>
        /// 使用
        /// </summary>
        public GameObject use;
    }

    /// <summary>
    /// 返回
    /// </summary>
    public GameObject back;

    /// <summary>
    /// 显示灰色相机照射图片
    /// </summary>
    public RawImage greyColorPhoto;

    /// <summary>
    /// 难易程度 slider
    /// </summary>
    public Slider slider;

    /// <summary>
    /// 拍摄中
    /// </summary>
    public Shooting shooting;

    /// <summary>
    /// 拍摄完成
    /// </summary>
    public Filming filming;

}

