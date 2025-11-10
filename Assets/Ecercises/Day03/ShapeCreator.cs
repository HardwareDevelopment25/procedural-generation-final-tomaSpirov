using Unity.VisualScripting;
using UnityEngine;

public class ShapeCreator : MonoBehaviour
{

    public float sizeOfShape = 1.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MeshFilter mf = this.AddComponent <MeshFilter>();
        MeshRenderer mr = this.AddComponent <MeshRenderer>();

        /*Material mat = new Material(Shader.Find("Unlit/Texture"));
        mf.mesh = ProcGenTool.makeTriangle(sizeOfShape);
        mr.material = mat;*/
        Material mat = new Material(Shader.Find("Unlit/Texture"));
        mf.mesh = ProcGenTool.makeSquare(sizeOfShape);
        mr.material = mat;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
