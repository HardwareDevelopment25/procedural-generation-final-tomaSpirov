//using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using TreeEditor;
using static UnityEngine.Rendering.DebugUI;


public class TextureLogicalAnd : MonoBehaviour
{
    public int imageSize = 64;
    public Texture2D texture;

    private System.Random rnd;
    public float scaler = 0.1f;
    private float lastScaler = 0f;

    private void Start()
    {

        texture = new Texture2D(imageSize, imageSize);
        //createPattern();
        ///createPatternByRandPos();
        createPatternByNoise();
        GetComponent<MeshRenderer>().material.mainTexture = texture;

    }

    private void Update()
    {
      

        if(scaler!=lastScaler)
            createPatternByNoise();
        
        /*if (Input.GetKeyDown(KeyCode.A))
        {
            createPatternByNoise();
            Debug.Log("pressed button A");
        }*/
    }

    public void createPattern() 
    {
        //go through each pixel in this texture
        for (int y = 0; y < imageSize; y++) 
        {
            for (int x = 0; x < imageSize; x++)
            {
                //var r = rnd.Next(256);
                //Color randomColor = Color.HSVToRGB(r, r, r);
                Color pixelColor = ((x&y)!=0 ? Color.black : Color.white);
                texture.SetPixel(x, y, pixelColor);

            }
        }
        texture.Apply();
    }
    public void createPatternByRandPos()
    {
        //go through each pixel in this texture
        for (int y = 0; y < imageSize; y++)
        {
            for (int x = 0; x < imageSize; x++)
            {
                float value = rnd.Next(1);
                //Color randomColor = Color.HSVToRGB(r, r, r);
                Color pixelColor = (value>0.5f ? Color.black : Color.white);
                texture.SetPixel(x, y, pixelColor);

            }
        }
        texture.Apply();
    }

    public void createPatternByNoise()
    {
        if (scaler < 0) scaler = 0.000001f;

        lastScaler = scaler;//need it for update

            //go through each pixel in this texture
            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {

                    float sampleX = (float)x / texture.width / scaler;
                    float sampleY = (float)y / texture.height / scaler;

                    float perlinResult = Mathf.PerlinNoise(sampleX, sampleY);
                    Color pixelColor = Color.Lerp(Color.black, Color.white, perlinResult);
                    texture.SetPixel(x, y, pixelColor);

                }
            }
        texture.Apply();
    }
}
