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
            if (toggle.GetReproduction())
            {
                //print("death");
                //agent.Death();
            }
            else
            {
                //print("death and replace");
                //agent.DeathandReplacement();
            }
                       
        }

        // can't change collection when iterating so need this step to remove dead agents from live list
        foreach (GameObject deadAgent in Agent.AvailableAgents)
        {
            Agent.LiveAgents.Remove(deadAgent.GetComponent<Agent>());
        }

        //print(Agent.LiveAgents.Count);
        for (int i = 0; i < Agent.LiveAgents.Count; ++i)
        {
            agent = Agent.LiveAgents[i];

            // Look around and harvest food
            agent.Harvest();
            // Wipes each agent's list of agents they have mated with in previous time step
            agent.AgentReproductionList.Clear();
            // Also calculates MRS in prep for trade and wipes agent's trading list
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

        // Variable to hold agent's wealth
        int wealth;
        // Variable to hold max wealth - feeds into agent social rank
        int maxWealth = 0;

        for (int i = 0; i < Agent.LiveAgents.Count; ++i)
        {
            agent = Agent.LiveAgents[i];

            wealth = agent.Sugar + agent.Spice;

            if (wealth > maxWealth)
                maxWealth = wealth;
        }

        // Update the agent static variable for max wealth
        Agent.MaxWealth = maxWealth;
        Agent.LowWealth = maxWealth / 4;
        Agent.LowMidWealth = Agent.LowWealth * 2;
        Agent.HighMidWealth = Agent.LowWealth * 3;

        for (int i = 0; i < Agent.LiveAgents.Count; ++i)
        {
            agent = Agent.LiveAgents[i];

            if (agent.Sugar + agent.Spice <= Agent.LowWealth)
            {
                agent.WealthScore = 1;
            }
            else if (agent.Sugar + agent.Spice <= Agent.LowMidWealth)
            {
                agent.WealthScore = 2;
            }
            else if (agent.Sugar + agent.Spice <= Agent.HighMidWealth)
            {
                agent.WealthScore = 3;
            }
            else
            {
                agent.WealthScore = 4;
            }
        }
        /*
        * ANALYSIS
        */

        // Analysis files created every 50 updates
        if (updateCounter % 50 == 1)
        {
            //wealthDistAnalysis.CreateWealthFile(updateCounter);
            //socialRankAnalysis.CreateRankFile(updateCounter);

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
            // Create new instance of the wealth class
            WealthDistributionList wealthDistListClass = new WealthDistributionList();

            for (int i = 0; i < Agent.LiveAgents.Count; ++i)
            {
                agent = Agent.LiveAgents[i];
                wealth = agent.Sugar + agent.Spice;

                wealthDistListClass.AddtoWealth(wealth);

                // Create new agent profile
                AgentProfile agProf = new AgentProfile(agent.SugarMetabolism, agent.SpiceMetabolism, agent.VisionHarvest, agent.VisionNeighbour, agent.Lifespan, agent.Dominance, agent.Influence, agent.Age, agent.CellPosition, agent.SocialRank);
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
            tradeAnalysis.SaveXML();
            socialMobilityAnalysis.SaveXML();

            //Wipe all agents list so you don't keep adding the same agents to the dataset
            //Agent.AllAgents.Clear();

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
                // if both sugar and spice is low, then don't do anything since already white
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
}



