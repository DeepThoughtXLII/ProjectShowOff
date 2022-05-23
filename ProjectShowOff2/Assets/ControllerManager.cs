using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ControllerManager : MonoBehaviour
{

    enum gameState { LOBBY, INGAME }


    gameState state;

    Dictionary<int, playerInput> players;
    PlayerManager playerManager;

    
    [SerializeField] Image[] playerUIs;
    public Image body;

    private void Awake()
    {
        players = new Dictionary<int, playerInput>();
        playerUIs = new Image[4];
        state = gameState.LOBBY;

              
    }

    private void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            playerUIs[i] = body.transform.GetChild(i).GetComponent<Image>();
        }
    }

    public void HandlePlayerJoin(PlayerInput pi)
    {
        Debug.Log("player joined " + pi.playerIndex);
        pi.transform.SetParent(transform);

        playerInput pIScript = pi.GetComponent<playerInput>();
        pIScript.Index = pi.playerIndex;
        pIScript.UIrep = playerUIs[pi.playerIndex];

        players.Add(pi.playerIndex, pIScript);
        Player player = playerManager.AddPlayer(pi.playerIndex);
        player.gameObject.SetActive(false);

       /* if (!playerConfigs.Any(p => p.PlayerIndex == pi.playerIndex))
        {
            playerConfigs.Add(new PlayerConfiguration(pi));
        }*/
    }

    private void Update()
    {
        if(state == gameState.INGAME)
        {

        }
    }




}
