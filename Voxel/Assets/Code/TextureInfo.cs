using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

namespace Color.Number.Texture
{
    [System.Serializable]
    public class TextureInfo
    {
        public Texture2D texture;

        public Image image;

        public UnityEngine.Color[] piexlArr;

        public Dictionary<UnityEngine.Color, List<int>> pixelColorDic;

        public void GetAllTexturePixel()
        {
            if (texture != null)
            {
                var count = texture.width * texture.height;
                piexlArr = new UnityEngine.Color[count];

                piexlArr = texture.GetPixels(0, 0, texture.width, texture.height);

                pixelColorDic = new Dictionary<UnityEngine.Color, List<int>>();

                for (int i = 0; i < count; i++)
                {
                    var color = piexlArr[i];

                    if (pixelColorDic.ContainsKey(color))
                    {
                        pixelColorDic[color].Add(i);
                    }
                    else
                    {
                        var list = new List<int>();
                        list.Add(i);
                        pixelColorDic[color] = list;
                    }
                }

                foreach (var item in pixelColorDic)
                {
                    Debug.LogError(" color " + item.Key + " num " + item.Value.Count);
                }


                var array = CreateGrayTexture(piexlArr);

                var destTex = new Texture2D(texture.width, texture.height);
                destTex.SetPixels(array);
                destTex.Apply();

                var rect = image.sprite.rect;
                var pivot = image.sprite.pivot;

                Sprite sprite = Sprite.Create(destTex, rect, pivot);

                image.sprite = sprite;
            }

            
        }


        /// <summary>
        /// 创建灰色图片
        /// </summary>
        public UnityEngine.Color[] CreateGrayTexture(UnityEngine.Color[] piexlArr)
        {
            if (piexlArr != null && piexlArr.Length > 0)
            {
                var piexlCount = piexlArr.Length;
                var array = new UnityEngine.Color[piexlCount];

                for (int i = 0; i < piexlCount; i++)
                {
                    var color = piexlArr[i];
                    var grey = Vector3.Dot(new Vector3(color.r, color.g, color.b), new Vector3(0.299f, 0.587f, 0.114f));
                    //array[i] = new UnityEngine.Color(color.r * 0.299f, color.g * 0.587f, color.b * 0.144f);
                    array[i] = new UnityEngine.Color(grey, grey, grey);

                }

                return array;
            }

            return null;
            
        }

    }
}


