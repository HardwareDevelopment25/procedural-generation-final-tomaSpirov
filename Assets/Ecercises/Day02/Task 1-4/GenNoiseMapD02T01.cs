using UnityEngine;

public class GenNoiseMapD02T01 : MonoBehaviour
{
    public Renderer textureRenderer;
    public int mapChunkSize = 100;
    public float noiseScale = 20f;
    public int octaves = 4;
    public float persistence = 0.5f;
    public float lacunarity = 2f;
    public int seed = 42;
    public Vector2 offset;

    public AnimationCurve falloffCurve;
    public bool useFalloff;

    [Header("Terrain Regions")]
    public MapColorGenerator.TerrainType[] regions;//FOR SOME REASON THIS NOT PICK THE VALUES FROM THE INSPECTOR - use Awake TO INITIALIZE IT


    private void Awake()
    {
        regions = new MapColorGenerator.TerrainType[]
{
    new MapColorGenerator.TerrainType { name = "Water", height = 0.3f, color = Color.blue },
    new MapColorGenerator.TerrainType { name = "Sand", height = 0.4f, color = Color.beige },
    new MapColorGenerator.TerrainType { name = "Grass", height = 0.6f, color = Color.green },
    new MapColorGenerator.TerrainType { name = "Mountain", height = 0.8f, color = Color.gray },
    new MapColorGenerator.TerrainType { name = "Snow", height = 1.0f, color = Color.white }
};
    }

    void Start()
    {
        DrawMap();
    }

    public void DrawMap()
    {
        // Generate noise map
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, noiseScale, octaves, persistence, lacunarity, seed, offset);

        // Apply falloff if enabled
        if (useFalloff)
        {
            float[,] falloffMap = Noise.GenerateFallOffMap(mapChunkSize, falloffCurve);
            for (int y = 0; y < mapChunkSize; y++)
            {
                for (int x = 0; x < mapChunkSize; x++)
                {
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
                }
            }
        }

        // Generate color map
        Color[] colourMap = MapColorGenerator.GenerateColorMap(noiseMap, regions, mapChunkSize);

        // Create texture and apply to renderer
        Texture2D texture = MapColorGenerator.GenerateTexture(colourMap, mapChunkSize);
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(mapChunkSize, 1, mapChunkSize);
       
    }
}