using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Server : MonoBehaviour
{

    public static Server instance;

    enum gameState { LOBBY, INGAME }
    gameState state;

    Dictionary<int, playerInput> players;

    [SerializeField] Image[] playerUIs;
    public Image body;

    [SerializeField]
    bool usingOnlineControllers = true;
    [SerializeField]
    bool keyboardTesting = true;

    PlayerManager playerManager;

    public int minAmountOfPlayers = 1;

    PlayerInputManager playerInputManager;

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            
        } else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        state = gameState.LOBBY;
        

        int numberOfControllers = InputSystem.devices.OfType<Gamepad>().Count();
        Debug.Log("controllers = " + numberOfControllers);
        if (numberOfControllers > 0)
        {
            //ControllerManagement(numberOfControllers);
            //usingOnlineControllers = false;
            //controllers = new Dictionary<int, int>();
        }
        
        
        if (usingOnlineControllers)
        {
            GetComponent<OnlineControllerManager>().enabled = true;
            GetComponent<ControllerManager>().enabled = false;
        } else
        {
            GetComponent<OnlineControllerManager>().enabled = false;
            GetComponent<ControllerManager>().enabled = true;
        }

        playerInputManager = GetComponent<PlayerInputManager>();
    }

    private void Start()
    {
        players = new Dictionary<int, playerInput>();
        playerManager = GetComponent<PlayerManager>();
        playerUIs = new Image[4];
        for (int i = 0; i < 4; i++)
        {
            playerUIs[i] = body.transform.GetChild(i).GetComponent<Image>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (state == gameState.LOBBY)
        {
            if (keyboardTesting)
            {
                if (!players.ContainsKey(0) || !players.ContainsKey(1))
                {
                    JoinPlayers();
                }
            } else
            {
                playerInputManager.joinBehavior = PlayerJoinBehavior.JoinPlayersWhenButtonIsPressed;
            }
        }
        if (state == gameState.INGAME)
        {

        }
    }

    /*
    void ControllerManagement(int playerCount)
    {
        Gamepad[] gamepads = InputSystem.devices.OfType<Gamepad>().ToArray();
        for (int i = 0; i < playerCount; i++)
        {
            int id = gamepads[i].deviceId;
            Debug.Log("id of controller: " + id);
            //playerManager.AddPlayer(id);           
        }
    }*/

    public void StartGame()
    {
        Debug.Log("start game");
        if (state == gameState.LOBBY && players.Count >= minAmountOfPlayers)
        {
            SceneManager.LoadScene(1);
            Debug.Log("change Scene");
            state = gameState.INGAME;
            OnGameStart();
        }
    }


    public void AddInput(PlayerInput pi)
    {
        if (!players.ContainsKey(pi.playerIndex))
        {
            playerInput pIScript = pi.transform.GetComponent<playerInput>();
            pi.transform.position = Vector3.zero;
            pIScript.Index = pi.playerIndex;
            pIScript.UIrep = playerUIs[pi.playerIndex];

            players.Add(pi.playerIndex, pIScript);
        }
    }


    public void OnGameStart()
    {
        foreach(KeyValuePair<int, playerInput> player in players)
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
}
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            