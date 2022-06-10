using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OnlinePlayerInput : MonoBehaviour
{

    [SerializeField] Image ui;
    public Color notReady;
    [SerializeField] int index = 0;
    public TextMeshProUGUI playerName;
    private string pname = null;

    Image bg;
    Image character;

    public int Index
    {
        set { index = value; }
        get { return index; }
    }

    public string Name
    {
        set { pname = value; }
        get { return pname; }
    }

    public Image UIrep
    {
        set { ui = value; }
        get { return ui; }
    }

    private void Awake()
    {
        
    }

    private void Start()
    {
        bg = ui.gameObject.transform.GetChild(0).GetComponent<Image>();
        character = ui.gameObject.transform.GetChild(1).GetComponent<Image>();
        playerName = ui.gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        notReady = new Color(255, 255, 255, 127);
        Connected();
    }

    public void Connected()
    {
        bg.color = Color.white;
        character.enabled = true;
        if (pname != null)
        {
            UpdateName();
        }
    }

    public void Disconnected()
    {
        bg.color = notReady;
        pname = "not ready";
        UpdateName();
        character.enabled = false;
        Destroy(this);
    }

    private void UpdateName()
    {
        playerName.text = pname;
    }

}
