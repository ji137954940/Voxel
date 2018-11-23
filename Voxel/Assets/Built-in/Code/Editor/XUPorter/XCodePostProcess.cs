using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.XCodeEditor;
#endif
using System.IO;

public static class XCodePostProcess
{

#if UNITY_EDITOR
    [PostProcessBuild(999)]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {
#if UNITY_5
        if (target != BuildTarget.iOS)
        {
#else
        if (target != BuildTarget.iOS) {
#endif
            Debug.LogWarning("Target is not iPhone. XCodePostProcess will not run");
            return;
        }

        // Create a new project object from build target
        XCProject project = new XCProject(pathToBuiltProject);

        // Find and run through all projmods files to patch the project.
        // Please pay attention that ALL projmods files in your project folder will be excuted!
        string[] files = Directory.GetFiles(Application.dataPath, "*.projmods", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            //Debug.Log("ProjMod File: " + file);
            //project.ApplyMod(file);

            ApplyMod(project, file);
        }

        ////TODO disable the bitcode for iOS 9
        //project.overwriteBuildSetting("ENABLE_BITCODE", "NO", "Release");
        //project.overwriteBuildSetting("ENABLE_BITCODE", "NO", "Debug");

        //关闭bitcode
        project.overwriteBuildSetting("ENABLE_BITCODE", "NO");

        //TODO implement generic settings as a module option
        //project.overwriteBuildSetting("CODE_SIGN_IDENTITY[sdk=iphoneos*]", "iPhone Distribution", "Release");
        //project.overwriteBuildSetting("CODE_SIGN_IDENTITY", "iPhone Developer: wang ke (LYDZ2B92D4)", "Release");
        //project.overwriteBuildSetting("CODE_SIGN_IDENTITY", "iPhone Developer: wang ke (LYDZ2B92D4)", "Debug");
        //project.overwriteBuildSetting("CODE_SIGN_IDENTITY", "iPhone Developer: client Tgame (8NK3ABPV64)", "Release");
        //project.overwriteBuildSetting("CODE_SIGN_IDENTITY", "iPhone Developer: client Tgame (8NK3ABPV64)", "Debug");

//#if AUDIT
//        #region krly
//        project.overwriteBuildSetting("CODE_SIGN_IDENTITY", "iPhone Distribution: BeiJing TianShenHuDong Science and Technology Co., Ltd. (9ZK4D6KRR3)", "Release");
//        project.overwriteBuildSetting("CODE_SIGN_IDENTITY", "iPhone Developer: wang ke (LYDZ2B92D4)", "Debug");

//        project.overwriteBuildSetting("PROVISIONING_PROFILE", "tfkrly-dev");
//        project.overwriteBuildSetting("PROVISIONING_PROFILE_SPECIFIER", "tfkrly-dis");
//        #endregion
//#else
        
//        #region tgame
//        project.overwriteBuildSetting("CODE_SIGN_IDENTITY", "iPhone Distribution: BeiJing TianShenHuDong Science and Technology Co., Ltd.", "Release");
//        project.overwriteBuildSetting("CODE_SIGN_IDENTITY", "iPhone Developer: Hengli Liu (98M285BSQB)", "Debug");

//        project.overwriteBuildSetting("PROVISIONING_PROFILE", "tgame-dev");
//        project.overwriteBuildSetting("PROVISIONING_PROFILE_SPECIFIER", "Tgame-dis");
//        #endregion

//#endif

        #region 设置包名

        
        if (PlayerSettings.applicationIdentifier == GameBundleName.TEST_FLIGTH_IDENTIFIER)
        {
            //Test Flight
            project.overwriteBuildSetting("CODE_SIGN_IDENTITY", "iPhone Distribution: BeiJing TianShenHuDong Science and Technology Co., Ltd. (9ZK4D6KRR3)", "Release");
            project.overwriteBuildSetting("CODE_SIGN_IDENTITY", "iPhone Developer: wang ke (LYDZ2B92D4)", "Debug");

            project.overwriteBuildSetting("PROVISIONING_PROFILE", "tfkrly-dev");
            project.overwriteBuildSetting("PROVISIONING_PROFILE_SPECIFIER", "tfkrly-dis");
        }
        else
        {
            //公司内部测试
            project.overwriteBuildSetting("CODE_SIGN_IDENTITY", "iPhone Distribution: BeiJing TianShenHuDong Science and Technology Co., Ltd.", "Release");
            project.overwriteBuildSetting("CODE_SIGN_IDENTITY", "iPhone Developer: Hengli Liu (98M285BSQB)", "Debug");

            project.overwriteBuildSetting("PROVISIONING_PROFILE", "tgame-dev");
            project.overwriteBuildSetting("PROVISIONING_PROFILE_SPECIFIER", "Tgame-dis");
        }
        

        #endregion



        project.overwriteBuildSetting("GCC_ENABLE_OBJC_EXCEPTIONS", "YES", "Release");
        project.overwriteBuildSetting("GCC_ENABLE_OBJC_EXCEPTIONS", "YES", "Debug");

        project.overwriteBuildSetting("GCC_ENABLE_CPP_EXCEPTIONS", "YES", "Release");
        project.overwriteBuildSetting("GCC_ENABLE_CPP_EXCEPTIONS", "YES", "Debug");

        project.overwriteBuildSetting("GCC_ENABLE_CPP_RTTI", "YES", "Release");
        project.overwriteBuildSetting("GCC_ENABLE_CPP_RTTI", "YES", "Debug");


        //#if AUDIT
        //        project.overwriteBuildSetting("PRODUCT_BUNDLE_IDENTIFIER", "com.zeus.awesome.krly");
        //        project.overwriteBuildSetting("PRODUCT_NAME","krly");
        //#elif NORMAL
        //        project.overwriteBuildSetting("PRODUCT_BUNDLE_IDENTIFIER","com.tianshen.shuguang.tgame");
        //        project.overwriteBuildSetting("PRODUCT_NAME", "krly");
        //#endif

        //TODO implement generic settings as a module option
        //		project.overwriteBuildSetting("CODE_SIGN_IDENTITY[sdk=iphoneos*]", "iPhone Distribution", "Release");

        //设置XCode app name
        //project.overwriteBuildSetting("PRODUCT_NAME", "krly");

        var pbxproj = project.project;

        var attrs = pbxproj.attributes;
        var targetAttrs = (PBXDictionary)attrs["TargetAttributes"];
        PBXDictionary targetSetting = new PBXDictionary();
        targetSetting["ProvisioningStyle"] = "Manual";


        var targets = pbxproj.targets;
        foreach (var t in targets)
        {
            var targetID = (string)t;
            if (targetAttrs.ContainsKey(targetID))
            {
                var TargetAttr = (PBXDictionary)targetAttrs[targetID];
                TargetAttr.Append(targetSetting);
            }
            else
            {
                targetAttrs[targetID] = targetSetting;
            }

        }


        // Finally save the xcode project
        project.Save();

    }
#endif

    #region 获取项目，修改mod

    /// <summary>
    /// 修改mod
    /// </summary>
    /// <param name="project"></param>
    /// <param name="pbxmod"></param>
    static void ApplyMod(XCProject project, string pbxmod)
    {
        if (string.IsNullOrEmpty(pbxmod))
        {
            Debug.LogError(" pbxmod is null ");
            return;
        }

        XCMod mod = new XCMod(pbxmod);

        //添加 plist
        //mod.AddPlistInfo("NSMicrophoneUsageDescription", "凯瑞利亚需要您的同意,才能访问麦克风");
        mod.AddPlistInfo("NSCameraUsageDescription", "App需要您的同意,才能访问相机");
        mod.AddPlistInfo("NSPhotoLibraryUsageDescription", "App需要您的同意,才能访问相册");

        project.ApplyMod(mod);
    }

    #endregion

    #region Plist 数据添加

    /// <summary>
    /// 添加 plist 信息 
    /// </summary>
    /// <param name="mod"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    static void AddPlistInfo(this XCMod mod, string key, object value)
    {
        if (mod == null)
        {
            Debug.LogError("mod is null");
            return;
        }

        if (mod.plist == null)
            mod.plist = new Hashtable();

        mod.plist.Add(key, value);
    }

    #endregion

    public static void Log(string message)
    {
        Debug.Log("PostProcess: " + message);
    }
}
