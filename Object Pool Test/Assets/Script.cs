using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script : MonoBehaviour
{
    public static List<GameObject> everyone = new List<GameObject>();
    public static List<Script> scripts = new List<Script>();

    public int x;
    // Start is called before the first frame update
    void Start()
    {
        x = 1;
        everyone.Add(this.gameObject);
        scripts.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
