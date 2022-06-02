using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class PlayerManager : MonoBehaviour
{


    [SerializeField]private Player playerPrefab = null;

    [SerializeField] Dictionary<int, Player> playerList = new Dictionary<int, Player>();

    public Transform[] spawnpoints;

    Server server;


    private void Start()
    {
        spawnpoints = new Transform[4];

        server = GetComponent<Server>();
        int numberOfControllers = InputSystem.devices.OfType<Gamepad>().Count();
        Debug.Log("controllers = " + numberOfControllers);
        if (numberOfControllers > 0)
        {

        }

        switch (server.UsesControls)
        {
            case Server.Controls.ONLINE:
                playerPrefab.IsUsingInput = Player.Input.ONLINE;

                break;
            case Server.Controls.GAMEPAD:
                playerPrefab.IsUsingInput = Player.Input.GAMEPAD;

                break;
            case Server.Controls.KEYBOARD:
                playerPrefab.IsUsingInput = Player.Input.KEYBOARD;

                break;
        }
    }

    public Player AddPlayer(int pId, string name = null)
    {
        if (HasPlayerAlready(pId))
        {
            throw new ArgumentException($"Cannot add player with id {pId}, already exists.");
        }
        if(name == null)
        {
            name = "Player" + pId;
        }
        if(spawnpoints[0] == null)
        {
           
        }

        //create a new view with ourselves as the transform parent
        Player player = Instantiate<Player>(playerPrefab, transform);
        playerList[pId] = player;
        player.name = name;
        player.Id = pId;
        //player.Spawn = new Vector2(spawnpoints[pId].position.x, spawnpoints[pId].position.x);
        return player;
    }

    public bool HasPlayerAlready(int pId)
    {
        return playerList.ContainsKey(pId);
    }

    public Player GetPlayer(int pId)
    {
        if (!HasPlayerAlready(pId))
        {
            throw new Exception($"Avatar with key {pId} not found.");
        }
        return playerList[pId];
    }

    public int GetPlayerCount()
    {
        return playerList.Count;
    }


    public void RemovePlayer(int pId)
    {
        Player player = GetPlayer(pId);
        player.Remove();
        playerList.Remove(pId);
    }

    public void MovePlayer(Vector2 direction, int pId)
    {
        Player player = GetPlayer(pId);
        player.Move(direction);
    }

    public bool PlayersAllDead()
    {
        foreach (KeyValuePair < int, Player> p in playerList)
        {
            if(p.Value.State != Player.PlayerState.REVIVING)//if any of the players are not revivng/not dead
            {
                return false;
            }
        }
        return true;
    }
}
