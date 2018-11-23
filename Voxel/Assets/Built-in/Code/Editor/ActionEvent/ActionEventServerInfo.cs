using ActionEvent.Table;
using AFrame.EditorCommon;
using AFrame.Table;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ActionEvent
{

    public class ActionEventServerInfo
    {

        #region 数据初始化

        public ActionEventServerInfo()
        {
            
        }

        public void OnEnable(Rect pos, EditorWindow parent)
        {
            m_Parent = parent;

            if(view == null)
            {
                view = new TableView(parent, typeof(Table_Skill));
                view.AddColum("id", "Id", 0.1f, TextAnchor.MiddleLeft);
                view.AddColum("name_i18n", "Name", 0.3f);
                view.AddColum("pre_duration", "前摇时间", 0.15f);
                view.AddColum("cast_duration", "施法时间", 0.15f);
                view.AddColum("sing_duration", "吟唱时间", 0.15f);
                view.AddColum("post_duration", "后摇时间", 0.15f);

                view.onSelected += OnSelectChange;
            }

            //刷新数据信息

            //动作资源数据
            UpdateActionRes();

            //技能动作设置
            UpdateSkillAction();

            //客户端动作数据
            UpdateClientAction();

            //更新特效资源数据
            UpdateClientEffect();
        }

        /// <summary>
        /// 当前选中的显示列表中条目数据改变
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="colId"></param>
        void OnSelectChange(object obj, int colId)
        {
            if (obj == null)
                return;
            
            skill = (Table_Skill)obj;
            if(skill != null)
            {
                skill.pre_duration = GetKeyPointTime(skill.pre_action_res, ActionEventKeyPoint.Action_Event_Type.OnKeyPoint);
                skill.cast_duration = GetKeyPointTime(skill.cast_actioin_res, ActionEventKeyPoint.Action_Event_Type.OnKeyPoint);
                skill.sing_duration = GetKeyPointTime(skill.sing_action_res, ActionEventKeyPoint.Action_Event_Type.OnKeyPoint);
                skill.hurt_time = GetKeyPointTime(skill.sing_action_res, ActionEventKeyPoint.Action_Event_Type.OnHurtDelay);
                skill.shift_time = GetKeyPointTime(skill.sing_action_res, ActionEventKeyPoint.Action_Event_Type.OnDashSkillStartMove);
                skill.post_duration = GetKeyPointTime(skill.post_action_res, ActionEventKeyPoint.Action_Event_Type.OnKeyPoint);
				skill.close_time = GetKeyPointTime(skill.post_action_res, ActionEventKeyPoint.Action_Event_Type.OnBreakPoint);

				sing_have_hurt_point = skill.hurt_time > 0;
				if(!sing_have_hurt_point)
					skill.hurt_time = GetKeyPointTime(skill.post_action_res, ActionEventKeyPoint.Action_Event_Type.OnHurtDelay);

                sing_have_shift_point = skill.shift_time > 0;
                if (!sing_have_shift_point)
                    skill.shift_time = GetKeyPointTime(skill.post_action_res, ActionEventKeyPoint.Action_Event_Type.OnDashSkillStartMove);
            }
        }

        #endregion

        #region 数据信息获取及更新

        //是否正在获取服务器数据信息
        bool is_get_server_info = false;

        #region 获取动作资源配置

        //资源列表数据
        List<Table_Client_Action_Res> action_res_list;
        //资源列表数据显示
        string[] action_res_arr;
        //资源列表id数据显示
        int[] action_res_int_arr;

        /// <summary>
        /// 更新服务器资源数据
        /// </summary>
        public void UpdateActionRes()
        {
            string url = string.Format("{0}{1}{2}", ActionEventConfig.SERVER_URL, ActionEventConfig.SERVER_RES_URL, ActionEventConfig.LIST);
            //设置正在获取服务器数据
            is_get_server_info = true;

            EditorCoroutineRunner.StartEditorCoroutine(GetServerInfo(url, UpdateActionResDone));
        }

        /// <summary>
        /// 获取动作资源数据完成
        /// </summary>
        /// <param name="www"></param>
        void UpdateActionResDone(WWW www)
        {
            if (www == null
                || string.IsNullOrEmpty(www.text))
                return;

            if (action_res_list == null)
                action_res_list = new List<Table_Client_Action_Res>();
            action_res_list.Clear();

            string[] arr = www.text.Split(new char[] { '\n' });
            int count = arr.Length;
            for (int i = 0; i < count; i++)
            {
                if (string.IsNullOrEmpty(arr[i]))
                    continue;

                object obj = JsonUtility.FromJson(arr[i], typeof(Table_Client_Action_Res));
                if (obj == null)
                {
                    Debug.LogError("Table_Client_Action_Res Create Failure~ ");
                    continue;
                }

                Table_Client_Action_Res res = (Table_Client_Action_Res)obj;
                if (res != null)
                    action_res_list.Add(res);
            }

            count = action_res_list.Count;
            action_res_arr = new string[count];
            action_res_int_arr = new int[count];
            for (int i = 0; i < count; i++)
            {
                action_res_arr[i] = string.Format("Id:{0} Paht:{1} Remark:{2}", action_res_list[i].id, action_res_list[i].path, action_res_list[i].remark);
                action_res_int_arr[i] = action_res_list[i].id;
            }

            //获取服务器数据完成
            is_get_server_info = false;
        }

        #endregion

        #region 获取技能动作设置

        //资源列表数据
        List<Table_Skill> skill_action_list;

        /// <summary>
        /// 更新服务器资源数据
        /// </summary>
        public void UpdateSkillAction()
        {
            string url = string.Format("{0}{1}{2}", ActionEventConfig.SERVER_URL, ActionEventConfig.SERVER_SKILL_ACTION_URL, ActionEventConfig.LIST);
            //设置正在获取服务器数据
            is_get_server_info = true;

            EditorCoroutineRunner.StartEditorCoroutine(GetServerInfo(url, UpdateSkillActionDone));
        }

        /// <summary>
        /// 获取动作资源数据完成
        /// </summary>
        /// <param name="www"></param>
        void UpdateSkillActionDone(WWW www)
        {
            if (www == null
                || string.IsNullOrEmpty(www.text))
                return;

            if (skill_action_list == null)
                skill_action_list = new List<Table_Skill>();
            skill_action_list.Clear();

            string[] arr = www.text.Split(new char[] { '\n' });
            int count = arr.Length;
            for (int i = 0; i < count; i++)
            {
                if (string.IsNullOrEmpty(arr[i]))
                    continue;

                object obj = JsonUtility.FromJson(arr[i], typeof(Table_Skill));
                if (obj == null)
                {
                    Debug.LogError("Table_Skill_Action Create Failure~ ");
                    continue;
                }

                Table_Skill sa = (Table_Skill)obj;
                if (sa != null)
                    skill_action_list.Add(sa);
            }

            //获取服务器数据完成
            is_get_server_info = false;
        }

        #endregion

        #region 获取动作数据设置

        //客户端动作列表
        List<Table_Client_Action> client_action_list;
        //客户端动作列表显示
        string[] client_action_arr;
        //客户端动作id列表显示
        int[] client_action_id_arr;

        /// <summary>
        /// 更新服务器资源数据
        /// </summary>
        public void UpdateClientAction()
        {
            string url = string.Format("{0}{1}{2}", ActionEventConfig.SERVER_URL, ActionEventConfig.SERVER_CLIENT_ACTION_URL, ActionEventConfig.LIST);
            //设置正在获取服务器数据
            is_get_server_info = true;

            EditorCoroutineRunner.StartEditorCoroutine(GetServerInfo(url, UpdateClientActionDone));
        }

        /// <summary>
        /// 获取动作资源数据完成
        /// </summary>
        /// <param name="www"></param>
        void UpdateClientActionDone(WWW www)
        {
            if (www == null
                || string.IsNullOrEmpty(www.text))
                return;

            if (client_action_list == null)
                client_action_list = new List<Table_Client_Action>();
            client_action_list.Clear();

            string[] arr = www.text.Split(new char[] { '\n' });
            int count = arr.Length;
            for (int i = 0; i < count; i++)
            {
                if (string.IsNullOrEmpty(arr[i]))
                    continue;

                object obj = JsonUtility.FromJson(arr[i], typeof(Table_Client_Action));
                if (obj == null)
                {
                    Debug.LogError("Table_Client_Action Create Failure~ ");
                    continue;
                }

                Table_Client_Action sa = (Table_Client_Action)obj;
                if (sa != null)
                    client_action_list.Add(sa);
            }

            count = client_action_list.Count;
            client_action_arr = new string[count];
            client_action_id_arr = new int[count];

            for (int i = 0; i < count; i++)
            {
                client_action_arr[i] = string.Format("Id:{0} Name:{1}", client_action_list[i].id, client_action_list[i].name);
                client_action_id_arr[i] = client_action_list[i].id;
            }

            //获取服务器数据完成
            is_get_server_info = false;
        }

        #endregion

        #region 获取特效数据设置

        //客户端特效列表
        List<Table_Client_Effect> client_effect_list;
        //客户端特效列表显示
        string[] client_effect_arr;
        //客户端特效id列表显示
        int[] client_effect_id_arr;

        /// <summary>
        /// 更新服务器资源数据
        /// </summary>
        public void UpdateClientEffect()
        {
            string url = string.Format("{0}{1}{2}", ActionEventConfig.SERVER_URL, ActionEventConfig.SERVER_CLIENT_EFFECT_URL, ActionEventConfig.LIST);
            //设置正在获取服务器数据
            is_get_server_info = true;

            EditorCoroutineRunner.StartEditorCoroutine(GetServerInfo(url, UpdateClientEffectDone));
        }

        /// <summary>
        /// 获取动作资源数据完成
        /// </summary>
        /// <param name="www"></param>
        void UpdateClientEffectDone(WWW www)
        {
            if (www == null
                || string.IsNullOrEmpty(www.text))
                return;

            if (client_effect_list == null)
                client_effect_list = new List<Table_Client_Effect>();
            client_effect_list.Clear();

            string[] arr = www.text.Split(new char[] { '\n' });
            int count = arr.Length;
            for (int i = 0; i < count; i++)
            {
                if (string.IsNullOrEmpty(arr[i]))
                    continue;

                object obj = JsonUtility.FromJson(arr[i], typeof(Table_Client_Effect));
                if (obj == null)
                {
                    Debug.LogError("Table_Client_Effect Create Failure~ ");
                    continue;
                }

                Table_Client_Effect sa = (Table_Client_Effect)obj;
                if (sa != null)
                    client_effect_list.Add(sa);
            }

			count = client_effect_list.Count;
            client_effect_arr = new string[count];
            client_effect_id_arr = new int[count];

            for (int i = 0; i < count; i++)
            {
				client_effect_arr[i] = string.Format("Id:{0} Name:{1}", client_effect_list[i].id, client_effect_list[i].name);
				client_effect_id_arr[i] = client_effect_list[i].id;
            }

            //获取服务器数据完成
            is_get_server_info = false;
        }

        #endregion

        #endregion

        #region 数据显示

        EditorWindow m_Parent = null;

        TableView view;

        int list_width = 15;
        int list_height = 15;
        int rect_offset = 5;

        //当前选择的 skill 
        Table_Skill skill;

        public void OnGUI(Rect rect)
        {
            if (action_res_list == null
                || action_res_list.Count == 0
                || skill_action_list == null
                || skill_action_list.Count == 0)
                return;

            ShowResList(rect);
        }

        Vector2 res_scroll;
        int select_indexId;
        //吟唱阶段是否有伤害点数据
        bool sing_have_hurt_point = false;
        //吟唱阶段是否有位移点数据
        bool sing_have_shift_point = false;

        /// <summary>
        /// 显示资源列表信息数据
        /// </summary>
        void ShowResList(Rect pos)
        {

            //left
            Rect rect = new Rect(pos.x + list_width, pos.y + list_height, pos.width / 2 - list_width * 2 - rect_offset, pos.height - list_height * 2);
            GUILayout.BeginArea(rect, EditorConst.WindowBackground);
            {
                if (view != null)
                {
                    view.Draw(new Rect(0, 0, rect.width, rect.height));
                    view.RefreshData(EditorTool.ToObjectList(skill_action_list));
                }
            }
            GUILayout.EndArea();

            //right
            rect = new Rect(pos.width / 2 - list_width, pos.y + list_height, pos.width / 2 - list_width * 2 - rect_offset, pos.height - list_height * 2);

            GUILayout.BeginArea(rect, EditorConst.WindowBackground);
            {
                res_scroll = EditorGUILayout.BeginScrollView(res_scroll);

                if (skill != null)
                {
                    //技能动作数据
                    EditorGUILayout.LabelField("Id", skill.id.ToString());

                    skill.name_i18n = EditorGUILayout.TextField("Name", skill.name_i18n);

                    EditorGUILayout.Separator();

                    //绘制伤害信息数据（关联吟唱动作）
                    DrawHurtInfo();
                    //关联 吟唱动作
                    EditorGUILayout.LabelField("位移开始时间(ms)", skill.shift_time.ToString());
                    skill.no_move_duration = EditorGUILayout.IntField("不可移动时间(ms)", skill.no_move_duration);

                    EditorGUILayout.Separator();

                    //绘制受击信息数据
                    DrawTargetInfo();

                    //前摇数据
                    DrawPreInfo();

                    //施法数据（单次，不循环）
                    DrawCostInfo();

                    //吟唱数据（循环）
                    DrawSingInfo();

                    //后摇数据
                    DrawPostInfo();

                    //子弹数据
                    DrawBulletInfo();
                }

                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Add"))
                {
                    Add();
                }

                if (GUILayout.Button("Delete"))
                {
                    Delete();
                }

                if (GUILayout.Button("Save"))
                {
                    Save();
                }

                GUILayout.EndHorizontal();

                EditorGUILayout.EndScrollView();

            }
            GUILayout.EndArea();
        }

        /// <summary>
        /// 绘制伤害信息数据
        /// </summary>
        void DrawHurtInfo()
        {
            EditorGUILayout.LabelField("伤害点(ms)", skill.hurt_time.ToString());
            skill.hurt_interval = EditorGUILayout.IntField("伤害触发间隔(ms)", skill.hurt_interval);
        }

        /// <summary>
        /// 绘制受击数据信息
        /// </summary>
        void DrawTargetInfo()
        {
            skill.target_action = EditorGUILayout.IntField("受击动作Id", skill.target_action);
            skill.target_action = EditorGUILayout.IntPopup("受击动作Id", skill.target_action, client_action_arr, client_action_id_arr);

            skill.target_effect = EditorGUILayout.IntField("受击特效Id", skill.target_effect);
            skill.target_effect = EditorGUILayout.IntPopup("受击特效Id", skill.target_effect, client_effect_arr, client_effect_id_arr);
        }

        /// <summary>
        /// 绘制前摇信息
        /// </summary>
        void DrawPreInfo()
        {
            EditorGUILayout.Separator();

            skill.pre_action = EditorGUILayout.IntField("前摇动作Id", skill.pre_action);
            skill.pre_action = EditorGUILayout.IntPopup("前摇动作Id", skill.pre_action, client_action_arr, client_action_id_arr);

            int res1 = EditorGUILayout.IntField("前摇动作资源Id", skill.pre_action_res);
            int res2 = EditorGUILayout.IntPopup("前摇动作资源Id", skill.pre_action_res, action_res_arr, action_res_int_arr);

            if (skill.pre_action_res >= 0)
            {
                if (res1 != skill.pre_action_res
                    || res2 != skill.pre_action_res)
                {
                    if (res1 != skill.pre_action_res)
                        skill.pre_action_res = res1;
                    else
                        skill.pre_action_res = res2;
                    skill.pre_duration = GetKeyPointTime(skill.pre_action_res, ActionEventKeyPoint.Action_Event_Type.OnKeyPoint);
                }
                EditorGUILayout.LabelField("前摇时间(ms)", skill.pre_duration.ToString());
            }

            skill.pre_effect = EditorGUILayout.IntField("前摇特效Id", skill.pre_effect);
            skill.pre_effect = EditorGUILayout.IntPopup("前摇特效Id", skill.pre_effect, client_effect_arr, client_effect_id_arr);

            EditorGUILayout.Separator();
        }

        /// <summary>
        /// 绘制施法信息（单次不循环）
        /// </summary>
        void DrawCostInfo()
        {
            EditorGUILayout.Separator();

            skill.cast_action = EditorGUILayout.IntField("施法动作Id", skill.cast_action);
            skill.cast_action = EditorGUILayout.IntPopup("施法动作Id", skill.cast_action, client_action_arr, client_action_id_arr);

            int res1 = EditorGUILayout.IntField("施法动作资源Id", skill.cast_actioin_res);
            int res2 = EditorGUILayout.IntPopup("施法动作资源Id", skill.cast_actioin_res, action_res_arr, action_res_int_arr);

            if (skill.cast_actioin_res >= 0)
            {
                if (res1 != skill.cast_actioin_res
                    || res2 != skill.cast_actioin_res)
                {
                    if (res1 != skill.cast_actioin_res)
                        skill.cast_actioin_res = res1;
                    else
                        skill.cast_actioin_res = res2;
                    skill.cast_duration = GetKeyPointTime(skill.cast_actioin_res, ActionEventKeyPoint.Action_Event_Type.OnKeyPoint);
                }
                EditorGUILayout.LabelField("施法时间(ms)", skill.cast_duration.ToString());
            }

            skill.cast_effect = EditorGUILayout.IntField("施法特效Id", skill.cast_effect);
            skill.cast_effect = EditorGUILayout.IntPopup("施法特效Id", skill.cast_effect, client_effect_arr, client_effect_id_arr);

            EditorGUILayout.Separator();
        }

        /// <summary>
        /// 绘制吟唱信息（循环）
        /// </summary>
        void DrawSingInfo()
        {
            EditorGUILayout.Separator();

            skill.sing_action = EditorGUILayout.IntField("吟唱动作Id", skill.sing_action);
            skill.sing_action = EditorGUILayout.IntPopup("吟唱动作Id", skill.sing_action, client_action_arr, client_action_id_arr);

            int res1 = EditorGUILayout.IntField("吟唱动作资源Id", skill.sing_action_res);
            int res2 = EditorGUILayout.IntPopup("吟唱动作资源Id", skill.sing_action_res, action_res_arr, action_res_int_arr);

            if (skill.sing_action_res >= 0)
            {
                if (res1 != skill.sing_action_res
                    || res2 != skill.sing_action_res)
                {
                    if (res1 != skill.sing_action_res)
                        skill.sing_action_res = res1;
                    else
                        skill.sing_action_res = res2;
                    skill.sing_duration = GetKeyPointTime(skill.sing_action_res, ActionEventKeyPoint.Action_Event_Type.OnKeyPoint);
                    skill.shift_time = GetKeyPointTime(skill.sing_action_res, ActionEventKeyPoint.Action_Event_Type.OnDashSkillStartMove);
                    skill.hurt_time = GetKeyPointTime(skill.sing_action_res, ActionEventKeyPoint.Action_Event_Type.OnHurtDelay);

                    sing_have_hurt_point = skill.hurt_time > 0;
                    sing_have_shift_point = skill.shift_time > 0;
                }
                EditorGUILayout.LabelField("吟唱时间(ms)", skill.sing_duration.ToString());
            }

            skill.sing_effect = EditorGUILayout.IntField("吟唱特效Id", skill.sing_effect);
            skill.sing_effect = EditorGUILayout.IntPopup("吟唱特效Id", skill.sing_effect, client_effect_arr, client_effect_id_arr);

            EditorGUILayout.Separator();
        }

        /// <summary>
        /// 绘制后摇信息
        /// </summary>
        void DrawPostInfo()
        {
            EditorGUILayout.Separator();

            skill.post_action = EditorGUILayout.IntField("后摇动作Id", skill.post_action);
            skill.post_action = EditorGUILayout.IntPopup("后摇动作Id", skill.post_action, client_action_arr, client_action_id_arr);

            int res1 = EditorGUILayout.IntField("后摇动作资源Id", skill.post_action_res);
            int res2 = EditorGUILayout.IntPopup("后摇动作资源Id", skill.post_action_res, action_res_arr, action_res_int_arr);

            if (skill.post_action_res >= 0)
            {
                if (res1 != skill.post_action_res
                    || res2 != skill.post_action_res)
                {
                    if (res1 != skill.post_action_res)
                        skill.post_action_res = res1;
                    else
                        skill.post_action_res = res2;
					skill.hurt_time = GetKeyPointTime(skill.post_action_res, ActionEventKeyPoint.Action_Event_Type.OnHurtDelay);
                    skill.post_duration = GetKeyPointTime(skill.post_action_res, ActionEventKeyPoint.Action_Event_Type.OnKeyPoint);
                    skill.close_time = GetKeyPointTime(skill.post_action_res, ActionEventKeyPoint.Action_Event_Type.OnBreakPoint);
                    if(!sing_have_hurt_point)
                        skill.hurt_time = GetKeyPointTime(skill.post_action_res, ActionEventKeyPoint.Action_Event_Type.OnHurtDelay);

                    if(!sing_have_shift_point)
                        skill.shift_time = GetKeyPointTime(skill.post_action_res, ActionEventKeyPoint.Action_Event_Type.OnDashSkillStartMove);
                }
                EditorGUILayout.LabelField("后摇时间(ms)", skill.post_duration.ToString());
                EditorGUILayout.LabelField("收招点时间[不可打断时间点](ms)", skill.close_time.ToString());
            }

            skill.post_effect = EditorGUILayout.IntField("后摇特效Id", skill.post_effect);
            skill.post_effect = EditorGUILayout.IntPopup("后摇特效Id", skill.post_effect, client_effect_arr, client_effect_id_arr);

            EditorGUILayout.Separator();
        }

        /// <summary>
        /// 绘制子弹信息
        /// </summary>
        void DrawBulletInfo()
        {
            EditorGUILayout.Separator();

            skill.bullet_effect = EditorGUILayout.IntField("子弹特效Id", skill.bullet_effect);
            skill.bullet_effect = EditorGUILayout.IntPopup("子弹特效Id", skill.bullet_effect, client_effect_arr, client_effect_id_arr);

            //if(sa.bullet_effect > 0)
            //    sa.bullet_speed = EditorGUILayout.FloatField("子弹特效飞行速度", sa.bullet_speed);

            EditorGUILayout.Separator();
        }

        /// <summary>
        /// 获取Animation Event 时间
        /// </summary>
        /// <param name="res_id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        int GetKeyPointTime(int res_id, ActionEventKeyPoint.Action_Event_Type type)
        {
            if (res_id <= 0
                || action_res_list == null
                || action_res_list.Count == 0)
                return 0;

            string path = null;
            int count = action_res_list.Count;
            for (int i = 0; i < count; i++)
            {
                if (action_res_list[i].id == res_id)
                {
                    path = action_res_list[i].path;
                    break;
                }
            }

            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(string.Format("{0}{1}", ActionEventConfig.RES_START_PATH, path));
            if (clip == null)
                return 0;

            AnimationEvent[] event_arr = AnimationUtility.GetAnimationEvents(clip);
            count = event_arr.Length;
            string str = type.ToString();
            for (int i = 0; i < count; i++)
            {
                if (string.Equals(event_arr[i].functionName, str))
                    return (int)(event_arr[i].time * 1000);
            }

            return 0;
        }

        /// <summary>
        /// 新加数据
        /// </summary>
        void Add()
        {
            skill = new Table_Skill();
            if (skill_action_list == null)
                skill_action_list = new List<Table_Skill>();

            int count = skill_action_list.Count;
            if (count > 0)
                skill.id = skill_action_list[count - 1].id + 1;
            else
                skill.id = 1;

            skill.name_i18n = "";
            skill.no_move_duration = -1;
            skill_action_list.Add(skill);

            //添加消息数据
            SendServerInfo(skill, UpdateServerInfoEnum.INSERT);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        void Delete()
        {
            if (skill_action_list != null && skill != null)
            {
                skill_action_list.Remove(skill);
                SendServerInfo(skill, UpdateServerInfoEnum.DELETE);
                skill = null;
            }
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        void Save()
        {
            if (skill == null)
                return;

            if(string.IsNullOrEmpty(skill.name_i18n))
            {
                EditorUtility.DisplayDialog("提示", "Name 不能为 空", "确认", "取消");
                return;
            }

            SendServerInfo(skill, UpdateServerInfoEnum.UPDATE);
        }

        #endregion

        #region 获取服务器信息数据

        /// <summary>
        /// 获取服务器资源数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        IEnumerator GetServerInfo(string url, Action<WWW> action)
        {
            if (string.IsNullOrEmpty(url))
            {
                if (action != null)
                {
                    action(null);
                }
            }

            WWW www = new WWW(url);

            yield return www;

            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.LogError("资源获取失败 " + url + "   \n" + www.error);
            }

            if (www.isDone)
            {
                if (action != null)
                {
                    action(www);
                }
            }
        }



        #endregion

        #region 更新服务器信息数据

        /// <summary>
        /// 更新服务器数据类型
        /// </summary>
        enum UpdateServerInfoEnum
        {
            LIST,                                   //列表数据
            INSERT,                                 //插入数据
            UPDATE,                                 //更新数据
            DELETE,                                 //删除数据
        }

        void SendServerInfo(Table_Skill skill, UpdateServerInfoEnum type)
        {
            string t = GetServerInfoType(type);
            string url = string.Format("{0}{1}{2}", ActionEventConfig.SERVER_URL, ActionEventConfig.SERVER_SKILL_ACTION_URL, t);

            switch(type)
            {
                case UpdateServerInfoEnum.INSERT:
                case UpdateServerInfoEnum.UPDATE:
                    EditorCoroutineRunner.StartEditorCoroutine(SendInfo(url, SendUpdateInfo(skill)));
                    break;
                case UpdateServerInfoEnum.DELETE:
                    EditorCoroutineRunner.StartEditorCoroutine(SendInfo(url, SendDeleteInfo(skill.id)));
                    break;
            }
        }

        /// <summary>
        /// 获取服务器信息数据类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        string GetServerInfoType(UpdateServerInfoEnum type)
        {
            string t = null;
            switch (type)
            {
                case UpdateServerInfoEnum.LIST:
                    t = ActionEventConfig.LIST;
                    break;
                case UpdateServerInfoEnum.INSERT:
                    t = ActionEventConfig.INSERT;
                    break;
                case UpdateServerInfoEnum.UPDATE:
                    t = ActionEventConfig.UPDATE;
                    break;
                case UpdateServerInfoEnum.DELETE:
                    t = ActionEventConfig.DELETE;
                    break;
            }
            return t;
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sa"></param>
        WWWForm SendUpdateInfo(Table_Skill sa)
        {
            WWWForm wf = new WWWForm();

            wf.AddField("id", sa.id);
            wf.AddField("name", sa.name_i18n);

            wf.AddField("hurt_time", sa.hurt_time);
            wf.AddField("hurt_interval", sa.hurt_interval);
            wf.AddField("close_time", sa.close_time);
            wf.AddField("shift_time", sa.shift_time);

            wf.AddField("pre_duration", sa.pre_duration);
            wf.AddField("pre_action", sa.pre_action);
            wf.AddField("pre_action_res", sa.pre_action_res);
            wf.AddField("pre_effect", sa.pre_effect);

            wf.AddField("cast_duration", sa.cast_duration);
            wf.AddField("cast_action", sa.cast_action);
            wf.AddField("cast_actioin_res", sa.cast_actioin_res);
            wf.AddField("cast_effect", sa.cast_effect);

            wf.AddField("sing_duration", sa.sing_duration);
            wf.AddField("sing_action", sa.sing_action);
            wf.AddField("sing_action_res", sa.sing_action_res);
            wf.AddField("sing_effect", sa.sing_effect);

            wf.AddField("post_duration", sa.post_duration);
            wf.AddField("post_action", sa.post_action);
            wf.AddField("post_action_res", sa.post_action_res);
            wf.AddField("post_effect", sa.post_effect);

            wf.AddField("target_action", sa.target_action);
            wf.AddField("target_effect", sa.target_effect);

            wf.AddField("bullet_effect", sa.bullet_effect);
            //wf.AddField("bullet_speed", sa.bullet_speed.ToString());

            return wf;
        }

        WWWForm SendDeleteInfo(int id)
        {
            if (id <= 0)
                return null;

            WWWForm wf = new WWWForm();
            wf.AddField("id", id);

            return wf;
        }

        /// <summary>
        /// 向服务器发送数据信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="action"></param>
        /// <param name="parameter"></param>
        IEnumerator SendInfo(string url, WWWForm form)
        {
            WWW www = new WWW(url, form);

            yield return www;
            if (www.isDone)
            {
                Debug.LogError(" 服务器数据修改完成 ");
            }
        }

        #endregion
    }

}
