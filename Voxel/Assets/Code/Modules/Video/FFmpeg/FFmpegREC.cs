using Color.Number.Camera;
using Color.Number.Utils;
using FFmpeg;
using System;
using System.IO;
using System.Text;
using UnityEngine;

public class FFmpegREC : MonoBehaviour, IFFmpegHandler
{

    /// <summary>
    /// 录屏相机
    /// </summary>
    public Camera screenshotCam;

    /// <summary>
    /// 截屏相机使用的 RT
    /// </summary>
    private RenderTexture _rt;

    //References
    Action<string> onStart, onProgress, onSuccess, onFailure, onFinish;



    //Paths
    const string FILE_FORMAT = "{0}_Frame.jpg";
    const string SOUND_NAME = "RecordedAudio.wav";
    const string VIDEO_NAME = "ScreenCapture.mp4";
    string cashDir, imgFilePathFormat, firstImgFilePath, soundPath, outputVideoPath;

    //Variables
    int framesCount;
    float startTime, frameInterval, frameTimer, totalTime, actualFPS;

    /// <summary>
    /// 存储截图数据信息
    /// </summary>
    public Texture2D _frameBuffer;

    /// <summary>
    /// 截图 rect 大小
    /// </summary>
    private Rect _camRect;

    /// <summary>
    /// 是否正在录屏
    /// </summary>
    public bool isREC { get; private set; }
    /// <summary>
    /// 是否正在创建视频文件
    /// </summary>
    public bool isProducing { get; private set; }

    #region 初始化

    void Start()
    {
        //注册监听
        FFmpegParser.Handler = this;

        //图片文件格式
        imgFilePathFormat = Path.Combine(ConstantConfig.FFMPEG_REC_CASH_DIR, ConstantConfig.GetGameConfigString(GameConfigKey.ffmpeg_file_format));
        //第一张图片的路径位置
        firstImgFilePath = String.Format(imgFilePathFormat, "%0d");
    }

    /// <summary>
    /// 添加事件监听
    /// </summary>
    /// <param name="onProcess"></param>
    /// <param name="onFinish"></param>
    public void AddAction(Action<string> onStart,
                            Action<string> onProgress,
                            Action<string> onSuccess,
                            Action<string> onFailure,
                            Action<string> onFinish)
    {
        this.onStart = onStart;
        this.onProgress = onProgress;
        this.onSuccess = onSuccess;
        this.onFailure = onFailure;
        this.onFinish = onFinish;
    }

    #endregion


    #region 录屏操作

    

    /// <summary>
    /// 开始录屏操作
    /// </summary>
    public void StartREC(string outputVideoPath)
    {
        if (!isREC && !isProducing)
        {

            this.outputVideoPath = outputVideoPath;

            //清理缓存数据
            Clear();

            //创建缓存目录
            Directory.CreateDirectory(ConstantConfig.FFMPEG_REC_CASH_DIR);

            var rectW = ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_rect_w);
            var rectH = (int)(((float)rectW / Screen.width) * Screen.height);
            _rt = new RenderTexture(rectW, rectH, 0);

            screenshotCam.transform.position = Camera.main.transform.position;
            screenshotCam.transform.rotation = Camera.main.transform.rotation;

            screenshotCam.gameObject.SetActive(true);
            screenshotCam.targetTexture = _rt;
            screenshotCam.Render();

            _frameBuffer = new Texture2D(ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_tex_w), ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_tex_h), TextureFormat.RGB24, false);
            _frameBuffer.filterMode = FilterMode.Point;

#if UNITY_ANDROID || UNITY_IOS

            _camRect = new Rect((int)(rectW - ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_tex_w)) / 2 - (int)(rectW * ConstantConfig.GetGameConfigFloat(GameConfigKey.capture_screen_shot_offset_w)),
                (int)((rectH - ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_tex_h)) / 2 - (int)(rectH * GameUtils.GetScreenshotOffsetH())),
                ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_tex_w),
                ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_tex_h));
#else
            _camRect = new Rect((int)(rectW - ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_tex_w)) / 2 + (int)(rectW * ConstantConfig.GetGameConfigFloat(GameConfigKey.capture_screen_shot_offset_w)),
                (int)((rectH - ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_tex_h)) / 2 + (int)(rectH * GameUtils.GetScreenshotOffsetH())),
                ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_tex_w),
                ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_tex_h));
#endif
            //计时初始化
            startTime = Time.time;
            framesCount = 0;
            frameInterval = 1.0f / ConstantConfig.GetGameConfigInt(GameConfigKey.ffmpeg_fps);
            frameTimer = frameInterval;

            isREC = true;
        }
    }

    public void StopREC()
    {
        isREC = false;
        isProducing = true;

        screenshotCam.targetTexture = null;

        GameObject.Destroy(_rt);
        RenderTexture.active = null;

        screenshotCam.gameObject.SetActive(false);

        totalTime = Time.time - startTime;
        actualFPS = framesCount / totalTime;

        //创建视频文件信息
        CreateVideo();
    }

    ///// <summary>
    ///// 屏幕绘制完毕之后，截取屏幕信息保存成 jpg 文件
    ///// </summary>
    //void OnPostRender()
    //{
    //    if (isREC && (frameTimer += Time.deltaTime) > frameInterval)
    //    {
    //        frameTimer -= frameInterval;

    //        _frameBuffer.ReadPixels(_camRect, 0, 0, false);

    //        File.WriteAllBytes(NextImgFilePath(), _frameBuffer.EncodeToJPG());
    //    }
    //}

    /// <summary>
    /// 屏幕绘制完毕之后，截取屏幕信息保存成 jpg 文件
    /// </summary>
    void Update()
    {
        if (isREC && (frameTimer += Time.deltaTime) > frameInterval)
        {
            frameTimer -= frameInterval;

            RenderTexture.active = _rt;

            _frameBuffer.ReadPixels(_camRect, 0, 0, false);
            

            File.WriteAllBytes(NextImgFilePath(), _frameBuffer.EncodeToJPG());
        }
    }

    /// <summary>
    /// 清理缓存目录信息
    /// </summary>
    private void Clear()
    {
        if(Directory.Exists(ConstantConfig.FFMPEG_REC_CASH_DIR))
            Directory.Delete(ConstantConfig.FFMPEG_REC_CASH_DIR, true);
    }

    /// <summary>
    /// 获取图片名字
    /// </summary>
    /// <returns></returns>
    private string NextImgFilePath()
    {
        return String.Format(imgFilePathFormat, framesCount++);
    }

    /// <summary>
    /// 获取最后一张图片文件地址
    /// </summary>
    /// <returns></returns>
    public string LastImgFilePath()
    {
        return String.Format(imgFilePathFormat, framesCount > 0 ? framesCount - 1 : framesCount);
    }

    /// <summary>
    /// 创建视频文件
    /// </summary>
    private void CreateVideo()
    {
        StringBuilder command = new StringBuilder();

        Debug.Log("firstImgFilePath: " + firstImgFilePath);
        Debug.Log("soundPath: " + soundPath);
        Debug.Log("outputVideoPath: " + outputVideoPath);

        //Input Image sequence params
        command.
            Append("-y -framerate ").
            Append(actualFPS.ToString()).
            Append(" -f image2 -i ").
            Append(GameUtils.AddQuotation(firstImgFilePath));

        ////Input Audio params
        //if (recAudioSource != RecAudioSource.None)
        //{
        //    command.
        //        Append(" -i ").
        //        Append(AddQuotation(soundPath)).
        //        Append(" -ss 0 -t ").
        //        Append(totalTime);
        //}

        //Output Video params
        command.
            Append(" -vcodec libx264 -crf 25 -pix_fmt yuv420p ").
            Append(GameUtils.AddQuotation(outputVideoPath));

        Debug.Log(command.ToString());

        //如果目录不存在，那么创建目录
        if (!Directory.Exists(Path.GetDirectoryName(outputVideoPath)))
            Directory.CreateDirectory(Path.GetDirectoryName(outputVideoPath));

        FFmpegManager.instance.DirectInput(command.ToString());
        //FFmpegCommands.DirectInput(command.ToString());
    }


    #endregion



    #region 创建视频进度信息

    //FFmpeg processing callbacks
    //------------------------------

    //Begining of video processing
    public void OnStart()
    {
        onStart("VideoProducing Started\n");
    }

    //You can make custom progress bar here (parse msg)
    public void OnProgress(string msg)
    {
        onProgress(msg);
    }

    //Notify user about failure here
    public void OnFailure(string msg)
    {
        onFailure(msg);
    }

    //Notify user about success here
    public void OnSuccess(string msg)
    {
        onSuccess(msg);
    }

    //Last callback - do whatever you need next
    public void OnFinish()
    {
        onFinish(outputVideoPath);
        //Clear();
        isProducing = false;
    }

    #endregion



}
