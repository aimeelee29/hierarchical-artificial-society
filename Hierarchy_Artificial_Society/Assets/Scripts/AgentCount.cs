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

    //List with count of agents for each time step
    public List<int> agentCount = new List<int>();
    int count;

    void Awake ()
    {
        //hold reference to graph
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
        //for graph display
        if (agentCount.Count <= 100)
            graphScript.CreateGraph(agentCount);
    }

    public void Count()
    {
        //count of agents
        count = GameObject.FindGameObjectsWithTag("Agent").Length;
        //add count to the list
        agentCount.Add(count);
    }

    /*
    public void SaveXML()
    {
        XmlSerializer save = new XmlSerializer(typeof(agentCount));
        FileStream path 
    }
    */
}
