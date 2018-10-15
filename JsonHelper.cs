using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public static class JsonHelper
{

    public static object FromJson<T>(string json)
    {
        if (json.StartsWith("["))
        {
            json = "{\"Items\":" + json + "}";
            var obj = JsonUtility.FromJson<Wrapper_From<T>>(json);
            return (T[])(object)obj.Items;
        }
        else
        {
            T obj = JsonUtility.FromJson<T>(json);
            return obj;
        }
    }

    public static string ToJson<T>(T obj)
    {
        if (obj is IList)
        {
            Wrapper_To<T> wrapper = new Wrapper_To<T>();
            wrapper.Items = obj;
            var json = JsonUtility.ToJson(wrapper);
            json = Regex.Replace(json, "^\\{\"Items\":", "");
            json = Regex.Replace(json, "\\}$", "");
            return json;
        }
        else
        {
            var json = JsonUtility.ToJson(obj);
            return json;
        }
    }

    [System.Serializable]
    public class Wrapper_From<T>
    {
        public T[] Items;
    }

    [System.Serializable]
    public class Wrapper_To<T>
    {
        public T Items;
    }

}