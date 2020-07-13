using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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
    // Need access to tilemap for tile colour
    private static Tilemap envTilemap;
    // Need access to tradeanalysis
    TradeAnalysis tradeAnalysis;

    // Vars used for colours later on
    float colourVal;

    void Start()
    {
        toggle = Resources.Load<Toggle>("ScriptableObjects/Toggle");
        world = GameObject.Find("World").GetComponent<World>();
        tradeAnalysis = GameObject.Find("Analysis: Trading").GetComponent<TradeAnalysis>();
        envTilemap = GameObject.Find("Environment").GetComponent<Tilemap>();
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
        }

        // can't change collection when iterating so need this step to remove dead agents from live list
        foreach (GameObject deadAgent in Agent.AvailableAgents)
        {
            Agent.LiveAgents.Remove(deadAgent.GetComponent<Agent>());
        }

        foreach (Agent agent in Agent.LiveAgents)
        {
            // Look around and harvest food
            agent.Harvest();

            // Wipes each agent's list of agents they have mated with in previous time step
            agent.AgentReproductionList.Clear();
        }

        if (Agent.LiveAgents.Count > 1)
        {
            // Finds neighbours 
            // Also calculates MRS in prep for trade and wipes agent's trading list
            foreach (Agent agent in Agent.LiveAgents)
            {
                agent.NeighbourAgentList = agent.GetComponent<NeighbourVision>().FindNeighbours(agent);
                // print(agent.NeighbourAgentList);

                agent.MRS = Trade.CalcMRS(agent);
                agent.AgentTradeList.Clear();
            }
            
            // Trade - only if selected in toggle (in inspector)
            if (toggle.GetTrade())
            {
                foreach (Agent agent in Agent.LiveAgents)
                {
                    Trade.MakeTrade(agent, tradeAnalysis);
                }
            }

            // Reproduce - only if selected in toggle (in inspector)
            if (toggle.GetReproduction())
            {
                foreach (Agent agent in Agent.LiveAgents)
                {
                    Reproduction.ReproductionProcess(agent, world);
                }
            }

            //print(Agent.ChildAgents.Count);
            //print(Agent.LiveAgents.Count);
            
            // Add children to live list
            foreach (Agent child in Agent.ChildAgents)
            {
                Agent.LiveAgents.Add(child);
            }

            // Reset child list
            Agent.ChildAgents.Clear();

            //print(Agent.ChildAgents.Count);
            //print(Agent.LiveAgents.Count);
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

                // Update Colour
                // if both sugar and spice is low, then set to white
                if (world.WorldArray[i,j].CurSugar <= World.Wasteland && world.WorldArray[i, j].CurSpice <= World.Wasteland)
                    envTilemap.SetColor(new Vector3Int(i, j, 0), new Color(0, 0, 0, 0));
                // if more sugar then colour yellow
                else if (world.WorldArray[i, j].CurSugar > world.WorldArray[i, j].CurSpice)
                {
                    colourVal = ((world.WorldArray[i, j].CurSugar - World.Wasteland) * (0.75f)) / (World.MountainTops - World.Wasteland);
                    envTilemap.SetColor(new Vector3Int(i, j, 0), new Color(1 - colourVal, 1- colourVal, 0, 0.75f));
                }
                else
                {
                    colourVal = (((world.WorldArray[i, j].CurSpice - World.Wasteland) * (0.75f - 0.1f)) / (World.MountainTops - World.Wasteland)) + 0.1f;
                    envTilemap.SetColor(new Vector3Int(i, j, 0), new Color(0, 1 - colourVal, 0, 0.75f));
                }
            }
        }
    }
}



