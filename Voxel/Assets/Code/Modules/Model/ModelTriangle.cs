using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Color.Number.Model
{
    /// <summary>
    /// 模型三角形信息
    /// </summary>
    public class ModelTriangle
    {
        /// <summary>
        /// 三角形id
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// 三角形顶点位置坐标
        /// </summary>
        public Vector3 P1 { get; private set; }
        public Vector3 P2 { get; private set; }
        public Vector3 P3 { get; private set; }

        /// <summary>
        /// 三角形包围盒大小信息
        /// </summary>
        private Bounds _bounds;

        /// <summary>
        /// 是否已经初始化完成
        /// </summary>
        private bool _isInitDone;

        #region 数据信息初始化

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        public ModelTriangle(int id, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            Id = id;

            P1 = p1;
            P2 = p2;
            P3 = p3;

            //初始化 Bounds 信息
            InitBounds();

            _isInitDone = true;
        }

        /// <summary>
        /// 初始化 Bounds 信息
        /// </summary>
        private void InitBounds()
        {
            _bounds = new Bounds();
            _bounds.Expand(P1);
            _bounds.Expand(P2);
            _bounds.Expand(P3);
        }

        #endregion

        #region 获取数据信息

        /// <summary>
        /// 获取三角形中心点位置
        /// </summary>
        /// <returns>三角形中心点位置</returns>
        public Vector3 GetCenter()
        {
            return (P1 + P2 + P3) / 3;
        }

        /// <summary>
        /// 获取三角形的 Bounds 信息
        /// </summary>
        /// <returns></returns>
        public Bounds GetBounds()
        {
            return _bounds;
        }

        /// <summary>
        /// 获取三角形所在平面的法向量
        /// </summary>
        /// <returns></returns>
        public Vector3 GetTriangleNormalVector3()
        {
            Vector3 v1 = (P1 - P2).normalized;
            Vector3 v2 = (P2 - P3).normalized;

            return Vector3.Cross(v1, v2);
        }

        #endregion
    }
}


