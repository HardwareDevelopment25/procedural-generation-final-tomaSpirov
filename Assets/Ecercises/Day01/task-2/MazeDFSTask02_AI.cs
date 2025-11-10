    using System.Collections.Generic;
using UnityEngine;

public class MazeDFSTask02_AI : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public float cellSize = 1f;//this is the size of each cell in the maze

    public int mazeSize = 21;

    void Start()
    {
        bool[,] maze = GenerateMaze(mazeSize);
        DrawMaze(maze);
    }

    public static bool[,] GenerateMaze(int size)
    {
        bool[,] maze = new bool[size, size];
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        Vector2Int current = new Vector2Int(0, 0);
        maze[current.x, current.y] = true;
        stack.Push(current);

        while (stack.Count > 0)
        {
            current = stack.Pop();
            List<Vector2Int> neighbors = new List<Vector2Int>();

            if (current.x > 1 && !maze[current.x - 2, current.y])
                neighbors.Add(new Vector2Int(current.x - 2, current.y));
            if (current.x < size - 2 && !maze[current.x + 2, current.y])
                neighbors.Add(new Vector2Int(current.x + 2, current.y));
            if (current.y < size - 2 && !maze[current.x, current.y + 2])
                neighbors.Add(new Vector2Int(current.x, current.y + 2));
            if (current.y > 1 && !maze[current.x, current.y - 2])
                neighbors.Add(new Vector2Int(current.x, current.y - 2));

            if (neighbors.Count > 0)
            {
                stack.Push(current);
                Vector2Int chosen = neighbors[Random.Range(0, neighbors.Count)];
                Vector2Int wall = new Vector2Int((current.x + chosen.x) / 2, (current.y + chosen.y) / 2);
                maze[wall.x, wall.y] = true;
                maze[chosen.x, chosen.y] = true;
                stack.Push(chosen);
            }
        }

        return maze;
    }

    public void DrawMaze(bool[,] maze)
    {
        int width = maze.GetLength(0);
        int height = maze.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x * cellSize, 0, y * cellSize);
                GameObject prefabToInstantiate = maze[x, y] ? floorPrefab : wallPrefab;
                Instantiate(prefabToInstantiate, position, Quaternion.identity, transform);
            }
        }
    }
}