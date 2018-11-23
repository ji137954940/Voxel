using System.Collections;
using Color.Number.GameInfo;
using Color.Number.Grid;
using System.Collections.Generic;
using Color.Number.Event;
using UnityEngine;
using UnityEngine.Tilemaps;
using ZFrame;
using Color.Number.Camera;

/// <summary>
/// tile map 信息对象
/// </summary>
[System.Serializable]
public class TilemapInfo
{
    #region Bg 

    /// <summary>
    /// pixel 对象
    /// </summary>
    public GameObject Panel;

    /// <summary>
    /// Bg Tilemap
    /// </summary>
    public Tilemap BgTilemap;
    /// <summary>
    /// Bg Tilemap 使用的 tile
    /// </summary>
    public Tile Bgtile;
    /// <summary>
    /// Bg Tilemap 生成的数据信息
    /// </summary>
    private Tile[] _bgArrTiles;

    /// <summary>
    /// _bgAlpha值
    /// </summary>
    private float _bgAlpha = 1.0f;

    /// <summary>
    /// 初始化 Bg Tilemap
    /// </summary>
    /// <param name="gridInfo"></param>
    public void InitBgTilemap(GridInfo gridInfo)
    {
        //var arr = gridInfo.GreyColorArr;
        //var count = arr.Length;
        //_bgArrTiles = new Tile[count];

        //for (int i = 0; i < count; i++)
        //{
        //    if (arr[i].a <= 0)
        //        continue;

        //    _bgArrTiles[i] = ScriptableObject.CreateInstance<Tile>();//创建Tile，注意，要使用这种方式
        //    _bgArrTiles[i].sprite = Bgtile.sprite;
        //    _bgArrTiles[i].color = arr[i];

        //    BgTilemap.SetTile(new Vector3Int(i % gridInfo.Width + _widthOffset,
        //        i / gridInfo.Width + _heightOffset, 0), _bgArrTiles[i]);

        //}


        var pixelColorArr = gridInfo.PixelColorArr;
        var count = pixelColorArr.Length;
        _bgArrTiles = new Tile[count];
        var indexId = 0;
        //从 0 开始
        for (int i = 0; i < count; i++)
        {
            var pixel = pixelColorArr[i];
            _bgArrTiles[i] = ScriptableObject.CreateInstance<Tile>();//创建Tile，注意，要使用这种方式
            _bgArrTiles[i].sprite = Bgtile.sprite;
            _bgArrTiles[i].color = pixelColorArr[i].PosGreyColor;

            var arr = pixelColorArr[i].PosColorArr;
            var num = arr.Length;
            for (int j = 0; j < num; j++)
            {
                indexId = arr[j];

                BgTilemap.SetTile(new Vector3Int(indexId % gridInfo.Width + _widthOffset,
                    indexId / gridInfo.Width + _heightOffset, 0), _bgArrTiles[i]);
            }

            //foreach (var item in pixel.PosColorDic)
            //{
            //    indexId = item.Key;

            //    BgTilemap.SetTile(new Vector3Int(indexId % gridInfo.Width + _widthOffset,
            //        indexId / gridInfo.Width + _heightOffset, 0), _bgArrTiles[i]);
            //}
        }

        indexId = 0;
    }

    /// <summary>
    /// 设置 bg Tilemap tile 色块颜色值
    /// </summary>
    /// <param name="id"></param>
    public void SetBgTilemapTileColor(int id)
    {
        if (_gridInfo == null
            || _gridInfo.IsTextureColoringComplete)
            return;

        //如果不符合规则，那么设置默认数据
        if (id <= 0)
            id = 1;

        var preId = PlayerGameInfo.instance.PreSelectColorId;
        if (preId != id)
        {
            var color = _gridInfo.PixelColorArr[preId - 1].PosGreyColor;

            color.a = _bgAlpha;
            SetBgTileColor(preId, color);
        }

        SetBgTileColor(id, ConstantConfig.GetGameConfigColor(GameConfigKey.default_tile_prompt_color));

        //刷新所有色块信息
        BgTilemap.RefreshAllTiles();

    }

    /// <summary>
    /// 设置 bg tile 色块颜色值
    /// </summary>
    /// <param name="id"></param>
    public void SetBgTileColor(int id, UnityEngine.Color color)
    {
        if (id <= 0)
            id = 1;

        //var info = _gridInfo.PixelColorArr[id];
        //if (info != null
        //    && info.PixelColorDic != null
        //    && info.PixelColorDic.Count > 0)
        //{
        //    var count = info.PixelColorDic.Count;
        //    var indexId = 0;
        //    foreach (var item in info.PixelColorDic)
        //    {
        //        indexId = item.Key;
        //        BgTilemap.SetColor(new Vector3Int(indexId % _gridInfo.Width + _widthOffset,
        //            indexId / _gridInfo.Width + _heightOffset, 0), ConstantConfig.DEFAULT_PROMPT_COLOR);
        //    }
        //    indexId = 0;
        //}

        //更新信息
        var info = _gridInfo.PixelColorArr[id - 1];
        if (info != null
            && info.PosColorDic != null
            && info.PosColorDic.Count > 0)
        {
            var tile = _bgArrTiles[id - 1];
            if (tile != null)
            {
                tile.color = color;
            }
        }
    }

    /// <summary>
    /// 设置 bg tile 颜色 alpha
    /// </summary>
    /// <param name="a"></param>
    public void SetBgTilemapTileAlpha(float a)
    {
        if (_bgAlpha.Equals(a)
            || (Mathf.Abs(_bgAlpha - a) < 0.05f &&
                !a.Equals(0f))
            || _bgArrTiles == null)
            return;

        //var count = _bgArrTiles.Length;
        //var info = _gridInfo.PixelColorArr[PlayerGameInfo.instance.SelectColorId - 1];
        //var color = UnityEngine.Color.white;
        //for (int i = 0; i < count; i++)
        //{
        //    if (_bgArrTiles[i] != null)
        //    {
        //        if (info.Contains(i))
        //        {
        //            if (a < ConstantConfig.DEFAULT_PROMPT_COLOR.a)
        //            {
        //                //BgTilemap.SetColor(new Vector3Int(i % _gridInfo.Width + _widthOffset,
        //                //    i / _gridInfo.Width + _heightOffset, 0), ConstantConfig.DEFAULT_PROMPT_COLOR);
        //                _bgArrTiles[i].color = ConstantConfig.DEFAULT_PROMPT_COLOR;
        //                continue;
        //            }
        //            else if(a.Equals(ConstantConfig.DEFAULT_PROMPT_COLOR.a))
        //            {
        //                continue;
        //            }
        //        }

        //        color = _gridInfo.PixelColorArr[i].PixelGreyColor;
        //        color.a = a;
        //        _bgArrTiles[i].color = color;

        //        //BgTilemap.SetColor(new Vector3Int(i % _gridInfo.Width + _widthOffset,
        //        //    i / _gridInfo.Width + _heightOffset, 0), color);


        //    }

        //}

        var count = _bgArrTiles.Length;
        var color = UnityEngine.Color.white;
        var defaultPromptColor = ConstantConfig.GetGameConfigColor(GameConfigKey.default_tile_prompt_color);
        for (int i = 0; i < count; i++)
        {
            if (_bgArrTiles[i] != null)
            {
                if (i + 1 == PlayerGameInfo.instance.SelectColorId)
                {
                    if (a < defaultPromptColor.a)
                    {
                        //BgTilemap.SetColor(new Vector3Int(i % _gridInfo.Width + _widthOffset,
                        //    i / _gridInfo.Width + _heightOffset, 0), ConstantConfig.DEFAULT_PROMPT_COLOR);
                        _bgArrTiles[i].color = defaultPromptColor;
                        continue;
                    }
                    else if (a.Equals(defaultPromptColor.a))
                    {
                        continue;
                    }
                    else
                    {
                        color = defaultPromptColor;
                        color.a = a;
                        _bgArrTiles[i].color = color;
                        continue;
                    }
                }

                color = _gridInfo.PixelColorArr[i].PosGreyColor;
                color.a = a;
                _bgArrTiles[i].color = color;

                //BgTilemap.SetColor(new Vector3Int(i % _gridInfo.Width + _widthOffset,
                //    i / _gridInfo.Width + _heightOffset, 0), color);


            }

        }

        _bgAlpha = a;
        BgTilemap.RefreshAllTiles();
    }

    /// <summary>
    /// 清理 bg 信息
    /// </summary>
    private void ClearBgInfo()
    {
        _bgAlpha = 1;
        _bgArrTiles = null;
        BgTilemap.ClearAllTiles();
    }

    #endregion

    #region Num

    /// <summary>
    /// Num Tilemap
    /// </summary>
    public Tilemap NumTilemap;
    /// <summary>
    /// Num Tilemap 使用的 tile
    /// </summary>
    public Tile[] NumTile;
    /// <summary>
    /// Num Tilemap 生成的数据信息
    /// </summary>
    private Tile[] _numArrTiles;

    /// <summary>
    /// 初始化 Color Tilemap
    /// </summary>
    /// <param name="gridInfo"></param>
    public void InitNumTilemap(GridInfo gridInfo)
    {
        //var arr = gridInfo.GreyColorArr;
        //var count = arr.Length;
        //_numArrTiles = new Tile[count];

        //var pixelColorArr = gridInfo.PixelColorArr;
        //count = pixelColorArr.Length;
        //var indexId = 0;
        ////从 0 开始
        //for (int i = 0; i < count; i++)
        //{
        //    var pixel = pixelColorArr[i];

        //    foreach (var item in pixel.PixelColorDic)
        //    {
        //        indexId = item.Key;
        //        _numArrTiles[indexId] = ScriptableObject.CreateInstance<Tile>();//创建Tile，注意，要使用这种方式
        //        _numArrTiles[indexId].sprite = NumTile[i + 1].sprite;

        //        NumTilemap.SetTile(new Vector3Int(indexId % gridInfo.Width + _widthOffset,
        //            indexId / gridInfo.Width + _heightOffset, 0), _numArrTiles[indexId]);
        //    }
        //}

        //indexId = 0;

        var pixelColorArr = gridInfo.PixelColorArr;
        var count = pixelColorArr.Length;
        _numArrTiles = new Tile[count];
        var indexId = 0;
        //从 0 开始
        for (int i = 0; i < count; i++)
        {
            var pixel = pixelColorArr[i];
            _numArrTiles[i] = ScriptableObject.CreateInstance<Tile>();//创建Tile，注意，要使用这种方式
            _numArrTiles[i].sprite = NumTile[i + 1].sprite;

            foreach (var item in pixel.PosColorDic)
            {
                indexId = item.Key;
                //_numArrTiles[indexId] = ScriptableObject.CreateInstance<Tile>();//创建Tile，注意，要使用这种方式
                //_numArrTiles[indexId].sprite = NumTile[i + 1].sprite;

                NumTilemap.SetTile(new Vector3Int(indexId % gridInfo.Width + _widthOffset,
                    indexId / gridInfo.Width + _heightOffset, 0), _numArrTiles[i]);
            }
        }

        indexId = 0;
    }

    /// <summary>
    /// 清理 Num 信息
    /// </summary>
    private void ClearNumInfo()
    {
        _numArrTiles = null;
        NumTilemap.ClearAllTiles();
    }

    #endregion

    #region Color

    /// <summary>
    /// Color Tilemap
    /// </summary>
    public Tilemap ColorTilemap;
    /// <summary>
    /// Color Tilemap 使用的 tile
    /// </summary>
    public Tile Colortile;
    ///// <summary>
    ///// Color Tilemap 生成的数据信息
    ///// </summary>
    //private Tile[] _colorArrTiles;
    /// <summary>
    /// Color Tilemap 生成的数据信息
    /// </summary>
    private Tile _colorTile;
    /// <summary>
    /// 上色完成的顺序
    /// </summary>
    private List<int> _colorOrderList;

    /// <summary>
    /// 是否操作的了 color tilemap
    /// </summary>
    private bool _isChangeColorTilemap = false;

    /// <summary>
    /// 初始化 Color Tilemap
    /// </summary>
    /// <param name="gridInfo"></param>
    public void InitColorTilemap(GridInfo gridInfo)
    {
        //var arr = gridInfo.GreyColorArr;
        //var count = arr.Length;
        //_colorArrTiles = new Tile[count];
        //_colorOrderList = new List<int>();

        //if (gridInfo.PixelColorCompleteOrder != null
        //    && gridInfo.PixelColorCompleteOrder.Count > 0)
        //{
        //    _colorOrderList.AddRange(gridInfo.PixelColorCompleteOrder);

        //    var pixelColorArr = gridInfo.PixelColorCompleteOrder;
        //    count = pixelColorArr.Count;
        //    var indexId = 0;
        //    for (int i = 0; i < count; i++)
        //    {
        //        indexId = pixelColorArr[i];
        //        _colorArrTiles[indexId] = ScriptableObject.CreateInstance<Tile>();//创建Tile，注意，要使用这种方式
        //        _colorArrTiles[indexId].sprite = Colortile.sprite;

        //        ColorTilemap.SetTile(new Vector3Int(indexId % gridInfo.Width + _widthOffset,
        //            indexId / gridInfo.Width + _heightOffset, 0), _colorArrTiles[indexId]);
        //    }
        //}

        _colorOrderList = new List<int>();

        if (gridInfo.PixelColorCompleteOrder != null
            && gridInfo.PixelColorCompleteOrder.Count > 0)
        {
            _colorOrderList.AddRange(gridInfo.PixelColorCompleteOrder);

            var pixelColorArr = gridInfo.PixelColorCompleteOrder;
            var count = pixelColorArr.Count;
            _colorTile = ScriptableObject.CreateInstance<Tile>();//创建Tile，注意，要使用这种方式
            _colorTile.sprite = Colortile.sprite;

            var indexId = 0;
            for (int i = 0; i < count; i++)
            {
                indexId = pixelColorArr[i];

                //获取此位置的原始颜色
                _colorTile.color = gridInfo.GetOriginalColor(indexId);

                ColorTilemap.SetTile(new Vector3Int(indexId % gridInfo.Width + _widthOffset,
                    indexId / gridInfo.Width + _heightOffset, 0), _colorTile);
            }
        }

    }

    /// <summary>
    /// 展示创建整个过程
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShowColorTilemapProcess()
    {
        var count = _colorOrderList.Count;
        var indexId = 0;
        var increment = 0;

        //开始录制循环视频
        FFmpegManager.instance.StartFillColorVideoREC(_gridInfo);

        PlayerGameInfo.instance.IsPixelColoring = true;

        for (int i = 0; i < count; i++)
        {
            indexId = _colorOrderList[i];

            //获取此位置的原始颜色
            _colorTile.color = _gridInfo.GetOriginalColor(indexId);

            ColorTilemap.SetTile(new Vector3Int(indexId % _gridInfo.Width + _widthOffset,
                indexId / _gridInfo.Width + _heightOffset, 0), _colorTile);

            increment++;

            if (increment == 5)
            {
                increment = 0;
                yield return null;
            }
        }

        PlayerGameInfo.instance.IsPixelColoring = false;

        if (BgTilemap != null)
            ClearBgInfo();

        //停止录制视频，开始生成视频数据
        FFmpegManager.instance.StopFillColorVideoREC(false);

        

        if (_gridInfo.IsAnimation)
        {
            _gridInfo.CaptureScreenshot = CameraManager.GetInst().CaptureScreenshot();

            Frame.DispatchEvent(ME.New<ME_Sprite_Frame>(p => { p.isPlay = true; }));
            ColorTilemap.ClearAllTiles();
        }
        else
        {
            Frame.DispatchEvent(ME.New<ME_Show_SaveUI>(p => { p.isActivity = true; }));
        }
    }

    /// <summary>
    /// 为 ColorTilemap 添加 tile 块
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="id"></param>
    private void AddTileForColorTilemap(Vector3Int cell, int id)
    {
        if (ColorTilemap != null)
        {
            if (_colorTile == null)
            {
                _colorTile = ScriptableObject.CreateInstance<Tile>();//创建Tile，注意，要使用这种方式
                _colorTile.sprite = Colortile.sprite;
            }

            _colorTile.color = PlayerGameInfo.instance.SelectColor;

            //_colorArrTiles[id] = tile;
            //记录顺序
            _colorOrderList.Add(id);

            ColorTilemap.SetTile(cell, _colorTile);

            _isChangeColorTilemap = true;
        }
    }

    /// <summary>
    /// 存储 color tilemap 信息
    /// </summary>
    /// <returns></returns>
    private bool SaveColorTilemapInfo()
    {
        if (_isChangeColorTilemap
            && _colorTile != null
            && _colorOrderList != null
            && _colorOrderList.Count > 0)
        {
            //确保数据已经发生了改变之后，在开始存储相应信息
            _gridInfo.UpdatePixelColorCompleteOrder(_colorOrderList);
        }

        return _isChangeColorTilemap;
    }

    /// <summary>
    /// 清理 color 信息
    /// </summary>
    private void ClearColorInfo()
    {

        _isChangeColorTilemap = false;
        _colorTile = null;
        if (_colorOrderList != null)
        {
            _colorOrderList.Clear();
            _colorOrderList = null;
        }

        ColorTilemap.ClearAllTiles();
    }

    #endregion

    #region Tilemap 操作

    /// <summary>
    /// grid info
    /// </summary>
    private GridInfo _gridInfo;

    /// <summary>
    /// 图片宽度偏移
    /// </summary>
    private int _widthOffset;
    /// <summary>
    /// 图片高度偏移
    /// </summary>
    private int _heightOffset;

    /// <summary>
    /// 初始化 Tilemap
    /// </summary>
    /// <param name="gridInfo"></param>
    public void InitTilemap(GridInfo gridInfo)
    {
        _gridInfo = gridInfo;

        _widthOffset = (ConstantConfig.GetGameConfigInt(GameConfigKey.texture_max_width) - gridInfo.Width) / 2;
        _heightOffset = (ConstantConfig.GetGameConfigInt(GameConfigKey.texture_max_height) - gridInfo.Height) / 2;

        if (!gridInfo.IsTextureColoringComplete)
        {
            //num
            InitNumTilemap(gridInfo);
        }

        //bg
        InitBgTilemap(gridInfo);
        //color
        InitColorTilemap(gridInfo);
    }

    /// <summary>
    /// 展示 tilemap 上色过程
    /// </summary>
    public void ShowTilemapColoringProcess()
    {
        //if(BgTilemap != null)
        //    ClearBgInfo();

        if(NumTilemap != null)
            ClearNumInfo();

        if(ColorTilemap != null)
            ColorTilemap.ClearAllTiles();

        //展示整个过程
        CoroutineManager.instance.StartCoroutine(ShowColorTilemapProcess());
    }

    /// <summary>
    /// 更改 tile map的 alpha值
    /// </summary>
    /// <param name="f"></param>
    public void TilemapAlpha(float f)
    {
        //var color = BgTilemap.color;
        //color.a = Mathf.Clamp01(1 - f * 2);
        //BgTilemap.color = color;

        //设置 bg tilemap alpha 值
        SetBgTilemapTileAlpha(Mathf.Clamp01(1 - f * 2));

        if (_numArrTiles != null)
        {
            var color = NumTilemap.color;
            color.a = Mathf.Clamp01(f * 2);
            NumTilemap.color = color;
        }

        //ColorTilemap 不论什么时候 alpha 一直为 1
    }

    /// <summary>
    /// 点击 Tilemap 处理
    /// </summary>
    /// <param name="worldPosition"></param>
    public void ClickTilemap(Vector3 worldPosition)
    {
        if (NumTilemap != null
            && _gridInfo != null
            && !_gridInfo.IsTextureColoringComplete)
        {
            var cell = NumTilemap.WorldToCell(worldPosition);

            ClickTilemap(cell);
        }
    }

    /// <summary>
    /// 点击 Tilemap 处理
    /// </summary>
    /// <param name="cell"></param>
    private void ClickTilemap(Vector3Int cell)
    {
        //检测此位置是否为当前选中的颜色的区域
        //因为创建 tile 的时候对位置坐了偏移，所以要进行逆向计算，获取 tile 对应的真正的 indexId
        var id = cell.x - _widthOffset + (cell.y - _heightOffset) * _gridInfo.Width;

        if (_gridInfo.IsInColorArea(PlayerGameInfo.instance.SelectColorId, id))
        {
            //移除Num中的tile
            if (RemoveTileFromTilemap(NumTilemap, cell))
            {
                //Color中添加tile
                AddTileForColorTilemap(cell, id);

                //移除已经填充的颜色数据信息
                _gridInfo.RemoveColorInfo(PlayerGameInfo.instance.SelectColorId, id);
            }
        }
    }

    /// <summary>
    /// 从 tilemap 中移除 cell 位置的 tile
    /// </summary>
    /// <param name="tilemap"></param>
    /// <param name="cell"></param>
    private bool RemoveTileFromTilemap(Tilemap tilemap, Vector3Int cell)
    {
        if (tilemap != null
            && tilemap.HasTile(cell))
        {
            tilemap.SetTile(cell, null);
            return true;
        }

        return false;
    }

    /// <summary>
    /// 存储 tilemap 信息
    /// </summary>
    /// <returns></returns>
    public bool SaveTilemapInfo()
    {
        return SaveColorTilemapInfo();
    }

    #endregion

    #region Tilemap 数据清理

    /// <summary>
    /// 数据信息清理
    /// </summary>
    public void Clear()
    {
        if (_gridInfo != null)
            _gridInfo = null;

        _widthOffset = 0;
        _heightOffset = 0;

        ClearBgInfo();
        ClearNumInfo();
        ClearColorInfo();
    }

    #endregion
}
