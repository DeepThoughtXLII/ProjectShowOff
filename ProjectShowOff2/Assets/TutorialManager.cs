using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    

    public GameObject[] popUps;
    private int popUpIndex;

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < popUps.Length; i++)
        {
            if (i == popUpIndex)
            {
                popUps[i].SetActive(true);
            }
            else
            {
                popUps[i].SetActive(false);
            }
        }

        if (popUpIndex == 0)
        {
            popUps[5].SetActive(true);
            if (Input.GetKeyDown(KeyCode.F))
            {
                popUpIndex++;
            }

        }
        else if (popUpIndex == 1)
        {
            popUps[5].SetActive(true);
            if (Input.GetKeyDown(KeyCode.F))
            {
                
                popUpIndex++;
            }

        }
        else if (popUpIndex == 2)
        {
            popUps[5].SetActive(true);
            if (Input.GetKeyDown(KeyCode.F))
            {
                popUpIndex++;
            }

        }
        else if (popUpIndex == 3)
        {
            popUps[5].SetActive(true);

            if (Input.GetKeyDown(KeyCode.F))
            {
                popUpIndex++;
            }
        }
        else if (popUpIndex == 4)
        {
            popUps[5].SetActive(true);

            if (Input.GetKeyDown(KeyCode.F))
            {
                popUpIndex++;
            }
        }
        else if (popUpIndex == 5)
        {
            popUps[5].SetActive(true);

            if (Input.GetKeyDown(KeyCode.F))
            {
                popUps[5].SetActive(false);
                Destroy(gameObject);
            }
        }


    }
}


