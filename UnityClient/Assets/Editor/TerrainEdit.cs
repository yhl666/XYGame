using UnityEngine;
using System.Collections;
using UnityEditor;


[RequireComponent(typeof(TerrainMeta))]
[CustomEditor(typeof(TerrainMeta))]
//  [CanEditMultipleObjects]
public class TerrainMetaEditor : Editor
{
    void OnEnable()
    {


    }


    public override void OnInspectorGUI()
    {

        serializedObject.Update();


        DrawDefaultInspector();


        if (GUILayout.Button("Save to file"))
        {
            Debug.Log("save to XX.xml");
        }

        serializedObject.ApplyModifiedProperties();
    }
 
}