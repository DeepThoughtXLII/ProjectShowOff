using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class Server : MonoBehaviour
{

    public static Server instance;

    public static event Action onGameOver;

    public enum gameState { LOBBY, INGAME, BOSS, GAMEOVER }
    gameState state;

    

    public enum Controls { ONLINE, GAMEPAD, KEYBOARD}

    [SerializeField]
    private Controls usesControls = Controls.ONLINE;

    [SerializeField] Image[] playerUIs;
    public Image body;

    private int GameOverScreenTime = 10;
   

    PlayerManager playerManager;

    [SerializeField]
    IControllerManager controllerManager;

    public int minAmountOfPlayers = 1;

    public bool testing = true;

    public gameState State
    {
        get { return state; }
    }

    public Controls UsesControls
    {
        get { return usesControls; }
        set { usesControls = value; }
    }


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
        
        
        if (usesControls == Controls.ONLINE)
        {
            controllerManager = GetComponent<OnlineControllerManager>();
            GetComponent<OnlineControllerManager>().enabled = true;
            GetComponent<ControllerManager>().enabled = false;
        } else
        {
            controllerManager = GetComponent<ControllerManager>();
            GetComponent<OnlineControllerManager>().enabled = false;
            GetComponent<ControllerManager>().enabled = true;
        }

        waveSpawner.onBossWave += bossFight;
        Player.onBossDeath += GameOver;
    }

    private void Start()
    {
       if(controllerManager == null)
        {
            throw new Exception("no controllerManager found. players cannot join.");
        }



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
           
        }
        else if (state == gameState.INGAME)
        {
            if (playerManager.PlayersAllDead() && !testing)
            {
                state = gameState.GAMEOVER;
            }
        }
        else if(state == gameState.GAMEOVER)
        {

            Destroy(this);
            
            //StartCoroutine(BackToLobby());
        }
    }


    private void bossFight()
    {
        SceneManager.LoadScene(2);
        playerManager.HeavenFallsNextBoss();
    }

    IEnumerator BackToLobby()
    {
        yield return new WaitForSeconds(GameOverScreenTime);
        Destroy(this);
    }

    void GameOver()
    {
        //SceneManager.LoadScene(0);
        state = gameState.GAMEOVER;
    }

    public void OnDestroy()
    {
        if(instance == this)
        {
            SceneManager.LoadScene(0);
            onGameOver();
        }
        waveSpawner.onBossWave -= bossFight;
        Player.onBossDeath -= GameOver;
    }

    public Image GetPlayerUILobby(int id)
    {
        if(id < 4 && id > -1)
        {
            return playerUIs[id];
        } else
        {
            throw new Exception("Player ID out of range");           
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
        if (state == gameState.LOBBY && controllerManager.GetControllerCount() >= minAmountOfPlayers)
        {
            SceneManager.LoadScene(1);
            Debug.Log("change Scene");
            state = gameState.INGAME;
            controllerManager.OnGameStart();
        }
    }


   


  
}
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            