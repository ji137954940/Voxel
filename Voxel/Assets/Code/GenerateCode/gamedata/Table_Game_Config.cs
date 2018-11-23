using System.Collections;
using System.Collections.Generic;
using ZLib;
using System;
using ZTool.Table;
namespace Tgame.Game.Table
{
    ///<summary>
    /// 游戏配置
    ///</summary>
    [Serializable]
    [TableName("game_config")]
    public partial class Table_Game_Config : TableContent
    {

        private static List<Table_Game_Config> all_Table_Game_Config_List = new List<Table_Game_Config>();
        //primary | 主键
        public static Dictionary<string, Table_Game_Config> pool_primary = new Dictionary<string, Table_Game_Config>();


        ///<summary>
        /// 主键：配置key
        ///</summary>
        public string id;


        ///<summary>
        /// 类型
        ///</summary>
        public string type;


        ///<summary>
        /// 模块
        ///</summary>
        public string module;


        ///<summary>
        /// 名称
        ///</summary>
        public string name;


        ///<summary>
        /// 数据
        ///</summary>
        public string data;


        ///<summary>
        /// 主键
        /// 查询数据
        ///</summary>
        ///	<param id> 主键：配置key</param>
        ///
        public static Table_Game_Config GetPrimary(string _id)
        {
            Table_Game_Config _map0 = null;
            pool_primary.TryGetValue(_id, out _map0);
            return _map0;
        }
        ///<summary>
        ///主键
        ///查询所有数据
        ///</summary>
        public static Dictionary<string, Table_Game_Config> GetAllPrimary()
        {
            return pool_primary;
        }


        ///查询出所有的数据
        public static List<Table_Game_Config> GetAllPrimaryList()
        {
            return all_Table_Game_Config_List;
        }

        ///<summary>
        /// 通过字典初始化对象值
        ///</summary>
        public override void ParseFrom(Dictionary<string, string> _itemData)
        {
            string _currValue = "";
            if (_itemData.TryGetValue("id", out _currValue))
            {
                this.id = _currValue;
            }
            if (_itemData.TryGetValue("type", out _currValue))
            {
                this.type = _currValue;
            }
            if (_itemData.TryGetValue("module", out _currValue))
            {
                this.module = _currValue;
            }
            if (_itemData.TryGetValue("name", out _currValue))
            {
                this.name = _currValue;
            }
            if (_itemData.TryGetValue("data", out _currValue))
            {
                this.data = _currValue;
            }
        }

        ///<summary>
        /// 获取table表名
        ///</summary>
        public override string Table()
        {
            return "game_config";
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
                case "module":
                    return this.module;
                case "name":
                    return this.name;
                case "data":
                    return this.data;
                default:
                    return null;
            }
        }

        ///<summary>
        /// 初始化Pool
        ///</summary>
        public static void InitPool(IList _rows)
        {
            List<Table_Game_Config> rows = _rows as List<Table_Game_Config>;
            pool_primary = TableContent.ListToPool<string, Table_Game_Config>(rows, "map", "id");
            all_Table_Game_Config_List = rows;
        }

        ///<summary>
        /// 清理静态数据
        ///</summary>
        public static void Clear()
        {
            pool_primary.Clear();
            all_Table_Game_Config_List.Clear();
        }
    }
}
