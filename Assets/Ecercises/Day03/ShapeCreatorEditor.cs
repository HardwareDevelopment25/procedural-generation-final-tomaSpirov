using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShapeCreator))]
public class ShapeCreatorEditor : Editor
{
    // Override the default Inspector GUI Rendering

    public override void OnInspectorGUI()
    {
        // Cast the target object to MazeGenerator so we can access its fields and methods
        ShapeCreator mapGen = (ShapeCreator)target;
        GUILayout.Label("---Configure Maze---", EditorStyles.largeLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        // Draw the default inspector UI for all serialised fields
        if (DrawDefaultInspector())
        {
            //switches to 2D and mazkes new maze;

           if(mapGen.autoUpdate)
            mapGen.DrawMapInEditor();
        }

        //Add a button to do something, regenerate.

        if (GUILayout.Button(" GENERATE A NEW MAP "))
        {
           mapGen.DrawMapInEditor();
        }

    }
}


