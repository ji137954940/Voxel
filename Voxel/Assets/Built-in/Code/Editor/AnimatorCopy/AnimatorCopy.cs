using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Text;
using System;
using System.Reflection;
using System.Threading;
using AnimatorTool.Table;

namespace AnimatorTool
{
    /// <summary>
    /// AnimatorController copy 管理类
    /// @author LiuLeiLei
    /// @data 5/8/2018
    /// @desc 
    /// </summary>
    public class AnimatorCopy
    {
        /// <summary>
        /// state 字典
        /// </summary>
        private Dictionary<string, Dictionary<string, State>> stateDic = new Dictionary<string, Dictionary<string, State>>();

        /// <summary>
        /// 复制的controller
        /// </summary>
        public AnimatorController copy;

        /// <summary>
        /// log
        /// </summary>
        private StringBuilder logSb = new StringBuilder();

        /// <summary>
        /// 动作文件前缀
        /// </summary>
        private int prePathIndex;

        /// <summary>
        /// 配表
        /// </summary>
        private List<Table_Animator_State_Pre_Path> tableList = new List<Table_Animator_State_Pre_Path>();

        /// <summary>
        /// 动作文件前缀list
        /// </summary>
        public List<string> prePathList = new List<string>();

        /// <summary>
        /// 配置文件名
        /// </summary>
        private const string configName = "animator_state_pre_path";

        #region 获取配置文件数据

        /// <summary>
        /// 解析配置文件
        /// </summary>
        public void OnGetConfig()
        {
            string url = string.Format("Config/{0}", configName);

            //加载配置文件
            TextAsset ta = Resources.Load(url, typeof(TextAsset)) as TextAsset;
            //获取配置文件内容
            string content = Encoding.Default.GetString(ta.bytes);

            string[] arr = content.Split(new char[] { '\n' });

            int count = arr.Length;

            prePathList.Clear();

            //第一行 ： 变量名
            //第二行 ： 变量类型
            //第三行 ： 变量描述
            //第四行... ： 具体数值
            if (count > 3)
            {
                //变量名
                string[] variableArr = arr[0].Split(',');

                //变量
                for (int i = 3; i < count; i++)
                {
                    if (string.IsNullOrEmpty(arr[i]))
                        continue;

                    Dictionary<string, string> tabDic = new Dictionary<string, string>();

                    string[] dataStrArr = arr[i].Split(',');

                    for (int j = 0; j < dataStrArr.Length; j++)
                    {
                        tabDic[variableArr[j]] = dataStrArr[j];
                    }

                    Table_Animator_State_Pre_Path table = new Table_Animator_State_Pre_Path();
                    table.ParseFrom(tabDic);

                    tableList.Add(table);

                    prePathList.Add(table.ckey);
                }
            }
        }

        #endregion

        #region 复制一个AnimatorController

        /// <summary>
        /// 复制一个controller
        /// </summary>
        /// <param name="_animator"></param>
        public void CopyAnimator(AnimatorController _animator, string _newPath)
        {
            if (_animator == null) return;

            var path = AssetDatabase.GetAssetPath(_animator);

            if (AssetDatabase.CopyAsset(path, _newPath))
            {
                copy = AssetDatabase.LoadAssetAtPath<AnimatorController>(_newPath);
            }

            AssetDatabase.Refresh();

            if (copy != null)
                EditorUtility.SetDirty(copy);
        }

        #endregion

        #region 获得State Dic

        /// <summary>
        /// 获得 state dic
        /// </summary>
        /// <param name="_animator"></param>
        public void OnGetStates(AnimatorController _animator)
        {
            if (_animator == null) return;

            stateDic = GetStateDic(_animator);
            //Debug.LogError("stateDic.count == " + stateDic.Count);
        }
        #endregion

        #region 一键关联所有的动作文件

        /// <summary>
        /// 一键关联所有动作文件
        /// </summary>
        /// <param name="_prePathIndex"></param>
        public void SetAllStateMotion(int _prePathIndex)
        {
            logSb.Remove(0, logSb.Length);

            string filePath = tableList[_prePathIndex].path;

            string logStr;

            for (var tempDic = stateDic.GetEnumerator(); tempDic.MoveNext();)
            {
                for (var state = tempDic.Current.Value.GetEnumerator(); state.MoveNext();)
                {
                    if (!state.Current.Value.SetMotion(prePathList[_prePathIndex], filePath, out logStr))
                    {
                        logSb.Append(logStr);
                        logSb.Append('\n');
                    }
                }
            }

            if (logSb.Length > 0)
            {
                Debug.LogError(logSb.ToString());
            }

            EditorUtility.SetDirty(copy);
        }

        #endregion

        #region 一键去除所有动作关联文件

        /// <summary>
        /// 一键去除所有动作关联文件
        /// </summary>
        public void RemoveMotion()
        {
            for (var tempDic = stateDic.GetEnumerator(); tempDic.MoveNext();)
            {
                for (var dic = tempDic.Current.Value.GetEnumerator(); dic.MoveNext();)
                {
                    var state = dic.Current.Value;
                    state.RemoveMotion();
                }
            }
        }

        #endregion

        #region 输出State对应文件表
        /// <summary>
        /// 输出对应数据表
        /// </summary>
        /// <param name="_filePath"></param>
        /// <param name="_prePathIndex"></param>
        public void OutputConfig(string _filePath, int _prePathIndex)
        {
            prePathIndex = _prePathIndex;

            List<State> stateList = GetAllStateList();

            CSVHelper<State> stateCsv = new CSVHelper<State>();
            stateCsv.onPropCallBack = OnPropCallBack;
            stateCsv.onValueCallBack = OnValueCallBack;
            stateCsv.CreateFile(_filePath);
            stateCsv.SaveDataToCSVFile(stateList, _filePath);
        }

        private void OnPropCallBack(StringBuilder _sb)
        {
            _sb.Append("FBX");
        }

        private void OnValueCallBack(StringBuilder _sb, State _state)
        {
            _sb.Append(_state.LayerName);
            _sb.Append(",");
            _sb.Append(_state.StatePath);
            _sb.Append(",");
            _sb.Append(string.Format("{0}{1}{2}", prePathList[prePathIndex], _state.value.name, ".FBX"));
        }

        #endregion

        #region Clear

        public void Clear()
        {
            prePathIndex = 0;
            copy = null;
        }

        #endregion

        #region State Dic

        /// <summary>
        /// 获得 AnimatorController state dic
        /// </summary>
        /// <param name="_animator"></param>
        /// <returns></returns>
        private Dictionary<string, Dictionary<string, State>> GetStateDic(AnimatorController _animator)
        {
            Dictionary<string, Dictionary<string, State>> tempDic = new Dictionary<string, Dictionary<string, State>>();

            _animator.layers.ToList().ForEach((layer) =>
            {
                Dictionary<string, State> tempStateDic = new Dictionary<string, State>();

                ForEachState(layer, layer.stateMachine, tempStateDic);

                tempDic.Add(layer.name, tempStateDic);
            });

            return tempDic;
        }

        /// <summary>
        /// 查找stateMachine中的state
        /// </summary>
        /// <param name="_stateMachine"></param>
        /// <param name="_tempStateDic"></param>
        private void ForEachState(AnimatorControllerLayer _layer, AnimatorStateMachine _stateMachine, Dictionary<string, State> _tempStateDic)
        {
            string keyStr = string.Empty;
            bool baseState = _layer.name == _stateMachine.name;

            _stateMachine.states.ToList().ForEach((state) =>
            {
                if (state.state.motion == null)
                {
                    Debug.LogError(string.Format("AnimationState : {0}'s motion is null", state.state.name));

                    if (baseState)
                    {
                        keyStr = state.state.name;
                    }
                    else
                    {
                        keyStr = string.Format("{0}/{1}", _stateMachine.name, state.state.name);
                    }

                    _tempStateDic.Add(keyStr, new State(state.state, _layer.name, keyStr));
                }
                else
                {
                    if (state.state.motion.GetType() == typeof(BlendTree))
                    {
                        BlendTree blendTree = (BlendTree)state.state.motion;

                        var blendTreeDic = GetBlendTreeDic(blendTree);
                        blendTreeDic.ToList().ForEach((pair) =>
                        {
                            if (baseState)
                            {
                                keyStr = string.Format("{0}/{1}", state.state.name, pair.Key);
                            }
                            else
                            {
                                keyStr = string.Format("{0}/{1}/{2}", _stateMachine.name, state.state.name, pair.Key);
                            }

                            _tempStateDic.Add(keyStr, new State(pair.Value, _layer.name, keyStr));
                        });
                    }
                    else
                    {
                        if (baseState)
                        {
                            keyStr = state.state.name;
                        }
                        else
                        {
                            keyStr = string.Format("{0}/{1}", _stateMachine.name, state.state.name);
                        }

                        _tempStateDic.Add(keyStr, new State(state.state, _layer.name, keyStr));
                    }
                }
            });

            _stateMachine.stateMachines.ToList().ForEach((stateMachine) =>
            {
                ForEachState(_layer, stateMachine.stateMachine, _tempStateDic);
            });
        }

        /// <summary>
        /// 获得 BlendTree state dic
        /// </summary>
        /// <param name="_blendTree"></param>
        /// <returns></returns>
        private Dictionary<string, BlendTree> GetBlendTreeDic(BlendTree _blendTree)
        {
            Dictionary<string, BlendTree> tempDic = new Dictionary<string, BlendTree>();

            int repeatKey = 0;
            int index = 0;

            _blendTree.children.ToList().ForEach((child) =>
            {
                if (child.motion == null)
                {
                    Debug.LogError(string.Format("BlendTree : {0}'s motion is null", _blendTree.name));

                    tempDic.Add(string.Format("{0}[{1}]", _blendTree.name, index), _blendTree);
                }
                else
                {
                    if (child.motion.GetType() == typeof(BlendTree))
                    {
                        //tempDic.Add(string.Format("{0}[{1}]", _blendTree.name, index), _blendTree);

                        var childDic = GetBlendTreeDic((BlendTree)child.motion);
                        childDic.ToList().ForEach((pair) =>
                        {
                            tempDic.Add(string.Format("{0}/{1}", _blendTree.name, pair.Key), pair.Value);
                        });
                    }
                    else
                    {
                        try
                        {
                            tempDic.Add(string.Format("{0}[{1}]", _blendTree.name, index), _blendTree);
                        }
                        catch (System.ArgumentException)
                        {
                            tempDic.Add(string.Format("{0} {1}", _blendTree.name, repeatKey++), _blendTree);
                        }
                    }
                }

                index++;
            });

            return tempDic;
        }

        /// <summary>
        /// 更新blendTree motion
        /// </summary>
        /// <param name="_value"></param>
        /// <param name="_childIndex"></param>
        /// <param name="_motion"></param>
        public static void OverrideBlendTree(BlendTree _value, int _childIndex, Motion _motion)
        {
            List<ChildMotion> childMotions = new List<ChildMotion>();

            _value.children.ToList().ForEach(child => childMotions.Add(child));

            for (int i = _value.children.Length - 1; i >= 0; i--)
            {
                _value.RemoveChild(i);
            }

            for (int i = 0; i < childMotions.Count; i++)
            {
                if (i == _childIndex)
                {
                    _value.AddChild(_motion);
                }
                else
                {
                    _value.AddChild(childMotions[i].motion);
                }
            }

            for (int i = 0; i < _value.children.Length; i++)
            {
                _value.children[i] = childMotions[i];
            }
        }

        #endregion

        /// <summary>
        /// 获得所有state list
        /// </summary>
        /// <returns></returns>
        public List<State> GetAllStateList()
        {
            List<State> tempList = new List<State>();

            for (var tempDic = stateDic.GetEnumerator(); tempDic.MoveNext();)
            {
                for (var item = tempDic.Current.Value.GetEnumerator(); item.MoveNext();)
                {
                    if (!tempList.Contains(item.Current.Value))
                    {
                        tempList.Add(item.Current.Value);
                    }
                }
            }

            return tempList;
        }
    }

    public class State
    {
        public UnityEngine.Object value;

        /// <summary>
        /// 层名字
        /// </summary>
        private string layerName;

        public string LayerName { get { return layerName; } }

        /// <summary>
        /// state 在 controller 中的路径
        /// </summary>
        private string statePath;

        public string StatePath { get { return statePath; } }

        public State()
        { }

        public State(AnimatorState value, string _layerName, string _statePath)
        {
            this.value = value;
            this.layerName = _layerName;
            this.statePath = _statePath;
        }

        public State(BlendTree value, string _layerName, string _statePath)
        {
            this.value = value;
            this.layerName = _layerName;
            this.statePath = _statePath;
        }

        /// <summary>
        /// 关联motion
        /// </summary>
        /// <param name="_prePath"></param>
        /// <param name="_filePath"></param>
        /// <param name="_log"></param>
        /// <returns></returns>
        public bool SetMotion(string _prePath, string _filePath, out string _log)
        {
            if (value == null)
            {
                _log = string.Format("{0}/{1}:value is null", layerName, statePath);

                return false;
            }

            //string motionName = string.Format("{0}{1}{2}", _prePath, value.name, ".FBX");
            string motionName = string.Format("{0}{1}{2}", _filePath, value.name, ".FBX");

            //Load AnimationClip
            AnimationClip clip = AssetDatabase.LoadAssetAtPath(motionName, typeof(AnimationClip)) as AnimationClip;
            if (clip != null)
            {
                if (value.GetType() == typeof(AnimatorState))
                {
                    ((AnimatorState)value).motion = clip;
                }
                else
                {
                    BlendTree blendTree = (BlendTree)value;
                    int index = int.Parse((statePath.Substring(statePath.Length - 3, 3)).Split('[', ']')[1]);

                    AnimatorCopy.OverrideBlendTree(blendTree, index, clip);
                }

                _log = string.Empty;

                return true;
            }

            _log = string.Format("Can't get AnimationClip by path : {0}{1}", _prePath, motionName);

            return false;
        }

        /// <summary>
        /// 移除motion
        /// </summary>
        public void RemoveMotion()
        {
            if (value.GetType() == typeof(AnimatorState))
            {
                AnimatorState animatorState = (AnimatorState)value;
                animatorState.motion = null;
            }
            else
            {
                BlendTree blendTree = (BlendTree)value;

                int index = int.Parse((statePath.Substring(statePath.Length - 3, 3)).Split('[', ']')[1]);

                AnimatorCopy.OverrideBlendTree(blendTree, index, null);
            }
        }
    }
}