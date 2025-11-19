using System.Collections.Generic;
using System.Data;
using System.Text;
using UnityEngine;

public class LSystem : MonoBehaviour
{
    public string axiom = "F";
    public float angle = 45;
    public int iterations = 3; 
    public string[] laws;

    public string currentString;

    [SerializeField]
    private Dictionary<char, string>rules = new Dictionary<char, string>();


    private void Awake()
    {
        foreach (var law in laws) 
        {
            var l= law.Split("->");
            rules.Add(l[0][0], l[1]);

        }
        currentString = axiom;
        GenerateSystemString();
    }

    private void GenerateSystemString() 
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
        Debug.Log(currentString);
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
