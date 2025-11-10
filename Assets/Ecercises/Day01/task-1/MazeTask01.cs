using System.Collections.Generic;
using UnityEngine;



public class MazeTask01 : MonoBehaviour
{

    [Header("Maze Prefabs")]
    //attach wall and floor prefabs in inspector
    public GameObject wallPrefab;
    public GameObject floorPrefab;

    [Header("Maze Settings")]
    public int width = 10;
    public int height = 10;
    public Vector2Int startPos = Vector2Int.zero;
    public int seed = 0;
    public int steps = 30;

    //maze grid bool array content int x,y and value true/false
    private bool[,] maze;//maze grid stores bool 'true' - path/floor 'false' - wall
    private Vector2Int currPosition; 

   private List<Vector2Int> DirectionsList = new List<Vector2Int> 
    {
        Vector2Int.up, Vector2Int.down,Vector2Int.left,Vector2Int.right
    };

    System.Random  rand = new System.Random();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateMaze();
        DrawMaze();


    }
    void GenerateMaze() 
    {
        //step 1: initialize the maze grid
        maze = new bool[width, height];
        
        //step 2: set the starting position (0,0)
        //startPos = Vector2Int.zero;
        
        //step 3: mark the starting cell as a path
        maze[startPos.x, startPos.y] = true;
        steps--; //reduce the steps as starting cell already in use

        currPosition = startPos;
        
        //step 4: generate the maze
        while (steps > 0)
        {
            //step 5: choose a new direction
            var direction = currPosition + GetRandomDirection();


            //step 6: check boundaries 
            if (!isInbounds(direction, width, height))
                continue;//skip the iteration

            //step 7: mark the new position as a path
            maze[currPosition.x, currPosition.y] = true;

            //step 8: update the current position
            currPosition = direction;

            steps--;

        }
    }

    void DrawMaze() 
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (maze[x, y])
                {
                    //draw floor
                    //use transform as parent to keep hierarchy clean
                    Instantiate(floorPrefab, new Vector3(x, 0, y), Quaternion.identity,transform);
                }
                else
                {
                    //draw wall
                    //use transform as parent to keep hierarchy clean
                    Instantiate(wallPrefab, new Vector3(x, 0, y), Quaternion.identity,transform);
                }
            }
        }
    }
    private Vector2Int GetRandomDirection()=> DirectionsList[rand.Next(DirectionsList.Count)];

    private bool isInbounds(Vector2Int pos, int MazeWidth, int MazeHeight) 
    {
        if (pos.x < 0 || pos.x >= MazeWidth || pos.y < 0 || pos.y >= MazeHeight)
        {
            return false;
        }
        else 
        {
            return true;
        }
    }

        

    // Update is called once per frame
    void Update()
    {
        
    }
}
