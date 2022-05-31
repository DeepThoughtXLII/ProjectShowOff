using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    PlayerManager playerManager;
    [SerializeField] PlayerUI [] playerUI = new PlayerUI[4];

    private void Awake()
    {
        findPlayerUI();
        playerManager = GameObject.FindGameObjectWithTag("server").GetComponent<PlayerManager>();
        for(int i = 0; i < playerManager.GetPlayerCount(); i++)
        {
            playerUI[i].Player = playerManager.GetPlayer(i);
           // playerUI[i].MaxHpRecalc();
        }
        foreach(PlayerUI ui in playerUI)
        {
            if(ui.Player == null)
            {
                ui.transform.gameObject.SetActive(false);
            }
        }
    }

    private void findPlayerUI()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            playerUI[i] = transform.GetChild(i).GetComponent<PlayerUI>();
        }
    }

}
