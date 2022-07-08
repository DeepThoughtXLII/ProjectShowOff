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

    public enum gameState { LOBBY, INGAME, BOSS, GAMEOVER, CUTSCENE }
    [SerializeField] gameState state;


    public enum endingType { DeathPreBoss, DeathDuringBoss, Won }
    public endingType ending;

    [SerializeField]
    private Controls usesControls = Controls.ONLINE;

    [SerializeField] Image[] playerUIs;
    public Image body;



    PlayerManager playerManager;

    [SerializeField]
    IControllerManager controllerManager;

    public int minAmountOfPlayers = 1;

    public bool testing = true;
    public int numberOfPlayersToTestWith = 4;

    public string[] scenes = { "Lobby", "Introduction", "Game", "BossTransition", "Hell", "GameOver" };
    public bool cutscenesOn = false;




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
        if (instance != null && instance != this)
        {
            Destroy(gameObject);

        }
        else
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
        }
        else
        {
            controllerManager = GetComponent<ControllerManager>();
            GetComponent<OnlineControllerManager>().enabled = false;
            GetComponent<ControllerManager>().enabled = true;
        }

        waveSpawner.onBossWave += bossFight;
        PlayerHealth.onBossDeath += BossDied;
        LoadMenu.onGameEnd += BackToLobby;
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     START()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void Start()
    {
        transform.position = Vector3.zero;
        if (controllerManager == null)
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
                //FindObjectOfType<SoundManager>().Play("gameOverVO");
                GameOver();
            }
        }
        else if (state == gameState.BOSS)
        {
            if (playerManager.PlayersAllDead() && !testing)
            {
                state = gameState.GAMEOVER;
                ending = endingType.DeathDuringBoss;
                //FindObjectOfType<SoundManager>().Play("gameOverVO");
                GameOver();

            }


        }
        else if (state == gameState.GAMEOVER)
        {

            //Destroy(this);
            //GameOver();
            //StartCoroutine(BackToLobby());
        }
    }



    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     BOSS FIGHT()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void bossFight()
    {
        state = gameState.BOSS;
        changeScene("BossTransition");
        //SceneManager.LoadScene(4);
       

        playerManager.ReviveAllPlayers();
        if (playerManager.IsAPlayerCorrupted())
        {
            startPlayerBossFight();
        }
        else
        {
            startBossAIFight();
        }
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("BossTransition")){
            playerManager.SpawnPlayers();

        }
    }

    private void startPlayerBossFight()
    {
        Debug.Log("PLAYER BOSS");
        playerManager.HeavenFallsNextBoss();
        //FindObjectOfType<SoundManager>().Play("finalBossTransitionVO");
    }


    private void startBossAIFight()
    {
        Debug.Log("AI BOSS");
        playerManager.HeavenFallsNextBoss();
        //FindObjectOfType<SoundManager>().Play("finalBossTransitionVO");
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     BACK TO LOBBY()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public void changeScene(string Scene)
    {
        //int index = Array.IndexOf(scenes, Scene);
        string sceneToLoad = Scene;
        Debug.Log($"attemot to load scene: {Scene}");
        if (Scene == "Introduction" || Scene == "BossTransition")
        {
            if (!cutscenesOn)
            {
                sceneToLoad = scenes[Array.IndexOf(scenes, Scene) + 1];
            }
            else
            {
                playerManager.playerCutsceneMode(true);
                state = gameState.CUTSCENE;
            }
        } else 
        {
            if (Scene == "Game")
            {
                state = gameState.INGAME;
            }
            else if (Scene == "Hell")
            {
                state = gameState.BOSS;
            }
            playerManager.playerCutsceneMode(false);
        }

            SceneManager.LoadScene(sceneToLoad);
        
        
    }

    public void LoadSceneWithLoadingScreen(string Scene, Slider slide)
    {
        //int index = Array.IndexOf(scenes, Scene);
        string sceneToLoad = Scene;
        Debug.Log($"attempt to load scene: {Scene}");
        if (Scene == "Introduction" || Scene == "BossTransition")
        {
            if (!cutscenesOn)
            {
                sceneToLoad = scenes[Array.IndexOf(scenes, Scene) + 1];
            }
            else
            {
                playerManager.playerCutsceneMode(true);
                state = gameState.CUTSCENE;
            }
        }
        else
        {
            if(Scene == "Game")
            {
                state = gameState.INGAME;
            }else if (Scene == "Hell")
            {
                state = gameState.BOSS;
            }
            playerManager.playerCutsceneMode(false);
        }

        StartCoroutine(AsynchronousLoad(sceneToLoad, slide));
    }

    IEnumerator AsynchronousLoad(string scene, Slider slide)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
        while (!operation.isDone)
        {
            // [0, 0.9] > [0, 1]
            //float progress = Mathf.Clamp01(operation.progress / 0.9f);
            Debug.Log("Loading progress: " + (operation.progress * 100) + "%");
            slide.value = operation.progress/0.9f;
            if(operation.progress == 0.9f)
            {
                slide.value = 1;
                operation.allowSceneActivation = true;
            }
            // Loading completed

            yield return null;
        }
    }

    void cutSceneMode()
    {

    }


    public void BackToLobby()
    {
        //FindObjectOfType<SoundManager>().Stop("ingameMusic");
        //FindObjectOfType<SoundManager>().Stop("bossMusic");

        controllerManager.ResetControllers();
        playerManager.PlayersReset();
        ResetServer();
        Destroy(GameObject.Find("SoundManager"));
        changeScene("Lobby");
        //end boss fight back to lobby
        //state = gameState.GAMEOVER;

    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     GAME OVER()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void GameOver()
    {
        //onGameOver();
        changeScene("GameOver");
        playerManager.removeAllPlayers();
        controllerManager.endGame();
    }

    void BossDied()
    {
        ending = endingType.Won;
        state = gameState.GAMEOVER;
        GameOver();
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     RESET SERVER()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void ResetServer()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.name != "LevelManager")
            {
                Destroy(child.gameObject);
            }

        }
        //FindObjectOfType<SoundManager>().Stop("ingameMusic");
        //FindObjectOfType<SoundManager>().Stop("bossMusic");
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
        if (body == null)
        {
            findLobbyPlayerUIComponents();
        }
        if (id < 4 && id > -1)
        {
            return playerUIs[id];
        }
        else
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
            changeScene("Introduction");
            Debug.Log("change Scene");
            state = gameState.INGAME;
            controllerManager.OnGameStart();


        }
    }





}
