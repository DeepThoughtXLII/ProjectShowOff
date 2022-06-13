using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class musicManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<SoundManager>().Play("ingameMusic");
        FindObjectOfType<SoundManager>().Play("windAmbience");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
