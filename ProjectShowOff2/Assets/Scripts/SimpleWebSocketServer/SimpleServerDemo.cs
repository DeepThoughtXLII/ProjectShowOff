using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Linq;
using WebSockets;
using UnityEngine.InputSystem;
using System;

public class SimpleServerDemo : MonoBehaviour
{
    //List<WebSocketConnection> clients;
    Dictionary<int, WebSocketConnection> clients;
    Dictionary<WebSocketConnection, int> clients2;
    //Dictionary<int, GameObject> circles;
    List<int> faultyClients;
    int idCount;

    PlayerManager playerManager;
    Dictionary<int, int> controllers;

    WebsocketListener listener;

    //public GameObject circle;
    public float speed;


    bool usingOnlineControllers = true;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
    }

    void Start()
    {
        int numberOfControllers = InputSystem.devices.OfType<Gamepad>().Count();
        Debug.Log("controllers = " + numberOfControllers);
        if (numberOfControllers > 0)
        {
            ControllerManagement(numberOfControllers);
            usingOnlineControllers = false;
            controllers = new Dictionary<int, int>();
        }

        if (usingOnlineControllers)
        {
            // Create a server that listens for connection requests:
            listener = new WebsocketListener();
            listener.Start();

            // Create a list of active connections:
            //clients = new List<WebSocketConnection>();
            clients = new Dictionary<int, WebSocketConnection>();
            clients2 = new Dictionary<WebSocketConnection, int>();
            //circles = new Dictionary<int, GameObject>();

            faultyClients = new List<int>();
        }
            idCount = 0;
        
    }

    void Update()
    {

        if (usingOnlineControllers)
        {
            // Check for new connections:
            ProcessNewClients();

            foreach (KeyValuePair<int, WebSocketConnection> client in clients)
            {
                if (client.Value.Status == ConnectionStatus.Connected)
                {
                    client.Value.Update();
                }
                else
                {
                    faultyClients.Add(client.Key);
                }
            }
            if (faultyClients.Count > 0)
            {
                foreach (int id in faultyClients)
                {
                    clients2.Remove(clients[id]);
                    clients.Remove(id);
                    playerManager.RemovePlayer(id);
                    //Destroy(circles[id]);
                    //circles.Remove(id);
                }
            }
        }
    }

    /// <summary>
    /// This method is called by WebSocketConnections when their Update method is called and a packet comes in.
    /// From here you can implement your own server functionality 
    ///   (parse the (string) package data, and depending on contents, call other methods, implement game play rules, etc). 
    /// Currently it only does some very simple string processing, and echoes and broadcasts a message.
    /// </summary>
    void OnPacketReceive(NetworkPacket packet, WebSocketConnection connection) {

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
            clients.Add(idCount, ws);
            clients2.Add(ws, idCount);
            InstantiateClient(idCount);
            Console.WriteLine("Accepted new client with id: " + idCount);
            idCount++;
        }
    }

    void InstantiateClient(int id)
    {
        //GameObject client = (GameObject)Instantiate(circle, transform.position, transform.rotation);
        //circles.Add(id, client);
        playerManager.AddPlayer(id);
    }

    void parseCommand(string text, int id)
    {
        if (text.Contains("newInput"))
        {
            int cmd1 = text.IndexOf('$');
            int cmd2 = text.IndexOf('$', cmd1 + 1);
            string input = text.Remove(cmd1, cmd2+1);
            resolveJoystickInput(parseInput(input), id);
        } else if (text.Contains("shoot")) { }
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
        Vector2 m = new Vector2(input.x, input.y * -1) * speed * Time.deltaTime;
        //circles[id].transform.Translate(m, Space.World);
        playerManager.MovePlayer(m, id);
    }

    void Broadcast(NetworkPacket packet) {
        foreach (KeyValuePair<int,WebSocketConnection> cl in clients) {
            cl.Value.Send(packet);
        }
    }

    void ControllerManagement(int playerCount)
    {
        Gamepad [] gamepads = InputSystem.devices.OfType<Gamepad>().ToArray();
        for (int i = 0; i<playerCount; i++)
        {
            playerManager.AddPlayer(gamepads[i].deviceId);           
        }       
    }
}
