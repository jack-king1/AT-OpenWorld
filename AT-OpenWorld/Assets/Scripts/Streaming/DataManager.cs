using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class DataManager
{
    static int chunkCount = 0;
    public static ChunkData LoadChunkData(int chunkID)
    {
        ChunkData newChunk;
        string json = File.ReadAllText
            (Application.dataPath + "/StreamingAssets/ChunkData" + chunkID.ToString() + ".json");
        newChunk = JsonUtility.FromJson<ChunkData>(json);
        return newChunk;
    }

    public static void SaveChunkData(ChunkData cd)
    {
        string json = JsonUtility.ToJson(cd);
        File.WriteAllText
            (Application.dataPath + "/StreamingAssets/ChunkData" + cd.chunkID.ToString() + ".json", json);
        ++chunkCount;
    }

    public static void SaveNoiseMapData(byte[] bytes)
    {
        File.WriteAllBytes(Application.dataPath + "/StreamingAssets/NoiseMap.png", bytes);
    }

    public static void SaveNoiseMapDataTest(byte[] bytes)
    {
        File.WriteAllBytes(Application.dataPath + "/StreamingAssets/NoiseMapTest.png", bytes);
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

    public static bool FileExist(int id)
    {
        return File.Exists(Application.dataPath + "/StreamingAssets/ChunkData" + id.ToString() + ".json");
    }
}
