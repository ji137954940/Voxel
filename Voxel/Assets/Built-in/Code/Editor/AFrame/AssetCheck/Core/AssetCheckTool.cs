using System.Collections;
using System.Collections.Generic;
using System.IO;
using Tgame.AssetBundle;
using UnityEditor;
using UnityEngine;

namespace AFrame.AssetCheck
{
	/// <summary>
	/// 资源检查公共类
	/// @author LiuLeiLei
	/// @data 3/2/2018
	/// @desc 
	/// </summary>
	public class AssetCheckTool 
	{
        #region Material

        /// <summary>
        /// 后期可以考虑改为走配置
        /// </summary>
        private static string[] useShaderArr = new string[] { "Particles/Additive (Soft)", "Particles/Alpha Blended Premultipy", "Mobile/Diffuse", "TGame/Particle/Additive SA" };

        /// <summary>
        /// 材质球Shader.name 校验
        /// </summary>
        /// <param name="_name"></param>
        /// <returns></returns>
        public static bool ShaderNameEquils(string _name)
        {
            int useShaderArrLen = useShaderArr.Length;
            for (int i = 0; i < useShaderArrLen; i++)
            {
                if (string.Equals(_name, useShaderArr[i]))
                    return true;
            }

            return false;
        }

        #endregion

        #region Animation

        private static string animEventName = "OnStartShakeCamera";

        public static bool IsShakeCameraName(string _name)
        {
            return string.Equals(_name,animEventName);
        }

        #endregion

        #region 资源检查开关

        /// <summary>
        /// 获得资源导入开启状态
        /// </summary>
        /// <returns></returns>
        public static bool OwnCheckOpen()
        {
            var currentDir = BuildUtils.GetUnityPath(Path.GetFullPath(Directory.GetCurrentDirectory()));
            return EditorPrefs.GetBool(currentDir);
        }

        /// <summary>
        /// 设置本地项目资源导入检查开启状态
        /// </summary>
        /// <param name="_open"></param>
        public static void SetOwnCheckOpenState(bool _open)
        {
            var currentDir = BuildUtils.GetUnityPath(Path.GetFullPath(Directory.GetCurrentDirectory()));
            EditorPrefs.SetBool(currentDir,_open);
        }

        #endregion
    }
}