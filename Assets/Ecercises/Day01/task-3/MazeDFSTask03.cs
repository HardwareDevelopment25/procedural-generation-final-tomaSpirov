//Finding the Furthest Tile Using Breadth-First Search (BFS)
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MazeDFSTask03 : MonoBehaviour
{
    private System.Random rand;
    private bool[,] maze;

    [Header("Maze Prefabs")]
    //attach wall and floor prefabs in inspector
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject goalPrefab;
    public GameObject playerPrefab;
    public GameObject trackPathPrefab;
    [Header("Maze Settings")]
    public int MazeSize;

    void Start()
    {
        maze = GenerateMaze(MazeSize);
        DrawMaze(maze);

        Vector2Int start = new Vector2Int(0, 0);
        Vector2Int end = FindFurthestTile(start, maze);

        //add at the furthest position a preafabGoal to mark it on the maze
        GameObject.Instantiate(goalPrefab, new Vector3(end.x, 0, end.y), Quaternion.identity, transform);
        GameObject.Instantiate(playerPrefab, new Vector3(start.x, 0f, start.y), Quaternion.identity, transform);
    }


    bool[,] GenerateMaze(int size)
    {
        bool[,] maze = new bool[size, size];
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        Vector2Int currentPos = new Vector2Int(0, 0);
        maze[currentPos.x, currentPos.y] = true;
        stack.Push(currentPos);

        while (stack.Count > 0)
        {
            currentPos = stack.Pop();
            List<Vector2Int> unvisitedNeighbors = new List<Vector2Int>();

            if (currentPos.x > 1 && !maze[currentPos.x - 2, currentPos.y])
                unvisitedNeighbors.Add(new Vector2Int(currentPos.x - 2, currentPos.y));
            if (currentPos.x < size - 2 && !maze[currentPos.x + 2, currentPos.y])
                unvisitedNeighbors.Add(new Vector2Int(currentPos.x + 2, currentPos.y));
            if (currentPos.y > 1 && !maze[currentPos.x, currentPos.y - 2])
                unvisitedNeighbors.Add(new Vector2Int(currentPos.x, currentPos.y - 2));
            if (currentPos.y < size - 2 && !maze[currentPos.x, currentPos.y + 2])
                unvisitedNeighbors.Add(new Vector2Int(currentPos.x, currentPos.y + 2));

            if (unvisitedNeighbors.Count > 0)
            {
                stack.Push(currentPos);
                Vector2Int chosenNeighbor = unvisitedNeighbors[Random.Range(0, unvisitedNeighbors.Count)];
                maze[(currentPos.x + chosenNeighbor.x) / 2, (currentPos.y + chosenNeighbor.y) / 2] = true; //mark the wall between as path
                maze[chosenNeighbor.x, chosenNeighbor.y] = true;
                stack.Push(chosenNeighbor);
            }
        }

        return maze;
    }

    void DrawMaze(bool[,] maze)
    {
        int width = maze.GetLength(0);
        int height = maze.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (maze[x, y])
                {
                    GameObject.Instantiate(floorPrefab, new Vector3(x, 0, y), Quaternion.identity, transform);
                }
                else
                {
                    GameObject.Instantiate(wallPrefab, new Vector3(x, 0, y), Quaternion.identity, transform);
                }
            }
        }
    }

    //1.define the FindFurthestTile method
    public Vector2Int FindFurthestTile(Vector2Int startPos, bool[,] maze)
    {
        int width = maze.GetLength(0);
        int height = maze.GetLength(1);

        //2. initialize a BFS structure.
        //Initialize a queue for BFS and enqueue the starting tile.
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(startPos);
        //initialize a variable to keep track of the furthest position, initially set to startPos
        Vector2Int furthestPos = startPos;
        //create a 2d array to keep track of visited positions
        bool[,] visited = new bool[width, height];

        //3. create a BFS loop until all reachable tiles have been visited
        while (queue.Count > 0)
        {
            //dequeue the front position from the queue
            Vector2Int currentPos = queue.Dequeue();
            //update furthest position
            furthestPos = currentPos;

            //5.Get Valid Neighbors
            foreach (Vector2Int neighbor in GetNeighbors(currentPos, maze))
            {
                //check if neighbor is a path and not visited
                if (!visited[neighbor.x, neighbor.y] && maze[neighbor.x, neighbor.y])
                {
                    //mark neighbor as visited
                    visited[neighbor.x, neighbor.y] = true;
                    
                    //enqueue the neighbor position
                    queue.Enqueue(neighbor);
                }
            }

        }
        Debug.Log("Furthest Position: " + furthestPos);

        return furthestPos;
    }

    List<Vector2Int> GetNeighbors(Vector2Int currCellPos, bool[,] maze)
    {
        List<Vector2Int> neighborsList = new List<Vector2Int>();
        int width = maze.GetLength(0);
        int height = maze.GetLength(1);



        //check all 4 possible directions (up, down, left, right)
        Vector2Int[] directions = new Vector2Int[]
        {
                new Vector2Int(1, 0), //right
                new Vector2Int(-1, 0), //left
                new Vector2Int(0, 1), //up
                new Vector2Int(0, -1) //down
        };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int neighborPos = currCellPos + dir;
            //check if neighbor is within bounds and is a path and not visited
            if (neighborPos.x >= 0 && neighborPos.x < width &&
                neighborPos.y >= 0 && neighborPos.y < height)
            {
                neighborsList.Add(neighborPos);
            }
        }

        return neighborsList;
    }

}