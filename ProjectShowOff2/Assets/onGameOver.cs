using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class onGameOver : MonoBehaviour
{
    public GameObject serverPrefab;

    Server.Controls controls;


    // Start is called before the first frame update
    void Start()
    {
        Server.onGameOver += DestroyServer;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void OnDestroy()
    {
        //Server.onGameOver -= DestroyServer;
    }

    public void DestroyServer()
    {
        foreach(GameObject g in GameObject.FindGameObjectsWithTag("server"))
        {
            Server s = g.GetComponent<Server>();
            if(s == Server.instance)
            {
                controls = s.UsesControls;
            }
            Destroy(g);
        }
        GameObject server = (GameObject)Instantiate(serverPrefab);
        server.GetComponent<Server>().UsesControls = controls;
    }
}
