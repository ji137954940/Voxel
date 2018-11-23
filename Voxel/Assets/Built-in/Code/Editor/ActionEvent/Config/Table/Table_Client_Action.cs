using System;

namespace ActionEvent.Table
{
    /// <summary>
    /// client_action
    /// </summary>
    [Serializable]
	public class Table_Client_Action
	{

		//主键：ID
		public int id;

		//名称
		public string name;

		//动作转换条件(int)
		public string action_condition;

		//动作转换条件(float)
		public string action_condition_float;

		//动作图层0
		public string action_layer;

		//移动状态动作融合度（0-1）
		public float action_layer_fusion;

		//IK脚本权重
		public string ik_weight;

		//动作时长（毫秒）
		public int action_time;
	}
}