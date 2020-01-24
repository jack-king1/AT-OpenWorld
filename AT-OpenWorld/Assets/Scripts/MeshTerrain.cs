using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class MeshTerrain: MonoBehaviour
{
    public int xSize, zSize, octaves;
    public float scale, persistance, lucanarity, meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;
    [SerializeField]MeshCollider collider;
    [SerializeField]private Mesh mesh;
    [SerializeField]private Vector3[] vertices;
    public float[,] noiseMap;
    public Color[] colourMap;
    public TerrainType[] regions;
    private void Awake()
    {
        GenerateNoiseMap();
        GenerateColourMap();
        Generate();
        
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Saving Chunk!");
            DataManager.SaveChunkData(gameObject.GetComponent<MeshTerrain>());
        }
    }

    private void Generate()
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
                vertices[i] = new Vector3(x, meshHeightCurve.Evaluate(noiseMap[x, z]) * meshHeightMultiplier , z);
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

    void GenerateNoiseMap()
    {
        noiseMap = Noise.GenerateNoise((float)xSize, (float)zSize, scale, octaves, persistance, lucanarity);
    }

    void GenerateColourMap()
    {
        colourMap = new Color[(xSize + 1) * (zSize + 1)];
        int colourCount = 0;
        for (int z = 0; z <= zSize; ++z)
        {
            for (int x = 0; x <= xSize; ++x)
            {
                float currentHeight = noiseMap[x, z];
                for (int i = 0; i < regions.Length; ++i)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        //Debug.Log("Vertex Count: " + colourCount + "Current Vertex Height: " + currentHeight + " Name: " + regions[i].name);
                        colourMap[colourCount] = regions[i].colour;
                    }
                    
                }
                ++colourCount;
            }
        }
    }

    [System.Serializable]
    public struct TerrainType
    {   
        public string name;
        public float height;
        public Color colour;
    }

    void SaveChunk()
    {

    }
}
