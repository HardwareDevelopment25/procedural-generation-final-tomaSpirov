using System.Collections.Generic;
using UnityEngine;

//Part 3 - Multiple Floors 
public class BSPDungeonStack : MonoBehaviour
{
    
    // Prefab reference to the single-floor dungeon (created earlier as BSPFloorPrefab).
    public BSPDungeon_day08 dungeonPrefab;

    // Number of floors to generate.
    public int floorCount = 3;

    // Vertical spacing between floors (height offset).
    public float floorHeight = 3f;

    // Internal list to keep track of generated floors.
    // Useful later for connecting floors with stairs or portals.
    private List<BSPDungeon_day08> _floors = new List<BSPDungeon_day08>();

    //Stairs properties
    public GameObject stairsPrefab;
    public float stairYOffset = 0f;
    public int landingRadius = 1;
    public int holeRadius = 0;


    // Option A: Automatically generate when the game starts.
    private void Start()
    {
        GenerateAll();

        Debug.Log("Total Levels "+_floors.Count+" generated of " +floorCount+"requested");
        for (int i = 0; i < floorCount-1; i++)//do not use <= will break the loop
        {
            Debug.Log("curr floor num: "+i+ " and next "+(i+1));
            AddStairsBetween(_floors[i], _floors[i + 1]);
        }
    }

    // Option B: Allow manual generation in the editor via context menu.
    [ContextMenu("Generate All Floors")]
    public void GenerateAll()
    {
        // GenerateAll():
        // 1) Clear old floors
        ClearChildren();
        _floors.Clear();

        // 2) For i from 0 to floorCount-1:
        for (int i = 0; i < floorCount; i++)
        {
            // - Instantiate dungeonPrefab as child
            var dungeonInstance = Instantiate(dungeonPrefab,transform);

            // - Set localPosition = (0, i * floorHeight, 0)
            dungeonInstance.transform.localPosition = new Vector3(0f, i * floorHeight, 0f);
            //dungeonInstance.transform.SetParent(transform);//this hierarchy is accurate
            //dungeonInstance.transform.localPosition = new Vector3(i*0f, i*3f, i * 0f);//just for test not in use

            // - Call Generate() on the BSPDungeon
            dungeonInstance.SendMessage("Generate");

            // - Store reference in a List<BSPDungeon>
            _floors.Add(dungeonInstance);
        }
    }
    //using for-loop bakwards to destroy objects because this will prevent skipping some of the object while iterate the objects list
    private void ClearChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i).gameObject;

#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(child);
            else
                Destroy(child);
#else
            Destroy(child);
#endif
        }
    }

    public void AddStairsBetween(BSPDungeon_day08 lower, BSPDungeon_day08 upper)
    {
        Debug.Log("UPPER LEVEL "+upper.ToString());
        // AddStairsBetween:
        // 1) Ask lower for a random room tile (stairs base).
        if (!lower.TryGetRandomRoomTile(out Vector2Int tile))
        {
            Debug.LogWarning("No room tile found for stairs.");
            return;
        }

        // 2) Ensure landing at same tile on upper.
        upper.EnsureLanding(tile, landingRadius);

        // 3) Cut a hole for stairs on upper if needed.
        if (holeRadius > 0)
            upper.CutHole(tile, holeRadius);

        // 4) Refresh visuals on upper so changes show.
        upper.RefreshVisualsFromGrid();
        lower.RefreshVisualsFromGrid();
        // 5) Instantiate stairsPrefab at base tile position.
        Vector3 basePos = lower.TileToWorldCenter(tile);
        basePos.y += stairYOffset/2; // optional offset
       
       // var stairs = Instantiate(stairsPrefab, basePos, Quaternion.identity, transform);
        var stairs = Instantiate(stairsPrefab, basePos,Quaternion.identity,transform);
        stairs.transform.SetParent(lower.transform, true);
        stairs.name = $"Stairs_{lower.name}_to_{upper.name}";
     
        
    }
}
