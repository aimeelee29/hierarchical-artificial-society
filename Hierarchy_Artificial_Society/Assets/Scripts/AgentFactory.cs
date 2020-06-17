//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentFactory : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //TO DO: will change the for loop when all working. Currently only spawning two agents.
        for (int i = 0; i < 2; ++i)
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
            
        }
    }

    public static Agent CreateComponent(GameObject obj)
    {
        Agent agentCom = obj.AddComponent<Agent>();
        //assigns variables below. Only have one in now to check
        //TODO - finish this. And also use values used for trading section of sugarscape.
        agentCom.sugar = Random.Range(5, 25);
        agentCom.spice = Random.Range(5, 25);
        agentCom.sugarMetabolism = Random.Range(1, 4);
        agentCom.spiceMetabolism = Random.Range(1, 4);
        agentCom.vision = Random.Range(1, 6);
        agentCom.isAlive = true;

        return agentCom;
    }
}
