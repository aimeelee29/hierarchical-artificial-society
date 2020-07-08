using System;
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

    

    // Generates a position for agent to spawn to (GIVEN TWO VECTORS FROM TWO PARENTS). 
    // freeVectors represent free neighbouring cells to parents. If parent has no free neighbouring cell then will be (-1, -1)
    public static void GeneratePosition(GameObject childObj, Vector2Int freeVector1, Vector2Int freeVector2)
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
        //sets position to free vector chosen
        childObj.transform.position = gridLayout.CellToWorld(new Vector3Int(x, y, 0));
        //tells world that agent is in that cell
        world.WorldArray[x, y].OccupyingAgent = childObj.GetComponent<Agent>();
    }

    // Generates the Agent component for the object and sets values (RANDOM)
    // TO DO: once done all checks don't need to return Agent?
    

    // Generates the Agent component for the object and sets values (GIVEN TWO PARENTS)
    public static Agent CreateAgentComponent(GameObject agentObj, Agent parentOne, Agent parentTwo)
    {
        // adds Agent script and sets variables 
        Agent agentCom = agentObj.AddComponent<Agent>();
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

        return agentCom;
    }   
}


