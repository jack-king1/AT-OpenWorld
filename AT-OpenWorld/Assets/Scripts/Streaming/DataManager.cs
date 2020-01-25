using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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
}
