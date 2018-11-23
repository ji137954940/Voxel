using Color.Number.GameInfo;
using Color.Number.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Color.Number.Event;
using Color.Number.File;
using Color.Number.Scene;
using Tgame.Game.Table;
using UnityEngine;
using ZFrame;
using Color = UnityEngine.Color;

namespace Color.Number.Grid
{

    /// <summary>
    /// Grid 信息
    /// </summary>
    public class GridInfo
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
        /// 是否为动画文件
        /// </summary>
        public bool IsAnimation { get; private set; }

        /// <summary>
        /// 屏幕截图 为动画文件的时候可以需要使用
        /// </summary>
        public Texture2D CaptureScreenshot { get; set; }

        /// <summary>
        /// 当前texture颜色是否全部填充完成
        /// </summary>
        public bool IsTextureColoringComplete { get; private set; }

        /// <summary>
        /// 宽
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// 高
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// 原始颜色数组
        /// </summary>
        private UnityEngine.Color[] OriginalColor { get; set; }

        /// <summary>
        /// 灰色存储数组
        /// </summary>
        public UnityEngine.Color[] GreyColorArr { get; private set; }

        /// <summary>
        /// 像素颜色字典(灰化之后颜色，像素颜色信息)
        /// </summary>
        public Dictionary<UnityEngine.Color, PosColorInfo> PixelColorDic { get; private set; }

        /// <summary>
        /// 像素颜色数组
        /// </summary>
        public PosColorInfo[] PixelColorArr { get; private set; }

        /// <summary>
        /// 像素颜色完成顺序 （记录的为id）
        /// </summary>
        public List<int> PixelColorCompleteOrder { get; private set; }

        /// <summary>
        /// 像素颜色完成 区域id 数组
        /// </summary>
        public List<int> PixelColorCompleteAreaIdList { get; private set; }

        /// <summary>
        /// 设置颜色数据信息
        /// </summary>
        /// <param name="table"></param>
        /// <param name="cameraPhoto"></param>
        /// <param name="arr"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetColorInfo(Table_Client_Icon table, CameraPhotoInfo cameraPhoto, UnityEngine.Color[] arr, int width, int height)
        {
            if (arr == null || arr.Length == 0)
            {
                Debug.LogError(" Can Not Set Color Info, Color Array Is Null ");
                return;
            }

            if (table != null)
            {
                TypeId = table.type_id;
                Id = table.id;
                IsAnimation = table.is_animation;
            }
            else
            {
                TypeId = cameraPhoto.type_id;
                Id = cameraPhoto.id;
                IsAnimation = false;
            }

            Width = width;
            Height = height;

            var count = arr.Length;

            //存储原始颜色数据
            OriginalColor = arr;

            //获取当前 texture 颜色是否完全填充完成
            IsTextureColoringComplete = PlayerGameInfo.instance.IsCompleteInfo(TypeId, Id);

            //if (!IsTextureColoringComplete)
            {
                //存储颜色字典
                PixelColorDic = new Dictionary<UnityEngine.Color, PosColorInfo>();

                //灰色颜色数组
                GreyColorArr = new UnityEngine.Color[count];

                //灰色颜色 系数
                Vector3 greyColorVector = new Vector3(0.299f, 0.587f, 0.114f);

                for (int i = 0; i < count; i++)
                {
                    if (arr[i].a <= 0)
                    {
                        GreyColorArr[i] = arr[i];
                        continue;
                    }

                    var color = arr[i];
                    var grey = Vector3.Dot(new Vector3(color.r, color.g, color.b), greyColorVector);
                    GreyColorArr[i] = new UnityEngine.Color(grey, grey, grey);

                    if (PixelColorDic.ContainsKey(arr[i]))
                    {
                        PixelColorDic[arr[i]].AddColorIndexId(i);
                    }
                    else
                    {
                        PixelColorDic[arr[i]] = new PosColorInfo(arr[i], GreyColorArr[i], i);
                    }
                }

                var array = PixelColorDic.Values.ToArray();

                //快速排序(从小到大顺序)
                GameUtils.QuickSort(ref array, 0, array.Length - 1);
                //翻转数组顺序
                Array.Reverse(array);
                //存储颜色值数组
                PixelColorArr = array;

                //设置默认选中颜色数据信息
                SetDefaultSelectColorInfo();

                //初始化颜色数组信息
                for (int i = 0; i < PixelColorArr.Length; i++)
                {
                    PixelColorArr[i].InitPosColorArrInfo();
                }

                Debug.LogError(" 获取的颜色种类 " + array.Length);

            }

            int[] colorCompleteAreaIdArr = null;
            UnityEngine.Color[] originalColorTypeArr = null;
            //获取已经完成的信息
            var completeColorArr = FileManager.instance.ReadFileInfo(TypeId, Id, out colorCompleteAreaIdArr, out originalColorTypeArr);
            if (completeColorArr != null && completeColorArr.Length > 0)
            {
                PixelColorCompleteOrder = new List<int>(completeColorArr);
                PixelColorCompleteAreaIdList = new List<int>(colorCompleteAreaIdArr);

                ////如果已经填充完成，那么才需要读取此数据
                if (IsTextureColoringComplete)
                {
                    //count = originalColorTypeArr.Length;
                    //PixelColorArr = new PosColorInfo[count];
                    //for (int i = 0; i < count; i++)
                    //{
                    //    var info = new PosColorInfo(originalColorTypeArr[i], UnityEngine.Color.white, 0);
                    //    PixelColorArr[i] = info;
                    //}
                }
                else
                {
                    //从已经排序的队列中，把已经填充完毕的数据移除
                    RemoveFillColorInfo();
                }

                //通知UI显示已经完成的数据
                //Frame.DispatchEvent(ME.New<ME_Coloring_Done>(p => { p.ColoringDoneIdArr = colorCompleteAreaIdArr; }));
            }
        }

        /// <summary>
        /// 获取一个位置的原始颜色数组
        /// </summary>
        /// <param name="indexId"></param>
        /// <returns></returns>
        public UnityEngine.Color GetOriginalColor(int indexId)
        {
            if(indexId < 0 || OriginalColor.Length <= indexId)
                return UnityEngine.Color.white;

            return OriginalColor[indexId];
        }

        /// <summary>
        /// 获取原始颜色类型数组
        /// </summary>
        /// <returns></returns>
        public UnityEngine.Color[] GetOriginalColorTypeArr()
        {
            var count = PixelColorArr.Length;
            UnityEngine.Color[] arr = new UnityEngine.Color[count];
            for (int i = 0; i < count; i++)
            {
                arr[i] = PixelColorArr[i].PosColor;
            }

            return arr;
        }

        /// <summary>
        /// 检测一个 indexId 位置所属的 颜色 area id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="indexId"></param>
        /// <returns></returns>
        public bool IsInColorArea(int id, int indexId)
        {
            if (id < 1 || id > PixelColorArr.Length)
                return false;

            var info = PixelColorArr[id - 1];
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
            if (areaId < 1 || areaId > PixelColorArr.Length)
                return;

            var info = PixelColorArr[areaId - 1];
            if (info != null)
            {
                info.Remove(indexId);
                if (info.PosColorDic.Count == 0)
                {
                    //是否当前所有的颜色都已经填充完毕，如果是那么标志为已经完成

                    if(PixelColorCompleteAreaIdList == null)
                        PixelColorCompleteAreaIdList = new List<int>();

                    PixelColorCompleteAreaIdList.Add(areaId - 1);

                    IsTextureColoringComplete = PixelColorCompleteAreaIdList.Count == PixelColorArr.Length;
                    //如果已经填图完成，那么存储当前已经完成的标识
                    if (IsTextureColoringComplete)
                    {
                        PlayerGameInfo.instance.AddCompleteInfo(TypeId, Id);
                        //已经完成了，那么保存信息
                        FileManager.instance.SaveColorInfo(this);
                        //保存动画视频
                        //FFmpegManager.instance.SaveOriginalStaticImageVideo(this);
                    }

                    //通知UI显示已经完成的数据
                    Frame.DispatchEvent(ME.New<ME_Coloring_Done>(p => { p.ColoringDoneIdArr = new int[] { areaId - 1}; }));
                }
            }
        }

        /// <summary>
        /// 移除填充颜色信息
        /// </summary>
        private void RemoveFillColorInfo()
        {
            if (PixelColorCompleteOrder != null)
            {
                var count = PixelColorArr.Length;
                for (int i = 0; i < PixelColorCompleteOrder.Count;)
                {
                    for (int j = 0; j < count; j++)
                    {
                        var info = PixelColorArr[j];
                        
                        if (info != null
                            && info.Remove(PixelColorCompleteOrder[i]))
                            break;
                    }

                    i++;
                }

            }
        }

        /// <summary>
        /// 设置默认选中的 颜色信息
        /// </summary>
        public void SetDefaultSelectColorInfo()
        {
            SceneManager.instance.SetSelectColorInfo(1);
        }

        /// <summary>
        /// 更新 像素颜色完成顺序列表
        /// </summary>
        /// <param name="list"></param>
        public void UpdatePixelColorCompleteOrder(List<int> list)
        {
            PixelColorCompleteOrder = list;
        }

        /// <summary>
        /// 自动填充颜色
        /// </summary>
        public void AutoFillColor()
        {
            var count = PixelColorArr.Length;
            for (int i = 0; i < count; i++)
            {
                if (PixelColorArr[i] != null
                    && PixelColorArr[i].PosColorDic.Count > 0)
                {
                    var arr = PixelColorArr[i].PosColorDic.Keys.ToArray();

                    for (int j = 0; j < arr.Length; j++)
                    {
                        RemoveColorInfo(i + 1, arr[j]);
                    }
                }
            }
        }

        /// <summary>
        /// 数据清理
        /// </summary>
        public void Clear()
        {
            TypeId = 0;
            Id = 0;
            IsAnimation = false;
            Width = 0;
            Height = 0;
            IsTextureColoringComplete = false;
            OriginalColor = null;
            GreyColorArr = null;

            if (CaptureScreenshot != null)
                CaptureScreenshot = null;

            if (PixelColorDic != null)
            {
                foreach (var item in PixelColorDic)
                {
                    item.Value.Clear();
                }

                PixelColorDic.Clear();
                PixelColorDic = null;
            }

            PixelColorArr = null;
            if (PixelColorCompleteOrder != null)
            {
                PixelColorCompleteOrder.Clear();
                PixelColorCompleteOrder = null;
            }
    }
    }

    /// <summary>
    /// pos 颜色信息
    /// </summary>
    public class PosColorInfo
    {
        /// <summary>
        /// 位置原始颜色
        /// </summary>
        public UnityEngine.Color PosColor { get; private set; }

        /// <summary>
        /// 位置 灰化之后的颜色
        /// </summary>
        public UnityEngine.Color PosGreyColor { get; private set; }

        /// <summary>
        /// 存储当前颜色像素的字典 （id, ）
        /// </summary>
        public Dictionary<int, bool> PosColorDic {get; private set; }

        /// <summary>
        /// 存储当前颜色像素的数组
        /// </summary>
        public int[] PosColorArr { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="color"></param>
        /// <param name="greyColor"></param>
        /// <param name="indexId"></param>
        public PosColorInfo(UnityEngine.Color color, UnityEngine.Color greyColor, int indexId)
        {
            PosColor = color;
            PosGreyColor = greyColor;

            PosColorDic = new Dictionary<int, bool>();
            AddColorIndexId(indexId);
        }

        /// <summary>
        /// 初始化颜色信息数组
        /// </summary>
        public void InitPosColorArrInfo()
        {
            PosColorArr = PosColorDic.Keys.ToArray();
        }

        /// <summary>
        /// 添加颜色 index id 列表
        /// </summary>
        /// <param name="id"></param>
        public void AddColorIndexId(int id)
        {
            PosColorDic[id] = true;
        }

        /// <summary>
        /// 是否包含一个 id 数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Contains(int id)
        {
            if (PosColorDic == null)
                return false;

            return PosColorDic.ContainsKey(id);
        }

        /// <summary>
        /// 移除一条数据信息
        /// </summary>
        /// <param name="id"></param>
        public bool Remove(int id)
        {
            if (PosColorDic == null)
                return false;

            return PosColorDic.Remove(id);
        }

        /// <summary>
        /// 清理数据信息
        /// </summary>
        public void Clear()
        {
            if (PosColorDic != null)
            {
                PosColorDic.Clear();
                PosColorDic = null;
            }

            if (PosColorArr != null
                && PosColorArr.Length > 0)
                PosColorArr = null;
        }
    }
}

