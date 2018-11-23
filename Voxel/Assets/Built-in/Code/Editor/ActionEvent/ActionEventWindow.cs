using UnityEditor;
using UnityEngine;

namespace ActionEvent
{


    public class ActionEventWindow : EditorWindow
    {

        //显示模式
        enum Mode
        {
            ACTION_RES,
            SKILL_ACTION,
            ACTION_EVENT,
        }

        [SerializeField]
        Mode m_Mode;

        const float k_ToolbarPadding = 15;
        const float k_MenubarPadding = 32;

        //技能动作信息
        ActionEventServerInfo server_info;
        //action event 关键点信息
        ActionEventKeyPoint key_point;
        //资源信息
        ActionResInfo res_info;

        Texture2D m_RefreshTexture;

        [MenuItem("Window/ActionEvent Window", priority = 2050)]
        static void ShowWindow()
        {
            var window = GetWindow<ActionEventWindow>();
            window.titleContent = new GUIContent("ActionEvent");
            window.Show();
        }

        private void OnEnable()
        {

            Selection.selectionChanged += OnSelectChanged;

            if (res_info == null)
                res_info = new ActionResInfo();

            if (server_info == null)
                server_info = new ActionEventServerInfo();

            if (key_point == null)
                key_point = new ActionEventKeyPoint();

            m_RefreshTexture = EditorGUIUtility.FindTexture("Refresh");

            OnSelectChanged();
        }

        private Rect GetSubWindowArea()
        {
            float padding = k_MenubarPadding;
            Rect subPos = new Rect(0, padding, position.width, position.height - padding);
            return subPos;
        }

        private void Update()
        {
            switch (m_Mode)
            {
                case Mode.ACTION_RES:
                    break;
                case Mode.SKILL_ACTION:
                    break;
                case Mode.ACTION_EVENT:
                    break;
                default:
                    break;
            }
        }

        private void OnGUI()
        {
            ModeToggle();

            switch (m_Mode)
            {
                case Mode.ACTION_RES:
                    res_info.OnGUI(GetSubWindowArea());
                    break;
                case Mode.SKILL_ACTION:
                    server_info.OnGUI(GetSubWindowArea());
                    break;
                case Mode.ACTION_EVENT:
                    key_point.OnGUI(GetSubWindowArea());
                    break;
                default:
                    break;
            }
        }

        void ModeToggle()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(k_ToolbarPadding);
            float toolbarWidth = position.width - k_ToolbarPadding * 4;// - m_RefreshTexture.width;
            string[] labels = new string[3] { "Action_Res", "Skill_Action", "Action_Event" };

            bool refresh = GUILayout.Button(m_RefreshTexture);

            Mode m = (Mode)GUILayout.Toolbar((int)m_Mode, labels, "LargeButton", GUILayout.Width(toolbarWidth));
            if(m != m_Mode || refresh)
            {
                m_Mode = m;
                //显示页签改变
                switch (m_Mode)
                {
                    case Mode.ACTION_RES:
                        res_info.OnEnable(GetSubWindowArea(),this);
                        break;
                    case Mode.SKILL_ACTION:
                        server_info.OnEnable(GetSubWindowArea(), this);
                        break;
                    case Mode.ACTION_EVENT:
                        key_point.OnEnable(GetSubWindowArea(), this);
                        OnSelectChanged();
                        break;
                    default:
                        break;
                }
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        #region 监听选中对象改变

        string path;

        /// <summary>
        /// 选中对象改变
        /// </summary>
        void OnSelectChanged()
        {
            InitSelectObj(Selection.activeObject);
        }

        /// <summary>
        /// 初始化选中资源对象
        /// </summary>
        /// <param name="obj"></param>
        void InitSelectObj(Object obj)
        {
            if (m_Mode != Mode.ACTION_EVENT
                && m_Mode != Mode.ACTION_RES)
                return;

            path = AssetDatabase.GetAssetPath(obj);

            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (clip == null)
            {
                Debug.LogError("当前没有选中 动画 FBX 文件");
            }

            switch(m_Mode)
            {
                case Mode.ACTION_RES:
                    if (res_info != null)
                        res_info.OnSelectObjChanged(path);
                    break;
                case Mode.ACTION_EVENT:
                    if (key_point != null)
                        key_point.OnSelectObjChanged(clip, path);
                    break;
            }

        }

        //有改变的时候重新绘制窗口
        void OnInspectorUpdate()
        {
            this.Repaint();  //重新画窗口
        }

        /// <summary>
        /// 窗口获取焦点时
        /// </summary>
        void OnFocus()
        {
            EditorApplication.delayCall += UpdateSelect;   
        }

        void UpdateSelect()
        {
            EditorApplication.delayCall -= UpdateSelect;

            if (!string.IsNullOrEmpty(path) && Selection.activeObject == null)
                InitSelectObj(AssetDatabase.LoadAssetAtPath<AnimationClip>(path));
            else
                OnSelectChanged();

            OnInspectorUpdate();
        }

        #endregion
    }


}