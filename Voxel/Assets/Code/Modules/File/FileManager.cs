using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Color.Number.Grid;
using UnityEngine;
using ZLib;
using System.IO;
using System.Linq;
using System.Text;
using Color.Number.Camera;
using Color.Number.GameInfo;
using Color.Number.Utils;
using Color.Number.Voxel;
using Color = UnityEngine.Color;

namespace Color.Number.File
{

    /// <summary>
    /// 对文件信息进行操作
    /// </summary>
    public class FileManager : Singleton<FileManager>
    {

        #region 读取存储文件

        /// <summary>
        /// 存储完成信息的文件名字
        /// </summary>
        private string _fileCompleteName = "Complete";

        /// <summary>
        /// 存储 相机拍照信息文件
        /// </summary>
        private string _fileCameraPhotoName = "CameraPhoto";

        /// <summary>
        /// 读取存储的颜色信息数据
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="id"></param>
        /// <param name="arr"></param>
        /// <param name="tex"></param>
        public void ReadColorInfo(int typeId, int id, out int[] arr, out int[] colorCompleteAreaIdArr, out UnityEngine.Color[] originalColorTypeArr, out Texture2D tex)
        {
            arr = ReadFileInfo(typeId, id, out colorCompleteAreaIdArr, out originalColorTypeArr);
            tex = ReadImageInfo(typeId, id);
        }

        /// <summary>
        /// 存储文件信息数据
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="id"></param>
        public int[] ReadFileInfo(int typeId, int id, out int[] colorCompleteAreaIdArr, out UnityEngine.Color[] originalColorTypeArr)
        {
            colorCompleteAreaIdArr = null;
            originalColorTypeArr = null;

            var path = string.Format("{0}/{1}/{2}/{3}/{4}{5}",
                    ConstantConfig.CACHE_PATH,
                    ConstantConfig.GROUP_SETUP,
                    _fileDirectory,
                    typeId,
                    id,
                    ConstantConfig.CONFIG_EXTENSION);

            if (System.IO.File.Exists(path))
            {
                //如果存在，那么读取文件

                byte[] data = null;
                using (Stream s = System.IO.File.OpenRead(path))
                {
                    //读取数据信息
                    data = new byte[s.Length];
                    s.Read(data, 0, (int)s.Length);
                    s.Dispose();
                }

                if (data == null || data.Length == 0)
                    return null;

                var str = GameUtils.BytesToString(data);

                var info = JsonUtility.FromJson(str, typeof(FileInfo)) as FileInfo;
                if (info == null)
                    return null;

                colorCompleteAreaIdArr = info.ColorCompleteAreaIdArr;
                originalColorTypeArr = info.OriginalColorTypeArr;
                return info.ColorOrderArr;
            }

            return null;
        }

        /// <summary>
        /// 存储图片信息
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public Texture2D ReadImageInfo(int typeId, int id)
        {
            var path = string.Format("{0}/{1}/{2}/{3}/{4}.png",
                ConstantConfig.CACHE_PATH,
                ConstantConfig.GROUP_SETUP,
                _imageDirectory,
                typeId,
                id);

            if (System.IO.File.Exists(path))
            {
                //如果存在，那么读取文件

                byte[] data = null;
                using (Stream s = System.IO.File.OpenRead(path))
                {
                    //读取数据信息
                    data = new byte[s.Length];
                    s.Read(data, 0, (int)s.Length);
                    s.Dispose();
                }

                if (data == null || data.Length == 0)
                    return null;

                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(data);
                tex.filterMode = FilterMode.Point;

                PlayerGameInfo.instance.SetSaveTexture(typeId, id, tex);

                return tex;
            }
            else
            {
                //如果不存在，直接返回

                return null;
            }

        }

        /// <summary>
        /// 获取完成信息数据
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, HashSet<int>> GetCompleteInfo()
        {

            var path = string.Format("{0}/{1}/{2}/{3}{4}",
                ConstantConfig.CACHE_PATH,
                ConstantConfig.GROUP_SETUP,
                _fileDirectory,
                _fileCompleteName,
                ConstantConfig.CONFIG_EXTENSION);

            //判断文件是否存在，如果存在那么删除（因为有可能为老版本文件）
            if (System.IO.File.Exists(path))
            {
                //文件存在，那么读取文件信息

                byte[] data = null;
                using (Stream s = System.IO.File.OpenRead(path))
                {
                    //读取数据信息
                    data = new byte[s.Length];
                    s.Read(data, 0, (int)s.Length);
                    s.Dispose();
                }

                if (data == null || data.Length == 0)
                    return null;

                var dic = new Dictionary<int, HashSet<int>>();

                var str = GameUtils.BytesToString(data);

                var arr = str.Split('\n');
                var count = arr.Length;
                for (int i = 0; i < count; i++)
                {
                    if (string.IsNullOrEmpty(arr[i]))
                        continue;

                    var info = JsonUtility.FromJson(arr[i], typeof(CompleteInfo)) as CompleteInfo;
                    if (info == null
                        || info.completeArr == null
                        || info.completeArr.Length == 0)
                        continue;

                    dic[info.typeId] = new HashSet<int>(info.completeArr);
                }

                return dic;
            }

            return null;
        }


        /// <summary>
        /// 获取相机拍照信息数据
        /// </summary>
        /// <returns></returns>
        public CameraPhotoInfo[] GetCameraPhotoInfo()
        {

            var path = string.Format("{0}/{1}/{2}/{3}{4}",
                ConstantConfig.CACHE_PATH,
                ConstantConfig.GROUP_SETUP,
                _fileDirectory,
                _fileCameraPhotoName,
                ConstantConfig.CONFIG_EXTENSION);

            //判断文件是否存在，如果存在那么删除（因为有可能为老版本文件）
            if (System.IO.File.Exists(path))
            {
                //文件存在，那么读取文件信息

                byte[] data = null;
                using (Stream s = System.IO.File.OpenRead(path))
                {
                    //读取数据信息
                    data = new byte[s.Length];
                    s.Read(data, 0, (int)s.Length);
                    s.Dispose();
                }

                if (data == null || data.Length == 0)
                    return null;

                var str = GameUtils.BytesToString(data);

                var arr = str.Split('\n');
                var count = arr.Length;

				var list = new List<CameraPhotoInfo> ();

                for (int i = 0; i < count; i++)
                {
                    if (string.IsNullOrEmpty(arr[i]))
                        continue;

                    var info = JsonUtility.FromJson(arr[i], typeof(CameraPhotoInfo)) as CameraPhotoInfo;
                    if (info == null)
                        continue;

					list.Add(info);
                }

				return list.ToArray();
            }

            return null;
        }

        #endregion

        #region 写入文件信息

        /// <summary>
        /// 存储文件目录
        /// </summary>
        public static string _fileDirectory = "file";

        /// <summary>
        /// 存储图片目录
        /// </summary>
        public static string _imageDirectory = "image";

        /// <summary>
        /// 存储文件信息
        /// </summary>
        /// <param name="gridInfo"></param>
        /// <returns></returns>
        public bool SaveColorInfo(GridInfo gridInfo)
        {
            if (gridInfo == null)
            {
                Debug.LogError(" 文件信息存储失败，_gridInfo 为 null ");
                return false;
            }

            if (gridInfo.PixelColorCompleteOrder == null
                || gridInfo.PixelColorCompleteOrder.Count == 0)
            {
                Debug.Log(" 当前没有修改数据，不需要存储相应信息 ");
                return false;
            }

            //已经完成，那么存储完成标识
            if(gridInfo.IsTextureColoringComplete)
                SaveCompleteInfo(PlayerGameInfo.instance.GetCompleteInfo());

            //存储文件信息数据
            SaveFileInfo(gridInfo);

            //存储图片信息数据
            SaveImageInfo(gridInfo);

            return true;
        }

		/// <summary>
		/// 存储文件信息
		/// </summary>
		/// <param name="gridInfo"></param>
		/// <returns></returns>
		public bool SaveColorInfo(VoxelInfo voxelInfo)
		{
			if (voxelInfo == null)
			{
				Debug.LogError(" 文件信息存储失败，voxelInfo 为 null ");
				return false;
			}

			if (voxelInfo.VoxelColorCompleteOrder == null
				|| voxelInfo.VoxelColorCompleteOrder.Count == 0)
			{
				Debug.Log(" 当前没有修改数据，不需要存储相应信息 ");
				return false;
			}

			//已经完成，那么存储完成标识
			if(voxelInfo.IsVoxelColoringComplete)
				SaveCompleteInfo(PlayerGameInfo.instance.GetCompleteInfo());

			//存储文件信息数据
			SaveFileInfo(voxelInfo);

			//存储图片信息数据
			SaveImageInfo(voxelInfo);

			return true;
		}

        /// <summary>
        /// 存储相机拍照信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="tex"></param>
        public void SaveCameraPhotoInfo(CameraPhotoInfo info, Texture2D tex)
        {
            if (info == null)
            {
                Debug.LogError(" 文件信息存储失败，相机拍照信息数据 为 null ");
                return;
            }

            //存储文件
            SaveCameraPhotoInfo(PlayerGameInfo.instance.GetCameraPhotoInfos());

            //存储图片
            SaveImageInfo(info, tex);
        }

        /// <summary>
        /// 存储文件信息数据
        /// </summary>
        /// <param name="gridInfo"></param>
        private void SaveFileInfo(GridInfo gridInfo)
        {
            try
            {
                var path = string.Format("{0}/{1}/{2}/{3}/{4}{5}",
                    ConstantConfig.CACHE_PATH,
                    ConstantConfig.GROUP_SETUP,
                    _fileDirectory,
                    gridInfo.TypeId,
                    gridInfo.Id,
                    ConstantConfig.CONFIG_EXTENSION);

                var fileInfo = new FileInfo();
                if(gridInfo.PixelColorCompleteAreaIdList != null)
                    fileInfo.ColorCompleteAreaIdArr = gridInfo.PixelColorCompleteAreaIdList.ToArray();
                fileInfo.ColorOrderArr = gridInfo.PixelColorCompleteOrder.ToArray();
                fileInfo.OriginalColorTypeArr = gridInfo.GetOriginalColorTypeArr();
				fileInfo.IsVoxel = false;

                //转换成 json 数据
                var str = JsonUtility.ToJson(fileInfo);

                var outpath = System.IO.Path.GetDirectoryName(path);
                //判断目录是否存在，如果不存在那么创建目录
                if (!Directory.Exists(outpath))
                    Directory.CreateDirectory(outpath);

                //判断文件是否存在，如果存在那么删除（因为有可能为老版本文件）
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);

                //写文件
                using (FileStream fs = System.IO.File.Create(path))
                {
                    var data = GameUtils.StringToBytes(str);
                    fs.Write(data, 0, data.Length);
                    fs.Dispose();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(" 存储文件失败！ typeId " + gridInfo.TypeId + " id " + gridInfo.Id + "\n" + e.ToString());

                if (e.Message.Contains("Disk full"))
                {
                    //内存不足
                    Debug.LogError(" 当前存储空间不足！ ");
                }
            }
        }

		/// <summary>
		/// 存储文件信息数据
		/// </summary>
		/// <param name="voxelInfo"></param>
		private void SaveFileInfo(VoxelInfo voxelInfo)
		{
			try
			{
				var path = string.Format("{0}/{1}/{2}/{3}/{4}{5}",
					ConstantConfig.CACHE_PATH,
					ConstantConfig.GROUP_SETUP,
					_fileDirectory,
					voxelInfo.TypeId,
					voxelInfo.Id,
					ConstantConfig.CONFIG_EXTENSION);

				var fileInfo = new FileInfo();
				if(voxelInfo.VoxelColorCompleteAreaIdList != null)
					fileInfo.ColorCompleteAreaIdArr = voxelInfo.VoxelColorCompleteAreaIdList.ToArray();
				fileInfo.ColorOrderArr = voxelInfo.VoxelColorCompleteOrder.ToArray();
				fileInfo.OriginalColorTypeArr = voxelInfo.GetOriginalColorTypeArr();
				fileInfo.IsVoxel = true;

				//转换成 json 数据
				var str = JsonUtility.ToJson(fileInfo);

				var outpath = System.IO.Path.GetDirectoryName(path);
				//判断目录是否存在，如果不存在那么创建目录
				if (!Directory.Exists(outpath))
					Directory.CreateDirectory(outpath);

				//判断文件是否存在，如果存在那么删除（因为有可能为老版本文件）
				if (System.IO.File.Exists(path))
					System.IO.File.Delete(path);

				//写文件
				using (FileStream fs = System.IO.File.Create(path))
				{
					var data = GameUtils.StringToBytes(str);
					fs.Write(data, 0, data.Length);
					fs.Dispose();
				}
			}
			catch (Exception e)
			{
				Debug.LogError(" 存储文件失败！ typeId " + voxelInfo.TypeId + " id " + voxelInfo.Id + "\n" + e.ToString());

				if (e.Message.Contains("Disk full"))
				{
					//内存不足
					Debug.LogError(" 当前存储空间不足！ ");
				}
			}
		}

        /// <summary>
        /// 存储图片信息
        /// </summary>
        /// <param name="gridInfo"></param>
        private void SaveImageInfo(GridInfo gridInfo)
        {
            var arr = gridInfo.GreyColorArr;
            var pixelComplete = gridInfo.PixelColorCompleteOrder;

            var count = pixelComplete.Count;
            for (int i = 0; i < count; i++)
            {
                arr[pixelComplete[i]] = gridInfo.GetOriginalColor(pixelComplete[i]);
            }

            Texture2D tex = new Texture2D(gridInfo.Width, gridInfo.Height, TextureFormat.ARGB32, false);
            tex.filterMode = FilterMode.Point;
            tex.SetPixels(arr);
            tex.Apply();

            //保存记录图片
            PlayerGameInfo.instance.SetSaveTexture(gridInfo.TypeId, gridInfo.Id, tex);

            try
            {
                var bytes = tex.EncodeToPNG();
                //var bytes = tex.GetRawTextureData();

                var path = string.Format("{0}/{1}/{2}/{3}/{4}.png",
                    ConstantConfig.CACHE_PATH,
                    ConstantConfig.GROUP_SETUP,
                    _imageDirectory,
                    gridInfo.TypeId,
                    gridInfo.Id);

                var outpath = System.IO.Path.GetDirectoryName(path);
                //判断目录是否存在，如果不存在那么创建目录
                if (!Directory.Exists(outpath))
                    Directory.CreateDirectory(outpath);

                System.IO.File.WriteAllBytes(path, bytes);
            }
            catch (Exception e)
            {
                Debug.LogError(" 存储图片失败！ typeId " + gridInfo.TypeId + " id " + gridInfo.Id + "\n" + e.ToString());

                if (e.Message.Contains("Disk full"))
                {
                    //内存不足
                    Debug.LogError(" 当前存储空间不足！ ");
                }
            }
        }

		/// <summary>
		/// 存储图片信息
		/// </summary>
		/// <param name="voxelInfo"></param>
		private void SaveImageInfo(VoxelInfo voxelInfo)
		{
			//var info = Vox25DSprite.CreateSprite (voxelInfo.GetVoxelData(), 0, new Vector3(0.5f, 0, 0.5f), 1, 0.5f, false, false);

//			var arr = voxelInfo.GreyColorArr;
//			var pixelComplete = voxelInfo.VoxelColorCompleteOrder;
//
//			var count = pixelComplete.Count;
//			for (int i = 0; i < count; i++)
//			{
//				arr[pixelComplete[i]] = voxelInfo.GetOriginalColor(pixelComplete[i]);
//			}

//			Texture2D tex = new Texture2D(gridInfo.Width, gridInfo.Height, TextureFormat.ARGB32, false);
//			tex.filterMode = FilterMode.Point;
//			tex.SetPixels(arr);
//			tex.Apply();

            //恢复成默认显示的 voxel
		    VoxelManager.instance.ChangeShowColorAlphaAndTexture(1);

            //获取截取的图片信息
            var tex = CameraManager.GetInst().CaptureScreenshot();

            //保存记录图片
            //PlayerGameInfo.instance.SetSaveTexture(voxelInfo.TypeId, voxelInfo.Id, info.Texture);
            PlayerGameInfo.instance.SetSaveTexture(voxelInfo.TypeId, voxelInfo.Id, tex);


            try
            {
                //var bytes = info.Texture.EncodeToPNG();
                //var bytes = tex.GetRawTextureData();

                var bytes = tex.EncodeToPNG();

                var path = string.Format("{0}/{1}/{2}/{3}/{4}.png",
					ConstantConfig.CACHE_PATH,
					ConstantConfig.GROUP_SETUP,
					_imageDirectory,
					voxelInfo.TypeId,
					voxelInfo.Id);

				var outpath = System.IO.Path.GetDirectoryName(path);
				//判断目录是否存在，如果不存在那么创建目录
				if (!Directory.Exists(outpath))
					Directory.CreateDirectory(outpath);

				System.IO.File.WriteAllBytes(path, bytes);
			}
			catch (Exception e)
			{
				Debug.LogError(" 存储图片失败！ typeId " + voxelInfo.TypeId + " id " + voxelInfo.Id + "\n" + e.ToString());

				if (e.Message.Contains("Disk full"))
				{
					//内存不足
					Debug.LogError(" 当前存储空间不足！ ");
				}
			}
		}

        /// <summary>
        /// 存储图片信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="tex"></param>
        private void SaveImageInfo(CameraPhotoInfo info, Texture2D tex)
        {

            //保存记录图片
            PlayerGameInfo.instance.SetSaveTexture(info.type_id, info.id, tex);

            try
            {
                var bytes = tex.EncodeToPNG();
                //var bytes = tex.GetRawTextureData();

                var path = string.Format("{0}/{1}/{2}",
                    ConstantConfig.CACHE_PATH,
                    ConstantConfig.GROUP_SETUP,
                    info.path);

                var outpath = System.IO.Path.GetDirectoryName(path);
                //判断目录是否存在，如果不存在那么创建目录
                if (!Directory.Exists(outpath))
                    Directory.CreateDirectory(outpath);

                System.IO.File.WriteAllBytes(path, bytes);
            }
            catch (Exception e)
            {
                Debug.LogError(" 存储图片失败！ typeId " + info.type_id + " id " + info.id + "\n" + e.ToString());

                if (e.Message.Contains("Disk full"))
                {
                    //内存不足
                    Debug.LogError(" 当前存储空间不足！ ");
                }
            }
        }

        /// <summary>
        /// 存储文件信息数据
        /// </summary>
        /// <param name="list"></param>
        private void SaveCameraPhotoInfo(List<CameraPhotoInfo> list)
        {
            if(list == null
               || list.Count == 0)
                return;

            try
            {
                var path = string.Format("{0}/{1}/{2}/{3}{4}",
                    ConstantConfig.CACHE_PATH,
                    ConstantConfig.GROUP_SETUP,
                    _fileDirectory,
                    _fileCameraPhotoName,
                    ConstantConfig.CONFIG_EXTENSION);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < list.Count; i++)
                {

                    if(list[i] == null)
                        continue;

                    //转换成 json 数据
                    var str = JsonUtility.ToJson(list[i]);

                    sb.Append(str);
                    sb.Append("\n");
                }

                var outpath = System.IO.Path.GetDirectoryName(path);
                //判断目录是否存在，如果不存在那么创建目录
                if (!Directory.Exists(outpath))
                    Directory.CreateDirectory(outpath);

                //判断文件是否存在，如果存在那么删除（因为有可能为老版本文件）
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);

                //写文件
                using (FileStream fs = System.IO.File.Create(path))
                {
                    var data = GameUtils.StringToBytes(sb.ToString());
                    fs.Write(data, 0, data.Length);
                    fs.Dispose();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(" 存储完成信息文件失败！ \n" + e.ToString());

                if (e.Message.Contains("Disk full"))
                {
                    //内存不足
                    Debug.LogError(" 当前存储空间不足！ ");
                }
            }
        }

        /// <summary>
        /// 存储完成的信息数据
        /// </summary>
        /// <param name="hash"></param>
        public void SaveCompleteInfo(Dictionary<int, HashSet<int>> dic)
        {
            if(dic == null
               || dic.Count == 0)
                return;

            try
            {
                var path = string.Format("{0}/{1}/{2}/{3}{4}",
                    ConstantConfig.CACHE_PATH,
                    ConstantConfig.GROUP_SETUP,
                    _fileDirectory,
                    _fileCompleteName,
                    ConstantConfig.CONFIG_EXTENSION);

                StringBuilder sb = new StringBuilder();
                foreach (var item in dic)
                {
                    var info = new CompleteInfo();
                    info.typeId = item.Key;
                    info.completeArr = item.Value.ToArray();

                    //转换成 json 数据
                    var str = JsonUtility.ToJson(info);

                    sb.Append(str);
                    sb.Append("\n");
                }

                var outpath = System.IO.Path.GetDirectoryName(path);
                //判断目录是否存在，如果不存在那么创建目录
                if (!Directory.Exists(outpath))
                    Directory.CreateDirectory(outpath);

                //判断文件是否存在，如果存在那么删除（因为有可能为老版本文件）
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);

                //写文件
                using (FileStream fs = System.IO.File.Create(path))
                {
                    var data = GameUtils.StringToBytes(sb.ToString());
                    fs.Write(data, 0, data.Length);
                    fs.Dispose();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(" 存储完成信息文件失败！ \n" + e.ToString());

                if (e.Message.Contains("Disk full"))
                {
                    //内存不足
                    Debug.LogError(" 当前存储空间不足！ ");
                }
            }
        }

        #endregion

    }
}


