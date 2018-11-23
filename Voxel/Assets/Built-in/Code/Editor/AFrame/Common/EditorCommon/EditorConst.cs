using UnityEngine;
using UnityEditor;

namespace AFrame.EditorCommon
{
    /// <summary>
    /// 各种常量
    /// 工具版本更迭，这里需要清理一下
    /// </summary>
    public static class EditorConst
    {
        #region TableConst

        /// <summary>
        /// 顶点个数
        /// </summary>
        public const int VertexCount = 1000;
        /// <summary>
        /// 三角形数
        /// </summary>
        public const int TriangleCount = 1000;

        /// <summary>
        /// 图片尺寸数组集合
        /// </summary>
        private static int[] TextureSizeList = { 64 * 64, 128 * 128, 256 * 256, 512 * 512, 1024 * 1024, 2048 * 2048, 4096 * 4096 };
        /// <summary>
        /// 获得小于等于当前尺寸图片个数
        /// </summary>
        /// <param name="__w"></param>
        /// <param name="__h"></param>
        /// <returns></returns>
        public static int GetTextureSizeIndex(int __w,int __h)
        {
            int index = 0, size = __w * __h;

            while (index < TextureSizeList.Length && size > TextureSizeList[index])
                index++;

            return index;
        }

        /// <summary>
        /// 图片尺寸范围集合
        /// </summary>
        public static string[] TextureSizeRangeList =
        {
            "[0 - 64]", "(64 - 128]", "(128 - 256]", "(256 - 512]", "(512 - 1024]","(1024 - 2048]","(2048 - 4096]","(4096 - ...]"
        };

        public static string[] TextureShape = { "2D","Cube" };
        public static TextureImporterShape[] TexImportShape = { TextureImporterShape.Texture2D,TextureImporterShape.TextureCube };

        public static int[] MaxSizeInt = { -1,64,128,256,512,1024,2048};
        public static string[] MaxSize = { "Auto","64","128","256","512","1024","2048"};

        public static string[] AlphaMode = { "FromTexture","None" };

        /// <summary>
        /// 会用到的贴图类型
        /// </summary>
        public static string[] TextureTypeArray = { "Default", "NormalMap", "Lightmap", "Sprite" };
        public static TextureImporterType[] TexImportTypeArray = { TextureImporterType.Default, TextureImporterType.NormalMap,TextureImporterType.Lightmap,TextureImporterType.Sprite };


        //public static string[] MeshCompression = { "Off","Low","Medium","High"};

        //public static string[] AnimationType = { "None", "Legacy", "Generic", "Human" };

        //public static string[] AnimationCompression = { "Off", "KeyframeReduction", "KeyframeReductionAndCompression", "Optimal" };

        public static string Smile = "(●'◡'●)";

        #endregion

        #region EditorConst

        /// <summary>
        /// 与顶部栏位的间距
        /// </summary>
        public static float TopbarHeight = 25;
        /// <summary>
        /// 距离边界长度
        /// </summary>
        public static int TableBorder = 10;
        /// <summary>
        /// 表格视图宽度占比 0.3
        /// </summary>
        public static float SplitterRatio = 0.3f;

        public static string[] MeshDataStr = { "tangent", "normal", "color", "uv4", "uv3", "uv2", "uv" };
        //窗体名字
        public static string[] WindowTypeName = { "Texture", "Model" };

        /// <summary>
        /// 图片
        /// </summary>
        public static string[] TextureExts = { ".tga", ".png", ".jpg", ".tif", ".psd", ".exr" };
        /// <summary>
        /// 材质
        /// </summary>
        public static string[] MaterialExts = { ".mat" };
        /// <summary>
        /// 模型
        /// </summary>
        public static string[] ModelExts = { ".fbx", ".asset", ".obj" };
        /// <summary>
        /// 动画
        /// </summary>
        public static string[] AnimationExts = { ".anim" };
        /// <summary>
        /// Meta文件
        /// </summary>
        public static string[] MetaExts = { ".meta" };
        /// <summary>
        /// Shader
        /// </summary>
        public static string[] ShaderExts = { ".shader" };
        /// <summary>
        /// cs文件
        /// </summary>
        public static string[] ScriptExts = { ".cs" };
        /// <summary>
        /// Prefab
        /// </summary>
        public static string[] PrefabExts = { ".prefab" };
        /// <summary>
        /// Mask遮罩
        /// </summary>
        public static string[] MaskExts = { ".mask" };
        /// <summary>
        /// AnimatorController
        /// </summary>
        public static string[] ControllerExts = { ".controller" };
        /// <summary>
        /// txt 文件
        /// </summary>
        public static string[] TextExts = { ".txt" };

        /// <summary>
        /// Android平台
        /// </summary>
        public static string PlatformAndroid = "Android";
        /// <summary>
        /// ios平台
        /// </summary>
        public static string PlatformIos = "iPhone";
        /// <summary>
        /// PC平台
        /// </summary>
        public static string PlatformStandalones = "Standalone";

        public static string EDITOR_ANICLIP_NAME = "__preview__Take 001";

        #endregion

        #region TableStyles
        /*  界面各种风格 备注：这样定义静态的 GUIStyle 有时会导致界面发生显示错误
         *  正确的方法 ：自定义一个 Style 类，把需要的 GUIStyle 初始化 （需要在OnGUI方法里初始化）
         *  详情查询 AFrame/GUI样式查看器
         */
        public static readonly GUIStyle ToolBar = "ToolBar";

        public static readonly GUIStyle ToolBarBtn = "ToolBarButton";

        public static readonly GUIStyle TE_ToolBarBtn = "TE toolbarbutton";

        public static GUIStyle ToolbarPopup = "ToolbarPopup";

        public static GUIStyle ToolbarDropDown = "ToolbarDropDown";

        public static GUIStyle TextField = "TextField";
        public static GUIStyle LargeTextField = "LargeTextField";

        public static GUIStyle LeftBtn = "ButtonLeft";
        public static GUIStyle MidBtn = "ButtonMid";
        public static GUIStyle RightBtn = "ButtonRight";

        public static GUIStyle LargeLeftBtn = "LargeButtonLeft";
        public static GUIStyle LargeMidBtn = "LargeButtonMid";
        public static GUIStyle LargeRightBtn = "LargeButtonRight";

        public static GUIStyle LinePlus = "OL Plus";
        public static GUIStyle LineMinus = "OL Minus";

        public static GUIStyle CnBox = "CN Box";
        public static GUIStyle WindowBackground = "WindowBackground";
        public static GUIStyle SV_Iconselector_Back = "sv_iconselector_back";
        public static GUIStyle Window = "window";
        public static GUIStyle ControlHighlight = "ControlHighlight";

        public static GUIStyle FlowNode_0_On = "flow node 0 on";

        public static GUIStyle FlowNodeHex_1_On = "flow node hex 1 on";

        public static GUIStyle RL_DragHandle = "RL DragHandle";

        public static GUIStyle ProfilerTimelineFoldout = "ProfilerTimelineFoldout";

        public static GUIStyle CN_EntryWarn = "CN EntryWarn";

        public static GUIStyle CN_StatusWarn = "CN StatusWarn" ;

        public static GUIStyle CN_EntryInfo = "CN EntryInfo";

        public static GUIStyle CN_EntryError = "CN EntryError";

        public static GUIStyle CN_StatusError = "CN StatusError";

        public static GUIStyle BreadCrumbLeft = "GUIEditor.BreadcrumbLeft";

        public static GUIStyle BreadCrumbMid = "GUIEditor.BreadcrumbMid";
    
        #endregion

        #region GUIStyles

        /// <summary>
        /// 文本框风格
        /// </summary>
        private static GUIStyle _style = null;
        public static GUIStyle LabelStyle(UnityEngine.Color __textColor,int __fontSize = -1, TextAnchor __anchor = TextAnchor.MiddleLeft)
        {
            if (_style == null)
                _style = new GUIStyle(EditorStyles.whiteLabel);

            _style.alignment = __anchor;
            _style.normal.textColor = __textColor;
            if (__fontSize > 0)
                _style.fontSize = __fontSize;

            return _style;
        }
        #endregion

        #region TableViewConst

        public readonly static string BytesFormatter = "<fmt_bytes>";

        //public readonly static Color SelectionColor = new Color32(62, 95, 150, 255);
        //public readonly static Color SelectionColorDark = new Color32(62, 95, 150, 128);

        public readonly static UnityEngine.Color SelectionColor = new Color32(70, 70, 70, 200);
        public readonly static UnityEngine.Color SelectionColorDark = new Color32(50, 50, 50, 200);

        //public readonly static Color TitleColor = new Color32(38, 158, 111, 255);
        //public readonly static Color TitleColorSelected = new Color32(19, 80, 60, 255);

        public readonly static UnityEngine.Color TitleColor = new Color32(150, 150, 150, 255);

        public readonly static UnityEngine.Color TitleColorSelected = new Color32(180, 180, 180, 255);

        #region AnimatorController Color
        public readonly static UnityEngine.Color BaseLayer = new Color32(38, 122, 204, 255);

        public readonly static UnityEngine.Color StateMachine = new Color32(92, 38, 204, 255);

        public readonly static UnityEngine.Color DefaultState = new Color32(204,138,38,255);

        public readonly static UnityEngine.Color BlendTree = UnityEngine.Color.green;

        public readonly static UnityEngine.Color Animation = new Color32(195, 38, 204, 255);
        #endregion
        
        #endregion

        #region FormatConfig

        /// <summary>
        /// 根路径
        /// </summary>
        public const string ResourceRootPath = "Assets";
        /// <summary>
        /// 表示没有文件夹
        /// </summary>
        public const string NoFile = "(●'◡'●)";
        /// <summary>
        /// 配置文件路径
        /// </summary>
        public const string ConfigDataRoot = "Assets/Built-in/Code/Editor/AFrame/ConfigData";
        /// <summary>
        /// 数据文件后缀
        /// </summary>
        public const string DataSuffix = ".txt";

        public const string DataPathLoader = ConfigDataRoot + "/PathLoader" + DataSuffix;

        public const string AssetConfigDataFile = "/AssetConfigData";

        public const string TextureImportPath = ConfigDataRoot + AssetConfigDataFile + "/TextureImportData" + DataSuffix;

        public const string ModelImportPath = ConfigDataRoot + AssetConfigDataFile + "/ModelImportData" + DataSuffix;

        public const string AnimationImportPath = ConfigDataRoot + AssetConfigDataFile + "/AnimationImportData" + DataSuffix;

        #endregion

        #region AnimatorController

        public const string AnimControllerSuffix = ".controller";

        public const string AnimatorConfigDataFile = "/AnimatorConfigData";
        /// <summary>
        /// 儿子模板json路径
        /// </summary>
        public const string AnimatorControllerImportPath = ConfigDataRoot + AnimatorConfigDataFile + "/AnimatorImportData" + DataSuffix;
        /// <summary>
        /// 通用数据json路径
        /// </summary>
        public const string AnimatorControllerGeneralDataPath = ConfigDataRoot + AnimatorConfigDataFile + "/AnimatorGeneralData" + DataSuffix;
        /// <summary>
        /// 爸爸模板json路径
        /// </summary>
        public const string AnimatorFatherControllerDataPath = ConfigDataRoot + AnimatorConfigDataFile + "/AnimatorFatherData" + DataSuffix;

        public const string AnimatorDescriptionDataPath = "Assets/Built-in/Code/Editor/AFrame/Animator/TableData/Config/language" + DataSuffix;

        #endregion
    }
}

