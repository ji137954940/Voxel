using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Color.Number.File;
using Color.Number.Grid;
using Color.Number.Utils;
using UnityEngine;
using ZLib;
using Color.Number.Animation;
using System.Collections;
using Color.Number.Camera;
using ZFrame;
using Color.Number.Event;

/// <summary>
/// FFmpeg 视频管理
/// </summary>
public class FFmpegManager : Singleton<FFmpegManager>
{

    #region 初始化

    public FFmpegManager()
    {
        GameInstance.FFmpegREC.AddAction(OnStart, OnProgress, OnSuccess, OnFailure, OnFinish);
    }

    /// <summary>
    /// 开始处理视频
    /// </summary>
    /// <param name="str"></param>
    private void OnStart(string str)
    {
        //Debug.Log(" video start " + str);
    }

    /// <summary>
    /// 处理视频进度
    /// </summary>
    /// <param name="str"></param>
    private void OnProgress(string str)
    {
        //Debug.Log(" video progress " + str);
    }

    /// <summary>
    /// 处理视频失败
    /// </summary>
    /// <param name="str"></param>
    private void OnFailure(string str)
    {
        var path1 = GetOriginalStaticVideoPath(_gridInfo);
        var folder = Path.GetDirectoryName(path1);
        var outputPath = folder + "/log.txt";

        Debug.LogError(outputPath);

        using (FileStream fileStream = File.Create(outputPath))
        {
            byte[] buffer =
                new UTF8Encoding(true).GetBytes(str);
            fileStream.Write(buffer, 0, buffer.Length);
        }

        //Debug.Log(" video failure " + str);
    }

    /// <summary>
    /// 处理视频成功
    /// </summary>
    /// <param name="str"></param>
    private void OnSuccess(string str)
    {
        //Debug.Log(" video success " + str);

#if UNITY_ANDROID || UNITY_IOS

        if(_isUpdateAlbum)
        {
            //刷新图片到相册
            PlatformManager.instance.SaveImageToAlbum(str);
        }
        else
        {
            _isUpdateAlbum = false;
        }

#endif
    }

    /// <summary>
    /// 创建视频结束的输出
    /// </summary>
    /// <param name="str"></param>
    private void OnFinish(string str)
    {
        _videoCommandRunning = false;

        if(_waitDirectInputQueue != null && _waitDirectInputQueue.Count > 0)
        {
            DirectInput(_waitDirectInputQueue.Dequeue());
        }

        //Debug.Log(" video finish " + str);
    }


#endregion


#region 保存视频

    /// <summary>
    /// grid 信息
    /// </summary>
    private GridInfo _gridInfo;
    /// <summary>
    /// voxel 信息
    /// </summary>
    private VoxelInfo _voxelInfo;

    private bool _isUpdateAlbum = false;

    /// <summary>
    /// 存储视频信息
    /// </summary>
    /// <param name="gridInfo"></param>
    public void SaveVideo(GridInfo gridInfo)
    {

        _gridInfo = gridInfo;

        var path1 = GetOriginalStaticVideoPath(gridInfo);
        var path2 = GetFillColorVideoPath(gridInfo);
        var path3 = GetLogoVideoPath();

        List<string> list = null;
        if(!_gridInfo.IsAnimation)
        {
            list = new List<string>() { path1, path2, path1, path3 };
        }
        else
        {
            var dynamicTextureVideo = GetOriginalDynamicVideoPath(_gridInfo);
            list = new List<string>() { path1, path2 };

            var times = ConstantConfig.GetGameConfigInt(GameConfigKey.ffmpeg_image_dynamic_texture_show_times);

            for (int i = 0; i < times; i++)
            {
                list.Add(dynamicTextureVideo);
            }

            list.Add(path1);
            list.Add(path3);
        }

        CreateVideo(list);

//        //存储位置
//        var outputPath = string.Format("{0}{1}.mp4", ConstantConfig.CAMERA_SCREEN_SHOTS_PATH_ANDROID, GameUtils.GetCurTime());


        //        List<string> cmd = new List<string>();
        //        StringBuilder filter = new StringBuilder();

        //#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_EDITOR
        //        filter.Append(FFmpegCommands.DOUBLE_QUOTE);
        //#endif
        //        for (int i = 0; i < list.Count; i++)
        //        {
        //            cmd.Add(FFmpegCommands.INPUT_INSTRUCTION);

        //            var str = list[i];

        //#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_EDITOR

        //            str = str.Substring(str.IndexOf(":") + 1).Replace('\\', '/');

        //#else
        //            str = str.Replace('\\', '/');
        //#endif

        //            cmd.Add(str);

        //            filter.Append(string.Format(FFmpegCommands.VIDEO_FORMAT, i));
        //        }

        //        filter.Append(string.Format(FFmpegCommands.CONCAT_VIDEO_FORMAT, FFmpegCommands.CONCAT_INSTRUCTION, list.Count)).
        //            Append(FFmpegCommands.SEPARATOR).
        //            Append(FFmpegCommands.VIDEO_INSTRUCTION);

        //#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_EDITOR
        //        filter.Append(FFmpegCommands.DOUBLE_QUOTE);
        //#endif
        //        cmd.Add(FFmpegCommands.FILTER_COMPLEX_INSTRUCTION);
        //        cmd.Add(filter.ToString());
        //        cmd.Add(FFmpegCommands.MAP_INSTRUCTION);
        //        cmd.Add(FFmpegCommands.VIDEO_INSTRUCTION);
        //        //cmd.Add(FFmpegCommands.MAP_INSTRUCTION);
        //        //cmd.Add(FFmpegCommands.AUDIO_INSTRUCTION);
        //        cmd.Add(FFmpegCommands.PRESET_INSTRUCTION);
        //        cmd.Add(FFmpegCommands.ULTRASAFE_INSTRUCTION);
        //        cmd.Add(GameUtils.AddQuotation(outputPath.Replace('\\', '/')));
        //        cmd.Add(FFmpegCommands.REWRITE_INSTRUCTION);

        //        _isUpdateAlbum = true;

        //        string[] command = cmd.ToArray();
        //        DirectInput(command);
        //        //FFmpegCommands.DirectInput(command);

    }

    /// <summary>
    /// 存储视频信息
    /// </summary>
    /// <param name="gridInfo"></param>
    public void SaveVideo(VoxelInfo voxelInfo)
    {
        _voxelInfo = voxelInfo;

        //var path1 = GetOriginalStaticVideoPath(voxelInfo);
        var path1 = GetFillColorVideoPath(voxelInfo);
        var path2 = GetLogoVideoPath();

        var list = new List<string>() { path1, path2 };

        CreateVideo(list);

    }

    /// <summary>
    /// 创建 video 
    /// </summary>
    /// <param name="list"></param>
    private void CreateVideo(List<string> list)
    {
        //存储位置
        var outputPath = string.Format("{0}{1}.mp4", ConstantConfig.GetGameConfigString(GameConfigKey.camera_screen_shots_path_android), GameUtils.GetCurTime());

        List<string> cmd = new List<string>();
        StringBuilder filter = new StringBuilder();

#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_EDITOR
        filter.Append(FFmpegCommands.DOUBLE_QUOTE);
#endif
        for (int i = 0; i < list.Count; i++)
        {
            cmd.Add(FFmpegCommands.INPUT_INSTRUCTION);

            var str = list[i];

#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_EDITOR

            str = str.Substring(str.IndexOf(":") + 1).Replace('\\', '/');

#else
            str = str.Replace('\\', '/');
#endif

            cmd.Add(str);

            filter.Append(string.Format(FFmpegCommands.VIDEO_FORMAT, i));
        }

        filter.Append(string.Format(FFmpegCommands.CONCAT_VIDEO_FORMAT, FFmpegCommands.CONCAT_INSTRUCTION, list.Count)).
            Append(FFmpegCommands.SEPARATOR).
            Append(FFmpegCommands.VIDEO_INSTRUCTION);

#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_EDITOR
        filter.Append(FFmpegCommands.DOUBLE_QUOTE);
#endif
        cmd.Add(FFmpegCommands.FILTER_COMPLEX_INSTRUCTION);
        cmd.Add(filter.ToString());
        cmd.Add(FFmpegCommands.MAP_INSTRUCTION);
        cmd.Add(FFmpegCommands.VIDEO_INSTRUCTION);
        //cmd.Add(FFmpegCommands.MAP_INSTRUCTION);
        //cmd.Add(FFmpegCommands.AUDIO_INSTRUCTION);
        cmd.Add(FFmpegCommands.PRESET_INSTRUCTION);
        cmd.Add(FFmpegCommands.ULTRASAFE_INSTRUCTION);
        cmd.Add(GameUtils.AddQuotation(outputPath.Replace('\\', '/')));
        cmd.Add(FFmpegCommands.REWRITE_INSTRUCTION);

        _isUpdateAlbum = true;

        string[] command = cmd.ToArray();
        DirectInput(command);
        //FFmpegCommands.DirectInput(command);
    }

    /// <summary>
    /// 保存颜色填充视频信息
    /// </summary>
    /// <param name="gridInfo"></param>
    public void StartFillColorVideoREC(GridInfo gridInfo)
    {
        _gridInfo = gridInfo;
        var path = GetFillColorVideoPath(gridInfo);

        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError(" 记录颜色填充过程视频错误 ");
            return;
        }

        //已经存在了视频，不需要重新录制
        if (File.Exists(path))
        {
            return;
        }

        GameInstance.FFmpegREC.StartREC(path);
    }

    /// <summary>
    /// 保存颜色填充视频信息
    /// </summary>
    /// <param name="gridInfo"></param>
    public void StartFillColorVideoREC(VoxelInfo voxelInfo)
    {
        _voxelInfo = voxelInfo;
        var path = GetFillColorVideoPath(voxelInfo);

        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError(" 记录颜色填充过程视频错误 ");
            return;
        }

        //已经存在了视频，不需要重新录制
        if (File.Exists(path))
        {
            return;
        }

        GameInstance.FFmpegREC.StartREC(path);
    }

    /// <summary>
    /// 停止颜色填充过程录制
    /// </summary>
    /// <param name="isVoxel"></param>
    public void StopFillColorVideoREC(bool isVoxel)
    {

        if (GameInstance.FFmpegREC.isREC)
        {
            if(!isVoxel)
                SaveOriginalStaticImageVideo(GameInstance.FFmpegREC.LastImgFilePath());
            GameInstance.FFmpegREC.StopREC();
        }
    }

    /// <summary>
    /// 保存静态原始颜色的图片为一个视频
    /// </summary>
    /// <param name="gridInfo"></param>
    public void SaveOriginalStaticImageVideo(string img)
    {
        var path = GetOriginalStaticVideoPath(_gridInfo);
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError(" 获取静态原始图片视频地址错误 ");
            return;
        }

        //已经存在了视频，不需要重新创建
        if (File.Exists(path))
        {
            return;
        }

        //创建视频文件
        if (!File.Exists(img))
        {
            Debug.LogError(" 创建视频失败， 图片不存在 " + img);
            return;
        }

        //创建循环视频
        CreateImageLoopVideo(img, path);
    }

    /// <summary>
    /// 创建 image 循环视频
    /// </summary>
    private void CreateImageLoopVideo(string firstImgUrl, string outVideoUrl)
    {
        var command = new StringBuilder();

        //输入信息
        command.
            Append("-y -framerate ").
            Append(ConstantConfig.GetGameConfigInt(GameConfigKey.ffmpeg_fps)).
            Append(" -t ").
            Append(ConstantConfig.GetGameConfigInt(GameConfigKey.ffmpeg_image_loop_video_time)).
            Append(" -loop 1 -i ").
            Append(GameUtils.AddQuotation(firstImgUrl));

        //输出视频信息
        command.
            Append(" -vcodec libx264 -crf 25 -pix_fmt yuv420p ").
            Append(GameUtils.AddQuotation(outVideoUrl));

        DirectInput(command.ToString());

        //创建视频文件
        //FFmpegCommands.DirectInput(command.ToString());
    }



#region 视频创建队列，同时只支持一条处理命令

    /// <summary>
    /// 存储视频处理命令， 同一时间只能处理一条数据
    /// </summary>
    private ZLib.Queue<string[]> _waitDirectInputQueue = new ZLib.Queue<string[]>();

    /// <summary>
    /// 视频处理命令是否正在运行
    /// </summary>
    private bool _videoCommandRunning = false;

    /// <summary>
    /// 创建视频文件输入命令
    /// </summary>
    /// <param name="str"></param>
    public void DirectInput(string str)
    {
        string[] command = str.Split(' ');

        DirectInput(command);
    }

    /// <summary>
    /// 创建视频文件输入命令
    /// </summary>
    /// <param name="command"></param>
    public void DirectInput(string[] command)
    {
        //正在运行命令，那么添加到等待运行队列
        if(_videoCommandRunning)
        {
            _waitDirectInputQueue.Enqueue(command);
            return;
        }

        _videoCommandRunning = true;
        FFmpegCommands.DirectInput(command);

    }

    #endregion


    #endregion


    #region Dynamic 图片信息

    /// <summary>
    /// 动态图片存储目录
    /// </summary>
    private static string FFMPEG_ORIGINAL_DYNAMIC_DIR = System.IO.Path.Combine(Application.temporaryCachePath, "Dynamic");
    /// <summary>
    /// 动态图片 第一张图片名字
    /// </summary>
    private static string FIRST_DAYNAMIC_NAME = "%0d.jpg";

    /// <summary>
    /// 正在运行的协成程序
    /// </summary>
    private Coroutine _coroutine;

    /// <summary>
    /// 是否需要创建 动态图片视频
    /// </summary>
    /// <param name="gridInfo"></param>
    /// <returns></returns>
    public bool IsNeedCreateOriginalDynamicVideo(GridInfo gridInfo)
    {
        var outPath = GetOriginalDynamicVideoPath(gridInfo);

        if (File.Exists(outPath))
        {
            //已经存在，不需要重新创建
            return false;
        }

        return true;
    }

    /// <summary>
    /// 创建动态图片视频
    /// </summary>
    /// <param name="textureId"></param>
    /// <param name="textureCount"></param>
    public void CreateOriginalDynamicVideo(int textureId, int textureCount)
    {

        var flag = IsNeedCreateOriginalDynamicVideo(_gridInfo);
        if(!flag)
        {
            if (textureId >= textureCount - 1)
                Frame.DispatchEvent(ME.New<ME_Show_SaveUI>(p => { p.isActivity = true; }));
            return;
        }

        var pngPath = string.Format("{0}/{1}/{2}", FFMPEG_ORIGINAL_DYNAMIC_DIR, _gridInfo.TypeId, _gridInfo.Id);

        //如果目录不存在，那么创建目录
        if (!Directory.Exists(pngPath))
            Directory.CreateDirectory(pngPath);

        //var tex = GameInstance.FFmpegREC._frameBuffer;
        var tex = CameraManager.GetInst().UICaptureScreenshot();
        var name = string.Format("{0}/{1}.jpg", pngPath, textureId);
        if (tex != null)
        {
            //存储所有的文件
            File.WriteAllBytes(name, tex.EncodeToJPG());
        }

        if(textureId >= textureCount - 1)
        {
            var imageFilePath = string.Format("{0}/{1}", pngPath, FIRST_DAYNAMIC_NAME);
            var outPath = GetOriginalDynamicVideoPath(_gridInfo);

            //开始创建视频文件
            StringBuilder command = new StringBuilder();

            //Input Image sequence params
            command.
                Append("-y -framerate ").
                Append(ConstantConfig.GetGameConfigInt(GameConfigKey.ffmpeg_fps)).
                Append(" -f image2 -i ").
                Append(GameUtils.AddQuotation(imageFilePath));

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
                Append(GameUtils.AddQuotation(outPath));

            Debug.Log(command.ToString());

            //如果目录不存在，那么创建目录
            if (!Directory.Exists(Path.GetDirectoryName(outPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(outPath));

            DirectInput(command.ToString());

            //显示UI保存界面
            Frame.DispatchEvent(ME.New<ME_Show_SaveUI>(p => { p.isActivity = true; }));
        }
    }

    ///// <summary>
    ///// 创建动态图片视频
    ///// </summary>
    ///// <param name="gridInfo"></param>
    //public void CreateOriginalDynamicVideo(GridInfo gridInfo, List<GifAnimation.GifTexture> list)
    //{
    //    _gridInfo = gridInfo;

    //    var outPath = GetOriginalDynamicVideoPath(gridInfo);

    //    if(File.Exists(outPath))
    //    {
    //        //已经存在，不需要重新创建
    //        return;
    //    }

    //    var pngPath = string.Format("{0}/{1}/{2}", FFMPEG_ORIGINAL_DYNAMIC_DIR, gridInfo.TypeId, gridInfo.Id);

    //    if(_coroutine != null)
    //    {
    //        CoroutineManager.instance.StopCoroutine(_coroutine);
    //        _coroutine = null;
    //    }
    //    _coroutine = CoroutineManager.instance.StartCoroutine(SaveOriginalDynamicTexture(pngPath, outPath, list));
    //}

    ///// <summary>
    ///// 存储图片文件，并且创建视频
    ///// </summary>
    ///// <param name="path"></param>
    ///// <param name="list"></param>
    ///// <returns></returns>
    //private IEnumerator SaveOriginalDynamicTexture(string pngPath, string outPath, List<GifAnimation.GifTexture> list)
    //{
    //    //如果目录不存在，那么创建目录
    //    if (!Directory.Exists(pngPath))
    //        Directory.CreateDirectory(pngPath);

    //    //存储 tex 图片
    //    for (int i = 0; i < list.Count; i++)
    //    {
    //        var tex = list[i].Texture2D;
    //        var name = string.Format("{0}/{1}.png", pngPath, i);
    //        if (tex != null)
    //        {
    //            //存储所有的文件
    //            File.WriteAllBytes(name, tex.EncodeToJPG());

    //            yield return null;
    //        }
    //    }

    //    var imageFilePath = string.Format("{0}/{1}", pngPath, FIRST_DAYNAMIC_NAME);

    //    //开始创建视频文件
    //    StringBuilder command = new StringBuilder();

    //    //Input Image sequence params
    //    command.
    //        Append("-y -framerate ").
    //        Append(FFMPEG_ORIGINAL_DYNAMIC_FPS.ToString()).
    //        Append(" -f image2 -i ").
    //        Append(GameUtils.AddQuotation(imageFilePath));

    //    ////Input Audio params
    //    //if (recAudioSource != RecAudioSource.None)
    //    //{
    //    //    command.
    //    //        Append(" -i ").
    //    //        Append(AddQuotation(soundPath)).
    //    //        Append(" -ss 0 -t ").
    //    //        Append(totalTime);
    //    //}

    //    //Output Video params
    //    command.
    //        Append(" -vcodec libx264 -crf 25 -pix_fmt yuv420p ").
    //        Append(GameUtils.AddQuotation(outPath));

    //    Debug.Log(command.ToString());

    //    //如果目录不存在，那么创建目录
    //    if (!Directory.Exists(Path.GetDirectoryName(outPath)))
    //        Directory.CreateDirectory(Path.GetDirectoryName(outPath));

    //    DirectInput(command.ToString());

    //    _coroutine = null;
    //}

    #endregion


    #region 视频片段路径信息

    /// <summary>
    /// 视频片段保存路径
    /// </summary>
    private string _videoDirectory = "video";

    /// <summary>
    /// 颜色填充视频名字
    /// </summary>
    private string _fillColorVideoName = "fill";

    /// <summary>
    /// 原始颜色静止图片名字
    /// </summary>
    private string _originalStaticImageVideoName = "originalStatic";

    /// <summary>
    /// 延时颜色动态图片名字
    /// </summary>
    private string _originalDynamicImageVideoName = "originalDynamic";

    /// <summary>
    /// logo 视频名字
    /// </summary>
    private string _logoVideoName = "Logo.mp4";

    /// <summary>
    /// 获取logo视频信息
    /// </summary>
    /// <returns></returns>
    public string GetLogoVideoPath()
    {
        var path = string.Format("{0}/{1}/{2}",
                ConstantConfig.CACHE_PATH,
                ConstantConfig.GROUP_SETUP,
                _logoVideoName);

        return path;
    }


    /// <summary>
    /// 获取颜色填充视频路径
    /// </summary>
    /// <param name="gridInfo"></param>
    /// <returns></returns>
    public string GetFillColorVideoPath(GridInfo gridInfo)

    {
        var path = string.Format("{0}/{1}/{2}/{3}/{4}/{5}.mp4",
            ConstantConfig.CACHE_PATH,
            ConstantConfig.GROUP_SETUP,
            _videoDirectory,
            gridInfo.TypeId,
            gridInfo.Id,
            _fillColorVideoName
        );
        return path;
    }

    /// <summary>
    /// 获取颜色填充视频路径
    /// </summary>
    /// <param name="voxelInfo"></param>
    /// <returns></returns>
    public string GetFillColorVideoPath(VoxelInfo voxelInfo)

    {
        var path = string.Format("{0}/{1}/{2}/{3}/{4}/{5}.mp4",
            ConstantConfig.CACHE_PATH,
            ConstantConfig.GROUP_SETUP,
            _videoDirectory,
            voxelInfo.TypeId,
            voxelInfo.Id,
            _fillColorVideoName
        );
        return path;
    }

    /// <summary>
    /// 获取静态原始颜色视频路径
    /// </summary>
    /// <param name="gridInfo"></param>
    /// <returns></returns>
    public string GetOriginalStaticVideoPath(GridInfo gridInfo)
    {
        var path = string.Format("{0}/{1}/{2}/{3}/{4}/{5}.mp4",
            ConstantConfig.CACHE_PATH,
            ConstantConfig.GROUP_SETUP,
            _videoDirectory,
            gridInfo.TypeId,
            gridInfo.Id,
            _originalStaticImageVideoName
        );
        return path;
    }

    /// <summary>
    /// 获取静态原始颜色视频路径
    /// </summary>
    /// <param name="voxelInfo"></param>
    /// <returns></returns>
    public string GetOriginalStaticVideoPath(VoxelInfo voxelInfo)
    {
        var path = string.Format("{0}/{1}/{2}/{3}/{4}/{5}.mp4",
            ConstantConfig.CACHE_PATH,
            ConstantConfig.GROUP_SETUP,
            _videoDirectory,
            voxelInfo.TypeId,
            voxelInfo.Id,
            _originalStaticImageVideoName
        );
        return path;
    }

    /// <summary>
    /// 获取动态原始颜色视频路径
    /// </summary>
    /// <param name="gridInfo"></param>
    /// <returns></returns>
    public string GetOriginalDynamicVideoPath(GridInfo gridInfo)
    {
        var path = string.Format("{0}/{1}/{2}/{3}/{4}/{5}.mp4",
            ConstantConfig.CACHE_PATH,
            ConstantConfig.GROUP_SETUP,
            _videoDirectory,
            gridInfo.TypeId,
            gridInfo.Id,
            _originalDynamicImageVideoName
        );
        return path;
    }

#endregion






}

