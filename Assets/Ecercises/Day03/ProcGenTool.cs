using UnityEngine;

public static class ProcGenTool
{
    public static Texture2D ConvertBoolArrayAsIntTexture(int[,] maze)
    {

        Texture2D texture2D = new Texture2D(maze.GetLength(0), maze.GetLength(1));

        for (int y = 0; y < maze.GetLength(0); y++)
        {
            for (int x = 0; x < maze.GetLength(1); x++)
            {
                if (maze[x, y] == 1)
                {
                    texture2D.SetPixel(x, y, Color.white);
                }
                else
                {
                    texture2D.SetPixel(x, y, Color.black);
                }
            }
        }

        texture2D.Apply();
        texture2D.filterMode = FilterMode.Point;
        texture2D.wrapMode = TextureWrapMode.Clamp;

        return texture2D;
    }
    public static Texture2D RenderBoolArrayAsTexture(bool[,] maze)
    {

        Texture2D texture2D = new Texture2D(maze.GetLength(0), maze.GetLength(1));

        for (int y = 0; y < maze.GetLength(0); y++)
        {
            for (int x = 0; x < maze.GetLength(1); x++)
            {
                if (maze[x, y])
                {
                    texture2D.SetPixel(x, y, Color.white);
                }
                else
                {
                    texture2D.SetPixel(x, y, Color.black);
                }
            }
        }

        texture2D.Apply();
        texture2D.filterMode = FilterMode.Point;
        texture2D.wrapMode = TextureWrapMode.Clamp;

        return texture2D;
    }
    public static Texture2D RenderNoiseAsColorTexture(float[,] maze)
    {
        Texture2D texture2D = new Texture2D(maze.GetLength(0), maze.GetLength(1));
        for (int x = 0; x < maze.GetLength(0); x++)
        {
            for (int y = 0; y < maze.GetLength(1); y++)
            {
                Color gradiant = new Color(maze[x, y], maze[x, y], maze[x, y]);
                texture2D.SetPixel(x, y, gradiant);
            }
        }
        texture2D.Apply();
        texture2D.filterMode = FilterMode.Point;
        texture2D.wrapMode = TextureWrapMode.Clamp;
        return texture2D;

    }
    public static Texture2D RenderNoiseAsGrayTexture(float[,] maze)
    {
        Texture2D texture2D = new Texture2D(maze.GetLength(0), maze.GetLength(1));
        for (int x = 0; x < maze.GetLength(0); x++)
        {
            for (int y = 0; y < maze.GetLength(1); y++)
            {
                Color gradiant = new Color(maze[x, y], maze[x, y], maze[x, y]);
                texture2D.SetPixel(x, y, gradiant);
            }
        }
        texture2D.Apply();
        texture2D.filterMode = FilterMode.Point;
        texture2D.wrapMode = TextureWrapMode.Clamp;
        return texture2D;

    }

    public static Mesh makeTriangle(float SizeOfTriangle)
    {
        Mesh triangle = new Mesh();

        Vector3[] vertices = new Vector3[]
        {
            new Vector3 (0,0,0),
            new Vector3 (SizeOfTriangle,0,0),
            new Vector3 (SizeOfTriangle/2,SizeOfTriangle,0)
        };

        Vector2[] uvs = new Vector2[]
        {
            new Vector2(0,0),
            new Vector2 (SizeOfTriangle,0),
            new Vector2 (SizeOfTriangle/2,SizeOfTriangle),
        };

        int[] triangles = new int[]
        {
            0,1,2
        };



        triangle.vertices = vertices;
        triangle.uv = uvs;
        triangle.triangles = triangles;

        return triangle;
    }


    public static Mesh makeSquare(float SizeOfSquare)
    {
        Mesh square = new Mesh();

        Vector3[] vertices = new Vector3[]
        {
            //front
            new Vector3 (0,0,0),
            new Vector3 (SizeOfSquare,0,0),
            new Vector3 (SizeOfSquare,SizeOfSquare,0),
            new Vector3 (0,SizeOfSquare,0),
             //right
            new Vector3 (0,0,SizeOfSquare),
            new Vector3 (SizeOfSquare,0,SizeOfSquare),
            new Vector3 (SizeOfSquare,SizeOfSquare,SizeOfSquare),
            new Vector3 (0,SizeOfSquare,SizeOfSquare),
             //back
          /*  new Vector3 (0,0,0),
            new Vector3 (SizeOfSquare,0,0),
            new Vector3 (SizeOfSquare,SizeOfSquare,0),
            new Vector3 (0,SizeOfSquare,0),
             //left
            new Vector3 (0,0,0),
            new Vector3 (SizeOfSquare,0,0),
            new Vector3 (SizeOfSquare,SizeOfSquare,0),
            new Vector3 (0,SizeOfSquare,0),
*/


        };

        Vector2[] uvs = new Vector2[]
        {
            //front
            new Vector2(0,0),
            new Vector2 (SizeOfSquare,0),
            new Vector2 (SizeOfSquare,SizeOfSquare),
            new Vector2 (0,SizeOfSquare),
             //right
            new Vector2(0,0),
            new Vector2 (SizeOfSquare,0),
            new Vector2 (SizeOfSquare,SizeOfSquare),
            new Vector2 (0,SizeOfSquare),
             //back
            new Vector2(0,0),
            new Vector2 (SizeOfSquare,0),
            new Vector2 (SizeOfSquare,SizeOfSquare),
            new Vector2 (0,SizeOfSquare),
             //left
            new Vector2(0,0),
            new Vector2 (SizeOfSquare,0),
            new Vector2 (SizeOfSquare,SizeOfSquare),
            new Vector2 (0,SizeOfSquare),
        };

        int[] triangles = new int[]
        {
            0,1,2,0,2,3,//front
            3,2,4,3,4,5//right
           // 5,4,6,5,6,7,//back
            //7,6,1,7,1,0//left
        };



        square.vertices = vertices;
        square.uv = uvs;
        square.triangles = triangles;


        return square;
    }

    public static int[,] BorderMe(int[,] gridBorder)
    {
        for (int x = 0; x < gridBorder.GetLength(0); x++)
        {
            for (int y = 0; y < gridBorder.GetLength(1); y++)
            {
                //all 0 and heights /widths are set to walls
                if (x == 0 || x == (gridBorder.GetLength(0) - 1) || y == 0 || y == (gridBorder.GetLength(1) - 1))
                { gridBorder[x, y] = 1; }
            }
        }
        return gridBorder;
    }

    public static int checkFortNeighbours(int x, int y, int[,] grid) 
    {
        int countNeighbour = 0;
        for (int currX = x - 1; currX <= x + 1; currX++)
        {
            for (int currY = y - 1; currY < y + 1; currY++)
            {
                if(grid[currX, currY] == 1)
                    { countNeighbour++; }
            }

        }
            


        return countNeighbour;

    }
}
