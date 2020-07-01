using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 
 *  Class which handles initial spawning of agents
 *  
 */
 
public class AgentFactory : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //TO DO: will change the for loop when all working.
        for (int i = 0; i < 1; ++i)
        {
            GameObject agentObj = CreateAgent.CreateAgentObject();
            //generates random position for agent to spawn into
            CreateAgent.GeneratePosition(agentObj);
            //adds Agent script to GameObject and sets values
            Agent agentCom = CreateAgent.CreateAgentComponent(agentObj);
        }
    }    
}
