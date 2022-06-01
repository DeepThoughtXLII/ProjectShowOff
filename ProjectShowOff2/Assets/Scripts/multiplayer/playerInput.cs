using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class playerInput : MonoBehaviour
{

    PlayerControls controls;

    [SerializeField] Image ui;
    Image bg;
    Image character;
    TextMeshProUGUI playerName;
    

    [SerializeField]int index = 0;

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

    private void Awake()
    {
        bg = ui.gameObject.transform.GetChild(0).GetComponent<Image>();
        character = ui.gameObject.transform.GetChild(1).GetComponent<Image>();
        playerName = ui.gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        controls = new PlayerControls();
        controls.Lobby.Enable();

        controls.Lobby.startGame.performed += ctx => GetComponentInParent<Server>().StartGame();
        bg.color = Color.white;
        character.enabled = true;
        playerName.text = "ready";
    }

    private void OnDisable()
    {
        controls.Lobby.Disable();
    }

    

    
}
