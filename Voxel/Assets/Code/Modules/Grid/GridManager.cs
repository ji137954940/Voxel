using System.Collections;
using System.Collections.Generic;
using Color.Number.File;
using Tgame.Game.Table;
using UnityEngine;
using ZLib;

namespace Color.Number.Grid
{

    /// <summary>
    /// Grid 管理
    /// </summary>
    public class GridManager : Singleton<GridManager>
    {

        #region 构建Grid信息

        /// <summary>
        /// 当前正在使用的 Grid 信息
        /// </summary>
        public GridInfo GameLevelGridInfo { get; private set; }

        /// <summary>
        /// 根据图片创建grid
        /// </summary>
        /// <param name="table"></param>
        /// <param name="cameraPhoto"></param>
        /// <param name="texture2D"></param>
        public void CreateGrid(Table_Client_Icon table, CameraPhotoInfo cameraPhoto, Texture2D texture2D)
        {
            if (texture2D == null)
            {
                Debug.LogError(" Create Grid Failure Texture2D Is Null!");
                return;
            }

            //获取颜色信息数据
            var arr = GetTexturePixel(texture2D);

            GameLevelGridInfo = new GridInfo();

            GameLevelGridInfo.SetColorInfo(table, cameraPhoto, arr, texture2D.width, texture2D.height);

            //创建Grid

            TilemapManager.instance.CreateTilemap(GameLevelGridInfo);
        }

        /// <summary>
        /// 获取图片的所有像素颜色信息
        /// </summary>
        /// <param name="texture2D"></param>
        /// <returns></returns>
        private UnityEngine.Color[] GetTexturePixel(Texture2D texture2D)
        {
            if (texture2D == null)
            {
                Debug.LogError(" Can Not Get Pixel Texture2D Is NUll ");
                return null;
            }

            var arr = texture2D.GetPixels();

            return arr;
        }



        #endregion


        #region 显隐操作

        /// <summary>
        /// panel 显隐操作
        /// </summary>
        /// <param name="active"></param>
        public void ShowPanelActive(bool active)
        {
            TilemapManager.instance.ShowPanelActive(active);
        }

        #endregion


        #region 数据清理

        /// <summary>
        /// 数据清理
        /// </summary>
        public void Clear()
        {
            if (GameLevelGridInfo != null)
            {
                GameLevelGridInfo.Clear();
                GameLevelGridInfo = null;
            }
        }

        #endregion

    }
}


