using System.Collections.Generic;
using UnityEngine;
//Task Sheet 2 - Edge Walls Around Floor Tiles 
// Edge wall plan:
// - Find edges where floor meets empty.
// - Merge consecutive edges into long strips.
// - For each strip, spawn a thin wall cube.


//Adding Edge Walls
public class BSPDungeon_day08 : MonoBehaviour
{
    // TODO: fields we need:
    // - map size (width, height)
    public int width = 80, height = 40;
    // - maxDepth, minLeafSize
    public int maxDepth = 6;
    public int minLeafSize = 10;
    // - room size min/max
    public Vector2Int roomSizeMin = new Vector2Int(4, 4);
    public Vector2Int roomSizeMax = new Vector2Int(12, 10);
    // - tile size
    public float tileSize = 1f;
    // - material for floor cubes
    public Material floorMaterial;
    public Material floorMaterialEmpty;

    // Edge walls configuration:

    public bool buildEdgeWalls = true;// toggle edge wall generation (thin boundary walls)
    
    public float wallHeight = 2f;// height of wall
    
    public float wallThickness = 0.25f;// thickness of wall strips
    
    public Material wallMaterial;

    //seed
    public int seed = 10;//use seed to get same result for testing

    // - bool[,] grid
    private bool[,] _grid;

    
    private GameObject parentFloor;

    private GameObject parentWalls;

    //private int SmallestPossibleRoom = 2;

    public int border = 2, corridorWidth = 1;
    // - System.Random rng
    private System.Random _rng;

    private Node _root;

    public double BiasToLongerRooms = 0.8;

    private List<RectInt> _rooms = new List<RectInt>();
    private List<RectInt> _corridors = new List<RectInt>();
    private List<RectInt> _leafs = new List<RectInt>();

    private void Awake()
    {
        _rng = new System.Random(seed);
    }
    private void Start()
    {
        Generate();
    }

    // Generate():
    private void Generate()
    {
        //Debug.Log("Generating Dungeon Boss");
        // 1) Clear previously spawned tiles
        //ClearePreviousSpawnedFloors();
        //ClearPreviouslySpawnedTiles();
        // 2) Initialise RNG
        // 3) Create root RectInt for map interior
        RectInt FirstLovelyRootRectangle = new RectInt(border, border, Mathf.Max(1, width - border), Mathf.Max(1, height - border));
        _root = new Node(FirstLovelyRootRectangle);

        // 4) Build BSP tree with SplitRecursive
        SplitRecursive(_root, depth: 0);
        // 5) For each leaf: create a room

       // Debug.Log("Deb");
        foreach (var leaf in _root.GetLeaves())
        {
            var room = CreateRoomInsideLeaf(leaf.rect);// YAY we made a room
            leaf.room = room;
            _rooms.Add(room);
            _leafs.Add(leaf.rect);

        }
        // 6) Connect rooms via corridors
        // 7) Rasterize rooms + corridors into _grid
        _grid = new bool[width, height];

        ConnectTree(_root);

        RasterizeRoomsAndCorridors(); // render this out in some way

        // 8) Instantiate floor cubes from _grid
       //SpawnFloorCubes();

        // 9) Build thin edge walls around floor tiles (boundary between floor and empty)
        // if (buildEdgeWalls) BuildEdgeWallsFromGrid();
        if (buildEdgeWalls) BuildEdgeWallsFromGrid();
    }

    private void SpawnFloorCubes()
    {
        parentFloor = new GameObject("BSP DUNGEON FLOOR");
        parentFloor.transform.SetParent(transform, false);//WORKS added as not works before due to different gameobjects hierarchy
        // go through x and y in the grid and create floor

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                
                if (_grid != null && _grid[x, y] == true)
                {
                    // create the floor
                    var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    go.name = $"Tile_{x}_{y}";
                    go.transform.localPosition = new Vector3(x * tileSize, 0f, y * tileSize);//testing
                    go.transform.localScale = new Vector3(tileSize, 0.1f, tileSize);
                    go.transform.SetParent(parentFloor.transform, false);//WORKS


                    if (floorMaterial != null)
                    {
                        var renderer = go.GetComponent<Renderer>();
                        if (renderer != null) renderer.material = floorMaterial;
                    }
                }

               
            }
        }
    }

    // Builds thin wall strips by scanning for edges in the _grid
    private void BuildEdgeWallsFromGrid()
    {
        parentWalls = new GameObject("BSP DUNGEON EDGE WALLS");
        parentWalls.transform.SetParent(this.transform,false);//WORKS added as not works before due to different gameobjects hierarchy
        
        // horizontal edge - between row y and y+1
        ScanHorizontalEdges();

        // vertical edge - between column x and x+1
        ScanVerticalEdges();

        // outer boundaries (floor touching 'outside' which is false)
        ScanOuterBoundaries();
    }

    private void ScanHorizontalEdges()
    {
        if (_grid == null) return;

        for (int y = 0; y <= height - 2; y++)
        {
            int x = 0;
            while (x < width)
            {
                bool diff = _grid[x, y] != _grid[x, y + 1];
                if (!diff)
                {
                    x++;
                    continue;
                }
                int xStart = x;
                
                while (x < width && (_grid[x, y] != _grid[x, y + 1]))
                {
                    x++;
                }
                int xEnd = x; 

                // create horizontal wall strip
                CreateWallStripHorizontal(xStart, xEnd, y);
            }
        }
    }

    // create a horizontal wall strip between row y and y+1 for x
    private void CreateWallStripHorizontal(int startX, int endX, int y)
    {
        int length = endX - startX;
        if (length <= 0) return;

       
        float centerX = ((startX + endX - 1) * 0.5f) * tileSize;
        float centerZ = (y + 0.5f) * tileSize;

        Vector3 center = new Vector3(centerX, wallHeight * 0.5f, centerZ);
        Vector3 scale = new Vector3(length * tileSize, wallHeight, wallThickness * tileSize);

        SpawnWallStrip(center, scale, $"Wall_H_{startX}_{endX - 1}_Row{y}");
    }

   
    private void ScanVerticalEdges()
    {
        if (_grid == null) return;

        for (int x = 0; x <= width - 2; x++)
        {
            int y = 0;
            while (y < height)
            {
                bool diff = _grid[x, y] != _grid[x + 1, y];
                if (!diff)
                {
                    y++;
                    continue;
                }

                int yStart = y;
                while (y < height && (_grid[x, y] != _grid[x + 1, y]))
                {
                    y++;
                }
                int yEnd = y;
                // create vertical wall strip
                CreateWallStripVertical(yStart, yEnd, x);
            }
        }
    }

    // Create a vertical wall strip between column x and x+1 for y
    private void CreateWallStripVertical(int startY, int endY, int x)
    {
        int length = endY - startY;
        if (length <= 0) return;

        
        float centerX = (x + 0.5f) * tileSize;
        float centerZ = ((startY + endY - 1) * 0.5f) * tileSize;

        Vector3 center = new Vector3(centerX, wallHeight * 0.5f, centerZ);
        Vector3 scale = new Vector3(wallThickness * tileSize, wallHeight, length * tileSize);

        SpawnWallStrip(center, scale, $"Wall_V_{startY}_{endY - 1}_Col{x}");
    }

    
    private void ScanOuterBoundaries()
    {
        if (_grid == null) return;

        // Top boundary (between outside and row 0)
        {
            int x = 0;
            while (x < width)
            {
                if (_grid[x, 0] == true)
                {
                    int start = x;
                    while (x < width && _grid[x, 0] == true) x++;
                    int end = x;

                    float centerX = ((start + end - 1) * 0.5f) * tileSize;
                    float centerZ = (-0.5f) * tileSize; // half step above row 0
                    Vector3 center = new Vector3(centerX, wallHeight * 0.5f, centerZ);
                    Vector3 scale = new Vector3((end - start) * tileSize, wallHeight, wallThickness * tileSize);
                    SpawnWallStrip(center, scale, $"Wall_OuterTop_{start}_{end - 1}");
                }
                else x++;
            }
        }

        // Bottom boundary (between row height-1 and outside)
        {
            int x = 0;
            while (x < width)
            {
                if (_grid[x, height - 1] == true)
                {
                    int start = x;
                    while (x < width && _grid[x, height - 1] == true) x++;
                    int end = x;

                    float centerX = ((start + end - 1) * 0.5f) * tileSize;
                    float centerZ = (height - 0.5f) * tileSize; // half step below last row
                    Vector3 center = new Vector3(centerX, wallHeight * 0.5f, centerZ);
                    Vector3 scale = new Vector3((end - start) * tileSize, wallHeight, wallThickness * tileSize);
                    SpawnWallStrip(center, scale, $"Wall_OuterBottom_{start}_{end - 1}");
                }
                else x++;
            }
        }

        // Left boundary (between outside and col 0)
        {
            int y = 0;
            while (y < height)
            {
                if (_grid[0, y] == true)
                {
                    int start = y;
                    while (y < height && _grid[0, y] == true) y++;
                    int end = y;

                    float centerX = (-0.5f) * tileSize; // half step left of col 0
                    float centerZ = ((start + end - 1) * 0.5f) * tileSize;
                    Vector3 center = new Vector3(centerX, wallHeight * 0.5f, centerZ);
                    Vector3 scale = new Vector3(wallThickness * tileSize, wallHeight, (end - start) * tileSize);
                    SpawnWallStrip(center, scale, $"Wall_OuterLeft_{start}_{end - 1}");
                }
                else y++;
            }
        }

        // Right boundary (between col width-1 and outside)
        {
            int y = 0;
            while (y < height)
            {
                if (_grid[width - 1, y] == true)
                {
                    int start = y;
                    while (y < height && _grid[width - 1, y] == true) y++;
                    int end = y;

                    float centerX = (width - 0.5f) * tileSize; // half step right of last col
                    float centerZ = ((start + end - 1) * 0.5f) * tileSize;
                    Vector3 center = new Vector3(centerX, wallHeight * 0.5f, centerZ);
                    Vector3 scale = new Vector3(wallThickness * tileSize, wallHeight, (end - start) * tileSize);
                    SpawnWallStrip(center, scale, $"Wall_OuterRight_{start}_{end - 1}");
                }
                else y++;
            }
        }
    }

    // Spawning thin wall strips
    //instantiate a cube, set position to center, scale to scale,
    //assign wallMaterial, parent under this dungeon's walls parent
    private void SpawnWallStrip(Vector3 center, Vector3 scale, string name)
    {
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        if (parentWalls == null)
        { 
            parentWalls = new GameObject("BSP DUNGEON EDGE WALLS");
            parentWalls.transform.SetParent(this.transform,false);//added as not works before due to different gameobjects hierarchy
        }
        cube.transform.SetParent(parentWalls.transform, false);

        cube.transform.localPosition = center;
        cube.transform.localScale = scale;

        if (wallMaterial != null)
        {
            var renderer = cube.GetComponent<Renderer>();
            if (renderer != null) renderer.material = wallMaterial;
        }
    }

    private void ConnectTree(Node node)
    {
        if (node == null || node.IsLeaf) return;

        ConnectTree(node.left);
        ConnectTree(node.right);

        var leftRoom = node.left.GetAnyRoom();
        var rightRoom = node.right.GetAnyRoom();

        // So we have two rooms left and right we now find the center and make the center point
        var a = FindCenter(leftRoom.Value);
        var b = FindCenter(rightRoom.Value);

        var mid = new Vector2Int(b.x, a.y);

        CarveCorridor(a, mid);
        CarveCorridor(mid, b);

    }
    private Vector2Int FindCenter(RectInt r)
    {
        return new Vector2Int(Mathf.RoundToInt(r.center.x), Mathf.RoundToInt(r.center.y));
    }

    private void RasterizeRoomsAndCorridors()//into a grid
    {
        foreach (var rooms in _rooms) FillRect(rooms, true);
        foreach (var c in _corridors) FillRect(c, true);
    }
    private void CarveCorridor(Vector2Int from, Vector2Int to)
    {

        if (from.y == to.y)
        {
            int x0 = Mathf.Min(from.x, to.x);
            int w = Mathf.Abs(from.x - to.x);
            var rect = new RectInt(x0, from.y, w + 1, 1);
            _corridors.Add(rect);
            return;
        }

        if (from.x == to.x)
        {
            int y0 = Mathf.Min(from.y, to.y);
            int h = Mathf.Abs(from.y - to.y);
            var rect = new RectInt(from.x, y0, 1, h + 1);
            _corridors.Add(rect);
            return;
        }

    }

    //converts my rectangles into a grid
    private void FillRect(RectInt r, bool value)
    {
        int x0 = Mathf.Clamp(r.xMin, 0, width - 1);
        int x1 = Mathf.Clamp(r.xMax - 1, 0, width - 1);
        int y0 = Mathf.Clamp(r.yMin, 0, height - 1);
        int y1 = Mathf.Clamp(r.yMax - 1, 0, height - 1);

        for (int y = y0; y <= y1; y++)
        {
            for (int x = x0; x <= x1; x++)
            {
                _grid[x, y] = value;
            }
        }
    }

    // This will simply take that leaf and within it put a random sized room
    private RectInt CreateRoomInsideLeaf(RectInt thisLeaf)
    {
        int maxW = Mathf.Min(roomSizeMax.x, thisLeaf.width - 2 * border);
        int maxH = Mathf.Min(roomSizeMax.y, thisLeaf.height - 2 * border);

        int minW = Mathf.Min(roomSizeMax.x, maxW);
        int minH = Mathf.Min(roomSizeMax.y, maxH);

        // Fallback: tiny 1x1 to avoid empty rooms if leaf got too small
        if (minW <= 0 || minH <= 0)
            return
       new RectInt((int)thisLeaf.center.x, (int)thisLeaf.center.y, 1, 1);

        // Next step is to randomly put the room into the leaf

        int w = _rng.Next(minW, maxW + 1); // choose a random w and H
        int h = _rng.Next(minH, maxH + 1);

        int x = _rng.Next(thisLeaf.xMin + border, thisLeaf.xMax - border - w + 1);
        int y = _rng.Next(thisLeaf.yMin + border, thisLeaf.yMax - border - h + 1);

        return new RectInt(x, y, w, h);
    }

    void SplitRecursive(Node node, int depth)
    {
        //Debug.Log("Recuresion " + depth);
        // a) Stopping rules: if depth is high enough OR rect is too small, return
        if (depth >= maxDepth ||
        node.rect.width < 2 * minLeafSize && node.rect.height < 2 * minLeafSize) return;
        // b) Decide whether we can split horizontally / vertically
        bool canSplitV = node.rect.width >= 2 * minLeafSize; // We know we can at least fit one
        bool canSplitH = node.rect.height >= 2 * minLeafSize;
        // If a cant split, then cancel everything and stop
        if (canSplitH == false && canSplitV == false) return;

        // c) Choose orientation and a split line that keeps both children >= minLeafSize
        bool splitVert;

        if (canSplitH == true && canSplitV == true)
        {
            // We know a split could happen , ooooo goodie

            // (prefer longer axis) which is longer
            bool widthIsHigher = node.rect.width > node.rect.height;
            // Deciding on a random whim if the room should be longer
            if (_rng.NextDouble() < BiasToLongerRooms)
                splitVert = widthIsHigher;
            else
                splitVert = !widthIsHigher;
        }
        else
        {
            splitVert = canSplitV;
        }

        if (splitVert)
        {
            // we dont want to put a room in a place it wont fit
            int minX = node.rect.xMin + minLeafSize;
            int maxX = node.rect.xMax - minLeafSize;
            //overkill  defensive check in case some how there aint enough room

            if (minX >= maxX) return;

            //choose your random slice of vert
            int splitXRand = _rng.Next(minX, maxX);

            // create our new two rectanges
            var left = new RectInt(node.rect.xMin, node.rect.yMin, splitXRand - node.rect.xMin, node.rect.height);
            var right = new RectInt(splitXRand, node.rect.yMin, node.rect.xMax - splitXRand, node.rect.height);
            node.left = new Node(left);
            node.right = new Node(right);

            // split verticaly
        }
        else
        {
            
            //split horizontally

            // we dont want to put a room in a place it wont fit
            int minY = node.rect.yMin + minLeafSize;
            int maxY = node.rect.yMax - minLeafSize;
            //overkill  defensive check in case some how there aint enough room

            if (minY >= maxY) return;

            //choose your random slice of vert
            int splitYRand = _rng.Next(minY, maxY);

            // create our new two rectanges
            var top = new RectInt(node.rect.xMin, node.rect.yMin, node.rect.width, splitYRand - node.rect.yMin);
            var bottom = new RectInt(node.rect.xMin, splitYRand, node.rect.width, node.rect.yMax - splitYRand);
            node.left = new Node(top);
            node.right = new Node(bottom);

        }

        SplitRecursive(node.left, depth + 1);
        SplitRecursive(node.right, depth + 1);
    }

    // d) Create left/right (or top/bottom) child RectInts
    // e) Create child Nodes and recurse

    private void ClearPreviouslySpawnedTiles()
    {
        // destroy everything you ever made
        if(parentFloor!=null) GameObject.Destroy(parentFloor);
        if(parentWalls != null) GameObject.Destroy(parentWalls);
       
    }

    private void ClearePreviousSpawnedFloors() 
    {
        // destroy everything you ever made
        if (parentFloor != null) GameObject.Destroy(parentFloor);
    }

    //choose a tile inside a random room
    public bool TryGetRandomRoomTile(out Vector2Int tile)
    {
        tile = Vector2Int.zero;
        if (_rooms == null || _rooms.Count == 0) return false;

        // random room
        var room = _rooms[_rng.Next(_rooms.Count)];

        // random tile inside that room
        int x = _rng.Next(room.xMin, room.xMax);
        int y = _rng.Next(room.yMin, room.yMax);
        tile = new Vector2Int(x, y);
        return true;
    }

    //force tiles around the tile to be floor
    public void EnsureLanding(Vector2Int tile, int radius)
    {
        for (int dy = -radius; dy <= radius; dy++)
        {
            for (int dx = -radius; dx <= radius; dx++)
            {
                int nx = tile.x + dx;
                int ny = tile.y + dy;
                if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                    _grid[nx, ny] = true;
            }
        }
    }

    //turn some floor tiles into empty for stair openings
    public void CutHole(Vector2Int tile, int radius)
    {
        for (int dy = -radius; dy <= radius; dy++)
        {
            for (int dx = -radius; dx <= radius; dx++)
            {
                int nx = tile.x + dx;
                int ny = tile.y + dy;
                if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                {
                    _grid[nx, ny] = false;
                    Debug.Log("Cuttet at x:" + nx + ",y: " + ny + " => false/" + _grid[nx, ny]);
                }
                   
            }
        }
        Debug.Log("Exit Cutter method");
    }

    //return world-space centre of a tile on this floor.
    public Vector3 TileToWorldCenter(Vector2Int tile)
    {
        float cx = (tile.x) * tileSize;
        float cz = (tile.y-1f) * tileSize;
        return transform.TransformPoint(new Vector3(cx, 0f, cz));
    }

    //destroy existing cubes/walls and rebuild from _grid
    public void RefreshVisualsFromGrid()
    {
        Debug.Log("UPDATED-REDRAWED");
        // Clear old visuals
       //ClearPreviouslySpawnedTiles();
        ClearePreviousSpawnedFloors();
       
        // Rebuild floor cubes
        SpawnFloorCubes();

        // Rebuild edge walls if enabled
        //if (buildEdgeWalls) BuildEdgeWallsFromGrid();
    }

}


// A leaf (this is the end of the tree branch) has no further splits, making it perfect to spawn a room in

class Node
{
    //It should hold: `RectInt rect; Node left, right; RectInt? room;`
    public RectInt rect;
    public Node left, right;
    public RectInt? room;

    // need to return if it has no further splits
    // Leafs is named from a tree, aka being the final result with no
    // further splitting or iteration.
    public bool IsLeaf => (left == null && right == null);

    public IEnumerable<Node> GetLeaves()
    {
        if (IsLeaf) { yield return this; yield break; }
        if (left != null) foreach (var n in left.GetLeaves()) yield return n;
        if (right != null) foreach (var n in right.GetLeaves()) yield return n;
    }

    //• Add a constructor that takes a RectInt.
    public Node(RectInt r)
    {
        rect = r;
    }

    public RectInt? GetAnyRoom() // Do we even have a room to give
    {
        if (room.HasValue) return room;
        RectInt? r = left?.GetAnyRoom();
        if (r.HasValue) return r;
        return right?.GetAnyRoom();
    }
}