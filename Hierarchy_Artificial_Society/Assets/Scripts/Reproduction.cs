using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 
 *  Class which handles the ability for agent's to reproduce
 *  
 */

public static class Reproduction
{
    public static void ReproductionProcess(Agent agent, World world)
    {
        //world = GameObject.Find("World").GetComponent<World>();
        // checks if there is an empty cell adjacent to current agent's cell - this will move to be handled by manager?
        Vector2Int currentEmpty = world.CheckEmptyCell(agent.CellPosition.x, agent.CellPosition.y);

        foreach (Agent partner in agent.NeighbourAgentList)
        {
            //print("neighbour count = " + agent.NeighbourAgentList.Count);
            //print(IsNeighbourPotentialPartner(agent, partner));
            // if the neighbour isn't a potential partner then skip
            if (!IsNeighbourPotentialPartner(agent, partner))
                continue;
            // if it is a potential partner
            else
            {
                //print("agent repro list" + agent.AgentReproductionList.Count);
                //print("partner repro list" + partner.AgentReproductionList.Count);

                // go through list of agents that partner has mated with and ensure that they haven't mated before and also ensure they don't mate with offspring
                if (partner.AgentReproductionList.Contains(agent) || agent.AgentChildList.Contains(partner))
                {
                    continue; //skips to next iteration of loop
                }

                // checks if there is an empty cell adjacent to the potential partner agent's cell
                Vector2Int partnerEmpty = world.CheckEmptyCell(partner.CellPosition.x, partner.CellPosition.y);

               // print("partner empty = " + partnerEmpty);
               // print("agent empty = " + currentEmpty);

                //if either current agent or neighbour has an empty neighbouring cell
                if (currentEmpty.x != -1 || partnerEmpty.x != -1)
                {
                    //print("reproduce");
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
    private static bool IsFertile(Agent agent)
    {
        return (agent.ChildBearingBegins <= agent.Age && agent.ChildBearingEnds > agent.Age && agent.Sugar >= agent.SugarInit && agent.Spice >= agent.SpiceInit);
    }

    private static bool IsNeighbourPotentialPartner(Agent agent, Agent neighbour)
    {
        //print("IsNeighbourPotentialPartner");
        //print(IsFertile(neighbour));
        //print(IsFertile(neighbour) && neighbour.Sex != agent.Sex);
        //  makes sure agent is fertile and of different sex
        // the different sex check also rules out an agent mating with itself
        return (IsFertile(neighbour) && neighbour.Sex != agent.Sex);
    }
}
