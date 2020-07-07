using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 
 * Class which handles agent's ability to identify who their neighbours are.
 * Needed for trading and reproduction.
 * 
 */

public class NeighbourVision : MonoBehaviour
{
    // reference to agent
    private Agent agent;

    void Start()
    {
        agent = GetComponent<Agent>();
    }

    // Update is called once per frame
    void Update()
    {
        // Sets the agent's neighbour list 
        agent.SetNeighbours(FindNeighbours());
    }

    // Method to find neighbours
    private List<Agent> FindNeighbours()
    {
        //Generate array of colliders within radius (set to vision)
        Collider2D[] colliderList = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), agent.GetVisionNeighbour());

        //Create empty List
        List<Agent> neighbourAgentList = new List<Agent>();

        //goes through each collider within radius
        foreach (Collider2D neighbour in colliderList)
        {
            // if collider is not attached to an agent then skip this iteration (as overlapcircleall will also catch collider for tilemap) 
            if (neighbour.tag != "Agent")
                continue;

            //get agent from object and add to list
            Agent agent = neighbour.gameObject.GetComponent<Agent>();
            if (agent.isAlive)
                neighbourAgentList.Add(agent);
        }
        return neighbourAgentList;
    }
}
