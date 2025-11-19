using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public struct MyObjectData { 
    public Vector3 position;
    public Quaternion rotation;
};

public class LSystemGen : MonoBehaviour
{

    public GameObject turtle;
    public GameObject point;

    public GameObject pointGO;

    public float length ;
    public int iterations;
    public float angle;
    public string axiom;//starting string 

    public string[] laws;


    
    public string currentString;

    [SerializeField]
    Dictionary<char, string> rules = new Dictionary<char, string>();
    MyObjectData myObjectData = new MyObjectData();
    Stack<MyObjectData> stack;

    private void Awake()
    {
        myObjectData = new MyObjectData();
        stack = new Stack<MyObjectData>();
        
        foreach (var law in laws)
        {
            var l = law.Split("->");
            rules.Add(l[0][0], l[1]);

        }
        currentString = axiom;
        GenerateSystemString();//create the string 
        ApplyPatternRules(currentString);//apply rules to the string
    }

    private void GenerateSystemString()
    {
        for (int i = 0; i < iterations; i++)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var c in currentString) // for each character in the string
            {
                sb.Append(rules.ContainsKey(c) ? rules[c] : char.ToString(c));

            }
            currentString = sb.ToString();
        }
        Debug.Log("the current string is: "+ currentString);
    }




    private void RrulesbyChar(char c) 
    {
       

        LineRenderer lineRenderer = turtle.GetComponent<LineRenderer>();


        switch (c) 
        {
            case 'X':
                //- Do nothing (break).
                break;
            case 'F':
                // - Draw forwards by length

                //store turtle's position
                myObjectData.position = turtle.transform.position;
                //move turtle forwards by length
                turtle.transform.position += new Vector3(0, 0, length);
                myObjectData.position = turtle.transform.position;
                //instantiate new turtle - store this to temporary variable
                //Instantiate(point, new Vector3(0, 0, length), Quaternion.identity);
                //access linerenderer from the new instantiate object
                //set its first position to initial 
                //set its second to newly instantiated object pos
                //lineRenderer.DrawLine(lineRenderer.transform.position, myObjectData.position);



                break;
            case '+':
                // - Rotate right by your angle attributes (should be set to 25 degrees).
                myObjectData.rotation = Quaternion.Euler(0, angle, 0);
                break;
            case '-':
                // - Rotate left by your angle attributes (should be set to 25 degrees).
                myObjectData.rotation = Quaternion.Euler(0, angle, 0);
                break;
            case '[':


                /*
                    Save the primary turtle’s current position and rotation in a newly initialised TransformData struct.
                    Save this data to your Stack. You can use the Stack.Push(T item) method to achieve this.
                 */

                break;
            case ']':

                //Restore the primary turtle’s position and rotation to be equal to the last stored position and rotation (this point you stored[)

                break;


        }
        
    }


    private void ApplyPatternRules(string currStr) 
    {

        string strArrOfChars = currStr;
        foreach (char c in strArrOfChars)
        {

            //Debug.Log(c);
            RrulesbyChar(c);

        }



    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
