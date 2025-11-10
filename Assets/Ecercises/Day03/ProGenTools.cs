using UnityEngine;

public static class ProGenTools
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public static Texture2D RenderBoolArrayAsTexture(bool[,] maze) 
    {

        Texture2D texture2D = new Texture2D(maze.GetLength(0), maze.GetLength(1));

        for (int y = 0; y < maze.GetLength(0); y++) 
        {
            for (int x = 0; x < maze.GetLength(1); x++) 
            {
                if (maze[x, y])
                {
                    texture2D.SetPixel(x,y, Color.white);
                }
                else
                {
                    texture2D.SetPixel(x,y, Color.black);
                }
            }
        }

        texture2D.Apply();
        texture2D.filterMode = FilterMode.Point;
        texture2D.wrapMode = TextureWrapMode.Clamp;

        return texture2D;
    }
}
