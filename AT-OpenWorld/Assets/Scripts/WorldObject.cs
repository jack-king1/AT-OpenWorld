using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public abstract class WorldObject : MonoBehaviour
{
    protected WorldData wd;
    protected ThreadQueuer tq;

    public abstract void CreateObject();

    public void LoadObject()
    {
        Chunk c = ChunkManager.instance.activeChunk;
        if (DataManager.ObjectFileExist((int)c.cd.arrayPos.x, (int)c.cd.arrayPos.y, gameObject.name.ToLower()))
        {
            string path = DataManager.CreateWorldObjectFilepath((int)c.cd.arrayPos.x, (int)c.cd.arrayPos.y, gameObject.name.ToLower());
            Debug.Log("Starting Thread For Loading WorldObject");
            tq.StartThreadedFunction(() => { LoadFile(path); });
        }
        else
        {
            Debug.Log("No Chunk With That File Name.");
        }
    }


    private void LoadFile(string path)
    {
        Debug.Log("@SavingChunkData");
        string json = JsonUtility.ToJson(wd);
        File.WriteAllText(path.ToString(), json);
    }

    public void UnloadObject()
    {
        Chunk c = ChunkManager.instance.activeChunk;
        string path = DataManager.CreateWorldObjectFilepath((int)c.cd.arrayPos.x, (int)c.cd.arrayPos.y, gameObject.name.ToLower());
        tq.StartThreadedFunction(() => { SaveFile(path); });
    }

    private void SaveFile(string path)
    {
        Debug.Log("@SavingChunkData");
        string json = JsonUtility.ToJson(wd);
        File.WriteAllText(path.ToString(), json);
    }
}
