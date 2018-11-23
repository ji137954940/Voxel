using System.Collections;
using System.Collections.Generic;
using ZLib;
using System;
using ZTool.Table;
namespace Tgame.Game.Table
{
    ///<summary>
    /// 触发器刷怪区域
    ///</summary>
    [Serializable]
    [TableName("client_icon")]
    public partial class Table_Client_Icon : TableContent
    {

        private static List<Table_Client_Icon> all_Table_Instance_Npc_Area_List = new List<Table_Client_Icon>();
        //primary | 主键
        public static Dictionary<int, Dictionary<int, Table_Client_Icon>> pool_primary = new Dictionary<int, Dictionary<int, Table_Client_Icon>>();


        ///<summary>
        /// 类型id
        ///</summary>
        public int type_id;


        ///<summary>
        /// 文件id
        ///</summary>
        public int id;

        /// <summary>
        /// 是否为动画文件
        /// </summary>
        public bool is_animation;

        /// <summary>
        /// 是否为voxel数据
        /// </summary>
        public bool is_voxel;

        ///<summary>
        /// 名称
        ///</summary>
        public string name;

        ///<summary>
        /// 资源路径
        ///</summary>
        public string path;

        /// <summary>
        /// voxel 数据路径
        /// </summary>
        public string voxel_path;

        ///<summary>
        /// 主键
        /// 查询数据
        ///</summary>
        ///	<param typeId> 类型id</param>
        ///
        public static Dictionary<int, Table_Client_Icon> GetPrimary(int _typeId)
        {
            Dictionary<int, Table_Client_Icon> _map0 = null;
            pool_primary.TryGetValue(_typeId, out _map0);
            if (_map0 == null)
            {
                return null;
            }
            return _map0;
        }
        ///<summary>
        /// 主键
        /// 查询数据
        ///</summary>
        ///	<param _typeId> 类型id</param>
        ///	<param id> 文件id</param>
        ///
        public static Table_Client_Icon GetPrimary(int _typeId, int _id)
        {
            Dictionary<int, Table_Client_Icon> _map0 = null;
            pool_primary.TryGetValue(_typeId, out _map0);
            if (_map0 == null)
            {
                return null;
            }

            Table_Client_Icon _map1 = null;
            _map0.TryGetValue(_id, out _map1);
            return _map1;
        }
        ///<summary>
        ///主键
        ///查询所有数据
        ///</summary>
        public static Dictionary<int, Dictionary<int, Table_Client_Icon>> GetAllPrimary()
        {
            return pool_primary;
        }


        ///查询出所有的数据
        public static List<Table_Client_Icon> GetAllPrimaryList()
        {
            return all_Table_Instance_Npc_Area_List;
        }

        ///<summary>
        /// 通过字典初始化对象值
        ///</summary>
        public override void ParseFrom(Dictionary<string, string> _itemData)
        {
            string _currValue = "";
            if (_itemData.TryGetValue("type_id", out _currValue))
            {
                this.type_id = Utils.GetIntFromString(_currValue);
            }
            if (_itemData.TryGetValue("id", out _currValue))
            {
                this.id = Utils.GetIntFromString(_currValue);
            }
            if (_itemData.TryGetValue("is_animation", out _currValue))
            {
                this.is_animation = Utils.GetBoolFromString(_currValue);
            }
            if (_itemData.TryGetValue("is_voxel", out _currValue))
            {
                this.is_voxel = Utils.GetBoolFromString(_currValue);
            }
            if (_itemData.TryGetValue("name", out _currValue))
            {
                this.name = _currValue;
            }
            if (_itemData.TryGetValue("path", out _currValue))
            {
                this.path = _currValue;
            }
            if (_itemData.TryGetValue("voxel_path", out _currValue))
            {
                this.voxel_path = _currValue;
            }
        }

        ///<summary>
        /// 获取table表名
        ///</summary>
        public override string Table()
        {
            return "client_icon";
        }

        ///<summary>
        ///根据column获取值
        ///</summary>
        public override object GetValue(string column)
        {
            switch (column)
            {
                case "type_id":
                    return this.type_id;
                case "id":
                    return this.id;
                case "is_animation":
                    return this.is_animation;
                case "is_voxel":
                    return this.is_voxel;
                case "name":
                    return this.name;
                case "path":
                    return this.path;
                case "voxel_path":
                    return this.voxel_path;
                default:
                    return null;
            }
        }

        ///<summary>
        /// 初始化Pool
        ///</summary>
        public static void InitPool(IList _rows)
        {
            List<Table_Client_Icon> rows = _rows as List<Table_Client_Icon>;
            pool_primary = TableContent.ListToPool<int, int, Table_Client_Icon>(rows, "map", "type_id", "id");
            all_Table_Instance_Npc_Area_List = rows;
        }

        ///<summary>
        /// 清理静态数据
        ///</summary>
        public static void Clear()
        {
            pool_primary.Clear();
            all_Table_Instance_Npc_Area_List.Clear();
        }
    }
}
