using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



[CustomEditor(typeof(Levelable))]
public class EditorLevelable : Editor
{

    int xp = 5;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Levelable levelable = (Levelable)target;
        

        if (GUILayout.Button("AddXp"))
        {
            xp = int.Parse(GUILayout.TextField(xp.ToString(), 4));
            levelable.GainXP(xp);

        }





    }
}
