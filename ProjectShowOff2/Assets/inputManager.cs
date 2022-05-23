using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class inputManager : MonoBehaviour
{

    [SerializeField]
    bool usingOnlineControllers = true;


    void Awake()
    {
        int numberOfControllers = InputSystem.devices.OfType<Gamepad>().Count();
        Debug.Log("controllers = " + numberOfControllers);
        if (numberOfControllers > 0)
        {
            //ControllerManagement(numberOfControllers);
            usingOnlineControllers = false;
            //controllers = new Dictionary<int, int>();
        }

        if (usingOnlineControllers)
        {
            GetComponent<SimpleServerDemo>().enabled = true;
            GetComponent<ControllerManager>().enabled = false;
        } else
        {
            GetComponent<SimpleServerDemo>().enabled = false;
            GetComponent<ControllerManager>().enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
    void ControllerManagement(int playerCount)
    {
        Gamepad[] gamepads = InputSystem.devices.OfType<Gamepad>().ToArray();
        for (int i = 0; i < playerCount; i++)
        {
            int id = gamepads[i].deviceId;
            Debug.Log("id of controller: " + id);
            //playerManager.AddPlayer(id);           
        }
    }*/
}
