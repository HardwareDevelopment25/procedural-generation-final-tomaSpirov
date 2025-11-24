using Unity.VisualScripting;
using UnityEngine;

public class ShapeCreator : MonoBehaviour
{
    public int mapChunkSize;// = 128;
    public float noiseScale;// = 20f;
    public int octaves;// = 4;
    public float persistence;// = 0.5f;
    public float lacunarity;// = 2f;
    public int seed;// = 42;
    public Vector2 offset = Vector2.zero;

    public float sizeOfShape;// = 1.0f;
    public float heightMultiplier = 10f;
    [Range(1, 6)]
    public int levelOfDetails;// = 1;
    
    public AnimationCurve falloffCurve;
    public bool useFalloff;

    public bool autoUpdate;//for Editor use

    [Header("Terrain Regions")]
    public MapColorGenerator.TerrainType[] regions;//FOR SOME REASON THIS NOT PICK THE VALUES FROM THE INSPECTOR - use Awake TO INITIALIZE IT
    float[,] falloffMap;

   public MeshFilter mf;
    public MeshRenderer mr;

    private void Awake()
    {
        this.mf = this.AddComponent<MeshFilter>();
        this.mr = this.AddComponent<MeshRenderer>();

        regions = new MapColorGenerator.TerrainType[]
{
    new MapColorGenerator.TerrainType { name = "Water", height = 0.3f, color = Color.blue },
    new MapColorGenerator.TerrainType { name = "Sand", height = 0.4f, color = Color.orange },
    new MapColorGenerator.TerrainType { name = "Grass", height = 0.6f, color = Color.green },
    new MapColorGenerator.TerrainType { name = "Mountain", height = 0.8f, color = Color.gray },
    new MapColorGenerator.TerrainType { name = "Snow", height = 1.0f, color = Color.white }
};
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        /*
         
        MeshFilter mf = this.AddComponent <MeshFilter>();
        MeshRenderer mr = this.AddComponent <MeshRenderer>();

        Material mat = new Material(Shader.Find("Unlit/Texture"));
        mf.mesh = ProcGenTool.makeTriangle(sizeOfShape);
        mr.material = mat;

        Material mat = new Material(Shader.Find("Unlit/Texture"));
        mf.mesh = ProcGenTool.makeSquare(sizeOfShape);
        mr.material = mat;

        */


       
        

        Material mat = new Material(Shader.Find("Unlit/Texture"));
        //mr.material = mat;
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, noiseScale, octaves, persistence, lacunarity, seed, offset);

        // Apply falloff if enabled
        if (useFalloff)
        {
            falloffMap = Noise.GenerateFallOffMap(mapChunkSize, falloffCurve);
            for (int y = 0; y < mapChunkSize; y++)
            {
                for (int x = 0; x < mapChunkSize; x++)
                {
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
                }
            }
        }



        MeshData md = MeshGenerator.GenerateTerrain(noiseMap, heightMultiplier, falloffCurve, levelOfDetails);
        mf.sharedMesh = md.CreateMesh();

        //mat.mainTexture = ProcGenTool.RenderNoiseAsGrayTexture(noiseMap);
        Color[] colourMap = MapColorGenerator.GenerateColorMap(noiseMap, regions, mapChunkSize);

        // Create texture and apply to renderer
        mat.mainTexture = MapColorGenerator.GenerateTexture(colourMap, mapChunkSize);
        //mat.mainTexture = MapColorGenerator.GenerateColorMap(noiseMap, regions, mapChunkSize);
        mr.sharedMaterial = mat;

    }
    public void DrawMapInEditor()
    {
        Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
