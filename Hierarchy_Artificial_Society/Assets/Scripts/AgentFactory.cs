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
            GeneratePosition(agentObj);
            SetAgentVars(agentObj);    
        }
    }

    // Generates a position for agent to spawn to (RANDOM). First checks if there is already an agent there. Works recursively.
    public void GeneratePosition(GameObject agentObj)
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
    public void SetAgentVars(GameObject agentObj)
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

    // Generates the Agent component for the object and sets values (from inheritance given two parents)
    public void CreateAgentComponent(GameObject agentObj, Agent parentOne, Agent parentTwo)
    {
        Agent agentCom = agentObj.GetComponent<Agent>();

        // initial endowment is half of mother's + half of father's initial endowment
        agentCom.Sugar = (parentOne.SugarInit / 2) + (parentTwo.SugarInit / 2);
        agentCom.Spice = (parentOne.SpiceInit / 2) + (parentTwo.SugarInit / 2);
        agentCom.SugarInit = agentCom.Sugar;
        agentCom.SpiceInit = agentCom.Spice;

        //then randomely take one of parents' attributes
        if (UnityEngine.Random.Range(1, 3) == 1)
            agentCom.SugarMetabolism = parentOne.SugarMetabolism;
        else
            agentCom.SugarMetabolism = parentTwo.SugarMetabolism;
        if (UnityEngine.Random.Range(1, 3) == 1)
            agentCom.SpiceMetabolism = parentOne.SpiceMetabolism;
        else
            agentCom.SpiceMetabolism = parentTwo.SpiceMetabolism;
        if (UnityEngine.Random.Range(1, 3) == 1)
            agentCom.VisionHarvest = parentOne.VisionHarvest;
        else
            agentCom.VisionHarvest = parentTwo.VisionHarvest;
        if (UnityEngine.Random.Range(1, 3) == 1)
            agentCom.ChildBearingBegins = parentOne.ChildBearingBegins;
        else
            agentCom.ChildBearingBegins = parentTwo.ChildBearingBegins;
        if (UnityEngine.Random.Range(1, 3) == 1)
            agentCom.ChildBearingEnds = parentOne.ChildBearingEnds;
        else
            agentCom.ChildBearingEnds = parentTwo.ChildBearingEnds;

        // sex and lifespan is still random
        int sex = UnityEngine.Random.Range(1, 3);
        if (sex == 1)
            agentCom.Sex = SexEnum.Female;
        else if (sex == 2)
            agentCom.Sex = SexEnum.Male;
        agentCom.Lifespan = UnityEngine.Random.Range(60, 101);
        agentCom.IsAlive = true;
        agentCom.Age = 0;

        return;
    }
}
