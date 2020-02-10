using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class Chunk : MonoBehaviour
{
    public ChunkData cd;
    [SerializeField] MeshCollider collider;
    public Mesh mesh;
    [SerializeField] public Vector2 chunkArrayPos;
    public Thread LoadChunk;
    SerializableMeshInfo smi;

    private void Start()
    {

    }

    public void BuildChunk(int x, int z)
    {
        cd = new ChunkData();
        //Check to see if chunk already has json.
        //if (DataManager.FileExist((int)chunkArrayPos.x, (int)chunkArrayPos.y))
        //{
        //    gameObject.GetComponent<MeshRenderer>().material = ColourMapData.instance.mat;
        //    Debug.Log("Loading From File: Chunk " + chunkArrayPos);
        //    DataManager.LoadChunkData((int)chunkArrayPos.x, (int)chunkArrayPos.y);
        //    CreateMeshFromFile();
        //    transform.position = cd.position;
        //}
        //else
        //{
        gameObject.GetComponent<MeshRenderer>().material = ColourMapData.instance.mat;
        cd.size = 128;
        SetChunkPosition(x,z);
        GetcolourMap();
        CreateMesh();
        cd.position.x = transform.position.x;
        cd.position.z = transform.position.z;
        SetChunkNeighbours();
        //CreateJSONFile();

        //}

    }

    private void GetcolourMap()
    {
        cd.meshColor = new Color[(cd.size + 1) * (cd.size + 1)];
        int colourCount = 0;
        Color assignedColour = new Color(UnityEngine.Random.Range(0F, 1F), UnityEngine.Random.Range(0, 1F), UnityEngine.Random.Range(0, 1F));
        for (int z = 0; z <= cd.size; ++z)
        {
            for (int x = 0; x <= cd.size; ++x)
            {
                cd.meshColor[colourCount] = assignedColour;
                ++colourCount;
            }
        }
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
            for (int x = 0; x <= cd.size; x++, i++)
            {
                cd.vertices[i] = new Vector3(x, 0, z);
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
        smi = new SerializableMeshInfo(mesh);
        //smi.MeshDump(mesh, (int)cd.arrayPos.x, (int)cd.arrayPos.y);
    }

    private IEnumerator CreateMeshFromFile()
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
        yield return null;
    }

    void SetChunkPosition(int x, int z)
    {
        cd.arrayPos = new Vector2(x, z);

        gameObject.transform.position = new Vector3(x * cd.size, 0, z * cd.size);
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
