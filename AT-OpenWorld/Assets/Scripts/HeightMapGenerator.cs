using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class HeightMapGenerator : MonoBehaviour
{
    private static float chunkSize;
    public int xSize, zSize, octaves;
    public float scale, persistance, lucanarity, meshHeightMultiplier;
    public static Texture2D HeightMap;
    [SerializeField] private Texture2D HeightMapPreview;

    void Start()
    {
        chunkSize = ChunkManager.instance.chunkSize;

        byte[] heightMapData;
        if(File.Exists(Application.dataPath + "/StreamingAssets/NoiseMap.png"))
        {
            SetTexture();
        }
        else
        {
            Noise.GenerateNoiseTest(chunkSize * Mathf.Sqrt((float)ChunkManager.instance.mapChunkTotal), 
                chunkSize * Mathf.Sqrt((float)ChunkManager.instance.mapChunkTotal),
            scale, octaves, persistance, lucanarity);
            SetTexture();
        }

        void SetTexture()
        {
            HeightMap = new Texture2D((int)chunkSize, (int)chunkSize, TextureFormat.ARGB32, true);
            heightMapData = File.ReadAllBytes(Application.dataPath + "/StreamingAssets/NoiseMap.png");
            HeightMap.LoadImage(heightMapData);
            HeightMapPreview = HeightMap;
        }
    }

    public static Texture2D GetHeightMap()
    {
        return HeightMap;
    }

    public static float[] GetChunkDimensions()
    {
        float[] dimensions = { chunkSize, chunkSize };
        return dimensions;
    }
}
