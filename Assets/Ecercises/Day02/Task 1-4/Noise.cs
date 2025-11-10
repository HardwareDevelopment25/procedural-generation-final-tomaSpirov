//Task 1: Create a static class Noise with a static method GenerateNoiseMap that generates a 2D array of Perlin noise values (0 or 1) based on given width, height, and scale.
//Task 2: adding octaves and lacunarity to the noise generation (see NoiseMapGenerator for reference).
//Task 3: create a 2D fallOff map that can be used to control terrain generation, ensuring smooth transition and natural-looking borders.
using UnityEngine;

//T1 step 1: create a class Noise
public static class Noise
{

    //T1 step 2: create a static method GenerateNoiseMap that takes in 3 parameters: mapWidth (int), mapHeight (int), scale (float)
    //T2 step 1: add parameters for octaves (int),persistence (float), lacunarity (float), seed (int), offset (Vector2) 
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale, int octaves, float persistence, float lacunarity, int seed, Vector2 offset)
    {
        //T1 step 3: create a 2D array to store noise values
        float[,] noiseMap = new float[mapWidth, mapHeight];

        //T1 step 4: validate the scale is > 0
        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        //T1 step 5: set up Height variables
        float maxPossibleHeight = float.MinValue;
        float minPossibleHeight = float.MaxValue;

        //T2 step 2: initialize the seed and offsets for octaves
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];//try have less than 5 octaves

        for (int i = 0; i < octaves; i++)
        {

            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;

            //optional:add at the end of this random number offsets extra value to force a positional offset if you want to scroll araund the perlin map
            offsetX *= scale;
            offsetY *= scale;

            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }


        //T1 step 6: loop through each point in the 2D array
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                //T2 step 3: generate noise map with octaves

                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    //all these create a multi-layered perlin noise pattern
                    float sampleX = (x - mapWidth / 2f) / scale * frequency + octaveOffsets[i].x;
                    float sampleY = (y - mapHeight / 2f) / scale * frequency + octaveOffsets[i].y;
                    float perlinResult = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;//this return value between -1 and 1
                    noiseHeight += perlinResult * amplitude;//scale the perlin result by amplitude added to noise height to every octave
                    amplitude *= persistence;//reduce amplitude by persistence for next octave
                    frequency *= lacunarity;//increase frequency by lacunarity for next octave
                }

                // update max and min possible heights
                if (noiseHeight > maxPossibleHeight)
                {
                    maxPossibleHeight = noiseHeight;
                }
                else if (noiseHeight < minPossibleHeight)
                {
                    minPossibleHeight = noiseHeight;
                }

                noiseMap[x, y] = Mathf.InverseLerp(minPossibleHeight, maxPossibleHeight, noiseHeight);

            }
        }
        return noiseMap;
    }

    public static float[,] GenerateFallOffMap(int size, AnimationCurve animationCurveRef) 
    {
        //T3 step 1: initialize the FallOffMap
        float[,] fallOffMap = new float[size, size];


        //T3 step2: iterate through each cell in the map
        for (int y = 0;y<size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                //T3 step 3: calculate the normalized coordinates
                float nx = x / (float)size * 2 - 1;
                float ny = y / (float)size * 2 - 1;
                //T3 step 4: determine the falloff value 
                float value = Mathf.Max(Mathf.Abs(nx), Mathf.Abs(ny));
                fallOffMap[x, y] = Evaluate(value);
                fallOffMap[x, y] = animationCurveRef.Evaluate(value);
            }
        }

        //T3 step 5: return the falloff map
        return fallOffMap;
    }

    static float Evaluate(float val) { return val; }

}
