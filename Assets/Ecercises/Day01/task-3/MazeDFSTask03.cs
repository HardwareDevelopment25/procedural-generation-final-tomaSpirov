using System.Collections.Generic;
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
    [Header("Maze Settings")]
    public int MazeSize = 10;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maze = GenerateMaze(MazeSize);
        DrawMaze();

        //find furthest tile from start (0,0)
        Vector2Int start = new Vector2Int(0, 0);
        Vector2Int end = FindFurthestTile(start, maze);
        //add at the furthest position a preafabGoal to mark it on the maze
        GameObject.Instantiate(goalPrefab, new Vector3(end.x, 0, end.y), Quaternion.identity, transform);
        GameObject.Instantiate(playerPrefab, new Vector3(start.x, -0.5f, start.y), Quaternion.identity, transform);
    }

    //step 1: define GenerateMaze method
    bool[,] GenerateMaze(int size) 
    {
        //step 2: create a 2D boolean array to represent the maze with dimensions size x size
        bool[,] maze = new bool[size,size]; 

        //step 3: initialize a stack vector2Int to implement DFS
        Stack<Vector2Int> stack = new Stack<Vector2Int>();


        //step 4: set starting position at (0,0) and mark it as visited
        Vector2Int currentPos = new Vector2Int(0, 0);

        //step 5: set the starting position as a path (true) in the maze
        maze[currentPos.x, currentPos.y] = true;

        //step 6: push the starting position onto the stack
        stack.Push(currentPos);

        //step 7: loop while the stack is not empty(until all cells have been visited) to generate the maze path using DFS algorithm
        while (stack.Count > 0) 
        {
            //step 8: pop the top position/cell from the stack to make it the current position/cell
            currentPos = stack.Pop();//get the last position added to the stack and remove it from the stack

            //step 9: get all unvisited neighbors of the current position/cell
            List<Vector2Int> unvisitedNeighbors = new List<Vector2Int>();

            //check left neighbor with the 2 steps to the left ensure it's within bounds and unvisited
            if (currentPos.x > 1 && !maze[currentPos.x - 2, currentPos.y])//start from 0 to size-1 and that we looking for x>1 to avoid out of bounds(1-2 = -1 out)
            {
                unvisitedNeighbors.Add(new Vector2Int(currentPos.x - 2, currentPos.y));
            }
            
            //check left neighbor with the 2 steps to the ri ensure it's within bounds and unvisited
            if (currentPos.x < size-2 && !maze[currentPos.x + 2, currentPos.y])//start from 0 to size-2 and that we looking for x<sizw-2 to avoid out of bounds(30+2 = 32 out)
            {
                unvisitedNeighbors.Add(new Vector2Int(currentPos.x + 2, currentPos.y));
            }

            //check top neighbor with the 2 steps up ensure it's within bounds and unvisited
            if (currentPos.y > 1 && !maze[currentPos.x, currentPos.y - 2])//start from 0 to size-1 and that we looking for y>1 to avoid out of bounds(1-2 = -1 out)
            {
                unvisitedNeighbors.Add(new Vector2Int(currentPos.x, currentPos.y - 2));
            }

            //check bottom neighbor with the 2 steps down ensure it's within bounds and unvisited
            if (currentPos.y < size - 2 && !maze[currentPos.x, currentPos.y + 2])//start from 0 to size-2 and that we looking for y<size-2 to avoid out of bounds(30+2 = 32 out)
            {
                unvisitedNeighbors.Add(new Vector2Int(currentPos.x, currentPos.y + 2));
            }


            //step 10: carve the path if there are unvisited neighbors
            if (unvisitedNeighbors.Count > 0) 
            {
                //push the current position back onto the stack to revisit later
                stack.Push(currentPos);
                rand = new System.Random();

                //randomly select one of the unvisited neighbors to move to
                Vector2Int chosenNeighbor = unvisitedNeighbors[rand.Next(0, unvisitedNeighbors.Count)];

                //carve a path between the current position and the chosen neighbor
                
                // maze[(currentPos.x + chosenNeighbor.x) / 2, (currentPos.y + chosenNeighbor.y) / 2] = true; //mark the wall between as path
                
                if (chosenNeighbor.x == currentPos.x)
                {
                    //same column, so it's a vertical move
                    maze[chosenNeighbor.x, chosenNeighbor.y+1] = true; //mark the chosen neighbor as path
                }
                else 
                {
                    //same row, so it's a horizontal move
                    maze[chosenNeighbor.x + 1, chosenNeighbor.y] = true; //mark the chosen neighbor as path
                }
                //mark the chosen neighbor as part of the maze (i.e., a path)
                maze[chosenNeighbor.x, chosenNeighbor.y] = true;
                //push the chosen neighbor onto the stack to continue the DFS from there
                stack.Push(chosenNeighbor);

            }

        }



        return maze;
    }

    void DrawMaze()
    {
        for (int x = 0; x < MazeSize; x++)
        {
            for (int y = 0; y < MazeSize; y++)
            {
                if (maze[x, y])
                {
                    //draw floor
                    //use transform as parent to keep hierarchy clean
                    GameObject.Instantiate(floorPrefab, new Vector3(x, 0, y), Quaternion.identity, transform);
                }
                else
                {
                    //draw wall
                    //use transform as parent to keep hierarchy clean
                    GameObject.Instantiate(wallPrefab, new Vector3(x, 0, y), Quaternion.identity, transform);
                }
            }
        }
    }

    public Vector2Int FindFurthestTile(Vector2Int startPos, bool[,] maze)
    {
        //optional: implement a method to find the furthest tile from the start using BFS

        //initialize a BFS structure.
        //initialize a queue with starting position
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(startPos);

        //initialize a variable to keep track of the furthest position, initially set to startPos
        Vector2Int furthestPos = startPos;

        //create a 2d array to keep track of visited positions
        bool[,] visited = new bool[MazeSize, MazeSize];

        //create a BFS loop until all reachable tiles have been visited
        while (queue.Count > 0)
        {
            //dequeue the front position from the queue
            Vector2Int currentPos = queue.Dequeue();
            //update furthest position
            furthestPos = currentPos;

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
                Vector2Int neighborPos = currentPos + dir;
                //check if neighbor is within bounds and is a path and not visited
                if (neighborPos.x >= 0 && neighborPos.x < MazeSize &&
                    neighborPos.y >= 0 && neighborPos.y < MazeSize &&
                    maze[(int)neighborPos.x, (int)neighborPos.y] &&
                    !visited[(int)neighborPos.x, (int)neighborPos.y])
                {
                    //mark neighbor as visited
                    visited[(int)neighborPos.x, (int)neighborPos.y] = true;
                    //enqueue neighbor position
                    queue.Enqueue(neighborPos);
                }
            }
        }
        Debug.Log("Furthest Position: " + furthestPos);

        return furthestPos;
    }


}
