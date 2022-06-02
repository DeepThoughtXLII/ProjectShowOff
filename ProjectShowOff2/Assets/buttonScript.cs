using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonScript : MonoBehaviour
{

    GameObject server;

    // Start is called before the first frame update
    void Start()
    {
        server = GameObject.FindGameObjectWithTag("server");
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void NewGame()
    {
        Destroy(server);
    }
}
