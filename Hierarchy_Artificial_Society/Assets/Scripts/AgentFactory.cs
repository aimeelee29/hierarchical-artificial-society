using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentFactory : MonoBehaviour
{

    //Holds reference to World since we need to know if an agent as already spawned in that location
    private GameObject objWorld;
    private World world;
    private GameObject objGrid;
    private GridLayout gridLayout;

    // Start is called before the first frame update
    void Start()
    {
        objWorld = GameObject.Find("World");
        world = objWorld.GetComponent<World>();
        objGrid = GameObject.Find("Grid");
        gridLayout = objGrid.GetComponent<GridLayout>();


        //TO DO: will change the for loop when all working.
        for (int i = 0; i < 200; ++i)
        {
            //creates agent gameobject 
            GameObject agentObj = new GameObject("Agent");

            //applies sprite and colour
            SpriteRenderer renderer = agentObj.AddComponent<SpriteRenderer>();
            renderer.sprite = Resources.Load<Sprite>("mm_red");
            renderer.color = Color.red;

            //generates random position for agent to spawn into
            generatePosition(agentObj);


            //applies order in layer (so it can be seen)
            renderer.sortingOrder = 1;
            Agent agentCom = CreateComponent(agentObj);

            //apply Agent tag
            agentObj.tag = "Agent";
        }
    }

    private Agent CreateComponent(GameObject obj)
    {
        Agent agentCom = obj.AddComponent<Agent>();
        //assigns variables below.
        //TODO - finish this. And also use values used for trading section of sugarscape. Am putting in dummy values currently.
        agentCom.sugar = Random.Range(25, 50);
        agentCom.spice = Random.Range(25, 50);
        agentCom.sugarInit = agentCom.sugar;
        agentCom.spiceInit = agentCom.spice;
        agentCom.sugarMetabolism = Random.Range(1, 5);
        agentCom.spiceMetabolism = Random.Range(1, 5);
        agentCom.vision = Random.Range(1, 5);
        if (Random.Range(1, 2) == 1)
            agentCom.sex = "Female";
        else
            agentCom.sex = "Male";
        agentCom.isAlive = true;
        agentCom.childBearingBegins = Random.Range(12, 15);
        agentCom.childBearingEnds = Random.Range(35, 45);
        agentCom.lifespan = Random.Range(60, 100);
        //print(agentCom.cellPosition);
        return agentCom;
    }

    //generates a position for agent to spawn to. First checks if there is already an agent there. Works recursively.
    private void generatePosition(GameObject agentObj)
    {
        //generate random grid position
        int x = Random.Range(0, world.GetRows()-1);
        int y = Random.Range(0, world.GetCols()-1);

        //if no agent currently in that position then set transform to that position
        if (world.checkAgent(x, y) == false)
        {
            agentObj.transform.position = gridLayout.CellToWorld(new Vector3Int(x, y, 0));
            world.worldArray[x, y].SetAgent(agentObj);
            return;
        }

        //else repeat process
        else
            generatePosition(agentObj);
    }
}
