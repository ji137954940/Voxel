using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class LoadingWindowData1
{

    [System.Serializable]
    public class SliderNumber
    {
        public GameObject obj;                      //数字对象
        public Text num;                            //数字显示控件
    }

    public Text text;                               //进度条要显示内容
    public SliderNumber sliderNumber;               //进度显示数字
    public Image sliderBG;
    public Image sliderFG;
    public RawImage loadingBG;
    public RectTransform rectTrans;

    public void SetSliderValue(float percent)
    {
        if (sliderBG == null || sliderFG == null)
        {
            Debug.LogError("loading 界面引用不完整，请检查...");
            return;
        }
        float maxvalue = sliderBG.rectTransform.sizeDelta.x;
        float currentValue = percent * (maxvalue - 20);
        var cusize = sliderFG.rectTransform.sizeDelta;
        sliderFG.rectTransform.sizeDelta = new Vector2(currentValue, cusize.y);
    }
}
