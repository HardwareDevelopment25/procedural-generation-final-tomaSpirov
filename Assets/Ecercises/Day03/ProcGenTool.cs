using UnityEngine;

public static class ProcGenTool
{
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
            3,2,4,3,4,5,//right
            5,4,6,5,6,7,//back
            7,6,1,7,1,0//left
        };



        square.vertices = vertices;
        square.uv = uvs;
        square.triangles = triangles;


        return square;
    }
}
