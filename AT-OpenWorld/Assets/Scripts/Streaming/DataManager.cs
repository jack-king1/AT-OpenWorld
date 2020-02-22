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

    public static void SaveAnimationCurve(AnimationCurve ac)
    {
        string json = JsonUtility.ToJson(ac);
        File.WriteAllText(Application.dataPath + "/StreamingAssets/AnimationCurve.json", json);
    }

    public static Material GetVertexShader()
    {
        //Shader shdr = Resources.Load("/Resources/VertexColors.shader", typeof (Shader)) as Shader;
        Material ac = Resources.Load<Material>(Application.dataPath + "Resources/Materials/VertexMat.mat");
        return ac;
    }

    public static bool FileExist(int x, int z)
    {
        string filePath = (Application.dataPath + "/StreamingAssets/ChunkData" + x.ToString() + z.ToString() + ".json");
        bool exists = File.Exists(filePath);
        return exists;
    }

    public static string CreateFilepath(int x, int z)
    {
        return (Application.dataPath + "/StreamingAssets/ChunkData" + x.ToString() + z.ToString() + ".json");
    }
}
