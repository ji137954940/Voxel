using System;
using System.Diagnostics;

namespace Tgame.AssetBundle
{
    /// <summary>
    /// 测试代码执行时间
    /// </summary>
    public class ExecuteTimer : IDisposable
    {
        private string _tag;
        private Stopwatch _stopWatch;

        public ExecuteTimer(string tag)
        {
            _tag = tag;
            _stopWatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            _stopWatch.Stop();

#if !ONLYCSHARP
            UnityEngine.Debug.Log(string.Format("'{0}' exec time: {1:0.000} (ms)", _tag, ((double)_stopWatch.ElapsedTicks / (double)Stopwatch.Frequency) * 1000));
#else
		Console.WriteLine(string.Format("'{0}' exec time: {1:0.000} (ms)", _tag, ((double)_stopWatch.ElapsedTicks / (double)Stopwatch.Frequency) * 1000));
#endif
        }
    }
}

