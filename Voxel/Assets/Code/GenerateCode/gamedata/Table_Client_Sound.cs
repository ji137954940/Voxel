using System.Collections;
using System.Collections.Generic;
using ZLib;
using System;
using ZTool.Table;
namespace  Tgame.Game.Table
{
    ///<summary>
    /// 音效
    ///</summary>
    [Serializable]
    [TableName("client_sound")]
    public partial class Table_Client_Sound : TableContent
    {

        private static List<Table_Client_Sound> all_Table_Client_Sound_List = new List<Table_Client_Sound>();
        //primary | 主键
        public static Dictionary<int, Table_Client_Sound > pool_primary = new Dictionary<int, Table_Client_Sound > ();
        
        
        ///<summary>
        /// 主键：ID
        ///</summary>
        public int id;
        
        
        ///<summary>
        /// 声音名称
        ///</summary>
        public string name;
        
        
        ///<summary>
        /// 事件名称
        ///</summary>
        public string event_str;
        
        
        ///<summary>
        /// bank路径
        ///</summary>
        public string bank_path;
        
        
        ///<summary>
        /// 是否是主包
        ///</summary>
        public bool is_master_bank;
        
        
        ///<summary>
        /// 参数1
        ///</summary>
        public string param;
        
        
        ///<summary>
        /// 参数1的值
        ///</summary>
        public string param_value;
        
        
        ///<summary>
        /// 参数1描述
        ///</summary>
        public string param_des;
        
        
        ///<summary>
        /// 打断参数名称
        ///</summary>
        public string interrupt_param;
        
        
        ///<summary>
        /// 打断参数值
        ///</summary>
        public int interrupt_param_value;
        
        
        ///<summary>
        /// 关联子声音
        ///</summary>
        public string sub_sound;
        
        
        ///<summary>
        /// 深水区ID
        ///</summary>
        public int deep_water_id;
        
        
        ///<summary>
        /// 浅水区ID
        ///</summary>
        public int shallow_water_id;
        

        ///<summary>
        /// 主键
        /// 查询数据
        ///</summary>
        ///	<param id> 主键：ID</param>
        ///
        public static Table_Client_Sound GetPrimary ( int _id ){        
            Table_Client_Sound _map0=null;        
            pool_primary. TryGetValue(_id,out _map0);        
            return  _map0;
        }
         ///<summary>
        ///主键
        ///查询所有数据
        ///</summary>
        public static Dictionary<int, Table_Client_Sound > GetAllPrimary()
        {
            return pool_primary;
        }


        ///查询出所有的数据
        public static List<Table_Client_Sound> GetAllPrimaryList()
        {
            return all_Table_Client_Sound_List;
        }

        ///<summary>
        /// 通过字典初始化对象值
        ///</summary>
        public override void ParseFrom(Dictionary<string, string> _itemData) 
        {
            string _currValue = "";
            if(_itemData.TryGetValue("id", out _currValue))
            {
                this.id = Utils.GetIntFromString(_currValue);
            }
            if(_itemData.TryGetValue("name", out _currValue))
            {
                this.name = _currValue;
            }
            if(_itemData.TryGetValue("event_str", out _currValue))
            {
                this.event_str = _currValue;
            }
            if(_itemData.TryGetValue("bank_path", out _currValue))
            {
                this.bank_path = _currValue;
            }
            if(_itemData.TryGetValue("is_master_bank", out _currValue))
            {
                this.is_master_bank = Utils.GetBoolFromString(_currValue);
            }
            if(_itemData.TryGetValue("param", out _currValue))
            {
                this.param = _currValue;
            }
            if(_itemData.TryGetValue("param_value", out _currValue))
            {
                this.param_value = _currValue;
            }
            if(_itemData.TryGetValue("param_des", out _currValue))
            {
                this.param_des = _currValue;
            }
            if(_itemData.TryGetValue("interrupt_param", out _currValue))
            {
                this.interrupt_param = _currValue;
            }
            if(_itemData.TryGetValue("interrupt_param_value", out _currValue))
            {
                this.interrupt_param_value = Utils.GetIntFromString(_currValue);
            }
            if(_itemData.TryGetValue("sub_sound", out _currValue))
            {
                this.sub_sound = _currValue;
            }
            if(_itemData.TryGetValue("deep_water_id", out _currValue))
            {
                this.deep_water_id = Utils.GetIntFromString(_currValue);
            }
            if(_itemData.TryGetValue("shallow_water_id", out _currValue))
            {
                this.shallow_water_id = Utils.GetIntFromString(_currValue);
            }
        }
        
        ///<summary>
        /// 获取table表名
        ///</summary>
        public override string Table()
        {
           return "client_sound";
        }
        
        ///<summary>
        ///根据column获取值
        ///</summary>
        public override object GetValue(string column)
        {
            switch (column)
            {
                case "id":
                    return this.id;
                case "name":
                    return this.name;
                case "event_str":
                    return this.event_str;
                case "bank_path":
                    return this.bank_path;
                case "is_master_bank":
                    return this.is_master_bank;
                case "param":
                    return this.param;
                case "param_value":
                    return this.param_value;
                case "param_des":
                    return this.param_des;
                case "interrupt_param":
                    return this.interrupt_param;
                case "interrupt_param_value":
                    return this.interrupt_param_value;
                case "sub_sound":
                    return this.sub_sound;
                case "deep_water_id":
                    return this.deep_water_id;
                case "shallow_water_id":
                    return this.shallow_water_id;
                default:
                    return null;
            }
        }
        
        ///<summary>
        /// 初始化Pool
        ///</summary>
        public static void InitPool(IList _rows){
            List<Table_Client_Sound> rows = _rows as List<Table_Client_Sound>;
            pool_primary=TableContent.ListToPool < int, Table_Client_Sound > ( rows, "map", "id" );
            all_Table_Client_Sound_List=rows;
        }
        
        ///<summary>
        /// 清理静态数据
        ///</summary>
        public static void Clear()
        {
            pool_primary.Clear();
            all_Table_Client_Sound_List.Clear();
        }
    }
}
