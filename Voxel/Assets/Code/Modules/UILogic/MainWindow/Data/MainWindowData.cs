using UnityEngine;

public class MainWindowData : BaseUIWindowData
{

    /// <summary>
    /// map 界面信息
    /// </summary>
    [System.Serializable]
    public class MapInfo
    {
        /// <summary>
        /// 2D滚动区域 tran
        /// </summary>
        public Transform scrollTran2D;

        /// <summary>
        /// 3D滚动区域 tran
        /// </summary>
        public Transform scrollTran3D;
    }

    /// <summary>
    /// my 界面信息
    /// </summary>
	[System.Serializable]
    public class MyInfo
    {
        /// <summary>
        /// 滚动区域
        /// </summary>
        public Transform scrollTran;
    }

    /// <summary>
    /// map 界面信息
    /// </summary>
    public MapInfo mapInfo;

    /// <summary>
    /// my 界面信息
    /// </summary>
    public MyInfo myInfo;

    /// <summary>
    /// camera
    /// </summary>
    public GameObject camera;
}
