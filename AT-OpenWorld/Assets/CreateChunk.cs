using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateChunk : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            DataManager.LoadChunkData(0);
        }
    }
}
