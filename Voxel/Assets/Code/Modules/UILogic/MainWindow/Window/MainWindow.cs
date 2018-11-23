using System;
using System.Collections;
using System.Collections.Generic;
using Color.Number.Camera;
using Color.Number.Event;
using Color.Number.File;
using Color.Number.GameInfo;
using Color.Number.Scene;
using Tgame.Game.Icon;
using Tgame.Game.Table;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ZFrame;

/// <summary>
/// 主控界面
/// </summary>
public class MainWindow : BaseUIWindow
{
    #region 初始化

    /// <summary>
    /// 当前界面的UI界面引用
    /// </summary>
    private MainWindowData _data;

    public MainWindow(Module module)
        : base(module)
    {

    }


    public override void Init(BaseUIWindowData data)
    {
        _data = data as MainWindowData;

        base.Init(data);

        InitTexture();

		InitMyTexture ();
    }

    protected override void EventListener()
    {

        if (_data != null)
        {
            UIEventListener.GetPointer(_data.camera).onClick = OnClickCamera;
        }
    }

    #endregion

    #region 显示图片列表

    /// <summary>
    /// 存储当前显示的图片信息 <类型, <id, RawImage>>
    /// </summary>
    private Dictionary<int, Dictionary<int, RawImage>> _showTexturesDic;

    /// <summary>
    /// 原始图片信息 <类型, <id, Texture2D>>
    /// </summary>
    private Dictionary<int, Dictionary<int, Texture2D>> _originalTexturesDic;

    /// <summary>
    /// 初始化显示的 texture 信息
    /// </summary>
    private void InitTexture()
    {
        //初始化 texture 信息

        var list = PlayerGameInfo.instance.IconList;
        var count = list.Count;
        Table_Client_Icon iconTable = null;

        for (int i = 0; i < count; i++)
        {
            iconTable = list[i];
            if (iconTable != null)
            {
                if (iconTable.is_voxel)
                    AddVoxelTextureModel(iconTable, i);
                else
                    AddColorTextureModel(iconTable, i);
            }
        }
    }

    /// <summary>
    /// 初始化显示 my Texture 信息
    /// </summary>
    private void InitMyTexture()
    {
        var list = PlayerGameInfo.instance.GetCameraPhotoInfos();
        if(list != null)
        {
            var count = list.Count;
            CameraPhotoInfo info = null;
            for (int i = 0; i < count; i++)
            {
                info = list[i];
                if (info != null)
                {
                    AddCameraPhotoModel(info);
                }
            }
        }
    }

    /// <summary>
    /// 创建图片模板信息
    /// </summary>
    /// <param name="iconTable"></param>
    /// <param name="indexId"></param>
    private void AddColorTextureModel(Table_Client_Icon iconTable, int indexId)
    {

        if(_showTexturesDic == null)
            _showTexturesDic = new Dictionary<int, Dictionary<int, RawImage>>();

        var go = GameObject.Instantiate(_data.ModelList[0], _data.mapInfo.scrollTran2D);
        go.name = string.Format("{0}_{1}", iconTable.type_id, iconTable.id);

        //获取图片引用
        var info = go.GetComponent<TextureModel>();

        //存储显示的 RawImage 组件
        SaveShowRawImage(iconTable.type_id, iconTable.id, info);

        //加载图片
        var isComplete = PlayerGameInfo.instance.IsCompleteInfo(iconTable.type_id, iconTable.id);
        if (isComplete)
        {
            //涂色以及完成，需要显示原始图片
            SetIcon(info.IconImage,
                iconTable.type_id,
                iconTable.id,
                OnOriginalColorTextureLoadDone,
                false);
        }
        else
        {
            //涂色已经进行，需要显示当前已经涂色之后的图片
            var tex = FileManager.instance.ReadImageInfo(iconTable.type_id, iconTable.id);
            if (tex != null)
            {
                //加载原始资源数据，但是不进行赋值，只是保存
                SetIcon(info.IconImage,
                    iconTable.type_id,
                    iconTable.id,
                    OnOriginalColorTextureLoadDone,
                    false,
                    false);

                //显示已经存储的texture
                ShowTextureChange(iconTable.type_id, iconTable.id, tex);
            }
            else
            {
                //涂色未开始，需要显示灰化图片
                SetIcon(info.IconImage,
                    iconTable.type_id,
                    iconTable.id,
                    OnOriginalColorTextureLoadDone,
                    true);
            }
        }

        //此处处理显示 new，flag 等标志图标

        //添加点击事件监听
        UIEventListener.GetPointer(go, indexId).onClick = OnClickImage;
    }

    /// <summary>
    /// 添加 voxel 图片模型
    /// </summary>
    /// <param name="iconTable"></param>
    /// <param name="indexId"></param>
    private void AddVoxelTextureModel(Table_Client_Icon iconTable, int indexId)
    {
        if (_showTexturesDic == null)
            _showTexturesDic = new Dictionary<int, Dictionary<int, RawImage>>();

        var go = GameObject.Instantiate(_data.ModelList[0], _data.mapInfo.scrollTran3D);
        go.name = string.Format("{0}_{1}", iconTable.type_id, iconTable.id);

        //获取图片引用
        var info = go.GetComponent<TextureModel>();

        //存储显示的 RawImage 组件
        SaveShowRawImage(iconTable.type_id, iconTable.id, info);

        //加载图片
        //var isComplete = PlayerGameInfo.instance.IsCompleteInfo(iconTable.type_id, iconTable.id);
        //if (isComplete)
        //{
        //    //涂色以及完成，需要显示原始图片
        //    SetIcon(info.IconImage,
        //        iconTable.type_id,
        //        iconTable.id,
        //        OnOriginalColorTextureLoadDone,
        //        false);
        //}
        //else
        {
            //涂色已经进行，需要显示当前已经涂色之后的图片
            //如果涂色已经完成，那么就显示完成之后的图片
            var tex = FileManager.instance.ReadImageInfo(iconTable.type_id, iconTable.id);
            if (tex != null)
            {
                //加载原始资源数据，但是不进行赋值，只是保存
                SetIcon(info.IconImage,
                    iconTable.type_id,
                    iconTable.id,
                    OnOriginalColorTextureLoadDone,
                    false,
                    false);

                //显示已经存储的texture
                ShowTextureChange(iconTable.type_id, iconTable.id, tex);
            }
            else
            {
                //涂色未开始，需要显示灰化图片
                SetIcon(info.IconImage,
                    iconTable.type_id,
                    iconTable.id,
                    OnOriginalColorTextureLoadDone,
                    true);
            }
        }

        //此处处理显示 new，flag 等标志图标

        //添加点击事件监听
        UIEventListener.GetPointer(go, indexId).onClick = OnClickImage;
    }

	/// <summary>
	/// 添加相机拍照模型信息显示
	/// </summary>
	/// <param name="cpi">Cpi.</param>
    private void AddCameraPhotoModel(CameraPhotoInfo cpi)
    {
        if (_showTexturesDic == null)
            _showTexturesDic = new Dictionary<int, Dictionary<int, RawImage>>();

        var go = GameObject.Instantiate(_data.ModelList[0], _data.myInfo.scrollTran);
        go.name = string.Format("{0}_{1}", cpi.type_id, cpi.id);

        //获取图片引用
        var info = go.GetComponent<TextureModel>();

        //存储显示的 RawImage 组件
        SaveShowRawImage(cpi.type_id, cpi.id, info);

        //加载图片
        var isComplete = PlayerGameInfo.instance.IsCompleteInfo(cpi.type_id, cpi.id);
        if (isComplete)
        {
            //涂色以及完成，需要显示原始图片
            SetIcon(info.IconImage,
                cpi.type_id,
                cpi.id,
                cpi.path,
                OnOriginalColorTextureLoadDone,
                false);
        }
        else
        {
            //涂色已经进行，需要显示当前已经涂色之后的图片
            var tex = FileManager.instance.ReadImageInfo(cpi.type_id, cpi.id);
            if (tex != null)
            {
                //加载原始资源数据，但是不进行赋值，只是保存
                SetIcon(info.IconImage,
                    cpi.type_id,
                    cpi.id,
                    cpi.path,
                    OnOriginalColorTextureLoadDone,
                    false,
                    false);

                //显示已经存储的texture
                ShowTextureChange(cpi.type_id, cpi.id, tex);
            }
            else
            {
                //涂色未开始，需要显示灰化图片
                SetIcon(info.IconImage,
                    cpi.type_id,
                    cpi.id,
                    cpi.path,
                    OnOriginalColorTextureLoadDone,
                    true);
            }
        }

        //此处处理显示 new，flag 等标志图标

        //添加点击事件监听
		UIEventListener.GetPointer(go, cpi).onClick = OnClickCameraPhotoImage;
    }

    /// <summary>
    /// 存储显示的 RawImage 组件
    /// </summary>
    /// <param name="typeId"></param>
    /// <param name="id"></param>
    /// <param name="info"></param>
    private void SaveShowRawImage(int typeId, int id, TextureModel info)
    {
        //存储，记录 RawImage 组件
        Dictionary<int, RawImage> dic = null;
        if (_showTexturesDic.TryGetValue(typeId, out dic))
        {
            dic[id] = info.IconImage;
        }
        else
        {
            dic = new Dictionary<int, RawImage>();
            dic[id] = info.IconImage;

            _showTexturesDic[typeId] = dic;
        }
    }

    /// <summary>
    /// 显示的 texture 加载完成
    /// </summary>
    /// <param name="typeId"></param>
    /// <param name="id"></param>
    /// <param name="obj"></param>
    private void OnOriginalColorTextureLoadDone(int typeId, int id, UnityEngine.Object obj)
    {
        if (obj != null)
        {
            if(_originalTexturesDic == null)
                _originalTexturesDic = new Dictionary<int, Dictionary<int, Texture2D>>();

            Dictionary<int, Texture2D> dic = null;
            if (_originalTexturesDic.TryGetValue(typeId, out dic))
            {
                dic[id] = obj as Texture2D;
            }
            else
            {
                dic = new Dictionary<int, Texture2D>();
                dic[id] = obj as Texture2D;

                _originalTexturesDic[typeId] = dic;
            }
        }
    }

    /// <summary>
    /// 更改显示的 Texture 
    /// </summary>
    /// <param name="typeId"></param>
    /// <param name="id"></param>
    /// <param name="tex"></param>
    private void ShowTextureChange(int typeId, int id, Texture2D tex)
    {
        Dictionary<int, RawImage> dic = null;
        if (_showTexturesDic.TryGetValue(typeId, out dic))
        {
            RawImage image = null;
            if (dic.TryGetValue(id, out image))
            {
                image.texture = tex;
                if (image.material != null)
                    image.material = null;
            }
        }
    }

    #endregion

    #region UI 事件监听

    /// <summary>
    /// 点击图片监听事件
    /// </summary>
    /// <param name="go"></param>
    /// <param name="eventData"></param>
    /// <param name="parameter"></param>
    private void OnClickImage(GameObject go, PointerEventData eventData, object parameter)
    {
        //点击了图片
        Debug.Log("点击了图片 " + parameter);

        //var arr = go.name.Split('_');
        //var typeId = int.Parse(arr[0]);
        //var textureId = int.Parse(arr[1]);

        ////创建游戏关卡
        //SceneManager.instance.CreateGameScene(typeId, textureId, (parameter as Texture2D));

        var indexId = (int) parameter;

        var iconTable = PlayerGameInfo.instance.IconList[indexId];
        var texture = _originalTexturesDic[iconTable.type_id][iconTable.id];

        //创建游戏关卡
        SceneManager.instance.CreateGameScene(iconTable, null, texture);
    }


	/// <summary>
	/// 点击图片监听事件
	/// </summary>
	/// <param name="go">Go.</param>
	/// <param name="eventData">Event data.</param>
	/// <param name="parameter">Parameter.</param>
	private void OnClickCameraPhotoImage(GameObject go, PointerEventData eventData, object parameter)
	{
		//点击了图片
		Debug.Log("点击了图片 " + parameter);

		//var arr = go.name.Split('_');
		//var typeId = int.Parse(arr[0]);
		//var textureId = int.Parse(arr[1]);

		////创建游戏关卡
		//SceneManager.instance.CreateGameScene(typeId, textureId, (parameter as Texture2D));


		var cpi = (CameraPhotoInfo)parameter;
		var texture = _originalTexturesDic[cpi.type_id][cpi.id];


		//创建游戏关卡
		SceneManager.instance.CreateGameScene(null, cpi, texture);
	}


    /// <summary>
    /// 点击相机操作
    /// </summary>
    /// <param name="go"></param>
    /// <param name="eventData"></param>
    /// <param name="parameter"></param>
    private void OnClickCamera(GameObject go, PointerEventData eventData, object parameter)
    {
        SceneManager.instance.CameraPhoto(false);
    }

    #endregion

    #region 事件监听处理

    /// <summary>
    /// 注册事件监听
    /// </summary>
    /// <returns></returns>
    protected override List<Type> ListenModuleEvents()
    {
        return new List<Type>()
        {
            typeof(ME_Texture_Change),               	//texture 图片更改
			typeof(ME_Camera_Photo_Change),				//拍照图片增加或删除
        };
    }

    /// <summary>
    /// 接收监听的事件信息
    /// </summary>
    /// <param name="me"></param>
    protected override void ReceivedModuleEvent(ModuleEvent me)
    {
        if (me is ME_Texture_Change)
        {
            OnTextureChange(me as ME_Texture_Change);
        }
		else if(me is ME_Camera_Photo_Change)
		{
			OnCameraPhotoChange (me as ME_Camera_Photo_Change);
		}
    }

    /// <summary>
    /// 处理 Texture 图片切换问题
    /// </summary>
    /// <param name="me"></param>
    private void OnTextureChange(ME_Texture_Change me)
    {
        if (me != null)
            ShowTextureChange(me.TextureTypeId, me.TextureId, me.Texture);
    }

	/// <summary>
	/// 相机拍照图片数据改变
	/// </summary>
	/// <param name="me">Me.</param>
	private void OnCameraPhotoChange(ME_Camera_Photo_Change me)
	{

		if (me != null)
		{
			if(me.IsAdd)
				AddCameraPhotoModel (me.cpi);
		}

	}

    #endregion
}
