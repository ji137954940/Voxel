using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Color.Number.Camera;
using Color.Number.GameInfo;
using Color.Number.Grid;
using UnityEngine;
using ZFrame;
using ZLib;
using Color.Number.Scene;

namespace Color.Number.Input
{
    /// <summary>
    /// 接收，管理各种输入的操作
    /// </summary>
    public class InputManager : Singleton<InputManager>
    {
        /// <summary>
        /// 是否正在长按状态
        /// </summary>
        private bool _isLongTap;

        /// <summary>
        /// 初始化事件监听
        /// </summary>
        public void Init()
        {
			//点击操作
            EasyTouch.On_SimpleTap += OnSimpleTap;
			//没有点中物体时的拖拽
            EasyTouch.On_Swipe += OnDrag;
            EasyTouch.On_SwipeEnd += OnDragEnd;
			//点中物体时的拖拽
			EasyTouch.On_Drag += OnDrag;
			EasyTouch.On_DragEnd += OnDragEnd;
			//缩放操作
            EasyTouch.On_Pinch += OnPinch;

            //设置长按支持拖动
            //注意：如果长按时拖动，那么长按的结束事件应该和拖动的结束事件一起处理
            EasyTouch.instance.longTapSupportDrag = true;
            EasyTouch.On_LongTapStart += OnLongTapStart;
            EasyTouch.On_LongTapEnd += OnLongTapEnd;
        }

        #region 设置输入操作数据

        /// <summary>
        /// 设置操作长按时间
        /// </summary>
        public void SetEasyTouchLongTapTime()
        {
            var time = ConstantConfig.GetGameConfigFloat(GameConfigKey.finger_long_tap_time);
            if (time <= 0)
                time = 1f;

            EasyTouch.SetLongTapTime(time);
        }

        #endregion

        #region 输入事件监听

        /// <summary>
        /// 点击事件
        /// </summary>
        /// <param name="gesture"></param>
        void OnSimpleTap(Gesture gesture)
        {
            if (gesture != null)
            {
                //Debug.Log(LogType.Log, "tap");

                //var message = ME.New<ME_ET_OnSimpleTap>();

                //message.gesture = gesture;
                ////按下
                //Frame.DispatchEvent(message);


                if (PlayerGameInfo.instance.IsAcceptInputOperation)
                {
                    OnHandleTap(gesture);
                }
            }
        }

        /// <summary>
        /// 长按开始
        /// </summary>
        /// <param name="gesture"></param>
        void OnLongTapStart(Gesture gesture)
        {
            if (gesture != null)
            {
                _isLongTap = true;
                _lastPos = Vector3.zero;
            }
        }

        /// <summary>
        /// 长按结束
        /// </summary>
        /// <param name="gesture"></param>
        void OnLongTapEnd(Gesture gesture)
        {
            if (gesture != null)
            {
                _isLongTap = false;
            }
        }

        /// <summary>
        /// 滑动
        /// </summary>
        /// <param name="gesture"></param>
        void OnDrag(Gesture gesture)
        {
            if (gesture != null)
            {
                if (gesture.touchCount == 1)
                {
                    if (PlayerGameInfo.instance.IsAcceptInputOperation)
                    {
						if (!SceneManager.instance.IsVoxel)
						{
							OnHandleSwipe (gesture);
						} 
						else 
						{
							if (!_isLongTap)
								OnHandleRotation (gesture);
							else
								OnHandleTap (gesture);
						}

							
                    }
                }
                else if (gesture.touchCount == 2)
                {
                    //2个手指处理拖动
					if(SceneManager.instance.IsVoxel)
						OnHandleSwipe(gesture);
                }

            }
        }
			

        void OnDragEnd(Gesture gesture)
        {
            if (gesture != null)
            {
                if (gesture.touchCount == 1)
                {
                    if (PlayerGameInfo.instance.IsAcceptInputOperation)
                    {
                        //如果正在处于长按的状态，那么结束长按状态
                        if (_isLongTap)
                            _isLongTap = false;
                    }

                }
                else if (gesture.touchCount == 2)
                {
                    //var message = ME.New<ME_ET_OnSwipe>();
                    //message.gesture = gesture;
                    //Frame.DispatchEvent(message);
                }
            }
        }

        /// <summary>
        /// 捏，收缩
        /// </summary>
        /// <param name="gesture"></param>
        void OnPinch(Gesture gesture)
        {
            if (gesture != null)
            {
                if (PlayerGameInfo.instance.IsAcceptInputOperation)
                {
                    OnHandlePinch(gesture);
                }
            }
        }

        #endregion

        #region 输入事件处理

        #region 点击操作

        /// <summary>
        /// 点击的最后位置
        /// </summary>
        private Vector3 _lastPos;

        /// <summary>
        /// 点击操作处理
        /// </summary>
        /// <param name="gesture"></param>
        private void OnHandleTap(Gesture gesture)
        {
            if (gesture != null)
            {
				SceneManager.instance.ClickGameObject(gesture.position, _lastPos, _isLongTap);

                _lastPos = gesture.position;
            }
        }

        #endregion

        #region 滑动操作处理

        /// <summary>
        /// 拖拽操作处理
        /// </summary>
        /// <param name="gesture"></param>
        private void OnHandleSwipe(Gesture gesture)
        {
            if (gesture != null)
            {
                if (!_isLongTap)
                {
                    //非长按状态
                    var move = Vector2.zero;
#if UNITY_IOS
                    if (SceneManager.instance.IsVoxel)
                    {
                        move = new Vector2(ConstantConfig.GetGameConfigFloat(GameConfigKey.camera_swipe_x_ios_voxel) * gesture.deltaPosition.x,
                            ConstantConfig.GetGameConfigFloat(GameConfigKey.camera_swipe_y_ios_voxel) * gesture.deltaPosition.y);
                    }
                    else
                    {
                        move = new Vector2(ConstantConfig.GetGameConfigFloat(GameConfigKey.camera_swipe_x_ios_grid) * gesture.deltaPosition.x,
                            ConstantConfig.GetGameConfigFloat(GameConfigKey.camera_swipe_y_ios_grid) * gesture.deltaPosition.y);
                    }

#elif UNITY_ANDROID

                    if (SceneManager.instance.IsVoxel)
                    {

                        move = new Vector2(ConstantConfig.GetGameConfigFloat(GameConfigKey.camera_swipe_x_android_voxel) * gesture.deltaPosition.x,
                            ConstantConfig.GetGameConfigFloat(GameConfigKey.camera_swipe_y_android_voxel) * gesture.deltaPosition.y);
                    }
                    else
                    {

                        move = new Vector2(ConstantConfig.GetGameConfigFloat(GameConfigKey.camera_swipe_x_android_grid) * gesture.deltaPosition.x,
                            ConstantConfig.GetGameConfigFloat(GameConfigKey.camera_swipe_y_android_grid) * gesture.deltaPosition.y);
                    }

#else
                    if (SceneManager.instance.IsVoxel)
                    {
                        move = new Vector2(ConstantConfig.GetGameConfigFloat(GameConfigKey.camera_swipe_x_pc_voxel) * gesture.deltaPosition.x,
                            ConstantConfig.GetGameConfigFloat(GameConfigKey.camera_swipe_y_pc_voxel) * gesture.deltaPosition.y);
                    }
                    else
                    {
                        move = new Vector2(ConstantConfig.GetGameConfigFloat(GameConfigKey.camera_swipe_x_pc_grid) * gesture.deltaPosition.x,
                            ConstantConfig.GetGameConfigFloat(GameConfigKey.camera_swipe_y_pc_grid) * gesture.deltaPosition.y);
                    }

                        
#endif

                    CameraManager.GetInst().SetInputMove(move);
                }
                else
                {
                    //长按状态, 每次拖动都处理点击事件
                    OnHandleTap(gesture);
                }
            }
        }

#endregion

		#region 旋转处理操作

		/// <summary>
		/// 处理旋转操作
		/// </summary>
		/// <param name="gesture">Gesture.</param>
		private void OnHandleRotation(Gesture gesture)
		{
			if (gesture != null)
			{
				var delta = new Vector2 (gesture.deltaPosition.y, gesture.deltaPosition.x) * 0.2f;

				//TODO 需要对各个平台做适配处理
				CameraManager.GetInst ().SetInputRotation (delta);
			}
		}

		#endregion

#region 缩放操作处理

        /// <summary>
        /// 缩放操作处理
        /// </summary>
        /// <param name="gesture"></param>
        private void OnHandlePinch(Gesture gesture)
        {
            if (gesture != null)
            {

                var dis = 0f;
#if UNITY_IOS

                if (SceneManager.instance.IsVoxel)
                {

                    dis = ConstantConfig.GetGameConfigFloat(GameConfigKey.camera_pinch_ratio_ios_voxel) * gesture.deltaPinch;
                }
                else
                {
                    dis = ConstantConfig.GetGameConfigFloat(GameConfigKey.camera_pinch_ratio_ios_grid) * gesture.deltaPinch;
                }

#elif UNITY_ANDROID

                if (SceneManager.instance.IsVoxel)
                {
                    dis = ConstantConfig.GetGameConfigFloat(GameConfigKey.camera_pinch_ratio_android_voxel) * gesture.deltaPinch;
                }
                else
                {
                    dis = ConstantConfig.GetGameConfigFloat(GameConfigKey.camera_pinch_ratio_android_grid) * gesture.deltaPinch;
                }


#else
                if (SceneManager.instance.IsVoxel)
                {
                    dis = ConstantConfig.GetGameConfigFloat(GameConfigKey.camera_pinch_ratio_pc_voxel) * gesture.deltaPinch;
                }
                else
                {
                    dis = ConstantConfig.GetGameConfigFloat(GameConfigKey.camera_pinch_ratio_pc_grid) * gesture.deltaPinch;
                }
#endif

                CameraManager.GetInst().SetInputDistance(dis);
            }
        }

#endregion

#endregion
    }
}


