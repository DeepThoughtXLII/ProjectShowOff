using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerTracker : MonoBehaviour
{
    public static List<GameObject> players;
    public static List<Vector2> playerPositions;
    void Start()
    {
        players = new List<GameObject>();
        playerPositions = new List<Vector2>();

        foreach (GameObject player in GameObject.FindGameObjectsWithTag("player"))
        {
            players.Add(player);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void setPlayerPositions()
    {
        foreach(GameObject player in players)
        {
            playerPositions.Add((Vector2)player.transform.position);
        }
    }

    public static void deletePlayerPositions()
    {
        playerPositions.Clear();
    }

    public static List<Vector2> PlayerPositions
    {
        
        get { setPlayerPositions(); return playerPositions;}
    }

}
