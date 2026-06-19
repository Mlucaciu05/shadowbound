using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private const string FileName = "shadowbound_story_progress.json";

    private static string SavePath
    {
        get { return Path.Combine(Application.persistentDataPath, FileName); }
    }

    public static void Save(SaveData data)
    {
        if (data == null) return;

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    public static SaveData Load()
    {
        if (!File.Exists(SavePath)) return null;

        string json = File.ReadAllText(SavePath);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static void Delete()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
        }
    }
}
