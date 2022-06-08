using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class ControllerManager : MonoBehaviour, IControllerManager
{

    Server server;
    Dictionary<int, playerInput> players;

   
    bool keyboardTesting = true;

    PlayerManager playerManager;

    PlayerInputManager playerInputManager;


    private void Start()
    {
        server = GetComponent<Server>();
        playerInputManager = GetComponent<PlayerInputManager>();
        playerManager = GetComponent<PlayerManager>();
        players = new Dictionary<int, playerInput>();

        if(server.UsesControls == Server.Controls.KEYBOARD)
        {
            keyboardTesting = true;
        } else
        {
            keyboardTesting = false;
        }
    }

    private void Update()
    {
        if (server.State == Server.gameState.LOBBY)
        {
            if (keyboardTesting)
            {
                if (!players.ContainsKey(0) || !players.ContainsKey(1))
                {
                    JoinPlayers();
                }
            }
            else
            {
                playerInputManager.joinBehavior = PlayerJoinBehavior.JoinPlayersWhenButtonIsPressed;
            }
        } else if(server.State == Server.gameState.INGAME)
        {
            
        }
    }

    public void HandlePlayerJoin(PlayerInput pi)
    {      
            Debug.Log("player joined " + pi.playerIndex);
            pi.transform.SetParent(transform);
            AddInput(pi);
    }

    public void AddInput(PlayerInput pi)
    {
        if (!players.ContainsKey(pi.playerIndex))
        {
            playerInput pIScript = pi.transform.GetComponent<playerInput>();
            pi.transform.position = Vector3.zero;
            pIScript.Index = pi.playerIndex;
            pIScript.UIrep = server.GetPlayerUILobby(pi.playerIndex); 

            players.Add(pi.playerIndex, pIScript);
        }
    }


    public void OnGameStart()
    {
        foreach (KeyValuePair<int, playerInput> player in players)
        {
            player.Value.UIrep = null;
            Player p = playerManager.AddPlayer(player.Key);
            p.transform.position = Vector3.zero;
            player.Value.transform.SetParent(p.transform);
            p.GetComponent<Player>().enabled = true;
            //p.transform.SetParent(player.Value.transform);
        }
    }

    public void JoinPlayers()
    {
        if (Keyboard.current.wKey.wasPressedThisFrame && !players.ContainsKey(0))
        {
            playerInputManager.JoinPlayer(0, 0, "keyboard", InputSystem.GetDevice<Keyboard>());
        }
        else if (Keyboard.current.iKey.wasPressedThisFrame && !players.ContainsKey(1))
        {
            playerInputManager.JoinPlayer(1, 0, "keyboard2", InputSystem.GetDevice<Keyboard>());
        }
    }

    public int GetControllerCount()
    {
        return players.Count;
    }
}