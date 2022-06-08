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

    //public GameObject circle;

    Server server;






    void Start()
    {


        Debug.Log("Start 1");
        // Create a server that listens for connection requests:
        listener = new WebsocketListener();
        listener.Start();


   


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

    }

    void Update()
    {
        // Check for new connections:
        ProcessNewClients();
        ProcessCurrentClients();
        detectFaultyClients();
    }

    private void OnDestroy()
    {
        Levelable.onUpgrade -= onUpgrade;
    }

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
        foreach (KeyValuePair<int, WebSocketConnection> client in clients)
        {
            if (client.Value.Status == ConnectionStatus.Connected)
            {
                client.Value.Update();

            }
        }
    }

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
                    addClient(clients.Count, text2);
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
            else if (text.Contains("upgrade"))
            {
                parseUpgrades(text, id);
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
        }

    }

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

    void Shoot(int id)
    {
        playerManager.PlayerShoot(id);
    }

    Vector2 parseInput(string text)
    {
        string[] inputTxt = text.Split('!');
        Console.WriteLine("x= " + inputTxt[0] + " y= " + inputTxt[1]);
        return new Vector2(float.Parse(inputTxt[0]), float.Parse(inputTxt[1]));
    }

    //TEMPORARY
    void resolveJoystickInput(Vector2 input, int id)
    {
        Vector2 m = new Vector2(input.x, input.y * -1);
        m.Normalize();
        //circles[id].transform.Translate(m, Space.World);
        playerManager.MovePlayer(m, id);
    }

    void Broadcast(string msg)
    {
        foreach (KeyValuePair<int, WebSocketConnection> cl in clients)
        {
            cl.Value.SendMessage(msg);
        }
    }

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

    public int GetControllerCount()
    {
        return clients.Count;
    }

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
            player.Value.gameObject.SetActive(false);
            //p.transform.SetParent(player.Value.transform);
        }

    }

    private void detectFaultyClients()
    {
        List<int> faultyClients = new List<int>();
        foreach (KeyValuePair<int, WebSocketConnection> client in clients)
        {
            try
            {
                client.Value.SendMessage("");
                //SendMsg(client.Value.GetStream(), "");
            }
            catch { }

            if (client.Value.Status != ConnectionStatus.Connected)
            {
                faultyClients.Add(client.Key);
                //client.Value.Close();
            }
        }
        if (faultyClients.Count > 0)
        {
            foreach (int id in faultyClients)
            {
                clients.Remove(id);
                controllers[id].Disconnected();
                controllers.Remove(id);
                Console.WriteLine(id + " was removed.");
            }

        }
    }

    void addClient(int id, string name)
    {
        clients.Add(id, onWaitlist[id]);
        //clients2.Add(onWaitlist[id], id);
        InstantiateClient(id, name);
        Console.WriteLine("Accepted new client with id: " + id);
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
