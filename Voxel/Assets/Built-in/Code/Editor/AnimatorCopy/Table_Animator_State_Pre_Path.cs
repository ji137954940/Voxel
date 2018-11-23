using System;
using System.Collections.Generic;

namespace AnimatorTool.Table
{
    ///<summary>
    /// 状态机职业动作路径与前缀表
    ///</summary>
    [Serializable]
    public class Table_Animator_State_Pre_Path
    {
        ///<summary>
        /// 职业ID
        ///</summary>
        public int id;
        ///<summary>
        /// 动作文件前缀
        ///</summary>
        public string ckey;
        ///<summary>
        /// 职业名称
        ///</summary>
        public string name;
        ///<summary>
        /// 职业名称国际化
        ///</summary>
        public string name_i18n;
        ///<summary>
        /// 文件夹路径
        ///</summary>
        public string path;

        /// <summary>
        /// 数据转换
        /// </summary>
        /// <param name="_dataDic"></param>
        public void ParseFrom(Dictionary<string, string> _dataDic)
        {
            string value = "";
            if (_dataDic.TryGetValue("id", out value))
            {
                this.id = int.Parse(value);
            }

            if (_dataDic.TryGetValue("ckey", out value))
            {
                this.ckey = value;
            }

            if (_dataDic.TryGetValue("name", out value))
            {
                this.name = value;
            }

            if (_dataDic.TryGetValue("name_i18n", out value))
            {
                this.name_i18n = value;
            }

            if (_dataDic.TryGetValue("path", out value))
            {
                this.path = value;
            }
        }
    }
}
