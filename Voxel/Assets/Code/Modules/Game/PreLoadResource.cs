using System.Collections.Generic;
using System.Linq;
using System.Text;
using tgame.consts;
using Tgame.Game.Table;
using UnityEngine;
using ZLib;
using ZTool.Res;
using ZTool.Story.Table;
using Object = System.Object;
using Table_Client_Sound = ZTool.Story.Table.Table_Client_Sound;


/// <summary>
/// 预加载资源数据
/// </summary>
public class PreLoadResource : Singleton<PreLoadResource>
{
    /// <summary>
    /// 预加载的类型
    /// </summary>
    public enum PreLoadEnum
    {
        /// <summary>
        /// 无
        /// </summary>
        NONE = 0,
        /// <summary>
        /// 预加载玩家自己相关的资源信息
        /// </summary>
        PRE_LOAD_ABOUT_HERO_TYPE = 1,
        /// <summary>
        /// 预加载常驻的资源信息
        /// </summary>
        PRE_LOAD_RESIDENT_TYPE = 2,
        /// <summary>
        /// 进游戏之前的预加载，主要是一些常用UI
        /// </summary>
        PRE_LOAD_BEFORE_GMAE_TYPE = 3,
    }



    /// <summary>
    /// 所有的预加载表
    /// </summary>
    List<Table_Client_Pre_Load_Resources> _tbPreLoadArr;
    //Table_Client_Pre_Load_Resources[] _tbPreLoadArr;
    /// <summary>
    /// 每次需要load的预加载表
    /// </summary>
    List<Table_Client_Pre_Load_Resources> _tbLoadList = new List<Table_Client_Pre_Load_Resources>();
    /// <summary>
    /// 所有资源路径list
    /// </summary>
    Dictionary<string, ResPreLoadInfo> _resDic = new Dictionary<string, ResPreLoadInfo>();
    /// <summary>
    /// 实际需要加载的资源字典
    /// </summary>
    Dictionary<string, ResPreLoadInfo> _loadResDic = new Dictionary<string, ResPreLoadInfo>();
    /// <summary>
    /// 不需要的其他资源
    /// </summary>
    List<string> _otherResList = new List<string>();

    /// <summary>
    /// 当前的load类型
    /// </summary>
    PreLoadEnum _curLoadEnum;
    /// <summary>
    /// 资源路径索引
    /// </summary>
    int _resIndex = 0;
    /// <summary>
    /// 实际已经加载的index
    /// </summary>
    int _infactResIndex = 0;
    /// <summary>
    /// 实际需要加载的资源总数
    /// </summary>
    int _needLoadResCount = 0;
    /// <summary>
    /// 开始加载
    /// </summary>
    bool _startLoading = false;
    /// <summary>
    /// 是否显示了loading 界面，显示了需要完成一个加载的时候，发消息更新loading进度条
    /// </summary>
    bool _showLoading = false;

    //当前load的info
    ResPreLoadInfo _curLoadInfo;

    Dictionary<string, ResPreLoadInfo>.Enumerator _curEn;


    /// <summary>
    /// 显示的loading界面要一定时间没有移除就强制移除
    /// </summary>
    private float loadingTimer = 0;

    /// <summary>
    /// 加载最长时间限制
    /// </summary>
    private float loadingLimitTime = 5f;
    /// <summary>
    /// 当前种族id
    /// </summary>
    private int curRaceId;
    /// <summary>
    /// 当前职业
    /// </summary>
    private int curProfessionId;

    public int CurProfessionId { get { return curProfessionId; } }


    Action<PreLoadEnum, bool> OnComplete = null;

    Action<int, int> onProgress = null;

    #region 预加载接口

    /// <summary>
    /// 预加载，默认只有选人界面显示loading界面
    /// </summary>
    /// <param name="type"></param>
    /// <param name="isShowLoading">只有选人的时候设置为true，其他时候都是false</param>
    public void PreLoad(PreLoadEnum __type, bool isShowLoading = false, Action<PreLoadEnum, bool> onComplete = null, int professionId = 0, Action<int, int> onProgress = null)
    {
        if (OnComplete != null)
            throw new System.Exception("不允许同时预加载多个");

        this.OnComplete = onComplete;

        this.onProgress = onProgress;

        this.curProfessionId = professionId;

        CheckPrepare(__type, isShowLoading);

        //下面是在计算当前应该加载哪些
        if (_tbPreLoadArr == null)
            _tbPreLoadArr = Table_Client_Pre_Load_Resources.GetAllPrimaryList();

        if (_tbPreLoadArr != null)
        {
            int totalCount = _tbPreLoadArr.Count;
            Table_Client_Pre_Load_Resources tbProLoad = null;
            for (int i = 0; i < totalCount; i++)
            {
                tbProLoad = _tbPreLoadArr[i];
                //加载自己的资源的时候，区分种族和职业
                if (__type == PreLoadEnum.PRE_LOAD_ABOUT_HERO_TYPE)
                {
                    if (tbProLoad.raceid == curRaceId
                        && (tbProLoad.professionid == curProfessionId || tbProLoad.professionid == 0))
                    {
                        if (tbProLoad.type == (int)__type)
                        {
                            _tbLoadList.Add(tbProLoad);
                        }
                    }
                }
                else
                {
                    if (tbProLoad.type == (int)__type)
                    {
                        _tbLoadList.Add(tbProLoad);
                    }
                }
            }

            //需要加载的资源存入实际要加载的路径字典
            int tbLoadCount = _tbLoadList.Count;
            Table_Client_Pre_Load_Resources info = null;
            Table_Client_Model mode = null;
            Table_Client_Effect effect = null;
            Table_Client_Sound sound = null;
            ResPreLoadInfo res_pre_load = null;
            for (int i = 0; i < tbLoadCount; i++)
            {
                info = _tbLoadList[i];
                if (!string.IsNullOrEmpty(info.path))
                {
                    if (!ContainsRes(info.path) && !_loadResDic.ContainsKey(info.path))
                    {
                        if (info.resource_type_id == PreLoadResourceType.SOUND
                            || info.resource_type_id == PreLoadResourceType.SCENE
                            || info.resource_type_id == PreLoadResourceType.UI)
                        {
                            //资源类型数据为3(声音)， 4（场景）, 5 UI界面资源

                            res_pre_load = new ResPreLoadInfo();
                            res_pre_load.Init((PreLoadEnum)info.type, info.path, false, null, true, info.raceid, info.professionid, info.resource_type_id);
                        }
                        else
                        {
                            res_pre_load = new ResPreLoadInfo();
                            res_pre_load.Init((PreLoadEnum)info.type, info.path, false, null, true, info.raceid, info.professionid, info.resource_type_id);
                        }
                        _loadResDic.Add(info.path, res_pre_load);
                    }
                }
                else
                {
                    switch (info.resource_type_id)
                    {
                        case PreLoadResourceType.MODEL: //模型
                            mode = Table_Client_Model.GetPrimary(info.resource_id);
                            if (mode != null && !string.IsNullOrEmpty(mode.path) && !ContainsRes(mode.path) && !_loadResDic.ContainsKey(mode.path))
                            {
                                res_pre_load = new ResPreLoadInfo();
                                res_pre_load.Init((PreLoadEnum)info.type, mode.path, false, null, true, info.raceid, info.professionid, info.resource_type_id);
                                _loadResDic.Add(mode.path, res_pre_load);
                                continue;
                            }
                            break;
                        case PreLoadResourceType.EFFECT: //特效
                            effect = Table_Client_Effect.GetPrimary(info.resource_id);
                            if (effect != null && !string.IsNullOrEmpty(effect.path) && !ContainsRes(effect.path) && !_loadResDic.ContainsKey(effect.path))
                            {
                                res_pre_load = new ResPreLoadInfo();
                                res_pre_load.Init((PreLoadEnum)info.type, effect.path, false, null, true, info.raceid, info.professionid, info.resource_type_id);
                                _loadResDic.Add(mode.path, res_pre_load);
                                continue;
                            }
                            break;
                        case PreLoadResourceType.SOUND: //音效，声音的暂时不加载
                            sound = Table_Client_Sound.GetPrimary(info.resource_id);
                            if (sound != null && !string.IsNullOrEmpty(sound.bank_path) && !ContainsRes(sound.bank_path) && !_loadResDic.ContainsKey(sound.bank_path))
                            {
                                res_pre_load = new ResPreLoadInfo();
                                res_pre_load.Init((PreLoadEnum)info.type, sound.bank_path, false, null, true, info.raceid, info.professionid, info.resource_type_id);
                                _loadResDic.Add(mode.path, res_pre_load);
                                continue;
                            }
                            break;
                        //case PreLoadResourceType.SCENE: //场景资源
                        //    var sceneTable = Table_Scene.GetPrimary(info.resource_id);
                        //    var mapTable = Table_Map.GetPrimary(sceneTable.map_id);
                        //    if (mapTable != null
                        //        && !string.IsNullOrEmpty(mapTable.path)
                        //        && !ContainsRes(mapTable.path)
                        //        && !_loadResDic.ContainsKey(mapTable.path))
                        //    {
                        //        _loadResDic.Add(mapTable.path, DispatchEventManager.instance.DispatchResPreLoadInfo((PreLoadEnum)info.type, sceneTable.map_id.ToString(), false, null, true, info.raceid, info.professionid, info.resource_type_id));
                        //        continue;
                        //    }
                        //    break;
                        //case PreLoadResourceType.UI: //UI界面资源
                        //    var uiConfig = Table_Client_Ui_Config_New.GetPrimary(info.resource_id);
                        //    if (uiConfig != null
                        //        && !string.IsNullOrEmpty(uiConfig.ui_res_name)
                        //        && !ContainsRes(uiConfig.ui_res_name)
                        //        && !_loadResDic.ContainsKey(uiConfig.ui_res_name))
                        //    {
                        //        _loadResDic.Add(uiConfig.ui_res_name, DispatchEventManager.instance.DispatchResPreLoadInfo((PreLoadEnum)info.type, info.resource_id.ToString(), false, null, true, info.raceid, info.professionid, info.resource_type_id));
                        //        continue;
                        //    }
                        //    break;
                    }
                }
            }
        }

        //如果实际需要加载的数量大于0,则加载
        _needLoadResCount = _loadResDic.Count;
        _curEn = _loadResDic.GetEnumerator();
        //Debug.LogError("_needLoadResCount == " + _needLoadResCount);
        //foreach (var item in _loadResList)
        //{
        //    Debug.Log("需要加载 == " + item.path + " ??? preLoadEnum == " + item.preLoadEnum);
        //}
        if (_needLoadResCount > 0)
        {
            _startLoading = true;
            GameData.instance.IsPreResourcesLoadOver = false;

            LoadAll();
        }
        else
        {
            //if (__type == PreLoadEnum.PRE_LOAD_RESIDENT_TYPE)
            //{
            //    LoadingManager.instance.UpdateEnterSceneResPreLoadProgress(1f);
            //    var task = GameData.GetLoadingTaskByID(LoadingTaskID.EnterSceneResPreLoad);
            //    if (task != null)
            //        task.CurrentProgress = 100;
            //    else
            //        Debug.LogError("不存在任务EnterSceneResPreLoad");
            //}
        }
    }

    public void PreLoad(PreLoadEnum type, List<string> list, bool isShowLoading = false, Action<PreLoadEnum, bool> onComplete = null, Action<int, int> onProgress = null)
    {
        if (OnComplete != null)
            throw new System.Exception("不允许同时预加载多个");

        this.OnComplete = onComplete;

        this.onProgress = onProgress;

        CheckPrepare(type, isShowLoading);
        int count = list.Count;
        ResPreLoadInfo info = null;
        for (int i = 0; i < count; i++)
        {
            if (!ContainsRes(list[i]) && !_loadResDic.ContainsKey(list[i]))
            {
                info = new ResPreLoadInfo();
                info.Init(type, list[i], false, null, true, curRaceId, curProfessionId, 0);
                _loadResDic.Add(list[i], info);
            }
        }
        _needLoadResCount = _loadResDic.Count;
        _curEn = _loadResDic.GetEnumerator();

        if (_needLoadResCount > 0)
        {
            _startLoading = true;
            GameData.instance.IsPreResourcesLoadOver = false;

            LoadAll();
        }
        else
        {
            //if (type == PreLoadEnum.PRE_LOAD_RESIDENT_TYPE)
            //{
            //    var task = GameData.GetLoadingTaskByID(LoadingTaskID.EnterSceneResPreLoad);

            //    if (task != null)
            //        task.CurrentProgress = 100;
            //    else
            //        Debug.LogError("任务不存在EnterSceneResPreLoad");

            //    LoadingManager.instance.UpdateEnterSceneResPreLoadProgress(1f);
            //}

        }
    }

    /// <summary>
    /// 重置数据
    /// </summary>
    /// <param name="type"></param>
    /// <param name="isShowLoading"></param>
    private void CheckPrepare(PreLoadEnum type, bool isShowLoading)
    {
        _curLoadEnum = type;
        _showLoading = isShowLoading;
        _resIndex = 0;
        _infactResIndex = 0;
        _needLoadResCount = 0;
        loadingTimer = Time.time;
        _startLoading = false;
        _loadResDic.Clear();
        _tbLoadList.Clear();
    }

    /// <summary>
    /// 是否已存在资源
    /// </summary>
    /// <param name="__path"></param>
    /// <returns></returns>
    private bool ContainsRes(string __path)
    {
        return _resDic.ContainsKey(__path);
    }

    #endregion

    /// <summary>
    /// 立即加载全部需要加载的资源
    /// </summary>
    private void LoadAll()
    {
        using (var ie = _loadResDic.GetEnumerator())
        {
            while (ie.MoveNext())
            {
                var _curLoadInfo = ie.Current.Value;

                if (!string.IsNullOrEmpty(_curLoadInfo.path))
                {
                    switch (_curLoadInfo.resourceType)
                    {
                        //case PreLoadResourceType.SOUND:
                        //    //("path===>>load :" + _curLoadInfo.path).SetLog(ColorEnum.red);
                        //    ResourcesLoad.instance.LoadAssetBundle(_curLoadInfo.path, OnResourceLoadOver, _curLoadInfo, isClone: false);
                        //    break;
                        //case PreLoadResourceType.SCENE:
                        //    PreLoadScene(_curLoadInfo.path);
                        //    break;
                        //case PreLoadResourceType.UI:

                        //    PreLoadUIRes(_curLoadInfo.path);
                        //    break;
                        default:
                            ResourcesLoad.instance.LoadAssetBundle(_curLoadInfo.path, OnResourceLoadOver, _curLoadInfo, _curLoadInfo.res_name, _curLoadInfo.isClone, _curLoadInfo.is_only_bundle);
                            break;
                    }
                }
            }
        }
    }

    #region 资源加载完成

    void OnResourceLoadOver(string path, Object obj, object parameter)
    {
        var info = parameter as ResPreLoadInfo;

        if (obj != null && info != null)
        {
            //_resIndex++;
            if (info.IsObjNull())
            {
                info.loadObject = obj;
            }

            if (!_resDic.ContainsKey(info.path))
            {
                _resDic.Add(info.path, info);
            }
            else
            {
                Debug.LogError("已经包含 == " + info.path);
                //卸载数据
                ResourcesLoad.instance.UnLoadAssetBundle(info.path, OnResourceLoadOver);
                //if (obj as GameObject)
                //    Object.Destroy(obj);
            }

            //如果是声音，要先加载bank包
            if (info.resourceType == PreLoadResourceType.SOUND)
            {
                //try
                //{
                //    //Debug.Log("预热bank包 , path = " + path);
                //    RuntimeManager.LoadBank(obj as TextAsset, true);
                //}
                //catch (BankLoadException e)
                //{
                //    //Debug.LogException(e);
                //    Debug.Log("预加载bank包出错 , 异常信息 " + e.ToString() + "\n 堆栈：\n" + e.StackTrace);
                //}

                //FMODManager.instance.Register(path);
                //RuntimeManager.WaitForAllLoads();
                //("unload====>>>>" + info.path).SetLog(ColorEnum.yellow);
                ResourcesLoad.instance.UnLoadAssetBundle(info.path, OnResourceLoadOver);
            }

            //检测资源是否加载完成
            CheckPreLoadDone();
        }
        else
        {
            Debug.LogError(string.Format("path == {0},obj is null", path));
            //遇到空对象，直接  跳出
            ClearLoadingDialog();

            ExecuteDone(true);
        }
    }

    #endregion

    #region 资源预先加载完成设置

    /// <summary>
    /// 执行进度回调
    /// </summary>
    private void UpdateProgress()
    {
        if (onProgress != null)
            onProgress(_infactResIndex, _needLoadResCount);
    }

    /// <summary>
    /// 在执行完毕的时候调用
    /// <param name="isBroken">是否在打断的情况下调用的</param>
    /// </summary>
    private void ExecuteDone(bool isBroken)
    {
        if (OnComplete != null)
        {
            OnComplete(_curLoadEnum, isBroken);

            OnComplete = null;

            onProgress = null;
        }
    }

    /// <summary>
    /// 检测预先加载资源是否完成
    /// </summary>
    private void CheckPreLoadDone()
    {
        _infactResIndex++;

        UpdateProgress();

        if (_infactResIndex >= _needLoadResCount)
        {
            if (_curLoadEnum == PreLoadEnum.PRE_LOAD_RESIDENT_TYPE)
            {
                ////预加载进入游戏场景数据
                //LoadingManager.instance.UpdateEnterSceneResPreLoadProgress(1f);
                //var task = GameData.GetLoadingTaskByID(LoadingTaskID.EnterSceneResPreLoad);
                //if (task != null)
                //{
                //    task.CurrentProgress = 100;
                //}
                //else
                //{
                //    Debug.LogError("没有任务EnterSceneResPreLoad");
                //}
            }

            ExecuteDone(false);
        }
    }

    #endregion

    private void ClearLoadingDialog()
    {
        _showLoading = false;
    }


    #region 卸载

    /// <summary>
    /// 根据类型卸载相应的部分资源
    /// </summary>
    /// <param name="type">要卸载的资源类型</param>
    public void UnLoad(PreLoadEnum __type, bool total = true)
    {
        if (total)
        {
            _otherResList.Clear();

            //卸载当前类型全部资源
            for (var item = _resDic.GetEnumerator(); item.MoveNext();)
            {
                ResPreLoadInfo info = item.Current.Value;
                if (info.preLoadEnum == __type)
                    _otherResList.Add(info.path);
            }
        }

        int otherCount = _otherResList.Count;
        for (int i = 0; i < otherCount; i++)
        {
            if (_resDic.ContainsKey(_otherResList[i]))
            {
                ResPreLoadInfo info = _resDic[_otherResList[i]];
                //info.DestroyObj();

                _resDic.Remove(_otherResList[i]);

                switch (info.resourceType)
                {
                    case PreLoadResourceType.SCENE:
                        UGE.mapMgr.UnloadMap();
                        break;
                    case PreLoadResourceType.SOUND:
                        //如果是声音，要先卸载bank包
                        //RuntimeManager.UnloadBank(info.path);
                        ResourcesLoad.instance.UnLoadAssetBundle(info.path, OnResourceLoadOver);
                        break;
                    default:
                        ResourcesLoad.instance.UnLoadAssetBundle(info.path, OnResourceLoadOver);
                        break;
                }
                info = null;
            }
        }
    }
    #endregion

    /// <summary>
    /// 预加载的单个数据信息
    /// </summary>
    public class ResPreLoadInfo
    {
        //load 类型
        public PreLoadEnum preLoadEnum;
        //路径
        public string path;
        //是否clone
        public bool isClone;
        //种族id
        public int raceId;
        //职业id
        public int professionId;
        //加载的对象
        public object loadObject;
        //assetbundle name
        public string res_name;
        /// <summary>
        /// 资源类型
        /// </summary>
        public int resourceType;

        //是否只是bundle加载
        public bool is_only_bundle;

        /// <summary>
        /// init
        /// </summary>
        /// <param name="type"></param>
        /// <param name="path"></param>
        /// <param name="isClone"></param>
        /// <param name="res_name"></param>
        /// <param name="is_only_bundle"></param>
        /// <param name="raceId"></param>
        /// <param name="professionId"></param>
        /// <param name="resourceType"></param>
        public void Init(PreLoadEnum type, string path, bool isClone, string res_name, bool is_only_bundle, int raceId, int professionId, int resourceType)
        {
            this.preLoadEnum = type;
            this.path = path;
            this.isClone = isClone;
            this.res_name = res_name;
            this.is_only_bundle = is_only_bundle;
            this.raceId = raceId;
            this.professionId = professionId;
            this.resourceType = resourceType;
        }

        public bool IsObjNull()
        {
            return loadObject == null;
        }

        public void DestroyObj()
        {
            if (loadObject != null && loadObject as GameObject)
            {
                //Destroy(loadObject as GameObject);
                loadObject = null;
            }
        }

        public void Clear()
        {
            preLoadEnum = PreLoadEnum.NONE;
            path = null;
            isClone = false;
            raceId = 0;
            professionId = 0;
            res_name = null;
            is_only_bundle = false;
            resourceType = 0;
            DestroyObj();
        }
    }

}

