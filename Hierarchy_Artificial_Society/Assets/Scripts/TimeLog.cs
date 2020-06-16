using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Here I am testing how to log time in Unity

public class TimeLog : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Frame " + Time.frameCount);
    }
}
