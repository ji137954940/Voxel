using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Color.Number.Grid;
using Color.Number.Utils;
using Color.Number.Voxel;
using UnityEngine;
using System;
using Color.Number.Event;
using Color.Number.File;
using Color.Number.GameInfo;
using ZFrame;

/// <summary>
/// voxel 信息引用
/// </summary>
[System.Serializable]
public class VoxelInfo
{

    /// <summary>
    /// 当前选中的图片 typeId
    /// </summary>
    public int TypeId { get; private set; }

    /// <summary>
    /// 当前选中的图片id
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// 当前voxel颜色是否全部填充完成
    /// </summary>
    public bool IsVoxelColoringComplete { get; private set; }

    /// <summary>
    /// voxel颜色完成顺序 （记录的为id）
    /// </summary>
    public List<int> VoxelColorCompleteOrder { get; private set; }

    /// <summary>
    /// voxel颜色完成 区域id 数组
    /// </summary>
    public List<int> VoxelColorCompleteAreaIdList { get; private set; }

    /// <summary>
    /// vox 数据信息
    /// </summary>
    private VoxData _voxData;

    /// <summary>
    /// 单位 voxel 信息，包含是否显示，以及使用颜色信息
    /// </summary>
    public Voxel[,,] Voxels;

    /// <summary>
    /// voxel 对象的大小
    /// </summary>
    public int SizeX { get; private set; }
    public int SizeY { get; private set; }
    public int SizeZ { get; private set; }

    #region 创建 voxel


    /// <summary>
	/// 创建 voxel 信息
    /// </summary>
    /// <param name="voxData">Vox data.</param>
    /// <param name="frame">Frame.</param>
    /// <param name="typeId">Type identifier.</param>
    /// <param name="id">Identifier.</param>
    public void SetVoxelInfo(VoxData voxData, int frame, int typeId, int id)
    {

		this.TypeId = typeId;
		this.Id = id;

		//读取是否已经完成的数据信息
		IsVoxelColoringComplete = PlayerGameInfo.instance.IsCompleteInfo(TypeId, Id);

		//if (!IsVoxelColoringComplete)
		{
			//数据初始化
			InitVoxels (voxData, frame);

			//处理是否需要显示
			FixVisible ();

			//voxel 颜色信息排序
			VoxelColorInfoSort ();

			//获取已经完成的 voxel信息数据
			GetVoxelCompleteInfo ();

			//设置默认选中颜色数据信息
			SetDefaultSelectColorInfo();
		} 
		//else 
		//{
  //          //数据初始化
  //          InitVoxels(voxData, frame);

  //          //处理是否需要显示
  //          FixVisible();

  //          //获取已经完成的 voxel信息数据
  //          GetVoxelCompleteInfo ();
		//}
        
    }


    /// <summary>
    /// 初始化 voxel 信息数据
    /// </summary>
    /// <param name="voxData"></param>
    /// <param name="frame"></param>
    private void InitVoxels(VoxData voxData, int frame)
    {
        this._voxData = voxData;

        SizeX = voxData.SizeX[frame];
        SizeY = voxData.SizeY[frame];
        SizeZ = voxData.SizeZ[frame];

        Voxels = new Voxel[SizeX, SizeY, SizeZ];
        for (int i = 0; i < SizeX; i++)
        {
            for (int j = 0; j < SizeY; j++)
            {
                for (int k = 0; k < SizeZ; k++)
                {
                    Voxels[i, j, k].Init();
                    Voxels[i, j, k].ColorIndex = voxData.Voxels[frame][i, j, k];
                }
            }
        }
    }

    /// <summary>
    /// 设置是否显示数据
    /// </summary>
    private void FixVisible()
    {
        for (int i = 0; i < SizeX; i++)
        {
            for (int j = 0; j < SizeY; j++)
            {
                for (int k = 0; k < SizeZ; k++)
                {
                    if (Voxels[i, j, k].IsEmpty)
                    {
                        Voxels[i, j, k].IsVisible = true;
                        continue;
                    }

                    Voxels[i, j, k].VisibleLeft = i > 0 ? Voxels[i - 1, j, k].IsEmpty : true;
                    Voxels[i, j, k].VisibleRight = i < SizeX - 1 ? Voxels[i + 1, j, k].IsEmpty : true;
                    Voxels[i, j, k].VisibleFront = j > 0 ? Voxels[i, j - 1, k].IsEmpty : true;
                    Voxels[i, j, k].VisibleBack = j < SizeY - 1 ? Voxels[i, j + 1, k].IsEmpty : true;
                    Voxels[i, j, k].VisibleDown = k > 0 ? Voxels[i, j, k - 1].IsEmpty : true;
                    Voxels[i, j, k].VisibleUp = k < SizeZ - 1 ? Voxels[i, j, k + 1].IsEmpty : true;


                    if (Voxels[i, j, k].IsVisible)
                    {
                        //如果此位置需要有数据显示，那么处理颜色信息，否则不进行处理

                        SetVoxelColorInfo(i, j, k, _voxData.Palatte[Voxels[i, j, k].ColorIndex - 1]);
                    }
                }
            }
        }

    }
		
    #endregion

	#region voxel 信息处理

	/// <summary>
	/// 获取 当前修改之后的 voxel 信息数据
	/// </summary>
	/// <returns>The voxel data.</returns>
	public VoxData GetVoxelData()
	{
		return _voxData;
	}

	#endregion

    #region 颜色信息处理

    /// <summary>
    /// voxel 颜色信息数据 字典
    /// </summary>
    public Dictionary<UnityEngine.Color, PosColorInfo> VoxelColorInfoDic;

    /// <summary>
    /// voxel 颜色信息数据 数组
    /// </summary>
    public PosColorInfo[] VoxelColorInfoArr;

    /// <summary>
    /// 灰色颜色列表
    /// </summary>
    public List<UnityEngine.Color> GreyColorList;

    /// <summary>
    /// 设置颜色信息
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="color"></param>
    private void SetVoxelColorInfo(int x, int y, int z, UnityEngine.Color color)
    {
        if (VoxelColorInfoDic == null)
            VoxelColorInfoDic = new Dictionary<UnityEngine.Color, PosColorInfo>();

        var id = x + SizeX * z + y * (SizeX * SizeZ);

        if (VoxelColorInfoDic.ContainsKey(color))
        {
            VoxelColorInfoDic[color].AddColorIndexId(id);
        }
        else
        {
            var grey = Vector3.Dot(new Vector3(color.r, color.g, color.b), ConstantConfig.GetGameConfigVector3(GameConfigKey.grey_color_vector));
            var greyColor = new UnityEngine.Color(grey, grey, grey);

            PosColorInfo info = new PosColorInfo(color, greyColor, id);
            VoxelColorInfoDic[color] = info;
        }
    }

    /// <summary>
    /// 颜色信息处理完成之后，进行排序
    /// </summary>
    private void VoxelColorInfoSort()
    {
        var array = VoxelColorInfoDic.Values.ToArray();

        //快速排序(从小到大顺序)
        GameUtils.QuickSort(ref array, 0, array.Length - 1);
        //翻转数组顺序
        Array.Reverse(array);
        //存储颜色值数组
        VoxelColorInfoArr = array;
    }

    /// <summary>
    /// 获取Voxel已经完成的信息数据
    /// </summary>
    private void GetVoxelCompleteInfo()
    {
        int[] colorCompleteAreaIdArr = null;
        UnityEngine.Color[] originalColorTypeArr = null;
        //获取已经完成的信息
        var completeColorArr =
            FileManager.instance.ReadFileInfo(TypeId, Id, out colorCompleteAreaIdArr, out originalColorTypeArr);
        if (completeColorArr != null && completeColorArr.Length > 0)
        {
            VoxelColorCompleteOrder = new List<int>(completeColorArr);
            VoxelColorCompleteAreaIdList = new List<int>(colorCompleteAreaIdArr);

            ////如果已经填充完成，那么才需要读取此数据
            //if (IsVoxelColoringComplete)
            //{
            //    var count = originalColorTypeArr.Length;
            //    VoxelColorInfoArr = new PosColorInfo[count];
            //    for (int i = 0; i < count; i++)
            //    {
            //        var info = new PosColorInfo(originalColorTypeArr[i], UnityEngine.Color.white, 0);
            //        VoxelColorInfoArr[i] = info;
            //    }
            //}
            //else
            //{
            //    //从已经排序的队列中，把已经填充完毕的数据移除
            //    //RemoveFillColorInfo();
            //}

            //通知UI显示已经完成的数据
            //Frame.DispatchEvent(ME.New<ME_Coloring_Done>(p => { p.ColoringDoneIdArr = colorCompleteAreaIdArr; }));
        }
    }

    /// <summary>
    /// 检测一个 indexId 位置所属的 颜色 area id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="indexId"></param>
    /// <returns></returns>
    public bool IsInColorArea(int id, int indexId)
    {
        if (id < 1 || id > VoxelColorInfoArr.Length)
            return false;

        var info = VoxelColorInfoArr[id - 1];
        if (info != null && info.Contains(indexId))
            return true;

        return false;
    }

    /// <summary>
    /// 移除已经填充的颜色数据
    /// </summary>
    /// <param name="areaId"></param>
    /// <param name="indexId"></param>
    public void RemoveColorInfo(int areaId, int indexId)
    {
        if (areaId < 1 || areaId > VoxelColorInfoArr.Length)
            return;

        var info = VoxelColorInfoArr[areaId - 1];
        if (info != null)
        {
            info.Remove(indexId);
            if (info.PosColorDic.Count == 0)
            {
                //是否当前所有的颜色都已经填充完毕，如果是那么标志为已经完成

                if (VoxelColorCompleteAreaIdList == null)
                    VoxelColorCompleteAreaIdList = new List<int>();

                VoxelColorCompleteAreaIdList.Add(areaId - 1);

                IsVoxelColoringComplete = VoxelColorCompleteAreaIdList.Count == VoxelColorInfoArr.Length;
                //如果已经填图完成，那么存储当前已经完成的标识
                if (IsVoxelColoringComplete)
                    PlayerGameInfo.instance.AddCompleteInfo(TypeId, Id);

                //通知UI显示已经完成的数据
                Frame.DispatchEvent(ME.New<ME_Coloring_Done>(p => { p.ColoringDoneIdArr = new int[] {areaId - 1}; }));
            }
        }
    }

    /// <summary>
    /// 移除填充颜色信息
    /// </summary>
    public void RemoveFillColorInfo()
    {
        if (VoxelColorCompleteOrder != null)
        {
            var count = VoxelColorInfoArr.Length;
            for (int i = 0; i < VoxelColorCompleteOrder.Count;)
            {
                for (int j = 0; j < count; j++)
                {
                    var info = VoxelColorInfoArr[j];

                    if (info != null
                        && info.Remove(VoxelColorCompleteOrder[i]))
                        break;
                }

                i++;
            }

        }
    }

    /// <summary>
    /// 获取颜色类型顺序id
    /// </summary>
    /// <param name="indexId"></param>
    /// <returns></returns>
    public int GetColorTypeOrderId(int indexId)
    {
        if (indexId < 0)
            return 0;

        var count = VoxelColorInfoArr.Length;
        for (int i = 0; i < count; i++)
        {
            if (VoxelColorInfoArr[i] != null
                && VoxelColorInfoArr[i].Contains(indexId))
                return i;
        }

        return 0;
    }

	/// <summary>
	/// 获取原始颜色类型数组
	/// </summary>
	/// <returns></returns>
	public UnityEngine.Color[] GetOriginalColorTypeArr()
	{
		var count = VoxelColorInfoArr.Length;
		UnityEngine.Color[] arr = new UnityEngine.Color[count];
		for (int i = 0; i < count; i++)
		{
			arr[i] = VoxelColorInfoArr[i].PosColor;
		}

		return arr;
	}

	/// <summary>
	/// 获取存储颜色信息数据
	/// </summary>
	/// <param name="indexId"></param>
	/// <returns></returns>
	public PosColorInfo GetColorInfoFromIndexId(int indexId)
	{
		if (indexId < 0)
			return null;


		var count = VoxelColorInfoArr.Length;
		for (int i = 0; i < count; i++)
		{
			if (VoxelColorInfoArr[i] != null
				&& VoxelColorInfoArr[i].Contains(indexId))
				return VoxelColorInfoArr[i];
		}

		return null;
	}

	/// <summary>
    /// 设置默认选中的 颜色信息
    /// </summary>
    public void SetDefaultSelectColorInfo()
    {
        SetSelectColorInfo(1);
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
        PlayerGameInfo.instance.SelectColor = VoxelColorInfoArr[id - 1].PosColor;

    }

	/// <summary>
	/// 存储 voxel 颜色填充顺序
	/// </summary>
	/// <param name="list">List.</param>
	public void SaveVoxelColorCompleteOrder(List<int> list)
	{
		VoxelColorCompleteOrder = list;
	}

    #endregion

	#region 数据清理

	/// <summary>
	/// 数据清理
	/// </summary>
	public void Clear()
	{
		
	}

	#endregion
}
