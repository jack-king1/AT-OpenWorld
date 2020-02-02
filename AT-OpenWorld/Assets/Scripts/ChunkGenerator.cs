using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    public static int chunkCount;
    public static List<GameObject> chunkPreviewList;
    public static int mapChunkTotal = 400;


    public static void GenerateChunks(float chunkWidth, float chunkHeight)
    {
        int totalChunks = (int)chunkWidth * (int)chunkHeight;

        for(int i = 0; i < mapChunkTotal; ++i)
        {
            GameObject newChunk = new GameObject("Chunk " + i.ToString());
            newChunk.AddComponent<Chunk>();
            ++chunkCount;
        }
    }

    public static GameObject GenerateChunk(int chunkID)
    {
        GameObject newChunk = new GameObject("Chunk " + chunkID.ToString());
        newChunk.AddComponent<Chunk>();
        newChunk.GetComponent<Chunk>().BuildChunk(chunkID);
        return newChunk;
    }
}
