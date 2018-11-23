using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Tgame.Game.Table;
using UnityEngine;
using ZLib;
using ZTool.Res;

namespace Color.Number.Animation
{
    /// <summary>
    /// Gif 图片动画
    /// </summary>
    public partial class GifAnimation : Singleton<GifAnimation>
    {

        #region 123

        public List<GifTexture> GetGifAnimation(int typeId, int id)
        {
            var path = Table_Client_Icon.GetPrimary(typeId, id).path;

            //根据不同的运行平台进行区分
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                case RuntimePlatform.IPhonePlayer:
                    break;
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    //path = string.Format("{0}/Resources/{1}.gif", Application.dataPath, path);
                    break;
            }

            TextAsset ta = Resources.Load(path, typeof(TextAsset)) as TextAsset;
            if (ta != null)
            {
                //var bytes = ta.bytes;
                //var ms = new MemoryStream(bytes);
                //var image = Image.FromStream(ms);

                //var list = GifToTextureByCs(image);

                //return list;

                return GetTextureListInfo(ta.bytes, ta.GetInstanceID());
            }

            //var gifImage = Image.FromFile(path);
            //var list = GifToTextureByCs(gifImage);

            //return list;

            return null;
        }

        void OnLoadBytesDone(string path, object obj, object parameter)
        {
            Debug.LogError(" load bytes path " + obj);

            byte[] bytes = (byte[]) obj;
            MemoryStream ms = new MemoryStream(bytes);
            Image image = Image.FromStream(ms);
        }


        /// <summary>
        /// gif转换图片
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private List<Texture2D> GifToTextureByCs(System.Drawing.Image image)
        {
            List<Texture2D> texture2D = null;
            if (null != image)
            {
                //texture2D = new List<Texture2D>();
                ////Debug.LogError(image.FrameDimensionsList.Length);
                ////image.FrameDimensionsList.Length = 1;
                ////根据指定的唯一标识创建一个提供获取图形框架维度信息的实例;
                //FrameDimension frameDimension = new FrameDimension(image.FrameDimensionsList[0]);
                ////获取指定维度的帧数;
                //int framCount = image.GetFrameCount(frameDimension);
                //for (int i = 0; i < framCount; i++)
                //{
                //    //选择由维度和索引指定的帧;
                //    image.SelectActiveFrame(frameDimension, i);
                //    var framBitmap = new Bitmap(image.Width, image.Height);
                //    //从指定的Image 创建新的Graphics,并在指定的位置使用原始物理大小绘制指定的 Image;
                //    //将当前激活帧的图形绘制到framBitmap上;
                //    System.Drawing.Graphics.FromImage(framBitmap).DrawImage(image, Point.Empty);
                //    var frameTexture2D = new Texture2D(framBitmap.Width, framBitmap.Height);
                //    for (int x = 0; x < framBitmap.Width; x++)
                //    {
                //        for (int y = 0; y < framBitmap.Height; y++)
                //        {
                //            //获取当前帧图片像素的颜色信息;
                //            System.Drawing.Color sourceColor = framBitmap.GetPixel(x, y);
                //            //设置Texture2D上对应像素的颜色信息;
                //            frameTexture2D.SetPixel(x, framBitmap.Height - 1 - y, new Color32(sourceColor.R, sourceColor.G, sourceColor.B, sourceColor.A));
                //        }
                //    }
                //    frameTexture2D.Apply();
                //    texture2D.Add(frameTexture2D);
                //}


                texture2D = new List<Texture2D>();
                Debug.Log("图片张数：" + image.FrameDimensionsList.Length);
                var frame = new FrameDimension(image.FrameDimensionsList[0]);
                int framCount = image.GetFrameCount(frame);//获取维度帧数
                for (int i = 0; i < framCount; ++i)
                {
                    image.SelectActiveFrame(frame, i);
                    var framBitmap = new Bitmap(image.Width, image.Height);
                    using (System.Drawing.Graphics graphic = System.Drawing.Graphics.FromImage(framBitmap))
                    {
                        graphic.DrawImage(image, Point.Empty);
                    }
                    var frameTexture2D = new Texture2D(framBitmap.Width, framBitmap.Height, TextureFormat.ARGB32, false);
                    frameTexture2D.filterMode = FilterMode.Point;
                    frameTexture2D.LoadImage(Bitmap2Byte(framBitmap));
                    texture2D.Add(frameTexture2D);
                }
            }
            return texture2D;
        }

        /// <summary>
        /// bitmap 数据 转换成 byte[] 数据
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        private byte[] Bitmap2Byte(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                // 将bitmap 以png格式保存到流中
                bitmap.Save(stream, ImageFormat.Png);
                // 创建一个字节数组，长度为流的长度
                byte[] data = new byte[stream.Length];
                // 重置指针
                stream.Seek(0, SeekOrigin.Begin);
                // 从流读取字节块存入data中
                stream.Read(data, 0, Convert.ToInt32(stream.Length));
                return data;
            }
        }

        #endregion

        #region 解析 gif 图片

        /// <summary>
        /// gif 图片信息数据缓存
        /// </summary>
        private Dictionary<int, GifInfo> _gifDict = new Dictionary<int, GifInfo>();

        /// <summary>
        /// 获取Texture 信息
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<GifTexture> GetTextureListInfo(byte[] bytes, int id)
        {
            var gifData = new GifData();
            if (!UniGifFormatter.SetGifData(bytes, ref gifData))
            {
                return null;
            }

            var gif = new GifInfo();
            gif.Width = gifData.logicalScreenWidth;
            gif.Height = gifData.logicalScreenHeight;
            gif.TextureList = new List<GifTexture>();

            _gifDict[id] = gif;

            //解析图片信息
            UniGifDecoder.DecodeTextureCoroutine(gifData, gif.TextureList, FilterMode.Point, TextureWrapMode.Repeat);

            return gif.TextureList;
        }

        #endregion
    }
}
