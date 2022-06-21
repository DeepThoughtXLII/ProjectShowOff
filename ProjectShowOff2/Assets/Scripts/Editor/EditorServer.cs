using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(Server))]

public class EditorServer : Editor
{


    public override void OnInspectorGUI()
    {

        Server server = (Server)target;

        base.OnInspectorGUI();


        if (GUILayout.Button("AddPlayers"))
        {
            
            server.testingWithPlayers();








        }



    }


}
