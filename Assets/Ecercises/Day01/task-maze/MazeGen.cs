using System.Collections.Generic;
using UnityEngine;

public class MazeGen : MonoBehaviour
{

    public GameObject WallPrefab, FloorPrefab;
    public Vector2Int gridSize = Vector2Int.one;
    public Vector2Int startPos = Vector2Int.zero;
    public int seed = 0;
    private bool[,] Maze;

    public int steps = 30;
    //added
    private Stack<Vector2Int> stack = new Stack<Vector2Int>();



    //private Stack<bool> stack = new Stack<bool>();
    private int offsetMove = 2;//add more steps to current

    System.Random rand;

    private void Awake()
    {
        rand = new System.Random();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        DrawMaze();
        DrawGrid();
       
    }

    public static List<Vector2Int> DirectionList = new List<Vector2Int> { 
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right 
    };

    //get random direction
    private Vector2Int GetRandomDirection() => DirectionList[rand.Next(0, DirectionList.Count)];





    private void DrawMaze()
    {
        Maze = new bool[gridSize.x, gridSize.y]; //reset each time

        //true Floor
        Maze[startPos.x, startPos.y] = true;//make any step at the area as a floor
        Vector2Int currentPossition = startPos;

        Debug.Log(currentPossition + " DIRECTION, " + currentPossition.x + " X / " + currentPossition.y + " Y");

        while (steps > 0)
        {
            
            steps--;
            //var direction = currentPossition + GetRandomDirection();//where I am plus a randon direction 
            var direction = currentPossition;// + GetRandomDirection();//where I am plus a randon direction 

            //check all avilable directions by checking all Neighbors and add them to a stack


            /*while (stack.Count > 0) 
            {


                if (current.x < sizeof - 2 && !Maze[current.x + 2, current.y])
                    neightmors.Add(new Vector2Int(current.x + 2, current.y));
            
            }*/



            for (int dir = 0; dir < DirectionList.Count; dir++) 
            {
                //checkdirection
              direction += DirectionList[dir];
                Debug.Log(direction +" DIRECTION, "+ direction.x +" X / " + direction.y+ " Y");
                
            }



            //  do some bounds checking
            if (!IsInbounds(direction, gridSize)) continue;

            Maze[currentPossition.x, currentPossition.y] = true;
            currentPossition = direction;
        }
    }
    //check bounds
    public bool IsInbounds(Vector2Int pos, Vector2Int matrixSize)
    {
      
      

        if (pos.x < 0 || pos.x >= matrixSize.x || pos.y < 0 || pos.y > matrixSize.y)
        {
            return false;
        }
        else
        {
            return true;
        }



    }
    //render the maze
    public void DrawGrid()
    {
        for (int y = 0; y < gridSize.x; y++)
        {
            for (int x = 0; x < gridSize.y; x++)
            {
                if (Maze[x, y])
                {
                    //true Floor
                    GameObject.Instantiate(FloorPrefab, new Vector3(x, 0f, y), Quaternion.identity);
                }
                else
                {
                    //false Wall
                    GameObject.Instantiate(WallPrefab, new Vector3(x, 0.5f, y), Quaternion.identity);
                }

            }

        }

    }

   
}
