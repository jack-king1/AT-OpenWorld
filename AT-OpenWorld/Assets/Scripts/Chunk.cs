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
    private bool loadedFromFile;

    public void BuildChunk(int _chunkID)
    {
        cd = new ChunkData();
        //Check to see if chunk already has json. 
        if (DataManager.FileExist(_chunkID))
        {
            Debug.Log("Loading From File: Chunk " + _chunkID.ToString());
            loadedFromFile = true;
            cd = DataManager.LoadChunkData(_chunkID);
            gameObject.GetComponent<MeshRenderer>().material = ColourMapData.instance.mat;
            GetHeightMap();
            GenerateAnimationCurve();
            CreateMesh();
            chunkID = cd.chunkID;
            transform.position = cd.position;
        }
        else
        {
            cd.chunkID = _chunkID;
            chunkID = _chunkID;
            gameObject.GetComponent<MeshRenderer>().material = ColourMapData.instance.mat;
            float[] dimensions = HeightMapGenerator.GetChunkDimensions();
            cd.xSize = (int)dimensions[0];
            cd.zSize = (int)dimensions[1];
            SetChunkPosition(_chunkID);
            GetHeightMap();
            GetcolourMap();
            GenerateAnimationCurve();
            CreateMesh();
            cd.position.x = transform.position.x;
            cd.position.z = transform.position.z;
            CreateJSONFile();
        }
    }

    private void GetHeightMap()
    {
        //Get height map from texture2D created from png in Noise.cs.
        Texture2D nm = HeightMapGenerator.GetHeightMap();
        noiseMap = new float[cd.xSize +1, cd.zSize+1];
       // Debug.Log("NoiseMap Size: " + noiseMap.Length);
        for(int x = 0; x < cd.xSize; ++x)
        {
            for(int z = 0; z < cd.zSize; ++z)
            {
                int xPos = x + ((int)cd.offset.x * cd.xSize);
                int zPos = z + ((int)cd.offset.y * cd.zSize);
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

    void SetChunkPosition(int chunkID)
    {
        float rowAmount = Mathf.Sqrt(ChunkGenerator.mapChunkTotal);
        float x = 0;
        float z = 0;
        if (chunkID == 0)
        {
            x = 0;
        }
        else
        {
            x = Mathf.FloorToInt((float)(chunkID) / rowAmount);
            z = (chunkID) % rowAmount;
        }

        cd.offset = new Vector2(x, z);


        Vector3 newPos = new Vector3(x * cd.xSize, 0, z * cd.zSize);
        gameObject.transform.position = newPos;
        cd.position = newPos;
    }

    void UnloadChunk()
    {
        DataManager.UnloadChunkData(this.cd);
        Destroy(this.gameObject);
    }

    void CreateJSONFile()
    {
        DataManager.UnloadChunkData(this.cd);
    }
}
