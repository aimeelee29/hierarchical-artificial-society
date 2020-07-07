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
    [SerializeField] private GameObject agentPrefab;

    // Start is called before the first frame update
    void Start()
    {
        // Set references
        world = GameObject.Find("World").GetComponent<World>();
        gridLayout = GameObject.Find("Grid").GetComponent<GridLayout>();

        //TO DO: will change the for loop when all working.
        for (int i = 0; i < 15; ++i)
        {
            //GameObject agentObj = CreateAgent.CreateAgentObject();
            //generates random position for agent to spawn into
            //CreateAgent.GeneratePosition(agentObj);
            //adds Agent script to GameObject and sets values
            //Agent agentCom = CreateAgent.CreateAgentComponent(agentObj);
            GameObject.Instantiate(agentPrefab);
            GeneratePosition(agentPrefab);

        }
    }

    // Generates a position for agent to spawn to (RANDOM). First checks if there is already an agent there. Works recursively.
    public static void GeneratePosition(GameObject agentObj)
    {
         
        //generate random grid position
        int x = UnityEngine.Random.Range(0, world.GetRows() - 1);
        int y = UnityEngine.Random.Range(0, world.GetCols() - 1);

        //if no agent currently in that position then set transform to that position
        if (world.checkAgent(x, y) == false)
        {
            agentObj.transform.position = gridLayout.CellToWorld(new Vector3Int(x, y, 0));
            world.worldArray[x, y].SetAgent(agentObj);
            return;
        }
        //else repeat process
        else
            GeneratePosition(agentObj);
    }

}
