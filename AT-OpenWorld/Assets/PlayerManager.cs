using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    [SerializeField] private GameObject playerPrefab;
    public GameObject Player;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Update()
    {
        
    }

    public void SpawnPlayer()
    {
        Vector3 position = ChunkManager.instance.activeChunks[0].GetComponent<Chunk>().GetWorldSpaceBounds().center;
        position = new Vector3(position.x, 32, position.z);
        Player = Instantiate(playerPrefab, position,
            Quaternion.identity);
    }

    public GameObject GetPlayer()
    {
        return Player;
    }
}
