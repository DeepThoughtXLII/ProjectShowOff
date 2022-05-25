using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class ControllerManager : MonoBehaviour
{

    Server server;
    

    private void Start()
    {
        server = GetComponent<Server>();
    }

    public void HandlePlayerJoin(PlayerInput pi)
    {
        
            Debug.Log("player joined " + pi.playerIndex);
            pi.transform.SetParent(transform);
            server.AddInput(pi);
            

            //playerManager.AddPlayer(pi.playerIndex);
            //player.gameObject.SetActive(false);
        

    }

   


}
