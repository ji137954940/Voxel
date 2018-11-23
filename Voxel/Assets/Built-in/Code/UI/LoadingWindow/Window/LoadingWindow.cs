//using Tgame.Event.Login;
//using Tgame.Foundation;
//using Tgame.Game.Table;
//using Tgame.UI.Window;
//using UnityEngine;
//using ZFrame;
//using ZLib;
//using ZTool.Table;

//public class LoadingWindow : BaseUIWindow
//{
//    //存储界面关联引用
//    LoadingWindowData data;
//    LoadingWindowModule lwm;

//    private Vector2 rectSize;

//    //通知类型
//    public static LoadMessageTypeEnum type;

//    /// <summary>
//    /// 重载构造函数
//    /// </summary>
//    /// <param name="_module"></param>
//	public LoadingWindow(Module _module) : base(_module) { this.lwm = (LoadingWindowModule)_module; }

//    #region 界面展示

//    /// <summary>
//    /// 初始化数据
//    /// </summary>
//    /// <param name="data"></param>
//    public override void Init(BaseUIWindowData data)
//    {
//        this.data = (LoadingWindowData)data;
//        base.Init(data);
//        Restart();
//        //如果loading界面刚打开，配置初始化已经完成了，就直接进入 高速 读条阶段
//        LoginModule m = Frame.instance.GetModule<LoginModule>();
//        if (type == LoadMessageTypeEnum.Login && m != null && m.IsLoadConfigOver())
//        {
//            m_fRealProgress = 1;
//            //Debug.LogError("真的已经是1了 == " + m_fRealProgress);
//        }
//        else if (type == LoadMessageTypeEnum.Scene)
//        {
//            //走loading进入场景的时候打开一些需要显示的界面
//            UIWindowManager.instance.Open<TGMainWindowModule>();
//            UIWindowManager.instance.Open<RadarMainWindowModel>();
//            UIWindowManager.instance.Open<SkillMainWindowModel>();
//            UIWindowManager.instance.Open<TargetMainWindowModel>();
//            UIWindowManager.instance.Open<SelfMainWindowModel>();
//        }

//        this.data.SetSliderValue(0);
//        //打开界面是,添加刷帧,更新虚拟进度
//        Tick.AddUpdate(UpdateProgress);

//        rectSize = new Vector2(Screen.width, Screen.height);
//    }

//    #region 刷帧显示虚拟进度
//    //真实进度
//    float m_fRealProgress = 0;
//    //虚拟进度
//    float m_fSimProgress = 0;


//    /// <summary>
//    /// 刷帧显示进度
//    /// </summary>
//    private void UpdateProgress()
//    {
//        if (data != null)
//        {
//            //开始以一个相对比较慢的速度去走虚拟进度,如果真实进度完成了,就以一个比较快的速度去完成虚拟进度
//            if (m_fRealProgress >= 1)
//            {
//                m_fSimProgress += ConstantConfig.Loading_Fast_Speed * Time.deltaTime;
//                //m_fSimProgress += Time.deltaTime * 3;
//            }
//            else
//            {
//                m_fSimProgress += ConstantConfig.Loading_Slow_Speed * Time.deltaTime;
//            }



//            if (m_fSimProgress > 0.8f)
//            {
//                if (GameData.instance.IsPreResourcesLoadOver)
//                {

//                    m_fSimProgress = Mathf.Clamp01(m_fSimProgress);
//                    //刷新进度显示
//                    if (data != null /*&& data.slider != null*/ && data.sliderNumber != null)
//                    {
//                        data.SetSliderValue(m_fSimProgress);
//                        OnSetSliderNum((int)(m_fSimProgress * 100));
//                    }

//                    waitTime = 30f;
//                    //如果进度接近最大,发送虚拟进度达到最大的消息
//                    if (m_fSimProgress >= 1.00f)
//                    {

//                        //移除本次loading刷帧
//                        Tick.RemoveUpdate(UpdateProgress);

//                        //发送虚拟进度完成事件
//                        ME_Loading_FakeProgressShowComplete me_fpsc = ScriptPool<ME_Loading_FakeProgressShowComplete>.GetIdleObject(new object[] { type });
//                        Frame.DispatchEvent(me_fpsc);
//                    }

//                }
//                else
//                {
//                    m_fSimProgress = 0.8f;


//                    waitTime -= Time.deltaTime;
//                    if (waitTime <= 0)
//                    {
//                        UIWindowManager.instance.Open<RepeatNetModule>(null, null, new object[] { 3});
//                    }
//                }

//            }
//            else
//            {

//                //刷新进度显示
//                if (data != null /*&& data.slider != null*/ && data.sliderNumber != null)
//                {
//                    data.SetSliderValue(m_fSimProgress);
//                    OnSetSliderNum((int)(m_fSimProgress * 100));
//                }
//            }
//        }
//    }
//    float waitTime = 30f;


//    internal void Restart()
//    {
//        m_fSimProgress = 0;
//        m_fRealProgress = 0;
//        if (data != null)
//            data.SetSliderValue(0);
//    }

//    #endregion

//    /// <summary>
//    /// 设置提示文字显示
//    /// </summary>
//    /// <param name="text"></param>
//    public void SetText(string text)
//    {
//        if (data != null && data.text != null)
//            data.text.text = text;
//    }


//    /// <summary>
//    /// 设置进度条位置
//    /// </summary>
//    /// <param name="progress"></param>
//    public void SetProgress(float progress, LoadMessageTypeEnum type)
//    {
//        //Debug.LogError("这里才是set的地方 == " + progress);
//        m_fRealProgress = progress;
//        LoadingWindow.type = type;
//    }

//    /// <summary>
//    /// 设置数字显示
//    /// </summary>
//    /// <param name="num"></param>
//    void OnSetSliderNum(int num)
//    {
//        if (data != null && data.text)
//        {
//            data.sliderNumber.num.text = num.ToString();
//        }
//    }

//    public void SetLoadingBg(bool __loadOver, Texture2D __bgTexture)
//    {
//        //int id = Random.Range(10000340,10000344);
//        //var table = TableManager.GetTable<Table_Client_Icon>(id);
//        //if(table != null && id != 10000343)
//        //{
//        //    FoundationTools.SetRawImageTexture(data.loadingBG, table.path, LoadCallBack);
//        //}

//        if (__loadOver && __bgTexture != null)
//        {
//            data.loadingBG.texture = __bgTexture;
//            //data.rectTrans.sizeDelta = rectSize;
//        }
//    }

//    void LoadCallBack()
//    {
//        data.rectTrans.sizeDelta = rectSize;
//    }

//    #endregion

//    #region 数据清理
//    public override void Clear()
//    {
//        data.SetSliderValue(0);
//        m_fRealProgress = 0;
//        m_fSimProgress = 0;
//        //type = LoadMessageTypeEnum.None;

//        //SetProgress(0, LoadMessageTypeEnum.None);
//        OnSetSliderNum(0);

//        data = null;
//    }

//    #endregion
//}

