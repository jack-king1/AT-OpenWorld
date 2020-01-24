using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


//Create chunk
public static class DataManager
{
    static int chunkCount = 0;

    public static void LoadChunkData(int chunkID)
    {
        MeshTerrain chunk = new MeshTerrain();
        string json = File.ReadAllText(Application.dataPath + "/StreamingAssets/ChunkData.json");

    }

    public static void SaveChunkData(MeshTerrain cd)
    {
        string json = JsonUtility.ToJson(cd);
        File.WriteAllText(Application.dataPath + "/StreamingAssets/ChunkData.json", json);
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
}
