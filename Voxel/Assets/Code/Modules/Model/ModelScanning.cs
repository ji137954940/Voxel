using System.Collections;
using System.Collections.Generic;
using Color.Number.Voxel;
using UnityEngine;

namespace Color.Number.Model
{
    /// <summary>
    /// 模型信息扫描
    /// </summary>
    [System.Serializable]
    public class ModelScanning 
    {
        /// <summary>
        /// 扫描模型 mesh filter
        /// </summary>
        public MeshFilter mf;

        /// <summary>
        /// 记录模型所有的三角形信息
        /// </summary>
        private ModelTriangle[] _modelTriangles;

        #region 数据扫描

        /// <summary>
        /// 进行数据扫描
        /// </summary>
        public void Scan()
        {
            var tris = GetMeshTriangles();
            var vertices = GetMeshVertices();

            var scale = mf.transform.localScale;

            if (tris != null && vertices != null)
            {
                //根据顶点数据信息，创建当前所有的三角形信息数据
                var count = tris.Length / 3;

                _modelTriangles = new ModelTriangle[count];
                for (int i = 0; i < count; i++)
                {
                    var tri = new ModelTriangle(i, vertices[tris[i * 3]] * scale.x, vertices[tris[i * 3 + 1]] * scale.y, vertices[tris[i * 3 + 2]] * scale.z);
                    _modelTriangles[i] = tri;
                }
            }

            //获取有用的 voxel 区域信息
            //VoxelManager.GetInst().VoxelAllArea(_modelTriangles);
        }

        #endregion

        #region 获取数据信息

        /// <summary>
        /// 获取Mesh信息数据
        /// </summary>
        /// <returns></returns>
        public Mesh GetMesh()
        {
            if (mf == null || mf.mesh == null)
            {
                Debug.LogError(" 扫描模型数据缺失 mesh 数据信息 ");
                return null;
            }

            return mf.mesh;
        }

        /// <summary>
        /// 获取Mesh中所有的Triangle信息
        /// </summary>
        /// <returns></returns>
        public int[] GetMeshTriangles()
        {
            Mesh mesh = GetMesh();
            if (mesh == null)
            {
                return null;
            }

            return mesh.triangles;
        }

        /// <summary>
        /// 获取Mesh中所有的Vertice信息
        /// </summary>
        /// <returns></returns>
        public Vector3[] GetMeshVertices()
        {
            Mesh mesh = GetMesh();
            if (mesh == null)
            {
                return null;
            }

            return mesh.vertices;
        }

        #endregion
    }
}


