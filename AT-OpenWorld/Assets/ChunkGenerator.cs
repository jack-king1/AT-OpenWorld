using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    [SerializeField]
    public enum ChunkSize
    {
        Six = 6,
        Nine = 9,
        Twelve = 12
    }

    public ChunkSize MapChunkSize;

}
