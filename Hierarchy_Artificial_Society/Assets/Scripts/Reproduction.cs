﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 
 *  Class which handles the ability for agent's to reproduce
 *  
 */

public class Reproduction : MonoBehaviour
{
    public void ReproductionProcess(Agent agent, World world)
    {
        //world = GameObject.Find("World").GetComponent<World>();
        // checks if there is an empty cell adjacent to current agent's cell - this will move to be handled by manager?
        Vector2Int currentEmpty = world.CheckEmptyCell(agent.CellPosition.x, agent.CellPosition.y);

        // refreshes list of agents mated with - this will move to be handled by manager
        agent.AgentReproductionList = new List<Agent>();

        foreach (Agent partner in agent.NeighbourAgentList)
        {
            // if the neighbour isn't a potential partner then skip
            if (!IsNeighbourPotentialPartner(agent, partner))
                continue;
            // if it is a potential partner
            else
            {
                // go through list of agents that current agent has mated with and ensure that they haven't mated before and to ensure they don't mate with offspring
                if (agent.AgentReproductionList.Contains(partner) ||
                   (partner.AgentReproductionList != null && partner.AgentReproductionList.Contains(agent)) ||
                    agent.AgentChildList.Contains(partner))
                {
                    continue; //skips to next iteration of loop
                }

                // checks if there is an empty cell adjacent to the potential partner agent's cell
                Vector2Int partnerEmpty = world.CheckEmptyCell(partner.CellPosition.x, partner.CellPosition.y);

                //if either current agent or neighbour has an empty neighbouring cell
                if (currentEmpty.x != -1 || partnerEmpty.x != -1)
                {
                    print("reproduce");
                    // then reproduce
                    // creates gameobject for child agent
                    GameObject agentObj = GameObject.Find("Agent Factory").GetComponent<AgentFactory>().CreateChild();
                    // sets position for child on grid
                    agentObj.GetComponent<Agent>().InitPosition(currentEmpty, partnerEmpty);
                    // sets Agent component values
                    agentObj.GetComponent<Agent>().InitVars(agent, partner);
                    // adds partner to list of agents mated with
                    agent.AgentReproductionList.Add(partner);
                    // adds child to agent's list of children - dont think i need this now
                    agent.AgentChildList.Add(agentObj.GetComponent<Agent>());
                    // adds child to list of child agents
                    Agent.ChildAgents.Add(agentObj.GetComponent<Agent>());

                    //agent reproduction was too much so for now have break in here, so it doesn't go through all
                    break;
                }
            }
        }
    }

    // Returns true if agent is currently fertile
    private bool IsFertile(Agent agent)
    {
        //print(agent.Sugar >= agent.SugarInit && agent.Spice >= agent.SpiceInit);
        return (agent.ChildBearingBegins <= agent.Age && agent.ChildBearingEnds > agent.Age && agent.Sugar >= agent.SugarInit && agent.Spice >= agent.SpiceInit);
    }

    private bool IsNeighbourPotentialPartner(Agent agent, Agent neighbour)
    {
        //print("IsNeighbourPotentialPartner");
        //print(IsFertile(neighbour));
        //print(IsFertile(neighbour) && neighbour.Sex != agent.Sex);
        //  makes sure agent is fertile and of different sex
        // the different sex check also rules out an agent mating with itself
        return (IsFertile(neighbour) && neighbour.Sex != agent.Sex);
    }
}
