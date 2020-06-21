using System.Collections;
using System.Collections.Generic;
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

        return agentCom;
    }
}
