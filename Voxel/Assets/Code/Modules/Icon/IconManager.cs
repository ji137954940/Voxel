using System.Collections.Generic;
using Color.Number.Utils;
using Tgame.Game.Table;
using UnityEngine;
using UnityEngine.UI;
using ZLib;
using ZTool.Res;
using ZTool.Table;
using System.IO;

namespace Tgame.Game.Icon
{
    /// <summary>
    /// Icon 数据管理
    /// </summary>
    public class IconManager : Singleton<IconManager>
    {
        /// <summary>
        /// 灰色材质球
        /// </summary>
        private Material _greyMat;

        /// <summary>
        /// 更换Sprite 图标
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="sprite"></param>
        /// <param name="typeId"></param>
        /// <param name="id"></param>
        /// <param name="callback"></param>
        /// <param name="isGrey"></param>
        /// <param name="isAutoUse"></param>
        /// <param name="setNative"></param>
        public void SetIcon(Object obj, Image sprite, 
            int typeId, int id, Action<int, int, Object> callback, 
            bool isGrey = false, bool isAutoUse = true, bool setNative = false)
        {
            if (obj == null
                || sprite == null
                || typeId == 0
                || id == 0)
                return;

            var icon = Table_Client_Icon.GetPrimary(typeId, id);
            if (icon != null)
            {
                OnSetIconRes(obj, sprite, icon.path, icon.name, typeId, id, callback, isAutoUse, setNative);
            }

            SetImageGrey(sprite, isGrey);
        }

        /// <summary>
        /// 更换Sprite 图标(RawImage)
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="sprite"></param>
        /// <param name="typeId"></param>
        /// <param name="id"></param>
        /// <param name="callback"></param>
        /// <param name="isGrey"></param>
        /// <param name="isAutoUse"></param>
        public void SetIcon(Object obj, RawImage sprite, 
            int typeId, int id, Action<int, int, Object> callback, 
            bool isGrey = false, bool isAutoUse = true)
        {
            if (obj == null
                || sprite == null
                || typeId == 0
                || id == 0)
                return;

            var icon = Table_Client_Icon.GetPrimary(typeId, id);
            if (icon != null)
            {
                OnSetIconRes(obj, sprite, icon.path, icon.name, typeId, id, callback, isAutoUse);
            }

            SetImageGrey(sprite, isGrey);
        }

        /// <summary>
        /// 更换Sprite 图标(RawImage)
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="sprite"></param>
        /// <param name="typeId"></param>
        /// <param name="id"></param>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        /// <param name="isGrey"></param>
        /// <param name="isAutoUse"></param>
        public void SetIcon(Object obj, RawImage sprite,
            int typeId, int id, string path, Action<int, int, Object> callback,
            bool isGrey = false, bool isAutoUse = true)
        {
            if (obj == null
                || sprite == null
                || typeId == 0
                || id == 0
                || string.IsNullOrEmpty(path))
                return;

            //var icon = Table_Client_Icon.GetPrimary(typeId, id);
            //if (icon != null)
            //{
            //    OnSetIconRes(obj, sprite, path, icon.name, typeId, id, callback, isAutoUse);
            //}

            LoadCacheIcon(obj, sprite, path, path, typeId, id, callback, isAutoUse);

            SetImageGrey(sprite, isGrey);
        }

        /// <summary>
        /// 灰色材质球加载完成，为资源赋值
        /// </summary>
        /// <param name="path"></param>
        /// <param name="obj"></param>
        /// <param name="parameter"></param>
        private void OnGreyMatLoadOver(string path, Object obj, object parameter)
        {
            if (obj != null)
            {
                _greyMat = obj as Material;
                var image = parameter as MaskableGraphic;
                if (image != null)
                {
                    image.material = _greyMat;
                }
            }
        }

        /// <summary>
        /// 更换Sprite 图标 ； 适用于单张图片一个图集的更换
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="sprite"></param>
        /// <param name="path"></param>
        public void SetIcon(Object obj, Image sprite, int typeId, int id, Action<int, int, Object> callback, string path)
        {
            if (obj == null || sprite == null || string.IsNullOrEmpty(path))
                return;

            OnSetIconRes(obj, sprite, path, System.IO.Path.GetFileName(path), typeId, id, callback);
        }

        /// <summary>
        /// 更换Sprite图标
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="sprite"></param>
        /// <param name="url"></param>
        /// <param name="name"></param>
        /// <param name="typeId"></param>
        /// <param name="id"></param>
        /// <param name="callback"></param>
        /// <param name="isAutoUse"></param>
        /// <param name="setNative"></param>
        /// <param name="callbackFun"></param>
        public void OnSetIconRes(Object obj, Object sprite, string url, string name, 
            int typeId, int id, Action<int, int, Object> callback,
            bool isAutoUse = true, bool setNative = false, Action callbackFun = null)
        {
            if (!IsAlreadyLoadRes(obj, sprite, url, name, typeId, id, callback, isAutoUse, setNative))
            {
                int spriteType = 1;
                if (sprite is Image)
                {
                    spriteType = 1;
                }
                else if (sprite is RawImage)
                {
                    spriteType = 2;
                }

                //当前资源不存在，那么就加载
                //ResIconInfo info = ScriptPool<ResIconInfo>.GetIdleObject(new object[] { url, name, spriteType });
                ResIconInfo info = ScriptPool<ResIconInfo>.GetIdleObject();
                info.Init(url, name, spriteType);
                info.OnSetRealtionObj(obj);

                //设置等待加载完成之后回调的数据资源
                var waitCallBack = ScriptPool<ResIconWaitCallBack>.GetIdleObject();
                waitCallBack.SetInfo(sprite, name, typeId, id, callback, isAutoUse, setNative);
                info.SetWaitCallBack(waitCallBack);
                res_iconDic[url.ToLower()] = info;

                ResourcesLoad.instance.LoadAssetBundle(url, OnLoadResOver);
            }
        }

        /// <summary>
        /// 把图片设置为灰态；注意：不会替换图片
        /// </summary>
        /// <param name="sprite">要设置为灰态的图片</param>
        /// <param name="isGrey">是否灰态</param>
        public void SetImageGrey(MaskableGraphic sprite, bool isGrey)
        {
            if (isGrey)
            {
                if (_greyMat == null)
                {
                    ResourcesLoad.instance.LoadAssetBundle("Res/UI/Materials/ImageGrey", OnGreyMatLoadOver, sprite, isClone: false);
                }
                else
                {
                    OnGreyMatLoadOver(string.Empty, _greyMat, sprite);
                }
            }
            else
            {
                //这个以后会不会有问题
                if (sprite.material != null)
                {
                    sprite.material = null;
                }
            }
        }

        /// <summary>
        /// 当前是否已经存在需要的数据，如果存在，那么就直接切换
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="sprite"></param>
        /// <param name="url"></param>
        /// <param name="name"></param>
        /// <param name="typeId"></param>
        /// <param name="id"></param>
        /// <param name="callback"></param>
        /// <param name="setNative"></param>
        /// <returns></returns>
        bool IsAlreadyLoadRes(Object obj, Object sprite, string url, string name,
            int typeId, int id, Action<int, int, Object> callback,
            bool isAutoUse = true, bool setNative = false)
        {
            if (sprite != null)
            {
                url = url.ToLower();

                if (res_iconDic.ContainsKey(url))
                {
                    ResIconInfo info = res_iconDic[url];

                    info.OnSetRealtionObj(obj);

                    if (info.IsLoadOver)
                    {
                        if (sprite is Image)
                        {
                            var sp = info.GetSprite(name) as UnityEngine.Sprite;
                            if (isAutoUse)
                            {
                                var image = sprite as Image;

                                image.overrideSprite = sp;
                                image.sprite = sp;

                                if (setNative)
                                    image.SetNativeSize();
                            }

                            if (callback != null)
                                callback(typeId, id, sp);
                        }
                        else if (sprite is RawImage)
                        {
                            var tex = info.GetTexture2D(name) as UnityEngine.Texture2D;
                            if (isAutoUse)
                            {
                                var rw = sprite as RawImage;

                                rw.texture = tex;
                            }

                            if (callback != null)
                                callback(typeId, id, tex);
                        }
                    }
                    else
                    {
                        //等待资源加载完成
                        var waitCallback = ScriptPool<ResIconWaitCallBack>.GetIdleObject();
                        waitCallback.SetInfo(sprite, name, typeId, id, callback, isAutoUse, setNative);
                        info.SetWaitCallBack(waitCallback);
                    }

                    return true;
                }
            }

            return false;
        }

        #region 已经加载的资源数据

        //存储当前已经加载出来的资源数据
        Dictionary<string, ResIconInfo> res_iconDic = new Dictionary<string, ResIconInfo>();

        /// <summary>
        /// 资源数据加载完成之后的回调数据
        /// </summary>
        /// <param name="path"></param>
        /// <param name="obj"></param>
        /// <param name="parameter"></param>
        void OnLoadResOver(string path, Object[] objs, object parameter)
        {
            if (string.IsNullOrEmpty(path))
                return;

            path = path.ToLower();
            path = GameUtils.GetFullPathWithoutExtension(path);

            if (res_iconDic.ContainsKey(path))
            {
                ResIconInfo info = res_iconDic[path];
                if (info != null)
                {
                    if (info.spriteType == 1)
                    {
                        info.SetSprites(objs);
                        info.CallBack();
                    }
                    else if (info.spriteType == 2)
                    {
                        info.SetSpritesTextrure2D(objs);
                        info.CallBack();
                    }

                }
            }
            else
            {
                Debug.LogError("!!!!!!IconManager出错!!!!!!!! 在加载容器中不存在");
            }
        }


        #endregion

        #region 资源数据卸载

        /// <summary>
        /// 检测资源数据信息，查看此数据是否可以卸载
        /// </summary>
        public void CheckResIconInfo()
        {
            List<ResIconInfo> list = new List<ResIconInfo>(res_iconDic.Values);
            for (int i = 0; i < list.Count;)
            {
                ResIconInfo info = list[i];
                if (info != null && info.IsHaveRelationObj())
                {
                    list.RemoveAt(i);
                    info.RecoveryScriptPool();
                }
                else
                {
                    i++;
                }
            }
        }

        #endregion

        #region 存储资源加载数据

        /// <summary>
        /// 存储当前资源的加载数据
        /// </summary>
        class ResIconInfo : BaseScriptPool
        {
            /// <summary>
            /// 资源地址
            /// </summary>
            public string Url { get; private set; }
            /// <summary>
            /// 资源名字
            /// </summary>
            public string ResName { get; private set; }
            /// <summary>
            /// 关联引用的对象
            /// </summary>
            public List<Object> relationList { get; private set; }

            /// <summary>
            /// 资源是否已经加载完成
            /// </summary>
            public bool IsLoadOver { get; private set; }

            /// <summary>
            /// 资源加载完成之后等待回调的数据
            /// </summary>
            public List<ResIconWaitCallBack> waitCallBackList { get; private set; }

            /// <summary>
            /// 图集资源中包含的数据
            /// </summary>
            Dictionary<string, Object> spriteDic = null;

            //1 image，2 rawImage
            public int spriteType;

            #region 初始化数据

            /// <summary>
            /// 初始化数据信息
            /// </summary>
            /// <param name="objs"></param>
            /// <returns></returns>
            public override bool Init(object[] objs)
            {
                //if (objs != null && objs.Length > 1)
                //{
                //    Url = objs[0].ToString();
                //    ResName = objs[1].ToString();
                //    spriteType = (int)objs[2];
                //    return true;
                //}
                //return false;

                return true;
            }

            /// <summary>
            /// 初始化数据信息
            /// </summary>
            /// <param name="url"></param>
            /// <param name="resName"></param>
            /// <param name="spriteType"></param>
            public void Init(string url, string resName, int spriteType)
            {
                this.Url = url;
                this.ResName = resName;
                this.spriteType = spriteType;
            }

            #endregion

            #region 设置，获取数据资源

            /// <summary>
            /// 存储等待资源加载完成之后的回调数据
            /// </summary>
            /// <param name="callback"></param>
            public void SetWaitCallBack(ResIconWaitCallBack callback)
            {
                if (callback != null)
                {
                    if (waitCallBackList == null)
                        waitCallBackList = new List<ResIconWaitCallBack>();
                    waitCallBackList.Add(callback);
                }
            }

            /// <summary>
            /// 资源数据加载完成，开始回调数据资源
            /// </summary>
            public void CallBack()
            {
                for (int i = waitCallBackList.Count - 1; i >= 0; i--)
                {
                    var callback = waitCallBackList[i];

                    waitCallBackList.RemoveAt(i);

                    if (callback.Sprite is Image)
                    {
                        var sprite = GetSprite(callback.Name) as UnityEngine.Sprite;

                        callback.CallBack(sprite);
                    }
                    else if (callback.Sprite is RawImage)
                    {
                        callback.CallBack(GetTexture2D(callback.Name) as UnityEngine.Texture2D);
                    }

                    callback.RecoveryScriptPool();
                }

                waitCallBackList.Clear();
            }

            /// <summary>
            /// 资源加载完成之后设置数据资源
            /// </summary>
            /// <param name="obj"></param>
            public void SetSprites(Object[] objs)
            {
                if (objs == null)
                    return;

                if (spriteDic == null)
                    spriteDic = new Dictionary<string, Object>();

                int count = objs.Length;
                for (int i = 1; i < count; i++)
                {
                    var s = objs[i];
                    spriteDic[s.name] = s;
                }
                IsLoadOver = true;
            }

            /// <summary>
            /// 资源加载完成之后设置数据资源
            /// </summary>
            /// <param name="obj"></param>
            public void SetSpritesTextrure2D(Object[] objs)
            {
                if (objs == null)
                    return;

                if (spriteDic == null)
                    spriteDic = new Dictionary<string, Object>();

                int count = objs.Length;
                for (int i = 0; i < count; i++)
                {
                    var s = objs[i] as Texture2D;
                    if(s != null)
                        spriteDic[s.name] = s;
                }
                IsLoadOver = true;
            }

            /// <summary>
            /// 获得一个Sprite资源数据
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public Object GetSprite(string name)
            {
                if (spriteDic == null || spriteDic.Count == 0
                    || string.IsNullOrEmpty(name))
                    return null;

                Object obj = null;
                if (spriteDic.TryGetValue(name, out obj))
                {
                    return obj;
                }
                else
                {
                    Debug.Log(string.Format("当前图集中没有找到此精灵SpriteName = {0}", name));
                }
                return null;
            }

            /// <summary>
            /// 获得一个Sprite资源数据
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public Object GetTexture2D(string name)
            {
                if (spriteDic == null || spriteDic.Count == 0
                    || string.IsNullOrEmpty(name))
                    return null;

                Object obj = null;
                if (spriteDic.TryGetValue(name, out obj))
                {
                    return obj;
                }
                else
                {
                    Debug.Log(string.Format("当前图集中没有找到此精灵SpriteName = {0}", name));
                }

                return null;
            }

            #endregion

            #region 引用关联数据

            /// <summary>
            /// 设置引用当前资源数据的资源（那个数据在使用此数据）
            /// </summary>
            /// <param name="obj"></param>
            public void OnSetRealtionObj(UnityEngine.Object obj)
            {
                if (relationList == null)
                    relationList = new List<Object>();

                //如果不包含此数据，那么就存储
                if (!relationList.Contains(obj))
                    relationList.Add(obj);
            }

            /// <summary>
            /// 检测当前是否还有引用此资源的数据存在
            /// </summary>
            /// <returns></returns>
            public bool IsHaveRelationObj()
            {
                return GameUtils.IsHaveNoValue(relationList);
            }

            #endregion

            #region 清理数据

            /// <summary>
            /// 清理数据信息
            /// </summary>
            public override void Clear()
            {
                Url = null;
                ResName = null;
                relationList.Clear();
                IsLoadOver = false;
                waitCallBackList.Clear();
                spriteDic.Clear();
            }

            /// <summary>
            /// 回收脚本对象数据
            /// </summary>
            public override void RecoveryScriptPool()
            {
                ScriptPool<ResIconInfo>.RecoveryObject(this);
            }

            #endregion
        }

        /// <summary>
        /// 当前资源等待回调数据（资源加载完成之后会使用此数据结构）
        /// </summary>
        class ResIconWaitCallBack : BaseScriptPool
        {
            //使用的资源Sprite数据
            public UnityEngine.Object Sprite { get; private set; }
            //Sprite使用的数据名字
            public string Name { get; private set; }
            //是否自动使用新资源
            private bool IsAutoUse { get; set; }
            public bool SetNative { get; private set; }

            //回调数据信息
            public int TypeId { get; private set; }

            public int Id { get; private set; }

            private Action<int, int, Object> callback;

            #region 初始化数据

            /// <summary>
            /// 初始化数据信息
            /// </summary>
            /// <param name="objs"></param>
            /// <returns></returns>
            public override bool Init(object[] objs)
            {
                //if (objs != null && objs.Length > 1)
                //{
                //    if (objs[0] is Image)
                //    {
                //        Sprite = (Image)objs[0];
                //    }
                //    else
                //    {
                //        Sprite = (RawImage)objs[0];
                //    }

                //    Name = objs[1].ToString();
                //    return true;
                //}
                return true;
            }

            #endregion

            /// <summary>
            /// 设置资源数据信息
            /// </summary>
            /// <param name="imageObj"></param>
            /// <param name="name"></param>
            /// <param name="typeId"></param>
            /// <param name="id"></param>
            /// <param name="callback"></param>
            /// <param name="isAutoUse"></param>
            /// <param name="setNative"></param>
            public void SetInfo(object imageObj, string name, 
                int typeId, int id, Action<int, int, Object> callback, 
                bool isAutoUse, bool setNative)
            {
                if (imageObj != null)
                {
                    if (imageObj.IsInstance<Image>())
                        this.Sprite = imageObj as Image;
                    else if (imageObj.IsInstance<RawImage>())
                        this.Sprite = imageObj as RawImage;
                }

                this.Name = name;
                this.IsAutoUse = isAutoUse;
                this.SetNative = setNative;

                this.TypeId = typeId;
                this.Id = id;
                this.callback += callback;
            }

            #region 设置数据资源

            /// <summary>
            /// 加载完成之后的数据回调
            /// </summary>
            /// <param name="sprite"></param>
            public void CallBack(Object sprite)
            {
                if (Sprite != null)
                {
                    if (Sprite is Image)
                    {
                        var sp = sprite as UnityEngine.Sprite;

                        if (IsAutoUse)
                        {
                            var image = Sprite as Image;

                            image.overrideSprite = sp;
                            image.sprite = sp;
                            if (SetNative)
                                image.SetNativeSize();
                        }

                        if (callback != null)
                        {
                            callback(TypeId, Id, sp);
                            callback = null;
                        }
                    }
                    else if (Sprite is RawImage)
                    {
                        var texture = sprite as UnityEngine.Texture2D;
                        if (IsAutoUse)
                        {
                            var raw = Sprite as RawImage;
                            raw.texture = texture;
                        }

                        if (callback != null)
                        {
                            callback(TypeId, Id, texture);
                            callback = null;
                        }
                    }

                }
            }

            #endregion

            #region 清理数据

            /// <summary>
            /// 清理数据信息
            /// </summary>
            public override void Clear()
            {
                Sprite = null;
                Name = null;
                SetNative = false;

                callback = null;
                TypeId = 0;
                Id = 0;
            }

            /// <summary>
            /// 回收脚本对象数据
            /// </summary>
            public override void RecoveryScriptPool()
            {
                ScriptPool<ResIconWaitCallBack>.RecoveryObject(this);
            }

            #endregion
        }

        #endregion


        #region 加载缓存中自定义图片信息

        private Dictionary<string, Object> _localCacheIconDic = new Dictionary<string, Object>();

        /// <summary>
        /// 加载本地存储icon
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="sprite"></param>
        /// <param name="url"></param>
        /// <param name="name"></param>
        /// <param name="typeId"></param>
        /// <param name="id"></param>
        /// <param name="callback"></param>
        /// <param name="isAutoUse"></param>
        /// <param name="setNative"></param>
        /// <param name="callbackFun"></param>
        private void LoadCacheIcon(Object obj, Object sprite, string url, string name,
            int typeId, int id, Action<int, int, Object> callback,
            bool isAutoUse = true, bool setNative = false, Action callbackFun = null)
        {
            Object icon = null;
            if(_localCacheIconDic.TryGetValue(url, out icon))
            {
                SetLocalCacheIcon(obj, sprite, icon, url, name, typeId, id, callback, isAutoUse, setNative, callbackFun);
            }
            else
            {
                var path = string.Format("{0}/{1}/{2}",
                    ConstantConfig.CACHE_PATH,
                    ConstantConfig.GROUP_SETUP,
                    url);


                if (System.IO.File.Exists(path))
                {
                    //如果存在，那么读取文件

                    byte[] data = null;
                    using (Stream s = System.IO.File.OpenRead(path))
                    {
                        //读取数据信息
                        data = new byte[s.Length];
                        s.Read(data, 0, (int)s.Length);
                        s.Dispose();
                    }

                    if (data == null || data.Length == 0)
                        return;

                    Texture2D tex = new Texture2D(2, 2);
                    tex.LoadImage(data);
                    tex.filterMode = FilterMode.Point;

                    _localCacheIconDic[url] = tex;

					SetLocalCacheIcon(obj, sprite, tex, url, name, typeId, id, callback, isAutoUse, setNative, callbackFun);
                }
            }
        }

        /// <summary>
        /// 设置本地缓存icon信息
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="sprite"></param>
        /// <param name="icon"></param>
        /// <param name="url"></param>
        /// <param name="name"></param>
        /// <param name="typeId"></param>
        /// <param name="id"></param>
        /// <param name="callback"></param>
        /// <param name="isAutoUse"></param>
        /// <param name="setNative"></param>
        /// <param name="callbackFun"></param>
        private void SetLocalCacheIcon(Object obj, Object sprite, Object icon, string url, string name,
            int typeId, int id, Action<int, int, Object> callback,
            bool isAutoUse = true, bool setNative = false, Action callbackFun = null)
        {
            if (sprite is RawImage)
            {
                var tex = icon as Texture2D;

                if (isAutoUse)
                {
                    var rawImg = sprite as RawImage;
                    rawImg.texture = tex;
                }
            }
            else if (sprite is Image)
            {
                var img = sprite as Image;
                var tex = icon as Texture2D;
                var sp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);

                img.overrideSprite = sp;
                img.sprite = sp;

                if (setNative)
                    img.SetNativeSize();
            }


            if (callback != null)
            {
                callback(typeId, id, icon);
            }
        }

        #endregion

    }


}
