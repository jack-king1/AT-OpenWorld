using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadUnloadTest : MonoBehaviour
{

    public GameObject chunkTest;
    int count = 0;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.O))
        {
            //Opening File
            for(int i = 0; i < (ChunkManager.instance.mapChunkTotal / 4); ++i)
            {
                chunkTest = ChunkManager.instance.GenerateChunk(count);
                count++;
            }
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log("Unlaoding File");
            DataManager.UnloadChunkData(chunkTest.GetComponent<ChunkData>());
            Destroy(chunkTest.gameObject);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {

                chunkTest = ChunkManager.instance.GenerateChunk(ChunkManager.instance.startChunk);
        }
    }
}
