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

    [SerializeField] Dictionary<int, playerShooting> playerShot = new Dictionary<int, playerShooting>();

    Dictionary<int, Levelable> playerLevel = new Dictionary<int, Levelable>();

   // List<string> disconnectedPlayers = new List<string>();
    Dictionary<string, int> disconnectedPlayers = new Dictionary<string, int>();

    Player Boss = null;
    Boss BossShoot = null;
    public GameObject bossBulletPrefab;

    public Transform[] spawnpoints;

    Server server;


    private void Start()
    {
        


        spawnpoints = new Transform[4];

        server = GetComponent<Server>();


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

    public void DisconnectPlayer(int id)
    {
        Player p = GetPlayer(id);
        p.Disconnected();
        disconnectedPlayers.Add(p.name, id);
    }


    public bool IsOldPlayer(string name)
    {
        if (disconnectedPlayers.ContainsKey(name)){
            return true;
        }
        return false;
    }

    public int getOldPlayerID(string name)
    {
        return disconnectedPlayers[name];
    }

    public void ReconnectPlayer(int id)
    {
        GetPlayer(id).Reconnected();
        Debug.LogError("reconnected player with id= " +id);
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
        playerShot.Add(pId, player.GetComponent<playerShooting>());
        playerLevel.Add(pId, player.GetComponent<Levelable>());
        player.name = name;
        player.Id = pId;
        Debug.Log("added player to manage with id= " + pId);
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


    public void PlayerShoot(int id)
    {
        if(GetPlayer(id).State != Player.PlayerState.BOSS)
        {
            playerShot[id].Shoot();
        } else
        {
           
            BossShoot.CanShoot();
        }

    }

    public bool PlayersAllDead()
    {
        foreach (KeyValuePair < int, Player> p in playerList)
        {
            if(p.Value.State != Player.PlayerState.REVIVING && p.Value.State != Player.PlayerState.BOSS)//if any of the players are not revivng/not dead
            {
                return false;
            }
        }
        return true;
    }


    public bool checkForPlayerUpgrades(int pId)
    {
        if (playerLevel[pId].upgradesAvailable)
        {
            return true;
        }
        return false;
    }

    public void changeUpgradeType(int pId, Upgrade.UpgradeType type)
    {
        playerLevel[pId].ChooseUpgrade(type);
    }

    public void HeavenFallsNextBoss()
    {
        int highestLevel = 0;
        int potentialBoss = -1;
        foreach (KeyValuePair<int, Levelable> player in playerLevel)
        {
            if(potentialBoss == -1 || player.Value.Level.id > highestLevel)
            {
                highestLevel = player.Value.Level.id;
                potentialBoss = player.Key;
            }          
        }
        Boss = GetPlayer(potentialBoss);
        BossShoot = Boss.gameObject.AddComponent<Boss>();
        Boss.bossMode();
        Boss.gameObject.GetComponent<playerShooting>().enabled = false;
        BossShoot.bulletPrefab = bossBulletPrefab;
        //playerList.Remove(potentialBoss);
    }


    public void PlayersReset()
    {
        foreach(KeyValuePair<int, Player> p in playerList)
        {
            Destroy(p.Value.gameObject);
        }
        playerList.Clear();
        playerShot.Clear();
        playerLevel.Clear();
        Boss = null;
        BossShoot = null;
    }
}
