using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkData 
{
    public int chunkID;
    public int xSize, zSize;
    public Vector2 offset;
    public float meshHeightMultiplier = 100;
    public Vector3[] vertices;
    public Color[] colourMap;
    public Vector3 position;
    public int[] chunkNeighbour;
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
