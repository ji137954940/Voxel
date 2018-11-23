using UnityEngine;

/// <summary>
/// 进度条信息数据显示
/// 
/// 注意这个 脚本只在Buid-in 下使用
/// 正常使用LoadingPanel 方式
/// 
/// </summary>
public class LoadingWindow1 : MonoBehaviour
{
    #region UI数据引用

    public LoadingWindowData1 data;
    #endregion

    #region 数据参数

    /// <summary>
    /// 是否可以刷新显示
    /// </summary>
    bool can_update_view = true;
    #endregion

    private float _targetProgress;
    /// <summary>
    /// 目标进度
    /// </summary>
    private float targetProgress
    {
        get { return _targetProgress; }

        set
        {
            if (Mathf.Abs(value - _targetProgress) > 0.001f)
            {
                _targetProgress = value;

                currentRadio = 0;
            }
        }
    }

    /// <summary>
    /// 当前进度
    /// </summary>
    internal float currentProgress;

    /// <summary>
    /// 当前进度
    /// </summary>
    internal float currentRadio;

    public void Start()
    {

        if (data != null)
            data.SetSliderValue(0);
    }

    public void Update()
    {
        if (can_update_view)
        {
            if (!CodeBridgeTool.instance.GamePreLoadComplete)
            {
                targetProgress = CodeBridgeTool.instance.QueryTaskProgress();
            }
            else
            {
                targetProgress = 1f;
            }
            UpdateProgress();
        }
    }

    /// <summary>
    /// 刷帧显示进度
    /// </summary>
    void UpdateProgress()
    {
        if (data != null)
        {
            currentProgress = Mathf.Lerp(currentProgress, targetProgress, currentRadio);

            currentRadio += Time.deltaTime;

            currentRadio = Mathf.Clamp01(currentRadio);

            data.SetSliderValue(currentProgress);

            OnSetSliderNum((int)(currentProgress * 100));
        }
    }



    /// <summary>
    /// 设置数字显示
    /// </summary>
    /// <param name="num"></param>
    void OnSetSliderNum(int num)
    {
        if (data != null && data.text)
        {
            data.sliderNumber.num.text = num.ToString();
        }
    }

    public void RuntimeDestroySelf()
    {
        Object.Destroy(this);
    }

    /// <summary>
    /// 设置是否可以更新view
    /// </summary>
    /// <param name="can_update_view"></param>
    public void SetLoadingUpdate(bool can_update_view)
    {
        this.can_update_view = can_update_view;
    }
}
