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
    private int playerActiveChunk;
    private bool newActiveChunk = true;
    private Chunk activeChunk;


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
        //activeChunk = activeChunks[0].GetComponent<Chunk>();
    }

    private void Update()
    {
        if(activeChunks.Count != 0)
        {

            foreach (GameObject chunk in activeChunks)
            {
                //Check to see if chunk is within distance of player to be rendered if npot delete it/unload it
                Chunk chunkObject = chunk.GetComponent<Chunk>();
                if (Vector3.Distance(PlayerManager.instance.GetPlayer().gameObject.transform.position,
                    chunk.gameObject.transform.position) > 256)
                {
                    Debug.Log("Deleting Chunk");
                    //Call chunk unload here
                    activeChunks.Remove(chunk);
                    Destroy(chunk);
                }

                //Check to see if player is in new chunk.
                if(chunkObject.GetWorldSpaceBounds().Contains(PlayerManager.instance.GetPlayer().gameObject.transform.position))
                {
                    playerActiveChunk = chunk.GetComponent<Chunk>().cd.chunkID;
                    newActiveChunk = true;
                    for(int i = 0; i < activeChunks.Count; ++i)
                    {
                        if(activeChunks[i].GetComponent<Chunk>().cd.chunkID == playerActiveChunk)
                        {
                            activeChunk = activeChunks[i].GetComponent<Chunk>();
                        }
                    }
                }

                //if player is in a new chunk generate its correct neighbours.
                if(newActiveChunk)
                {
                    if(activeChunk == null)
                    {
                        activeChunk = activeChunks[0].GetComponent<Chunk>();
                    }
                    //Check if neighbour already exists.
                    int activeChunkCount = activeChunks.Count;
                    for(int n = 0; n < activeChunk.cd.chunkNeighbour.Length; ++n)
                    {
                        if(chunkExists(activeChunk.cd.chunkNeighbour[n]) && activeChunk.cd.chunkNeighbour[n] != -1)
                        {
                            GenerateChunk(activeChunk.cd.chunkNeighbour[n]);
                        }
                    }
                    newActiveChunk = false;
                }
            }
        }
    }

    public void GenerateChunks(float chunkWidth, float chunkHeight)
    {
        int totalChunks = (int)chunkWidth * (int)chunkHeight;

        for (int i = 0; i < mapChunkTotal; ++i)
        {
            GameObject newChunk = new GameObject("Chunk " + i.ToString());
            newChunk.AddComponent<Chunk>();
            ++chunkCount;
        }
    }

    public GameObject GenerateChunk(int chunkID)
    {
        //Check if chunk doesnt exist.
        GameObject newChunk = new GameObject("Chunk " + chunkID.ToString());
        newChunk.AddComponent<Chunk>();
        newChunk.GetComponent<Chunk>().BuildChunk(chunkID);
        activeChunks.Add(newChunk);
        return newChunk;   
    }

    bool chunkExists(int n)
    {
        for(int i =0; i < activeChunks.Count; ++i)
        {
            if(activeChunks[i].GetComponent<Chunk>().cd.chunkID == n)
            {
                return false;
            }
        }

        return true;
    }
}
