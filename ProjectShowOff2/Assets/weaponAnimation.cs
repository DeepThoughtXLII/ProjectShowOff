using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponAnimation : MonoBehaviour
{

    Animator anim;
    public Transform origin;
    Vector3 direction;
    float shootLength;

    Vector3 position;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if(clip.name == "shoot")
            {
                shootLength = clip.length;
            }
        }
    }

    private void Update()
    {
        
    }


    public float getAnimationLength()
    {
        return shootLength;
    }

    public void playShootAnimation()
    {
        anim.SetTrigger("shoot");
        //anim.ResetTrigger("shoot");
    }
    

    //get angle that it should be from vector3.one
    //get angle its currently at from vector3.one
    //if angle that it should be is smaller than the current, we substract
    //otherwise we add
    //move the difference between thses angles
    public void faceDirection(Vector3 position)
    {
        direction = position - transform.parent.position;
        direction.Normalize();
        this.position = position;
        Vector3 currentDirection = transform.position - transform.parent.position;
        currentDirection.Normalize();
        Debug.Log(currentDirection);
        float angleToMove = Vector3.SignedAngle(currentDirection, direction, new Vector3(0,0,1));
        float currentAngle = Vector3.Angle(new Vector3(1, 0, 0), currentDirection);
        //float angle = Mathf.Lerp(0f, angleToMove, Time.deltaTime * 0.5f);
       
       // Debug.Log($"angleItShouldBe: {angleItShouldBe} currentAngle: {currentAngle} und dann angleToMove:{angleToMove} ");
        //transform.RotateAround(transform.parent.position, new Vector3(0, 0, 1), -angleToMove);
       
        
              transform.RotateAround(transform.parent.position, new Vector3(0, 0, 1), angleToMove);
         

         









          /*
          if (Vector3.Angle())
          {
              transform.RotateAround(transform.parent.position, new Vector3(0, 0, 1), angle);
          }
          else
          {
              transform.RotateAround(transform.parent.position, new Vector3(0, 0, 1), -angle);
          }*/


        //transform.rotation = Quaternion.LookRotation(direction);
    }

    public Vector2 rotateVector(Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    public float angleBetweenVectors(Vector2 a, Vector2 b)
    {
        float x = b.x - a.x;
        float y = b.y - a.y;
        return Mathf.Atan2(y, x) * (180 / Mathf.PI);
    }




    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 point = (transform.position - transform.parent.position).normalized * 20;
        Gizmos.DrawLine(transform.parent.position, position);
    }
}
/*/*
        transform.position = Vector3.zero;
        float angleItShouldBe = angleBetweenVectors(direction, Vector2.one);
        lastAngle = angleBetweenVectors(transform.parent.position - transform.localPosition, Vector2.one);
        
        transform.localPosition = direction * offset;
        transform.localRotation = Quaternion.Euler(0, 0, angleItShouldBe);
        lastAngle = angleItShouldBe;




        /*  transform.position = Vector3.zero;
          lastAngle = angleBetweenVectors(transform.parent.position - transform.localPosition, Vector2.one);
          float angleItShouldBe = angleBetweenVectors(direction, Vector2.one);
          float newAngle = Mathf.Lerp(lastAngle, angleItShouldBe, Time.deltaTime*0.5f);
          Vector2 newPosition = rotateVector(direction, newAngle);
          transform.localPosition =  newPosition * offset;
          transform.localRotation = Quaternion.Euler(0, 0, angleItShouldBe);
          lastAngle = angleItShouldBe;
          //rotateVector()
        */


//Debug.Log(direction);

//float angleItShouldBe = Vector3.A//angleBetweenVectors(direction, Vector2.one);//Vector3.Angle(new Vector3(1, 0, 0), new Vector3(direction.x, direction.y, 0)); 
// float currentAngle = angleBetweenVectors(transform.position.normalized, Vector2.one);//Vector3.Angle(new Vector3(1, 0, 0), transform.position.normalized); //???????