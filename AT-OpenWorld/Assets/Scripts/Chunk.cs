using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class Chunk : MonoBehaviour
{
    public ChunkData cd;

    public AnimationCurve meshHeightCurve;
    [SerializeField] MeshCollider collider;
    public Mesh mesh;
    public float[,] noiseMap;
    [SerializeField]private int chunkID;
    void Awake()
    {
        cd = new ChunkData();

        //Check to see if chunk already has json. 
        if(DataManager.FileExist(ChunkGenerator.chunkCount))
        {
            cd = DataManager.LoadChunkData(cd.chunkID);
            gameObject.GetComponent<MeshRenderer>().material = ColourMapData.instance.mat;
            GetHeightMap();
            GenerateAnimationCurve();
            CreateMesh();
            chunkID = cd.chunkID;
        }
        else
        {
            cd.chunkID = ChunkGenerator.chunkCount;
            gameObject.GetComponent<MeshRenderer>().material = ColourMapData.instance.mat;
            float[] dimensions = HeightMapGenerator.GetChunkDimensions();
            cd.xSize = (int)dimensions[0];
            cd.zSize = (int)dimensions[1];
            SetChunkPosition();
            GetHeightMap();
            GetcolourMap();
            GenerateAnimationCurve();
            CreateMesh();
            cd.position.x = transform.position.x;
            cd.position.z = transform.position.z;
            CreateJSONObject();
        }
    }

    private void GetHeightMap()
    {
        //Get height map from texture2D created from png in Noise.cs.
        Texture2D nm = HeightMapGenerator.GetHeightMap();
        noiseMap = new float[cd.xSize +1, cd.zSize+1];
        Debug.Log("NoiseMap Size: " + noiseMap.Length);
        for(int z = 0; z < cd.xSize; ++z)
        {
            for(int x = 0; x < cd.zSize; ++x)
            {
                //int xPos = x + (ChunkGenerator.chunkCount * cd.xSize);
                int xPos = x;
                int zPos = z + (ChunkGenerator.chunkCount * cd.zSize);
                noiseMap[x, z] = nm.GetPixel( xPos, zPos).grayscale;
            }
        }
    }

    private void GetcolourMap()
    {
        cd.colourMap = new Color[(cd.xSize + 1) * (cd.zSize + 1)];
        int colourCount = 0;
        for (int z = 0; z <= cd.zSize; ++z)
        {
            for (int x = 0; x <= cd.xSize; ++x)
            {
                float currentHeight = noiseMap[x, z];
                for (int i = 0; i < ColourMapData.instance.regions.Length; ++i)
                {
                    if (currentHeight <= ColourMapData.instance.regions[i].height)
                    {
                        //Debug.Log("Vertex Count: " + colourCount + "Current Vertex Height: " + currentHeight + " Name: " + regions[i].name);
                        cd.colourMap[colourCount] = ColourMapData.instance.regions[i].colour;
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
        cd.vertices = new Vector3[(cd.xSize + 1) * (cd.zSize + 1)];
        Vector2[] uv = new Vector2[cd.vertices.Length];
        Vector4[] tangents = new Vector4[cd.vertices.Length];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
        for (int i = 0, z = 0; z <= cd.zSize; z++)
        {
            for (int x = 0; x <= cd.xSize; x++, i++)
            {
                cd.vertices[i] = new Vector3(x, meshHeightCurve.Evaluate(noiseMap[x, z]) * cd.meshHeightMultiplier, z);
                uv[i] = new Vector2((float)x / cd.xSize, (float)z / cd.zSize);
                tangents[i] = tangent;
            }
        }

        mesh.vertices = cd.vertices;
        mesh.uv = uv;
        mesh.tangents = tangents;
        int[] triangles = new int[cd.xSize * cd.zSize * 6];
        for (int ti = 0, vi = 0, z = 0; z < cd.zSize; z++, vi++)
        {
            for (int x = 0; x < cd.xSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + cd.xSize + 1;
                triangles[ti + 5] = vi + cd.xSize + 2;
            }
        }
        mesh.colors = cd.colourMap;
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
        //set z to moduals and rounded divion on the x
        int count = ChunkGenerator.chunkCount;
        float xPos = 0;
        float offSetX;
        if (count >= 0 && count <= 3)
        {
            xPos = 0;

        }
        else if(count >= 4 && count <= 7)
        {
            xPos = 1;
        }
        else if (count >= 8 && count <= 11)
        {
            xPos = 2;
        }
        else if (count >= 12 && count <= 15)
        {
            xPos = 3;
        }

        float offsetZ = (ChunkGenerator.chunkCount % Mathf.Sqrt(ChunkGenerator.mapChunkTotal));
        Vector3 newPos = new Vector3(cd.xSize * xPos, 0, cd.zSize * offsetZ);
        gameObject.transform.position = newPos;
        cd.position = newPos;
    }

    void CreateJSONObject()
    {
        DataManager.SaveChunkData(this.cd);
    }
}
