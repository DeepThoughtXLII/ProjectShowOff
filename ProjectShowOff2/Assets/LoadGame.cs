using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadGame : MonoBehaviour
{

    public Slider slider;
    public GameObject loadScreen;


    private void OnEnable()
    {
        loadScreen.SetActive(true);
        GameObject.FindGameObjectWithTag("server").GetComponent<Server>().LoadSceneWithLoadingScreen("Game", slider);
    }
}
