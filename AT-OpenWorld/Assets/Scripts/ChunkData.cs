using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkData 
{
    public int size;
    public Vector3[] vertices;
    public Vector3 position;
    public Vector2 arrayPos;
    public Vector2[] chunkNeighbour;
    public Color[] meshColor;
    public Vector2[] uv;
    public Vector4[] tangents;
    public Vector4 tangent;
    public int[] triangles;
}

[System.Serializable]
public enum directions
{
    N = 0,
    NE = 1,
    E = 2,
    SE = 3,
    S = 4,
    SW = 5,
    W = 6,
    NW = 7
}
