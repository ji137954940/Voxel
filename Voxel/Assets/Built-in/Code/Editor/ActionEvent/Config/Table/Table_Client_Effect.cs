using System;

namespace ActionEvent.Table
{
    /// <summary>
    /// client_effect
    /// </summary>
    [Serializable]
	public class Table_Client_Effect
	{

		//主键：ID
		public int id;

		//名称
		public string name;

		//关联子特效：多个ID用逗号分割
		public string sub_effect;

		//资源路径
		public string path;

		//关联的声音ID
		public int refer_sound_id;

		//备注
		public string remark;
	}
}