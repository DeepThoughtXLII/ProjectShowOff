using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraFollow : MonoBehaviour
{

    Transform target;

    public float smoothSpeed = 0.125f;

    private Vector3 velocity = Vector3.one;

    public float distanceDamp = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        FollowTarget();
    }

    void FollowTarget()
    {
        if (transform.position != target.position)
        {
            //Vector2 distanceT = transform.position - target.position;
            //Vector2 velocity = distance.normalized * smoothSpeed;
            Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, target.position, ref velocity, distanceDamp);
            transform.position = smoothedPosition;
        }
    }
}
