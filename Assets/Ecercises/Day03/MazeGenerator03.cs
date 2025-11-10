using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class MazeGenerator03 : MonoBehaviour
{
    System.Random m_Random;
    public int size = 10, seed = 0;
    public GameObject wall, floor;
    private bool[,] maze;
    public bool is2DOnly = false;

    void Start()
    {
        m_Random = new System.Random(seed);
        maze = new bool[size, size];// make a new maze of falses

        GenerateMaze();
        MeshRenderer mr;
        if (is2DOnly)
        {
            mr = GetComponent<MeshRenderer>();
           // mr.material.mainTexture = ProGenTools.RenderBoolArrayAsTexture(maze);
            mr.sharedMaterial.mainTexture = ProGenTools.RenderBoolArrayAsTexture(maze);

        }
        else
        {
            DrawMazeIn3D();
        }




    }
    public void GenerateImageOfNewMaze()
    {
        // just incase 3D generation was on, I dont want thousands of mazes stacked.
        is2DOnly = true;
        Start();
    }


    private void GenerateMaze()
    {

        // first we need a stack to store all unvisited locations
        Stack<Vector2Int> stack = new Stack<Vector2Int>();

        // start maze generation from top left
        Vector2Int current = new Vector2Int(0, 0);

        // mark the starting cell as part of the maze
        maze[current.x, current.y] = true;

        // add start pos to stack
        stack.Push(current);

        while (stack.Count > 0)// stuff exists in the stack
        {
            current = stack.Pop();// takes the top position to be checked.

            // make a list of up to 4 neibouts
            List<Vector2Int> neighbours = new List<Vector2Int>();

            if (current.x > 1 && !maze[current.x - 2, current.y])// is left available?
                neighbours.Add(new Vector2Int(current.x - 2, current.y));


            if (current.x < size - 2 && !maze[current.x + 2, current.y])// is two steps right available
                neighbours.Add(new Vector2Int(current.x + 2, current.y));

            // Check the cell two steps up and add it to the neighbors list if it's unvisited.
            if (current.y > 1 && !maze[current.x, current.y - 2])
                neighbours.Add(new Vector2Int(current.x, current.y - 2));

            // Check the cell two steps down and add it to the neighbors list if it's unvisited.
            if (current.y < size - 2 && !maze[current.x, current.y + 2])
                neighbours.Add(new Vector2Int(current.x, current.y + 2));

            // choose a neighbour  and add it to the stack
            if (neighbours.Count > 0) // did we add anything to the list
            {
                stack.Push(current);// puts back what we poped of the stack

                // choose a neighbour, but make sure you check its count as may differ
                Vector2Int chosenOne = neighbours[m_Random.Next(0, neighbours.Count)];

                if (chosenOne.x == current.x)
                {
                    maze[chosenOne.x, current.y + 1] = true;// mark gap vertically as true
                }
                else
                {
                    maze[chosenOne.x + 1, chosenOne.y] = true;// bridge the gap horizontally
                }
                maze[chosenOne.x, chosenOne.y] = true;

                stack.Push(chosenOne);
            }

        }

    }

    void DrawMazeIn3D()
    {
        // Loop through all the rows and columns for this grid looking to see what is
        // true (floor) false ( wall )
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (maze[x, y])
                {
                    GameObject.Instantiate(floor, new Vector3(x, 0, y), Quaternion.identity);
                }
                else
                {
                    GameObject.Instantiate(wall, new Vector3(x, 0, y), Quaternion.identity);
                }
            }
        }
    }
}

