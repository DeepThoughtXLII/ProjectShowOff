using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class endingManager : MonoBehaviour
{

    public Image deathPreBoss;
    public Image deathDuringBoss;
    public Image playersWin;

    public GameObject Hell;
    public GameObject Heaven;

    Server server;


    private void Awake()
    {
        server = GameObject.Find("server").GetComponent<Server>();
        if(server.ending == Server.endingType.DeathPreBoss)
        {
            deathPreBoss.gameObject.SetActive(true);
            deathDuringBoss.gameObject.SetActive(false);
            playersWin.gameObject.SetActive(false);
            Heaven.SetActive(true);
            Hell.SetActive(false);
        }
        else if (server.ending == Server.endingType.DeathDuringBoss)
        {
            deathPreBoss.gameObject.SetActive(false);
            deathDuringBoss.gameObject.SetActive(true);
            playersWin.gameObject.SetActive(false);
            Heaven.SetActive(false);
            Hell.SetActive(true);
        }
        else if (server.ending == Server.endingType.Won)
        {
            deathPreBoss.gameObject.SetActive(false);
            deathDuringBoss.gameObject.SetActive(false);
            playersWin.gameObject.SetActive(true);
            Heaven.SetActive(false);
            Hell.SetActive(true);
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
