using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 
 * Class which handles agent's ability to identify who their neighbours are.
 * Needed for trading and reproduction.
 * 
 */

public static class NeighbourVision
{
    // Method to find neighbours
    public static void FindNeighbours(Agent agent)
    {
        //Generate array of colliders within radius (set to vision)
        Collider2D[] colliderList = Physics2D.OverlapCircleAll(agent.TransformPosition, agent.VisionNeighbour);

        Collider2D neighbour;

        //goes through each collider within vision sradius
        for (int i = 0; i < colliderList.Length; ++i)
        {
            neighbour = colliderList[i];
            // if collider is not attached to an agent then skip this iteration (as overlapcircleall will also catch collider for tilemap) 
            // also removes itself
            if (neighbour.tag != "Agent" || neighbour.gameObject == agent.gameObject)
            {
                continue;
            }
            agent.NeighbourAgentList.Add(neighbour.GetComponent<Agent>());
        }
        return ;
    }

    // Method to find neighbours if restrict neighbour toggle is turned on
    public static void FindNeighboursRestricted(Agent agent)
    {
        //Generate array of colliders within radius (set to vision)
        //does transform need to come from agent??
        Collider2D[] colliderList = Physics2D.OverlapCircleAll(agent.TransformPosition, agent.VisionNeighbour);
        //print("collider list length = " + colliderList.Length);

        Collider2D neighbour;

        //goes through each collider within radius
        for (int i = 0; i < colliderList.Length; ++i)
        {
            neighbour = colliderList[i];
            // if collider is not attached to an agent then skip this iteration (as overlapcircleall will also catch collider for tilemap) 
            // also removes itself
            // also removes any agent that isn't of similar social rank
            int rankDiff = neighbour.GetComponent<Agent>().SocialRank - agent.SocialRank;
            rankDiff = (rankDiff < 0) ? -rankDiff : rankDiff;

            if (neighbour.tag != "Agent" || neighbour.gameObject == agent.gameObject || rankDiff > 2)
            {
                continue;
            }
            agent.NeighbourAgentList.Add(neighbour.GetComponent<Agent>());
        }
        return;
    }
}
