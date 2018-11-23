using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxel2Unity;

namespace Voxel2Unity
{

    public class VoxObj
    {


        public VoxData _voxData;

        private Voxel[,,] Voxels;

        private Vector3 Pivot = new Vector3(0.5f, 0f, 0.5f);
        private int SizeX = 0, SizeY = 0, SizeZ = 0;
        private float Scale = 0.01f;


        public Shader TheShader
        {
            get
            {
                return Shader.Find("Unlit/PureColor");
            }
        }


        public void CreateObj(VoxData voxData, int frame)
        {
            
            InitVoxels(voxData, frame);

            FixVisible();

            CreateVoxelObj();

        }

        /// <summary>
        /// 初始化 voxel 信息数据
        /// </summary>
        /// <param name="voxData"></param>
        /// <param name="frame"></param>
        private void InitVoxels(VoxData voxData, int frame)
        {
            this._voxData = voxData;

            SizeX = voxData.SizeX[frame];
            SizeY = voxData.SizeY[frame];
            SizeZ = voxData.SizeZ[frame];

            Voxels = new Voxel[SizeX, SizeY, SizeZ];
            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    for (int k = 0; k < SizeZ; k++)
                    {
                        Voxels[i, j, k].Init();
                        Voxels[i, j, k].ColorIndex = voxData.Voxels[frame][i, j, k];
                    }
                }
            }
        }

        /// <summary>
        /// 设置是否显示数据
        /// </summary>
        private void FixVisible()
        {
            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    for (int k = 0; k < SizeZ; k++)
                    {
                        if (Voxels[i, j, k].IsEmpty)
                        {
                            Voxels[i, j, k].IsVisible = true;
                            continue;
                        }
                        Voxels[i, j, k].VisibleLeft = i > 0 ? Voxels[i - 1, j, k].IsEmpty : true;
                        Voxels[i, j, k].VisibleRight = i < SizeX - 1 ? Voxels[i + 1, j, k].IsEmpty : true;
                        Voxels[i, j, k].VisibleFront = j > 0 ? Voxels[i, j - 1, k].IsEmpty : true;
                        Voxels[i, j, k].VisibleBack = j < SizeY - 1 ? Voxels[i, j + 1, k].IsEmpty : true;
                        Voxels[i, j, k].VisibleDown = k > 0 ? Voxels[i, j, k - 1].IsEmpty : true;
                        Voxels[i, j, k].VisibleUp = k < SizeZ - 1 ? Voxels[i, j, k + 1].IsEmpty : true;
                    }
                }
            }

        }

        /// <summary>
        /// 通过 voxel 数据创建出相应的 obj 对象
        /// </summary>
        private void CreateVoxelObj()
        {
            var pos = Vector3.zero;

            GameObject root = new GameObject();
            Transform tran = root.transform;

            Material mat = new Material(TheShader);


            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    for (int k = 0; k < SizeZ; k++)
                    {
                        if (Voxels[i, j, k].IsEmpty || !Voxels[i, j, k].IsVisible)
                        {
                            //此位置没有数据，或者此位置周围的6个方向都有数据，那么就不需要显示此数据信息
                            continue;
                        }

                        pos.x = Pivot.x + i;
                        pos.z = Pivot.y + j;
                        pos.y = Pivot.z + k;

                        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        Transform t = go.transform;
                        t.SetParent(tran);
                        t.SetPositionAndRotation(pos, Quaternion.identity);
                        t.name = string.Format("{0}_{1}_{2}", i, j, k);

                        Renderer renderer = go.GetComponent<Renderer>();
                        Material m = new Material(mat);
                        m.color = _voxData.Palatte[Voxels[i, j, k].ColorIndex - 1];
                        renderer.material = m;
                    }
                }
            }
        }
    }
}


