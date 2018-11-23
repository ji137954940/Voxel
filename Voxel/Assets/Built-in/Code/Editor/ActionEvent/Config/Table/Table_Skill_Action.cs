using System;

namespace ActionEvent.Table
{
    /// <summary>
    /// skill_action
    /// </summary>
    [Serializable]
	public class Table_Skill_Action
	{

		//主键
		public int id;

		//动作组名称
		public string name;

		//前摇时间
		public int pre_duration;

		//施法时间
		public int cast_duration;

		//吟唱时间
		public int sing_duration;

		//后摇时间
		public int post_duration;

        //位移开始时间
        public int shift_time;

        //伤害点延时
        public int hurt_time;

        //伤害触发间隔
        public int hurt_interval;

		//不可移动时间(ms)，-1时使用动作时间
		public int no_move_duration = -1;

        //收招点时间
        public int close_time;

        //前摇动作资源
        public int pre_action_res;

		//施法动作资源
		public int cast_actioin_res;

		//吟唱动作资源
		public int sing_action_res;

		//后摇动作资源
		public int post_action_res;

		//前摇动作
		public int pre_action;

		//施法动作
		public int cast_action;

		//吟唱动作
		public int sing_action;

		//后摇动作
		public int post_action;

		//前摇特效
		public int pre_effect;

		//施法特效
		public int cast_effect;

		//吟唱特效
		public int sing_effect;

		//后摇特效
		public int post_effect;

		//受击动作
		public int target_action;

		//受击特效
		public int target_effect;

        //子弹特效
        public int bullet_effect;
    }
}