using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class playerInput : MonoBehaviour
{

    PlayerControls controls;

    [SerializeField] Image ui;

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

    private void Start()
    {
        controls = new PlayerControls();
        controls.Lobby.Enable();

        controls.Lobby.startGame.performed += ctx => GetComponentInParent<Server>().StartGame();
        ui.color = Color.green;
    }

    private void OnDisable()
    {
        controls.Lobby.Disable();
    }

    

    
}
