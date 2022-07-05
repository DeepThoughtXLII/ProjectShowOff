using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingManager : MonoBehaviour
{
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     FIELDS
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    List<Player> players;

    List<enemyScript> enemies;


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     START()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // Start is called before the first frame update

    private void Awake()
    {
        players = new List<Player>();
        try
        {
            if (GameObject.FindGameObjectWithTag("server").TryGetComponent<PlayerManager>(out PlayerManager pm))
            {
                PlayerManager server = pm;
                for (int i = 0; i < server.GetPlayerCount(); i++)
                {
                    players.Add(server.GetPlayer(i));
                    Debug.Log("added player");
                }
            }
        }
        catch { }

    }


    void Start()
    {
       
    }

    private void Update()
    {
        if (players.Count <= 0)
        {
            if (players.Count <= 0)
            {
                players.Add(GameObject.FindGameObjectWithTag("Player").GetComponent<Player>());
            }
            /*
            PlayerManager server = GameObject.FindGameObjectWithTag("server").GetComponent<PlayerManager>();
            for (int i = 0; i < server.GetPlayerCount(); i++)
            {
                players.Add(server.GetPlayer(i));

            }*/
        }
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     GET TARGET()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public Player GetTarget(Transform self)
    {
        Player target = null;
        float shortestDist = 0;
        foreach(Player p in players)
        {
            if(p.GetPlayerHealth().State != PlayerHealth.PlayerState.REVIVING)
            {
                float dist = Vector3.Distance(self.position, p.transform.position);
                if (target == null || shortestDist > dist)
                {
                    shortestDist = dist;
                    target = p;
                }
            }          
        }
        if(target != null)
        {
            return target;
        }
        return null;

    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     GET TARGET IN SHOOTING RANGE()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public Player GetTargetInShootingRange(Transform self, float range)
    {
        List<Player> targetsInRange = new List<Player>();
        Player target = null;
        target = GetTarget(self);
        if (target != null)
        {
            if (Vector3.Distance(target.transform.position, self.position) < range)
            {
                return target;
            }
            else
            {
                return null;
            }
        }
        return null;
    }


}


/*        foreach (GameObject player in players)
        {
            if (Physics2D.OverlapCircle(new Vector2(player.transform.position.x, player.transform.position.y), range))
            {                
                    targetsInRange.Add(player.transform);   
            }
        }
        if (targetsInRange.Count > 0)
        {
            float shortestDistance = range;
            foreach (Transform target in targetsInRange)
            {
                float dist = Vector3.Distance(target.position, transform.position);
                if (dist < shortestDistance)
                {
                    shortestDistance = dist;
                    this.target = target;
                }
            }          
        }*/