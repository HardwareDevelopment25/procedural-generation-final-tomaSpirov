using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MazeGenerator03))]
public class MazeGeneratorEditor : Editor
{
    // Override the default Inspector GUI Rendering

    public override void OnInspectorGUI()
    {
        // Cast the target object to MazeGenerator so we can access its fields and methods
        MazeGenerator03 mazeGen = (MazeGenerator03)target;
        GUILayout.Label("---Configure Maze---", EditorStyles.largeLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        // Draw the default inspector UI for all serialised fields
        if (DrawDefaultInspector())
        {
            //switches to 2D and mazkes new maze;

            if (mazeGen.size < 16 || mazeGen.size > 1000)
            {
                mazeGen.size = 32;
            }
            mazeGen.GenerateImageOfNewMaze();
        }

        //Add a button to do something, regenerate.

        if (GUILayout.Button(" GENERATE A NEW MAZE "))
        {
            mazeGen.GenerateImageOfNewMaze();
        }

    }
}

// Define a custom editor window for generate complete maze Gameobjects where ever we like

public class MazeGeneratorWindow : EditorWindow
{
    public int InitialMazeSize = 32;

    // This generates a menu item inside of unity across the project
    [MenuItem("Tools/Generate Maze By Size")]

    public static void ShowWindow()
    {
        GetWindow<MazeGeneratorWindow>();
    }

    // Lets decorate our window and create this new game obejct with a maze anywhere we like.
    private void OnGUI()
    {

        EditorGUILayout.Space();
        GUILayout.Label(" Maze Generator, will Create maze in your scene ");
        EditorGUILayout.Space();
        GUILayout.Label(" Configure Maze: ");

        // creates a input field in case we want to choose defaul maze size
        InitialMazeSize = EditorGUILayout.IntField("Size: ", InitialMazeSize);

        // once button is pressed we run a thread to make out maze in editor
        if (GUILayout.Button("Generate"))
        {
            // create a new gameobject
            GameObject newGameObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            newGameObject.name = InitialMazeSize + "x" + InitialMazeSize + "Generated Maze";

            Undo.RegisterCreatedObjectUndo(newGameObject, "Undo Create Object");
            // crate material and add a shader
            Material mat = new Material(Shader.Find("Unlit/Texture"));
            // Adding a component also returns its refrence.
            MazeGenerator03 mazeGen = newGameObject.AddComponent<MazeGenerator03>();


            // get the meshrenderer put on the material and run the generation ofthe maze.
            newGameObject.GetComponent<MeshRenderer>().material = mat;

            mazeGen.size = InitialMazeSize;
            mazeGen.GenerateImageOfNewMaze();


        }

    }

}
