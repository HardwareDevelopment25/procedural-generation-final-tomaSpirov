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

    public GameObject cellPrefab;
    public GameObject wallPrefab;
    public GameObject waterPrefab;
    public GameObject woodPrefab;
    public GameObject grassPrefab;
    public GameObject floorPrefab;
    public Sprite[] martinSprite;

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
        ApplyFloorAndWall();

        planeRenderer.material.mainTexture = ProcGenTool.ConvertBoolArrayAsIntTexture(intGrid);
        //int totalNeighbours = ProcGenTool.checkFortNeighbours(intGrid);
        StartCoroutine(animator());
        GenerateMarchinSquares();
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
                PlaceCell(x,y,(GetConfigIndex(x, y)));
            }
        }
        
    }

    void PlaceCell(int x, int y, int configIndex) 
    {
        Vector3 pos = new Vector3(x,0, y);
        GameObject cell = Instantiate(cellPrefab, pos, Quaternion.identity);
        cell.transform.rotation = Quaternion.Euler(90,0,0);
        SpriteRenderer spriteRenderer = cell.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = martinSprite[configIndex];


    }

    public void Render3D(int[,] grid) 
    {
        for (int x = 0; x < intGrid.GetLength(0); x++) 
        {
            for (int y = 0; y < intGrid.GetLength(1); y++)
            {
                switch (grid[x, y]) 
                {
                    case 0:
                        GameObject.Instantiate(floorPrefab, new Vector3(x, 0, y), Quaternion.identity, this.transform);
                        break;
                        case 1: GameObject.Instantiate(wallPrefab, new Vector3(x, 0, y), Quaternion.identity, this.transform);
                        break;
                    case 2:
                        GameObject.Instantiate(waterPrefab, new Vector3(x, 0, y), Quaternion.identity, this.transform);
                        break;
                    case 3:
                        GameObject.Instantiate(grassPrefab, new Vector3(x, 0, y), Quaternion.identity, this.transform);
                        break;
                    case 4:
                        GameObject.Instantiate(waterPrefab, new Vector3(x, 0, y), Quaternion.identity, this.transform);
                        break;

                }

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
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (intGrid[x, y] == 1)
                {
                    //draw a floor
                    GameObject.Instantiate(floorPrefab, new Vector3(x, 0, y), Quaternion.identity, transform);
                }
                else if(intGrid[x, y] == 0)
                {
                    //draw a wall
                    GameObject.Instantiate(wallPrefab, new Vector3(x, 0, y), Quaternion.identity, transform);
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
