using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OnlinePlayerInput : MonoBehaviour
{

    [SerializeField] Image ui;
    private Color def;
    [SerializeField] int index = 0;
    public TextMeshProUGUI playerName;
    private string pname = null;

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
        playerName = ui.transform.GetComponentInChildren<TextMeshProUGUI>();
        def = ui.color;
        Connected();
        
       
    }

    public void Connected()
    {
        ui.color = Color.green;
        if (pname != null)
        {
            UpdateName();
        }
    }

    public void Disconnected()
    {
        ui.color = def;
        pname = "no player";
        UpdateName();
        Destroy(this);
    }

    private void UpdateName()
    {
        playerName.text = pname;
    }

}
