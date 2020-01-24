using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float [,] GenerateNoise(float xSize, float zSize, float scale, int octaves, 
        float persistance, float lacunarity)
    {
        float[,] noiseMap = new float[(int)xSize + 1,(int)zSize + 1];

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        if(scale <= 0)
        {
            scale = 0.0001f;
        }

        for(float z = 0f; z < zSize; ++z)
        {
            for(float x = 0f; x < xSize; ++x)
            {

                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for(int i = 0; i < octaves; ++i)
                {
                    float sampleX = x / scale * frequency;
                    float sampleZ = z / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleZ) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if(noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if(noiseHeight< minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                noiseMap[(int)x, (int)z] = noiseHeight;
            }
        }

        for (float z = 0f; z < zSize; ++z)
        {
            for (float x = 0f; x < xSize; ++x)
            {
                //NORMALIZE
                noiseMap[(int)x, (int)z] = 
                    Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[(int)x, (int)z]);
            }
        }
        //DataManager.SaveNoiseMapData(noiseMap);
        GenerateTextureFile(noiseMap, xSize, zSize);
        return noiseMap;
    }

    static void GenerateTextureFile(float[,] noiseMap, float xSize, float zSize)
    {
        Texture2D nm = new Texture2D((int)xSize, (int)zSize, TextureFormat.ARGB32, true);
    }
}
