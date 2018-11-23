using Color.Number.Model;
using UnityEngine;

namespace Color.Number.Utils
{

    /// <summary>
    /// 扩展类
    /// </summary>
    public static class Expand
    {

        #region Bounds

        /// <summary>
        /// 返回一个 bounds a 是否完全包含另外一个 bounds b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool ContainsBounds(this Bounds a, Bounds b)
        {
            return a.min.x <= b.min.x
                   && a.min.y <= b.min.y
                   && a.min.z <= b.min.z
                   && b.max.x <= a.max.x
                   && b.max.y <= a.max.y
                   && b.max.z <= a.max.z;
        }

        /// <summary>
        /// 返回一个 bounds b 是否包含一个三角形
        /// </summary>
        /// <param name="b"></param>
        /// <param name="triangle"></param>
        /// <returns></returns>
        public static bool ContainsTriangle(this Bounds b, ModelTriangle triangle)
        {
            return b.Contains(triangle.P1) && b.Contains(triangle.P2) && b.Contains(triangle.P3);
        }

        /// <summary>
        /// 返回一个 bounds b 是否和一个三角形 triangle 相交
        /// 1 bounds 中的每一个顶点 是否在 triangle 所构成的平面的两侧，如果在同侧那么不相交
        /// 2 如果相交，在检测顶点在立方体于 taiangle 构成的平面 的交线 是否与三角形相交，如果相交那么就相交
        /// </summary>
        /// <param name="b"></param>
        /// <param name="triangle"></param>
        /// <returns></returns>
        public static bool Intersect(this Bounds b, ModelTriangle triangle)
        {
            return false;
        }

        #endregion

        #region Vector3

        /// <summary>
        /// 获取Vector3的绝对值数据
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector3 GetAbsVector3(this Vector3 v)
        {
            v.x = v.x < 0 ? -v.x : v.x;
            v.y = v.y < 0 ? -v.y : v.y;
            v.z = v.z < 0 ? -v.z : v.z;

            return v;
        }

        #endregion


        #region Texture

        /// <summary>
        /// 转换成 Texture2D 图片
        /// </summary>
        /// <param name="web"></param>
        /// <returns></returns>
        public static Texture2D GetTexture2D(this WebCamTexture web)
        {
            var tex = new Texture2D(web.width, web.height, TextureFormat.RGB24, false);
            tex.SetPixels(web.GetPixels());
            tex.Apply();
            return tex;
        }

        /// <summary>
        /// 转换成 Texture2D 图片
        /// </summary>
        /// <param name="web"></param>
        /// <returns></returns>
        public static Texture2D GetRotationTexture2D(this WebCamTexture web, bool isQuare)
        {
            var rot = Quaternion.Euler(0, 0, -web.videoRotationAngle);

            UnityEngine.Color[] arr = null;
            UnityEngine.Color[] rotArr = null;

            Texture2D tex = null;

            var width = 0;
            var height = 0;

            if (isQuare)
            {
                var size = web.width;
                size = web.height > size ? size : web.height;
                var offset = new Vector2((web.width - size) / 2, (web.height - size) / 2);

                arr = web.GetPixels((int)offset.x, (int)offset.y, size, size);

                width = height = size;
            }
            else
            {
                arr = web.GetPixels();

                width = web.width;
                height = web.height;
            }

            rotArr = new UnityEngine.Color[arr.Length];

            UnityEngine.Color c;
            var v = Vector3.zero;
            var wh = new Vector3(width, height);
            wh = rot * wh;

            var offsetX = wh.x < 0 ? (int)(-wh.x - 0.5f) : 0;
            var offsetY = wh.y < 0 ? (int)(-wh.y - 0.5f) : 0;

            var targetW = (int)(Mathf.Abs(wh.x) + 0.5f);
            var targetH = (int)(Mathf.Abs(wh.y) + 0.5f);

            tex = new Texture2D(targetW, targetH, TextureFormat.RGB24, false);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    c = arr[i * width + j];
                    v.Set(j, i, 0);
                    v = rot * v;
                    v.x = v.x < 0 ? v.x - 0.5f : v.x + 0.5f;
                    v.y = v.y < 0 ? v.y - 0.5f : v.y + 0.5f;

                    rotArr[((int)v.x + offsetX) + (((int)(v.y) + offsetY) * targetW)] = c;

                }
            }

            tex.SetPixels(rotArr);
            tex.Apply();
            return tex;
        }


        /// <summary>
        /// 转换成 Texture2D 图片
        /// </summary>
        /// <param name="web"></param>
        /// <returns></returns>
        public static Texture2D GetRotationTexture2D(this Texture2D web, int angle, bool isQuare)
        {
            var rot = Quaternion.Euler(0, 0, -angle);

            UnityEngine.Color[] arr = null;
            UnityEngine.Color[] rotArr = null;

            Texture2D tex = null;

            var width = 0;
            var height = 0;

            if (isQuare)
            {
                var size = web.width;
                size = web.height > size ? size : web.height;
                var offset = new Vector2((web.width - size) / 2, (web.height - size) / 2);

                arr = web.GetPixels((int)offset.x, (int)offset.y, size, size);

                width = height = size;
            }
            else
            {
                arr = web.GetPixels();

                width = web.width;
                height = web.height;
            }

            rotArr = new UnityEngine.Color[arr.Length];

            UnityEngine.Color c;
            var v = Vector3.zero;
            var wh = new Vector3(width, height);
            wh = rot * wh;

            var offsetX = wh.x < 0 ? (int)(-wh.x - 0.5f) : 0;
            var offsetY = wh.y < 0 ? (int)(-wh.y - 0.5f) : 0;

            var targetW = (int)(Mathf.Abs(wh.x) + 0.5f);
            var targetH = (int)(Mathf.Abs(wh.y) + 0.5f);

            tex = new Texture2D(targetW, targetH, TextureFormat.RGB24, false);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    c = arr[i * width + j];
                    v.Set(j, i, 0);
                    v = rot * v;
                    v.x = v.x < 0 ? v.x - 0.5f : v.x + 0.5f;
                    v.y = v.y < 0 ? v.y - 0.5f : v.y + 0.5f;

                    rotArr[((int)v.x + offsetX) + (((int)(v.y) + offsetY) * targetW)] = c;

                }
            }

            tex.SetPixels(rotArr);
            tex.Apply();
            return tex;
        }


        #endregion
    }
}
