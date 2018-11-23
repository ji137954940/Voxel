using System;

namespace ActionEvent.Table
{
	///<summary>
    /// 技能
    ///</summary>
	[Serializable]
	public class Table_Skill
	{

		///<summary>
    	/// 主键：技能ID
    	///</summary>
        public int id;

		///<summary>
    	/// 名称国际化
    	///</summary>
        public string name_i18n;

		///<summary>
    	/// 类型
    	///</summary>
        public int type;

		///<summary>
    	/// 释放方式
    	///</summary>
        public int use_type;

		///<summary>
    	/// 是否可以移动施法
    	///</summary>
        public bool move_spell;

		///<summary>
    	/// 是否触发公共冷却时间
    	///</summary>
        public bool trigger_public_cd;

		///<summary>
    	/// 是否免疫控制
    	///</summary>
        public bool ignore_control;

		///<summary>
    	/// 位移速度：米/秒
    	///</summary>
        public int shift_speed;

		///<summary>
    	/// 追踪子弹飞行速度：米/秒
    	///</summary>
        public int track_bullet_speed;

		///<summary>
    	/// 定位子弹飞行时间：毫秒
    	///</summary>
        public int locate_bullet_time;

		///<summary>
    	/// 技能图标
    	///</summary>
        public int icon_id;

		///<summary>
    	/// 描述国际化
    	///</summary>
        public string description_i18n;

		///<summary>
    	/// 效果描述
    	///</summary>
        public string effect_description;

		///<summary>
    	/// 飘字总次数
    	///</summary>
        public int floating_count;

		///<summary>
    	/// 飘字间隔时间(ms)
    	///</summary>
        public int floating_interval;

		///<summary>
    	/// 是否触发摄像机自动旋转
    	///</summary>
        public bool trigger_camera_auto_rotation;

		///<summary>
    	/// 是否使用连击点
    	///</summary>
        public bool use_combo_point;

		///<summary>
    	/// 不可移动时间
    	///</summary>
        public int no_move_duration;

		///<summary>
    	/// 收招点，基于后摇
    	///</summary>
        public int close_time;

		///<summary>
    	/// 前摇动作
    	///</summary>
        public int pre_action;

		///<summary>
    	/// 施法动作
    	///</summary>
        public int cast_action;

		///<summary>
    	/// 吟唱动作
    	///</summary>
        public int sing_action;

		///<summary>
    	/// 后摇动作
    	///</summary>
        public int post_action;

		///<summary>
    	/// 前摇特效
    	///</summary>
        public int pre_effect;

		///<summary>
    	/// 施法特效
    	///</summary>
        public int cast_effect;

		///<summary>
    	/// 吟唱特效
    	///</summary>
        public int sing_effect;

		///<summary>
    	/// 后摇特效
    	///</summary>
        public int post_effect;

		///<summary>
    	/// 子弹特效
    	///</summary>
        public int bullet_effect;

		///<summary>
    	/// 子物体特效
    	///</summary>
        public int subobject_effect;

		///<summary>
    	/// 受击动作
    	///</summary>
        public int target_action;

		///<summary>
    	/// 受击特效
    	///</summary>
        public int target_effect;

		///<summary>
    	/// 受击音效
    	///</summary>
        public int target_sound;

		///<summary>
    	/// IK脚本权重
    	///</summary>
        public string ik_weight;


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

        //前摇动作资源
        public int pre_action_res;

        //施法动作资源
        public int cast_actioin_res;

        //吟唱动作资源
        public int sing_action_res;

        //后摇动作资源
        public int post_action_res;
    }
}

