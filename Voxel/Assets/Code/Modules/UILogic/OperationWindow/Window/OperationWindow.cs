using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Color.Number.Animation;
using Color.Number.Camera;
using Color.Number.Event;
using Color.Number.GameInfo;
using Color.Number.Grid;
using Color.Number.Scene;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ZFrame;
using ZLib;


/// <summary>
/// 操作界面UI处理
/// </summary>
public class OperationWindow : BaseUIWindow
{
    #region 数据初始化

    /// <summary>
    /// 当前窗口界面的引用
    /// </summary>
    private OperationWindowData _data;

    public OperationWindow(Module module)
        : base(module)
    {

    }


    public override void Init(BaseUIWindowData data)
    {
        _data = data as OperationWindowData;
        base.Init(data);

        //初始化按钮
        InitBtn();

        //初始化提示信息
        InitTipsInfo(false);

        //初始化序列帧动画播放
        InitSpriteFrameInfo();

        //初始化保存和分享界面
        InitSaveAndShareInfoUI(false);

        //初始化颜色列表
        InitColorScrollView();
    }

    

    #endregion


    #region UI 添加监听

    protected override void EventListener()
    {
        if (_data != null)
        {
            UIEventListener.GetPointer(_data.backBtn).onClick = OnClickBackBtn;
            UIEventListener.GetPointer(_data.resetBtn).onClick = OnClickResetBtn;
            UIEventListener.GetPointer(_data.okBtn).onClick = OnClickOkBtn;


            UIEventListener.GetPointer(_data.saveInfo.image).onClick = OnClickSaveImageBtn;
            UIEventListener.GetPointer(_data.saveInfo.video).onClick = OnClickSaveVideoBtn;
        }
    }

    /// <summary>
    /// 点击返回操作按钮
    /// </summary>
    /// <param name="go"></param>
    /// <param name="eventData"></param>
    /// <param name="parameter"></param>
    private void OnClickBackBtn(GameObject go, PointerEventData eventData, object parameter)
    {
        //退出当前游戏场景
        if(!PlayerGameInfo.instance.IsPixelColoring)
            SceneManager.instance.ExitGameScene();
    }

    /// <summary>
    /// 点击重置按钮
    /// </summary>
    /// <param name="go"></param>
    /// <param name="eventData"></param>
    /// <param name="parameter"></param>
    private void OnClickResetBtn(GameObject go, PointerEventData eventData, object parameter)
    {
        CameraManager.GetInst().CameraResetPos();
    }

    /// <summary>
    /// 点击OK操作按钮
    /// </summary>
    /// <param name="go"></param>
    /// <param name="eventData"></param>
    /// <param name="parameter"></param>
    private void OnClickOkBtn(GameObject go, PointerEventData eventData, object parameter)
    {
        if (_data.playFrameAnimation != null)
            _data.playFrameAnimation.go.SetActive(false);

        //展示整个填充过程
        if (!PlayerGameInfo.instance.IsPixelColoring)
            SceneManager.instance.ShowColoringProcess();
    }

    /// <summary>
    /// 保存图片到相册
    /// </summary>
    /// <param name="go"></param>
    /// <param name="eventData"></param>
    /// <param name="parameter"></param>
    public void OnClickSaveImageBtn(GameObject go, PointerEventData eventData, object parameter)
    {
        SceneManager.instance.SaveCaptureScreenshot();

        InitTipsInfo(true);

        //显示1秒之后隐藏
        Tick.AddCallback<bool>(InitTipsInfo, false, 1f);

        Debug.LogError(" 保存成功 ");
    }

    /// <summary>
    /// 保存视频到相册
    /// </summary>
    /// <param name="go"></param>
    /// <param name="eventData"></param>
    /// <param name="parameter"></param>
    public void OnClickSaveVideoBtn(GameObject go, PointerEventData eventData, object parameter)
    {

        SceneManager.instance.SaveVideo();

        InitTipsInfo(true);

        //显示1秒之后隐藏
        Tick.AddCallback<bool>(InitTipsInfo, false, 1f);

        Debug.LogError(" 保存成功 ");
    }

    #endregion

    #region 按钮操作

    /// <summary>
    /// 初始化按钮信息数据
    /// </summary>
    private void InitBtn()
    {
        InitResetBtn();

        InitOkBtn();
    }

    /// <summary>
    /// 初始化重置按钮
    /// </summary>
    private void InitResetBtn()
    {
        if (_data.resetBtn != null)
            _data.resetBtn.SetActive(SceneManager.instance.IsVoxel);
    }

    /// <summary>
    /// 初始化 ok按钮
    /// </summary>
    private void InitOkBtn()
    {
        if (_data.okBtn != null)
            _data.okBtn.SetActive(SceneManager.instance.IsColoringComplete());
    }

    #endregion

    #region 保存信息数据

    /// <summary>
    /// 初始化存储和分享功能UI
    /// </summary>
    private void InitSaveAndShareInfoUI(bool active)
    {
        if (_data != null)
        {
            _data.saveInfo.panel.SetActive(active);
        }
    }

    #endregion

    #region 提示信息数据

    /// <summary>
    /// 初始化提示信息
    /// </summary>
    /// <param name="active"></param>
    private void InitTipsInfo(bool active)
    {
        if(_data != null)
            _data.tipsInfo.panel.SetActive(active);
    }

    #endregion

    #region 构建颜色列表

    /// <summary>
    /// color grid
    /// </summary>
    private ColorModel[] _colorGrid;

    /// <summary>
    /// 初始化颜色滚动区域UI
    /// </summary>
    /// <param name="active"></param>
    private void InitColorScrollView(bool active)
    {
        if(_data != null)
            _data.scrollContentTran.gameObject.SetActive(active);
    }

    /// <summary>
    /// 初始化颜色列表
    /// </summary>
    private void InitColorScrollView()
    {
        ////已经填充完成，那么就不显示颜色色块了？？？
        //if(GridManager.instance.GameLevelGridInfo.IsTextureColoringComplete)
        //    return;

        InitColorScrollView(true);

        var arr = SceneManager.instance.GetCurrGameLevelColorInfo();
        var count = arr.Length;
        var module = _data.ModelList[0];

        _colorGrid = new ColorModel[count];

        for (int i = 0; i < count; i++)
        {
            var go = GameObject.Instantiate(module, _data.scrollContentTran);
            var tran = go.transform;
            tran.rotation = Quaternion.identity;
            tran.localScale = Vector3.one;
            tran.name = (i + 1).ToString();

            InitColorGrid(go, i, arr[i].PosColor);

            UIEventListener.GetPointer(go, i).onClick = OnClickColorBtn;
        }

        //默认显示第一个
        ShowSelectColorGrid(0);

        int[] a = new int[_colorGrid.Length];
        for (int i = 0; i < a.Length; i++)
        {
            a[i] = i;
        }

        //TextureColoringDone(a);

        TextureColoringDone(SceneManager.instance.GetCurrGameLevelColorCompleteAreaIdList());
    }

    /// <summary>
    /// 初始化颜色 Grid
    /// </summary>
    /// <param name="go"></param>
    /// <param name="id"></param>
    /// <param name="color"></param>
    private void InitColorGrid(GameObject go, int id, UnityEngine.Color color)
    {
        if (go == null)
            return;

        var info = go.GetComponent<ColorModel>();
        if (info != null)
        {
            info.bgImage.color = color;
            info.num.text = (id + 1).ToString();
        }

        _colorGrid[id] = info;
    }

    /// <summary>
    /// 显示当前选中的 color grid
    /// </summary>
    /// <param name="id"></param>
    private void ShowSelectColorGrid(int id)
    {
        var preId = PlayerGameInfo.instance.PreSelectColorId;
        if (preId < 1
            || (preId == id + 1 && id > 0))
        {
            return;
        }

        _colorGrid[preId - 1].selectGo.SetActive(false);
        _colorGrid[id].selectGo.SetActive(true);
    }

    /// <summary>
    /// 点击了颜色按钮
    /// </summary>
    /// <param name="go"></param>
    /// <param name="eventData"></param>
    /// <param name="parameter"></param>
    private void OnClickColorBtn(GameObject go, PointerEventData eventData, object parameter)
    {
        //var indexId = int.Parse(go.name);
        //var color = (UnityEngine.Color)parameter;

        var indexId = (int)parameter;
        SceneManager.instance.SetSelectColorInfo(indexId + 1);

        ShowSelectColorGrid(indexId);

        SceneManager.instance.UpdateTileOrVoxelColor();
    }

    #endregion

    #region sprite 序列帧播放

    /// <summary>
    /// 初始化序列帧播放
    /// </summary>
    private void InitSpriteFrameInfo()
    {
        if (_data != null
            && _data.playFrameAnimation != null)
        {
            _data.playFrameAnimation.go.SetActive(false);

            var list = SceneManager.instance.GetGifAnimation();

            _data.playFrameAnimation.PlayAnimation(
                list, 
                FFmpegManager.instance.CreateOriginalDynamicVideo, 
                true, 
                false);
        }
    }

    /// <summary>
    /// 播放序列帧动画信息
    /// </summary>
    private void PlayFrameAnimation(bool isPlay)
    {
        if (_data != null
            && _data.playFrameAnimation != null)
        {
            _data.playFrameAnimation.go.SetActive(true);
            if (isPlay)
            {
                _data.playFrameAnimation.PlayAnimation(true);
            }
            else
            {
                _data.playFrameAnimation.StopAnimation(false);
            }
        }
    }

    #endregion

    #region 颜色上色处理

    //texture 上色完成的数组
    private bool[] _textureColoringDoneArr;

    /// <summary>
    /// 某一种颜色上色完成
    /// </summary>
    /// <param name="id"></param>
    private void TextureColoringDone(int[] arr)
    {

        if(arr == null || arr.Length == 0)
            return;

        //记录已经完成的id
        if (_textureColoringDoneArr == null)
            _textureColoringDoneArr = new bool[_colorGrid.Length];

        var count = arr.Length;
        for (int i = 0; i < count; i++)
        {
            var id = arr[i];
            _colorGrid[id].selectGo.SetActive(false);
            _colorGrid[id].OkGo.SetActive(true);

            _textureColoringDoneArr[id] = true;
        }

        count = _textureColoringDoneArr.Length;
        for (int i = 0; i < count; i++)
        {
            if (!_textureColoringDoneArr[i])
            {
                //某一项还未完成，那么默认选中此项，从头开始计算
                OnClickColorBtn(_colorGrid[i].gameObject, null, i);

                return;
            }
        }

        //所有的颜色都填充完成，那么显示完成按钮
        InitOkBtn();

        //隐藏操作界面
        InitColorScrollView(false);

        //显示保存和分享UI
        //InitSaveAndShareInfoUI(true);
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
            typeof(ME_Coloring_Done),               //某一种颜色上色完成
            typeof(ME_Sprite_Frame),                //Sprite帧动画播放
            typeof(ME_Show_SaveUI),                 //显示存储UI
        };
    }

    /// <summary>
    /// 接收监听的事件信息
    /// </summary>
    /// <param name="me"></param>
    protected override void ReceivedModuleEvent(ModuleEvent me)
    {
        if (me is ME_Coloring_Done)
        {
            OnColoringDone(me as ME_Coloring_Done);
        }
        else if(me is ME_Sprite_Frame)
        {
            OnSpriteFrameAnimation(me as ME_Sprite_Frame);
        }
        else if(me is ME_Show_SaveUI)
        {
            OnShowSaveUI(me as ME_Show_SaveUI);
        }
    }

    /// <summary>
    /// 处理 图片某一种颜色上册完成
    /// </summary>
    /// <param name="me"></param>
    private void OnColoringDone(ME_Coloring_Done me)
    {
        if (me != null)
            TextureColoringDone(me.ColoringDoneIdArr);
    }

    /// <summary>
    /// sprite 帧动画播放
    /// </summary>
    /// <param name="me"></param>
    private void OnSpriteFrameAnimation(ME_Sprite_Frame me)
    {
        if (me != null)
            PlayFrameAnimation(me.isPlay);
    }

    private void OnShowSaveUI(ME_Show_SaveUI me)
    {
        if (me != null)
            InitSaveAndShareInfoUI(me.isActivity);
    }

    #endregion
}
