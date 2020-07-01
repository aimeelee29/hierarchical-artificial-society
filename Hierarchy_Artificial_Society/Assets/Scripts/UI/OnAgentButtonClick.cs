using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO; //file management

public class OnAgentButtonClick : MonoBehaviour
{
    private AgentCount agentCount;

    void Awake()
    {
        agentCount = GameObject.Find("Analysis: Agent Count").GetComponent<AgentCount>();
    }

    public void CallSaveXML()
    {
        agentCount.SaveXML();
    }
}
