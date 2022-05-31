using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(LevelManager))]
public class LevelManagerEditor : Editor
{



    SerializedProperty m_Upgrade;
    SerializedProperty m_toggleUpgrade;
    SerializedProperty m_levels;


    private void OnEnable()
    {
       // m_Upgrade = serializedObject.FindProperty("upgrade");
        //m_toggleUpgrade = serializedObject.FindProperty("hasUpgrade");
        //m_levels = serializedObject.FindProperty("levels");

    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        LevelManager levelManager = (LevelManager)target;

        //EditorGUILayout.PropertyField(m_levels, new GUIContent("levels"));



        if (GUILayout.Button("AddLevel"))
        {
            Level l = levelManager.AddLevel();
            
        }



        //serializedObject.ApplyModifiedProperties();

    }
    


}
