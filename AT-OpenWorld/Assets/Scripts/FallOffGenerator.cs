using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FallOffGenerator
{
    public static float[,] GenerateFallOffMap(int size)
    {
        float[,] map = new float[size + 1, size + 1];

        for(int i = 0; i < size; ++i)
        {
            for(int j = 0; j < size; ++j)
            {
                float x = i / (float)size * 2 - 1;
                float z = j / (float)size * 2 - 1;
                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(z));
                //map[i, j] = Evaluate(value);
                map[i, j] = value;
            }
        }
        return map;
    }

    static float Evaluate(float value)
    {
        float a = 0.1f;
        float b = 0.01f;

        float evaluation = Mathf.Pow(value, a) / Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a);
        return evaluation;
    }
}
