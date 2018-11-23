using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

namespace AnimatorTool
{
    /// <summary>
    /// AnimatorController copy 工具
    /// @author LiuLeiLei
    /// @data 5/8/2018
    /// @desc 
    /// </summary>
    public class AnimatorCopyWindow : EditorWindow
    {
        private static AnimatorCopyWindow window;

        [MenuItem("AFrame/AnimatorControllerCopy")]
        public static void OpenWindow()
        {
            if (window == null)
            {
                window = GetWindow<AnimatorCopyWindow>();

                window.titleContent = new GUIContent("CopyController");

                window.minSize = new Vector2(300, 150);
            }
        }

        /// <summary>
        /// AnimatorController 复制管理类
        /// </summary>
        private AnimatorCopy animatorCopy;

        /// <summary>
        /// 模板controller
        /// </summary>
        private AnimatorController template;

        private AnimatorController lastTemplate;

        private AnimatorController lastCopy;

        private SerializedObject serializedObj;

        private UnityEditorInternal.ReorderableList reorderableList;

        /// <summary>
        /// 要显示的state list
        /// </summary>
        private List<State> stateList = new List<State>();

        /// <summary>
        /// State 动作文件名前缀
        /// </summary>
        private int statePrePathIndex;

        /// <summary>
        /// 视图宽度一半
        /// </summary>
        private float halfViewWidth;
        /// <summary>
        /// 视图高度一半
        /// </summary>
        private float halfViewHeight;

        private Vector2 scroll;

        private void OnEnable()
        {
            animatorCopy = new AnimatorCopy();
            animatorCopy.OnGetConfig();

            serializedObj = new SerializedObject(this);

            reorderableList = new UnityEditorInternal.ReorderableList(stateList, typeof(State), true, true, false, false);

            reorderableList.drawHeaderCallback = DrawHeaderCallBack;
            reorderableList.drawElementCallback = DrawElementCallBack;
            reorderableList.drawElementBackgroundCallback = DrawElementBackgroundCallBack;
        }

        private void OnGUI()
        {
            serializedObj.Update();

            if (window == null)
                window = GetWindow<AnimatorCopyWindow>();

            halfViewWidth = EditorGUIUtility.currentViewWidth / 2f;
            halfViewHeight = window.position.height / 2f;

            using (new EditorGUILayout.HorizontalScope())
            {
                using (EditorGUILayout.VerticalScope vScope = new EditorGUILayout.VerticalScope(GUILayout.Width(EditorGUIUtility.currentViewWidth)))
                {
                    GUI.backgroundColor = UnityEngine.Color.white;
                    Rect rect = vScope.rect;
                    rect.height = window.position.height;
                    GUI.Box(rect, "");

                    //模板区域UI绘制
                    DrawTemplateUI();
                    //绘制元素列表
                    DrawList(rect);
                }

                //using (new EditorGUILayout.VerticalScope(GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.5f)))
                //{

                //}
            }

            serializedObj.ApplyModifiedProperties();
        }

        /// <summary>
        /// 模板区域绘制
        /// </summary>
        private void DrawTemplateUI()
        {
            EditorGUILayout.Space();

            using (EditorGUILayout.VerticalScope vScope = new EditorGUILayout.VerticalScope())
            {
                using (EditorGUILayout.HorizontalScope hScope = new EditorGUILayout.HorizontalScope())
                {
                    GUI.backgroundColor = UnityEngine.Color.white;
                    Rect rect = hScope.rect;
                    rect.height = EditorGUIUtility.singleLineHeight;
                    GUI.Box(rect, "");

                    EditorGUILayout.LabelField(new GUIContent("选择模板Controller:", "Drag refer AnimatorController to template field"), GUILayout.Width(halfViewWidth / 3f));

                    //template
                    template = EditorGUILayout.ObjectField(template, typeof(AnimatorController), true) as AnimatorController;

                    if (template != null && lastTemplate != template)
                    {
                        lastTemplate = template;

                        //选择一个路径
                        var copyPath = OnCopyPath();

                        if (!string.IsNullOrEmpty(copyPath))
                        {
                            animatorCopy.Clear();

                            animatorCopy.CopyAnimator(template, copyPath);
                        }
                    }
                }

                using (EditorGUILayout.HorizontalScope hScope = new EditorGUILayout.HorizontalScope())
                {
                    GUI.backgroundColor = UnityEngine.Color.white;
                    Rect rect = hScope.rect;
                    rect.height = EditorGUIUtility.singleLineHeight;
                    GUI.Box(rect, "");

                    EditorGUILayout.LabelField(new GUIContent("Copy Controller:", "也可以拖拽一个Controller进行动作文件替换"), GUILayout.Width(halfViewWidth / 3f));

                    //copy
                    animatorCopy.copy = EditorGUILayout.ObjectField(animatorCopy.copy, typeof(AnimatorController), true) as AnimatorController;

                    if (lastCopy != animatorCopy.copy)
                    {
                        lastCopy = animatorCopy.copy;

                        if (animatorCopy.copy != null)
                        {
                            //获取所有的state
                            animatorCopy.OnGetStates(animatorCopy.copy);

                            //刷新state list 显示数据
                            stateList = animatorCopy.GetAllStateList();
                        }
                        else
                        {
                            stateList.Clear();
                        }

                        reorderableList.list = stateList;
                    }
                }

                using (EditorGUILayout.HorizontalScope hScope = new EditorGUILayout.HorizontalScope())
                {
                    GUI.backgroundColor = UnityEngine.Color.white;
                    Rect rect = hScope.rect;
                    rect.height = EditorGUIUtility.singleLineHeight;
                    GUI.Box(rect, "");

                    GUILayout.Label(new GUIContent("前缀名称: ", "所有动作前缀名称"), GUILayout.Width(halfViewWidth / 3f));

                    statePrePathIndex = EditorGUILayout.Popup(statePrePathIndex, animatorCopy.prePathList.ToArray(), EditorStyles.toolbarPopup, GUILayout.Width(halfViewWidth / 3f));

                    if (GUILayout.Button(new GUIContent("一键关联动作文件", "根据StatePrePath和AnimatorState名进行查找并关联"), EditorStyles.toolbarButton))
                    {
                        if (animatorCopy.copy != null)
                        {
                            if (animatorCopy.prePathList.Count > 0)
                            {
                                animatorCopy.SetAllStateMotion(statePrePathIndex);
                            }
                            else
                            {
                                Debug.LogError("请检查配置文件，获取不到前缀名");
                            }
                        }
                        else
                        {
                            Debug.LogError("目标Controller为空");
                        }
                    }

                    if (GUILayout.Button(new GUIContent("一键移除动作文件", "所有动作文件包括BlendTree下的motion都会移除"), EditorStyles.toolbarButton))
                    {
                        if (animatorCopy.copy != null)
                        {
                            animatorCopy.RemoveMotion();
                        }
                    }

                    if (GUILayout.Button(new GUIContent("输出动作文件表", "根据StatePrePath和AnimatorState名进行输出"), EditorStyles.toolbarButton))
                    {
                        if (animatorCopy.copy != null)
                        {
                            //选择一个路径
                            var statePath = OnOutputConfigPath();

                            if (!string.IsNullOrEmpty(statePath))
                            {
                                if (animatorCopy.prePathList.Count > 0)
                                {
                                    animatorCopy.OutputConfig(statePath, statePrePathIndex);
                                }
                            }
                        }
                        else
                        {
                            Debug.LogError("目标Controller为空");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 绘制state 显示列表
        /// </summary>
        private void DrawList(Rect _rect)
        {
            EditorGUILayout.Space();

            scroll = GUILayout.BeginScrollView(scroll, GUILayout.MaxHeight(_rect.height));
            reorderableList.DoLayoutList();
            GUILayout.EndScrollView();
        }

        /// <summary>
        /// 绘制列表标题栏
        /// </summary>
        /// <param name="_rect"></param>
        private void DrawHeaderCallBack(Rect _rect)
        {
            GUI.Label(_rect, "State List", EditorStyles.label);
        }

        /// <summary>
        /// 绘制单个element
        /// </summary>
        /// <param name="_rect"></param>
        /// <param name="_index"></param>
        /// <param name="_isActive"></param>
        /// <param name="_isFocused"></param>
        private void DrawElementCallBack(Rect _rect, int _index, bool _isActive, bool _isFocused)
        {
            _rect.y += 2;
            _rect.height = EditorGUIUtility.singleLineHeight;

            State state = stateList[_index];

            _rect.xMax = _rect.xMax / 10.0f;
            GUI.Label(_rect, state.LayerName, EditorStyles.label);
            _rect.xMin = _rect.xMax;
            _rect.xMax *= 10.0f;

            _rect.xMax = _rect.xMax / 2.5f;
            GUI.Label(_rect, state.StatePath);
            _rect.xMin = _rect.xMax;
            _rect.xMax *= 2.5f;

            if (state.value.GetType() == typeof(AnimatorState))
            {
                _rect.xMax = _rect.xMax / 1.5f;

                EditorGUI.ObjectField(_rect, state.value, typeof(AnimatorState), false);

                _rect.xMin = _rect.xMax;
                _rect.xMax *= 1.5f;

                AnimatorState animatorState = (AnimatorState)state.value;

                animatorState.motion = EditorGUI.ObjectField(_rect, animatorState.motion, typeof(Motion), false) as Motion;
            }
            else
            {
                _rect.xMax = _rect.xMax / 1.5f;

                EditorGUI.ObjectField(_rect, state.value, typeof(BlendTree), false);

                _rect.xMin = _rect.xMax;
                _rect.xMax *= 1.5f;

                BlendTree blendTree = (BlendTree)state.value;

                int index = int.Parse((state.StatePath.Substring(state.StatePath.Length - 3, 3)).Split('[', ']')[1]);

                if (index < blendTree.children.Length)
                {
                    var motion = EditorGUI.ObjectField(_rect, blendTree.children[index].motion, typeof(Motion), false) as Motion;

                    if (motion != blendTree.children[index].motion)
                    {
                        AnimatorCopy.OverrideBlendTree(blendTree, index, motion);
                    }
                }
            }
        }

        /// <summary>
        /// 单个element背景绘制回调
        /// </summary>
        /// <param name="_rect"></param>
        /// <param name="_index"></param>
        /// <param name="_isActive"></param>
        /// <param name="_isFocused"></param>
        private void DrawElementBackgroundCallBack(Rect _rect, int _index, bool _isActive, bool _isFocused)
        {
            if (_index >= 0 && _index < stateList.Count)
            {
                State state = stateList[_index];
                if (state.value.GetType() == typeof(AnimatorState))
                {
                    if (((AnimatorState)state.value).motion == null)
                    {
                        GUI.backgroundColor = UnityEngine.Color.red;
                    }
                    else
                    {
                        GUI.backgroundColor = UnityEngine.Color.green;
                    }
                }
                else
                {
                    BlendTree blendTree = (BlendTree)state.value;

                    int index = int.Parse((state.StatePath.Substring(state.StatePath.Length - 3, 3)).Split('[', ']')[1]);

                    if (index < blendTree.children.Length)
                    {
                        if (blendTree.children[index].motion == null)
                        {
                            GUI.backgroundColor = UnityEngine.Color.red;
                        }
                        else
                        {
                            GUI.backgroundColor = UnityEngine.Color.cyan;
                        }
                    }
                    else
                    {
                        GUI.backgroundColor = UnityEngine.Color.red;
                    }
                }
            }
        }

        /// <summary>
        /// 路径
        /// </summary>
        /// <returns></returns>
        private string OnCopyPath()
        {
            return EditorUtility.SaveFilePanelInProject("AnimatorController Copy", "NewAnimatorController", "controller", "复制AnimatorController");
        }

        /// <summary>
        /// State对应动作文件表路径
        /// </summary>
        /// <returns></returns>
        private string OnOutputConfigPath()
        {
            return EditorUtility.SaveFilePanelInProject("AnimatorState.action output", animatorCopy.copy.name, "csv", "输出AnimatorState对应动作文件表");
        }
    }
}