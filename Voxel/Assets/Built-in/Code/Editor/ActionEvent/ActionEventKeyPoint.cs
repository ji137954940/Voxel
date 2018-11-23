using AFrame.EditorCommon;
using AFrame.Table;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ActionEvent
{

    public class ActionEventKeyPoint
    {

        #region 操作信息

        /// <summary>
        /// 关键点结构类
        /// </summary>
        [Serializable]
        public class KeyPointData
        {
            public string functionName;
            public float floatParameter;
            public int intParameter;
            public string stringParameter;
            public float time;

            string _functionName;
            float _floatParameter;
            int _intParameter;
            string _stringParameter;
            float _time;

            public KeyPointData() { }

            public KeyPointData(string functionName, float floatParameter, int intParameter, string stringParameter, float time)
            {
                OnInit(functionName, floatParameter, intParameter, stringParameter, time);

                Revert();
            }

            /// <summary>
            /// 存储初始化数据
            /// </summary>
            /// <param name="functionName"></param>
            /// <param name="floatParameter"></param>
            /// <param name="intParameter"></param>
            /// <param name="stringParameter"></param>
            /// <param name="time"></param>
            void OnInit(string functionName, float floatParameter, int intParameter, string stringParameter, float time)
            {
                this._functionName = functionName;
                this._floatParameter = floatParameter;
                this._intParameter = intParameter;
                this._stringParameter = stringParameter;
                this._time = time;
            }

            /// <summary>
            /// 数据恢复
            /// </summary>
            public void Revert()
            {
                functionName = _functionName;
                floatParameter = _floatParameter;
                intParameter = _intParameter;
                stringParameter = _stringParameter;
                time = _time;
            }

            /// <summary>
            /// 获取一个动作事件
            /// </summary>
            /// <returns></returns>
            public AnimationEvent GetAnimationEvent(float time_length = -1)
            {
                AnimationEvent ae = new AnimationEvent();
                ae.functionName = functionName;
                ae.floatParameter = floatParameter;
                ae.intParameter = intParameter;
                ae.stringParameter = stringParameter;
                if (time_length > 0)
                    ae.time = time / time_length;
                else
                    ae.time = time;

                return ae;
            }
        }

        #endregion

        #region 数据显示

        /// <summary>
        /// 动作事件类型
        /// </summary>
        public enum Action_Event_Type
        {
            OnHurtDelay,                                            //伤害点延时
            OnKeyPoint,                                             //关键点
            OnBreakPoint,                                           //打断关键点
            OnStartEffect,                                          //开始播放特效
            OnForcePlayEffect,                                      //直接播放特效不管处理什么状态
            OnPlayScreenEffect,                                     //播放屏幕效果
            OnStartSound,                                           //播放声音
            OnStepSound,                                            //播放脚步声
            OnStartShakeCamera,                                     //相机震屏
            OnShowScreenBlur,                                       //屏幕模糊
            OnDashSkillStartMove,                                   //位移可开始移动时间
            OnAnimEventTrigger,                                     //动画事件触发
            OnShaderEffect                                          //Shader特效
        }

        //动作事件类型 信息
        string[] action_event_type_desc = new string[]
        {
            "伤害点延时",
            "关键点",
            "打断关键点",
            "播放特效",
            "强制播放特效不管处理什么状态",
            "播放屏幕效果",
            "播放声音",
            "播放脚步声",
            "相机震屏",
            "屏幕模糊",
            "位移可开始移动时间",
            "动画事件触发 \n{wh 武器隐藏} \n{ws 武器显示} \n{mh 模型隐藏} \n{ms 模型显示}",
            "Shader特效",
        };

        TableView view;

        int list_width = 15;
        int list_height = 15;
        int rect_offset = 5;

        //当前选中的 clip 文件
        AnimationClip clip;
        string path;
        //clip 上面的 Event 列表
        List<AnimationEvent> event_list;

        //当前选中的对象数据
        object selectObj;
        //当前选中的 AnimationEvent对象
        KeyPointData kp;

        List<KeyPointData> key_point_list;
        int kp_Id = 0;

        public void OnEnable(Rect pos, EditorWindow parent)
        {
            if(view == null)
            {
                view = new TableView(parent, typeof(KeyPointData));
                view.AddColum("functionName", "Action_Event", 0.3f);
                view.AddColum("floatParameter", "float", 0.2f);
                view.AddColum("intParameter", "int", 0.1f);
                view.AddColum("stringParameter", "string", 0.2f);
                view.AddColum("time", "time", 0.2f);

                view.onSelected += OnSelectChange;
            }
        }

        /// <summary>
        /// 当前选中的显示列表中条目数据改变
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="colId"></param>
        void OnSelectChange(object obj, int colId)
        {
            if (selectObj == obj)
                return;

            selectObj = obj;

            kp = (KeyPointData)selectObj;

            kp_Id = key_point_list.IndexOf(kp);
            int count = key_point_list.Count;
            for (int i = 0; i < count; i++)
            {
                if (key_point_list[i] != null)
                    key_point_list.Reverse();
            }
        }

        /// <summary>
        /// 当前选中的对象已经改变
        /// </summary>
        public void OnSelectObjChanged(AnimationClip clip, string path)
        {
            this.clip = clip;
            this.path = path;

            if(clip != null)
            {
                AnimationEvent[] arr = AnimationUtility.GetAnimationEvents(clip);
                event_list = new List<AnimationEvent>(arr);

                int count = arr.Length;
                if (key_point_list != null)
                    key_point_list.Clear();
                key_point_list = new List<KeyPointData>();

                for (int i = 0; i < count; i++)
                {
                    KeyPointData data = new KeyPointData(arr[i].functionName, arr[i].floatParameter, arr[i].intParameter, arr[i].stringParameter, arr[i].time);
                    key_point_list.Add(data);
                }
            }

            kp = null;
        }

        public void OnGUI(Rect pos)
        {
            if (clip == null)
                return;

            //left
            Rect rect = new Rect(pos.x + list_width, pos.y + list_height, pos.width / 2 - list_width * 2 - rect_offset, pos.height - list_height * 2);
            GUILayout.BeginArea(rect, EditorConst.WindowBackground);
            {
                if (view != null)
                {
                    view.Draw(new Rect(0, 0, rect.width, rect.height));
                    view.RefreshData(EditorTool.ToObjectList(key_point_list));
                }
            }
            GUILayout.EndArea();

            //right
            rect = new Rect(pos.width / 2 - list_width, pos.y + list_height, + pos.width / 2 - list_width * 2 - rect_offset, pos.height - list_height * 2);

            GUILayout.BeginArea(rect, EditorConst.WindowBackground);
            {
                //view.Draw(new Rect(0, 0, rect.width, rect.height));
                //view.RefreshData(EditorTool.ToObjectList(event_list));

                GUILayout.BeginVertical();

                if(clip != null)
                {
                    GUILayout.Label(" Animation Event ");

                    //basic options
                    EditorGUILayout.Space();

                    //动画文件信息
                    EditorGUILayout.LabelField("Animation Clip Name", clip.name);
                    EditorGUILayout.LabelField("Animation Clip Length", string.Format("{0} s", clip.length.ToString()));
                    EditorGUILayout.LabelField("Animation Clip Frame Rate", clip.frameRate.ToString());
                }

                if(kp != null)
                {
                    //动画事件信息
                    Action_Event_Type curr = GetActionEventTypeEnum(kp.functionName);
                    Action_Event_Type type = (Action_Event_Type)EditorGUILayout.EnumPopup("Function Type", curr);
                    if (type != curr)
                    {
                        curr = type;
                        kp.functionName = type.ToString();
                    }
                    EditorGUILayout.LabelField("Function 描述", action_event_type_desc[(int)curr], GUILayout.MinHeight(100));


                    kp.floatParameter = EditorGUILayout.FloatField("Float", kp.floatParameter);
                    kp.intParameter = EditorGUILayout.IntField("Int", kp.intParameter);
                    kp.stringParameter = EditorGUILayout.TextField("String", kp.stringParameter);
                    kp.time = EditorGUILayout.Slider("Time", kp.time, 0, clip.length);
                }

                GUILayout.BeginHorizontal();

                if(GUILayout.Button(" Add(no save) "))
                {
                    kp = new KeyPointData(Action_Event_Type.OnStartEffect.ToString(), 0, 0, null, 0.1f);

                    key_point_list.Add(kp);
                    kp_Id = key_point_list.Count - 1;
                }

                if(GUILayout.Button(" Delete "))
                {
                    if (kp == null)
                        return;

                    key_point_list.RemoveAt(kp_Id);
                    int count = key_point_list.Count;
                    event_list = new List<AnimationEvent>();
                    for (int i = 0; i < count; i++)
                    {
                        key_point_list[i].Revert();
                        AnimationEvent ae = key_point_list[i].GetAnimationEvent(clip.length);
                        event_list.Add(ae);
                    }

                    kp = null;
                    kp_Id = 0;
                    SaveAnimationEventInfo();
                }

                if(GUILayout.Button(" Save "))
                {
                    if (kp == null
                        || event_list.Count <= view.CurSelectIndex - 1)
                        return;

                    int count = key_point_list.Count;
                    event_list = new List<AnimationEvent>();
                    for (int i = 0; i < count; i++)
                    {
                        if(kp_Id != i)
                            key_point_list[i].Revert();
                        AnimationEvent ae = key_point_list[i].GetAnimationEvent(clip.length);
                        event_list.Add(ae);
                    }

                    SaveAnimationEventInfo();
                }

                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }
            GUILayout.EndArea();
        }

        /// <summary>
        /// string 转换成 enumerate
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        Action_Event_Type GetActionEventTypeEnum(string value)
        {
            return (Action_Event_Type)Enum.Parse(typeof(Action_Event_Type), value);
        }

        /// <summary>
        /// 存储动画事件信息
        /// </summary>
        void SaveAnimationEventInfo()
        {
            if(clip != null && event_list != null)
            {
                ModelImporter modelImporter = AssetImporter.GetAtPath(path) as ModelImporter;
                if (modelImporter == null)
                    return;
                modelImporter.clipAnimations = modelImporter.defaultClipAnimations;

                SerializedObject serializedObject = new SerializedObject(modelImporter);
                SerializedProperty clipAnimations = serializedObject.FindProperty("m_ClipAnimations");
                for (int i = 0; i < clipAnimations.arraySize; i++)
                {
                    AnimationClipInfoProperties clipInfoProperties = new AnimationClipInfoProperties(clipAnimations.GetArrayElementAtIndex(i));
                    clipInfoProperties.SetEvents(event_list.ToArray());
                    clipInfoProperties.name = System.IO.Path.GetFileNameWithoutExtension(path);
                    serializedObject.ApplyModifiedProperties();
                    ////重新导入资源数据
                    //AssetDatabase.ImportAsset(path);
                }
                
                //////保存文件数据
                //AssetDatabase.SaveAssets();
                //AssetDatabase.Refresh();

                //////重新导入资源数据
                //AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

                Debug.LogError(" Animation Event 更新成功 ");

                EditorApplication.delayCall += ChangeSelectObj;
            }
        }

        void ChangeSelectObj()
        {
            EditorApplication.delayCall -= ChangeSelectObj;

            ////保存文件数据
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            ////重新导入资源数据
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            ////切换当前选中显示
            //UnityEngine.Object obj = Selection.activeObject;
            //Selection.activeObject = null;
            //Selection.activeObject = obj;
        }

        #endregion

    }

    #region Animation Clip 属性

    class AnimationClipInfoProperties
    {
        SerializedProperty m_Property;

        private SerializedProperty Get(string property) { return m_Property.FindPropertyRelative(property); }

        public AnimationClipInfoProperties(SerializedProperty prop) { m_Property = prop; }

        public string name { get { return Get("name").stringValue; } set { Get("name").stringValue = value; } }
        public string takeName { get { return Get("takeName").stringValue; } set { Get("takeName").stringValue = value; } }
        public float firstFrame { get { return Get("firstFrame").floatValue; } set { Get("firstFrame").floatValue = value; } }
        public float lastFrame { get { return Get("lastFrame").floatValue; } set { Get("lastFrame").floatValue = value; } }
        public int wrapMode { get { return Get("wrapMode").intValue; } set { Get("wrapMode").intValue = value; } }
        public bool loop { get { return Get("loop").boolValue; } set { Get("loop").boolValue = value; } }

        // Mecanim animation properties  
        public float orientationOffsetY { get { return Get("orientationOffsetY").floatValue; } set { Get("orientationOffsetY").floatValue = value; } }
        public float level { get { return Get("level").floatValue; } set { Get("level").floatValue = value; } }
        public float cycleOffset { get { return Get("cycleOffset").floatValue; } set { Get("cycleOffset").floatValue = value; } }
        public bool loopTime { get { return Get("loopTime").boolValue; } set { Get("loopTime").boolValue = value; } }
        public bool loopBlend { get { return Get("loopBlend").boolValue; } set { Get("loopBlend").boolValue = value; } }
        public bool loopBlendOrientation { get { return Get("loopBlendOrientation").boolValue; } set { Get("loopBlendOrientation").boolValue = value; } }
        public bool loopBlendPositionY { get { return Get("loopBlendPositionY").boolValue; } set { Get("loopBlendPositionY").boolValue = value; } }
        public bool loopBlendPositionXZ { get { return Get("loopBlendPositionXZ").boolValue; } set { Get("loopBlendPositionXZ").boolValue = value; } }
        public bool keepOriginalOrientation { get { return Get("keepOriginalOrientation").boolValue; } set { Get("keepOriginalOrientation").boolValue = value; } }
        public bool keepOriginalPositionY { get { return Get("keepOriginalPositionY").boolValue; } set { Get("keepOriginalPositionY").boolValue = value; } }
        public bool keepOriginalPositionXZ { get { return Get("keepOriginalPositionXZ").boolValue; } set { Get("keepOriginalPositionXZ").boolValue = value; } }
        public bool heightFromFeet { get { return Get("heightFromFeet").boolValue; } set { Get("heightFromFeet").boolValue = value; } }
        public bool mirror { get { return Get("mirror").boolValue; } set { Get("mirror").boolValue = value; } }

        public AnimationEvent GetEvent(int index)
        {
            AnimationEvent evt = new AnimationEvent();
            SerializedProperty events = Get("events");

            if (events != null && events.isArray)
            {
                if (index < events.arraySize)
                {
                    evt.floatParameter = events.GetArrayElementAtIndex(index).FindPropertyRelative("floatParameter").floatValue;
                    evt.functionName = events.GetArrayElementAtIndex(index).FindPropertyRelative("functionName").stringValue;
                    evt.intParameter = events.GetArrayElementAtIndex(index).FindPropertyRelative("intParameter").intValue;
                    evt.objectReferenceParameter = events.GetArrayElementAtIndex(index).FindPropertyRelative("objectReferenceParameter").objectReferenceValue;
                    evt.stringParameter = events.GetArrayElementAtIndex(index).FindPropertyRelative("data").stringValue;
                    evt.time = events.GetArrayElementAtIndex(index).FindPropertyRelative("time").floatValue;
                }
                else
                {
                    Debug.LogWarning("Invalid Event Index");
                }
            }

            return evt;
        }

        public void SetEvent(int index, AnimationEvent animationEvent)
        {
            SerializedProperty events = Get("events");

            if (events != null && events.isArray)
            {
                if (index < events.arraySize)
                {
                    events.GetArrayElementAtIndex(index).FindPropertyRelative("floatParameter").floatValue = animationEvent.floatParameter;
                    events.GetArrayElementAtIndex(index).FindPropertyRelative("functionName").stringValue = animationEvent.functionName;
                    events.GetArrayElementAtIndex(index).FindPropertyRelative("intParameter").intValue = animationEvent.intParameter;
                    events.GetArrayElementAtIndex(index).FindPropertyRelative("objectReferenceParameter").objectReferenceValue = animationEvent.objectReferenceParameter;
                    events.GetArrayElementAtIndex(index).FindPropertyRelative("data").stringValue = animationEvent.stringParameter;
                    events.GetArrayElementAtIndex(index).FindPropertyRelative("time").floatValue = animationEvent.time;
                }

                else
                {
                    Debug.LogWarning("Invalid Event Index");
                }
            }
        }


        public void ClearEvents()
        {
            SerializedProperty events = Get("events");

            if (events != null && events.isArray)
            {
                events.ClearArray();
            }
        }

        public int GetEventCount()
        {
            int ret = 0;

            SerializedProperty curves = Get("events");

            if (curves != null && curves.isArray)
            {
                ret = curves.arraySize;
            }

            return ret;
        }

        public void SetEvents(AnimationEvent[] newEvents)
        {
            SerializedProperty events = Get("events");

            if (events != null && events.isArray)
            {
                events.ClearArray();

                foreach (AnimationEvent evt in newEvents)
                {
                    events.InsertArrayElementAtIndex(events.arraySize);
                    SetEvent(events.arraySize - 1, evt);
                }
            }
        }

        public AnimationEvent[] GetEvents()
        {
            AnimationEvent[] ret = new AnimationEvent[GetEventCount()];
            SerializedProperty events = Get("events");

            if (events != null && events.isArray)
            {
                for (int i = 0; i < GetEventCount(); ++i)
                {
                    ret[i] = GetEvent(i);
                }
            }

            return ret;

        }

    }

    #endregion
}
