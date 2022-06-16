using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     FIELDS
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    PlayerManager playerManager;
    [SerializeField] PlayerUI [] playerUI = new PlayerUI[4];




    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     AWAKE()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void Awake()
    {
        findPlayerUI();
        playerManager = GameObject.FindGameObjectWithTag("server").GetComponent<PlayerManager>();

        int uiCount = 0;
        for(int i = 0; i < playerManager.GetPlayerCount(); i++)
        {
            Player p = playerManager.GetPlayer(i);
            if(p.GetPlayerHealth().State != PlayerHealth.PlayerState.BOSS)
            {
                playerUI[uiCount].Player = p;
                uiCount++;
            }
            else
            {
                transform.parent.GetComponentInChildren<BossUI>().Player = p;
            }

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


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     FIND PLAYER UI()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void findPlayerUI()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            playerUI[i] = transform.GetChild(i).GetComponent<PlayerUI>();
        }
    }



}
