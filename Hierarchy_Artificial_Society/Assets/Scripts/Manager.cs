using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq; // for ordering lists on a variable

/*
 * 
 * Class which manages the order that events happen in for all agents.
 * Avoids situations where some agents have gone through update and others haven't.
 * 
 */

public class Manager : MonoBehaviour
{
    // Need access to scriptable object Toggle to enable/disable certain behaviours
    private static Toggle toggle;
    // Need access to world
    private static World world;
    // Need access to tilemap for tile colour
    private static Tilemap envTilemap;
    // Need access to analysis files
    private static TradeAnalysis tradeAnalysis;
    private static WealthDistributionAnalysis wealthDistributionAnalysis;
    private static WealthInequalityAnalysis wealthInequalityAnalysis;
    private static AgentProfileAnalysis agentProfileAnalysis;
    private static AgentCount agentCount;
    private static SocialMobilityAnalysis socialMobilityAnalysis;

    // Counter to keep track of number of fixedupdates
    private static int updateCounter = 0;

    // Vars used for colours later on
    private float colourVal;

    void Start()
    {
        toggle = Resources.Load<Toggle>("ScriptableObjects/Toggle");
        world = GameObject.Find("World").GetComponent<World>();
        tradeAnalysis = GameObject.Find("Analysis: Trading").GetComponent<TradeAnalysis>();
        wealthDistributionAnalysis = GameObject.Find("Analysis: Wealth Distribution").GetComponent<WealthDistributionAnalysis>();
        agentProfileAnalysis = GameObject.Find("Analysis: Agent Profiles").GetComponent<AgentProfileAnalysis>();
        agentCount = GameObject.Find("Analysis: Agent Count").GetComponent<AgentCount>();
        envTilemap = GameObject.Find("Environment").GetComponent<Tilemap>();
        wealthInequalityAnalysis = GameObject.Find("Analysis: Wealth Inequality").GetComponent<WealthInequalityAnalysis>();
        socialMobilityAnalysis = GameObject.Find("Analysis: Social Mobility").GetComponent<SocialMobilityAnalysis>();

        // Order list (used for assigning wealth bands)
        Agent.LiveAgentsOrdered = OrderListWealth(Agent.LiveAgents);

        for (int i = 0; i < Agent.LiveAgents.Count; ++i)
        {
            Agent.LiveAgents[i].CreateWealthScore();
            Agent.LiveAgents[i].Rank();
            Agent.LiveAgents[i].BegSocialRank = Agent.LiveAgents[i].SocialRank;
            Agent.LiveAgents[i].TrackSocialRank = Agent.LiveAgents[i].SocialRank;
        }

        // When agents are spawned, find neighbours method is called so agent 'knows' who is within its vision
        for (int i = 0; i < Agent.LiveAgents.Count; ++i)
        {
            NeighbourVision.FindNeighboursManager(Agent.LiveAgents[i], toggle);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Incremenent Counter - keeps count of how many updates there have been
        ++updateCounter;
        if(updateCounter >5000)
        {
            Application.Quit(); //only works in standalone
            //UnityEditor.EditorApplication.isPlaying = false;
        }

        /*
         * AGENT MANAGEMENT
         */

        Agent agent;

        // First Updates things which don't rely on other agents
        for (int i = 0; i < Agent.LiveAgents.Count; ++i)
        {
            agent = Agent.LiveAgents[i];

            //increase agent's age
            ++agent.Age;

            //decrease sugar & spice through metabolism
            agent.Sugar -= agent.SugarMetabolism;
            agent.Spice -= agent.SpiceMetabolism;

            //check for death
            if (toggle.GetReproduction())
            {
                agent.Death();
            }
            else
            {
                agent.DeathandReplacement();
            }
                       
        }

        // can't change collection when iterating so need this step to remove dead agents from live list
        foreach (GameObject deadAgent in Agent.AvailableAgents)
        {
            Agent.LiveAgents.Remove(deadAgent.GetComponent<Agent>());
        }

        for (int i = 0; i < Agent.LiveAgents.Count; ++i)
        {
            agent = Agent.LiveAgents[i];

            // Look around and harvest food
            agent.HarvestResource();
            // Wipes each agent's list of agents they have mated with in previous time step
            agent.AgentReproductionList.Clear();
            // Also calculates MRS in prep for trade and wipes agent's trading list
            agent.MRS = Trade.CalcMRS(agent);
            agent.AgentTradeList.Clear();
        }

        // Update social rank
        for (int i = 0; i < Agent.LiveAgents.Count; ++i)
        {
            agent = Agent.LiveAgents[i];

            Agent.LiveAgentsOrdered = OrderListWealth(Agent.LiveAgents);
            agent.CreateWealthScore();
            agent.Rank();
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
                Reproduction.ReproductionProcess(Agent.LiveAgents[i], world, toggle);
            }
        }

        // Add children to live list
        for (int i = 0; i < Agent.ChildAgents.Count; ++i)
        {
            Agent.LiveAgents.Add(Agent.ChildAgents[i]);
            //print(child.Sugar + child.Spice);
        }

        // Reset child list
        Agent.ChildAgents.Clear();

        
        /*
        * ANALYSIS
        */

        // Variable to hold agent's wealth
        int wealth;

        // Analysis files created every 50 updates
        if (updateCounter % 50 == 1)
        {

            // Create classes for wealth info to go into
            WealthInequality wealthOne = new WealthInequality(1);
            WealthInequality wealthTwo = new WealthInequality(2);
            WealthInequality wealthThree = new WealthInequality(3);
            WealthInequality wealthFour = new WealthInequality(4);

            // Creates new agent profile list class (class contains list for agent profiles to be added to)
            AgentProfileList agentProfileListClass = new AgentProfileList();
            // Creates new wealth inequality list class (class contains list for wealth info to be added to)
            WealthInequalityList wealthInequalityListClass = new WealthInequalityList();
            // Creates new wealth distribution list class
            WealthDistributionList wealthDistListClass = new WealthDistributionList();

            for (int i = 0; i < Agent.LiveAgents.Count; ++i)
            {
                agent = Agent.LiveAgents[i];
                wealth = agent.Sugar + agent.Spice;

                wealthDistListClass.AddtoWealth(wealth);

                // Create new agent profile
                AgentProfile agProf = new AgentProfile(agent.SugarMetabolism, agent.SpiceMetabolism, agent.VisionHarvest, agent.VisionNeighbour, agent.Lifespan, agent.Dominance, agent.Influence, agent.Age, agent.CellPosition, agent.SocialRank, agent.BegSocialRank);
                // add agent's profile to list
                agentProfileListClass.agentProfileList.Add(agProf);
                
                //print(agent.WealthScore);
                // Add to wealth classes
                if (agent.WealthScore == 1)
                {
                    wealthOne.AddToWealth(wealth);
                }
                else if (agent.WealthScore == 2)
                {
                    wealthTwo.AddToWealth(wealth);
                }
                else if (agent.WealthScore == 3)
                {
                    wealthThree.AddToWealth(wealth);
                }
                else if (agent.WealthScore == 4)
                {
                    wealthFour.AddToWealth(wealth);
                }
            }

            // Add wealth classes to list
            wealthInequalityListClass.wealthInequalityList.Add(wealthOne);
            wealthInequalityListClass.wealthInequalityList.Add(wealthTwo);
            wealthInequalityListClass.wealthInequalityList.Add(wealthThree);
            wealthInequalityListClass.wealthInequalityList.Add(wealthFour);

            //Save XMLs
            wealthDistributionAnalysis.SaveXML(updateCounter, wealthDistListClass);
            agentProfileAnalysis.SaveXML(updateCounter, agentProfileListClass);
            wealthInequalityAnalysis.SaveXML(updateCounter, wealthInequalityListClass);
            agentCount.SaveXML();
            socialMobilityAnalysis.SaveXML();
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
                // if both sugar and spice is low, then don't do anything
                // if more sugar then colour shades of yellow
                if (world.WorldArray[i, j].CurSugar > world.WorldArray[i, j].CurSpice)
                {
                    colourVal = 1 - (((world.WorldArray[i, j].CurSugar - World.Wasteland) * (0.75f)) / (World.MountainTops - World.Wasteland));
                    world.WorldArray[i, j].cellColor.r = colourVal;
                    world.WorldArray[i, j].cellColor.g = colourVal;
                    world.WorldArray[i, j].cellColor.b = 0;
                    world.WorldArray[i, j].cellColor.a = 0.75f;
                    envTilemap.SetColor(world.WorldArray[i, j].CellCoords, world.WorldArray[i, j].cellColor);
                }
                else if (world.WorldArray[i, j].CurSugar < world.WorldArray[i, j].CurSpice)
                {
                    colourVal = 1 - ((((world.WorldArray[i, j].CurSpice - World.Wasteland) * (0.75f - 0.1f)) / (World.MountainTops - World.Wasteland)) + 0.1f);
                    world.WorldArray[i, j].cellColor.r = 0;
                    world.WorldArray[i, j].cellColor.g = colourVal;
                    world.WorldArray[i, j].cellColor.b = 0;
                    world.WorldArray[i, j].cellColor.a = 0.75f;
                    envTilemap.SetColor(world.WorldArray[i, j].CellCoords, world.WorldArray[i, j].cellColor);
                }
            }
        }
    }

    // Method for ordering agent list by wealth
    public List<Agent> OrderListWealth(List<Agent> origList)
    {
        return origList.OrderBy(e => (e.Sugar + e.Spice)).ToList();
    }
}



