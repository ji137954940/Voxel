using System.Collections;
using System.Collections.Generic;
using Color.Number.Utils;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{

    public RawImage img;

    public RawImage img1;

    public Image test;

    public Texture2D t;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnGUI()
    {
        if (GUILayout.Button(" 122333 "))
        {
            GetTexture();
        }

        if(GUILayout.Button("  TTT "))
        {
            GetT();
        }

        if (GUILayout.Button("  Test "))
        {
            GetTest();
        }
    }


    private void GetTexture()
    {
        //var time = Time.time;
        //img1.texture = GameUtils.GetMosaicTexture2D2(img.mainTexture, 0.02f, 50, 25);

        //Debug.LogError((Time.time - time));
        //Texture2D t = (Texture2D)img.mainTexture;
        //img1.texture = GameUtils.ScaleTexture(t, 20, 20);

        Texture2D t = (Texture2D)img.mainTexture;
        img1.texture = t.GetRotationTexture2D(-90, true);

        //img1.texture = GameUtils.GetMosaicTexture2D3(img.mainTexture, 0.015f, 25);


        //img1.texture = GameUtils.GetMosaicTexture2D(img.mainTexture, 0.03f);
    }

    private void GetT()
    {
        var arr = t.GetPixels();
        Dictionary<UnityEngine.Color, List<int>> dic = new Dictionary<UnityEngine.Color, List<int>>();
        for (int i = 0; i < arr.Length; i++)
        {
            if (dic.ContainsKey(arr[i]))
            {
                dic[arr[i]].Add(i);
            }
            else
            {
                List<int> l = new List<int>();
                l.Add(i);
                dic[arr[i]] = l;
            }

            //if (i <= 600)
            //    Debug.Log(" Color " + i + "  " + arr[i]);

            //if (arr[i] == UnityEngine.Color.white)
            //    arr[i].a = 0;
        }

        return;

        string path = Application.streamingAssetsPath + "/Test.png";

        Texture2D tt = new Texture2D(t.width, t.height, TextureFormat.RGBA32, false);
        tt.SetPixels(arr);


        var b = tt.EncodeToPNG();

        System.IO.File.WriteAllBytes(path, b);

        Debug.Log(dic.Count);
    }

    private void GetTest()
    {
        var t = test.mainTexture as Texture2D;

        var arr = t.GetPixels();
        Dictionary<UnityEngine.Color, List<int>> dic = new Dictionary<UnityEngine.Color, List<int>>();
        for (int i = 0; i < arr.Length; i++)
        {
            //if (dic.ContainsKey(arr[i]))
            //{
            //    dic[arr[i]].Add(i);
            //}
            //else
            //{
            //    List<int> l = new List<int>();
            //    l.Add(i);
            //    dic[arr[i]] = l;
            //}

                Debug.Log(" Color " + i + "  " + arr[i]);
        }

        return;

        string path = Application.streamingAssetsPath + "/Test.png";

        Texture2D tt = new Texture2D(t.width, t.height, TextureFormat.RGBA32, false);
        tt.SetPixels(arr);


        var b = tt.EncodeToPNG();

        System.IO.File.WriteAllBytes(path, b);

        Debug.Log(dic.Count);
    }
}
