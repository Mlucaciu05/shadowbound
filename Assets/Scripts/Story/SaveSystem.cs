using System;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private const string FileName = "shadowbound_story_progress.json";

    private static string SavePath
    {
        get { return Path.Combine(Application.persistentDataPath, FileName); }
    }

    private static string BackupPath
    {
        get { return SavePath + ".bak"; }
    }

    private static string TempPath
    {
        get { return SavePath + ".tmp"; }
    }

    public static void Save(SaveData data)
    {
        if (data == null) return;

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(TempPath, json);

        try
        {
            if (File.Exists(SavePath))
            {
                File.Replace(TempPath, SavePath, BackupPath);
            }
            else
            {
                File.Move(TempPath, SavePath);
            }
        }
        catch (IOException)
        {
            File.Copy(TempPath, SavePath, true);
            File.Delete(TempPath);
        }
    }

    public static SaveData Load()
    {
        SaveData data = TryLoadFrom(SavePath);
        if (data != null) return data;

        return TryLoadFrom(BackupPath);
    }

    public static void Delete()
    {
        DeleteIfExists(SavePath);
        DeleteIfExists(BackupPath);
        DeleteIfExists(TempPath);
    }

    private static SaveData TryLoadFrom(string path)
    {
        if (!File.Exists(path)) return null;

        try
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<SaveData>(json);
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed to read save file at " + path + ": " + e.Message);
            return null;
        }
    }

    private static void DeleteIfExists(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
