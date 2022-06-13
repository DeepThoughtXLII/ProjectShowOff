using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XpCarrier : MonoBehaviour
{

    [SerializeField] private int xpGains = 5;


    PlayerManager playerManager;

    ILevelable killer;

    private void Start()
    {
        playerManager = GameObject.FindGameObjectWithTag("server").GetComponent<PlayerManager>();
    }

    public void SetKiller(int id)
    {
        killer = playerManager.GetPlayer(id).GetComponent<ILevelable>();
       
    }

    private void OnDestroy()
    {
        if(killer != null)
        {
            killer.GainXP(xpGains);
        }
    }
}
