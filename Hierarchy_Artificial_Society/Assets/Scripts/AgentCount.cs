using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO; //file management

public class AgentCount : MonoBehaviour
{

    //Want to display on a graph
    private GameObject graph;
    private AgentCountGraph graphScript;


    public List<int> agentCount = new List<int>();
    int count;

    void Awake ()
    {
        graph = GameObject.Find("Agent Count Graph");
        graphScript = graph.GetComponent<AgentCountGraph>();
    }

    // Start is called before the first frame update
    void Start()
    {
 
    }

    // Update is called once per frame
    void Update()
    {
        Count();
        graphScript.CreateGraph(agentCount);
    }

    public void Count()
    {
        count = GameObject.FindGameObjectsWithTag("Agent").Length;
        //print(count);
        agentCount.Add(count);
    }
}
