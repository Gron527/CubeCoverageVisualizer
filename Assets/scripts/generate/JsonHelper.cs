using UnityEngine;

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        string wrapped = "{\"items\":" + json + "}";
        Wrapper<T> w = JsonUtility.FromJson<Wrapper<T>>(wrapped);
        return w.items;
    }
    // { "items": [...] }
    [System.Serializable]
    private class Wrapper<T> { public T[] items; }
}
