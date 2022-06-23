using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadBoss : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.LoadScene(2);
    }
}
