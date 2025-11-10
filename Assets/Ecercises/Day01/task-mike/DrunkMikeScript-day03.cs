using NUnit.Framework;
using System;
using System.Security.Cryptography.X509Certificates;
using UnityEditorInternal;
using UnityEngine;
using System.Collections.Generic;



public class DrunkMikeScriptDay03 : MonoBehaviour
{

    public GameObject WallPrefab, FloorPrefab;
    public Vector2Int gridSize = Vector2Int.one;
    public Vector2Int startPos = Vector2Int.zero;
    public int seed = 0;
    private bool[,] Maze;
    
    public int steps = 30 ;


    System.Random rand;

    private void Awake()
    {
        // Maze = new bool[gridSize.x, gridSize.y];

        rand = new System.Random(seed);

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    //think about repeatability

    //get directions

   
}


