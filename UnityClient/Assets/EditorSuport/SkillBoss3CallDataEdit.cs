/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com
Boss 召唤技能 编辑器
 */
using UnityEngine;
using System.Collections;
#if UNITY_EDITOR

using UnityEditor;

//[RequireComponent(typeof(TerrainObjectEnemyBornPointData))]
[CustomEditor(typeof(SkillBoss_3_Data))]
///[CanEditMultipleObjects]
public class SkillBoss3CallDataEdit : Editor
{
    void OnEnable()
    {
    }
    static float time_add = 1;
    static string type = "202001";// 原型类型
    static int count = 10;//数量
    static int id = 1;//出生点id
    static int level = 1; // 召唤次数
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();

        SkillBoss_3_Data obj = target as SkillBoss_3_Data;
        if (obj == null) return;

        int max_level = 0;

        {
            SkillBoss_3_CallData[] comps = obj.gameObject.GetComponents<SkillBoss_3_CallData>();
            int[] count_levels = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            if (obj.call_times == null) return;

            max_level = obj.call_times.Length;
      
            foreach (SkillBoss_3_CallData data in comps)
            {
                count_levels[data.call_level - 1]++;
            }
            GUILayout.Label("-------召唤数据数据汇总---------");

            for (int i = 0; i < max_level; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("第 " + (i + 1).ToString() + " 波召唤总数:");
                GUILayout.Label(count_levels[i].ToString(), GUILayout.Width(50));
                GUILayout.EndHorizontal();

            }
            GUILayout.Label("-------批量生成设置---------");

            GUILayout.BeginHorizontal();
            GUILayout.Label("每个单位时间增量");
            time_add = EditorGUILayout.FloatField(time_add, GUILayout.Width(100));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("怪物原型");
            type = EditorGUILayout.TextField(type, GUILayout.Width(100));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("生成个数");
            count = EditorGUILayout.IntField(count, GUILayout.Width(100));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("出生点id");
            id = EditorGUILayout.IntField(id, GUILayout.Width(100));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("召唤波数");
            level = EditorGUILayout.IntField(level, GUILayout.Width(100));
            GUILayout.EndHorizontal();


        }

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("顺序批量添加"))
        {
            if (0 == max_level)
            {
                EditorUtility.DisplayDialog("错误", "请先设置Boss技能的Call_times的Size(召唤波数)", "确定");
                return;
            }
            if (level > max_level)
            {
                EditorUtility.DisplayDialog("错误", "召唤波数不能大于Boss技能最大波数", "确定");
                return;
            }
       
            SkillBoss_3_CallData[] comps = obj.gameObject.GetComponents<SkillBoss_3_CallData>();
            float t = 0f;
            if (comps.Length > 0)
            {
                t = time_add + comps[comps.Length - 1].time;
            }
            for (int i = 0; i < count; i++)
            {
                SkillBoss_3_CallData data = obj.gameObject.AddComponent<SkillBoss_3_CallData>();
                data.time = t;
                data.code = type;
                data.call_level = level;
                data.id = id;
                t += time_add;
            }
            serializedObject.ApplyModifiedProperties();
        }
        if (GUILayout.Button("顺序添加一个"))
        {
            if (0 == max_level)
            {
                EditorUtility.DisplayDialog("错误", "请先设置Boss技能的Call_times的Size(召唤波数)", "确定");
                return;
            }
            if (level > max_level)
            {
                EditorUtility.DisplayDialog("错误", "召唤波数不能大于Boss技能最大波数", "确定");
                return;
            }

            SkillBoss_3_CallData[] comps = obj.gameObject.GetComponents<SkillBoss_3_CallData>();
            float t = 0f;
            if (comps.Length > 0)
            {
                t = time_add + comps[comps.Length - 1].time;
            }
            for (int i = 0; i < 1; i++)
            {
                SkillBoss_3_CallData data = obj.gameObject.AddComponent<SkillBoss_3_CallData>();
                data.time = t;
                data.code = type;
                data.call_level = level;
                data.id = id;
                t += time_add;
            }
            serializedObject.ApplyModifiedProperties();
        }

        if (GUILayout.Button("倒序删除"))
        {
            Component[] comp = obj.gameObject.GetComponents<SkillBoss_3_CallData>();
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
            Component[] comp = obj.gameObject.GetComponents<SkillBoss_3_CallData>();
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
public class SkillBoss3CallDataEdit : MonoBehaviour
{

}


#endif