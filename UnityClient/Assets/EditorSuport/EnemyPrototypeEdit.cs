/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com
怪物原型 编辑器
 */
using UnityEngine;
using System.Collections;
#if UNITY_EDITOR

using UnityEditor;

//[RequireComponent(typeof(EnemyPrototypeEditor))]
[CustomEditor(typeof(EnemyPrototypeEditor))]
public class EnemyPrototypeEdit : Editor
{
    static int damage = 200;
    static int exp = 100;
    static int hp = 100;
    static string code="202001";
    static EnemyProtoType type = EnemyProtoType.小怪1;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();

        EnemyPrototypeEditor obj = target as EnemyPrototypeEditor;
        if (obj == null) return;

        {
            EnemyPrototype[] comps = obj.gameObject.GetComponents<EnemyPrototype>();

            GUILayout.Label("--------怪物原型编辑器---------");
            GUILayout.Label("--------生成设置---------");

            GUILayout.BeginHorizontal();
            GUILayout.Label("怪物Code");
            code = EditorGUILayout.TextField(code, GUILayout.Width(100));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("血量hp");
            hp = EditorGUILayout.IntField(hp, GUILayout.Width(100));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("经验exp");
            exp = EditorGUILayout.IntField(exp, GUILayout.Width(100));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("基础伤害damage");
            damage = EditorGUILayout.IntField(damage, GUILayout.Width(100));
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Label("类型type");
            type = (EnemyProtoType)EditorGUILayout.EnumPopup("", type, GUILayout.Width(100)); // 单选
            GUILayout.EndHorizontal();
      
        }

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("添加一个原型"))
        {
            EnemyPrototype data = obj.gameObject.AddComponent<EnemyPrototype>();
            data.hp = hp;
            data.type = type;
            data.damage = damage;
            data.exp = exp;
            data.code = code;
            serializedObject.ApplyModifiedProperties();
        }


        if (GUILayout.Button("倒序删除一个原型"))
        {
            Component[] comp = obj.gameObject.GetComponents<EnemyPrototype>();
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
            Component[] comp = obj.gameObject.GetComponents<EnemyPrototype>();
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
public class EnemyPrototypeEdit : MonoBehaviour
{

}


#endif