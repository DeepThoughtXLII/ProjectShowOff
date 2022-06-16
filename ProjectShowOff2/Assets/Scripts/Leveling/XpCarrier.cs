using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XpCarrier : MonoBehaviour
{
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     FIELDS
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    [SerializeField] private int xpGains = 5;


    PlayerManager playerManager;

    ILevelable killer;


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     START()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void Start()
    {
        playerManager = GameObject.FindGameObjectWithTag("server").GetComponent<PlayerManager>();
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     SET KILLER()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void SetKiller(int id)
    {
        killer = playerManager.GetPlayer(id).GetComponent<ILevelable>();
       
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     ON DESTROY
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void OnDestroy()
    {
        if(killer != null)
        {
            killer.GainXP(xpGains);
        }
    }
}
