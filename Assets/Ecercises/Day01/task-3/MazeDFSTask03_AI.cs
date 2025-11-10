using System.Collections.Generic;
using UnityEngine;

public class MazeDFSTask03_AI : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject goalPrefab;
    public GameObject playerPrefab;
    public float cellSize = 1f;
    public int mazeSize = 21;

    void Start()
    {
        bool[,] maze = GenerateMaze(mazeSize);
        DrawMaze(maze);

        Vector2Int start = new Vector2Int(0, 0);
        Vector2Int end = FindFurthestTile(start, maze);
        Debug.Log($"Furthest tile from start is at: {end}");

        Instantiate(goalPrefab, new Vector3(end.x * cellSize, 0, end.y * cellSize), Quaternion.identity, transform);
        Instantiate(playerPrefab, new Vector3(start.x * cellSize, 0, start.y * cellSize), Quaternion.identity, transform);

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

    public static Vector2Int FindFurthestTile(Vector2Int start, bool[,] maze)
    {
        int width = maze.GetLength(0);
        int height = maze.GetLength(1);
        bool[,] visited = new bool[width, height];
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Vector2Int furthest = start;

        queue.Enqueue(start);
        visited[start.x, start.y] = true;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            furthest = current;

            foreach (Vector2Int neighbor in GetNeighbors(current, maze))
            {
                if (!visited[neighbor.x, neighbor.y] && maze[neighbor.x, neighbor.y])
                {
                    visited[neighbor.x, neighbor.y] = true;
                    queue.Enqueue(neighbor);
                }
            }
        }

        return furthest;
    }

    public static List<Vector2Int> GetNeighbors(Vector2Int cell, bool[,] maze)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        int width = maze.GetLength(0);
        int height = maze.GetLength(1);

        Vector2Int[] directions = {
            new Vector2Int(-1, 0), // left
            new Vector2Int(1, 0),  // right
            new Vector2Int(0, -1), // down
            new Vector2Int(0, 1)   // up
        };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int neighbor = cell + dir;
            if (neighbor.x >= 0 && neighbor.x < width && neighbor.y >= 0 && neighbor.y < height)
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }
}
