using Unity.VisualScripting;
using UnityEngine;

public class ShapeCreator : MonoBehaviour
{
    public int SizeOfGrid = 128;
    public float sizeOfShape = 1.0f;
    public float heightMultiplier = 10f;
    [Range(1, 6)]
    public int levelOfDetails = 1;
    public AnimationCurve animationCurve;



    [Header("Terrain Regions")]
    public MapColorGenerator.TerrainType[] regions;//FOR SOME REASON THIS NOT PICK THE VALUES FROM THE INSPECTOR - use Awake TO INITIALIZE IT


    private void Awake()
    {
        regions = new MapColorGenerator.TerrainType[]
{
    new MapColorGenerator.TerrainType { name = "Lava", height = 0.1f, color = Color.red},
    new MapColorGenerator.TerrainType { name = "Water", height = 0.2f, color = Color.blue},
    new MapColorGenerator.TerrainType { name = "WaterLight", height = 0.3f, color = Color.lightSkyBlue },
    new MapColorGenerator.TerrainType { name = "Sand", height = 0.4f, color = new Color(0.9f, 0.8f, 0.6f) },
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



        MeshFilter mf = this.AddComponent <MeshFilter>();
        MeshRenderer mr = this.AddComponent <MeshRenderer>();

        Material mat = new Material(Shader.Find("Unlit/Texture"));
        //mr.material = mat;

        float[,] noiseMap = NoiseMapGenerator.GenerateNoiseMap(SizeOfGrid, SizeOfGrid, 10,1,5,1,0,Vector2.zero);
        MeshData md = MeshGenerator.GenerateTerrain(noiseMap, heightMultiplier, animationCurve, levelOfDetails);
        mf.sharedMesh = md.CreateMesh();

        //mat.mainTexture = ProcGenTool.RenderNoiseAsGrayTexture(noiseMap);
        Color[] colourMap = MapColorGenerator.GenerateColorMap(noiseMap, regions, SizeOfGrid);

        // Create texture and apply to renderer
        mat.mainTexture = MapColorGenerator.GenerateTexture(colourMap, SizeOfGrid);
        //mat.mainTexture = MapColorGenerator.GenerateColorMap(noiseMap, regions, SizeOfGrid);
        mr.material = mat;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
