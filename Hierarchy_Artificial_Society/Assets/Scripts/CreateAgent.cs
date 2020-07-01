using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 
 * Static class which handles the creation of agents. 
 * This includes creating the object, applying a position on the grid, and setting the values for the agent component.
 * Used by AgentFactory to spawn initial set of agents and by Agent to create children through agent reproduction.
 * 
 */

public static class CreateAgent
{
    //Holds reference to World since we need to know if an agent as already spawned in that location
    private static GameObject objWorld = GameObject.Find("World");
    private static World world = objWorld.GetComponent<World>();
    private static GameObject objGrid = GameObject.Find("Grid");
    private static GridLayout gridLayout = objGrid.GetComponent<GridLayout>();

    //Method for creating an agent gameobject
    public static GameObject CreateAgentObject()
    {
        //creates agent gameobject 
        GameObject agentObj = new GameObject("Agent");

        //applies sprite and colour
        SpriteRenderer renderer = agentObj.AddComponent<SpriteRenderer>();
        renderer.sprite = Resources.Load<Sprite>("mm_red");
        renderer.color = Color.red;

        //applies order in layer (so it can be seen)
        renderer.sortingOrder = 1;

        //apply Agent tag
        agentObj.tag = "Agent";

        //apply circle collider
        CircleCollider2D collider = agentObj.AddComponent<CircleCollider2D>();

        return agentObj;
    }

    // Generates a position for agent to spawn to (RANDOM). First checks if there is already an agent there. Works recursively.
    public static void GeneratePosition(GameObject agentObj)
    {
        //generate random grid position
        int x = Random.Range(0, world.GetRows() - 1);
        int y = Random.Range(0, world.GetCols() - 1);

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

    // Generates a position for agent to spawn to (GIVEN TWO VECTORS FROM TWO PARENTS). 
    // Vectors represent free neighbouring cells to parent. If no free cell then will be (-1, -1)
    public static void GeneratePosition(GameObject agentObj, Vector2Int freeVector1, Vector2Int freeVector2)
    {
        //if neither parent have a free neighbouring cell then return
        if (freeVector1.x == -1 && freeVector2.x == -1)
            return;

        int x;
        int y;

        //position for child is one of the empty neighbouring cells to one of the parents
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
        agentObj.transform.position = gridLayout.CellToWorld(new Vector3Int(x, y, 0));
        world.worldArray[x, y].SetAgent(agentObj);
    }

    // Generates the Agent component for the object and sets values (RANDOM)
    // TO DO: once done all checks don't need to return Agent?
    public static Agent CreateAgentComponent(GameObject obj)
    {
        Agent agentCom = obj.AddComponent<Agent>();
        //assigns variables below.
        //TODO - finish this. And also use values used for trading section of sugarscape. Am putting in dummy values currently.
        agentCom.sugar = Random.Range(25, 51); // max exclusive
        agentCom.spice = Random.Range(25, 51);
        agentCom.sugarInit = agentCom.sugar;
        agentCom.spiceInit = agentCom.spice;
        agentCom.sugarMetabolism = Random.Range(1, 6);
        agentCom.spiceMetabolism = Random.Range(1, 6);
        agentCom.vision = Random.Range(1, 6);
        int sex = Random.Range(1, 3);
        if (sex == 1)
            agentCom.SetSex("Female");
        else if(sex == 2)
            agentCom.SetSex("Male");
        agentCom.isAlive = true;
        agentCom.childBearingBegins = Random.Range(12, 16);
        agentCom.childBearingEnds = Random.Range(35, 46);
        int lifespan = Random.Range(60, 101);
        agentCom.lifespan = lifespan;
        agentCom.age = Random.Range(1, lifespan + 1);
        //agentCom.dominance =
        //agentCom.influence = 
        return agentCom;
    }

    // Generates the Agent component for the object and sets values (GIVEN TWO PARENTS)
    public static void CreateAgentComponent(GameObject obj, Agent parentOne, Agent parentTwo)
    {
        // adds Agent script and sets variables 
        Agent agentCom = obj.AddComponent<Agent>();
        // initial endowment is half of mother's + half of father's initial endowment
        agentCom.sugar = parentOne.sugarInit / 2 + parentTwo.sugarInit / 2;
        agentCom.spice = parentOne.spiceInit / 2 + parentTwo.sugarInit / 2;
        agentCom.sugarInit = agentCom.sugar;
        agentCom.spiceInit = agentCom.spice;
        if (Random.Range(1, 2) == 1)
            agentCom.sugarMetabolism = parentOne.sugarMetabolism;
        else
            agentCom.sugarMetabolism = parentTwo.sugarMetabolism;
        if (Random.Range(1, 2) == 1)
            agentCom.spiceMetabolism = parentOne.spiceMetabolism;
        else
            agentCom.spiceMetabolism = parentTwo.spiceMetabolism;
        if (Random.Range(1, 2) == 1)
            agentCom.vision = parentOne.vision;
        else
            agentCom.vision = parentTwo.vision;
        if (Random.Range(1, 2) == 1)
            agentCom.childBearingBegins = parentOne.childBearingBegins;
        else
            agentCom.childBearingBegins = parentTwo.childBearingBegins;
        if (Random.Range(1, 2) == 1)
            agentCom.childBearingEnds = parentOne.childBearingEnds;
        else
            agentCom.childBearingEnds = parentTwo.childBearingEnds;
        // sex is still random
        if (Random.Range(1, 2) == 1)
            agentCom.SetSex("Female");
        else
            agentCom.SetSex("Male");
        agentCom.isAlive = true;
        agentCom.lifespan = Random.Range(60, 100);
        agentCom.age = 0;
    }   
}


