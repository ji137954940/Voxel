using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class NetErrorData
{
    //网络断开对象
    public GameObject net_error_obj;
    //网络断开显示内容
    public Text net_error_text;
    //网络断开退出按钮
    public Button net_error_exit_btn;

    //网络异常（慢，断开）对象
    public GameObject net_slow_obj;
    //网络异常（慢，断开）显示内容
    public Text net_slow_text;
    //网络异常（慢，断开）退出按钮
    public Button net_slow_exit_btn;

}
