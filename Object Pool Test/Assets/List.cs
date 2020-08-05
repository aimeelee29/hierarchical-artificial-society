using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class List : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject go = Script.everyone[0];
        Script script = go.GetComponent<Script>();
        print(Script.scripts[0].x);
        script.x = 2;
        Script.scripts.Add(script);
        print(Script.scripts[0].x);
        print(Script.scripts[1].x);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
