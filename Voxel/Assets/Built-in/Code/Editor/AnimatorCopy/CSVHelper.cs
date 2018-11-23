using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

/// <summary>
/// 导出CSV文件
/// @author LiuLeiLei
/// @data 5/9/2018
/// @desc 
/// </summary>
public class CSVHelper<T>
{
    public Action<StringBuilder> onPropCallBack;

    public Action<StringBuilder, T> onValueCallBack;

    public string CreateFile(string _filePath, string _fileExtension)
    {
        string filePath = _filePath + "." + _fileExtension;

        return CreateFile(filePath);
    }

    public string CreateFile(string _filePath)
    {
        FileStream fs = null;

        try
        {
            fs = File.Create(_filePath);
        }
        catch (Exception _ex)
        {
            Debug.LogException(_ex);
        }
        finally
        {
            if (fs != null)
            {
                fs.Dispose();
            }
        }

        return _filePath;
    }

    public bool SaveDataToCSVFile(List<T> _dataList, string _filePath)
    {
        bool success = true;

        StringBuilder columSb = new StringBuilder();
        StringBuilder valueSb = new StringBuilder();
        StreamWriter sw = null;
        PropertyInfo[] props = GetPropertyInfoArr();

        try
        {
            sw = new StreamWriter(_filePath);
            for (int i = 0; i < props.Length; i++)
            {
                columSb.Append(props[i].Name);
                columSb.Append(",");

                if (onPropCallBack != null)
                {
                    onPropCallBack(columSb);
                }
            }

            columSb.Remove(columSb.Length - 1, 1);
            sw.WriteLine(columSb);

            for (int i = 0; i < _dataList.Count; i++)
            {
                valueSb.Remove(0, valueSb.Length);

                if (onValueCallBack != null)
                {
                    onValueCallBack(valueSb, _dataList[i]);
                }

                sw.WriteLine(valueSb);
            }
        }
        catch (Exception _ex)
        {
            success = false;

            Debug.LogException(_ex);
        }
        finally
        {
            if (sw != null)
            {
                sw.Dispose();
            }
        }

        return success;
    }

    private PropertyInfo[] GetPropertyInfoArr()
    {
        PropertyInfo[] props = null;

        try
        {
            Type type = typeof(T);
            object obj = Activator.CreateInstance(type);
            props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }
        catch (Exception _ex)
        {
            Debug.LogException(_ex);
        }

        return props;
    }
}
