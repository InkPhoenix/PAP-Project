using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class scr_save_data_manager
{
    public static void saveToJSON<T>(List<Save_Header> header, List<T> data, string filename) //Lists
    {
        string content = JSONHelper.toJSON<T>(header.ToArray(), data.ToArray(), true);
        writeFile(getPath(filename), content);
        Debug.Log($"Successfully saved to file: {getPath(filename)}");
    }

    public static void saveToJSON<T>(T to_save, string filename) //single objects
    {
        string content = JsonUtility.ToJson(to_save, true);
        writeFile(getPath(filename), content);
        Debug.Log($"Successfully saved to file: {getPath(filename)}");
    }

    public static List<T> loadListFromJSON<T>(string filename)
    {
        string content = readFile(getPath(filename));

        if (string.IsNullOrEmpty(content) || content == "{}")
        {
            return new List<T>();
        }

        List<T> res = JSONHelper.fromJSON<T>(content).ToList();
        return res;
    }

    public static T loadFromJSON<T>(string filename)
    {
        string content = readFile(getPath(filename));

        if (string.IsNullOrEmpty(content) || content == "{}")
        {
            return default(T);
        }

        T res = JsonUtility.FromJson<T>(content);
        return res;
    }

    public static List<Save_Header> loadHeaderFromJSON<T>(string filename)
    {
        string content = readFile(getPath(filename));

        if (string.IsNullOrEmpty(content) || content == "{}")
        {
            return new List<Save_Header>();
        }

        List<Save_Header> res = JSONHelper.headerFromJSON<Save_Header>(content).ToList();
        return res;
    }

    private static string getPath(string filename)
    {
        return Application.dataPath + "/.saves/" + filename;
    }

    private static void writeFile(string path, string content)
    {
        FileStream fileStream = new FileStream(path, FileMode.Create);

        using (StreamWriter writer = new StreamWriter(fileStream))
        {
            writer.Write(content);
        }
    }

    private static string readFile(string path)
    {
        if (File.Exists(path))
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string content = reader.ReadToEnd();
                return content;
            }
        }
        return "";
    }
}

public static class JSONHelper
{
    public static T[] fromJSON<T> (string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.GameData;
    }

    public static Save_Header[] headerFromJSON<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Header;
    }

    public static string toJSON<T> (Save_Header[] header, T[] data, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Header = header; //HEADER
        wrapper.GameData = data; //DATA
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable] private class Wrapper<T>
    {
        public Save_Header[] Header;
        public T[] GameData;
    }
}
