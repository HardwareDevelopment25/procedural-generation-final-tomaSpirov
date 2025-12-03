using System.Collections.Generic;
using System.Data;
using System.Text;
using UnityEngine;



public class Tree3D : MonoBehaviour
{
    [Header("L-System Prefabs")]
    public GameObject woodPrefab;
    public GameObject leafPrefab;

    //Step 1: Setting Up the Script
    [Header("L-System Configuration")]
    public string Name = "Tree3D";
    public int iterations = 5;
    public float length = 1.0f;
    public float angle = 25;
    
    public string axiom = "X";
    public string[] laws;
    [SerializeField]
    private Dictionary<char, string>rules = new Dictionary<char, string>();
    
    public string currentString;
   

    //Step 2: Initializing the Rules
    private void Awake()
    {
        foreach (var law in laws) 
        {
            var l= law.Split("->");
            rules.Add(l[0][0], l[1]);
        }

         currentString = axiom;
    }

    //Step 3: Generating the L-System String
    private void GenerateLSystem() 
    {
        for (int i = 0; i < iterations; i++) 
        {   
            StringBuilder sb = new StringBuilder();
            
            foreach (var c in currentString)
            {
                sb.Append(rules.ContainsKey(c)? rules[c]: char.ToString(c));
            }

            currentString = sb.ToString();
        }
        Debug.Log("Generated Sting is: "+currentString);
    }

    //Step 4: Drawing the L-System
    private void DrawLSystem() 
    {
        Stack<TransformInfo> transformStack = new Stack<TransformInfo>();
        
        Vector3 currentPos = transform.position;
        Quaternion currentRot = transform.rotation;
        Vector3 startPos = Vector3.zero;

        //apply all the rules to the current string char by char
        foreach (var c in currentString)
        {
            switch (c)
            {
                //	Moves the drawing position forward when encountering 'F' or 'G'.
                case 'F':
                case 'G':
                    startPos = currentPos;
                    currentPos += currentRot * Vector3.forward * length;
                    
                    Instantiate(woodPrefab, (startPos+currentPos)/2, Quaternion.LookRotation(currentPos-startPos)).transform.localScale =new Vector3(0.5f,0.5f,length);
                  break;
                //	Rotates the drawing direction when encountering '+' or '-'.
                case 'X':
                    startPos = currentPos;
                    Instantiate(leafPrefab, currentPos, currentRot);
                    //Instantiate(leafPrefab, (startPos + currentPos) / 2, Quaternion.LookRotation(currentPos - startPos)).transform.localScale = new Vector3(0.1f, 0.1f, length);

                    break;
                case '+':
                    currentRot *= Quaternion.Euler(0, angle, 0);
                    break;
                case '-':
                    currentRot *= Quaternion.Euler(0, -angle, 0);
                    break;
                //	Saves the current position and rotation on encountering '[' and restores it on ']'
                case '[':
                    transformStack.Push(new TransformInfo(currentPos, currentRot));
                    break;
                case ']':
                    if (transformStack.Count > 0)
                    {
                        TransformInfo ti = transformStack.Pop();
                        currentPos = ti.position;
                        currentRot = ti.rotation;
                        
                    }
                    break;
                default:
                    // Do nothing for uknown character
                    break;
            }
        }

        

    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateLSystem();
        DrawLSystem();
    }

    //Step 5: Completing the TransformInfo Class
    private class TransformInfo
    {
        public Vector3 position;
        public Quaternion rotation;
        public TransformInfo(Vector3 pos, Quaternion rot)
        {
            position = pos;
            rotation = rot;
        }
    }
}
