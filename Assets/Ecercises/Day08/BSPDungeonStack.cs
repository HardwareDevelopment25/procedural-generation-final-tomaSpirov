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

    // Option A: Automatically generate when the game starts.
    private void Start()
    {
        GenerateAll();
       
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
            dungeonInstance.transform.SetParent(transform);//this hierarchy is accurate
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
}
