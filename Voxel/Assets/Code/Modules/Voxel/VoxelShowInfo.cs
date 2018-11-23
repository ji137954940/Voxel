using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Color.Number.GameInfo;
using ZFrame;
using Color.Number.Event;

/// <summary>
/// 显示创建出来的 voxel信息数据
/// </summary>
[System.Serializable]
public class VoxelShowInfo
{

    /// <summary>
    /// 对象数据信息
    /// </summary>
    public GameObject Panel;

    /// <summary>
    /// 最下面 panel tran
    /// </summary>
    public Transform ButtomPanelTran;

    /// <summary>
    /// voxel root 结点
    /// </summary>
    public Transform VoxelRoot;

    /// <summary>
    /// 数字材质球模板对象
    /// </summary>
    public Material[] NumMatTemplateArr;

	/// <summary>
	/// 完成颜色填充 material 数组
	/// </summary>
	public Material[] _completeTemplateArr;

    /// <summary>
    /// 当前模板对象
    /// </summary>
    public GameObject TemplateObj;

    /// <summary>
    /// 当前显示的 voxel 信息的对象
    /// </summary>
    private VoxelInfo _voxelInfo;

	/// <summary>
	/// voxel 大小
	/// </summary>
    private int _sizeX, _sizeY, _sizeZ;

	/// <summary>
	/// 已经完成的 voxel id hase
	/// </summary>
	private HashSet<int> _orderCompleteHash;

	/// <summary>
	/// 已经完成的 voxel id List
	/// </summary>
	private List<int> _orderCompleteList;

	/// <summary>
	/// voxel 对象 renderer 组件 dic
	/// </summary>
	private Dictionary<int, Renderer> _voxelGoRenderDic;

    /// <summary>
    /// voxel 对象的 pos 位置信息
    /// </summary>
    private Dictionary<int, Vector3> _voxelGoPosDic;

	/// <summary>
	/// 是否有填充 voxel 颜色
	/// </summary>
	private bool _isFillVoxelColor = false;

    /// <summary>
    /// 初始化 voxel 显示信息
    /// </summary>
    /// <param name="voxelInfo"></param>
    public void InitVoxelShowInfo(VoxelInfo voxelInfo)
    {
        this._voxelInfo = voxelInfo;
        this._sizeX = voxelInfo.SizeX;
        this._sizeY = voxelInfo.SizeY;
        this._sizeZ = voxelInfo.SizeZ;

		if (voxelInfo.VoxelColorCompleteOrder != null)
		{
			this._orderCompleteHash = new HashSet<int> (voxelInfo.VoxelColorCompleteOrder);
			this._orderCompleteList = new List<int> (voxelInfo.VoxelColorCompleteOrder);
		} 
		else 
		{
			this._orderCompleteHash = new HashSet<int> ();
			this._orderCompleteList = new List<int> ();
		}

        //存储voxel中每一个go的位置
        _voxelGoPosDic = new Dictionary<int, Vector3>();

        //初始化旋转
        Panel.transform.rotation = Quaternion.identity;

        //初始化材质球颜色信息
        InitMaterialsColor ();

        var pos = Vector3.zero;
        var pivot = ConstantConfig.GetGameConfigVector3(GameConfigKey.voxel_pivot);

		_voxelGoRenderDic = new Dictionary<int, Renderer> ();

		//临时父对象，便于数据清理
		var go = new GameObject ("root");
		var tran = go.transform;
		tran.SetParent (VoxelRoot);
		tran.SetPositionAndRotation (Vector3.zero, Quaternion.identity);

		// 获取 bounds 大小
		var bounds = new Bounds ();

        for (int i = 0; i < _sizeX; i++)
        {
            for (int j = 0; j < _sizeY; j++)
            {
                for (int k = 0; k < _sizeZ; k++)
                {
                    if (voxelInfo.Voxels[i, j, k].IsEmpty || !voxelInfo.Voxels[i, j, k].IsVisible)
                    {
                        //此位置没有数据，或者此位置周围的6个方向都有数据，那么就不需要显示此数据信息
                        continue;
                    }

                    //居中显示(x, z)
                    pos.x = pivot.x + i - _sizeX / 2;
                    // y 轴和 z 轴位置互换
                    pos.y = pivot.z + k;
                    pos.z = pivot.y + j - _sizeY / 2;

					var id = i + _sizeX * k + j * (_sizeX * _sizeZ);

                    //存储每一个对象的位置
                    _voxelGoPosDic[id] = pos;

                    go = GameObject.Instantiate(TemplateObj, pos, Quaternion.identity, tran);
                    go.name = id.ToString();

                    var indexId = _voxelInfo.GetColorTypeOrderId(id);
					var corlorInfo = _voxelInfo.VoxelColorInfoArr[indexId];

                    Renderer renderer = go.GetComponent<Renderer>();

                    if (voxelInfo.IsVoxelColoringComplete)
                    {
                        renderer.material = _completeTemplateArr[indexId];
                    }
                    else
                    {
                        var haveColor = IsHaveComplete(id);

                        if (haveColor)
                            renderer.material = _completeTemplateArr[indexId];
                        else
                            renderer.material = NumMatTemplateArr[indexId + 1];
                    }

					//存储 renderer 对象
					_voxelGoRenderDic[id] = renderer;

					bounds.Encapsulate (renderer.bounds);
                }
            }
        }

		//默认刷新显示数据
		ChangeShowColorAlphaAndTexture (1);

		//显示最底下的panel的大小
		ButtomPanelTran.localScale = bounds.size / 8;

        //移除已经填充完毕的数据
        voxelInfo.RemoveFillColorInfo();
    }

	/// <summary>
	/// 初始化材质球的颜色信息
	/// </summary>
	private void InitMaterialsColor()
	{
		if(_voxelInfo == null)
		{
			Debug.LogError (" 初始化材质球信息失败， _voxelInfo 数据为null ");
			return;
		}

		var arr = _voxelInfo.VoxelColorInfoArr;
		var count = arr.Length;

		_completeTemplateArr = new Material[count];

		for (int i = 0; i < count; i++)
		{
			if (arr [i] != null) 
			{
				NumMatTemplateArr [i + 1].color = arr [i].PosGreyColor;

				var mat = new Material (NumMatTemplateArr[0]);
				mat.color = arr [i].PosColor;

				_completeTemplateArr [i] = mat;
			}
		}
			
	}

	/// <summary>
	/// 当前 整体的alpha 数值
	/// </summary>
	private float _currAlpha = 0;

	/// <summary>
	/// 改变显示的颜色和图片信息
	/// </summary>
	/// <param name="f">F.</param>
	public void ChangeShowColorAlphaAndTexture(float f)
	{
		if (_voxelInfo == null) 
		{
			Debug.LogError (" 改变材质球信息失败， _voxelInfo 数据为null ");
			return;
		}

		var count = _voxelInfo.VoxelColorInfoArr.Length;

        var defaultColor = ConstantConfig.GetGameConfigColor(GameConfigKey.default_voxel_prompt_color);
        var defaultColor1 = ConstantConfig.GetGameConfigColor(GameConfigKey.default_voxel_prompt_color1);

        for (int i = 0; i < count; i++) 
		{
			if (NumMatTemplateArr [i + 1] != null)
			{
				_currAlpha = (1 - f) * 2;

                //if (PlayerGameInfo.instance.SelectColorId == (i + 1) && _currAlpha >= 0.1f)
                //            {
                //	NumMatTemplateArr [i + 1].color = ConstantConfig.DEFAULT_VOXEL_PROMPT_COLOR;
                //                NumMatTemplateArr[i + 1].SetFloat("_NumAlphaCutoff", ConstantConfig.DEFAULT_VOXEL_NUM_ALPHA_CUTOFF);
                //            }
                //else
                //            {
                //	NumMatTemplateArr [i + 1].color = UnityEngine.Color.Lerp(_voxelInfo.VoxelColorInfoArr [i].PosGreyColor, UnityEngine.Color.white, _currAlpha);
                //                NumMatTemplateArr[i + 1].SetFloat("_NumAlphaCutoff", _currAlpha);
                //            }
                //NumMatTemplateArr [i + 1].SetFloat ("_Cutoff", _currAlpha >= 0.3f ? 0.5f : 0);
                //            //NumMatTemplateArr[i + 1].SetFloat("_NumAlphaCutoff", _currAlpha);

                if (PlayerGameInfo.instance.SelectColorId == (i + 1))
                {
                    if(_currAlpha >= 0.1f)
                    {
                        //NumMatTemplateArr[i + 1].color = ConstantConfig.DEFAULT_VOXEL_PROMPT_COLOR;

                        NumMatTemplateArr[i + 1].color = UnityEngine.Color.Lerp(defaultColor1, defaultColor, _currAlpha - 0.1f);

                        //NumMatTemplateArr[i + 1].SetFloat("_NumAlphaCutoff", ConstantConfig.DEFAULT_VOXEL_NUM_ALPHA_CUTOFF);
                    }
                    else
                    {
                        NumMatTemplateArr[i + 1].color = defaultColor1;
                    }
                }
                else
                {
                    NumMatTemplateArr[i + 1].color = UnityEngine.Color.Lerp(_voxelInfo.VoxelColorInfoArr[i].PosGreyColor, UnityEngine.Color.white, _currAlpha);
                    
                }

                //NumMatTemplateArr[i + 1].SetFloat("_Cutoff", _currAlpha >= 0.3f ? 0.5f : 0);
                NumMatTemplateArr[i + 1].SetFloat("_Cutoff", _currAlpha >= 0.5f ? 0.5f : _currAlpha);
                NumMatTemplateArr[i + 1].SetFloat("_NumAlphaCutoff", _currAlpha);


                //NumMatTemplateArr[i + 1].SetFloat("_NumAlphaCutoff", _currAlpha);
            }
        }

	}

	/// <summary>
	/// 设置刷新选中的 颜色区域信息
	/// </summary>
	public void SetSelectVoxelAreaColor()
	{
		if (_voxelInfo == null) 
		{
			Debug.LogError (" 改变材质球信息失败， _voxelInfo 数据为null ");
			return;
		}

		var preId = PlayerGameInfo.instance.PreSelectColorId;
		var currId = PlayerGameInfo.instance.SelectColorId;

        var defaultColor = ConstantConfig.GetGameConfigColor(GameConfigKey.default_voxel_prompt_color);
        var defaultColor1 = ConstantConfig.GetGameConfigColor(GameConfigKey.default_voxel_prompt_color1);

        if (preId != currId)
		{
			//恢复之前选中的 area color
			NumMatTemplateArr [preId].color = UnityEngine.Color.Lerp(_voxelInfo.VoxelColorInfoArr [preId - 1].PosGreyColor, UnityEngine.Color.white, _currAlpha);
            NumMatTemplateArr[preId].SetFloat("_NumAlphaCutoff", _currAlpha);
        }

        if (_currAlpha >= 0.1f)
        {
            //NumMatTemplateArr[i + 1].color = ConstantConfig.DEFAULT_VOXEL_PROMPT_COLOR;

            NumMatTemplateArr[currId].color = UnityEngine.Color.Lerp(defaultColor1, defaultColor, _currAlpha - 0.1f);

            //NumMatTemplateArr[i + 1].SetFloat("_NumAlphaCutoff", ConstantConfig.DEFAULT_VOXEL_NUM_ALPHA_CUTOFF);
        }
        else
        {
            NumMatTemplateArr[currId].color = defaultColor1;
        }
    }

	/// <summary>
	/// 是否已经填充完成
	/// </summary>
	/// <returns><c>true</c> if this instance is have complete the specified id; otherwise, <c>false</c>.</returns>
	/// <param name="id">Identifier.</param>
	private bool IsHaveComplete(int id)
	{
		if (id < 0)
			return false;

		if (_orderCompleteHash == null
		   || _orderCompleteHash.Count == 0)
			return false;

		return _orderCompleteHash.Contains (id);

	}

	/// <summary>
	/// 添加存储 颜色填充记录数据
	/// </summary>
	/// <param name="id">Identifier.</param>
	private void AddVoxelColorComplete(int id)
	{
		if (id < 0)
			return;

		if (_orderCompleteHash == null)
			_orderCompleteHash = new HashSet<int> ();

		if (_orderCompleteList == null)
			_orderCompleteList = new List<int> ();

		_orderCompleteHash.Add (id);
		_orderCompleteList.Add (id);
	}

	/// <summary>
	/// voxel 颜色填充
	/// </summary>
	/// <param name="id">Identifier.</param>
	public void VoxelColorFill(int id)
	{

		if (id < 0)
			return;

		//颜色已经填充完成，那么就不处理
		if (IsHaveComplete (id))
			return;

		//获取当前id 所在的 area id
		var indexId = _voxelInfo.GetColorTypeOrderId(id);

		if(PlayerGameInfo.instance.SelectColorId == (indexId + 1))
		{
			_isFillVoxelColor = true;

			//记录颜色填充
			AddVoxelColorComplete(id);
			//颜色填充
			VoxelColorShow (indexId, id);

			_voxelInfo.RemoveColorInfo (indexId + 1, id);
		}
	}

	/// <summary>
	/// voxel renderer 材质球切换
	/// </summary>
	/// <param name="areaId">Area identifier.</param>
	/// <param name="indexId">Index identifier.</param>
	private void VoxelColorShow(int areaId, int indexId)
	{
		Renderer renderer = null;
		if(_voxelGoRenderDic.TryGetValue(indexId, out renderer))
		{
			renderer.material = _completeTemplateArr [areaId];
		}
	}

	/// <summary>
	/// 存储 voxel 颜色信息
	/// </summary>
	public bool SaveVoxelColorInfo()
	{
		if (_voxelInfo != null
		   && _isFillVoxelColor) 
		{
			_voxelInfo.SaveVoxelColorCompleteOrder (_orderCompleteList);

			return true;
		}

		return false;
	}

    /// <summary>
    /// 显示voxel填充颜色过程
    /// </summary>
    public IEnumerator ShowVoxelColoringProcess()
    {
        if (_voxelInfo != null
            && _voxelInfo.IsVoxelColoringComplete)
        {
            PlayerGameInfo.instance.IsPixelColoring = true;

            //初始化旋转
            Panel.transform.rotation = Quaternion.identity;

            var count = _orderCompleteList.Count;
            var indexId = 0;
            var increment = 0;

            //开始录制循环视频
            FFmpegManager.instance.StartFillColorVideoREC(_voxelInfo);

            //移除所有子对象
            GameObject.Destroy(VoxelRoot.GetChild(0).gameObject);

            //临时父对象，便于数据清理
            var go = new GameObject("root");
            var tran = go.transform;
            tran.SetParent(VoxelRoot);
            tran.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

            var pos = Vector3.zero;

            for (int i = 0; i < count; i++)
            {
                indexId = _orderCompleteList[i];

                pos = _voxelGoPosDic[indexId];

                //go = GameObject.Instantiate(TemplateObj, pos, Quaternion.identity, tran);
                go = GameObject.Instantiate(TemplateObj);
                go.name = indexId.ToString();
                var t = go.transform;
                t.SetParent(tran);
                t.localPosition = pos;
                t.localRotation = Quaternion.identity;

                indexId = GetCompleteTemplateId(pos);

                //indexId = _voxelInfo.GetColorTypeOrderId(indexId);
                var renderer = go.GetComponent<Renderer>();

                renderer.material = _completeTemplateArr[indexId];

                increment++;

                if (increment == 5)
                {
                    increment = 0;
                    Panel.transform.Rotate(Vector3.up, ConstantConfig.GetGameConfigFloat(GameConfigKey.voxel_rotation_speed));
                    yield return null;
                }
            }

            while(true)
            {
                Panel.transform.Rotate(Vector3.up, ConstantConfig.GetGameConfigFloat(GameConfigKey.voxel_rotation_speed));

                var rot = Panel.transform.rotation;
                if (Mathf.Abs(rot.eulerAngles.y) <= ConstantConfig.GetGameConfigFloat(GameConfigKey.voxel_rotation_speed))
                    rot = Quaternion.identity;

                if (rot != Quaternion.identity)
                    yield return null;
                else
                    break;
            }   

            //等待 1 秒
            yield return new WaitForSeconds(1.0f);

            PlayerGameInfo.instance.IsPixelColoring = false;

            //停止录制视频
            FFmpegManager.instance.StopFillColorVideoREC(true);

            //显示保存界面UI
            Frame.DispatchEvent(ME.New<ME_Show_SaveUI>(p => { p.isActivity = true; }));
        }
    }

    private int GetCompleteTemplateId(Vector3 pos)
    {
        ////居中显示(x, z)
        //pos.x = pivot.x + i - _sizeX / 2;
        //// y 轴和 z 轴位置互换
        //pos.y = pivot.z + k;
        //pos.z = pivot.y + j - _sizeY / 2;


        var pivot = ConstantConfig.GetGameConfigVector3(GameConfigKey.voxel_pivot);
        //var info = _voxelInfo.Voxels[(int)(pos.x - pivot.x + _sizeX / 2), (int)(pos.z - pivot.z), (int)(pos.y - pivot.y + _sizeY / 2)];

        var info = _voxelInfo.Voxels[(int)(pos.x - pivot.x + _sizeX / 2), (int)(pos.z - pivot.y + _sizeY / 2), (int)(pos.y - pivot.z)];


        var color = _voxelInfo.GetVoxelData().Palatte[info.ColorIndex - 1];

        var count = _completeTemplateArr.Length;
        for (int i = 0; i < count; i++)
        {
            if (_completeTemplateArr[i] != null
                && _completeTemplateArr[i].color == color)
                return i;
        }

        return 0;
    }

    /// <summary>
    /// 数据清理
    /// </summary>
    public void Clear()
    {
		_completeTemplateArr = null;
		_isFillVoxelColor = false;

		if (_voxelInfo != null) 
		{
			_voxelInfo.Clear ();
		}

		//移除所有子对象
        if(VoxelRoot.childCount > 0)
		    GameObject.Destroy (VoxelRoot.GetChild(0).gameObject);

		_orderCompleteHash.Clear ();
		_orderCompleteList.Clear ();
		_voxelGoRenderDic.Clear ();
        _voxelGoPosDic.Clear();

    }
}

