/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com
刷新点 编辑器
 */
using UnityEngine;
using System.Collections;
#if UNITY_EDITOR

using UnityEditor;

//[RequireComponent(typeof(TerrainObjectEnemyBornPointData))]
[CustomEditor(typeof(TerrainObjectEnemyBornPointData))]
//[CanEditMultipleObjects]
public class EnemyBornEdit : Editor
{
    void OnEnable()
    {
    }
    static float time_add = 1;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();

        TerrainObjectEnemyBornPointData obj = target as TerrainObjectEnemyBornPointData;
        if (obj == null) return;

        {
            OneEnemyBornData[] comps = obj.gameObject.GetComponents<OneEnemyBornData>();

            GUILayout.Label("--------该刷新点数据汇总---------");
            GUILayout.Label("--------该修改不会立刻生效,需要重新run---------");

            GUILayout.BeginHorizontal();
            GUILayout.Label("敌人总数:");
            GUILayout.Label(comps.Length.ToString(), GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("自动填充的秒数增量");
            time_add = EditorGUILayout.FloatField(time_add, GUILayout.Width(50));
            GUILayout.EndHorizontal();
        }

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("顺序添加"))
        {
            OneEnemyBornData[] comps = obj.gameObject.GetComponents<OneEnemyBornData>();
            OneEnemyBornData data = obj.gameObject.AddComponent<OneEnemyBornData>();
            data.time = time_add + comps[comps.Length - 1].time;
            serializedObject.ApplyModifiedProperties();
        }

        if (GUILayout.Button("顺序添加10个"))
        {
            OneEnemyBornData[] comps = obj.gameObject.GetComponents<OneEnemyBornData>();
            float t = 0f;
            if (comps.Length > 0)
            {
                t = time_add + comps[comps.Length - 1].time;
            }
            for (int i = 0; i < 10; i++)
            {
                OneEnemyBornData data = obj.gameObject.AddComponent<OneEnemyBornData>();
                data.time = t;
                t += time_add;
            }
            serializedObject.ApplyModifiedProperties();
        }
        if (GUILayout.Button("顺序添加100个"))
        {
            OneEnemyBornData[] comps = obj.gameObject.GetComponents<OneEnemyBornData>();
            float t = 0f;
            if (comps.Length > 0)
            {
                t = time_add + comps[comps.Length - 1].time;
            }
            for (int i = 0; i < 100; i++)
            {
                OneEnemyBornData data = obj.gameObject.AddComponent<OneEnemyBornData>();
                data.time = t;
                t += time_add;
            }
            serializedObject.ApplyModifiedProperties();
        }
        if (GUILayout.Button("倒序删除"))
        {
            Component[] comp = obj.gameObject.GetComponents<OneEnemyBornData>();
            if (comp != null && comp.Length >= 1)
            {
                if (comp[comp.Length - 1] != null)
                {
                    GameObject.DestroyImmediate(comp[comp.Length - 1]);

                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        if (GUILayout.Button("清空"))
        {
            Component[] comp = obj.gameObject.GetComponents<OneEnemyBornData>();
            foreach (Component cm in comp)
            {
                GameObject.DestroyImmediate(cm);
            }
            serializedObject.ApplyModifiedProperties();
        }

        GUILayout.EndHorizontal();

        serializedObject.Update();

    }
}


#else
public class EnemyBornEdit : MonoBehaviour
{

}


#endif