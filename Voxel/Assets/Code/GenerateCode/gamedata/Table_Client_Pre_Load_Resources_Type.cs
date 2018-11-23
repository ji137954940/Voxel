using System.Collections;
using System.Collections.Generic;
using ZLib;
using System;
using ZTool.Table;
namespace  Tgame.Game.Table
{
    ///<summary>
    /// 客户端预加载资源类型
    ///</summary>
    [Serializable]
    [TableName("client_pre_load_resources_type")]
    public partial class Table_Client_Pre_Load_Resources_Type : TableContent
    {

        private static List<Table_Client_Pre_Load_Resources_Type> all_Table_Client_Pre_Load_Resources_Type_List = new List<Table_Client_Pre_Load_Resources_Type>();
        //primary | 主键
        public static Dictionary<int, Table_Client_Pre_Load_Resources_Type > pool_primary = new Dictionary<int, Table_Client_Pre_Load_Resources_Type > ();
        
        
        ///<summary>
        /// 主键：ID
        ///</summary>
        public int id;
        
        
        ///<summary>
        /// 资源类型
        ///</summary>
        public string ckey;
        
        
        ///<summary>
        /// 类型说明
        ///</summary>
        public string name;
        

        ///<summary>
        /// 主键
        /// 查询数据
        ///</summary>
        ///	<param id> 主键：ID</param>
        ///
        public static Table_Client_Pre_Load_Resources_Type GetPrimary ( int _id ){        
            Table_Client_Pre_Load_Resources_Type _map0=null;        
            pool_primary. TryGetValue(_id,out _map0);        
            return  _map0;
        }
         ///<summary>
        ///主键
        ///查询所有数据
        ///</summary>
        public static Dictionary<int, Table_Client_Pre_Load_Resources_Type > GetAllPrimary()
        {
            return pool_primary;
        }


        ///查询出所有的数据
        public static List<Table_Client_Pre_Load_Resources_Type> GetAllPrimaryList()
        {
            return all_Table_Client_Pre_Load_Resources_Type_List;
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
            if(_itemData.TryGetValue("ckey", out _currValue))
            {
                this.ckey = _currValue;
            }
            if(_itemData.TryGetValue("name", out _currValue))
            {
                this.name = _currValue;
            }
        }
        
        ///<summary>
        /// 获取table表名
        ///</summary>
        public override string Table()
        {
           return "client_pre_load_resources_type";
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
                case "ckey":
                    return this.ckey;
                case "name":
                    return this.name;
                default:
                    return null;
            }
        }
        
        ///<summary>
        /// 初始化Pool
        ///</summary>
        public static void InitPool(IList _rows){
            List<Table_Client_Pre_Load_Resources_Type> rows = _rows as List<Table_Client_Pre_Load_Resources_Type>;
            pool_primary=TableContent.ListToPool < int, Table_Client_Pre_Load_Resources_Type > ( rows, "map", "id" );
            all_Table_Client_Pre_Load_Resources_Type_List=rows;
        }
        
        ///<summary>
        /// 清理静态数据
        ///</summary>
        public static void Clear()
        {
            pool_primary.Clear();
            all_Table_Client_Pre_Load_Resources_Type_List.Clear();
        }
    }
}
