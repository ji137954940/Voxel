using System.Collections;
using System.Collections.Generic;
using ZLib;
using System;
using ZTool.Table;
namespace  Tgame.Game.Table
{
    ///<summary>
    /// 客户端预加载资源表
    ///</summary>
    [Serializable]
    [TableName("client_pre_load_resources")]
    public partial class Table_Client_Pre_Load_Resources : TableContent
    {

        private static List<Table_Client_Pre_Load_Resources> all_Table_Client_Pre_Load_Resources_List = new List<Table_Client_Pre_Load_Resources>();
        //primary | 主键
        public static Dictionary<int, Table_Client_Pre_Load_Resources > pool_primary = new Dictionary<int, Table_Client_Pre_Load_Resources > ();
        
        
        ///<summary>
        /// id
        ///</summary>
        public int id;
        
        
        ///<summary>
        /// 类型(1:创建角色,2:玩家自身资源,3:常驻资源)
        ///</summary>
        public int type;
        
        
        ///<summary>
        /// 预加载资源的路径
        ///</summary>
        public string path;
        
        
        ///<summary>
        ///  资源类型(1:模型,2:特效,3:声音,4:场景,5:UI)
        ///</summary>
        public int resource_type_id;
        
        
        ///<summary>
        /// 对应资源的表格id
        ///</summary>
        public int resource_id;
        
        
        ///<summary>
        /// 族 
        ///</summary>
        public int raceid;
        
        
        ///<summary>
        /// 职业
        ///</summary>
        public int professionid;
        
        
        ///<summary>
        /// 名称说明
        ///</summary>
        public string name;
        

        ///<summary>
        /// 主键
        /// 查询数据
        ///</summary>
        ///	<param id> id</param>
        ///
        public static Table_Client_Pre_Load_Resources GetPrimary ( int _id ){        
            Table_Client_Pre_Load_Resources _map0=null;        
            pool_primary. TryGetValue(_id,out _map0);        
            return  _map0;
        }
         ///<summary>
        ///主键
        ///查询所有数据
        ///</summary>
        public static Dictionary<int, Table_Client_Pre_Load_Resources > GetAllPrimary()
        {
            return pool_primary;
        }


        ///查询出所有的数据
        public static List<Table_Client_Pre_Load_Resources> GetAllPrimaryList()
        {
            return all_Table_Client_Pre_Load_Resources_List;
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
            if(_itemData.TryGetValue("type", out _currValue))
            {
                this.type = Utils.GetIntFromString(_currValue);
            }
            if(_itemData.TryGetValue("path", out _currValue))
            {
                this.path = _currValue;
            }
            if(_itemData.TryGetValue("resource_type_id", out _currValue))
            {
                this.resource_type_id = Utils.GetIntFromString(_currValue);
            }
            if(_itemData.TryGetValue("resource_id", out _currValue))
            {
                this.resource_id = Utils.GetIntFromString(_currValue);
            }
            if(_itemData.TryGetValue("raceid", out _currValue))
            {
                this.raceid = Utils.GetIntFromString(_currValue);
            }
            if(_itemData.TryGetValue("professionid", out _currValue))
            {
                this.professionid = Utils.GetIntFromString(_currValue);
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
           return "client_pre_load_resources";
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
                case "type":
                    return this.type;
                case "path":
                    return this.path;
                case "resource_type_id":
                    return this.resource_type_id;
                case "resource_id":
                    return this.resource_id;
                case "raceid":
                    return this.raceid;
                case "professionid":
                    return this.professionid;
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
            List<Table_Client_Pre_Load_Resources> rows = _rows as List<Table_Client_Pre_Load_Resources>;
            pool_primary=TableContent.ListToPool < int, Table_Client_Pre_Load_Resources > ( rows, "map", "id" );
            all_Table_Client_Pre_Load_Resources_List=rows;
        }
        
        ///<summary>
        /// 清理静态数据
        ///</summary>
        public static void Clear()
        {
            pool_primary.Clear();
            all_Table_Client_Pre_Load_Resources_List.Clear();
        }
    }
}
