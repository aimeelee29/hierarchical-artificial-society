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
    /*
     * REFERENCES
     */

    //Holds reference to World since we need to know if an agent as already spawned in that location
    private static World world;
    private static GridLayout gridLayout;

    // Can set this to the Agent prefab from inspector
    [SerializeField] private GameObject agentPrefab = null;
    // Can set this to number of agents you want to spawn in inspector
    [SerializeField] private int numberOfAgents = 0;

    /*
    * METHOD FOR INITIAL SPAWN
    */

    // Start is called before the first frame update
    void Start()
    {
        // Set references
        world = GameObject.Find("World").GetComponent<World>();
        gridLayout = GameObject.Find("Grid").GetComponent<GridLayout>();

        //TO DO: will change the for loop when all working.
        for (int i = 0; i < numberOfAgents; ++i)
        {
            GameObject agentObj = GameObject.Instantiate(agentPrefab);
            agentObj.GetComponent<Agent>().InitPosition();
            agentObj.GetComponent<Agent>().InitVars();
            Agent.LiveAgents.Add(agentObj.GetComponent<Agent>());
            Agent.AllAgents.Add(agentObj.GetComponent<Agent>());
        }
    }

    /*
    * METHOD FOR CREATING CHILD
    */
    // Sits in this class so we only have to set the prefab once

    public GameObject CreateChild()
    {
        GameObject agentObj;

        if (Agent.AvailableAgents.Count > 0)
        {
            // Sets agentObj as the first entry in the list (object pooling)
            agentObj = Agent.AvailableAgents[0];
            // Removes it from available list
            Agent.AvailableAgents.Remove(agentObj);
        }
        else
        {
            agentObj = GameObject.Instantiate(agentPrefab);
        }
        return agentObj;
    }
}
