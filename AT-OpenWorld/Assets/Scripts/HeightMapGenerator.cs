using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightMapGenerator : MonoBehaviour
{
    private float ChunkWidth = 128f;
    private float ChunkHeight = 128f;
    public int xSize, zSize, octaves;
    public float scale, persistance, lucanarity, meshHeightMultiplier;

    [SerializeField]
    public enum ChunkSize
    {
        Four = 4,
        Nine = 9,
        Sixteen = 16,
        TwentyFive = 25

    }

    public ChunkSize MapChunkSize;

    void Start()
    {
        Noise.GenerateNoiseTest(ChunkWidth * (float)MapChunkSize, ChunkHeight * (float)MapChunkSize, scale, octaves, persistance, lucanarity);
    }

}
