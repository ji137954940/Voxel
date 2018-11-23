using System.Collections;
using System.Collections.Generic;
using Color.Number.File;
using Color.Number.Grid;
using Color.Number.Voxel;
using Tgame.Game.Table;
using UnityEngine;
using ZLib;

namespace Color.Number.GameInfo
{

    /// <summary>
    /// 角色信息数据
    /// </summary>
    public class PlayerGameInfo : Singleton<PlayerGameInfo>
    {

        #region 输入数据

        private bool _isAcceptInputOperation;

        /// <summary>
        /// 是否接受输入操作
        /// </summary>
        public bool IsAcceptInputOperation
        {
            get { return _isAcceptInputOperation; }
            set { _isAcceptInputOperation = value; }
        }

        #endregion

        #region 还原填充过程

        private bool _isPixelColoring;

        /// <summary>
        /// 是否正在像素还原过程
        /// 在此过程中，禁止操作
        /// </summary>
        public bool IsPixelColoring
        {
            get { return _isPixelColoring; }
            set { _isPixelColoring = value; }
        }

        #endregion

        #region 选择的数据

        private int _selectColorId = 1;

        /// <summary>
        /// 当前选中的颜色Id
        /// </summary>
        public int SelectColorId
        {
            get { return _selectColorId; }
            set { _preSelectColorId = _selectColorId; _selectColorId = value; }
        }

        private int _preSelectColorId = 1;
        /// <summary>
        /// 之前选中的颜色Id
        /// </summary>
        public int PreSelectColorId
        {
            get { return _preSelectColorId; }
        }

        private UnityEngine.Color _selectColor;

        /// <summary>
        /// 当前选中的颜色色值
        /// </summary>
        public UnityEngine.Color SelectColor
        {
            get { return _selectColor; }
            set { _selectColor = value; }
        }

        /// <summary>
        /// 重置颜色选择设置
        /// </summary>
        public void ResetSelectInfo()
        {
            _preSelectColorId = _selectColorId = 1;
            _selectColor = UnityEngine.Color.white;
        }

        #endregion

        #region 保存修改的 texture

        /// <summary>
        /// 存储生成的 texture
        /// </summary>
        private Dictionary<int, Dictionary<int, Texture2D>> _saveTextureDic;

        /// <summary>
        /// 存储保存，修改的图片
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="id"></param>
        /// <param name="tex"></param>
        public void SetSaveTexture(int typeId, int id, Texture2D tex)
        {
            if(_saveTextureDic == null)
                _saveTextureDic = new Dictionary<int, Dictionary<int, Texture2D>>();

            Dictionary<int, Texture2D> dic = null;
            if (_saveTextureDic.TryGetValue(typeId, out dic))
            {
                dic[id] = tex;
            }
            else
            {
                dic = new Dictionary<int, Texture2D>();
                dic[id] = tex;

                _saveTextureDic[typeId] = dic;
            }
        }

        /// <summary>
        /// 获取生成的图片
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public Texture2D GetSaveTexture(int typeId, int id)
        {
            if (_saveTextureDic == null
                || _saveTextureDic.Count == 0)
                return null;

            Dictionary<int, Texture2D> dic = null;
            if (_saveTextureDic.TryGetValue(typeId, out dic))
            {
                Texture2D tex = null;
                dic.TryGetValue(id, out tex);
                return tex;
            }
            else
            {
                return null;
            }
            
        }

        #endregion

        #region 存储当前已经完成的 文件id

        /// <summary>
        /// 存储已经完成的信息数据 <typeId, <textureId>>
        /// </summary>
        private Dictionary<int, HashSet<int>> _completeDic;

        /// <summary>
        /// 设置已经完成的信息数据
        /// </summary>
        /// <param name="hash"></param>
        public void SetCompleteInfo(Dictionary<int, HashSet<int>> hash)
        {
            _completeDic = hash;
        }

        /// <summary>
        /// 获取完成信息数据
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, HashSet<int>> GetCompleteInfo()
        {
            return _completeDic;
        }

        /// <summary>
        /// 添加完成信息数据
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="id"></param>
        public void AddCompleteInfo(int typeId, int id)
        {
            if(_completeDic == null)
                _completeDic = new Dictionary<int, HashSet<int>>();

            HashSet<int> hash = null;
            if (_completeDic.TryGetValue(typeId, out hash))
            {
                hash.Add(id);
            }
            else
            {
                hash = new HashSet<int>();
                hash.Add(id);

                _completeDic[typeId] = hash;
            }
        }

        /// <summary>
        /// 检测是否为已经完成的信息数据
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsCompleteInfo(int typeId, int id)
        {
            if (_completeDic == null
                || _completeDic.Count == 0)
                return false;

            HashSet<int> hash = null;
            if (_completeDic.TryGetValue(typeId, out hash))
            {
                return hash.Contains(id);
            }

            return false;
        }

        #endregion

        #region 存储拍照信息数据

        private List<CameraPhotoInfo> _cameraPhotoInfosList;

        public List<CameraPhotoInfo> GetCameraPhotoInfos()
        {
            return _cameraPhotoInfosList;
        }

        /// <summary>
        /// 设置相机拍照信息数据
        /// </summary>
        /// <param name="arr"></param>
        public void SetCameraPhotoInfo(CameraPhotoInfo[] arr)
        {
			if (arr == null)
				return;

            if(_cameraPhotoInfosList == null)
                _cameraPhotoInfosList = new List<CameraPhotoInfo>();
			
            _cameraPhotoInfosList.AddRange(arr);

        }

        /// <summary>
        /// 添加相机拍照信息
        /// </summary>
        /// <param name="info"></param>
        public void AddCameraPhotoInfo(CameraPhotoInfo info)
        {
            if(_cameraPhotoInfosList == null)
                _cameraPhotoInfosList = new List<CameraPhotoInfo>();

            _cameraPhotoInfosList.Add(info);
        }

        /// <summary>
        /// 移除一条相机拍照信息
        /// </summary>
        /// <param name="id"></param>
        public void RemoveCameraPhotoInfo(int id)
        {
            if(_cameraPhotoInfosList == null)
                return;

            for (int i = 0; i < _cameraPhotoInfosList.Count; i++)
            {
                if (_cameraPhotoInfosList[i] != null
                    && _cameraPhotoInfosList[i].id == id)
                {
                    _cameraPhotoInfosList.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary>
        /// 获取新的相机拍照信息id
        /// </summary>
        /// <returns></returns>
        public int GetNewCameraPhotoInfoId()
        {
            if (_cameraPhotoInfosList == null)
                return 1;

            return _cameraPhotoInfosList[_cameraPhotoInfosList.Count - 1].id + 1;
        }

        #endregion

        #region 所有的icon信息

        private List<Table_Client_Icon> _iconList;

        /// <summary>
        /// Icon List
        /// </summary>
        public List<Table_Client_Icon> IconList
        {
            get { return _iconList; }
            set { _iconList = value; }
        }

        #endregion

        #region 数据清理

        /// <summary>
        /// 数据清理
        /// </summary>
        public void Clear()
        {
            _isAcceptInputOperation = false;
            _isPixelColoring = false;

            _preSelectColorId = 1;
            _selectColorId = 1;
            _selectColor = UnityEngine.Color.white;

            if (_saveTextureDic != null)
            {
                _saveTextureDic.Clear();
                _saveTextureDic = null;
            }

            if (_completeDic != null)
            {
                _completeDic.Clear();
                _completeDic = null;
            }

            if (_iconList != null)
            {
                _iconList.Clear();
                _iconList = null;
            }
        }

        #endregion
    }
}


