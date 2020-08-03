﻿using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
    private static TradeAnalysis tradeAnalysis;
    // Need access to wealth dist analysis
    private static WealthDistributionAnalysis wealthDistAnalysis;
    // Need access to social rank analysis
    private static SocialRankAnalysis socialRankAnalysis;
    private static SocialMobilityAnalysis socialMobilityAnalysis;
    // Need access to agent profile analysis
    private static AgentProfileAnalysis agentProfileAnalysis;

    // Counter to keep track of number of fixedupdates
    private static int updateCounter = 0;

    // Vars used for colours later on
    private float colourVal;

    void Start()
    {
        toggle = Resources.Load<Toggle>("ScriptableObjects/Toggle");
        world = GameObject.Find("World").GetComponent<World>();
        tradeAnalysis = GameObject.Find("Analysis: Trading").GetComponent<TradeAnalysis>();
        wealthDistAnalysis = GameObject.Find("Analysis: Wealth Distribution").GetComponent<WealthDistributionAnalysis>();
        socialRankAnalysis = GameObject.Find("Analysis: Social Rank").GetComponent<SocialRankAnalysis>();
        socialMobilityAnalysis = GameObject.Find("Analysis: Social Mobility").GetComponent<SocialMobilityAnalysis>();
        agentProfileAnalysis = GameObject.Find("Analysis: Agent Profiles").GetComponent<AgentProfileAnalysis>();
        envTilemap = GameObject.Find("Environment").GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        /*
         * AGENT MANAGEMENT
         */
        Agent agent;

        // First Updates things which don't rely on other agents
        //foreach (Agent agent in Agent.LiveAgents)
        for (int i = 0; i < Agent.LiveAgents.Count; ++i)
        {
            agent = Agent.LiveAgents[i];
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

        //print(Agent.LiveAgents.Count);
        //foreach (Agent agent in Agent.LiveAgents)
        for (int i = 0; i < Agent.LiveAgents.Count; ++i)
        {
            agent = Agent.LiveAgents[i];
            //print("pre harvest " + agent.Sugar + agent.Spice);
            // Look around and harvest food
            agent.Harvest();

            // Wipes each agent's list of agents they have mated with in previous time step
            agent.AgentReproductionList.Clear();
            // Wipes each agent's neighbour list
            agent.NeighbourAgentList.Clear();

            //print("post harvest" + agent.Sugar + " " + agent.Spice);
        }

        // Finds neighbours 
        // Also calculates MRS in prep for trade and wipes agent's trading list
        //foreach (Agent agent in Agent.LiveAgents)
        for (int i = 0; i < Agent.LiveAgents.Count; ++i)
        {
            agent = Agent.LiveAgents[i];
            //If neighbour restrictions are toggled on then it calls restricted variation of findneighbours
            if (toggle.GetRestrictNeighbour())
            {
                NeighbourVision.FindNeighboursRestricted(agent);
            }   
            else
            {
                NeighbourVision.FindNeighbours(agent);
            }
            agent.MRS = Trade.CalcMRS(agent);
            agent.AgentTradeList.Clear();
            agent.TotalTradesinUpdate = 0;
        }

        // Trade - only if selected in toggle (in inspector)
        if (toggle.GetTrade())
        {
            for (int i = 0; i < Agent.LiveAgents.Count; ++i)
            {
                Trade.MakeTrade(Agent.LiveAgents[i], tradeAnalysis, toggle.GetBiasTrade());
            }
        }

        // Reproduce - only if selected in toggle (in inspector)
        if (toggle.GetReproduction())
        {
            for (int i = 0; i < Agent.LiveAgents.Count; ++i)
            {
                Reproduction.ReproductionProcess(Agent.LiveAgents[i], world);
            }
        }

        //print(Agent.ChildAgents.Count);
        //print(Agent.LiveAgents.Count);

        // Add children to live list
        for (int i = 0; i < Agent.ChildAgents.Count; ++i)
        {
            Agent.LiveAgents.Add(Agent.ChildAgents[i]);
            //print(child.Sugar + child.Spice);
        }

        // Reset child list
        Agent.ChildAgents.Clear();

        //print(Agent.ChildAgents.Count);
        //print(Agent.LiveAgents.Count);

        //print(Agent.AvailableAgents.Count);

        // Incremenent Counter
        ++updateCounter;

        // Create new class every 50 updates to report wealth distribution and social rank distribution
        // Create new class on 50th update to report on social mobility and agent profiling
        if (updateCounter % 50 == 1)
        {
            wealthDistAnalysis.CreateWealthFile(updateCounter);
            socialRankAnalysis.CreateRankFile(updateCounter);

            foreach (Agent ag in Agent.AllAgents)
            {
                SocialMobility socMob = new SocialMobility(ag.IsChild, ag.BegSocialRank, ag.SocialRank, ag.NumberRankChanges, ag.Age);
                socialMobilityAnalysis.socialMobiltyListClass.socialMobilityList.Add(socMob);
                AgentProfile agProf = new AgentProfile(ag.SugarMetabolism, ag.SpiceMetabolism, ag.VisionHarvest, ag.VisionNeighbour, ag.Lifespan, ag.Dominance, ag.Influence, ag.Age, ag.CellPosition);
                agentProfileAnalysis.agentProfileListClass.agentProfileList.Add(agProf);
            }

            socialMobilityAnalysis.CreateMobilityFile(updateCounter);
            agentProfileAnalysis.CreateAgentProfileFile(updateCounter);

            //Wipe all agents list so you don't keep adding the same agents to the dataset
            Agent.AllAgents.Clear();

            //print("agent tag = " + GameObject.FindGameObjectsWithTag("Agent").Length + " " + updateCounter);
            //print("social mobility analysis list = " + socialMobilityAnalysis.socialMobiltyListClass.socialMobilityList.Count + " " + updateCounter);
            
        }
        

        //print(Agent.AllAgents.Count);

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
                    envTilemap.SetColor(world.WorldArray[i, j].CellCoords, new Color(0, 0, 0, 0));
                // if more sugar then colour yellow
                else if (world.WorldArray[i, j].CurSugar > world.WorldArray[i, j].CurSpice)
                {
                    colourVal = ((world.WorldArray[i, j].CurSugar - World.Wasteland) * (0.75f)) / (World.MountainTops - World.Wasteland);
                    envTilemap.SetColor(world.WorldArray[i, j].CellCoords, new Color(1 - colourVal, 1- colourVal, 0, 0.75f));
                }
                else
                {
                    colourVal = (((world.WorldArray[i, j].CurSpice - World.Wasteland) * (0.75f - 0.1f)) / (World.MountainTops - World.Wasteland)) + 0.1f;
                    envTilemap.SetColor(world.WorldArray[i, j].CellCoords, new Color(0, 1 - colourVal, 0, 0.75f));
                }
            }
        }
    }
}



