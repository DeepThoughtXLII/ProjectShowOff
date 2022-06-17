using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class PlayerManager : MonoBehaviour
{
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     FIELDS
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    [SerializeField]private Player [] playerPrefab = new Player[4];

    [SerializeField] Dictionary<int, Player> playerList = new Dictionary<int, Player>();

    //[SerializeField] Dictionary<int, playerShooting> playerShot = new Dictionary<int, playerShooting>();

    //Dictionary<int, Levelable> playerLevel = new Dictionary<int, Levelable>();

   // List<string> disconnectedPlayers = new List<string>();
    Dictionary<string, int> disconnectedPlayers = new Dictionary<string, int>();

    Player Boss = null;
    Boss BossShoot = null;
    public GameObject bossBulletPrefab;

    public Transform[] spawnpoints;
    public GameObject spawn;

    Server server;

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     START()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void Start()
    {
        


        spawnpoints = new Transform[4];

        server = GetComponent<Server>();

        /*
        switch (server.UsesControls)
        {
            case Controls.ONLINE:
                playerPrefab.GetPlayerMovement().IsUsingInput = Controls.ONLINE;
 
                break;
            case Controls.GAMEPAD:
                playerPrefab.GetPlayerMovement().IsUsingInput = Controls.GAMEPAD;

                break;
            case Controls.KEYBOARD:
                playerPrefab.GetPlayerMovement().IsUsingInput = Controls.KEYBOARD;

                break;
        }*/
    }

    private void Update()
    {
        if(server.State == Server.gameState.INGAME && spawn == null)
        {
            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(1))
            {
                Debug.Log("scene lodaded");
                SpawnPlayers();
            }
        }
    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     DISCONNECT PLAYER()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void DisconnectPlayer(int id)
    {
        Player p = GetPlayer(id);
        p.Disconnected();
        disconnectedPlayers.Add(p.name, id);
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     IS OLD PLAYER()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///checks if the name that was input is the name of a player that was disconnected
    public bool IsOldPlayer(string name)
    {
        if (disconnectedPlayers.ContainsKey(name)){
            return true;
        }
        return false;
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     GET OLD PLAYER ID()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///return sth eid of a disconnected player by its name
    public int getOldPlayerID(string name)
    {
        return disconnectedPlayers[name];
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     RECONNECT PLAYER ()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void ReconnectPlayer(int id)
    {
        GetPlayer(id).Reconnected();
        Debug.LogError("reconnected player with id= " +id);
    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     ADD PLAYER()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
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
        Player player = Instantiate<Player>(playerPrefab[pId], transform);
        playerList[pId] = player;
        //playerShot.Add(pId, player.GetComponent<playerShooting>());
        //playerLevel.Add(pId, player.GetComponent<Levelable>());
        player.name = name;
        player.Id = pId;
        player.GetPlayerMovement().IsUsingInput = server.UsesControls;
        Debug.Log("added player to manage with id= " + pId);
        //player.Spawn = new Vector2(spawnpoints[pId].position.x, spawnpoints[pId].position.x);
        return player;
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     HAS PLAYER ALREADY()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public bool HasPlayerAlready(int pId)
    {
        return playerList.ContainsKey(pId);
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     GET PLAYER()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public Player GetPlayer(int pId)
    {
        if (!HasPlayerAlready(pId))
        {
            throw new Exception($"Avatar with key {pId} not found.");
        }
        return playerList[pId];
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     GET PLAYER COUNT()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public int GetPlayerCount()
    {
        return playerList.Count;
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     REMOVE PLAYER()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void RemovePlayer(int pId)
    {
        Player player = GetPlayer(pId);
        player.Remove();
        playerList.Remove(pId);
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     MOVE PLAYER()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void MovePlayer(Vector2 direction, int pId)
    {
        Player player = GetPlayer(pId);
        player.GetPlayerMovement().Move(direction);
    }



    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     PLAYER SHOOT()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void PlayerShoot(int id)
    {
        if(GetPlayer(id).GetPlayerHealth().State != PlayerHealth.PlayerState.BOSS)
        {
            playerList[id].GetPlayerShooting().Shoot();
        } else
        {
           
            BossShoot.CanShoot();
        }

    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     PLAYERS ALL DEAD()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ////checks if all players are alive
    public bool PlayersAllDead()
    {
        foreach (KeyValuePair < int, Player> p in playerList)
        {
            if(p.Value.GetPlayerHealth().State != PlayerHealth.PlayerState.REVIVING && p.Value.GetPlayerHealth().State != PlayerHealth.PlayerState.BOSS)//if any of the players are not revivng/not dead
            {
                return false;
            }
        }
        return true;
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     REVIVE ALL PLAYERS()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void ReviveAllPlayers()
    {
        foreach(KeyValuePair<int, Player> player in playerList)
        {
            //player.Value.GetPlayerHealth().State = PlayerHealth.PlayerState.ALIVE;
            player.Value.GetPlayerHealth().Revive();
        }
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     CHEKC FOR PLAYER UPGRADES()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///checks if upgrades are ready so OnlinControllerManager knows to send messages to the client and enable upgradeButtons etc.
    public bool checkForPlayerUpgrades(int pId)
    {
        if (playerList[pId].GetPlayerLevel().upgradesAvailable)
        {
            return true;
        }
        return false;
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     CHANGE UPGRADE TYPE()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///chooses the upgrade the player decided on
    public void changeUpgradeType(int pId, Upgrade.UpgradeType type)
    {
        playerList[pId].GetPlayerLevel().ChooseUpgrade(type);
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     HEAVEN FALLS NEXT BOSS()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    /////calculates which player should turn into the boss upon fulfilling the conditions for a boss fight
    public void HeavenFallsNextBoss()
    {
        int highestLevel = 0;
        int potentialBoss = -1;
        foreach (KeyValuePair<int, Player> player in playerList)
        {
            if(potentialBoss == -1 || player.Value.GetPlayerLevel().Level.id > highestLevel)
            {
                highestLevel = player.Value.GetPlayerLevel().Level.id;
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


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     PLAYERS RESET()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void PlayersReset()
    {
        foreach(KeyValuePair<int, Player> p in playerList)
        {
            Destroy(p.Value.gameObject);
        }
        playerList.Clear();
        //playerShot.Clear();
        //playerLevel.Clear();
        Boss = null;
        BossShoot = null;
    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     SAVE SPAWN POINTS()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void saveSpawnPoints()
    {
            spawn = GameObject.Find("spawnPoints");

            for (int i = 0; i < spawn.transform.childCount; i++)
            {
                spawnpoints[i] = spawn.transform.GetChild(i);
            }
        
    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                    SPAWN PLAYERS()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public void SpawnPlayers()
    {
        saveSpawnPoints();
        for (int i = 0; i < playerList.Count; i++)
        {
            playerList[i].transform.position = spawnpoints[i].position;
        }
    }







}
