using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Color.Number.File;
using Color.Number.Scene;
using Color.Number.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using ZFrame;


/// <summary>
/// 拍照模式 界面
/// </summary>
public class CameraPhotoWindow : BaseUIWindow
{

    #region 数据初始化

    /// <summary>
    /// 当前窗口界面的引用
    /// </summary>
    private CameraPhotoData _data;

    /// <summary>
    /// photo材质
    /// </summary>
    private Material _photoMat;

    /// <summary>
    /// 最小的马赛克大小
    /// </summary>
    private float _minMosaicSize = 0.01f;

    /// <summary>
    /// 最大的马赛克大小
    /// </summary>
    private float _maxMosaicSize = 0.1f;

    /// <summary>
    /// 当前的马赛克大小
    /// </summary>
    private float _currMosaicSize = 0.01f;

    /// <summary>
    /// 默认 slider 数值
    /// </summary>
    private float _defaultSliderValue = 0.4f;

    /// <summary>
    /// 马赛克大小范围
    /// </summary>
    private float _mosaicSizeRange;

    /// <summary>
    /// shader 参数数据
    /// </summary>
    private int _mosaicShaderPropertyToID;

    public CameraPhotoWindow(Module module)
        : base(module)
    {

    }


    public override void Init(BaseUIWindowData data)
    {
        _data = data as CameraPhotoData;
        base.Init(data);

        ShowCameraTextureUI(false);

        InitSlider();

        CoroutineManager.instance.StartCoroutine(CameraPhoto());
    }

    /// <summary>
    /// 初始显示相机信息
    /// </summary>
    /// <param name="isDone"></param>
    private void ShowCameraTextureUI(bool isDone)
    {
        if (_data != null)
        {
            _data.shooting.panel.SetActive(!isDone);

            _data.filming.panel.SetActive(isDone);
        }
    }

    /// <summary>
    /// 初始化 slider
    /// </summary>
    private void InitSlider()
    {
        if (_data != null)
        {
            _mosaicSizeRange = _maxMosaicSize - _minMosaicSize;

            _photoMat = _data.greyColorPhoto.material;

            _mosaicShaderPropertyToID = Shader.PropertyToID("_MosaicSize");

            _data.slider.value = _defaultSliderValue;
        }
    }

    #endregion

    #region 事件监听

    protected override void EventListener()
    {
        if (_data != null)
        {
            UIEventListener.GetPointer(_data.back).onClick = OnClickBack;

            UIEventListener.GetPointer(_data.shooting.cameraPhoto).onClick = OnClickCameraPhoto;
            UIEventListener.GetPointer(_data.shooting.flipCamera).onClick = OnClickCameraFlip;

            UIEventListener.GetPointer(_data.filming.remake).onClick = OnClickCameraRemake;
            UIEventListener.GetPointer(_data.filming.use).onClick = OnClickUseCameraPhoto;

            _data.slider.onValueChanged.AddListener(OnSliderValueChange);
        }
    }

    /// <summary>
    /// 返回界面
    /// </summary>
    /// <param name="go"></param>
    /// <param name="eventData"></param>
    /// <param name="parameter"></param>
    private void OnClickBack(GameObject go, PointerEventData eventData, object parameter)
    {
        Clear();
        SceneManager.instance.CameraPhoto(true);
    }

    /// <summary>
    /// 点击拍照功能
    /// </summary>
    /// <param name="go"></param>
    /// <param name="eventData"></param>
    /// <param name="parameter"></param>
    private void OnClickCameraPhoto(GameObject go, PointerEventData eventData, object parameter)
    {
        if(_webCamTexture != null)
            _webCamTexture.Pause();

        ShowCameraTextureUI(true);
    }

    /// <summary>
    /// 点击镜头翻转
    /// </summary>
    /// <param name="go"></param>
    /// <param name="eventData"></param>
    /// <param name="parameter"></param>
    private void OnClickCameraFlip(GameObject go, PointerEventData eventData, object parameter)
    {
        CameraTextureShow(!_isShowFrontCamTexture);
    }

    /// <summary>
    /// 点击重新拍摄
    /// </summary>
    /// <param name="go"></param>
    /// <param name="eventData"></param>
    /// <param name="parameter"></param>
    private void OnClickCameraRemake(GameObject go, PointerEventData eventData, object parameter)
    {

        ShowCameraTextureUI(false);
        if(_webCamTexture != null && !_webCamTexture.isPlaying)
            _webCamTexture.Play();
    }

    /// <summary>
    /// 点击使用相机照相图片
    /// </summary>
    /// <param name="go"></param>
    /// <param name="eventData"></param>
    /// <param name="parameter"></param>
    private void OnClickUseCameraPhoto(GameObject go, PointerEventData eventData, object parameter)
    {
        //存储图片信息
        SaveCameraTexture();
    }

    /// <summary>
    /// 进入条数值改变
    /// </summary>
    /// <param name="f"></param>
    private void OnSliderValueChange(float f)
    {
        if (_photoMat != null)
        {
            _currMosaicSize = Mathf.Clamp((1 - f) * _mosaicSizeRange, _minMosaicSize, _maxMosaicSize);

            _photoMat.SetFloat(_mosaicShaderPropertyToID, _currMosaicSize);

        }
    }

    #endregion

    #region Camera 拍照设置

    /// <summary>
    /// 当前所有的相机设备
    /// </summary>
    private WebCamDevice[] _devices = null;

    /// <summary>
    /// 获取第一个相机的名字
    /// </summary>
    private string _cameraName = null;

    /// <summary>
    /// 是否为相机前置摄像头启用
    /// </summary>
    private bool _isShowFrontCamTexture = true;

    /// <summary>
    /// 相机贴图
    /// </summary>
    private WebCamTexture _webCamTexture = null;

    /// <summary>
    /// 相机拍照
    /// </summary>
    public IEnumerator CameraPhoto()
    {
        //请求使用相机权限
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);

        //通过了使用相机的权限
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            _devices = WebCamTexture.devices;

            if (_devices == null || _devices.Length == 0)
            {
                Debug.LogError(" 当前设备没有摄像头 ");
                yield break;
            }


            //默认获取前置相机
            CameraTextureShow(true);
        }
    }

    /// <summary>
    /// 获取相机 Camera Texture 显示
    /// </summary>
    /// <param name="isFrontCam"></param>
    private void CameraTextureShow(bool isFrontCam)
    {
        _isShowFrontCamTexture = isFrontCam;
        _cameraName = _devices[0].name;
        for (int i = 0; i < _devices.Length; i++)
        {
            if (_devices[i].isFrontFacing == isFrontCam)
            {
                _cameraName = _devices[i].name;
                break;
            }
        }

        if (_webCamTexture != null && _webCamTexture.isPlaying)
            _webCamTexture.Stop();

        var size = ConstantConfig.GetGameConfigInt(GameConfigKey.camera_save_texture_size);

        _webCamTexture = new WebCamTexture(_cameraName,
                                            size,
                                            size,
                                            ConstantConfig.GetGameConfigInt(GameConfigKey.camera_save_texture_fps));

        _data.greyColorPhoto.texture = _webCamTexture;

        _webCamTexture.Play();

        _data.greyColorPhoto.transform.rotation = Quaternion.Euler(0, 0, -_webCamTexture.videoRotationAngle);

    }

    /// <summary>
    /// 存储相机图片信息
    /// </summary>
    private void SaveCameraTexture()
    {
        if (_data)
        {
            if (_webCamTexture != null)
                _webCamTexture.Pause();

            //var tex = GameUtils.GetMosaicTexture2D(_data.greyColorPhoto.texture, _currMosaicSize);


            var size = GameUtils.GetCameraPhotoTextureSize(_data.slider.value);
            Debug.Log(" size " + size + "  w " + _webCamTexture.width + "  h " + _webCamTexture.height + " angle " + _webCamTexture.videoRotationAngle);
            var tex = GameUtils.ScaleTexture(_webCamTexture.GetRotationTexture2D(false), size, size);

            Debug.LogError(tex.width + "   " + tex.height + " 123 " + _webCamTexture.width + "   " + _webCamTexture.height);

            tex = GameUtils.GetMosaicTexture2D2(tex,
                                                    _currMosaicSize,
                                                    GameUtils.GetCameraPhotoTextureMaxColorNum(_data.slider.value));

            Debug.LogError(tex.width + "  123 " + tex.height);

            SceneManager.instance.SaveCameraPhotoInfo(tex);
        }
    }

    #endregion

    #region 清理数据信息

    /// <summary>
    /// 数据清理
    /// </summary>
    private void Clear()
    {
        if (_webCamTexture != null)
        {
            _webCamTexture.Stop();
            _webCamTexture = null;
        }

        if (_devices != null)
            _devices = null;
    }

    #endregion
}

