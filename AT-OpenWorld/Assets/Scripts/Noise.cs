using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    

    public static void GenerateTextureFile(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1); 

        Texture2D nm = new Texture2D(width, height, TextureFormat.ARGB32, true);
        Color[] colourMap = new Color[width * height];
        for(int x = 0; x < width; ++x)
        {
            for(int z = 0; z < height; ++z)
            {
                colourMap[z * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, z]);
            }
        }
        nm.SetPixels(colourMap);
        nm.Apply();
        // Encode texture into PNG
        byte[] bytes = nm.EncodeToPNG();
        Object.Destroy(nm);
        DataManager.SaveNoiseMapData(bytes);
    }

    public static void GenerateTextureFileName(float[,] noiseMap, string name)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Texture2D nm = new Texture2D(width, height, TextureFormat.ARGB32, true);
        Color[] colourMap = new Color[width * height];
        for (int x = 0; x < width; ++x)
        {
            for (int z = 0; z < height; ++z)
            {
                colourMap[z * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, z]);
            }
        }
        nm.SetPixels(colourMap);
        nm.Apply();
        // Encode texture into PNG
        byte[] bytes = nm.EncodeToPNG();
        Object.Destroy(nm);
        DataManager.SaveNoiseMapData(bytes, name);
    }

    public static void GenerateNoiseTest(int size, float scale, int octaves,
    float persistance, float lacunarity)
    {
        Debug.Log("@GenerateNoiseTest");
        HeightMapGenerator.noiseMap = new float[size, size];
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        for (float z = 0f; z < size; ++z)
        {
            for (float x = 0f; x < size; ++x)
            {

                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; ++i)
                {
                    float sampleX = x / scale * frequency;
                    float sampleZ = z / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleZ) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                HeightMapGenerator.noiseMap[(int)x, (int)z] = noiseHeight;
            }
        }

        for (float z = 0f; z < size; ++z)
        {
            for (float x = 0f; x < size; ++x)
            {
                //NORMALIZE
                HeightMapGenerator.noiseMap[(int)x, (int)z] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, HeightMapGenerator.noiseMap[(int)x, (int)z]);
            }
        }
        CircularFallOff((int)size, ref HeightMapGenerator.noiseMap);
        GenerateTextureFile(HeightMapGenerator.noiseMap);
    }

    static void CircularFallOff(int size, ref float[,] noiseMap)
    {
        float[,] fallOffMap = FallOffGenerator.GenerateFallOffMap(size);
        for(int x = 0; x < size; ++x)
        {
            for (int z = 0; z < size; ++z)
            {
                float nm = noiseMap[x, z];
                float fm = fallOffMap[x, z];
                noiseMap[x, z] = Mathf.Clamp01(noiseMap[x, z] - fallOffMap[x, z]);
            }
        }
    }
}
