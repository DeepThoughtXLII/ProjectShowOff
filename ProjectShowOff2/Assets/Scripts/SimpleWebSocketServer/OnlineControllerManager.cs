using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Linq;
using WebSockets;
using UnityEngine.InputSystem;
using System;
using System.Net.Sockets;
using System.Net; // For IPEndPoint

public class OnlineControllerManager : MonoBehaviour, IControllerManager
{


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     FIELDS
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //List<WebSocketConnection> clients;
    Dictionary<int, WebSocketConnection> clients;
    Dictionary<WebSocketConnection, int> clients2;
    //Dictionary<int, GameObject> circles;
    List<int> faultyClients;
    int idCount;

    Dictionary<int, WebSocketConnection> onWaitlist;

    Dictionary<int, OnlinePlayerInput> controllers;


    PlayerManager playerManager;


    WebsocketListener listener;

    List<int> ascendedToClient;

    //public GameObject circle;

    Server server;

    bool readyToPollAgain = true;


    int[] ids = new int[4];

    [SerializeField] int GameCount = 0;


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     START()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void Start()
    {
        for(int i = 0; i < 4; i++)
        {
            ids[i] = i;
        }

        Debug.Log("we do start again");
            // Create a server that listens for connection requests:
            listener = new WebsocketListener();
            listener.Start();
           // GameCount--;
        



        Levelable.onUpgrade += onUpgrade;

        // Create a list of active connections:
        //clients = new List<WebSocketConnection>();
        clients = new Dictionary<int, WebSocketConnection>();
        clients2 = new Dictionary<WebSocketConnection, int>();
        //circles = new Dictionary<int, GameObject>();

        faultyClients = new List<int>();

        idCount = 0;


        playerManager = GetComponent<PlayerManager>();
        server = GetComponent<Server>();
        controllers = new Dictionary<int, OnlinePlayerInput>();
        onWaitlist = new Dictionary<int, WebSocketConnection>();
        ascendedToClient = new List<int>();
        

    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     UPDATE()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void Update()
    {
        // Check for new connections:
        ProcessNewClients();
        ProcessCurrentClients();
        detectFaultyClients();
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     ON DESTROY()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void OnDestroy()
    {
        Levelable.onUpgrade -= onUpgrade;
    }



    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     ON PACKET RECEIVE()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// This method is called by WebSocketConnections when their Update method is called and a packet comes in.
    /// From here you can implement your own server functionality 
    ///   (parse the (string) package data, and depending on contents, call other methods, implement game play rules, etc). 
    /// Currently it only does some very simple string processing, and echoes and broadcasts a message.
    /// </summary>
    void OnPacketReceive(NetworkPacket packet, WebSocketConnection connection)
    {

        string text = Encoding.UTF8.GetString(packet.Data);
        Console.WriteLine("Received a packet: {0}", text);

        parseCommand(text, clients2[connection]);



        //byte[] bytes;

        //// echo:
        /*
        string response = "You said: " + text;
        bytes = Encoding.UTF8.GetBytes(response);
        connection.Send(new NetworkPacket(bytes));*/

        /* //// broadcast:
         string message = connection.RemoteEndPoint.ToString() + " says: " + text;
         bytes = Encoding.UTF8.GetBytes(message);
         Broadcast(new NetworkPacket(bytes));*/
    }



    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     PROCESS NEW CLIENTS()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void ProcessNewClients()
    {
        listener.Update();
        while (listener.Pending())
        {
            WebSocketConnection ws = listener.AcceptConnection(OnPacketReceive);
                onWaitlist.Add(idCount, ws);
                clients2.Add(ws, idCount);
                idCount++;
                Console.WriteLine("added " + idCount + " to waitlist");
            
            
            /*
            if (!IsClientIsNew(ws))
            {
                addClient(ws);
            }          */
        }
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     PROCESS CURRENT CLIENTS()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///calls update on clients and players alike
    void ProcessCurrentClients()
    {
        if (onWaitlist.Count > 0)
        {
            foreach (KeyValuePair<int, WebSocketConnection> client in onWaitlist)
            {
                if (client.Value.Status == ConnectionStatus.Connected)
                {
                    client.Value.Update();
                }
            }
        }
        updateWaitlist();
        foreach (KeyValuePair<int, WebSocketConnection> client in clients)
        {
            if (client.Value.Status == ConnectionStatus.Connected)
            {
                client.Value.Update();

            }
        }
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     UPDATE WAITLIST()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///removes anyone from the waitlist that recently became a client and already joined the game
    void updateWaitlist()
    {
        if (ascendedToClient.Count > 0)
        {
            foreach (int client in ascendedToClient)
            {
                onWaitlist.Remove(client);
            }
            ascendedToClient.Clear();
        }
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     INSTANTIATE CLIENT()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///creates the client object (OnlinePlayerInput)
    void InstantiateClient(int id, string name = null)
    {
        //GameObject client = (GameObject)Instantiate(circle, transform.position, transform.rotation);
        //circles.Add(id, client);
        //playerManager.AddPlayer(id);
        GameObject pi = new GameObject();
        pi.transform.parent = transform;
        pi.transform.position = Vector3.zero;
        pi.AddComponent<OnlinePlayerInput>();
        OnlinePlayerInput pIScript = pi.GetComponent<OnlinePlayerInput>();
        pIScript.Index = id;
        pIScript.UIrep = server.GetPlayerUILobby(id);
        if (name != null)
        {
            pIScript.Name = name;
        }
        controllers.Add(id, pIScript);
        if (id != 0)
        {
            clients[id].SendMessage("nameAccepted");
        }
        else
        {
            clients[id].SendMessage("gameLeader");
        }
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     INSTANTIATE CLIENT()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------








    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     PARSE COMMAND()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///reads string and checks which command was used and calls the necessary functions
    void parseCommand(string text, int id)
    {
        if (server.State == Server.gameState.LOBBY)
        {
            if (text.Contains("playerName"))
            {
                if (onWaitlist.ContainsKey(id))
                {
                    int cmd1 = text.IndexOf('!');
                    string text2 = text.Remove(0, cmd1 + 1);
                    Console.WriteLine(text2);
                    if (clients.Count < 4)
                    {
                        addClient(getFreeID(), id, text2);
                    }
                    else
                    {
                        onWaitlist.Remove(id);
                    }

                }
                else
                {
                    throw new Exception("already admitted player " + id + " submitted playername request. shouldnt be possible.");
                }
            }
            else if (text.Contains("startGame"))
            {
                server.StartGame();

            }
        }
        else if (server.State == Server.gameState.INGAME)
        {
            if (text.Contains("newInput"))
            {
                int cmd1 = text.IndexOf('$');
                int cmd2 = text.IndexOf('$', cmd1 + 1);
                string input = text.Remove(cmd1, cmd2 + 1);
                resolveJoystickInput(parseInput(input), id);
            }
            else if (text.Contains("shoot"))
            {
                Shoot(id);
            }
            else if (text.Contains("upgrade"))
            {
                parseUpgrades(text, id);
            }
            else if (text.Contains("playerName"))
            {
                if (onWaitlist.ContainsKey(id))
                {
                    int cmd1 = text.IndexOf('!');
                    string text2 = text.Remove(0, cmd1 + 1);
                    Console.WriteLine(text2);
                    if (playerManager.IsOldPlayer(text2))
                    {
                        Debug.Log("trying to connect old player");
                        clients2.Remove(onWaitlist[id]);
                        ascendedToClient.Add(id);
                        reconnectClient(onWaitlist[id], playerManager.getOldPlayerID(text2));
                        
                    }
                }
            }
        }
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     RECONNECT CLIENT()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void reconnectClient(WebSocketConnection client, int id)
    {
        clients.Add(id, client);
        clients2.Add(client, id);
        clients[id].SendMessage("gameStart");
        playerManager.ReconnectPlayer(id);
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     GET FREE ID()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///checks which id of {0,1,2,3} is the next free one
    int getFreeID()
    {
        foreach (int id in ids)
        {
            if (!clients.ContainsKey(id)) {
                return id;
            }
        }
        return -1;
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     PASRE UPGRADES()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------#
    ///reads the upgrade type from string
    void parseUpgrades(string text, int id)
    {
        int cmd1 = text.IndexOf('!');
        text = text.Remove(0, cmd1 + 1);
        switch (text)
        {
            case "Health":
                playerManager.changeUpgradeType(id, Upgrade.UpgradeType.HEALTH);
                break;
            case "Attack":
                playerManager.changeUpgradeType(id, Upgrade.UpgradeType.ATTACK);
                break;
            case "Speed":
                playerManager.changeUpgradeType(id, Upgrade.UpgradeType.SPEED);
                break;
        }
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     SHOOT()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void Shoot(int id)
    {
        playerManager.PlayerShoot(id);
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     PARSE INPUT()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///seperates the joystick input from the string
    Vector2 parseInput(string text)
    {
        string[] inputTxt = text.Split('!');
        //Console.WriteLine("x= " + inputTxt[0] + " y= " + inputTxt[1]);
        return new Vector2(float.Parse(inputTxt[0]), float.Parse(inputTxt[1]));
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     RESOLVE JOYSTICK INPUT()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///calls teh player manager to take care of moving the player
    void resolveJoystickInput(Vector2 input, int id)
    {
        Vector2 m = new Vector2(input.x, input.y * -1);
        m.Normalize();
        //circles[id].transform.Translate(m, Space.World);
        playerManager.MovePlayer(m, id);
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     BROADCAST()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void Broadcast(string msg)
    {
        foreach (KeyValuePair<int, WebSocketConnection> cl in clients)
        {
            cl.Value.SendMessage(msg);
        }
    }



    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     ON UPGRADE()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///checks if any of the players have reached an upgrade threshold and there is need to start the upgrade conversation
    private void onUpgrade()
    {
        foreach (KeyValuePair<int, WebSocketConnection> client in clients)
        {
            if (server.State == Server.gameState.INGAME && playerManager.checkForPlayerUpgrades(client.Key))
            {
                client.Value.SendMessage("upgrade");
                Debug.Log("AHHHHHHHHHHHHHHHHHHHHHHHHHH UPGRADE YIPPIEYAYYOOOOOOOO");
            }
        }
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     GET CONTROLLER COUNT()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public int GetControllerCount()
    {
        return clients.Count;
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     ON GAME START()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void OnGameStart()
    {
        foreach (KeyValuePair<int, OnlinePlayerInput> player in controllers)
        {
            clients[player.Key].SendMessage("gameStart");
            player.Value.UIrep = null;
            Player p = playerManager.AddPlayer(player.Key, player.Value.Name);
            p.transform.position = Vector3.zero;
            player.Value.transform.SetParent(p.transform);
            p.GetComponent<Player>().enabled = true;
            player.Value.enabled = false;
            //p.transform.SetParent(player.Value.transform);
        }

    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     POLL CLIENT()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///checks every .5 seconds if client is still conencted
    IEnumerator pollClient()
    {
        foreach (KeyValuePair<int, WebSocketConnection> client in clients)
        {
            try
            {
                client.Value.SendMessage("");
            }catch
            {

            }
            if (client.Value.Status != ConnectionStatus.Connected)
            {
                faultyClients.Add(client.Key);
                //client.Value.Close();
            }
        }
        readyToPollAgain = false;
        yield return new WaitForSeconds(0.5f);
        readyToPollAgain = true;
    }



    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     DETECT FAULTY CLIENT()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///removes clients upon detecting that they disconnected
    private void detectFaultyClients()
    {
        /*List<int> faultyClients = new List<int>();
        foreach (KeyValuePair<int, WebSocketConnection> client in clients)
        {
            

            if (client.Value.Status != ConnectionStatus.Connected)
            {
                faultyClients.Add(client.Key);
                //client.Value.Close();
            }
        }*/
        if (readyToPollAgain)
        {
            StartCoroutine(pollClient());
        }
        if (faultyClients.Count > 0)
        {
            foreach (int id in faultyClients)
            {
                //clients[id].Close();
                clients.Remove(id);
                if (controllers[id].gameObject.activeSelf) {
                    controllers[id].Disconnected();
                    controllers.Remove(id);
                }
                if(server.State != Server.gameState.LOBBY)
                {
                    playerManager.DisconnectPlayer(id);
                }
                
                Console.WriteLine(id + " was removed.");
            }
            faultyClients.Clear();
        }
    }



    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     ADD CLIENT()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void addClient(int id, int waitId, string name)
    {
        clients.Add(id, onWaitlist[waitId]);
        ascendedToClient.Add(waitId);
        clients2.Remove(onWaitlist[waitId]);
        clients2.Add(clients[id], id);
        InstantiateClient(id, name);
        Console.WriteLine("Accepted new client with id: " + id);
    }



    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     RESET CONTROLLER()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void ResetControllers()
    {
        clients.Clear();
        onWaitlist.Clear();
        controllers.Clear();
        faultyClients.Clear();
        
    }


    /*
    bool IsClientIsNew(WebSocketConnection newConnection)
    {
        if(oldClients.Count > 0)
        {
            foreach(KeyValuePair<TcpClient, int> client in oldClients)
            if(newConnection.GetClient() == client.Key)
            {
                    addClient(newConnection, client.Value);
                    Console.WriteLine("Old client detected");
                    return true;
            }
        }
        Console.WriteLine("not an old client");
        return false;
    }
    */
}
