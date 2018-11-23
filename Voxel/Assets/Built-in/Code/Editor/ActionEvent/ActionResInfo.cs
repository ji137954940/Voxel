using ActionEvent.Table;
using AFrame.EditorCommon;
using AFrame.Table;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ActionEvent
{
    public class ActionResInfo
    {
        EditorWindow m_Parent = null;

        TableView view;

        int list_width = 15;
        int list_height = 15;
        int rect_offset = 5;

        string path;

        Table_Client_Action_Res ar;


        public void OnEnable(Rect pos, EditorWindow parent)
        {
            m_Parent = parent;

            if (view == null)
            {
                view = new TableView(parent, typeof(Table_Client_Action_Res));
                view.AddColum("id", "Id", 0.1f);
                view.AddColum("path", "Path", 0.5f);
                view.AddColum("remark", "Remark", 0.4f);

                view.onSelected += OnSelectChange;
            }

            //获取动作资源数据
            UpdateActionRes();
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

            ar = (Table_Client_Action_Res)obj;
            if(ar != null
                && !string.IsNullOrEmpty(ar.path))
            {
                select_res = AssetDatabase.LoadAssetAtPath<GameObject>(string.Format("{0}{1}", ActionEventConfig.RES_START_PATH, ar.path));
                //设置选中目标
                if (select_res != null)
                    Selection.activeObject = select_res;
            }
        }

        /// <summary>
        /// 当前选中的对象已经改变
        /// </summary>
        public void OnSelectObjChanged(string path)
        {
            this.path = path;
        }

        #region 获取动作资源配置

        //资源列表字典数据
        Dictionary<string, Table_Client_Action_Res> action_res_dic;
        //资源列表数据
        List<Table_Client_Action_Res> action_res_list;
        //资源列表数据显示
        string[] action_res_arr;
        //资源列表id数据显示
        int[] action_res_int_arr;

        //是否正在获取服务器数据信息
        bool is_get_server_info = false;

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

            if (action_res_dic == null)
                action_res_dic = new Dictionary<string, Table_Client_Action_Res>();
            action_res_dic.Clear();

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
                {
                    action_res_list.Add(res);
                    action_res_dic[res.path] = res;
                }
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

        #region 显示绘制


        public void OnGUI(Rect rect)
        {
            if (is_get_server_info
                || action_res_list == null
                || action_res_list.Count == 0)
                return;

            ShowResList(rect);
        }

        Vector2 res_scroll;
        int select_indexId;

        GameObject select_res;

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
                    view.RefreshData(EditorTool.ToObjectList(action_res_list));
                }
            }
            GUILayout.EndArea();

            //right
            rect = new Rect(pos.width / 2 - list_width, pos.y + list_height, +pos.width / 2 - list_width * 2 - rect_offset, pos.height - list_height * 2);

            GUILayout.BeginArea(rect, EditorConst.WindowBackground);
            {

                if(ar != null)
                {
                    EditorGUILayout.LabelField("Id", ar.id.ToString());
                    ar.path = EditorGUILayout.TextField("资源路径", ar.path);

                    GameObject go = (GameObject)EditorGUILayout.ObjectField("资源数据", select_res, typeof(GameObject), true);
                    if(go != select_res)
                    {
                        string path = AssetDatabase.GetAssetPath(go);
                        AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
                        if(clip != null)
                        {
                            select_res = go;
                            ar.path = path.Remove(0, ActionEventConfig.RES_START_PATH.Length);
                        }
                        else
                        {
                            //选中目标不是 包含 动画文件的 FBX
                            EditorUtility.DisplayDialog("提示", "选中对象不包含 AnimationClip 文件", "确认", "取消");
                        }
                        
                    }

                    ar.remark = EditorGUILayout.TextField("备注描述", ar.remark, GUILayout.MinHeight(50));

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

            }
            GUILayout.EndArea();
        }

        /// <summary>
        /// 新加数据
        /// </summary>
        void Add()
        {
            UnityEngine.Object go = Selection.activeObject;
            if(go == select_res || go == null)
            {
                ar = GetNewActionRes("");

                //添加消息数据
                SendServerInfo(ar, UpdateServerInfoEnum.INSERT);
            }
            else
            {
                List<string> list = new List<string>();
                UnityEngine.Object[] objs = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
                int count = objs.Length;
                for (int i = 0; i < count; i++)
                {
                    if (select_res == objs[i])
                        continue;

                    string path = AssetDatabase.GetAssetPath(objs[i]);

                    AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
                    if(clip != null)
                    {
                        path = path.Remove(0, ActionEventConfig.RES_START_PATH.Length);

                        if(!IsAlreadyHaveRes(path))
                        {
                            list.Add(path);
                        }
                    }
                }

                //批量创建数据
                count = list.Count;
                for (int i = 0; i < count; i++)
                {
                    Table_Client_Action_Res ar = GetNewActionRes(list[i]);

                    //添加消息数据
                    SendServerInfo(ar, UpdateServerInfoEnum.INSERT);
                }
            }
            
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        void Delete()
        {
            if (action_res_list != null && ar != null)
            {
                action_res_list.Remove(ar);
                action_res_dic.Remove(ar.path);
                SendServerInfo(ar, UpdateServerInfoEnum.DELETE);
                ar = null;
            }
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        void Save()
        {
            SendServerInfo(ar, UpdateServerInfoEnum.UPDATE);
        }

        /// <summary>
        /// 检测是否已经存在此资源数据
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        bool IsAlreadyHaveRes(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            return action_res_dic.ContainsKey(url);
        }

        /// <summary>
        /// 获取一个新的 Action Res 数据
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Table_Client_Action_Res GetNewActionRes(string path)
        {
            Table_Client_Action_Res ar = new Table_Client_Action_Res();
            if (action_res_list == null)
                action_res_list = new List<Table_Client_Action_Res>();

            int count = action_res_list.Count;
            if (count > 0)
                ar.id = action_res_list[count - 1].id + 1;
            else
                ar.id = 1;

            ar.path = path;
            ar.remark = "";
            action_res_list.Add(ar);

            return ar;
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

        void SendServerInfo(Table_Client_Action_Res ar, UpdateServerInfoEnum type)
        {
            string t = GetServerInfoType(type);
            string url = string.Format("{0}{1}{2}", ActionEventConfig.SERVER_URL, ActionEventConfig.SERVER_RES_URL, t);

            switch (type)
            {
                case UpdateServerInfoEnum.INSERT:
                case UpdateServerInfoEnum.UPDATE:
                    EditorCoroutineRunner.StartEditorCoroutine(SendInfo(url, SendUpdateInfo(ar)));
                    break;
                case UpdateServerInfoEnum.DELETE:
                    EditorCoroutineRunner.StartEditorCoroutine(SendInfo(url, SendDeleteInfo(ar.id)));
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
        WWWForm SendUpdateInfo(Table_Client_Action_Res sa)
        {
            WWWForm wf = new WWWForm();

            wf.AddField("id", sa.id);
            wf.AddField("path", sa.path);
            wf.AddField("remark", sa.remark);

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
