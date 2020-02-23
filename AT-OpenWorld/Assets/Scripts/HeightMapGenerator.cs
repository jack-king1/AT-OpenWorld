using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class HeightMapGenerator : MonoBehaviour
{
    int mapSizeWidth;
    public int octaves;
    public float scale, persistance, lucanarity, meshHeightMultiplier;
    public static Texture2D HeightMap;
    public static float[,] noiseMap;
    [SerializeField] private Texture2D HeightMapPreview;
    public AnimationCurve ac;

    public static HeightMapGenerator instance;

    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }

        mapSizeWidth = (int)Mathf.Sqrt(ChunkManager.instance.mapChunkTotal) * (int)ChunkManager.instance.chunkSize;
        byte[] heightMapData;
        if(File.Exists(Application.dataPath + "/StreamingAssets/NoiseMap.png"))
        {
            SetTexture();
        }
        else
        {
            Noise.GenerateNoiseTest(mapSizeWidth,
            scale, octaves, persistance, lucanarity);
            Noise.GenerateTextureFile(noiseMap);
            SetTexture();
        }

        void SetTexture()
        {
            HeightMap = new Texture2D(mapSizeWidth, mapSizeWidth, TextureFormat.ARGB32, true);
            heightMapData = File.ReadAllBytes(Application.dataPath + "/StreamingAssets/NoiseMap.png");
            HeightMap.LoadImage(heightMapData);
            HeightMapPreview = HeightMap;
        }
    }

    public static Texture2D GetHeightMap()
    {
        return HeightMap;
    }
}
