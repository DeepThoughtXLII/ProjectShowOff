using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGame : MonoBehaviour
{
    private void OnEnable()
    {
        GameObject.FindGameObjectWithTag("server").GetComponent<Server>().changeScene("Game");
    }
}
