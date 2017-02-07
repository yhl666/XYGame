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

            TerrainMeta meta = block.serializedObject.targetObject as TerrainMeta;
            meta.blocks[0] = TerrainBlock.Create(10.0f, 10.0f, 1.0f);

            //pros.floatValue = 5;
            block.serializedObject.ApplyModifiedProperties();



        }

        serializedObject.ApplyModifiedProperties();
    }
    private void Methodd()
    {
        Debug.Log("1111111");

        SerializedProperty pros = serializedObject.FindProperty("blocks");


        SerializedProperty block = pros.GetArrayElementAtIndex(0);

        TerrainMeta meta = block.serializedObject.targetObject as TerrainMeta;
        TerrainBlock b = meta.blocks[0];

        Debug.Log(b.x_left);

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
