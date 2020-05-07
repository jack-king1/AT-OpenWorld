using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public static class DataManager
{
    public static void LoadChunkData(Chunk chunk, string path)
    {
        ChunkData newChunk;
        string json = File.ReadAllText(path);
        newChunk = JsonUtility.FromJson<ChunkData>(json);

    }
    public static void SaveChunk(ChunkData cd, object FilePath)
    {
        Debug.Log("Saving ChunkData");
        string json = JsonUtility.ToJson(cd);
        File.WriteAllText(FilePath.ToString(), json);
    }

    public static void SaveNoiseMapData(byte[] bytes)
    {
        File.WriteAllBytes(Application.dataPath + "/StreamingAssets/NoiseMap.png", bytes);
    }

    public static void SaveNoiseMapData(byte[] bytes, string fileName)
    {
        File.WriteAllBytes(Application.dataPath + "/StreamingAssets/" + fileName + ".png", bytes);
    }

    public static bool FileExist(int x, int z)
    {
        string filePath = (Application.dataPath + "/StreamingAssets/" + x.ToString() + z.ToString() + "ChunkData" + x.ToString() + z.ToString() + ".json");
        bool exists = File.Exists(filePath);
        return exists;
    }

    public static bool ObjectFileExist(int x, int z, string name)
    {
        string filePath = (Application.dataPath + "/StreamingAssets/" + x.ToString() + z.ToString() + "ChunkData" + x.ToString() + z.ToString() + ".json");
        bool exists = File.Exists(filePath);
        return exists;
    }

    public static string CreateChunkFilepath(int x, int z)
    {
        return (Application.dataPath + "/StreamingAssets/" + x.ToString() + z.ToString() + "/ChunkData" + x.ToString() + z.ToString() + ".json");
    }

    public static string CreateWorldObjectFilepath(int x, int z, string objectName)
    {
        return (Application.dataPath + "/StreamingAssets/" + x.ToString() + z.ToString() + "/WorldObjects/" + x.ToString() + z.ToString() + ".json");
    }

    public static void CreateDirectory(int x, int z)
    {
        string filePath = (Application.dataPath + "/StreamingAssets/" + x.ToString() + z.ToString());
        Directory.CreateDirectory(filePath);
    }

    public static void UnloadWorldObject()
    {

    }
}
