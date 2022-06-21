using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public enum Controls { ONLINE, GAMEPAD, KEYBOARD }

public class Server : MonoBehaviour
{
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     FIELDS
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public static Server instance;

    public static event Action onGameOver;

    public enum gameState { LOBBY, INGAME, BOSS, GAMEOVER }
    [SerializeField]gameState state;

    
    public enum endingType { DeathPreBoss, DeathDuringBoss, Won}
    public endingType ending;

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
    public int numberOfPlayersToTestWith = 4;


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     GET() AND SET()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public gameState State
    {
        get { return state; }
    }

    public Controls UsesControls
    {
        get { return usesControls; }
        set { usesControls = value; }
    }



    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     AWAKE()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
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

        if (!testing)
        {
            state = gameState.LOBBY;
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
        PlayerHealth.onBossDeath += BossDied;
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     START()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void Start()
    {
        transform.position = Vector3.zero;
       if(controllerManager == null)
        {
            throw new Exception("no controllerManager found. players cannot join.");
        }



        playerManager = GetComponent<PlayerManager>();
        findLobbyPlayerUIComponents();
    }

    public void testingWithPlayers()
    {
        
            for (int i = 0; i < numberOfPlayersToTestWith; i++)
            {
                playerManager.AddPlayer(i);
            }
        
    }
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     FIND LOBBY �LAYER UI COMPONENTS()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void findLobbyPlayerUIComponents()
    {
        body = GameObject.FindGameObjectWithTag("UIParentLobby").GetComponent<Image>();
        playerUIs = new Image[4];
        for (int i = 0; i < 4; i++)
        {
            playerUIs[i] = body.transform.GetChild(i).GetComponent<Image>();
        }
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     UPDATE()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
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
                ending = endingType.DeathPreBoss;
                FindObjectOfType<SoundManager>().Play("gameOverVO");
            }
        }
        else if(state == gameState.BOSS)
        {
            if(playerManager.PlayersAllDead() && !testing)
            {
                state = gameState.GAMEOVER;
                ending = endingType.DeathDuringBoss;
                FindObjectOfType<SoundManager>().Play("gameOverVO");
            }
        }
        else if(state == gameState.GAMEOVER)
        {

            //Destroy(this);
            GameOver();
            //StartCoroutine(BackToLobby());
        }
    }



    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     BOSS FIGHT()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void bossFight()
    {
        SceneManager.LoadScene(2);
        playerManager.ReviveAllPlayers();
        if (playerManager.IsAPlayerCorrupted())
        {
            playerManager.HeavenFallsNextBoss();
            FindObjectOfType<SoundManager>().Play("finalBossTransitionVO");
        }
        else
        {
            startBossAIFight();
        }
    }


    private void startBossAIFight()
    {

    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     BACK TO LOBBY()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   /* IEnumerator BackToLobby()
    {
        yield return new WaitForSeconds(GameOverScreenTime);
        Destroy(this);
    }*/

    void BackToLobby()
    {
        SceneManager.LoadScene(0);
        controllerManager.ResetControllers();
        playerManager.PlayersReset();
        ResetServer();
        //end boss fight back to lobby
        //state = gameState.GAMEOVER;

    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     GAME OVER()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void GameOver()
    {
        SceneManager.LoadScene(3);
        
    }

    void BossDied()
    {
        ending = endingType.Won;
        state = gameState.GAMEOVER;
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     RESET SERVER()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void ResetServer()
    {
        for(int i = 0; i < transform.childCount; i++){
            Transform child = transform.GetChild(i);
            if(child.name != "LevelManager")
            {
                Destroy(child);
            }
            
        }
        state = gameState.LOBBY;
        body = null;
        //findLobbyPlayerUIComponents();
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     ON DESTROY()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void OnDestroy()
    {
        /*if(instance == this)
        {
            SceneManager.LoadScene(0);
            onGameOver();
        }*/
        waveSpawner.onBossWave -= bossFight;
        PlayerHealth.onBossDeath -= GameOver;
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     GET PLAYER UI LOBBY ()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public Image GetPlayerUILobby(int id)
    {
        if(body == null)
        {
            findLobbyPlayerUIComponents();
        }
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


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     START GAME()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
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
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            