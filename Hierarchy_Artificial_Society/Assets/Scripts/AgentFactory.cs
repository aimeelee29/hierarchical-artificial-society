using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 
 *  Class which handles spawning of agents - both initial spawn and creation of children
 *  
 */
 
public class AgentFactory : MonoBehaviour
{
    /*
     * REFERENCES
     */

    // Holds reference to World since we need to know if an agent as already spawned in that location
    private static World world;
    private static GridLayout gridLayout;

    // Holds reference to Toggle to feed in to initvars on initial spawn
    private Toggle toggle;

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
        toggle = Resources.Load<Toggle>("ScriptableObjects/Toggle");

        // Spawn Agents
        for (int i = 0; i < numberOfAgents; ++i)
        {
            GameObject agentObj = GameObject.Instantiate(agentPrefab);
            Agent agentComponent = agentObj.GetComponent<Agent>();
            if (toggle.GetRandomPlacement())
            {
                agentComponent.InitPosition();
            }
            else
            {
                agentComponent.InitPosition(numberOfAgents, i);
            }
            agentComponent.InitVars(toggle);
            Agent.LiveAgents.Add(agentComponent);
        }
    }

    /*
    * METHOD FOR CREATING CHILD
    */
    // Sits in this class so we only have to set the prefab once in the inspector

    public GameObject CreateChild()
    {
        GameObject agentObj;

        if (Agent.AvailableAgents.Count > 0)
        {
            // Sets agentObj as the first entry in the list (object pooling)
            agentObj = Agent.AvailableAgents[0];
            // Removes it from available list
            Agent.AvailableAgents.Remove(agentObj);
            // Activates it
            agentObj.SetActive(true);
        }
        else
        {
            agentObj = GameObject.Instantiate(agentPrefab);
        }
        return agentObj;
    }
}
