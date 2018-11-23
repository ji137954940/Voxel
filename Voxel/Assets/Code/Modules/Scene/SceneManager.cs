using System.Collections;
using System.Collections.Generic;
using Color.Number.Animation;
using Color.Number.Camera;
using Color.Number.Event;
using Color.Number.File;
using Color.Number.GameInfo;
using Color.Number.Grid;
using Color.Number.Voxel;
using Tgame.Game.Icon;
using Tgame.Game.Table;
using UnityEngine;
using ZFrame;
using ZLib;

namespace Color.Number.Scene
{

    /// <summary>
    /// 场景关卡处理
    /// </summary>
    public class SceneManager : Singleton<SceneManager>
    {

        #region 预先处理资源数据

        /// <summary>
        /// 初始化场景信息数据
        /// </summary>
        public void InitSceneInfo()
        {
            //预先加载灰化材质球
            IconManager.instance.SetImageGrey(null, true);

            PlayerGameInfo.instance.SetCompleteInfo(FileManager.instance.GetCompleteInfo());

            PlayerGameInfo.instance.SetCameraPhotoInfo(FileManager.instance.GetCameraPhotoInfo());

            PlayerGameInfo.instance.IconList = Table_Client_Icon.GetAllPrimaryList();

            //添加相机缩放事件监听
            CameraManager.GetInst().AddCameraZAxisChangeAction(OncameraZAxisChangeAction);

        }

        #endregion


        #region 创建关卡内容

        /// <summary>
        /// 创建游戏场景
        /// </summary>
        /// <param name="table"></param>
        /// <param name="cameraPhoto"></param>
        /// <param name="texture"></param>
        public void CreateGameScene(Table_Client_Icon table, CameraPhotoInfo cameraPhoto, Texture2D texture)
        {
            if (texture == null)
            {
                Debug.LogError(" 创建 Game Scene 失败，Texture 为 null ！ ");
                return;
            }

            //解锁输入操作
            PlayerGameInfo.instance.IsAcceptInputOperation = true;
            IconTable = table;

            if (table != null && table.is_voxel)
            {
                IsVoxel = true;

                //设置相机的位置为默认的位置
                CameraManager.GetInst().SetCameraDafultPos();

                //创建voxel
                VoxelManager.instance.CreateVoxel(table);
            }
            else
            {
                IsVoxel = false;

                //设置相机的位置为默认的位置
                CameraManager.GetInst().SetCameraDafultPos();

                //创建 Grid
                GridManager.instance.CreateGrid(table, cameraPhoto, texture);
            }

            //UI显示切换
            UIWindowSwitch(true);
        }

        /// <summary>
        /// UI显示切换
        /// </summary>
        /// <param name="create"></param>
        private void UIWindowSwitch(bool create)
        {
            if (create)
            {
                //打开操作界面
                UIWindowManager.instance.Open<OperationModule>();
                //隐藏主界面
                UIWindowManager.instance.Visible<MainWindowModule>(false);
                //关闭相机拍照界面
                UIWindowManager.instance.Close<CameraPhotoModule>();

            }
            else
            {
                //显示主控界面
                UIWindowManager.instance.Visible<MainWindowModule>(true);
                //隐藏操作界面
                //UIWindowManager.instance.Visible<OperationModule>(false);
                UIWindowManager.instance.Close<OperationModule>();
            }
        }

        #endregion

        #region 退出游戏关卡

        /// <summary>
        /// 退出当前游戏关卡
        /// </summary>
        public void ExitGameScene()
        {
            //存储grid信息
            var flag = SaveInfo();

            //UI显示切换
            UIWindowSwitch(false);

            //如果有对数据进行操作改变，那么处理图片切换操作，否则不处理
            if(flag)
                ShowTextureChange();

            //解锁输入操作
            PlayerGameInfo.instance.IsAcceptInputOperation = false;

            //重置颜色的选择设置
            PlayerGameInfo.instance.ResetSelectInfo();

            IsVoxel = false;

            //清理无用信息数据
            ClearGameSceneInfo();
        }

        /// <summary>
        /// 存储 grid 信息
        /// </summary>
        /// <returns></returns>
        private bool SaveInfo()
        {
			if (IsVoxel)
			{
				var flag = VoxelManager.instance.SaveVoxelColorInfo ();

				if(flag)
					return FileManager.instance.SaveColorInfo(VoxelManager.instance.GameLevelVoxelInfo);
				
				return flag;
			}
			else 
			{
				var flag = TilemapManager.instance.SaveTilemapInfo();

				if(flag)
					return FileManager.instance.SaveColorInfo(GridManager.instance.GameLevelGridInfo);

				return flag;
			}
            
        }

        /// <summary>
        /// 显示 Texture 图片改变
        /// </summary>
        private void ShowTextureChange()
        {
			int typeId;
			int id;

			if (IsVoxel)
			{
				var info = VoxelManager.instance.GameLevelVoxelInfo;
				typeId = info.TypeId;
				id = info.Id;
			}
			else
			{
				var info = GridManager.instance.GameLevelGridInfo;
				typeId = info.TypeId;
				id = info.Id;
			}

            Frame.DispatchEvent(ME.New<ME_Texture_Change>(p =>
                {
                    p.TextureTypeId = typeId;
                    p.TextureId = id;
                    p.Texture = PlayerGameInfo.instance.GetSaveTexture(typeId, id);
                })); 
        }

        /// <summary>
        /// 清理游戏关卡内部信息
        /// </summary>
        public void ClearGameSceneInfo()
        {
            GridManager.instance.Clear();
            TilemapManager.instance.Clear();

            VoxelManager.instance.Clear();
        }

        #endregion

        #region 相机拍照

        /// <summary>
        /// 相机拍照功能
        /// </summary>
        public void CameraPhoto(bool isExit)
        {
            if (!isExit)
            {
                //进入拍照模式
                UIWindowManager.instance.Open<CameraPhotoModule>();

                //隐藏主界面
                UIWindowManager.instance.Visible<MainWindowModule>(false);
            }
            else
            {
                //退出拍照模式
                UIWindowManager.instance.Close<CameraPhotoModule>();

                //显示主控界面
                UIWindowManager.instance.Visible<MainWindowModule>(true);
            }
        }

        /// <summary>
        /// 创建 相机 photo 文件信息
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private CameraPhotoInfo CreateCameraPhotoInfo(int id, string name, string path)
        {
            CameraPhotoInfo info = new CameraPhotoInfo();
            info.type_id = ConstantConfig.GetGameConfigInt(GameConfigKey.camera_photo_type_id);
            info.id = id;
            info.name = name;
            info.path = path;

            return info;
        }

        /// <summary>
        /// 存储拍照图片信息数据
        /// </summary>
        /// <param name="tex"></param>
        public void SaveCameraPhotoInfo(Texture2D tex)
        {
            var id = PlayerGameInfo.instance.GetNewCameraPhotoInfoId();
            var path = string.Format("{0}/{1}.png", ConstantConfig.GetGameConfigString(GameConfigKey.camera_photo_path), id);

            var info = CreateCameraPhotoInfo(id, "test", path);
            PlayerGameInfo.instance.AddCameraPhotoInfo(info);

            FileManager.instance.SaveCameraPhotoInfo(info, tex);

			//通知UI添加图片
			Frame.DispatchEvent(ME.New<ME_Camera_Photo_Change>(p =>
				{
					p.cpi = info;
					p.IsAdd = true;
				})); 

            //数据存储完成，创建游戏关卡
            CreateGameScene(null, info, tex);
        }

        #endregion

        #region 保存图片

        /// <summary>
        /// 保存截图到相册
        /// </summary>
        public void SaveCaptureScreenshot()
        {
            if(!IsVoxel)
            {
                if(GridManager.instance.GameLevelGridInfo.IsAnimation)
                {
                    CameraManager.GetInst().SaveCaptureScreenshot(GridManager.instance.GameLevelGridInfo.CaptureScreenshot, null);
                    return;
                }
            }

            CameraManager.GetInst().SaveCaptureScreenshot(null);
        }

        #endregion

        #region 保存视频

        /// <summary>
        /// 保存视频
        /// </summary>
        public void SaveVideo()
        {
            if (!IsVoxel)
                FFmpegManager.instance.SaveVideo(GridManager.instance.GameLevelGridInfo);
            else
                FFmpegManager.instance.SaveVideo(VoxelManager.instance.GameLevelVoxelInfo);
        }

        #endregion

        #region 数据获取

        /// <summary>
        /// 当前是否为 voxel
        /// </summary>
        public bool IsVoxel { get; private set; }
        /// <summary>
        /// 当前 level 中的 icon 配置数据
        /// </summary>
        public Table_Client_Icon IconTable { get; private set; }

        /// <summary>
        /// 是否当前颜色填充完成
        /// </summary>
        /// <returns></returns>
        public bool IsColoringComplete()
        {
            if (!IsVoxel && GridManager.instance.GameLevelGridInfo != null)
                return GridManager.instance.GameLevelGridInfo.IsTextureColoringComplete;

            if (IsVoxel && VoxelManager.instance.GameLevelVoxelInfo != null)
                return VoxelManager.instance.GameLevelVoxelInfo.IsVoxelColoringComplete;

            return false;
        }

        /// <summary>
        /// 获取颜色类型数据
        /// </summary>
        /// <returns></returns>
        public PosColorInfo[] GetCurrGameLevelColorInfo()
        {
            if (!IsVoxel && GridManager.instance.GameLevelGridInfo != null)
                return GridManager.instance.GameLevelGridInfo.PixelColorArr;

            if (IsVoxel && VoxelManager.instance.GameLevelVoxelInfo != null)
                return VoxelManager.instance.GameLevelVoxelInfo.VoxelColorInfoArr;

            return null;
        }

        /// <summary>
        /// 获取颜色填充完成区域列表
        /// </summary>
        /// <returns></returns>
        public int[] GetCurrGameLevelColorCompleteAreaIdList()
        {
            if (!IsVoxel
                && GridManager.instance.GameLevelGridInfo != null
                && GridManager.instance.GameLevelGridInfo.PixelColorCompleteAreaIdList != null)
                return GridManager.instance.GameLevelGridInfo.PixelColorCompleteAreaIdList.ToArray();

            if (IsVoxel 
                && VoxelManager.instance.GameLevelVoxelInfo != null
                && VoxelManager.instance.GameLevelVoxelInfo.VoxelColorCompleteAreaIdList != null)
                return VoxelManager.instance.GameLevelVoxelInfo.VoxelColorCompleteAreaIdList.ToArray();

            return null;
        }

        /// <summary>
        /// 设置选中的颜色信息数据
        /// </summary>
        /// <param name="id"></param>
        public void SetSelectColorInfo(int id)
        {
            if (id <= 0)
                id = 1;

            PlayerGameInfo.instance.SelectColorId = id;

            if (IsVoxel)
                PlayerGameInfo.instance.SelectColor = VoxelManager.instance.GameLevelVoxelInfo.VoxelColorInfoArr[id - 1].PosColor;
            else
                PlayerGameInfo.instance.SelectColor = GridManager.instance.GameLevelGridInfo.PixelColorArr[id - 1].PosColor;
        }

        /// <summary>
        /// 获取 gif 图片动画信息
        /// </summary>
        public List<GifAnimation.GifTexture> GetGifAnimation()
        {
            if (IconTable != null && IconTable.is_animation)
            {
                var list = GifAnimation.instance.GetGifAnimation(IconTable.type_id, IconTable.id);
                //创建序列帧动画
                //FFmpegManager.instance.CreateOriginalDynamicVideo(GridManager.instance.GameLevelGridInfo, list);
                return list;
            }

            return null;
        }

        #endregion

        #region 刷新数据信息

        /// <summary>
        /// 刷新 tile 或者 voxel 颜色显示
        /// </summary>
        public void UpdateTileOrVoxelColor()
        {
            if (IsVoxel)
            {
				VoxelManager.instance.SetSelectVoxelAreaColor ();
            }
            else
            {
                TilemapManager.instance.SetBgTilemapTileColor();
            }
        }

        /// <summary>
        /// 显示颜色填充过程
        /// </summary>
        public void ShowColoringProcess()
        {
            //设置相机的位置为默认的位置
            CameraManager.GetInst().SetCameraDafultPos();

            if (IsVoxel)
            {
                VoxelManager.instance.ShowTilemapColoringProcess();
            }
            else
            {
                TilemapManager.instance.ShowTilemapColoringProcess();
            }
        }

        #endregion

		#region 点击操作

		/// <summary>
		/// 点击场景中的对象操作
		/// </summary>
		/// <param name="pos">Position.</param>
		public void ClickGameObject(Vector3 pos, Vector3 prePos, bool isComplementPoint = false)
		{
			if (!IsVoxel)
			{
                if(!isComplementPoint || prePos == Vector3.zero)
                {
                    var worldPosition = CameraManager.GetInst().ScreenToWorldPoint(pos);

                    TilemapManager.instance.ClickTilemap(worldPosition);
                }
                else
                {
                    //路径开始补点
                    GridPathComplementPoint(pos, prePos);
                }
				
			}
			else
			{
                if (!isComplementPoint || prePos == Vector3.zero)
                {
                    RaycastClickVoxel(pos);
                }
                else
                {
                    VoxelPathComplementPoint(pos, prePos);
                }
			}
		}

        /// <summary>
        /// 射线检测选中的物体
        /// </summary>
        /// <param name="pos"></param>
        private void RaycastClickVoxel(Vector3 pos)
        {
            var ray = CameraManager.GetInst().ScreenPointToRay(pos);
            RaycastHit hitInfo;

            //只检测 voxel layer 数据
            if (Physics.Raycast(ray, out hitInfo))
            {
                var go = hitInfo.collider.gameObject;
                VoxelManager.instance.ClickVoxel(int.Parse(go.name));
            }
        }

        /// <summary>
        /// grid 路径补点
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="prePos"></param>
        private void GridPathComplementPoint(Vector3 pos, Vector3 prePos)
        {
            var preWorldPos = CameraManager.GetInst().ScreenToWorldPoint(prePos);
            var worldPos = CameraManager.GetInst().ScreenToWorldPoint(pos);

            var dir = (worldPos - preWorldPos).normalized;
            float d = ConstantConfig.GetGameConfigFloat(GameConfigKey.grid_path_complement_point_dis);
            var tempPos = Vector3.zero;
            while (true)
            {

                tempPos = preWorldPos + dir * d;

                if (Vector3.Dot(tempPos - preWorldPos, worldPos - tempPos) > 0)
                {
                    TilemapManager.instance.ClickTilemap(tempPos);

                    preWorldPos = tempPos;
                }
                else
                {
                    TilemapManager.instance.ClickTilemap(worldPos);

                    return;
                }
            }
        }

        /// <summary>
        /// voxel 路径补点
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="prePos"></param>
        private void VoxelPathComplementPoint(Vector3 pos, Vector3 prePos)
        {

            var currDis = (1 - _cameraZAxisValueRatio)
                            * (ConstantConfig.GetGameConfigFloat(GameConfigKey.voxel_path_complement_point_max_dis) - ConstantConfig.GetGameConfigFloat(GameConfigKey.voxel_path_complement_point_min_dis))
                            + ConstantConfig.GetGameConfigFloat(GameConfigKey.voxel_path_complement_point_min_dis);

            var preWorldPos = prePos;
            var worldPos = pos;

            var dir = (worldPos - preWorldPos).normalized;
            var tempPos = Vector3.zero;
            while (true)
            {

                tempPos = preWorldPos + dir * currDis;

                if (Vector3.Dot(tempPos - preWorldPos, worldPos - tempPos) > 0)
                {
                    RaycastClickVoxel(tempPos);

                    preWorldPos = tempPos;
                }
                else
                {
                    RaycastClickVoxel(worldPos);

                    return;
                }
            }
        }

        /// <summary>
        /// 相机Z轴数值所处比例
        /// </summary>
        private float _cameraZAxisValueRatio = 0;

        /// <summary>
        /// 相机的Z轴数值改变
        /// </summary>
        /// <param name="f"></param>
        private void OncameraZAxisChangeAction(float f)
        {
            _cameraZAxisValueRatio = f;
        }

        #endregion
    }
}


