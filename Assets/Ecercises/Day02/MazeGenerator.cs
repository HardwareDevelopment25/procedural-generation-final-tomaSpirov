using UnityEngine;
using System.Drawing;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour
{

    System.Random m_Random;
    public int size = 10, seed = 0;
    public GameObject wall, floor;
    private bool[,] maze;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_Random     = new System.Random(seed);
        maze = new bool[size, size];

        GenerateMaze();
        DrawMaze();

    }


    private void GenerateMaze() 
    {
        //first we need a stack  to store all unvisited locations
        Stack<Vector2Int> stack = new Stack<Vector2Int>();

        //start maze generation from top left
        Vector2Int current = new Vector2Int(0,0);

        //mark the starting cell as part of the maze
        maze[current.x, current.y] = true;

        //add start pos to the stack
        stack.Push(current);


        while (stack.Count>0)//stuff exist on the stackj
        {
        current = stack.Pop();//takes the top position to be checked
        
            //malke list of uo to 4 neighbors
            List<Vector2Int> neighbors = new List<Vector2Int>();

            if(current.x > 1 && !maze[current.x-2, current.y]) //is left available
                neighbors.Add(new Vector2Int(current.x - 2, current.y));

            if (current.x < size-2 && !maze[current.x + 2, current.y]) //is right available
                neighbors.Add(new Vector2Int(current.x + 2, current.y));

            if (current.y > 1 && !maze[current.x, current.y-2]) //is up available
                neighbors.Add(new Vector2Int(current.x, current.y-2));

            if (current.y < size-2 && !maze[current.x, current.y+2]) //is down available
                neighbors.Add(new Vector2Int(current.x, current.y+2));

            //4.after CHECK next choice a neighbour
            //choice a neighbour and add it to the stack
            if (neighbors.Count > 0)//did we add anything to the list
            {
                stack.Push(current);//puts back what we poped of the stack

                //choice a neighbor, but make sure you check its count as may differ
                Vector2Int choisenOne = neighbors[m_Random.Next(0,neighbors.Count)];

                if (choisenOne.x == current.x)
                {
                    maze[choisenOne.x, current.y + 1] = true;//mark gap as 'true' make vertically bridge (connection to endpoint)

                }
                else 
                {
                    maze[choisenOne.x+1, current.y] = true; ;//mark gap as 'true' make horizontally bridge (connection to endpoint)

                }

            }

        }


    }


    void DrawMaze()
    {
        //loop 
        for (int x = 0; x < size; x++) 
        {
            for (int y = 0; y < size; y++) 
            {
                if (maze[x, y])
                {
                    Instantiate(floor, new Vector3(x, 0f, y), Quaternion.identity, transform);
                }
                else 
                {
                    Instantiate(wall, new Vector3(x, 0f, y), Quaternion.identity, transform);
                }
            }
        }
    }

}


