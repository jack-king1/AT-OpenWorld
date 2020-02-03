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
                }

                //if player is in a new chunk generate its correct neighbours.
                if(newActiveChunk)
                {
                    //Check if neighbour already exists.
 
                    foreach(GameObject currentChunk in activeChunks)
                    {
                        for (int i = 0; i < chunkObject.cd.chunkNeighbour.Length; ++i)
                        {
                            if (chunkObject.cd.chunkNeighbour[i] != currentChunk.GetComponent<Chunk>().cd.chunkID)
                            {
                                if(chunkObject.cd.chunkNeighbour[i] != -1)
                                {
                                    GenerateChunk(chunkObject.cd.chunkNeighbour[i]);
                                }
                            }
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
}
