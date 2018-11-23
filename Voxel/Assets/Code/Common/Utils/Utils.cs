using System;
using Color.Number.Grid;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using Color.Number.Scene;

namespace Color.Number.Utils
{

    public class GameUtils
    {

        #region 快速排序

        /// <summary>
        /// 快速排序（从小到大顺序）
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        public static void QuickSort(ref PosColorInfo[] arr, int startIndex, int endIndex)
        {
            //基数
            var currentIndex = startIndex;
            //顺序查找
            var isOrderSearched = true;
            //反序查找
            var isDisOrderSearched = true;

            while (isOrderSearched || isDisOrderSearched)
            {
                isDisOrderSearched = false;
                isOrderSearched = false;
                for (var i = endIndex; i > currentIndex; i--)
                {
                    if (arr[i] == null
                        || arr[i].PosColorDic.Count == 0)
                        continue;

                    if (arr[i].PosColorDic.Count < arr[currentIndex].PosColorDic.Count)
                    {
                        //ExChangeValue(ref arr[i], ref arr[current_index]);
                        var temp = arr[i];
                        arr[i] = arr[currentIndex];
                        arr[currentIndex] = temp;

                        currentIndex = i;
                        isDisOrderSearched = true;
                        break;
                    }
                }

                for (var i = startIndex; i < currentIndex; i++)
                {
                    if (arr[currentIndex] == null
                        || arr[currentIndex].PosColorDic.Count == 0)
                        continue;

                    if (arr[i].PosColorDic.Count > arr[currentIndex].PosColorDic.Count)
                    {
                        //ExChangeValue(ref arr[i], ref arr[current_index]);
                        var temp = arr[i];
                        arr[i] = arr[currentIndex];
                        arr[currentIndex] = temp;

                        currentIndex = i;
                        isOrderSearched = true;
                        break;
                    }
                }
            }

            if (endIndex - startIndex > 0)
            {
                if (currentIndex != startIndex)
                {
                    QuickSort(ref arr, startIndex, currentIndex - 1);
                }

                if (currentIndex != endIndex)
                {
                    QuickSort(ref arr, currentIndex + 1, endIndex);
                }
            }
        }

        #endregion

        #region string 和 bytes 互相转换

        /// <summary>
        /// byte[] 转换为string
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static string BytesToString(byte[] b)
        {
            if (b == null || b.Length == 0)
                return null;

            return Encoding.UTF8.GetString(b);
        }

        /// <summary>
        /// string 转换成 byte[]
        /// </summary>
        /// <param name="str">需要转换的string数据</param>
        /// <returns></returns>
        public static byte[] StringToBytes(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            return Encoding.UTF8.GetBytes(str);
        }

        #endregion

        #region 字符串对比

        /// <summary>
        /// 获得去除后缀的路径
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetFullPathWithoutExtension(string url)
        {
            if (string.IsNullOrEmpty(url))
                return url;

            int length;
            if ((length = url.LastIndexOf('.')) == -1)
            {
                return url;
            }
            return url.Substring(0, length);
        }

        #endregion

        #region 链表数据操作

        /// <summary>
        /// 检测链表是否所有数据都为null,并且把为null的数据移除掉
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool IsHaveNoValue(List<UnityEngine.Object> list)
        {
            if (list == null || list.Count == 0)
                return true;

            for (int i = 0; i < list.Count;)
            {
                if (list[i] == null)
                    list.RemoveAt(i);
                else
                    i++;
            }

            if (list.Count == 0)
                return true;

            return false;
        }

        /// <summary>
        /// 删除列表里保存的所有数据
        /// </summary>
        /// <param name="list"></param>
        public static void RemoveAllFromList(List<UnityEngine.Object> list, bool allowDestroyingAssets = true)
        {
            if (list == null || list.Count == 0)
                return;

            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                if (!(list[i] is AssetBundle))
                    UnityEngine.GameObject.DestroyImmediate(list[i], allowDestroyingAssets);
            }
            list.Clear();
        }

        #endregion


        #region Texture 操作

        /// <summary>
        /// 生成一张图片的缩略图
        /// </summary>
        /// <param name="source"></param>
        /// <param name="targetWidth"></param>
        /// <param name="targetHeight"></param>
        /// <returns></returns>
        public static Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
        {
            if (source == null)
                return null;

            var tex = new Texture2D(targetWidth, targetHeight, TextureFormat.RGB24, false);

            UnityEngine.Color c;
            for (int i = 0; i < targetHeight; ++i)
            {
                for (int j = 0; j < targetWidth; ++j)
                {
                    c = source.GetPixelBilinear((float)j / targetWidth, (float)i / targetHeight);
                    tex.SetPixel(j, i, c);
                }
            }

            tex.Apply();

            return tex;
        }


        /// <summary>
        /// 获取马赛克图片
        /// </summary>
        /// <param name="tex"></param>
        /// <param name="mosaicSize"></param>
        /// <returns></returns>
        public static Texture2D GetMosaicTexture2D(UnityEngine.Texture tex, float mosaicSize)
        {
            if (tex != null && mosaicSize > 0)
            {
                var size = (int)(tex.width * mosaicSize);
                ////获取颜色数组
                //WebCamTexture texture2D = (WebCamTexture)tex;

                //获取颜色数组
                Texture2D texture2D = (Texture2D)tex;

                var width = tex.width;
                var height = tex.height;

                Texture2D t = new Texture2D(width, height, TextureFormat.RGB24, false);
                t.filterMode = FilterMode.Point;


                for (int heightOfffset = 0; heightOfffset < height; heightOfffset += size)
                {
                    for (int widthOffset = 0; widthOffset < width; widthOffset += size)
                    {
                        float avgR = 0, avgG = 0, avgB = 0;
                        int blurPixelCount = 0;

                        for (int x = widthOffset; x < widthOffset + size && x < width; x++)
                        {
                            for (int y = heightOfffset; y < heightOfffset + size && y < height; y++)
                            {
                                var color = texture2D.GetPixel(x, y);

                                avgR += color.r;
                                avgG += color.g;
                                avgB += color.b;

                                blurPixelCount++;
                            }

                        }

                        //计算范围平均值
                        avgR = avgR / blurPixelCount;
                        avgG = avgG / blurPixelCount;
                        avgB = avgB / blurPixelCount;

                        var c = new UnityEngine.Color(avgR, avgG, avgB);

                        for (int x = widthOffset; x < widthOffset + size && x < width; x++)
                        {
                            for (int y = heightOfffset; y < heightOfffset + size && y < height; y++)
                            {
                                t.SetPixel(x, y, c);
                            }

                        }
                    }
                }

                t.Apply();

                return t;
            }

            return null;
        }

        /// <summary>
        /// 获取马赛克图片
        /// </summary>
        /// <param name="tex"></param>
        /// <param name="mosaicSize"></param>
        /// <returns></returns>
        public static Texture2D GetMosaicTexture2D1(UnityEngine.Texture tex, float mosaicSize)
        {
            if (tex != null && mosaicSize > 0)
            {
                var size = (int)(tex.width * mosaicSize);
                //获取颜色数组
                WebCamTexture texture2D = (WebCamTexture)tex;

                var width = tex.width;
                var height = tex.height;

                Debug.LogError(" size " + size + "  " + mosaicSize + " sss " + texture2D.width + "   " + tex.width + " aaa " + texture2D.height + "  " + tex.height
                               + " dasda " + texture2D.requestedWidth + "   " + texture2D.requestedHeight);

                //Dictionary<UnityEngine.Color>

                List<UnityEngine.Color> list = new List<UnityEngine.Color>();

                Texture2D t = new Texture2D(width, height, TextureFormat.ARGB32, false);
                t.filterMode = FilterMode.Point;


                //for (int heightOfffset = 0; heightOfffset < height; heightOfffset += size)
                //{
                //    for (int widthOffset = 0; widthOffset < width; widthOffset += size)
                //    {

                //        //获取颜色
                //        var c = texture2D.GetPixel(widthOffset, heightOfffset);

                //        list.Add(c);

                //        for (int x = widthOffset; x < widthOffset + size && x < width; x++)
                //        {
                //            for (int y = heightOfffset; y < heightOfffset + size && y < height; y++)
                //            {
                //                t.SetPixel(x, y, c);
                //            }
                //        }
                //    }
                //}

                //t.Apply();


                for (int heightOfffset = 0; heightOfffset < height; heightOfffset += size)
                {
                    for (int widthOffset = 0; widthOffset < width; widthOffset += size)
                    {

                        //获取颜色
                        var c = texture2D.GetPixel(widthOffset, heightOfffset);

                        list.Add(c);
                    }
                }

                float colorDis = 10;

                for (int i = 0; i < list.Count; i++)
                {
                    
                }



                return t;
            }

            return null;
        }

        /// <summary>
        /// 获取马赛克图片
        /// </summary>
        /// <param name="tex"></param>
        /// <param name="mosaicSize"></param>
        /// <returns></returns>
        public static Texture2D GetMosaicTexture2D2(UnityEngine.Texture tex, float mosaicSize, int maxCount, int delta = 24)
        {
            if (tex != null && mosaicSize > 0)
            {
                var size = (int)(tex.width * mosaicSize);
                if (size < 1)
                    size = 1;

                Debug.LogError(size + "    123456   " + mosaicSize);

                //获取颜色数组
                Texture2D texture2D = (Texture2D)tex;

                //WebCamTexture texture2D = (WebCamTexture)tex;

                var width = tex.width;
                var height = tex.height;

                //Debug.LogError(" size " + size + "  " + mosaicSize + " sss " + texture2D.width + "   " + tex.width + " aaa " + texture2D.height + "  " + tex.height
                //               + " dasda " + texture2D.requestedWidth + "   " + texture2D.requestedHeight);

                //Dictionary<UnityEngine.Color>

                var list = new List<UnityEngine.Color>();

                var t = new Texture2D(width, height, TextureFormat.RGB24, false);
                t.filterMode = FilterMode.Point;


                //for (int heightOfffset = 0; heightOfffset < height; heightOfffset += size)
                //{
                //    for (int widthOffset = 0; widthOffset < width; widthOffset += size)
                //    {

                //        //获取颜色
                //        var c = texture2D.GetPixel(widthOffset, heightOfffset);

                //        list.Add(c);

                //        for (int x = widthOffset; x < widthOffset + size && x < width; x++)
                //        {
                //            for (int y = heightOfffset; y < heightOfffset + size && y < height; y++)
                //            {
                //                t.SetPixel(x, y, c);
                //            }
                //        }
                //    }
                //}

                //t.Apply();

                var colorMap = new int[256];
                var halfDelta = 0;
                if (delta > 2)
                    halfDelta = delta / 2 - 1;

                //初始化color map 信息
                for (int i = 0; i < 256; i++)
                {
                    colorMap[i] = ((i + halfDelta) / delta) * delta;
                    if (colorMap[i] > 255)
                        colorMap[i] = 255;
                }

                int r, g, b = 0;

                //Dictionary<UnityEngine.Color, List<ColorInfo>> dic = new Dictionary<UnityEngine.Color, List<ColorInfo>>();

                Dictionary<Vector3, List<ColorInfo>> dic = new Dictionary<Vector3, List<ColorInfo>>();

                //Dictionary<UnityEngine.Color, int> dic = new Dictionary<UnityEngine.Color, int>();
                for (int heightOfffset = 0; heightOfffset < height; heightOfffset += size)
                {
                    for (int widthOffset = 0; widthOffset < width; widthOffset += size)
                    {

                        //获取颜色
                        var c = texture2D.GetPixel(widthOffset, heightOfffset);

                        r = colorMap[(int)(c.r * 255)];
                        g = colorMap[(int)(c.g * 255)];
                        b = colorMap[(int)(c.b * 255)];

                        //c = new UnityEngine.Color((float)r / 255, (float)g / 255, (float)b / 255);

                        Vector3 vec = new Vector3((float)r / 255, (float)g / 255, (float)b / 255);


                        //c = new UnityEngine.Color((float)((int)(c.r * 10)) / 10, (float)((int)(c.g * 10)) / 10, (float)((int)(c.b * 10)) / 10);

                        //if (dic.ContainsKey(c))
                        //    dic[c]++;
                        //else
                        //    dic[c] = 1;


                        if (dic.ContainsKey(vec))
                        {

                            dic[vec].Add(new ColorInfo(vec, widthOffset, heightOfffset));
                        }
                        else
                        {
                            List<ColorInfo> l = new List<ColorInfo>();
                            l.Add(new ColorInfo(vec, widthOffset, heightOfffset));
                            dic[vec] = l;
                        }
                    }
                }

                var ll = dic.Values.ToList();

                //对数据进行排序
                ll.Sort((left, right) =>
                {
                    if (left.Count > right.Count)
                        return -1;
                    else if (left.Count == right.Count)
                        return 0;
                    else
                        return 1;
                });
                //for (int i = 0; i < ll.Count; i++)
                //{
                //    var info = ll[i];

                //}

                //disList = disList.Distinct().ToList();
                //disList.Sort();

                //去除颜色数量较少的，用相近的颜色进行填位
                var v = Vector3.zero;
                var minDis = float.MaxValue;
                var minIndexId = 0;
                var d = 0f;

                while(ll.Count > maxCount)
                {
                    var indexId = ll.Count - 1;
                    v = ll[indexId][0].v;
                    minDis = float.MaxValue;
                    minIndexId = 0;
                    for (int j = 0; j < maxCount; j++)
                    {
                        d = Vector3.Distance(v, ll[j][0].v);
                        if (d < minDis)
                        {
                            minDis = d;
                            minIndexId = j;
                        }
                    }

                    ll[minIndexId].AddRange(ll[indexId]);
                    ll.RemoveAt(indexId);
                }

                //创建颜色数组
                var count = ll.Count;
                var arr = new UnityEngine.Color[width * height];
                for (int i = 0; i < count; i++)
                {
                    var l = ll[i];
                    var num = l.Count;
                    var c = GetColorFromVector3(l[0].v);
                    for (int j = 0; j < num; j++)
                    {

                        //        for (int x = widthOffset; x < widthOffset + size && x < width; x++)
                        //        {
                        //            for (int y = heightOfffset; y < heightOfffset + size && y < height; y++)
                        //            {
                        //                t.SetPixel(x, y, c);
                        //            }
                        //        }

                        var w1 = l[j].x;
                        var h1 = l[j].y;
                        for (int x = w1; x < w1 + size && x < width; x++)
                        {
                            for (int y = h1; y < h1 + size && y < height; y++)
                            {
                                arr[x + y * width] = c;
                            }
                        }
                    }
                }

                t.SetPixels(arr);
                t.Apply();
                return t;
            }

            return null;
        }


        public class ColorInfo
        {
            public UnityEngine.Color c;

            public Vector3 v;

            public int x;
            public int y;

            public ColorInfo(UnityEngine.Color c, int x, int y)
            {
                this.c = c;
                this.x = x;
                this.y = y;
            }

            public ColorInfo(UnityEngine.Vector3 v, int x, int y)
            {
                this.v = v;
                this.x = x;
                this.y = y;
            }


        }


        /// <summary>
        /// 获取马赛克图片
        /// </summary>
        /// <param name="tex"></param>
        /// <param name="mosaicSize"></param>
        /// <returns></returns>
        public static Texture2D GetMosaicTexture2D3(UnityEngine.Texture tex, float mosaicSize, int delta = 24)
        {
            if (tex != null && mosaicSize > 0)
            {
                var size = (int)(tex.width * mosaicSize);
                //获取颜色数组
                Texture2D texture2D = (Texture2D)tex;

                var width = tex.width;
                var height = tex.height;

                //Debug.LogError(" size " + size + "  " + mosaicSize + " sss " + texture2D.width + "   " + tex.width + " aaa " + texture2D.height + "  " + tex.height
                //               + " dasda " + texture2D.requestedWidth + "   " + texture2D.requestedHeight);

                //Dictionary<UnityEngine.Color>

                List<UnityEngine.Color> list = new List<UnityEngine.Color>();

                Texture2D t = new Texture2D(width, height, TextureFormat.RGB24, false);
                t.filterMode = FilterMode.Point;


                //for (int heightOfffset = 0; heightOfffset < height; heightOfffset += size)
                //{
                //    for (int widthOffset = 0; widthOffset < width; widthOffset += size)
                //    {

                //        //获取颜色
                //        var c = texture2D.GetPixel(widthOffset, heightOfffset);

                //        list.Add(c);

                //        for (int x = widthOffset; x < widthOffset + size && x < width; x++)
                //        {
                //            for (int y = heightOfffset; y < heightOfffset + size && y < height; y++)
                //            {
                //                t.SetPixel(x, y, c);
                //            }
                //        }
                //    }
                //}

                //t.Apply();

                var colorMap = new int[256];
                var halfDelta = 0;
                if (delta > 2)
                    halfDelta = delta / 2 - 1;

                //初始化color map 信息
                for (int i = 0; i < 256; i++)
                {
                    colorMap[i] = ((i + halfDelta) / delta) * delta;
                    if (colorMap[i] > 255)
                        colorMap[i] = 255;
                }

                int r, g, b = 0;

                Dictionary<UnityEngine.Color, int> dic = new Dictionary<UnityEngine.Color, int>();

                List<float> disList = new List<float>();

                for (int heightOfffset = 0; heightOfffset < height; heightOfffset += size)
                {
                    for (int widthOffset = 0; widthOffset < width; widthOffset += size)
                    {

                        //获取颜色
                        var c = texture2D.GetPixel(widthOffset, heightOfffset);

                        if (dic.ContainsKey(c))
                        {
                            dic[c]++;
                        }
                        else
                        {
                            bool isNew = true;
                            UnityEngine.Color c1 = UnityEngine.Color.white;
                            //foreach (var item in dic)
                            //{
                            //    c1 = item.Key;

                            //    float f = LABColorDis(c1, c);
                            //    disList.Add(f);
                            //    if (f <= 0.005f)
                            //    {
                            //        isNew = false;
                            //        c = c1;
                            //        break;
                            //    }
                            //}

                            if (isNew)
                                dic[c] = 1;
                            else
                                dic[c1]++;

                            
                        }


                        for (int x = widthOffset; x < widthOffset + size && x < width; x++)
                        {
                            for (int y = heightOfffset; y < heightOfffset + size && y < height; y++)
                            {
                                t.SetPixel(x, y, c);
                            }
                        }
                    }
                }

                UnityEngine.Debug.LogError(" dic  " + dic.Count);

                t.Apply();
                return t;
            }

            return null;
        }

        #endregion


        #region Color

        /// <summary>
        /// LAB 颜色对比
        /// https://blog.csdn.net/qq_16564093/article/details/80698479?utm_source=blogxgwz2
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public static float LABColorDis(UnityEngine.Color c1, UnityEngine.Color c2)
        {
            var rmean = (c1.r + c2.r) / 2;
            var r = c1.r - c2.r;
            var g = c1.g - c2.g;
            var b = c1.b - c2.b;

            //return Mathf.Sqrt(((2 + rmean / 256) * r * r) + 4 * g * g + (2 + (255-rmean) / 256) * b * b );
            return ((2 + rmean / 256) * r * r) + 4 * g * g + (2 + (255 - rmean) / 256) * b * b;

        }

        /// <summary>
        /// Vector3 转换成 Color
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static UnityEngine.Color GetColorFromVector3(Vector3 v)
        {
            return new UnityEngine.Color(v.x, v.y, v.z);
        }

        #endregion


        #region 拍照

        /// <summary>
        /// 获取相机拍照存储图片大小
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static int GetCameraPhotoTextureSize(float f)
        {
            var min = ConstantConfig.GetGameConfigInt(GameConfigKey.camera_save_texture_min_width);
            var max = ConstantConfig.GetGameConfigInt(GameConfigKey.camera_save_texture_max_width);
            var baseNum = ConstantConfig.GetGameConfigInt(GameConfigKey.camera_save_texture_width_base);
            if (baseNum <= 0)
                baseNum = 1;

            var curr = (int)(f * (max - min)) + min;

            var remainder = curr % baseNum;

            if (remainder != 0)
                curr -= remainder;

            return curr;
        }

        /// <summary>
        /// 获取相机拍照存储图片颜色最大个数
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static int GetCameraPhotoTextureMaxColorNum(float f)
        {
            var min = ConstantConfig.GetGameConfigInt(GameConfigKey.camera_svae_texture_color_min_num);
            var max = ConstantConfig.GetGameConfigInt(GameConfigKey.camera_svae_texture_color_max_num);

            var curr = (int)f * (max - min) + min;

            return curr;
        }

        #endregion

        #region 时间

        /// <summary>
        /// 获取当前年月日时分秒，如201803081916
        /// </summary>
        /// <returns></returns>
        public static string GetCurTime()
        {
            var now = DateTime.Now;

            return string.Format("{0}{1}{2}{3}{4}{5}", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
        }

        #endregion

        #region Voxel 操作

        #region --- File ---



        public static string Load(string _path)
        {
            try
            {
                StreamReader _sr = System.IO.File.OpenText(_path);
                string _data = _sr.ReadToEnd();
                _sr.Close();
                return _data;
            }
            catch (System.Exception)
            {
                return "";
            }
        }



        public static void Save(string _data, string _path)
        {
            try
            {
                FileStream fs = new FileStream(_path, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
                sw.Write(_data);
                sw.Close();
                fs.Close();
            }
            catch (System.Exception)
            {
                return;
            }
        }



        public static byte[] FileToByte(string path)
        {
            if (System.IO.File.Exists(path))
            {
                byte[] bytes = null;
                try
                {
                    bytes = System.IO.File.ReadAllBytes(path);
                }
                catch
                {
                    return null;
                }
                return bytes;
            }
            else
            {
                return null;
            }
        }



        public static bool ByteToFile(byte[] bytes, string path)
        {
            try
            {
                string parentPath = new FileInfo(path).Directory.FullName;
                CreateFolder(parentPath);
                FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();
                fs.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }



        public static void CreateFolder(string _path)
        {
            _path = GetFullPath(_path);
            if (Directory.Exists(_path))
                return;
            string _parentPath = new FileInfo(_path).Directory.FullName;
            if (Directory.Exists(_parentPath))
            {
                Directory.CreateDirectory(_path);
            }
            else
            {
                CreateFolder(_parentPath);
                Directory.CreateDirectory(_path);
            }
        }



        #endregion



        #region --- Path ---



        public static string FixPath(string _path)
        {
            _path = _path.Replace('\\', '/');
            _path = _path.Replace("//", "/");
            while (_path.Length > 0 && _path[0] == '/')
            {
                _path = _path.Remove(0, 1);
            }
            return _path;
        }



        public static string GetFullPath(string path)
        {
            return new FileInfo(path).FullName;
        }



        public static string RelativePath(string path)
        {
            path = FixPath(path);
            if (path.StartsWith("Assets"))
            {
                return path;
            }
            if (path.StartsWith(FixPath(Application.dataPath)))
            {
                return "Assets" + path.Substring(FixPath(Application.dataPath).Length);
            }
            else
            {
                return "";
            }
        }



        public static string CombinePaths(params string[] paths)
        {
            string path = "";
            for (int i = 0; i < paths.Length; i++)
            {
                path = Path.Combine(path, FixPath(paths[i]));
            }
            return FixPath(path);
        }



        public static string GetExtension(string path)
        {
            return Path.GetExtension(path);
        }



        public static string GetName(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }



        public static string ChangeExtension(string path, string newEx)
        {
            return Path.ChangeExtension(path, newEx);
        }



        public static bool PathIsDirectory(string path)
        {
            FileAttributes attr = System.IO.File.GetAttributes(path);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                return true;
            else
                return false;
        }



        #endregion



        #region --- Watch ---


        private static System.Diagnostics.Stopwatch TheWatch;


        public static void StartWatch()
        {
            TheWatch = new System.Diagnostics.Stopwatch();
            TheWatch.Start();
        }


        public static void PauseWatch()
        {
            if (TheWatch != null)
            {
                TheWatch.Stop();
            }
        }


        public static void RestartWatch()
        {
            if (TheWatch != null)
            {
                TheWatch.Start();
            }
        }


        public static double StopWatchAndGetTime()
        {
            if (TheWatch != null)
            {
                TheWatch.Stop();
                return TheWatch.Elapsed.TotalSeconds;
            }
            return 0f;
        }


        #endregion

        #endregion


        #region video

        /// <summary>
        /// 路径信息转换
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string AddQuotation(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new Exception("Empty path.");
            }
#if UNITY_EDITOR || UNITY_STANDALONE
            const char DOUBLE_QUOTATION = '\"';
            if (path[0] != DOUBLE_QUOTATION)
            {
                return DOUBLE_QUOTATION + path + DOUBLE_QUOTATION;
            }
#endif
            return path;
        }

        #endregion


        #region 截屏

        /// <summary>
        /// 获取截屏 H 方向上的 偏移操作
        /// </summary>
        /// <returns></returns>
        public static float GetScreenshotOffsetH()
        {
            if (SceneManager.instance.IsVoxel)
                return ConstantConfig.GetGameConfigFloat(GameConfigKey.capture_screen_shot_offset_h);
            return 0;
        }

        #endregion
    }

}
