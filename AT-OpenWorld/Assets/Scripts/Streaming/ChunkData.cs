using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}

public class ChunkData : MonoBehaviour
{
    public int xSize, zSize, octaves;
    public float scale, persistance, lucanarity, meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;
    MeshCollider collider;

    private Mesh mesh;
    private Vector3[] vertices;
    public float[,] noiseMap;
    public Color[] colourMap;
    public TerrainType[] regions;
}
