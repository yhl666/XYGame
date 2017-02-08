﻿using UnityEngine;
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


        if (GUILayout.Button("Save to file"))
        {
            Debug.Log("save to XX.xml");
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
