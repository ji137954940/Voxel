using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionEvent
{

    public class ActionEventConfig
    {
        //资源开始路径信息
        public const string RES_START_PATH = "Assets/Resources/Res/";

        //服务器资源地址
        public const string SERVER_URL = "http://127.0.0.1:20000/";

        //服务器资源 资源配表 url
        public const string SERVER_RES_URL = "client_action_res/";
        //服务器资源 技能动作配置 url
        public const string SERVER_SKILL_ACTION_URL = "skill/";
        //动作数据表 url
        public const string SERVER_CLIENT_ACTION_URL = "client_action/";
        //特效数据表 url
        public const string SERVER_CLIENT_EFFECT_URL = "client_effect/";


        //列表数据
        public const string LIST = "list";
        //插入数据
        public const string INSERT = "insert";
        //更新数据
        public const string UPDATE = "update";
        //删除数据
        public const string DELETE = "delete";
    }
}
