using UnityEngine;
using Unity.Collections;
using System.Collections;
using NUnit.Framework.Internal.Commands;

public class CellAutomat : MonoBehaviour
{
    public int gridSize = 64;
    public float percentageOfOnes = 0.4f;
    public int seed = 0;

    public MeshRenderer planeRenderer;
    private int[,] intGrid;
    public float speed = 5;

    public GameObject wall;
    public GameObject floor;


    private void Awake()
    {
        intGrid = new int[gridSize, gridSize];
    }

    private void Start()
    {
        System.Random  rnd = new System.Random(seed);

        for (int i = 0; i < gridSize;  i++) 
        {
            for (int j = 0; j < gridSize; j++)
            {
                double randomFloat = rnd.NextDouble();

                if (randomFloat< percentageOfOnes)
                {
                    intGrid[i, j] = 1;
                }
                else 
                {
                    intGrid[i, j] = 0;
                }

            }
        }

        planeRenderer = GameObject.CreatePrimitive(PrimitiveType.Plane).GetComponent<MeshRenderer>();
        intGrid = ProcGenTool.BorderMe(intGrid);
        SmoothMap();
        //ApplyFloorAndWall();

        planeRenderer.material.mainTexture = ProcGenTool.ConvertBoolArrayAsIntTexture(intGrid);
        //int totalNeighbours = ProcGenTool.checkFortNeighbours(intGrid);
        StartCoroutine(animator());

        //add this to render texture of prefabs
        //Render3D();

    }
    void GenerateMarchinSquares() 
    {

        for (int x = 0; x < intGrid.GetLength(0); x++) 
        {
            for (int y = 0; y < intGrid.GetLength(1); y++) 
            {
                Debug.Log(GetConfigIndex(x, y));
            }
        }
        
    }

    int GetConfigIndex(int x, int y) 
    {
        int configIndex = 0;
        if (intGrid[x,y] == 1) configIndex |= 1;
        if (intGrid[x+1,y] == 1) configIndex |= 2;
        if (intGrid[x+1,y+1] == 1) configIndex |= 4;
        if (intGrid[x,y+1] == 1) configIndex |= 8;

        return configIndex;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SmoothMap();
            planeRenderer.material.mainTexture = ProcGenTool.ConvertBoolArrayAsIntTexture(intGrid);
            Debug.Log("space update");
        }
    }

    IEnumerator animator() 
    {
        while (true) {

            SmoothMap();
            planeRenderer.material.mainTexture = ProcGenTool.ConvertBoolArrayAsIntTexture(intGrid);
            yield return new WaitForSeconds(speed);
        }
    }

    void ApplyFloorAndWall() 
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if (intGrid[i, j] == 1)
                {
                    //draw a floor

                }
                else {
                    //draw a wall
                }
                
            }
        }

    }

    void SmoothMap() 
    {

        for (int i = 0; i < gridSize; i++) 
        {
            for (int j = 0; j < gridSize; j++)
            {
                int nei = GetNeighbours(i, j);
                if (nei > 4) intGrid[i, j] = 1;
                else if(nei<4)intGrid[i, j] = 0;
            }
        }
    }



    void WalframsRule() 
    {
        for (int x = 0; x < gridSize; x++) 
        {
            for (int y = 0; y < gridSize; y++) 
            {
                int neighbours = GetNeighbours(x, y);
                //int neighbours = ProcGenTool.checkFortNeighbours(x, y);


                if (intGrid[x, y] == 1)//for space is populated
                {
                    if (neighbours < 2) intGrid[x, y] = 0;
                    else if (neighbours < 3) intGrid[x, y] = 0;
                    else if (neighbours == 2 || neighbours == 3) intGrid[x, y] = 1;
                }
                else 
                {
                    if (neighbours == 3) intGrid[x, y] = 1;
                }
            }
        }
        
    }

    public int GetNeighbours(int currXpos, int currYpos) 
    {

        int totalNeighbours = 0;

        for (int x = - 1; x <=  1; x++) 
        {
            for (int y = - 1; y <= 1; y++)
            {
                if (isMapInRange(currXpos+x, currYpos+y)) 
                { //Debug.Log("x: " + currYpos + "y: " + currYpos);
                    //Debug.Log("curr value on grid with above pos" + intGrid[currXpos, currYpos]);//do not chech use it outside this border check throw exeptions
                    if (intGrid[currXpos+x, currYpos+y]==1) totalNeighbours++;
                }
               
            }
        }
        if (intGrid[currXpos, currYpos] == 1) totalNeighbours--;


        return totalNeighbours;
    }

    bool isMapInRange(int x, int y)=> x>=0 && y>=0 && x <gridSize && y < gridSize; 
    
}
