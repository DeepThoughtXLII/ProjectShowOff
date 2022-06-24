using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMenu : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.LoadScene(0);
    }
}
