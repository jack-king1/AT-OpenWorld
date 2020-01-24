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
        string json = File.ReadAllText(Application.dataPath + "ChunkData.json");

    }

    public static void SaveChunkData(MeshTerrain cd)
    {
        string json = JsonUtility.ToJson(cd);
        File.WriteAllText(Application.dataPath + "/ChunkData.json", json);
        ++chunkCount;
    }

    public static void SaveNoiseMapData(byte[] bytes)
    {
        File.WriteAllBytes(Application.dataPath + "/NoiseMap.png", bytes);
    }
}
