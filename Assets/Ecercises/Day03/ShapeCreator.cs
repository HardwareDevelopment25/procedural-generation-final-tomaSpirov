using Unity.VisualScripting;
using UnityEngine;

public class ShapeCreator : MonoBehaviour
{
    public int SizeOfGrid = 128;
    public float sizeOfShape = 1.0f;
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
        mr.material = mat;

        float[,] noiseMap = NoiseMapGenerator.GenerateNoiseMap(SizeOfGrid, SizeOfGrid, 10,1,5,1,0,Vector2.zero);
        MeshData md = MeshGenerator.GenerateTerrain(noiseMap, 10f);
        mf.sharedMesh = md.CreateMesh();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
