using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourMapData : MonoBehaviour
{
    public TerrainType[] regions;
    public Material test;
    public static ColourMapData instance;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    [System.Serializable]
    public struct TerrainType
    {
        public string name;
        public float height;
        public Color colour;
    }
}
