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
        Agent neighbourAgent;

        //goes through each collider within vision sradius
        for (int i = 0; i < colliderList.Length; ++i)
        {
            neighbour = colliderList[i];
            neighbourAgent = neighbour.GetComponent<Agent>();
            // if collider is not attached to an agent then skip this iteration (as overlapcircleall will also catch collider for tilemap) 
            // also removes itself
            // checks if neighbour is already in list
            if (neighbour.tag != "Agent" || neighbour.gameObject == agent.gameObject || agent.NeighbourAgentList.Contains(neighbourAgent))
            {
                continue;
            }
            agent.NeighbourAgentList.Add(neighbourAgent);
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
        Agent neighbourAgent;

        //goes through each collider within radius
        for (int i = 0; i < colliderList.Length; ++i)
        {
            neighbour = colliderList[i];
            neighbourAgent = neighbour.GetComponent<Agent>();

            // if collider is not attached to an agent then skip this iteration (as overlapcircleall will also catch collider for tilemap) 
            // also removes itself
            // checks if neighbour is already in list
            // also removes any agent that isn't of similar social rank
            if (neighbour.tag != "Agent" || neighbour.gameObject == agent.gameObject || agent.NeighbourAgentList.Contains(neighbourAgent))
            {
                continue;
            }
            int rankDiff = neighbourAgent.SocialRank - agent.SocialRank;
            rankDiff = (rankDiff < 0) ? -rankDiff : rankDiff;
            if (rankDiff > 4)
            {
                continue;
            }

            agent.NeighbourAgentList.Add(neighbourAgent);
        }
        return;
    }

    // Manager method
    public static void FindNeighboursManager(Agent agent, Toggle toggle)
    {
        //If neighbour restrictions are toggled on then it calls restricted variation of findneighbours
        if (toggle.GetRestrictNeighbour())
        {
            FindNeighboursRestricted(agent);
        }
        else if (toggle.GetRestrictNeighbourLowerRank())
        {
            if (agent.SocialRank < 8)
            {
                FindNeighboursRestricted(agent);
            }
            else
            {
                FindNeighbours(agent);
            }
        }
        else
        {
            FindNeighbours(agent);
        }
    }
}
