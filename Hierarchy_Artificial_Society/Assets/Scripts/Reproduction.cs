using System;
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
    /*
     * References
     */

    // Reference to Agent
    private Agent agent;
    // Reference to world
    private World world;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<Agent>();
        world = GameObject.Find("World").GetComponent<World>();
    }

    private void ReproductionProcess()
    {
        // checks if there is an empty cell adjacent to current agent's cell - this will move to be handled by manager?
        Vector2Int currentEmpty = world.CheckEmptyCell(agent.GetCellPosition().x, agent.GetCellPosition().y);

        // refreshes list of agents mated with - this will move to be handled by manager
        agent.agentReproductionList = new List<Agent>();

        foreach (Agent partner in agent.GetNeighbourAgentList())
        {
            // if the neighbour isn't a potential partner then skip
            if (!IsNeighbourPotentialPartner(partner))
                continue;
            // if it is a potential partner
            else
            {
                // go through list of agents that current agent has mated with and ensure that they haven't mated before and to ensure they don't mate with offspring
                if (agent.agentReproductionList.Contains(partner) ||
                   (partner.agentReproductionList != null && partner.agentReproductionList.Contains(agent)) ||
                    agent.agentChildList.Contains(partner))
                {
                    continue; //skips to next iteration of loop
                }

                // checks if there is an empty cell adjacent to the potential partner agent's cell
                Vector2Int partnerEmpty = world.CheckEmptyCell(partner.GetCellPosition().x, partner.GetCellPosition().y);

                //if either current agent or neighbour has an empty neighbouring cell
                if (currentEmpty.x != -1 || partnerEmpty.x != -1)
                {
                    //then reproduce
                    //creates gameobject for child agent
                    GameObject agentObj = CreateAgent.CreateAgentObject();
                    //sets position for child on grid
                    CreateAgent.GeneratePosition(agentObj, currentEmpty, partnerEmpty);
                    //sets Agent component values
                    Agent childCom = CreateAgent.CreateAgentComponent(agentObj, agent, partner);
                    //adds partner to list of agents mated with
                    agent.agentReproductionList.Add(partner);
                    //adds child to list of children
                    agent.agentChildList.Add(childCom);

                    //agent reproduction was too much so for now have break in here, so it doesn't go through all
                    break;
                }
            }
        }
    }

    // Returns true if agent is currently fertile
    private bool IsFertile(Agent agent)
    {
        return (agent.childBearingBegins <= agent.age && agent.childBearingEnds > agent.age && agent.sugar >= agent.sugarInit && agent.spice >= agent.spiceInit);
    }

    private bool IsNeighbourPotentialPartner(Agent neighbour)
    {
        //  makes sure agent is fertile and of different sex
        // the different sex check also rules out an agent mating with itself
        if (IsFertile(neighbour) && String.Equals(neighbour.GetSex(), agent.GetSex()) == false)
            return true;
        else
            return false;
    }
}
