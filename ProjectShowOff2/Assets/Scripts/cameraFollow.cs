using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraFollow : MonoBehaviour
{
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     FIELDS
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public Transform[] targets;

    List<Transform> targetsOnCamera;
    List<Transform> targetsThatDontFit;

    public float smoothSpeed = 0.125f;
    public float smoothZoomVelocity = 0;

    private Vector3 velocity = Vector3.one;

    public float distanceDamp = 0.5f;
    public float smoothTime = 0.1f;

    private Vector3 targetMidpoint;

    [SerializeField] float minZoom;
    [SerializeField] float maxZoom;

    Camera cam;

    float offset = 15;

    float cameraSizeDist;

    Vector3 cameraZoomRange;
    Vector3 camCorner;

    Transform outSider;

    public bool zoomOut = false;

    Vector3 closestPointOnZoomBox;

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     START()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // Start is called before the first frame update
    void Start()
    {
        minZoom = 30;
        maxZoom = 50;
        cam = GetComponent<Camera>();
        cam.orthographicSize = minZoom;
        targetMidpoint = Vector3.zero;
        targetsOnCamera = new List<Transform>();
        targetsThatDontFit = new List<Transform>();

        PlayerManager pm = GameObject.FindGameObjectWithTag("server").GetComponent<PlayerManager>();
        GameObject[] tempT = GameObject.FindGameObjectsWithTag("Player");



            targets = new Transform[pm.GetPlayerCount()];

            for (int i = 0; i < pm.GetPlayerCount(); i++)
            {
                targets[i] = pm.GetPlayer(i).transform;//tempT[i].transform;
            }
        

        FollowTarget();
    }



    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     LATE UPDATE()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // Update is called once per frame
    void LateUpdate()
    {
        FollowTarget();
        //changeColors();
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     FOLLOW TARGET()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void FollowTarget()
    {
        if (targets[0] != null)
        {
            //getMidpointOfTargets();
            newCamera();
            if (transform.position != targetMidpoint)
            {
                //Vector2 distanceT = transform.position - target.position;
                //Vector2 velocity = distance.normalized * smoothSpeed;

                Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, targetMidpoint, ref velocity, distanceDamp);
                transform.position = smoothedPosition;
            }
        }
    }


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     GET MIDPOINT()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void getMidpoint(Transform[] targets)
    {
        if(targets.Length > 1)
        {
            var bound = new Bounds(targets[0].position, Vector3.zero); //creates AABB bounding box with player one as center
            for (int i = 1; i < targets.Length; i++)
            {
                bound.Encapsulate(targets[i].position); //make box grow to encapsulate each point
            }
            targetMidpoint = bound.center; //target camera to the center of the box
        }
        else if( targets.Length > 0)
        {
            targetMidpoint = targets[0].position;
        }

    }


    /*
    Bounds getCameraBoxWithAllTargetsInside(Transform [] targets)
    {
        if (targets.Length > 1)
        {
            Bounds bound = new Bounds(targets[0].position, Vector3.zero); //creates AABB bounding box with player one as center
            for (int i = 1; i < targets.Length; i++)
            {
                bound.Encapsulate(targets[i].position); //make box grow to encapsulate each point
            }
            return bound;
        }
        else
        {
            return new Bounds(targets[0].position, new Vector3(0, 0, 0));
        }
    }*/

    bool isTargetOnCamera(Transform target)
    {
        Bounds bound = new Bounds(targetMidpoint, new Vector3(cam.aspect*cam.orthographicSize*2, cam.orthographicSize*2)); //creates AABB bounding box with player one as center
        if (bound.Contains(target.position))
        {
            return true;
        }
        return false;
    }

    bool AllTargetsOnCamera(Transform [] targets)
    {
        int targetsNotOnCamera = 0;
        foreach(Transform target in targets)
        {
            if (!isTargetOnCamera(target))
            {
                targetsNotOnCamera++;
            }
        }
        if(targetsNotOnCamera > 0)
        {
            return false;
        }else
        {
            return true;
        }
    }

    bool isTargetOutOfZoomRange(Transform target)
    {
        Bounds bound = new Bounds(targetMidpoint, cameraZoomRange); //creates AABB bounding box with player one as center
        if (!bound.Contains(target.position))
        {
            return true;
        }
        return false;
    }


    /*
    void checkIfCameraIsTooSmall()
    {
        Bounds idealCameraBox = getCameraBoxWithAllTargetsInside(targets);
        if(idealCameraBox.size.y > maxZoom*2) //if not all the targets would fit into the cameraBox
        {
            targetsThatDontFit.Clear();
            for (int i = 0; i < targets.Length; i++)
            {
                if (!idealCameraBox.Contains(targets[i].position))
                {
                    targetsThatDontFit.Add(targets[i]);
                }
            }
            if (targetsThatDontFit.Count > 0)
            {

            }
        }
        
    }*/


    void changeColors()
    {
        foreach(Transform t in targets)
        {
            SpriteRenderer rend = t.GetComponent<SpriteRenderer>();
            if(outSider == t)
            {
                rend.color = Color.blue;
            }
            else if (targetsOnCamera.Contains(t))
            {
                rend.color = Color.green;
            }else if (targetsThatDontFit.Contains(t))
            {
                rend.color = Color.red;
            }
            else
            {
                rend.color = Color.white;
            }
            
        }
        
    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     GET MIDPOINT OF TARGETS()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   /* void getMidpointOfTargets()
    {
          if (targets.Length == 1)
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
          }

        //CameraZoomForMultiplePlayers(targets);
        
        getMidpoint(targets);

        if(targets.Length > 1)
        {
            float[] camDist = new float[targets.Length];
            for (int i = 0; i < targets.Length; i++)
            {              
                camDist[i] = Vector3.Distance(targetMidpoint, targets[i].position);
            }
            CameraZoomForMultiplePlayers(targets);

        } else if(targets.Length == 1)
        {
            float camDist = Vector3.Distance(targetMidpoint, targets[0].position);
            CameraZoomForOnePlayer(camDist);
        }
       
        
        

    }*/

    void newCamera()
    {
        //CameraZoomForMultiplePlayers(targets);
        camCorner = new Vector3(targetMidpoint.x + (cam.orthographicSize * cam.aspect), targetMidpoint.y - (cam.orthographicSize), 0);
        cameraSizeDist = Vector3.Distance(targetMidpoint, camCorner); // - offset;
        cameraZoomRange = new Vector3((cam.orthographicSize * 2 * cam.aspect) * 0.75f, cam.orthographicSize, 0);
        CameraTargetUpdate(targets);
        cameraCalculations(targetsOnCamera.ToArray());
        //cameraCalculations(targets);
    }

    void cameraCalculations(Transform[] targets)
    {
        getMidpoint(targets);
        //float camDistOne = Vector3.Distance(targetMidpoint, targets[0].position);
        zoomCamera(GetPlayerClosestToZoom(targets));
        //CameraZoomForOnePlayer(camDistOne);
    }

    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     CAMERA ZOOM FOR MULTIPLE PLAYERS()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    
    //if camera distance too big
    //take furthest player out of calculations
    //if camera distance still too big
    //take furthest player out of calculations

    //if player in range again, take player into calculations again

    Transform GetPlayerClosestToZoom(Transform[] targets) 
    {
        Transform furthestTarget = null;
        float furthestDist = 0;
        for (int i = 0; i < targets.Length; i++)
        {
            float thisDist = Vector3.Distance(targetMidpoint, targets[i].position);
                if (furthestTarget == null || thisDist > furthestDist)
                {
                    furthestTarget = targets[i];
                    furthestDist = thisDist;
                }
        }
        return furthestTarget;
    }
    
    Transform GetPlayerFurthestAway(Transform[] targets)
    {
        Transform furthestTarget = null;
        float furthestDist = 0;
        foreach (Transform target in targets)
        {
            float thisDistance = 0;
            foreach (Transform other in targets)
            {
                if (target != other)
                {
                    thisDistance += Vector3.Distance(target.position, other.position);
                }
            }
            if (thisDistance > furthestDist || furthestTarget == null)
            {
                furthestDist = thisDistance;
                furthestTarget = target;
            }
        }
        outSider = furthestTarget;
        return furthestTarget;
    }


    void CameraTargetUpdate(Transform[] target)
    {
        Transform furthestTarget = GetPlayerFurthestAway(target);
        targetsOnCamera.Clear();
        targetsThatDontFit.Clear();
        for (int i = 0; i < target.Length; i++)
        {
            if (furthestTarget == target[i])
            {
                if (!isTargetOnCamera(furthestTarget))
                {
                    targetsThatDontFit.Add(furthestTarget);
                }
                else
                {
                    targetsOnCamera.Add(target[i]);
                }
            }
            else
            {
                targetsOnCamera.Add(target[i]);
            }
        }
        if (targetsOnCamera.Count <= 0)
        {
            targetsOnCamera.Add(target[0]);
        }
        else if(targetsThatDontFit.Count > 0)
        {
            if (AllTargetsOnCamera(targetsOnCamera.ToArray())!= true)
            {
                CameraTargetUpdate(targetsOnCamera.ToArray());
            }
        }
    }


    /*
    void CameraTargetUpdate(Transform [] target)
    {
        camCorner = new Vector3(targetMidpoint.x + (cam.orthographicSize * cam.aspect), targetMidpoint.y - (cam.orthographicSize), 0);
        cameraSizeDist = Vector3.Distance(targetMidpoint, camCorner) - offset;

        Transform furthestTarget = GetPlayerFurthestAway(target);
        //float furthestDist = 0;
        float[] camDist = new float[target.Length];
        targetsOnCamera.Clear();
        targetsThatDontFit.Clear();
        for (int i = 0; i < target.Length; i++)
        {
            if(furthestTarget == target[i])
            {
                camDist[i] = Vector3.Distance(targetMidpoint, target[i].position);
                if (camDist[i] > cameraSizeDist)
                {
                    targetsThatDontFit.Add(furthestTarget);
                }
                else
                {
                    targetsOnCamera.Add(target[i]);
                }
            }
            else
            {
                targetsOnCamera.Add(target[i]);
            }        
        }
        if (targetsOnCamera.Count <= 0)
        {
            targetsOnCamera.Add(target[0]);
        }
    }
    */

    
    /*private void CameraZoomForMultiplePlayers(Transform [] target)
    {
        Transform furthestTarget = null;
        float furthestDist = 0;
        float[] camDist = new float[target.Length];
        targetsOnCamera.Clear();
        targetsThatDontFit.Clear();
        for (int i = 0; i < target.Length; i++)
        {
            camDist[i] = Vector3.Distance(targetMidpoint, target[i].position);
            if (camDist[i] > cameraSizeDist)
            {
                if (furthestTarget == null)
                {
                    furthestTarget = target[i];
                    furthestDist = camDist[i];
                }
                else if (camDist[i] > furthestDist)
                {
                    furthestTarget = target[i];
                    furthestDist = camDist[i];
                }else
                {
                    targetsOnCamera.Add(target[i]);
                }               
            }else
            {
                targetsOnCamera.Add(target[i]);
            }
        }
        if(targetsOnCamera.Count <= 0)
        {
            targetsOnCamera.Add(target[0]);
        }
        if(furthestTarget != null)
        {
            targetsThatDontFit.Add(furthestTarget);
        }

        

        /* targetsThatDontFit.Clear();
         Transform furthestTarget = null;
         float furthestDist = 0;
         float[] camDist = new float[target.Length];
         for (int i = 0; i < target.Length; i++)
         {
             camDist[i] = Vector3.Distance(targetMidpoint, target[i].position);
             if(camDist[i] > cam.orthographicSize)
             {
                 if(furthestTarget == null)
                 {
                     furthestTarget = target[i];
                     furthestDist = camDist[i];
                 }
                 if(camDist[i] > furthestDist)
                 {
                     targetsThatDontFit.Add(furthestTarget);
                     furthestTarget = target[i];
                     furthestDist = camDist[i];
                 }
                 else
                 {
                     targetsOnCamera.Add(target[i]);
                 }
             }
             else
             {
                 targetsOnCamera.Add(target[i]); //targets that fit into camera zoom
             }
         }
         if(targetsOnCamera.Count == 0)
         {
             targetsOnCamera.Add(target[0]);          
         }
         getMidpoint(targetsOnCamera.ToArray());

         float camDistOne = Vector3.Distance(targetMidpoint, targetsOnCamera[0].position);
         CameraZoomForOnePlayer(camDistOne);


         */


        //getMidpointOfTargets(targetsOnCamera.ToArray());



        /* int camDistsTooBig = 0; //number of players who are out of camera bounds
         for(int i = 0; i<camDist.Length; i++)
         {
             if(camDist[i] > maxZoom)
             {
                 camDistsTooBig++;
             }
         }
         if(camDistsTooBig > 0)
         {

         }*/




        /*if (camDist > cam.orthographicSize && cam.orthographicSize <= maxZoom)
        {
            cam.orthographicSize += camDist - cam.orthographicSize;
            if (cam.orthographicSize > maxZoom)
            {
                cam.orthographicSize = maxZoom;
            }
        }
        else if (camDist < cam.orthographicSize && cam.orthographicSize >= minZoom)
        {
            cam.orthographicSize -= cam.orthographicSize - camDist;
            if (cam.orthographicSize < minZoom)
            {
                cam.orthographicSize = minZoom;
            }
        }
    }
        */
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     CAMERA ZOMM FOR ONE PLAYER()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   float getDistBtwTargetAndZoomRange(Transform target)
   {
        Bounds bound = new Bounds(targetMidpoint, cameraZoomRange); //creates AABB bounding box with player one as center
        
        float dist = Vector3.Distance(bound.ClosestPoint(target.position), target.position);
      
        return dist;
        //float dist = bound.SqrDistance(target.position);
        
        //return dist;
   }


    private void zoomCamera(Transform target)
    {

            zoomOut = isTargetOutOfZoomRange(target);
            if (zoomOut && cam.orthographicSize < maxZoom)
            {
            cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, cam.orthographicSize+getDistBtwTargetAndZoomRange(target), ref smoothZoomVelocity, smoothTime);
                if (cam.orthographicSize > maxZoom) //else if camera bigger than max zoom
                {
                    cam.orthographicSize = maxZoom; //camera = max zoom
                }
            }else if(!zoomOut && cam.orthographicSize > minZoom)
            {
            //Debug.Log(getDistBtwTargetAndZoomRange(target));
                cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, cam.orthographicSize - 0.5f, ref smoothZoomVelocity, smoothTime);
            if (cam.orthographicSize < minZoom) //else if camera bigger than max zoom
                {
                    cam.orthographicSize = minZoom; //camera = max zoom
                }
            }
        

    }
    

    /*
    private void CameraZoomForOnePlayer(float camDist)
    {
        if (camDist > cam.orthographicSize) //if player outside of camera 
        {
            if(cameraZoomRange.x <= maxZoom){ //but camera smaller than max zoom
                    cam.orthographicSize += camDist - cam.orthographicSize; //zoom out
                if (cam.orthographicSize > maxZoom) //else if camera bigger than max zoom
                {
                    cam.orthographicSize = maxZoom; //camera = max zoom
                }
            }
        }
        else if (camDist < cam.orthographicSize && targetsThatDontFit.Count == 0) //if player inside camera and camera bigger than min zoom
        {
            if(cam.orthographicSize >= minZoom)
            { 
                cam.orthographicSize -= cam.orthographicSize - camDist; //zoom in
                if (cam.orthographicSize < minZoom)
                {
                    cam.orthographicSize = minZoom;
                }
            }
           
        }
    }
    */


    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ///                                                                     GIZMOS()
    ///--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void OnDrawGizmos()
    {
       /*
        if (targetMidpoint != null)
        {
            Gizmos.DrawWireSphere(targetMidpoint, 2);
            Gizmos.DrawWireSphere(targetMidpoint, cameraSizeDist);
           // Gizmos.DrawWireSphere(targetMidpoint, cam.orthographicSize);
            //Gizmos.DrawWireSphere(targetMidpoint, cameraZoomRange);
            //Gizmos.DrawWireCube(targetMidpoint, new Vector3((cam.orthographicSize*2*cam.aspect)*0.75f, cam.orthographicSize, 0));
            Gizmos.DrawWireCube(targetMidpoint, cameraZoomRange);
            //Gizmos.DrawLine(GetPlayerClosestToZoom(targets).position, )
            foreach (Transform t in targets)
            {
                if(Vector3.Distance(targetMidpoint, t.position)> cameraSizeDist)
                {
                    Gizmos.color = Color.red;
                }
                else
                {
                    Gizmos.color = Color.green;
                }
                Gizmos.DrawLine(t.position, targetMidpoint);
                Gizmos.color = Color.white;
                for (int i = 0; i<targets.Length; i++)
                {
                    if(t != targets[i])
                    {
                        Gizmos.DrawLine(t.position, targets[i].position);
                    }
                    
                }
                
            }
        }*/

    }
}
