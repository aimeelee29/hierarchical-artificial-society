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

    /*
    * METHODS FOR INITIAL SPAWN
    */

    // Start is called before the first frame update
    void Start()
    {
        // Set references
        world = GameObject.Find("World").GetComponent<World>();
        gridLayout = GameObject.Find("Grid").GetComponent<GridLayout>();

        //TO DO: will change the for loop when all working.
        for (int i = 0; i < 1; ++i)
        {
            GameObject agentObj = GameObject.Instantiate(agentPrefab);
            agentObj.GetComponent<Agent>().InitPosition();
            agentObj.GetComponent<Agent>().InitVars();
            Agent.LiveAgents.Add(agentObj.GetComponent<Agent>());
        }
    }

    
    /*
     * METHODS FOR CREATING CHILD AGENT
     */

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

    public void GenerateChildPosition(GameObject childObj, Vector2Int freeVector1, Vector2Int freeVector2)
    {
        // If neither parent have a free neighbouring cell then return
        if (freeVector1.x == -1 && freeVector2.x == -1)
            return;

        int x;
        int y;

        // Position for child is one of the empty neighbouring cells to one of the parents
        if (freeVector1.x != -1)
        {
            x = freeVector1.x;
            y = freeVector1.y;
        }
        else
        {
            x = freeVector2.x;
            y = freeVector2.y;
        }
        // sets position to free vector chosen
        childObj.transform.position = gridLayout.CellToWorld(new Vector3Int(x, y, 0));
        // tells world that agent is in that cell
        world.WorldArray[x, y].OccupyingAgent = childObj.GetComponent<Agent>();
    }
}
