using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingManager : MonoBehaviour
{

    List<Player> players;

    List<enemyScript> enemies;



    // Start is called before the first frame update
    void Start()
    {
        players = new List<Player>();
        PlayerManager server = GameObject.FindGameObjectWithTag("server").GetComponent<PlayerManager>();
        for(int i = 0; i < server.GetPlayerCount(); i++)
        {
            players.Add(server.GetPlayer(i));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Player GetTarget(Transform self)
    {
        Player target = null;
        float shortestDist = 0;
        foreach(Player p in players)
        {
            if(p.State != Player.PlayerState.REVIVING)
            {
                float dist = Vector3.Distance(self.position, p.transform.position);
                if (target == null || shortestDist > dist)
                {
                    shortestDist = dist;
                    target = p;
                }
            }          
        }
        return target;
    }

    public Player GetTargetInShootingRange(Transform self, float range)
    {
        List<Player> targetsInRange = new List<Player>();
        Player target = null;
        target = GetTarget(self);
        if(Vector3.Distance(target.transform.position, self.position) < range)
        {
            return target;
        }
        else
        {
            return null;
        }
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