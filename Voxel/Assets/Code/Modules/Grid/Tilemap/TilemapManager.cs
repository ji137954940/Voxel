using Color.Number.Camera;
using Color.Number.GameInfo;
using UnityEngine;
using ZLib;

namespace Color.Number.Grid
{

    public class TilemapManager : Singleton<TilemapManager>
    {

        /// <summary>
        /// 当前正在使用的 tilemap 对象
        /// </summary>
        private TilemapInfo _tilemapInfo;

        #region 初始化

        public TilemapManager()
        {
            CameraManager.GetInst().AddCameraZAxisChangeAction(OnCameraZAxisChange);
        }

        #endregion

        #region 创建 Tilemap

        /// <summary>
        /// 创建 Tilemap 信息
        /// </summary>
        /// <param name="gridInfo"></param>
        public void CreateTilemap(GridInfo gridInfo)
        {
            if (gridInfo == null)
            {
                Debug.LogError(" Can Not Create Tilemap Info, GridInfo Is Null ");
                return;
            }

            _tilemapInfo = GameInstance.Main.tileMapInfo;

            //CoroutineManager.instance.StartCoroutine(_tilemapInfo.InitBgTilemap(gridInfo));
            if (_tilemapInfo != null)
            {

                ShowPanelActive(true);

                _tilemapInfo.InitTilemap(gridInfo);
                
                //设置默认的 alpha值
                OnCameraZAxisChange(0);
            }
        }

        /// <summary>
        /// 展示 tilemap 上色过程
        /// </summary>
        public void ShowTilemapColoringProcess()
        {
            if (_tilemapInfo != null)
            {
                _tilemapInfo.ShowTilemapColoringProcess();
            }
        }

        #endregion

        #region tilemap 中 tile 的颜色和alpha值改变

        /// <summary>
        /// 相机 Z 值改变时的回调
        /// </summary>
        /// <param name="f"></param>
        private void OnCameraZAxisChange(float f)
        {
            if (_tilemapInfo != null)
                _tilemapInfo.TilemapAlpha(f);
        }

        /// <summary>
        /// 设置 bg tilemap 颜色值
        /// </summary>
        public void SetBgTilemapTileColor()
        {
            if(_tilemapInfo != null)
                _tilemapInfo.SetBgTilemapTileColor(PlayerGameInfo.instance.SelectColorId);
        }

        #endregion

        #region Tilemap 坐标转换

        /// <summary>
        /// 点击 tilemap 处理
        /// </summary>
        /// <param name="worldPosition"></param>
        public void ClickTilemap(Vector3 worldPosition)
        {
            if(_tilemapInfo != null)
                _tilemapInfo.ClickTilemap(worldPosition);
        }

        #endregion

        #region 存储 grid 信息

        /// <summary>
        /// 保存 tile map 信息
        /// </summary>
        /// <returns></returns>
        public bool SaveTilemapInfo()
        {
            if (_tilemapInfo != null)
                return _tilemapInfo.SaveTilemapInfo();

            return false;
        }

        #endregion

        #region 显隐操作

        /// <summary>
        /// panel 显隐操作
        /// </summary>
        /// <param name="active"></param>
        public void ShowPanelActive(bool active)
        {
            if(_tilemapInfo != null)
                _tilemapInfo.Panel.SetActive(active);
        }

        #endregion

        #region 数据清理

        /// <summary>
        /// 数据清理
        /// </summary>
        public void Clear()
        {
            if (_tilemapInfo != null)
            {
                _tilemapInfo.Clear();
                ShowPanelActive(false);
            }
        }

        #endregion
    }
}


