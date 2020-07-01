using System;
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

    //Count that gets updated each time - for XML file so we can use append
    public int count;

    //instance of AgentCountList class which holds list - needs to be its own class for serialisation
    AgentCountList agentCountList;

    [Serializable]
    public class AgentCountList
    {
        //List with count of agents for each time step
        public List<int> agentCount = new List<int>();
    }

    void Start ()
    {
        //hold reference to graph
        graph = GameObject.Find("Agent Count Graph");
        graphScript = graph.GetComponent<AgentCountGraph>();

        //create instance of class
        agentCountList = new AgentCountList();
    }

    // Update is called once per frame
    void Update()
    {
        //call count method to count number of agents
        Count();
        //for graph display
        if (agentCountList.agentCount.Count <= 100)
            graphScript.CreateGraph(agentCountList.agentCount);
    }

    public void Count()
    {
        //count of agents
        count = GameObject.FindGameObjectsWithTag("Agent").Length;
        //add count to the list
        agentCountList.agentCount.Add(count);
    }

    public void SaveXML()
    {
        XmlSerializer save = new XmlSerializer(typeof(AgentCountList));
        FileStream path = new FileStream(Application.dataPath + "/XMLFiles/AgentCount.xml", FileMode.Create);
        save.Serialize(path, agentCountList);
        path.Close();
    }
}
