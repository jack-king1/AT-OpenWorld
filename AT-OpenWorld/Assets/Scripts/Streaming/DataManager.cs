using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataManager : MonoBehaviour
{
    public ChunkData chunk;

    public string filepath;

    public void Save()
    {

    }

    public void Load()
    {

    }

    private void WriteToFile(string fileName, string json)
    {

    }

    private string ReadFromFile(string fileName)
    {
        string path = GetFilePath(fileName);
        if(File.Exists(path))
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
                return json;
            }

        }
        else
        {
            Debug.LogWarning("File not found!");
        }

        return "";
    }

    private string GetFilePath(string fileName)
    {
        return "";
    }
}
