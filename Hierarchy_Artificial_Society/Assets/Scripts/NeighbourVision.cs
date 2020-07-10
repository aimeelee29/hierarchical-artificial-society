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
    // Method to find neighbours
    public List<Agent> FindNeighbours(Agent agent)
    {
        //Generate array of colliders within radius (set to vision)
        //does transform need to come from agent??
        Collider2D[] colliderList = Physics2D.OverlapCircleAll(new Vector2(agent.transform.position.x, agent.transform.position.y), agent.VisionNeighbour);
        //print("collider list length = " + colliderList.Length);

        //Create empty List
        List<Agent> neighbourAgentList = new List<Agent>();

        //goes through each collider within radius
        foreach (Collider2D neighbour in colliderList)
        {
            // if collider is not attached to an agent then skip this iteration (as overlapcircleall will also catch collider for tilemap) 
            // also removes itself
            if (neighbour.tag != "Agent" || neighbour == agent)
                continue;

            if (agent.IsAlive) //may not need this now
                neighbourAgentList.Add(neighbour.GetComponent<Agent>());
        }
        return neighbourAgentList;
    }
}
