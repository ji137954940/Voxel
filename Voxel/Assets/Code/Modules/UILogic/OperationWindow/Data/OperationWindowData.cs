using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OperationWindowData : BaseUIWindowData
{

    /// <summary>
    /// 保存信息数据
    /// </summary>
    [System.Serializable]
    public class SaveInfo
    {
        /// <summary>
        /// 保存信息界面
        /// </summary>
        public GameObject panel;
        /// <summary>
        /// 保存为图像
        /// </summary>
        public GameObject image;
        /// <summary>
        /// 保存为视频
        /// </summary>
        public GameObject video;
    }

    /// <summary>
    /// 提示信息数据
    /// </summary>
    [System.Serializable]
    public class TipsInfo
    {
        /// <summary>
        /// 界面panel
        /// </summary>
        public GameObject panel;

        /// <summary>
        /// 显示内容
        /// </summary>
        public Text text;
    }

    /// <summary>
    /// 返回按钮
    /// </summary>
    public GameObject backBtn;

    /// <summary>
    /// 重置位置按钮（Voxel的状态下显示）
    /// </summary>
    public GameObject resetBtn;

    /// <summary>
    /// 完成按钮
    /// </summary>
    public GameObject okBtn;

    /// <summary>
    /// 滚动区域 content 父对象
    /// </summary>
    public Transform scrollContentTran;

    /// <summary>
    /// play frame 动画
    /// </summary>
    public PlayFrameAnimation playFrameAnimation;

    /// <summary>
    /// 保存信息数据
    /// </summary>
    public SaveInfo saveInfo;

    /// <summary>
    /// 提示信息数据
    /// </summary>
    public TipsInfo tipsInfo;
}
