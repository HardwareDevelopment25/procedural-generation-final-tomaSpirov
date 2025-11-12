using System;
using UnityEngine;

public static class MeshGenerator
{
	public static MeshData GenerateTerrain(float[,] heightMap, float heightMultiplier, AnimationCurve aC, int levelOfDetail) 
	{
		int height = heightMap.GetLength(0);
		int width = heightMap.GetLength(1);

		float topLeftX = (width - 1) / -2f;
		float topLeftZ = (height - 1) / 2f;

		int simplificationIncrement = 3;//= (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
		int verticesPerline = (width-1)/simplificationIncrement;//subtrak one from the edge

		MeshData meshData = new MeshData(verticesPerline, height);
		int vertexINdex = 0;

		for (int y = 0; y < height; y += simplificationIncrement) 
		{
			for (int x = 0; x < width; x+= simplificationIncrement) 
			{
				
				meshData.vertices[vertexINdex]= new Vector3(topLeftX+x, aC.Evaluate(heightMap[x, y])*heightMultiplier, topLeftZ-y);

				meshData.uvs[vertexINdex] = new Vector2(x / (float)width, y / (float)width);

				//finally lets create those triangle faces
				if (x < width - 1 && y < height - 1) //we are making a square
				{
					// i                 i+W=1              i+1
					meshData.AddTriangle(vertexINdex, vertexINdex+ verticesPerline + 1, vertexINdex+ verticesPerline);
					//i+w+1,   i ,   i+1
					meshData.AddTriangle(vertexINdex+ verticesPerline + 1, vertexINdex, vertexINdex+1);

				}
				vertexINdex++;


			}
		}
		return meshData;
	}
}


public class MeshData
{
	public Vector3[] vertices;
	public int[] triangles;

	public  int trianglesIndex = 0;
	public Vector2[] uvs;

	public MeshData(int meshWidth, int meshHeight)
	{ 
		vertices = new Vector3[meshWidth * meshHeight];

		// i dont want to fall off the grid into invalid space
		triangles = new int[(meshWidth * meshHeight *6)];// (128*128*6 = 98304) correct 
        uvs = new Vector2[meshWidth*meshHeight];
	}
	public void AddTriangle(int a,int b, int c) 
	{
		triangles[trianglesIndex] = a;
		triangles[trianglesIndex+1] = b;
		triangles[trianglesIndex+2] = c;
		trianglesIndex += 3;
	}

	public  Mesh CreateMesh() 
	{
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;	
		mesh.uv = uvs;
		mesh.RecalculateNormals();
		//mesh.trianglesIndex = trianglesIndex;
		return mesh;
	}


}