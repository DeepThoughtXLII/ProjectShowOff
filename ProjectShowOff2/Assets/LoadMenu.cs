using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMenu : MonoBehaviour
{
    private void OnEnable()
    {
        Server server = GameObject.FindGameObjectWithTag("server").GetComponent<Server>();
        server.BackToLobby();
        //SceneManager.LoadScene(0);
    }
}
