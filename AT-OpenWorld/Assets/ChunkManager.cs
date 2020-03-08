using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public static ChunkManager instance;
    public List<GameObject> activeChunks;
    public int chunkCount;
    public int mapChunkTotal = 16;
    public int startChunk;
    [SerializeField] private int playerActiveChunk;
    private bool newActiveChunk = true;
    public Chunk activeChunk;

    public float verticySpaceing;
    public float chunkSize;
    public float rowSize;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        activeChunks = new List<GameObject>();
        startChunk = mapChunkTotal / 4;
        playerActiveChunk = startChunk;
        newActiveChunk = true;
    }

    void Start()
    {
        rowSize = (int)Mathf.Sqrt(mapChunkTotal);
        //for(int x = 0; x < size; ++x)
        //{
        //    for (int z = 0; z < size; ++z)
        //    {
        //        GenerateChunk(x,z);
        //    }
        //}
    }

    public void StartGame(int x, int y)
    {
        GenerateChunk(x, y);
        activeChunk = activeChunks[0].GetComponent<Chunk>();
        newActiveChunk = true;
    }

    private void Update()
    {
        if(activeChunks.Count != 0 && PlayerManager.instance.Player != null)
        {
            if(activeChunk == null)
            {
                activeChunk = activeChunks[0].GetComponent<Chunk>();
            }
            else
            {
                foreach(GameObject c in activeChunks.ToArray())
                {
                    if(Vector3.Distance(c.gameObject.transform.position, PlayerManager.instance.GetPlayer().transform.position) > 
                        (c.GetComponent<Chunk>().GetWorldSpaceBounds().size.x * 3))
                    {
                        activeChunks.Remove(c);
                        c.GetComponent<Chunk>().UnloadChunk();
                    }
                }

                //Checks if player has left activechunk.
                if (!PlayerInside())
                {
                    Debug.Log("Player Left active chunk");
                    FindActiveChunk();
                }

                if(newActiveChunk)
                {
                    //loadNeighbours
                    LoadNeighbours();
                    newActiveChunk = false; 
                }
            }
        }
    }

    public void AddChunkToList(GameObject chunk)
    {
        activeChunks.Add(chunk);
    }

    public GameObject GenerateChunk(int x, int z)
    {
        //Check if chunk doesnt exist.
        GameObject newChunk = new GameObject("Chunk " + x.ToString() + z.ToString());
        newChunk.AddComponent<Chunk>();
        newChunk.AddComponent<ThreadQueuer>();
        newChunk.GetComponent<Chunk>().BuildChunk(x, z);
        return newChunk;
    }

    bool ChunkExists(int x, int z)
    {
        bool exists = false;
        foreach(GameObject go in activeChunks.ToArray())
        {
            Vector2 tempArrayPos = new Vector2(x,z);
            ChunkData test = go.GetComponent<Chunk>().cd;
            Vector2 arrayPos2 = new Vector2(go.GetComponent<Chunk>().cd.arrayPos.x, go.GetComponent<Chunk>().cd.arrayPos.y);
            if(arrayPos2 == tempArrayPos)
            {
                exists = true;
            }
        }
        return exists;
    }

    void AssignActiveChunk()
    {
        foreach (GameObject c in activeChunks.ToArray())
        {
            if (c.GetComponent<Chunk>().GetWorldSpaceBounds().Contains(PlayerManager.instance.GetPlayer().transform.position))
            {
                Debug.Log("Player in new chunk!");
                activeChunk = c.GetComponent<Chunk>();
                newActiveChunk = true;
            }
        }
    }

    void FindActiveChunk()
    {
        foreach(GameObject c in activeChunks.ToArray())
        {
            Vector3 playerPos = PlayerManager.instance.GetPlayer().gameObject.transform.position;
            Vector3 chunkPos = c.GetComponent<Chunk>().gameObject.transform.position;
            Bounds chunkSize = c.GetComponent<Chunk>().GetWorldSpaceBounds();
            if (playerPos.x >= chunkPos.x && playerPos.x <= chunkPos.x + chunkSize.size.x &&
                playerPos.z >= chunkPos.z && playerPos.z <= chunkPos.z + chunkSize.size.z)
            {
                activeChunk = c.GetComponent<Chunk>();
                newActiveChunk = true;
                Debug.Log("New Active Chunk Found!" + activeChunk.cd.arrayPos);
            }
        }
    }

    bool PlayerInside()
    {
        Vector3 playerPos = PlayerManager.instance.GetPlayer().gameObject.transform.position;
        Vector3 chunkPos = activeChunk.GetComponent<Chunk>().gameObject.transform.position;
        Bounds chunkSize = activeChunk.GetComponent<Chunk>().GetWorldSpaceBounds();
        if (playerPos.x >= chunkPos.x && playerPos.x <= chunkPos.x + chunkSize.size.x &&
            playerPos.z >= chunkPos.z && playerPos.z <= chunkPos.z + chunkSize.size.z)
        {
            return true;
        }
        else
        {
            
            return false;
        }
    }

    void LoadNeighbours()
    {
        Debug.Log("Loading Neighbours");
        for(int i = 0; i < activeChunk.cd.chunkNeighbour.Length; ++i)
        {
            if(activeChunk.cd.chunkNeighbour[i].x != -1 || activeChunk.cd.chunkNeighbour[i].y != -1)
            {
                if(!ChunkExists((int)activeChunk.cd.chunkNeighbour[i].x, (int)activeChunk.cd.chunkNeighbour[i].y))
                {
                    GenerateChunk((int)activeChunk.cd.chunkNeighbour[i].x, (int)activeChunk.cd.chunkNeighbour[i].y);
                }
            }
        }
    }
}
