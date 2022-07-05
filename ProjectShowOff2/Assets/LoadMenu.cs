using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class LoadMenu : MonoBehaviour
{

    public static event Action onGameEnd;

    private void OnEnable()
    {
        onGameEnd();
        //SceneManager.LoadScene(0);
    }
}
