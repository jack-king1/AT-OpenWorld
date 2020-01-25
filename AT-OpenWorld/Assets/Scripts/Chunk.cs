using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class Chunk : MonoBehaviour
{
    public int xSize, zSize;
    float meshHeightMultiplier = 50;
    public AnimationCurve meshHeightCurve;
    [SerializeField] MeshCollider collider;
    public Mesh mesh;
    public Vector3[] vertices;
    public Color[] colourMap;
    public float[,] noiseMap;

    void Awake()
    {
        gameObject.GetComponent<MeshRenderer>().material = ColourMapData.instance.test;
        float[] dimensions = HeightMapGenerator.GetChunkDimensions();
        xSize = (int)dimensions[0];
        zSize = (int)dimensions[1];
        GetHeightMap();
        GetColourMap();
        GenerateAnimationCurve();
        CreateMesh();
    }

    private void GetHeightMap()
    {
        //Get height map from texture2D created from png in Noise.cs.
        Texture2D nm = HeightMapGenerator.GetHeightMap();
        noiseMap = new float[xSize +1, zSize+1];
        Debug.Log("NoiseMap Size: " + noiseMap.Length);
        for(int z = 0; z < xSize; ++z)
        {
            for(int x = 0; x < zSize; ++x)
            {
                int xPos = x + (ChunkGenerator.chunkCount * xSize);
                int zPos = z + (ChunkGenerator.chunkCount * zSize);
                noiseMap[x, z] = nm.GetPixel( xPos, zPos).grayscale;
            }
        }
    }

    private void GetColourMap()
    {
        colourMap = new Color[(xSize + 1) * (zSize + 1)];
        int colourCount = 0;
        for (int z = 0; z <= zSize; ++z)
        {
            for (int x = 0; x <= xSize; ++x)
            {
                float currentHeight = noiseMap[x, z];
                for (int i = 0; i < ColourMapData.instance.regions.Length; ++i)
                {
                    if (currentHeight <= ColourMapData.instance.regions[i].height)
                    {
                        //Debug.Log("Vertex Count: " + colourCount + "Current Vertex Height: " + currentHeight + " Name: " + regions[i].name);
                        colourMap[colourCount] = ColourMapData.instance.regions[i].colour;
                    }

                }
                ++colourCount;
            }
        }
    }

    private void CreateMesh()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        collider = GetComponent<MeshCollider>();
        mesh.name = "Procedural Grid";
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        Vector4[] tangents = new Vector4[vertices.Length];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                vertices[i] = new Vector3(x, meshHeightCurve.Evaluate(noiseMap[x, z]) * meshHeightMultiplier, z);
                uv[i] = new Vector2((float)x / xSize, (float)z / zSize);
                tangents[i] = tangent;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.tangents = tangents;
        int[] triangles = new int[xSize * zSize * 6];
        for (int ti = 0, vi = 0, z = 0; z < zSize; z++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
        }
        mesh.colors = colourMap;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        collider.sharedMesh = mesh;
    }

    private void GenerateAnimationCurve()
    {
        if (meshHeightCurve == null)
        {
            meshHeightCurve = new AnimationCurve(new Keyframe(0, 0),new Keyframe(0.3f,0), new Keyframe(1, 1));
        }
        meshHeightCurve.preWrapMode = WrapMode.Clamp;
        meshHeightCurve.postWrapMode = WrapMode.Clamp;
    }

    private void SetChunkPosition()
    {

    }
}
