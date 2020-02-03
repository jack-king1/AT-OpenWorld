using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    [SerializeField]private GameObject playerPrefab;
    public GameObject Player;
    float timer = 0.5f;
    void Start()
    {
        
    }

    private void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            ChunkManager.instance.GenerateChunk(ChunkManager.instance.startChunk);
            
            
            PlayerManager.instance.SpawnPlayer();
            Destroy(this);
        }   
    }


}
