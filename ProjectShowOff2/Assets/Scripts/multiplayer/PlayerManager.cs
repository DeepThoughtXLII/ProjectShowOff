using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class PlayerManager : MonoBehaviour
{


    [SerializeField]private Player playerPrefab = null;

    Dictionary<int, Player> playerList = new Dictionary<int, Player>();




    private void Start()
    {
        int numberOfControllers = InputSystem.devices.OfType<Gamepad>().Count();
        Debug.Log("controllers = " + numberOfControllers);
        if (numberOfControllers > 0)
        {

        }
    }

    public Player AddPlayer(int pId)
    {
        if (HasPlayerAlready(pId))
        {
            throw new ArgumentException($"Cannot add AvatarView with id {pId}, already exists.");
        }

        //create a new view with ourselves as the transform parent
        Player player = Instantiate<Player>(playerPrefab, transform);
        playerList[pId] = player;
        player.Id = pId;
        return player;

    }

    public bool HasPlayerAlready(int pId)
    {
        return playerList.ContainsKey(pId);
    }

    public Player GetPlayer(int pId)
    {
        if (!HasPlayerAlready(pId))
        {
            throw new Exception($"Avatar with key {pId} not found.");
        }
        return playerList[pId];
    }




    public void RemovePlayer(int pId)
    {
        Player player = GetPlayer(pId);
        player.Remove();
        playerList.Remove(pId);
    }

    public void MovePlayer(Vector2 direction, int pId)
    {
        Player player = GetPlayer(pId);
        player.Move(direction);
    }
}
