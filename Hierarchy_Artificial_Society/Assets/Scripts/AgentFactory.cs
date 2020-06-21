//using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AgentFactory : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //TO DO: will change the for loop when all working.
        for (int i = 0; i < 100; ++i)
        {
            //creates agent gameobject 
            GameObject agentObj = new GameObject("Agent");
            //applies sprite and colour
            SpriteRenderer renderer = agentObj.AddComponent<SpriteRenderer>();
            renderer.sprite = Resources.Load<Sprite>("mm_red");
            renderer.color = Color.red;
            //places object in random position
            int x = Random.Range(0, 25);
            int y = Random.Range(0, 15);
            Vector3 randomPosition = new Vector3(x, y, 0);
            agentObj.transform.position = randomPosition;
            //applies order in layer (so it can be seen)
            renderer.sortingOrder = 1;
            Agent agentCom = CreateComponent(agentObj);
            //apply Agent tag
            agentObj.tag = "Agent";
        }
    }

    public static Agent CreateComponent(GameObject obj)
    {
        Agent agentCom = obj.AddComponent<Agent>();
        //assigns variables below.
        //TODO - finish this. And also use values used for trading section of sugarscape. Am putting in dummy values currently.
        agentCom.sugar = 10;
        agentCom.spice = 4;
        agentCom.sugarMetabolism = 1;
        agentCom.spiceMetabolism = 1;
        agentCom.vision = 5;
        agentCom.isAlive = true;

        return agentCom;

        /*
         * from book:
         * Here vision is uniformly distributed in the agent population between I and 10, while metabolism for each of the two commodities is unifom1ly distributed between I 
         * and 5. Agents move between the two mountains. Before trade is introduced, the carrying capacity is lower as there are two ways to die. 
         * 
         * 
         * 
         */
    }
}
