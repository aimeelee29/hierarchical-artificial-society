using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 
 * Class which manages the order that events happen in for all agents.
 * Avoids situations where some agents have gone through update and others haven't.
 * 
 */

public class AgentManager : MonoBehaviour
{
    // Need access to scriptable object Toggle to enable/disable certain behaviours
    private static Toggle toggle;
    // Need access to world
    private static World world;
    // Need access to tradeanalysis
    TradeAnalysis tradeAnalysis;

    void Start()
    {
        toggle = Resources.Load<Toggle>("ScriptableObjects/Toggle");
        world = GameObject.Find("World").GetComponent<World>();
        tradeAnalysis = GameObject.Find("Analysis: Trading").GetComponent<TradeAnalysis>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /*
         * AGENT MANAGEMENT
         */

        // First Updates things which don't rely on other agents
        foreach (Agent agent in Agent.LiveAgents)
        {
            //increase agent's age
            ++agent.Age;

            //decrease sugar & spice through metabolism
            agent.Sugar -= agent.SugarMetabolism;
            agent.Spice -= agent.SpiceMetabolism;

            //check for death
            agent.Death();

            // Look around and harvest food
            agent.Harvest();

            //time until death for each commodity
            agent.TimeUntilSugarDeath = agent.Sugar / agent.SugarMetabolism;
            agent.TimeUntilSpiceDeath = agent.Spice / agent.SpiceMetabolism;
            if (agent.TimeUntilSugarDeath != 0) //avoids divide by zero error. Maybe could put this inside isalive if statement and then wouldn't need this
            {
                agent.MRS = agent.TimeUntilSpiceDeath / agent.TimeUntilSugarDeath;
            }
            else
                agent.MRS = 0;
        }

        // can't change collection when iterating so need this step to remove dead agents from live list
        foreach (GameObject deadAgent in Agent.AvailableAgents)
        {
            Agent.LiveAgents.Remove(deadAgent.GetComponent<Agent>());
        }
        
        if (Agent.LiveAgents.Count > 1)
        {
            // Finds neighbours 
            foreach (Agent agent in Agent.LiveAgents)
            {
                agent.NeighbourAgentList = agent.GetComponent<NeighbourVision>().FindNeighbours(agent);
                // print(agent.NeighbourAgentList);
            }
            
            // Trade - only if selected in toggle (in inspector)
            if (toggle.GetTrade())
            {
                foreach (Agent agent in Agent.LiveAgents)
                {
                    agent.GetComponent<Trade>().MakeTrade(agent, tradeAnalysis);
                }
            }

            // Reproduce - only if selected in toggle (in inspector)
            if (toggle.GetReproduction())
            {
                foreach (Agent agent in Agent.LiveAgents)
                {
                    agent.GetComponent<Reproduction>().ReproductionProcess(agent, world);
                }
            }

            // Add children to live list
            foreach (Agent child in Agent.ChildAgents)
            {
                Agent.LiveAgents.Add(child);
            }

            // Reset child list
            Agent.ChildAgents.Clear();
        }

        /* 
         * WORLD MANAGEMENT
         */

        // Allow for sugar and spice growback and reset occupied harvest
        for (int i = 0; i < World.Rows; ++i)
        {
            for (int j = 0; j < World.Cols; ++j)
            {                
                world.WorldArray[i, j].Growback();
                world.WorldArray[i, j].OccupiedHarvest = false;
            }
        }
    }
}
