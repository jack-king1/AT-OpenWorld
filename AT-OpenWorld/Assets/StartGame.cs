using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    [SerializeField]private GameObject playerPrefab;
    public GameObject Player;
    float timer = 0.5f;
    public int x;
    public int y;

    void Start()
    {
        
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            ChunkManager.instance.StartGame();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayerManager.instance.SpawnPlayer();
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            //ChunkManager.instance.GenerateChunk(2, 3);
        }
    }
}
