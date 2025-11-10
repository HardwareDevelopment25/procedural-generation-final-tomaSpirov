using System;
using UnityEngine;

public static class MeshGenerator
{
	public static MeshData GenerateTerrain(float[,] heightMap, float heightMultiplier) 
	{
		int height = heightMap.GetLength(0);
		int width = heightMap.GetLength(1);

		float topLeftX = (width - 1) / -2f;
		float topLeftZ = (height - 1) / 2f;
		float heighValue = 0f;
		MeshData meshData = new MeshData(width, height);
		int vertexINdex = 0;

		for (int y = 0; y < height; y++) 
		{
			for (int x = 0; x < width; x++) 
			{
				
				meshData.vertices[vertexINdex]= new Vector3(topLeftX+x, heightMap[x, y]*heightMultiplier, topLeftZ-y);

				meshData.uvs[vertexINdex] = new Vector2(x / (float)width, y / (float)width);

				//finally lets create those triangle faces
				if (x < width - 1 && y < height - 1) //we are making a square
				{
					meshData.AddTriangle(vertexINdex, vertexINdex+width+1, vertexINdex+width);
					meshData.AddTriangle(vertexINdex+width+1, vertexINdex, vertexINdex+1);

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
		triangles = new int[(meshWidth-1) * (meshHeight-1)*6];
		uvs = new Vector2[meshWidth*meshHeight];
	}
	public void AddTriangle(int a,int b, int c) 
	{
		triangles[trianglesIndex] = a;
		triangles[trianglesIndex+1] = b;
		triangles[trianglesIndex+2] = c;
		trianglesIndex += 2;
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