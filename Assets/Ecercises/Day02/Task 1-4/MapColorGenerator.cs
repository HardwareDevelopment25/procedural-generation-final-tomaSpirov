using UnityEngine;

// This class is separate from Noise.cs. It handles color mapping and texture generation based on noise values.
public static class MapColorGenerator
{
    // Struct to define terrain regions based on height
    [System.Serializable]
    public struct TerrainType
    {
        public string name;
        public float height;
        public Color color;
    }

    // Generates a color map from a noise map and terrain regions
    public static Color[] GenerateColorMap(float[,] noiseMap, TerrainType[] regions, int mapChunkSize)
    {
        Color[] colourMap = new Color[mapChunkSize * mapChunkSize];

        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];

                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        colourMap[y * mapChunkSize + x] = regions[i].color;
                        break;
                    }
                }
            }
        }

        return colourMap;
    }

    // Converts a color map into a Texture2D for visualization
    public static Texture2D GenerateTexture(Color[] colourMap, int mapChunkSize)
    {
        Texture2D texture = new Texture2D(mapChunkSize, mapChunkSize);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        texture.SetPixels(colourMap);
        texture.Apply();

        return texture;
    }
}