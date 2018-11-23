using System;
using System.Collections.Generic;
using System.Text;
using ZLib;
using ZTool.Table;

/// <summary>
/// 资源预先加载，配置数据信息
/// </summary>
public class GamePreLoadData : Singleton<GamePreLoadData>
{
    #region 加载本地资源配置数据
    /// <summary>
    /// 存储本地资源配置列表数据
    /// </summary>
    HashSet<string> local_res = null;
    public HashSet<string> Local_Res { get { return local_res; } }

    /// <summary>
    /// 设置本地资源配置数据信息
    /// </summary>
    /// <param name="str"></param>
    public void SetLoadLocalResConfig(string str)
    {
        if (string.IsNullOrEmpty(str))
            return;

        local_res = new HashSet<string>();

        var sb = new StringBuilder();

        var index = 0;

        while (index < str.Length)
        {
            if (str[index] != '\n')
            {
                sb.Append(str[index]);
            }
            else
            {
                var extensionIndex = -1;

                for (int i = sb.Length - 1; i >= 0; i--)
                {
                    if (sb[i] == '.')
                    {
                        extensionIndex = i;
                        break;
                    }
                }

                if (extensionIndex != -1)
                {
                    local_res.Add(sb.ToString(0, extensionIndex));
                }
                else
                {
                    local_res.Add(sb.ToString());
                }

                sb.Length = 0;
            }

            index++;
        }
    }

    /// <summary>
    /// 是否包含此路径数据
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public bool IsHaveRes(string url)
    {
        if (local_res != null && local_res.Count > 0)
        {
            return local_res.Contains(url);
        }
        return false;
    }

    #endregion

    #region 加载配置表数据信息

    //预先加载的配表数据
    string[] pre_load_tables = new string[] { };
    bool inited = false;
    //预先加载的配表数据
    public string[] Pre_Load_Tables
    {
        get
        {

            //根据属性 获取处所有需要读取的配置文件合集
            if (!inited)
            {
                inited = true;
                //Debug.Log("Init UI Pre_Load_Tables");
                var classes = typeof(GamePreLoadData).Assembly.GetTypes();
                var att = typeof(TableNameAttribute);
                List<string> tableNames = new List<string>();
                for (int i = 0; i < classes.Length; i++)
                {
                    if (!string.IsNullOrEmpty(classes[i].Namespace) && classes[i].Namespace.Equals("Tgame.Game.Table"))
                    {
                        Type type = classes[i];
                        object[] objs = type.GetCustomAttributes(att, false);
                        if (objs.Length > 0)
                        {
                            string tName = ((TableNameAttribute)objs[0]).tableName;
                            tableNames.Add(tName);
                        }
                    }
                }
                pre_load_tables = tableNames.ToArray();
            }
            return pre_load_tables;
        }
    }

    /// <summary>
    /// 清理预先加载的 table 表数据信息
    /// </summary>
    public void ClearPreLoadTables()
    {
        if (pre_load_tables != null
            && pre_load_tables.Length > 0)
            pre_load_tables = null;
    }

    #endregion
}

