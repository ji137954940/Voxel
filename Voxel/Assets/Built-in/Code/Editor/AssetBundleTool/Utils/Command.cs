using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tgame.AssetBundle
{
    public class Command
    {

        /// <summary>
        /// 日志回调数据
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="stackTrace"></param>
        /// <param name="type"></param>
        public static void LogMessageReceived(string condition, string stackTrace, LogType type)
        {
            System.Console.Write(condition);
            System.Console.WriteLine();
            System.Console.Write(stackTrace);
            System.Console.WriteLine();
            System.Console.Write(type.ToString());
            System.Console.WriteLine();
        }

        /// <summary>
        /// 获取命令行信息数据
        /// </summary>
        /// <returns></returns>
        public static string GetCommandLineArgs()
        {
            foreach (string arg in System.Environment.GetCommandLineArgs())
            {
                if (arg.StartsWith("platform"))
                {
                    Debug.Log("获取命令行参数：" + arg.Split("="[0])[1]);
                    return arg.Split("="[0])[1];
                }
            }
            return "";
        }
    }
}


