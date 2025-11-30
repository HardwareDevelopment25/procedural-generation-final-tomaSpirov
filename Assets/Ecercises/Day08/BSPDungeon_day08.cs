using System.Collections.Generic;
using UnityEngine;


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
    // - bool[,] grid
    private bool[,] _grid;

    private GameObject parent;

    private int SmallestPossibleRoom = 2;

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
        _rng = new System.Random();
    }
    private void Start()
    {
        Generate();
    }

    // Generate():
    private void Generate()
    {
        Debug.Log("Generating Dungeon Boss");
        // 1) Clear previously spawned tiles
        ClearPreviouslySpawnedTiles();
        // 2) Initialise RNG
        // 3) Create root RectInt for map interior
        RectInt FirstLovelyRootRectangle = new RectInt(border, border, Mathf.Max(1, width - border), Mathf.Max(1, height - border));
        _root = new Node(FirstLovelyRootRectangle);
       
        // 4) Build BSP tree with SplitRecursive
        SplitRecursive(_root, depth: 0);
        // 5) For each leaf: create a room

        Debug.Log("Deb");
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
        SpawnFloorCubes();

    }

    private void SpawnFloorCubes()
    {
        parent = new GameObject("BSP DUNGEON FLOOR");

        // go through x and y in the grid and create floor

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (_grid != null && _grid[x, y] == true)
                {
                    // create the floor where
                    var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    go.transform.SetParent(parent.transform);
                    go.name = $"Tile_{x}_{y}";
                    go.transform.localPosition = new Vector3(x * tileSize, 0f, y * tileSize);
                    go.transform.localScale = new Vector3(tileSize, 0.1f, tileSize);

                    // maybe later add some material for prettyness

                }
            }
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
        Debug.Log("Recuresion " + depth);
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
            // Copy the above but slice horizontally
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
        // foreach (GameObject g in transform) GameObject.Destroy(g);
        GameObject.Destroy(parent);// killes em all
    }
}


//Write a comment above the class explaining what a leaf is and why leaves are where rooms go.
// A leaf has no further splits, making it perfect to spawn a room in

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
