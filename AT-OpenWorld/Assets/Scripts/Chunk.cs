using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class Chunk : MonoBehaviour
{
    public ChunkData cd;
    [SerializeField] MeshCollider collider;
    public Mesh mesh;
    [SerializeField]private int chunkID;
    public ThreadQueuer tq;
    float[,] localNoiseMap;
    private void Start()
    {
        tq = gameObject.GetComponent<ThreadQueuer>();
    }

    public void BuildChunk(int x, int z)
    {
        Debug.Log("@BuildChunk. Building Chunk: " + x.ToString() + z.ToString());
        cd = new ChunkData();
        tq = gameObject.GetComponent<ThreadQueuer>();
        //Check to see if chunk already has json.
        if (DataManager.FileExist(x,z) )
        {
            gameObject.GetComponent<MeshRenderer>().material = ColourMapData.instance.mat;
            LoadChunk(x,z);
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().material = ColourMapData.instance.mat;
            cd.size = (int)ChunkManager.instance.chunkSize;
            SetChunkPosition(x,z);
            GetLocalHeightMap();
            GetLocalColourMap();
            //GetcolourMap();
            CreateMesh();
            cd.position.x = transform.position.x;
            cd.position.z = transform.position.z;
            SetChunkNeighbours();
            CreateJSONFile(x,z);
            ChunkManager.instance.AddChunkToList(gameObject);
        }
    }

    private void GetcolourMap()
    {
        //assign a random colouir //old colour map.
        cd.meshColor = new Color[(cd.size + 1) * (cd.size + 1)];
        int colourCount = 0;
        Color assignedColour = new Color(Random.Range(0F, 1F), Random.Range(0, 1F), Random.Range(0, 1F));
        for (int z = 0; z <= cd.size; ++z)
        {
            for (int x = 0; x <= cd.size; ++x)
            {
                cd.meshColor[colourCount] = assignedColour;
                ++colourCount;
            }
        }
    }


    private void GetLocalColourMap()
    {
        cd.meshColor = new Color[(cd.size + 1) * (cd.size + 1)];
        int colourCount = 0;
        for (int z = 0; z <= cd.size; ++z)
        {
            for (int x = 0; x <= cd.size; ++x)
            {
                float currentHeight = localNoiseMap[x, z];
                for (int i = 0; i < ColourMapData.instance.regions.Length; ++i)
                {
                    if (currentHeight <= ColourMapData.instance.regions[i].height)
                    {
                        cd.meshColor[colourCount] = ColourMapData.instance.regions[i].colour;
                    }
                }
                ++colourCount;
            }
        }
        bool fam = false;
    }

    private void GetLocalHeightMap()
    {
        //Get height map from texture2D created from png in Noise.cs.
        Texture2D nm = HeightMapGenerator.GetHeightMap();
        localNoiseMap = new float[cd.size + 1, cd.size + 1];
        // Debug.Log("NoiseMap Size: " + noiseMap.Length);
        for (int x = 0; x <= cd.size; ++x)
        {
            for (int z = 0; z <= cd.size; ++z)
            {
                int xPos = x + ((int)cd.arrayPos.x * cd.size);
                int zPos = z + ((int)cd.arrayPos.y * cd.size);
                localNoiseMap[x, z] = nm.GetPixel(xPos, zPos).grayscale;
            }
        }

        bool fam = false;
    }

    private void CreateMesh()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        collider = GetComponent<MeshCollider>();
        mesh.name = "Procedural Grid";
        cd.vertices = new Vector3[(cd.size + 1) * (cd.size + 1)];
        cd.uv = new Vector2[cd.vertices.Length];
        cd.tangents = new Vector4[cd.vertices.Length];
        cd.tangent = new Vector4(1f, 0f, 0f, -1f);

        for (int i = 0, z = 0; z <= cd.size; z++)
        {
            for (int x = 0; x <= cd.size ; x++, i++)
            {
                float evHeight = HeightMapGenerator.instance.ac.Evaluate(localNoiseMap[x, z]);
                cd.vertices[i] = new Vector3(x * ChunkManager.instance.verticySpaceing,
                    (evHeight * HeightMapGenerator.instance.meshHeightMultiplier),
                    z * ChunkManager.instance.verticySpaceing);


                //cd.vertices[i] = new Vector3(x * ChunkManager.instance.verticySpaceing,
                //    0,
                //     z * ChunkManager.instance.verticySpaceing);


                cd.uv[i] = new Vector2((float)x / cd.size, (float)z / cd.size);
                cd.tangents[i] = cd.tangent;
            }
        }

        mesh.vertices = cd.vertices;
        mesh.uv = cd.uv;
        mesh.tangents = cd.tangents;
        cd.triangles = new int[cd.size * cd.size * 6];
        for (int ti = 0, vi = 0, z = 0; z < cd.size; z++, vi++)
        {
            for (int x = 0; x < cd.size; x++, ti += 6, vi++)
            {
                cd.triangles[ti] = vi;
                cd.triangles[ti + 3] = cd.triangles[ti + 2] = vi + 1;
                cd.triangles[ti + 4] = cd.triangles[ti + 1] = vi + cd.size + 1;
                cd.triangles[ti + 5] = vi + cd.size + 2;
            }
        }
        mesh.colors = cd.meshColor;
        mesh.triangles = cd.triangles;
        mesh.RecalculateNormals();
        collider.sharedMesh = mesh;
    }

    void SetChunkPosition(int x, int z)
    {
        cd.arrayPos = new Vector2(x, z);

        gameObject.transform.position = new Vector3((x * (cd.size * ChunkManager.instance.verticySpaceing)), 0, 
            z * (cd.size * ChunkManager.instance.verticySpaceing));
    }

    public void UnloadChunk()
    {
        string path = DataManager.CreateFilepath((int)cd.arrayPos.x, (int)cd.arrayPos.y);
        tq.StartThreadedFunction(() => { SaveChunkData(this.cd, path); });
        //Destroy(this.gameObject);
    }

    void DestroyChunk()
    {
        Destroy(this.gameObject);
    }

    public void LoadChunk(int x, int z)
    {
        if (DataManager.FileExist(x, z))
        {
            string path = DataManager.CreateFilepath(x,z);
            Debug.Log("Starting Thread For Loading:" + x.ToString() + z.ToString());
            tq.StartThreadedFunction(() => { LoadChunkData(path); });
        }
        else
        {
            Debug.Log("No Chunk With That File Name.");
        }
    }

    public void LoadChunkData(string path)
    {
        Debug.Log("@LoadingChunkData");
        ChunkData newChunk;
        string json = File.ReadAllText(path);
        newChunk = JsonUtility.FromJson<ChunkData>(json);
        tq.QueueMainThreadFunction(() => AssignChunkData(newChunk));
    }
    public void SaveChunkData(ChunkData cd, object FilePath)
    {
        Debug.Log("@SavingChunkData");
        string json = JsonUtility.ToJson(cd);
        File.WriteAllText(FilePath.ToString(), json);
        tq.QueueMainThreadFunction(() => DestroyChunk());
    }

    void CreateJSONFile(int x, int z)
    {
        //DataManager.UnloadChunkData(this.cd);
        string path = DataManager.CreateFilepath(x,z);
        tq.StartThreadedFunction(() => { DataManager.SaveChunk(this.cd, path ); });
    }

    public void AssignChunkData(ChunkData _cd)
    {
        Debug.Log("@Assigning Chunk Data with position as: " + cd.position + "Array ID: " + cd.arrayPos);
        cd = _cd;
        Debug.Log(cd.position);
        CreateMeshFromFile();
    }

    public void CreateMeshFromFile()
    {
        gameObject.transform.position = cd.position;
        if (cd != null)
        {
            collider = GetComponent<MeshCollider>();
            GetComponent<MeshFilter>().mesh = mesh = new Mesh();
            mesh.vertices = cd.vertices;
            mesh.uv = cd.uv;
            mesh.tangents = cd.tangents;
            mesh.colors = cd.meshColor;
            mesh.triangles = cd.triangles;
            mesh.RecalculateNormals();
            collider.sharedMesh = mesh;
        }
        else
        {
            Debug.Log("Chunk Data is NULL");
        }
        ChunkManager.instance.AddChunkToList(gameObject);
    }

    public Bounds GetWorldSpaceBounds()
    {
        return gameObject.GetComponent<Renderer>().bounds;
    }

    void SetChunkNeighbours()
    {
        int size = (int)Mathf.Sqrt(ChunkManager.instance.mapChunkTotal);
        size -= 1;
        
        cd.chunkNeighbour = new Vector2[8];

        if(cd.arrayPos.y < size)
        {
            cd.chunkNeighbour[(int)directions.N] = new Vector2(cd.arrayPos.x, cd.arrayPos.y + 1);
        }
        else
        {
            cd.chunkNeighbour[(int)directions.N] = new Vector2(-1, -1);
        }

        if (cd.arrayPos.x < size && cd.arrayPos.y < size)
        {
            cd.chunkNeighbour[(int)directions.NE] = new Vector2(cd.arrayPos.x + 1, cd.arrayPos.y + 1);
        }
        else
        {
            cd.chunkNeighbour[(int)directions.NE] = new Vector2(-1, -1);
        }

        if (cd.arrayPos.x < size)
        {
            cd.chunkNeighbour[(int)directions.E] = new Vector2(cd.arrayPos.x + 1, cd.arrayPos.y);
        }
        else
        {
            cd.chunkNeighbour[(int)directions.E] = new Vector2(-1, -1);
        }

        if (cd.arrayPos.x < size && cd.arrayPos.y > 0)
        {
            cd.chunkNeighbour[(int)directions.SE] = new Vector2(cd.arrayPos.x + 1, cd.arrayPos.y - 1);
        }
        else
        {
            cd.chunkNeighbour[(int)directions.SE] = new Vector2(-1, -1);
        }

        if (cd.arrayPos.y > 0)
        {
            cd.chunkNeighbour[(int)directions.S] = new Vector2(cd.arrayPos.x, cd.arrayPos.y - 1);
        }
        else
        {
            cd.chunkNeighbour[(int)directions.S] = new Vector2(-1, -1);
        }

        if (cd.arrayPos.x > 0 && cd.arrayPos.y > 0)
        {
            cd.chunkNeighbour[(int)directions.SW] = new Vector2(cd.arrayPos.x - 1, cd.arrayPos.y - 1);
        }
        else
        {
            cd.chunkNeighbour[(int)directions.SW] = new Vector2(-1, -1);
        }

        if (cd.arrayPos.x > 0)
        {
            cd.chunkNeighbour[(int)directions.W] = new Vector2(cd.arrayPos.x - 1, cd.arrayPos.y);
        }
        else
        {
            cd.chunkNeighbour[(int)directions.W] = new Vector2(-1, -1);
        }

        if (cd.arrayPos.x > 0 && cd.arrayPos.y < size)
        {
            cd.chunkNeighbour[(int)directions.NW] = new Vector2(cd.arrayPos.x - 1, cd.arrayPos.y + 1);
        }
         else
        {
            cd.chunkNeighbour[(int)directions.NW] = new Vector2(-1, -1);
        }
    }
}
