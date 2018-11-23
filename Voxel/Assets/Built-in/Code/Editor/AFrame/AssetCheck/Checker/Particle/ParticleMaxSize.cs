using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AFrame.AssetCheck
{
    /// <summary>
    /// 粒子最大数量检查(超出10)
    /// @author LiuLeiLei
    /// @data 3/2/2018
    /// @desc 
    /// </summary>
    [AssetCheck(checkType = CheckEnumType.Particle, option = CheckOption.Particle_MaxSize,desc = "粒子最大数量检查(超出10)")]
    public class ParticleMaxSize : BaseCheck
	{
        private ParticleSystem[] particleArr;

        private int particleArrLen;

        public override bool OnCheck(Object _obj, string _path, AssetImporter _importer)
        {
            GameObject gameObj = _obj as GameObject;
            if (gameObj == null)
                return true;

            particleArr = gameObj.GetComponentsInChildren<ParticleSystem>();
            particleArrLen = particleArr.Length;

            for (int i = 0; i < particleArrLen; i++)
            {
                var particle = particleArr[i];
                if (particle.main.maxParticles > 10)
                    return false;
            }

            return base.OnCheck(_obj, _path, _importer);
        }

        public override bool OnFormat(Object _obj, string _path, AssetImporter _importer)
        {
            if (particleArrLen > 0 && particleArr != null)
            {
                for (int i = 0; i < particleArrLen; i++)
                {
                    var particle = particleArr[i];
                    if (particle.main.maxParticles > 10)
                        particle.Emit(10);
                }
            }

            return base.OnFormat(_obj, _path, _importer);
        }

        public override string OnCheckMessage()
        {
            return "粒子最大数量超过10";
        }
    }
}