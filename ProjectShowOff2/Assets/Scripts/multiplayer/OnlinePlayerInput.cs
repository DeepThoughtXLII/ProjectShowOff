using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OnlinePlayerInput : MonoBehaviour
{
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     FIELDS
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    [SerializeField] Image ui;
    public Color notReady;
    [SerializeField] int index = 0;
    public TextMeshProUGUI playerName;
    private string pname = null;

    Image bg;
    Image character;

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



    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     START ()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void Start()
    {
        anim = ui.gameObject.transform.GetComponent<Animator>();
       /* bg = ui.gameObject.transform.GetChild(0).GetComponent<Image>();
        character = ui.gameObject.transform.GetChild(1).GetComponent<Image>(); */
        playerName = ui.gameObject.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        notReady = new Color(255, 255, 255, 0.5f);
        Connected();

        Server.onGameOver += gameOverControls;
    }

    void gameOverControls()
    {
        gameOverScreen = true;
    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     CONNECTED()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void Connected()
    {
        anim.SetBool("Player_Joined", true);
        anim.SetBool("Player_Left", false);
       
        /*bg.color = Color.white;
        character.enabled = true;*/
        if (pname != null)
        {
            UpdateName();
        }
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     DISCONNECTED
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void Disconnected()
    {
        /*bg.color = notReady;
        character.enabled = false;
        */
        anim.SetBool("Player_Left", true);
        anim.SetBool("Player_Joined", false);

        pname = "not ready";
        UpdateName();
        Destroy(this);
    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     UPDATE NAME()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void UpdateName()
    {
        playerName.text = pname;
    }

}
