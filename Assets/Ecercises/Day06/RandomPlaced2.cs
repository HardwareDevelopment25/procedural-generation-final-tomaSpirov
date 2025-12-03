using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RandomPlaced2 : MonoBehaviour
{
    public int totalTrees = 10;
    public GameObject treePrefab;
    System.Random random;
    private List<GameObject> list;

    private void Awake()
    {
        list = new List<GameObject>();
        StartCoroutine(SpawnPoints());
    }




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }


    IEnumerator SpawnPoints()
    {
        do
        {
            GameObject point = Instantiate(treePrefab);
            //var point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //Renderer renderer = point.GetComponent<Renderer>();
            //renderer.material.color = Color.red;
            point.transform.localPosition = generateCandidates(4, point.transform.localPosition);
            Debug.Log("object Sphere placed" + point.transform.localPosition);

            // generateCandidates(4, point.transform.localPosition);//generate candidate and secect by min distance to the last object placed

            totalTrees--;
            yield return new WaitForSeconds(0.01f);
        }
        while (totalTrees > 0);



    }



    private Vector3 generateCandidates(int candidates, Vector3 startPoss)
    {
        //int maxdistance = int.MinValue;
        float minDistance = float.MaxValue;
        Vector3 minObjPoss = startPoss;//keep thack the nearest object position to place on the scene



        //create number of candidate
        for (int i = 0; i < candidates; i++)
        {
            var nextPoss = new Vector3(Random.Range(0, totalTrees), 0f, Random.Range(-100, 100));
            //gameObjectsTemp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            // Renderer renderer = gameObjectsTemp.GetComponent<Renderer>();
            // renderer.material.color = Color.blue;
            // gameObjectsTemp.transform.localPosition = nextPoss;
            var distance = Vector3.Distance(startPoss, nextPoss);
            Debug.Log("Distanse so far: " + i + " = " + distance + " (startPos:" + startPoss + "/ nextPoss:" + nextPoss + ")");

            //list.Add(gameObjectsTemp);
            ////check for the closest one and select it. 
            if (distance < minDistance)
            {
                minDistance = distance;
                minObjPoss = nextPoss;

                Debug.Log("the curr MIN distance is: " + minDistance);
            }
        }

        //deleted all and return the new position
        // Debug.Log("Destroy all");
        /*foreach (var item in list) 
        {
            
            Destroy(item);//remove each of them from the list 
        }*/

        return minObjPoss;
    }

    // Update is called once per frame
    void Update()
    {



    }
}
