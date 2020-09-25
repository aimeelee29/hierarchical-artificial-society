using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

/*
 * 
 *  Class which handles the ability for agent's to reproduce
 *  
 */

public static class Reproduction
{
    public static void ReproductionProcess(Agent agent, World world, Toggle toggle)
    {
        // If agent isn't fertile, return
        if (!IsFertile(agent, toggle))
        {
            return;
        }
        // checks if there is an empty cell adjacent to current agent's cell
        Vector2Int currentEmpty = world.CheckEmptyCell(agent.CellPosition.x, agent.CellPosition.y);

        Agent partner;

        for (int i = 0; i < agent.NeighbourAgentList.Count; ++i)
        {
            partner = agent.NeighbourAgentList[i];
            
            // if the neighbour isn't a potential partner then skip

            if (IsNeighbourPotentialPartner(agent, partner, toggle) == false)
            {
                continue;
            }
            // if it is a potential partner
            else
            {
                // go through list of agents that partner has mated with and ensure that they haven't mated before and also ensure they don't mate with offspring
                if (partner.AgentReproductionList.Contains(agent) || agent.AgentChildList.Contains(partner))
                {
                    continue; //skips to next iteration of loop
                }

                // checks if there is an empty cell adjacent to the potential partner agent's cell
                Vector2Int partnerEmpty = world.CheckEmptyCell(partner.CellPosition.x, partner.CellPosition.y);

                //if either current agent or neighbour has an empty neighbouring cell
                if ((currentEmpty.x != -1 || partnerEmpty.x != -1))
                {
                    // then reproduce
                    // creates gameobject for child agent
                    GameObject agentObj = GameObject.Find("Agent Factory").GetComponent<AgentFactory>().CreateChild();
                    Agent agentComponent = agentObj.GetComponent<Agent>();
                    // sets position for child on grid
                    agentComponent.InitPosition(currentEmpty, partnerEmpty);
                    // sets Agent component values
                    agentComponent.InitVars(agent, partner);
                    NeighbourVision.FindNeighboursManager(agentComponent, Resources.Load<Toggle>("ScriptableObjects/Toggle"));
                    // adds partner to list of agents mated with
                    agentComponent.AgentReproductionList.Add(partner);
                    // adds child to agent's list of children
                    agent.AgentChildList.Add(agentObj.GetComponent<Agent>());
                    // adds child to list of child agents
                    Agent.ChildAgents.Add(agentComponent);
                }
            }
        }
    }

    // Returns true if agent is currently fertile
    private static bool IsFertile(Agent agent, Toggle toggle)
    {
        if (toggle.GetReproductionWealthReduction())
        {
            return (agent.ChildBearingBegins <= agent.Age && agent.ChildBearingEnds > agent.Age && agent.Sugar > 20 && agent.Spice > 20);
        }
        else
        {
            return (agent.ChildBearingBegins <= agent.Age && agent.ChildBearingEnds > agent.Age && agent.Sugar > agent.SugarInit && agent.Spice > agent.SpiceInit);
        }
    }

    private static bool IsNeighbourPotentialPartner(Agent agent, Agent neighbour, Toggle toggle)
    {
        // makes sure agent is fertile and of different sex (and alive)
        return (IsFertile(neighbour, toggle) && neighbour.Sex != agent.Sex && neighbour.IsAlive);
    }
}
