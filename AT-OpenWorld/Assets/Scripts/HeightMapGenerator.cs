using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class HeightMapGenerator : MonoBehaviour
{
    private static float ChunkWidth = 128f;
    private static float ChunkHeight = 128f;
    public int xSize, zSize, octaves;
    public float scale, persistance, lucanarity, meshHeightMultiplier;
    public static Texture2D HeightMap;
    [SerializeField] private Texture2D HeightMapPreview;

    [SerializeField]
    public enum ChunkSize
    {
        One = 1,
        Four = 4,
        Nine = 9,
        Sixteen = 16,
        TwentyFive = 25
    }
    public ChunkSize MapChunkSize;

    void Start()
    {
        byte[] heightMapData;
        if(File.Exists(Application.dataPath + "/StreamingAssets/NoiseMapTest.png"))
        {
            SetTexture();
        }
        else
        {
            Noise.GenerateNoiseTest(ChunkWidth * (float)ChunkGenerator.mapChunkTotal, ChunkHeight * (float)ChunkGenerator.mapChunkTotal,
            scale, octaves, persistance, lucanarity);
            SetTexture();
        }
        void SetTexture()
        {
            HeightMap = new Texture2D((int)ChunkWidth, (int)ChunkHeight, TextureFormat.ARGB32, true);
            heightMapData = File.ReadAllBytes(Application.dataPath + "/StreamingAssets/NoiseMapTest.png");
            HeightMap.LoadImage(heightMapData);
            HeightMapPreview = HeightMap;
        }

        //once the noise map is generated we can generqate the chunks if they dont exist already.
        ChunkGenerator.GenerateChunks(ChunkWidth, ChunkHeight);
    }

    public static Texture2D GetHeightMap()
    {
        return HeightMap;
    }

    public static float[] GetChunkDimensions()
    {
        float[] dimensions = { ChunkWidth, ChunkHeight };
        return dimensions;
    }
}
