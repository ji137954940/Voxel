using Color.Number.Model;
using Color.Number.Utils;
using System.Collections.Generic;
using Tgame.Game.Table;
using UnityEngine;
using ZLib;
using ZTool.Res;
using Color.Number.Camera;

namespace Color.Number.Voxel
{

    public class VoxelManager : Singleton<VoxelManager>
    {

        //#region Voxel 构建

        ///// <summary>
        ///// voxel size
        ///// </summary>
        //private int _voxelTileSize = 1;

        ///// <summary>
        ///// voxel 最大区域
        ///// </summary>
        //private int _voxelMaxSizeX = 128;
        //private int _voxelMaxSizeY = 128;
        //private int _voxelMaxSizeZ = 128;

        ///// <summary>
        ///// 存储所有的 voxel info
        ///// x, y, z
        ///// </summary>
        //private VoxelInfo[,,] _voxelInfoArr;

        ///// <summary>
        ///// 对所有区域进行 voxel 
        ///// </summary>
        //public void VoxelAllArea(ModelTriangle[] tris)
        //{
        //    //获取当前空间中 voxel 总个数
        //    var tileCountX = _voxelMaxSizeX / _voxelTileSize;
        //    var tileCountY = _voxelMaxSizeY / _voxelTileSize;
        //    var tileCountZ = _voxelMaxSizeZ / _voxelTileSize;

        //    //创建 voxel 数组
        //    _voxelInfoArr = new VoxelInfo[tileCountX, tileCountY, tileCountZ];

        //    //获取 voxel 基本信息
        //    var voxelSize = new Vector3(_voxelTileSize, _voxelTileSize, _voxelTileSize);
        //    var voxelPosHalfExtents = voxelSize * 0.5f;

        //    //var startPos = new Vector3(-_voxelMaxSizeX / 2, -_voxelMaxSizeY / 2, -_voxelMaxSizeZ / 2);

        //    //var startPos = new Vector3(0, 0, 0);

        //    //List<VoxelInfo> voxelInfoList = new List<VoxelInfo>();

        //    //var pos = Vector3.zero;
        //    //for (int i = 0; i < tileCountX; i++)
        //    //{
        //    //    for (int j = 0; j < tileCountY; j++)
        //    //    {
        //    //        for (int k = 0; k < tileCountZ; k++)
        //    //        {
        //    //            pos.x = startPos.x + i * _voxelTileSize + _voxelTileSize / 2;
        //    //            pos.y = startPos.y + j * _voxelTileSize + _voxelTileSize / 2;
        //    //            pos.z = startPos.z + k * _voxelTileSize + _voxelTileSize / 2;

        //    //            var isCollider = Physics.CheckBox(pos, voxelPosHalfExtents);
        //    //            if (isCollider)
        //    //            {
        //    //                var voxel = new VoxelInfo(pos, voxelSize);
        //    //                voxel.Id = voxelInfoList.Count;
        //    //                voxelInfoList.Add(voxel);
        //    //            }

        //    //        }
        //    //    }
        //    //}

        //    //var count = voxelInfoList.Count;
        //    //Debug.LogError(" 扫描出 cube 个数为 " + count);
        //    //GameObject root = new GameObject();
        //    //var rootTran = root.transform;
        //    //for (int i = 0; i < count; i++)
        //    //{
        //    //    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    //    go.name = i.ToString();
        //    //    var tran = go.transform;
        //    //    tran.SetParent(rootTran);
        //    //    tran.SetPositionAndRotation(voxelInfoList[i].Pos, Quaternion.identity);
        //    //    tran.localScale = voxelSize;
        //    //}

        //    var treeNodeList = new QuadtreeBuilder().BuildQuadtree(tris, new Vector3(_voxelMaxSizeX, _voxelMaxSizeY, _voxelMaxSizeZ), _voxelTileSize);

        //    if (treeNodeList == null || treeNodeList.Count == 0)
        //    {
        //        Debug.LogError(" 构建四叉树结构返回为 null ");
        //        return;
        //    }
        //    else
        //    {
        //        Debug.Log(" 通过四叉树扫描出来的node个数为 " + treeNodeList.Count + " 个");
        //    }

        //    var triDic = new Dictionary<int, ModelTriangle>();

        //    var treeNodeCount = treeNodeList.Count;
        //    GameObject root = new GameObject();
        //    var rootTran = root.transform;
        //    for (int i = 0; i < treeNodeCount; i++)
        //    {
        //        //检测当前 node 是否包含三角形或者和三角形相交
        //        var triList = treeNodeList[i].TriangleList;
        //        for (int j = 0; j < triList.Count; j++)
        //        {
        //            if (treeNodeList[i].NodeBounds.ContainsTriangle(triList[j])
        //                || GameUtils.Intersect(treeNodeList[i], triList[j]))
        //            {
        //                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //                go.name = i.ToString();
        //                var tran = go.transform;
        //                tran.SetParent(rootTran);
        //                tran.SetPositionAndRotation(treeNodeList[i].NodeBounds.center, Quaternion.identity);
        //                tran.localScale = Vector3.one * _voxelTileSize;
        //            }
        //        }


        //        //GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //        //go.name = i.ToString();
        //        //var tran = go.transform;
        //        //tran.SetParent(rootTran);
        //        //tran.SetPositionAndRotation(treeNodeList[i].NodeBounds.center, Quaternion.identity);
        //        //tran.localScale = Vector3.one * _voxelTileSize;

        //        //for (int j = 0; j < treeNodeList[i].TriangleList.Count; j++)
        //        //{
        //        //    if (!triDic.ContainsKey(treeNodeList[i].TriangleList[j].Id))
        //        //        triDic[treeNodeList[i].TriangleList[j].Id] = treeNodeList[i].TriangleList[j];
        //        //}


        //    }

        //    Debug.LogError(" 包含三角形总个数为 " + triDic.Count);
        //}

        //#endregion




		private VoxelInfo _GameLevelVoxelInfo;
        /// <summary>
        /// 当前scene使用的voxel数据
        /// </summary>
        public VoxelInfo GameLevelVoxelInfo
        {
            get { return _GameLevelVoxelInfo; }
        }


		#region 初始化

		public VoxelManager()
		{
			CameraManager.GetInst().AddCameraZAxisChangeAction(OnCameraZAxisChange);
		}

		#endregion

        #region Voxel 模型构建

        /// <summary>
        /// 创建 Voxel 信息数据
        /// </summary>
        /// <param name="iconTable"></param>
        public void CreateVoxel(Table_Client_Icon iconTable)
        {
            if (iconTable == null)
            {
                Debug.LogError(" Create Voxel Failure Icon_Table Is Null!");
                return;
            }

            //加载voxel二进制数据
			ResourcesLoad.instance.LoadBytes(iconTable.voxel_path, OnVoxelBytesLoadDone, iconTable, false);
        }

        /// <summary>
        /// voxel 二进制数据加载完成
        /// </summary>
        /// <param name="path"></param>
        /// <param name="obj"></param>
        /// <param name="parameter"></param>
        private void OnVoxelBytesLoadDone(string path, object obj, object parameter)
        {
            if (obj == null)
            {
                Debug.LogError(" 加载资源 " + path + " 失败！ ");
                return;
            }

            var bytes = (byte[]) obj;

            //解析voxel数据信息
            var data = VoxFile.LoadVoxel(bytes);

			var table = (Table_Client_Icon)parameter;

            //默认为第0帧的数据
            _GameLevelVoxelInfo = new VoxelInfo();
			_GameLevelVoxelInfo.SetVoxelInfo(data, 0, table.type_id, table.id);

            //创建 voxel
            CreateVoxelShowInfo();

        }

        #endregion

        #region Voxel 显示操作

        private VoxelShowInfo _voxelShowInfo;

        /// <summary>
        /// 创建 voxel 显示相关信息Ø
        /// </summary>
        private void CreateVoxelShowInfo()
        {
            _voxelShowInfo = GameInstance.Main.voxelShowInfo;

            if (_voxelShowInfo != null)
            {
                ShowPanelActive(true);

                _voxelShowInfo.InitVoxelShowInfo(GameLevelVoxelInfo);
            }
        }

        /// <summary>
        /// 显隐操作
        /// </summary>
        /// <param name="active"></param>
        public void ShowPanelActive(bool active)
        {
            if(_voxelShowInfo != null)
                _voxelShowInfo.Panel.SetActive(active);
        }

		private void OnCameraZAxisChange(float f)
		{
			if (_voxelShowInfo != null)
				_voxelShowInfo.ChangeShowColorAlphaAndTexture (f);
		}

		/// <summary>
		/// 设置 选中 voxel 颜色值
		/// </summary>
		public void SetSelectVoxelAreaColor()
		{
			if(_voxelShowInfo != null)
				_voxelShowInfo.SetSelectVoxelAreaColor();
		}

		/// <summary>
		/// 存储 voxel 颜色信息数据
		/// </summary>
		/// <returns><c>true</c>, if voxel color info was saved, <c>false</c> otherwise.</returns>
		public bool SaveVoxelColorInfo()
		{
			if (_voxelShowInfo != null)
				return _voxelShowInfo.SaveVoxelColorInfo ();

			return false;
		}

		/// <summary>
		/// 点击场景中的 voxel
		/// </summary>
		/// <param name="id">Identifier.</param>
		public void ClickVoxel(int id)
		{
			if (_voxelShowInfo != null)
				_voxelShowInfo.VoxelColorFill (id);
		}

        /// <summary>
        /// 更改当前显示颜色信息
        /// </summary>
        /// <param name="f"></param>
        public void ChangeShowColorAlphaAndTexture(float f)
        {
            if (_voxelShowInfo != null)
                _voxelShowInfo.ChangeShowColorAlphaAndTexture(f);
        }

        /// <summary>
        /// 展示整个颜色填充过程
        /// </summary>
        public void ShowTilemapColoringProcess()
        {
            if (_voxelShowInfo != null)
                CoroutineManager.instance.StartCoroutine(_voxelShowInfo.ShowVoxelColoringProcess());
        }

        #endregion

        #region 数据清理

        /// <summary>
        /// 数据清理
        /// </summary>
        public void Clear()
        {
            if (_voxelShowInfo != null)
            {
                _voxelShowInfo.Clear();

                ShowPanelActive(false);
            }
        }

        #endregion

    }
}



