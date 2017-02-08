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


        SerializedProperty pros = serializedObject.FindProperty("blocks");


        if (GUILayout.Button("add point"))
        {
            int size = pros.arraySize;

            pros.arraySize = size + 1;
            SerializedProperty block = pros.GetArrayElementAtIndex(0);

           // TerrainMeta meta = block.serializedObject.targetObject as TerrainMeta;
    
            //pros.floatValue = 5;
            block.serializedObject.ApplyModifiedProperties();

        }

        serializedObject.ApplyModifiedProperties();
    }
 
}
public class TerrainEdit : Editor
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
