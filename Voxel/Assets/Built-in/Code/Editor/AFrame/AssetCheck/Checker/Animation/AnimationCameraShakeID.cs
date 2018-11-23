using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AFrame.AssetCheck
{
    /// <summary>
    /// 替换震屏参数改成id
    /// @author LiuLeiLei
    /// @data 3/2/2018
    /// @desc 
    /// </summary>
    [AssetCheck(checkType = CheckEnumType.Animation, option = CheckOption.Animation_CameraShakeID,desc = "替换震屏参数改成id")]
    public class AnimationCameraShakeID : BaseCheck
    {
        //TODO 替换震屏参数改成id，定义了一个全局列表？
        public override bool OnFormat(Object _obj, string _path, AssetImporter _importer)
        {
            ModelImporter modelImporter = _importer as ModelImporter;
            if (modelImporter == null) return true;

            ModelImporterClipAnimation[] mcAnimationArr = modelImporter.clipAnimations;

            int mcAnimationArrLen = mcAnimationArr.Length;
            int animEventArrLen = 0;

            for (int i = 0; i < mcAnimationArrLen; i++)
            {
                ModelImporterClipAnimation mcAnimation = mcAnimationArr[i];

                AnimationEvent[] animEventArr = mcAnimation.events;

                animEventArrLen = animEventArr.Length;

                for (int j = 0; j < animEventArrLen; j++)
                {
                    AnimationEvent animEvent = animEventArr[j];

                    if (AssetCheckTool.IsShakeCameraName(animEvent.functionName))
                        return true;
                }
            }

            return base.OnFormat(_obj, _path, _importer);
        }
    }
}