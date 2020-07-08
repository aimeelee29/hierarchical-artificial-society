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
     * METHODS
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
            GeneratePosition(agentObj);
            SetAgentVars(agentObj);    
        }
    }

    // Generates a position for agent to spawn to (RANDOM). First checks if there is already an agent there. Works recursively.
    public static void GeneratePosition(GameObject agentObj)
    {
        // generate random grid position
        int x = UnityEngine.Random.Range(0, World.Rows);
        int y = UnityEngine.Random.Range(0, World.Cols);

        // if no agent currently in that position then set transform to that position
        if (world.WorldArray[x, y].OccupyingAgent == null)
        {
            agentObj.transform.position = gridLayout.CellToWorld(new Vector3Int(x, y, 0));
            // Set the agent as occupying agent in the grid location
            world.WorldArray[x, y].OccupyingAgent = agentObj.GetComponent<Agent>();
            // Set the cell position in agent component
            agentObj.GetComponent<Agent>().CellPosition = new Vector2Int(x, y);
            return;
        }
        // else repeat process
        else
            GeneratePosition(agentObj);
    }

    // Sets Agent variables
    public static void SetAgentVars(GameObject agentObj)
    {
        Agent agentCom = agentObj.GetComponent<Agent>();
        //TODO - finish this. And also use values used for trading section of sugarscape. Am putting in dummy values currently.
        agentCom.Sugar = UnityEngine.Random.Range(25, 51); // max exclusive
        agentCom.Spice = UnityEngine.Random.Range(25, 51);
        agentCom.SugarInit = agentCom.Sugar;
        agentCom.SpiceInit = agentCom.Spice;
        agentCom.SugarMetabolism = UnityEngine.Random.Range(1, 6);
        agentCom.SpiceMetabolism = UnityEngine.Random.Range(1, 6);
        agentCom.VisionHarvest = UnityEngine.Random.Range(1, 6);
        int sex = UnityEngine.Random.Range(1, 3);
        if (sex == 1)
            agentCom.Sex = SexEnum.Female;
        else if (sex == 2)
            agentCom.Sex = SexEnum.Male;
        agentCom.IsAlive = true;
        agentCom.ChildBearingBegins = UnityEngine.Random.Range(12, 16);
        agentCom.ChildBearingEnds = UnityEngine.Random.Range(35, 46);
        int lifespan = UnityEngine.Random.Range(60, 101);
        agentCom.Lifespan = lifespan;
        agentCom.Age = UnityEngine.Random.Range(1, lifespan + 1);
        //agentCom.dominance =
        //agentCom.influence = 
        return;
    }

}
