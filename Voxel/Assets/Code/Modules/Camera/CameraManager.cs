using System;
using System.Collections;
using System.IO;
using Color.Number.GameInfo;
using Color.Number.Scene;
using Color.Number.Utils;
using UnityEngine;
using ZLib;

namespace Color.Number.Camera
{

    /// <summary>
    /// Camera 管理
    /// </summary>
    public class CameraManager : MonoBehaviour
    {

        /// <summary>
        /// 显示 tilemap 时 参数控制
        /// </summary>
        [System.Serializable]
        public class TilemapControl
        {
            /// <summary>
            /// 相机最小位置（缩放最大时, Z值为相应的最大值）
            /// </summary>
            public Vector3 MinPos;
            /// <summary>
            /// 相机最大位置（缩放最大时, Z值为相应的最大值）
            /// </summary>
            public Vector3 MaxPos;
            /// <summary>
            /// 相机默认位置(缩放最小时)
            /// </summary>
            public Vector3 DefultPos;
            /// <summary>
            /// 默认旋转角度
            /// </summary>
            public Vector3 DefulltRotation;

            /// <summary>
            /// 鼠标滚轮缩放倍数 （缩放时能有更好的体验）
            /// </summary>
            public float MouseScrollWheelRatio = 200;
        }

        /// <summary>
        /// 显示 voxel 时 参数控制
        /// </summary>
        [System.Serializable]
        public class VoxelControl
        {
            /// <summary>
            /// 目标对象
            /// </summary>
            public Transform target;

            /// <summary>
            /// 相机最小位置（缩放最大时, Z值为相应的最大值）
            /// </summary>
            public Vector3 MinPos;
            /// <summary>
            /// 相机最大位置（缩放最大时, Z值为相应的最大值）
            /// </summary>
            public Vector3 MaxPos;

            /// <summary>
            /// 相机距离目标点最小距离
            /// </summary>
            public float MinDis;

            /// <summary>
            /// 相机距离目标点最大距离
            /// </summary>
            public float MaxDis;

			/// <summary>
			/// 相机距离目标当前距离
			/// </summary>
			public float currDis;

            /// <summary>
            /// 相机默认位置(缩放最小时)
            /// </summary>
            public Vector3 DefaultOffsetPos;

			/// <summary>
			/// 当前位置偏移
			/// </summary>
			public Vector3 CurrOffsetPos;

            /// <summary>
            /// 默认旋转角度
            /// </summary>
            public Vector3 DefulltRotation;
            /// <summary>
            /// 鼠标滚轮缩放倍数 （缩放时能有更好的体验）
            /// </summary>
            public float MouseScrollWheelRatio = 100;
        }

        /// <summary>
        /// 截屏的相机操作
        /// </summary>
        public UnityEngine.Camera screenshotCamera;

        /// <summary>
        /// UI相机
        /// </summary>
        public UnityEngine.Camera uiCamera;

        //tilemap 参数控制
        public TilemapControl tilemapControl;
        //voxel 参数控制
        public VoxelControl voxelControl;

        /// <summary>
        /// 当前需要控制的相机
        /// </summary>
        private UnityEngine.Camera _camera;

        /// <summary>
        /// 当前控制的相机的 tran
        /// </summary>
        private Transform _cameraTran;

        /// <summary>
        /// 缩放比例
        /// </summary>
        private float _scaleRatio = 1;

        /// <summary>
        /// 相机Z轴改变事件
        /// </summary>
        private Action<float> _cameraZAxisChangeAction;

        #region 初始化

        // Use this for initialization
        void Awake()
        {
            _instance = this;

            _camera = UnityEngine.Camera.main;

            if (_camera != null)
                _cameraTran = _camera.transform;

            //_scaleRatio = DefultPos.z / (DefultPos.x - MinPos.x);

            //默认使用了 tile 设置
            _scaleRatio = (tilemapControl.DefultPos.x - tilemapControl.MinPos.x) / (tilemapControl.MinPos.z - tilemapControl.DefultPos.z);

        }

        #endregion

        #region 单例

        private static CameraManager _instance;

        public static CameraManager GetInst()
        {
            return _instance;
        }

        #endregion

        #region 相机Z轴改变事件通知

        /// <summary>
        /// 添加相机Z轴事件更改通知
        /// </summary>
        /// <param name="action"></param>
        public void AddCameraZAxisChangeAction(Action<float> action)
        {
            _cameraZAxisChangeAction += action;
        }

        /// <summary>
        /// 清理相机Z轴改变事件通知
        /// </summary>
        public void ClearCameraZAxisChangeAction()
        {
            _cameraZAxisChangeAction = null;
        }

        /// <summary>
        /// 事件改变时进行通知
        /// </summary>
        private void CameraZAxisChange()
        {
            if (_cameraZAxisChangeAction != null)
            {
				float f = 0;
				if (!SceneManager.instance.IsVoxel)
				{
					//计算出，当前位置，占总体位置的百分比
					var z = _cameraTran.position.z;
					var minZ = tilemapControl.DefultPos.z;
					var maxZ = tilemapControl.MaxPos.z;

					f = (z - minZ) / (maxZ - minZ);
				} 
				else
				{
					f = (voxelControl.currDis - voxelControl.MinDis) / (voxelControl.MaxDis - voxelControl.MinDis);
				}

                _cameraZAxisChangeAction(f);
            }
        }

        #endregion

        #region 刷帧更新相机数据

#if UNITY_STANDALONE || UNITY_STANDALONE_WIN

        // Update is called once per frame
        void Update()
        {
            if (_camera == null)
                return;

            //鼠标移动
            MouseMoveCam();
        }

#endif

        #endregion

        #region 相机归位

        /// <summary>
        /// 相机是否处于重置位置状态
        /// </summary>
        private bool _isCameraResetingPos;

        

        public void CameraResetPos()
        {
            if(SceneManager.instance.IsVoxel)
            {
                //显示 voxel 的时候才能使用此功能
                
                //if (GameData.instance.IsShowCameraMaxDis)
                {
                    Vector3 pos;
                    Quaternion rot;
                    rot = Quaternion.Euler(voxelControl.DefulltRotation);

                    var curDis = voxelControl.currDis;

                    //voxelControl.currDis = voxelControl.MaxDis;
                    voxelControl.CurrOffsetPos = voxelControl.DefaultOffsetPos;

                    //从当前距离开始计算
                    pos = rot * new Vector3(0, 0, -voxelControl.currDis) + voxelControl.CurrOffsetPos;
                    _cameraTran.position = pos;
                    _cameraTran.rotation = rot;
                }


                //设置成默认位置的时候，重新刷新一次Z值
                CameraZAxisChange();

                if(!_isCameraResetingPos)
                {
                    _isCameraResetingPos = true;
                    Tick.AddUpdate(AutoUpdateCameraPos);
                }
            }
        }

        /// <summary>
        /// 自动更新相机位置信息
        /// </summary>
        private void AutoUpdateCameraPos()
        {

            var dis = ConstantConfig.GetGameConfigFloat(GameConfigKey.camera_auto_reset_pos_speed);

            SetInputDistance(dis);

            var f = (voxelControl.currDis - voxelControl.MinDis) / (voxelControl.MaxDis - voxelControl.MinDis);

            //相机显示最大的距离，那么就是相机拉到最远的时候
            if (f >= 1)
            {
                _isCameraResetingPos = false;
                GameData.instance.IsShowCameraMaxDis = false;
                Tick.RemoveUpdate(AutoUpdateCameraPos);
            }

            //if(GameData.instance.IsShowCameraMaxDis)
            //{
            //    //相机显示最大的距离，那么就是相机拉到最远的时候
            //    if(f >= 1)
            //    {
            //        _isCameraResetingPos = false;
            //        GameData.instance.IsShowCameraMaxDis = false;
            //        Tick.RemoveUpdate(AutoUpdateCameraPos);
            //    }
            //}
            //else
            //{
            //    //相机显示最小的距离，那么就是相机拉到最近的时候
            //    if (f <= 0)
            //    {
            //        _isCameraResetingPos = false;
            //        GameData.instance.IsShowCameraMaxDis = true;
            //        Tick.RemoveUpdate(AutoUpdateCameraPos);
            //    }
            //}
        }

        #endregion

        #region 相机移动操作

        /// <summary>
        /// 设置缩放设置
        /// </summary>
        /// <param name="dis"></param>
        public void SetInputDistance(float dis)
        {
            if(_camera == null
               || Mathf.Abs(dis) <= 0.2f)
                return;

			Vector3 pos;

			if (SceneManager.instance.IsVoxel)
			{
			    dis = -dis;
				var currDis = voxelControl.currDis;
				voxelControl.currDis = Mathf.Clamp (voxelControl.currDis + dis, voxelControl.MinDis, voxelControl.MaxDis);
				dis = voxelControl.currDis - currDis;
				if (dis == 0)
					return;

				//z 轴为负值，所以转换一下坐标方向
				pos = _cameraTran.position - _cameraTran.forward * dis;

				//pos.z = Mathf.Clamp(pos.z, -voxelControl.MaxDis, -voxelControl.MinDis);
			} 
			else 
			{
				pos = _cameraTran.position + _cameraTran.forward * dis;
				pos.z = Mathf.Clamp(pos.z, tilemapControl.DefultPos.z, tilemapControl.MaxPos.z);
			}

            //设置相机位置
            SetCameraPos(pos, _cameraTran.rotation, pos.z);

            //Z值改变了
            CameraZAxisChange();

            //_cameraTran.position = pos;
        }

        /// <summary>
        /// 设置输入移动设置
        /// </summary>
        /// <param name="delta"></param>
        public void SetInputMove(Vector2 delta)
        {
            if (_camera == null)
                return;

            var rotation = Quaternion.Euler(0, _cameraTran.rotation.eulerAngles.y, 0);
            var pos = rotation * new Vector3(-delta.x, -delta.y, 0) + _cameraTran.position;

			if (SceneManager.instance.IsVoxel)
			{
				voxelControl.CurrOffsetPos += pos - _cameraTran.position;
			}

            //设置相机位置
            SetCameraPos(pos, _cameraTran.rotation, _cameraTran.position.z);

            //var scaleZ = (pos.z - DefultPos.z) * _scaleRatio;

            //pos.x = Mathf.Clamp(pos.x, DefultPos.x - scaleZ, DefultPos.x + scaleZ);
            //pos.y = Mathf.Clamp(pos.y, DefultPos.y - scaleZ, DefultPos.y + scaleZ);
            //pos.z = _cameraTran.position.z;
            //_cameraTran.position = pos;
        }

		/// <summary>
		/// 设置相机的旋转角度
		/// </summary>
		/// <param name="delta">Delta.</param>
		public void SetInputRotation(Vector2 delta)
		{
			if (_camera == null)
				return;


			var angle = _cameraTran.rotation.eulerAngles;
			angle += new Vector3 (-delta.x, delta.y);
			var rotation = Quaternion.Euler (angle);
			var pos = rotation * new Vector3 (0, 0, -voxelControl.currDis) + voxelControl.CurrOffsetPos;

            //设置相机位置
            SetCameraPos(pos, rotation, _cameraTran.position.z);
		}

        /// <summary>
        /// 设置相机位置
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="z"></param>
		private void SetCameraPos(Vector3 pos, Quaternion rot, float z)
        {
            if (_camera == null)
                return;

            if (SceneManager.instance.IsVoxel)
            {
                //var scaleZ = (z - voxelControl.DefultPos.z) * _scaleRatio;
//				Debug.LogError ("  sdasdsd " + scaleZ);
//                pos.x = Mathf.Clamp(pos.x, voxelControl.DefultPos.x - scaleZ, voxelControl.DefultPos.x + scaleZ);
//                pos.y = Mathf.Clamp(pos.y, voxelControl.DefultPos.y - scaleZ, voxelControl.DefultPos.y + scaleZ);
//                pos.z = z;
                _cameraTran.position = pos;
				_cameraTran.rotation = rot;
            }
            else
            {
                var scaleZ = (z - tilemapControl.DefultPos.z) * _scaleRatio;

                pos.x = Mathf.Clamp(pos.x, tilemapControl.DefultPos.x - scaleZ, tilemapControl.DefultPos.x + scaleZ);
                pos.y = Mathf.Clamp(pos.y, tilemapControl.DefultPos.y - scaleZ, tilemapControl.DefultPos.y + scaleZ);
                pos.z = z;
                _cameraTran.position = pos;
            }
        }

        /// <summary>
        /// 设置相机默认位置
        /// </summary>
        public void SetCameraDafultPos()
        {
            if (_camera == null)
                return;

            Vector3 pos;
            Quaternion rot;
            if (SceneManager.instance.IsVoxel)
            {
                rot = Quaternion.Euler(voxelControl.DefulltRotation);

				voxelControl.currDis = voxelControl.MaxDis;
				voxelControl.CurrOffsetPos = voxelControl.DefaultOffsetPos;
				pos = rot * new Vector3 (0, 0, -voxelControl.MaxDis) + voxelControl.CurrOffsetPos;

            }
            else
            {
                pos = tilemapControl.DefultPos;
                rot = Quaternion.Euler(tilemapControl.DefulltRotation);
            }

            _cameraTran.position = pos;
            _cameraTran.rotation = rot;

            //设置成默认位置的时候，重新刷新一次Z值
            CameraZAxisChange();
        }

        /// <summary>
        /// 鼠标移动
        /// </summary>
        private void MouseMoveCam()
        {

            if (!PlayerGameInfo.instance.IsAcceptInputOperation)
                return;

            //鼠标滚动，缩放场景
            if (UnityEngine.Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                var ratio = SceneManager.instance.IsVoxel
                    ? voxelControl.MouseScrollWheelRatio
                    : tilemapControl.MouseScrollWheelRatio;

                //自由缩放方式;
                float m_distance = UnityEngine.Input.GetAxis("Mouse ScrollWheel") * ratio;

                SetInputDistance(m_distance);
            }

			//鼠标右键控制旋转
			if(UnityEngine.Input.GetMouseButton(1))
			{
//				var delta_x = UnityEngine.Input.GetAxis("Mouse X");
//			    var delta_y = UnityEngine.Input.GetAxis("Mouse Y");
//
//				SetInputRotation (new Vector2(delta_y, delta_x) * 3);

				var delta_x = UnityEngine.Input.GetAxis("Mouse X");
			    var delta_y = UnityEngine.Input.GetAxis("Mouse Y");
			    SetInputMove(new Vector2(delta_x, delta_y) * 3);
			}

            ////按住鼠标左键移动
            //if (UnityEngine.Input.GetMouseButton(0))
            //{
            //    ////如果当前正在操作目标数据，那么就不移动相机
            //    //if (TransformGizmo.GetInst().IsChangeTarget())
            //    //    return;

            //    var delta_x = UnityEngine.Input.GetAxis("Mouse X");
            //    var delta_y = UnityEngine.Input.GetAxis("Mouse Y");
            //    SetInputMove(new Vector2(delta_x, delta_y) * 3);
            //}
        }

        #endregion

        #region 相机坐标转换

        /// <summary>
        /// 屏幕坐标转换
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <returns></returns>
        public Vector3 ScreenToWorldPoint(Vector2 mousePosition)
        {
            if (_camera == null)
            {
                Debug.LogError(" 当前 主相机为 null, 不能进行屏幕坐标转换成世界坐标 ");
                return Vector3.zero;
            }

            var pos = new Vector3(mousePosition.x, mousePosition.y, -_cameraTran.position.z);

            return _camera.ScreenToWorldPoint(pos);
        }

		/// <summary>
		/// 从屏幕坐标点发射 射线
		/// </summary>
		/// <returns>The point to ray.</returns>
		/// <param name="mousePosition">Mouse position.</param>
		public Ray ScreenPointToRay(Vector2 mousePosition)
		{
			if (_camera == null)
			{
				Debug.LogError(" 当前 主相机为 null, 不能进行屏幕坐标转换成射线 ");
				return default(Ray);
			}

			var pos = new Vector3(mousePosition.x, mousePosition.y, -_cameraTran.position.z);
			//Debug.Log (pos + "  123 " + UnityEngine.Input.mousePosition);
			return _camera.ScreenPointToRay (pos);
		}

        #endregion

        #region 相机截屏操作

        /// <summary>
        /// 相机截屏操作
        /// </summary>
        public Texture2D CaptureScreenshot()
        {
            if (screenshotCamera == null)
            {
                Debug.LogError(" 截屏相机丢失 ");
                return null;
            }

            screenshotCamera.gameObject.SetActive(true);

            //相机位置归位
            if (SceneManager.instance.IsVoxel)
            {
                var rot = Quaternion.Euler(voxelControl.DefulltRotation);

                voxelControl.currDis = voxelControl.MaxDis;
                voxelControl.CurrOffsetPos = voxelControl.DefaultOffsetPos;
                var pos = rot * new Vector3(0, 0, -voxelControl.MaxDis) + voxelControl.CurrOffsetPos;

                screenshotCamera.transform.position = pos;
                screenshotCamera.transform.rotation = rot;

            }
            else
            {
                var pos = tilemapControl.DefultPos;
                var rot = Quaternion.Euler(tilemapControl.DefulltRotation);

                screenshotCamera.transform.position = pos;
                screenshotCamera.transform.rotation = rot;
            }


            //var rt = new RenderTexture(ConstantConfig.CAPTURE_SCREEN_SHOT_RECT_W, ConstantConfig.CAPTURE_SCREEN_SHOT_RECT_H, 0);

            //screenshotCamera.targetTexture = rt;
            //screenshotCamera.Render();

            //RenderTexture.active = rt;

            //var tex = new Texture2D(ConstantConfig.CAPTURE_SCREEN_SHOT_TEX_W,
            //    ConstantConfig.CAPTURE_SCREEN_SHOT_TEX_H, TextureFormat.RGB24, false);

            //var rect = new Rect((int)((ConstantConfig.CAPTURE_SCREEN_SHOT_RECT_W - ConstantConfig.CAPTURE_SCREEN_SHOT_TEX_W) / 2 + ConstantConfig.CAPTURE_SCREEN_SHOT_OFFSET_W),
            //    (int)((ConstantConfig.CAPTURE_SCREEN_SHOT_RECT_H - ConstantConfig.CAPTURE_SCREEN_SHOT_TEX_H) / 2 + ConstantConfig.CAPTURE_SCREEN_SHOT_OFFSET_H),
            //    ConstantConfig.CAPTURE_SCREEN_SHOT_TEX_W,
            //    ConstantConfig.CAPTURE_SCREEN_SHOT_TEX_H);

            //var tex = new Texture2D(ConstantConfig.CAPTURE_SCREEN_SHOT_RECT_W,
            //    ConstantConfig.CAPTURE_SCREEN_SHOT_RECT_H, TextureFormat.RGB24, false);

            //var rect = new Rect(0,
            //    0,
            //    ConstantConfig.CAPTURE_SCREEN_SHOT_RECT_W,
            //    ConstantConfig.CAPTURE_SCREEN_SHOT_RECT_H);

            var rectW = ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_rect_w);
            var rectH = (int)(((float)rectW / Screen.width) * Screen.height);

            var rt = new RenderTexture(rectW, rectH, 0);

            screenshotCamera.targetTexture = rt;
            screenshotCamera.Render();

            RenderTexture.active = rt;

            var tex = new Texture2D(ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_tex_h),
                ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_tex_h), TextureFormat.RGB24, false);

#if UNITY_ANDROID || UNITY_IOS

            var rect = new Rect((int)(rectW - ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_tex_w)) / 2 - (int)(rectW * ConstantConfig.GetGameConfigFloat(GameConfigKey.capture_screen_shot_offset_w)),
                (int)((rectH - ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_tex_h)) / 2 - (int)(rectH * GameUtils.GetScreenshotOffsetH())),
                ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_tex_w),
                ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_tex_h));

#else
            var rect = new Rect((int)(rectW - ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_tex_w)) / 2 + (int)(rectW * ConstantConfig.GetGameConfigFloat(GameConfigKey.capture_screen_shot_offset_w)),
                (int)((rectH - ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_tex_h)) / 2 + (int)(rectH * GameUtils.GetScreenshotOffsetH())),
                ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_tex_w),
                ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_tex_h));

#endif

            //读取像素信息
            tex.ReadPixels(rect, 0, 0, false);

            tex.filterMode = FilterMode.Point;

            tex.Apply();

            screenshotCamera.targetTexture = null;
            RenderTexture.active = null;
            GameObject.Destroy(rt);

            screenshotCamera.gameObject.SetActive(false);

            return tex;
        }

        /// <summary>
        /// 相机截屏操作
        /// </summary>
        public Texture2D UICaptureScreenshot()
        {
            if (uiCamera == null)
            {
                Debug.LogError(" 截屏相机丢失 ");
                return null;
            }

            var rectW = ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_rect_w);
            var rectH = (int)(((float)rectW / Screen.width) * Screen.height);

            var rt = new RenderTexture(rectW, rectH, 0);

            var clearFlags = uiCamera.clearFlags;
            uiCamera.clearFlags = CameraClearFlags.SolidColor;
            uiCamera.targetTexture = rt;
            uiCamera.Render();

            RenderTexture.active = rt;

            var tex = new Texture2D(ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_tex_w),
                ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_tex_h), TextureFormat.RGB24, false);

#if UNITY_ANDROID || UNITY_IOS

            var rect = new Rect((int)(rectW - ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_tex_w)) / 2 - (int)(rectW * ConstantConfig.GetGameConfigFloat(GameConfigKey.capture_screen_shot_offset_w)),
                (int)((rectH - ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_tex_h)) / 2 - (int)(rectH * GameUtils.GetScreenshotOffsetH())),
                ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_tex_w),
                ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_tex_h));

#else
            var rect = new Rect((int)(rectW - ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_tex_w)) / 2 + (int)(rectW * ConstantConfig.GetGameConfigFloat(GameConfigKey.capture_screen_shot_offset_w)),
                (int)((rectH - ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_tex_h)) / 2 + (int)(rectH * GameUtils.GetScreenshotOffsetH())),
                ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_tex_w),
                ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_tex_h));

#endif

            //读取像素信息
            tex.ReadPixels(rect, 0, 0, false);

            tex.filterMode = FilterMode.Point;

            tex.Apply();

            uiCamera.clearFlags = clearFlags;
            uiCamera.targetTexture = null;
            RenderTexture.active = null;
            GameObject.Destroy(rt);

            

            return tex;
        }

        /// <summary>
        /// 存储相机截屏图片
        /// </summary>
        public Texture2D SaveCaptureScreenshot(string name)
        {
            if (screenshotCamera == null)
            {
                Debug.LogError(" 截屏相机丢失 ");
                return null;
            }

            screenshotCamera.gameObject.SetActive(true);

            //相机位置归位
            if (SceneManager.instance.IsVoxel)
            {
                var rot = Quaternion.Euler(voxelControl.DefulltRotation);

                voxelControl.currDis = voxelControl.MaxDis;
                voxelControl.CurrOffsetPos = voxelControl.DefaultOffsetPos;
                var pos = rot * new Vector3(0, 0, -voxelControl.MaxDis) + voxelControl.CurrOffsetPos;

                screenshotCamera.transform.position = pos;
                screenshotCamera.transform.rotation = rot;

            }
            else
            {
                var pos = tilemapControl.DefultPos;
                var rot = Quaternion.Euler(tilemapControl.DefulltRotation);

                screenshotCamera.transform.position = pos;
                screenshotCamera.transform.rotation = rot;
            }


            var rectW = ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_rect_w);
            var rectH = (int)(((float)rectW / Screen.width) * Screen.height);

            var rt = new RenderTexture(rectW, rectH, 0);

            screenshotCamera.targetTexture = rt;
            screenshotCamera.Render();

            RenderTexture.active = rt;

            var tex = new Texture2D(ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_tex_w),
                ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_tex_h), TextureFormat.RGB24, false);

#if UNITY_ANDROID || UNITY_IOS

            var rect = new Rect((int)(rectW - ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_tex_w)) / 2 - (int)(rectW * ConstantConfig.GetGameConfigFloat(GameConfigKey.capture_screen_shot_offset_w)),
                (int)((rectH - ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_tex_h)) / 2 - (int)(rectH * GameUtils.GetScreenshotOffsetH())),
                ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_tex_w),
                ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_tex_h));

#else
            var rect = new Rect((int)(rectW - ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_tex_w)) / 2 + (int)(rectW * ConstantConfig.GetGameConfigFloat(GameConfigKey.capture_screen_shot_offset_w)),
                (int)((rectH - ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_tex_h)) / 2 + (int)(rectH * GameUtils.GetScreenshotOffsetH())),
                ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_tex_w),
                ConstantConfig.GetGameConfigInt(GameConfigKey.capture_screen_shot_tex_h));

#endif

            //读取像素信息
            tex.ReadPixels(rect, 0, 0, false);

            tex.filterMode = FilterMode.Point;

            tex.Apply();

            screenshotCamera.targetTexture = null;
            RenderTexture.active = null;
            GameObject.Destroy(rt);

            screenshotCamera.gameObject.SetActive(false);

            //保存图片信息
            SaveCaptureScreenshot(tex, name);

            return tex;
        }

        /// <summary>
        /// 保存图片信息
        /// </summary>
        /// <param name="tex"></param>
        /// <param name="name"></param>
        public void SaveCaptureScreenshot(Texture2D tex, string name)
        {
            //保存图片
            var bs = tex.EncodeToPNG();

#if UNITY_ANDROID

            string path = ConstantConfig.GetGameConfigString(GameConfigKey.camera_screen_shots_path_android);

#elif UNITY_IOS

            string path = ConstantConfig.CAMERA_SCREEN_SHOTS_PATH_IOS;

#else

            string path = ConstantConfig.CAMERA_SCREEN_SHOTS_PATH_PC;

#endif

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);


            if (string.IsNullOrEmpty(name))
                name = GameUtils.GetCurTime();

            path = string.Format("{0}Screenshot_{1}.png", path, name);

            System.IO.File.WriteAllBytes(path, bs);

#if UNITY_ANDROID || UNITY_IOS

            //刷新图片到相册
            PlatformManager.instance.SaveImageToAlbum(path);

#else

#endif

            //扫描相册
            //ScanFile(path);
        }

        /// <summary>
        /// 刷新图片，显示到相册中
        /// </summary>
        /// <param name="path"></param>
        static void ScanFile(string path)
        {
            using (AndroidJavaClass PlayerActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject playerActivity = PlayerActivity.GetStatic<AndroidJavaObject>("currentActivity");
                using (AndroidJavaObject Conn = new AndroidJavaObject("android.media.MediaScannerConnection", playerActivity, null))
                {
                    Conn.CallStatic("scanFile", playerActivity, new string[] { path }, null, null);
                }
            }
        }

        

#endregion

    }
}


