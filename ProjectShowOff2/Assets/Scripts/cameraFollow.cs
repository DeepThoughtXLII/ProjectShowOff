using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraFollow : MonoBehaviour
{

    Transform [] targets;

    public float smoothSpeed = 0.125f;

    private Vector3 velocity = Vector3.one;

    public float distanceDamp = 0.5f;

    private Vector3 targetMidpoint;

    [SerializeField] float minZoom;
    [SerializeField] float maxZoom;

    Camera cam;

    float offset = 3f;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographicSize = minZoom;
        

        GameObject[] tempT = GameObject.FindGameObjectsWithTag("Player");
       
        targets = new Transform[tempT.Length];
        
        for (int i = 0; i< tempT.Length; i++)
        {
            targets[i] = tempT[i].transform;
        }

        FollowTarget();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        FollowTarget();
    }

    void FollowTarget()
    {
        getMidpointOfTargets();
        if (transform.position != targetMidpoint)
        {
            //Vector2 distanceT = transform.position - target.position;
            //Vector2 velocity = distance.normalized * smoothSpeed;
            
            Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, targetMidpoint, ref velocity, distanceDamp);
            transform.position = smoothedPosition;
        }
    }



    void getMidpoint(int playerCount)
    {
        if(playerCount > 1)
        {
            var bound = new Bounds(targets[0].position, Vector3.zero);
            for (int i = 1; i < targets.Length; i++)
            {
                bound.Encapsulate(targets[i].position);
            }
            targetMidpoint = bound.center;
        }
        else
        {
            targetMidpoint = targets[0].position;
        }

    }

    void getMidpointOfTargets()
    {
        /*  if (targets.Length == 1)
          {
              targetMidpoint = targets[0].position;
          }
          if (targets.Length == 2)
          {
              //Vector3 dist = targets[0].position - targets[1].position;
              //targetMidpoint = targets[0].position - dist / 2;
              //Vector3 direction = (targets[0].position - targets[1].position).normalized;
              //targetMidpoint = direction * (Vector3.Distance(targets[0].position, targets[1].position)/2);
              targetMidpoint = Vector3.Lerp(targets[0].position, targets[1].position, 0.5f);
          }*/

        getMidpoint(targets.Length);

        float camDist = Vector3.Distance(targetMidpoint, targets[0].position);
        if (camDist > cam.orthographicSize && cam.orthographicSize <= maxZoom)
        {
            cam.orthographicSize += camDist - cam.orthographicSize;
            if(cam.orthographicSize > maxZoom)
            {
                cam.orthographicSize = maxZoom;
            }
        } 
        else if(camDist < cam.orthographicSize && cam.orthographicSize >= minZoom)
        {
            cam.orthographicSize -= cam.orthographicSize - camDist;
            if(cam.orthographicSize < minZoom)
            {
                cam.orthographicSize = minZoom;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(targetMidpoint != null)
        {
            Gizmos.DrawWireSphere(targetMidpoint, 2);
        }

    }
}
