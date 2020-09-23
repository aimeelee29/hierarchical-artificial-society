using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeighbourUpdater : MonoBehaviour
{
    //Set in inspector
    [SerializeField] private Toggle toggle = null;

    // If child agent spawns within the agent's collider (set to agent's neighbour vision)
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "NeighbourUpdater")
        {
            return;
        }

        Agent agentCur = this.GetComponentInParent<Agent>();
        Agent agentNew = collider.GetComponentInParent<Agent>();

        // Check if the agent causing trigger isn't already in the neighbour list
        if (agentCur.NeighbourAgentList.Contains(agentNew))
        {
            return;
        }

        int rankDiff = agentCur.SocialRank - agentNew.SocialRank;
        rankDiff = (rankDiff < 0) ? -rankDiff : rankDiff;

        if (toggle.GetRestrictNeighbour())
        {
            if (rankDiff == 0)
            {
                agentCur.NeighbourAgentList.Add(agentNew);
            }
        }
        else if (toggle.GetRestrictNeighbourLowerRank())
        {
            if (agentCur.SocialRank < 8)
            {
                if (rankDiff == 0)
                {
                    agentCur.NeighbourAgentList.Add(agentNew);
                }
            }
            else
            {
                agentCur.NeighbourAgentList.Add(agentNew);
            }
        }
        else
        {
            agentCur.NeighbourAgentList.Add(agentNew);
        }
    }
}
