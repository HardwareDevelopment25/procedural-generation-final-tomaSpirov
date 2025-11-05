using System.Data;
using UnityEngine;


public static class NoiseMapGenerator
{
    public static float[,] GenerateFallOffMap(int size, AnimationCurve ac) 
    {
        float[,] falloffMap = new float[size, size];


        for (int i = 0; i < size; i++) 
        {
            for (int j = 0; j < size; j++) 
            {
                float x = i / (float)size * 2 - 1;
                float y = j / (float)size * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                falloffMap[i,j] = Evaluate(value);//uses the eval maht constant
                falloffMap[i,j] =ac.Evaluate(value);//uses the unity curve in the inspector
            }
        }

        return falloffMap;
    }

    static  float Evaluate(float val) { return val; }

    public static float[,] GenerateNoiseMap(int mapHeight, int mapWidth, float scale, float lacunarity, int octaves, float persistance, int seed, Vector2 offset)
    {
        float[,] noiseMap = new float[mapHeight, mapWidth];
        if (scale < 0) scale = 0.001f;

        float maxPossibleHeight = float.MinValue;
        float minPossibleHeight = float.MaxValue;


        System.Random rand = new System.Random(seed);

        Vector2[] octaveOffsets = new Vector2[octaves]; // try have less than 5 octaves

        // Creates x amount of sames of octave offsets for randomness
        for (int i = 0; i < octaves; i++)
        {
            float ox = rand.Next(-100000, 100000) + offset.x;
            float oy = rand.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(ox, oy);
        }

        // CREATE THE PERLIN MAP grid layout
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float amp = 1, freq = 1, noiseHeight = 0;

                for (int i = 0; i < octaves; i++) // create indiviual perlin maps 
                {
                    float sampleX = (float)(x - (mapWidth / 2)) / scale * freq + octaveOffsets[i].x;
                    float sampleY = (float)(y - (mapHeight / 2)) / scale * freq + octaveOffsets[i].y;
                    float perlinResult = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinResult * amp;
                    amp *= persistance;
                    freq *= lacunarity;
                }
                // were after the most highest peak and the lowest peak for our future lerp
                if (noiseHeight > maxPossibleHeight) maxPossibleHeight = noiseHeight;
                else if (noiseHeight < minPossibleHeight) minPossibleHeight = noiseHeight;
                noiseMap[x, y] = Mathf.InverseLerp(minPossibleHeight, maxPossibleHeight, noiseHeight);
            }
        }

        return noiseMap;


    }
}
