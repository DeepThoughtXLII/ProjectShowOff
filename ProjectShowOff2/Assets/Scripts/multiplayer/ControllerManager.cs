using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class ControllerManager : MonoBehaviour, IControllerManager
{
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     FIELDS
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    Server server;
    Dictionary<int, playerInput> players;

   
    bool keyboardTesting = true;

    PlayerManager playerManager;

    PlayerInputManager playerInputManager;

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     START ()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void Start()
    {
        server = GetComponent<Server>();
        playerInputManager = GetComponent<PlayerInputManager>();
        playerManager = GetComponent<PlayerManager>();
        players = new Dictionary<int, playerInput>();

        if(server.UsesControls == Controls.KEYBOARD)
        {
            keyboardTesting = true;
        } else
        {
            keyboardTesting = false;
        }
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     UPDATE()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
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


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     HANDLE PLAYER JOIN()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void HandlePlayerJoin(PlayerInput pi)
    {      
            Debug.Log("player joined " + pi.playerIndex);
            pi.transform.SetParent(transform);
            AddInput(pi);
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     ADD INPUT()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void AddInput(PlayerInput pi)
    {
        if (pi != null)
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
    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     ON GAME START()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void OnGameStart()
    {
        foreach (KeyValuePair<int, playerInput> player in players)
        {
            player.Value.UIrep = null;
            Player p = playerManager.AddPlayer(player.Key);
            p.transform.position = Vector3.zero;
            player.Value.transform.SetParent(p.transform);
            player.Value.transform.SetAsLastSibling();
            p.GetComponent<Player>().enabled = true;
            //p.transform.SetParent(player.Value.transform);
        }
    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     JOIN PLAYERS()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
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

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     GET CONTROLLER COUNT()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public int GetControllerCount()
    {
        return players.Count;
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     RESET CONTROLLERS()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void ResetControllers()
    {
       // Dictionary<int, playerInput> players;
        foreach(KeyValuePair<int, playerInput> p in players)
        {
            Destroy(p.Value.gameObject);
        }
        players.Clear();
    }
}
