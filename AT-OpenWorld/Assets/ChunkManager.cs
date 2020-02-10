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
        int size = (int)Mathf.Sqrt(mapChunkTotal);
        //for(int x = 0; x < size; ++x)
        //{
        //    for (int z = 0; z < size; ++z)
        //    {
        //        GenerateChunk(x,z);
        //    }
        //}

        GenerateChunk(1, 1);

        activeChunk = activeChunks[0].GetComponent<Chunk>();
        newActiveChunk = true;
    }

    private void Update()
    {
        if(activeChunks.Count != 0 && PlayerManager.instance.Player != null)
        {
            if(activeChunk == null)
            {

            }
            else
            {
                foreach(GameObject c in activeChunks)
                {
                    if(Vector3.Distance(c.gameObject.transform.position, PlayerManager.instance.GetPlayer().transform.position) > 
                        (c.GetComponent<Chunk>().GetWorldSpaceBounds().size.x * 2))
                    {
                        activeChunks.Remove(c);
                        Destroy(c);
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

    public GameObject GenerateChunk(int x, int z)
    {
        //Check if chunk doesnt exist.
        GameObject newChunk = new GameObject("Chunk " + x.ToString() + z.ToString());
        newChunk.AddComponent<Chunk>();
        StartCoroutine(newChunk.GetComponent<Chunk>().BuildChunk(x, z));
        activeChunks.Add(newChunk);
        return newChunk;
    }

    bool ChunkExists(int x, int z)
    {
        for(int i =0; i < activeChunks.Count; ++i)
        {
            int xPos = (int)activeChunks[i].GetComponent<Chunk>().cd.arrayPos.x;
            int zPos = (int)activeChunks[i].GetComponent<Chunk>().cd.arrayPos.y;
            if ( xPos == x && zPos == z)
            {
                return true;
            }
        }
        return false;
    }

    void AssignActiveChunk()
    {
        foreach (GameObject c in activeChunks)
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
        foreach(GameObject c in activeChunks)
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
                if(ChunkExists((int)activeChunk.cd.arrayPos.x, (int)activeChunk.cd.arrayPos.y))
                {
                    GenerateChunk((int)activeChunk.cd.chunkNeighbour[i].x, (int)activeChunk.cd.chunkNeighbour[i].y);
                }
            }
        }
    }
}
