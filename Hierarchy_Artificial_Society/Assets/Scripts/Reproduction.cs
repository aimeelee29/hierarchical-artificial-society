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
    public static void ReproductionProcess(Agent agent, World world)
    {
        // checks if there is an empty cell adjacent to current agent's cell
        Vector2Int currentEmpty = world.CheckEmptyCell(agent.CellPosition.x, agent.CellPosition.y);
        //UnityEngine.Debug.Log("is child =" + agent.isChild);
        //UnityEngine.Debug.Log("neighbour count = " + agent.NeighbourAgentList.Count);

        // To keep track of how many times the agent has reproduced
        int counter = 0;

        Agent partner;

        //foreach (Agent partner in agent.NeighbourAgentList)
        for (int i = 0; i < agent.NeighbourAgentList.Count; ++i)
        {
            partner = agent.NeighbourAgentList[i];
            // If agent isn't fertile, return
            if (!IsFertile(agent))
            {
                return;
            }
            // if the neighbour isn't a potential partner then skip
            //UnityEngine.Debug.Log("partner is child = " + partner.isChild);
            //UnityEngine.Debug.Log("potential partner = " + IsNeighbourPotentialPartner(agent, partner));
            if (IsNeighbourPotentialPartner(agent, partner) == false)
            {
                continue;
            }
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
                if ((currentEmpty.x != -1 || partnerEmpty.x != -1) && counter <= 4)
                {
                    //UnityEngine.Debug.Log("reproduce");
                    // then reproduce
                    // creates gameobject for child agent
                    GameObject agentObj = GameObject.Find("Agent Factory").GetComponent<AgentFactory>().CreateChild();
                    Agent agentComponent = agentObj.GetComponent<Agent>();
                    // sets position for child on grid
                    agentComponent.InitPosition(currentEmpty, partnerEmpty);
                    // sets Agent component values
                    agentComponent.InitVars(agent, partner);
                    // adds partner to list of agents mated with
                    agentComponent.AgentReproductionList.Add(partner);
                    // adds child to agent's list of children - dont think i need this now
                    //agent.AgentChildList.Add(agentObj.GetComponent<Agent>());
                    // adds child to list of child agents
                    Agent.ChildAgents.Add(agentComponent);
                    // adds child to list of all agents
                    // Agent.AllAgents.Add(agentComponent);
                    // increment counter
                    ++counter;
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
        //UnityEngine.Debug.Log("neihbour fertile = " + IsFertile(neighbour));
        //UnityEngine.Debug.Log("opposite sex = " + (neighbour.Sex != agent.Sex));
        //  makes sure agent is fertile and of different sex
        // the different sex check also rules out an agent mating with itself
        return (IsFertile(neighbour) && neighbour.Sex != agent.Sex);
    }
}
