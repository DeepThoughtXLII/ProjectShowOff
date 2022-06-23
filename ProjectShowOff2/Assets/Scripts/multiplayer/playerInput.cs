using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class playerInput : MonoBehaviour
{
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     FIELDS
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    PlayerControls controls;

    [SerializeField] Image ui;
    Image bg;
    Image character;
    TextMeshProUGUI playerName;
    

    [SerializeField]int index = 0;

    static Animator anim;

    bool gameOverScreen = false;


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     GET() AND SET()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public int Index
    {
        set { index = value; }
        get { return index; }
    }

    public Image UIrep
    {
        set { ui = value; }
        get { return ui; }
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     AWAKE()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void Awake()
    {
        /*bg = ui.gameObject.transform.GetChild(0).GetComponent<Image>();
        character = ui.gameObject.transform.GetChild(1).GetComponent<Image>(); */
        anim = ui.gameObject.transform.GetComponent<Animator>();
        playerName = ui.gameObject.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     START()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void Start()
    {
        controls = new PlayerControls();
        controls.Lobby.Enable();

        anim.SetBool("Player_Joined", true);
        anim.SetBool("Player_Left", false);

        Server.onGameOver += gameOverControls;

        controls.Lobby.startGame.performed += ctx => GetComponentInParent<Server>().StartGame();

       /* bg.color = Color.white;
        character.enabled = true; */
        playerName.text = "ready";
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     ON DISABLE()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void OnDisable()
    {
        //anim.SetBool("Player_Left", true);
        //anim.SetBool("Player_Joined", false);

        controls.Lobby.Disable();
    }

    void gameOverControls()
    {
        gameOverScreen = true;
    }

    private void OnEnable()
    {
        if (gameOverScreen)
        {
            controls.GameOver.Enable();
            controls.GameOver.backToLobby.performed += ctx => GetComponentInParent<Server>().BackToLobby();
            controls.GameOver.playAgain.performed += ctx => GetComponentInParent<Server>().BackToLobby();
        }

    }

    private void OnDestroy()
    {
        Server.onGameOver -= gameOverControls;
    }


}
