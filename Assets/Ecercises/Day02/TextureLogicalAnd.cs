//using System.Drawing;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;


public class TextureLogicalAnd : MonoBehaviour
{
    public int imageSize = 64;
    private System.Random rnd;
    public float scaler = 0.1f;
    public int seed = 0, lac = 1, oct = 4;
    System.Random rnd2;
    public Vector2 offsetsPos = Vector2.zero;
   // private float lastScaler = 0f;
    public  Color whatColor_0 = Color.pink;
    public  Color whatColor_1 = Color.pink;
    public  Color whatColor_2 = Color.pink;
    public  Color whatColor_3 = Color.pink;
    public  Color whatColor_4 = Color.pink;
    public  Color whatColor_5 = Color.pink;

    public Color[] colors;
    
    [SerializeField]
    public AnimationCurve animCurve = new AnimationCurve();

    private void Awake()
    {
        colors = new Color[6];
        colors[0] = whatColor_0;
        colors[1] = whatColor_1;
        colors[2] = whatColor_2;
        colors[3] = whatColor_3;
        colors[4] = whatColor_4;
    }

    private void Start()
    {


        offsetsPos.x += 0.1f;
        scaler += 0.1f;


        rnd2 = new System.Random(seed);

        float[,] map = NoiseMapGenerator.GenerateNoiseMap(imageSize, imageSize, scaler, lac, oct, 1, rnd2.Next(), offsetsPos);
        float[,] falloffmap = NoiseMapGenerator.GenerateFallOffMap(imageSize, animCurve);
        GetComponent<MeshRenderer>().material.mainTexture = DrawColorMapToTexture(map);

    }

    private void Update()
    {
        Start();
    }
    public Texture2D DrawGreyScaleMapToTexture(float[,] mapToDraw)
    {
        Texture2D texture = new Texture2D(mapToDraw.GetLength(0), mapToDraw.GetLength(1));
        //go through each pixel in this texture
        for (int i = 0; i < mapToDraw.GetLength(0); i++)
        {
            for (int j = 0; j < mapToDraw.GetLength(1); j++)
            {
                float c = mapToDraw[i, j];
                texture.SetPixel(i, j, new Color(c,c,c));
            }
        }
        texture.Apply();
        return texture;
    }

    public Texture2D DrawColorMapToTexture(float[,] mapToDraw)
    {
       
        Texture2D texture = new Texture2D(mapToDraw.GetLength(0), mapToDraw.GetLength(1));
        //go through each pixel in this texture
        
            
            for (int i = 0; i < mapToDraw.GetLength(0); i++)
            {
            for (int j = 0; j < mapToDraw.GetLength(1); j++)
            {

                if (mapToDraw[i, j] < .2f) texture.SetPixel(i, j, colors[0]);
                else if (mapToDraw[i, j] < .4f && mapToDraw[i, j] > .2f) texture.SetPixel(i, j, colors[1]);
                else if (mapToDraw[i, j] < .6f && mapToDraw[i, j] > .4f) texture.SetPixel(i, j, colors[2]);
                else if (mapToDraw[i, j] < .8f && mapToDraw[i, j] > .6f) texture.SetPixel(i, j, colors[3]);
                else if (mapToDraw[i, j] < .9f && mapToDraw[i, j] > .8f) texture.SetPixel(i, j, colors[4]);
                else texture.SetPixel(i, j, Color.black);
                // Here you choose color by Variable in c
            }

        }
        texture.Apply();
        return texture;


    }
}
