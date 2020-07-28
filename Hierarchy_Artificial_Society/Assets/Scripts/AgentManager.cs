using System.Collections;
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
    // // Counter to keep track of number of fixedupdates
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
            //print("pre harvest " + agent.Sugar + agent.Spice);
            // Look around and harvest food
            agent.Harvest();

            // Wipes each agent's list of agents they have mated with in previous time step
            agent.AgentReproductionList.Clear();

            //print("post harvest" + agent.Sugar + " " + agent.Spice);
        }

        if (Agent.LiveAgents.Count > 1)
        {
            
            // Finds neighbours 
            // Also calculates MRS in prep for trade and wipes agent's trading list
            foreach (Agent agent in Agent.LiveAgents)
            {

                //If neighbour restrictions are toggled on then it calls restricted variation of findneighbours
                if (toggle.GetRestrictNeighbour())
                    agent.NeighbourAgentList = agent.GetComponent<NeighbourVision>().FindNeighboursRestricted(agent);
                else
                    agent.NeighbourAgentList = agent.GetComponent<NeighbourVision>().FindNeighbours(agent);

                agent.MRS = Trade.CalcMRS(agent);
                agent.AgentTradeList.Clear();
                agent.TotalTradesinUpdate = 0;
            }

            // Trade - only if selected in toggle (in inspector)
            if (toggle.GetTrade())
            {
                foreach (Agent agent in Agent.LiveAgents)
                {
                    Trade.MakeTrade(agent, tradeAnalysis, toggle.GetBiasTrade());
                    //print("post trade " + agent.Sugar + " " + agent.Spice);

                    //Trades have an effect on an agent's influence 
                    if (agent.TotalTradesinUpdate > 0)
                        ++agent.InfluenceCounter;
                    else
                        --agent.InfluenceCounter;

                    /*
                    if (agent.InfluenceCounter > 0)
                        print(">0");
                    else
                        print("<0");
                    */
                }
            }

            // Reproduce - only if selected in toggle (in inspector)
            if (toggle.GetReproduction())
            {
                foreach (Agent agent in Agent.LiveAgents)
                {
                    //"pre rep"
                    Reproduction.ReproductionProcess(agent, world);
                }
            }

            //print(Agent.ChildAgents.Count);
            //print(Agent.LiveAgents.Count);

            // Add children to live list
            foreach (Agent child in Agent.ChildAgents)
            {
                Agent.LiveAgents.Add(child);
                //print(child.Sugar + child.Spice);
            }

            // Reset child list
            Agent.ChildAgents.Clear();

            //print(Agent.ChildAgents.Count);
            //print(Agent.LiveAgents.Count);

            //print(Agent.AvailableAgents.Count);

            // Incremenent Counter
            ++updateCounter;

            //Create new class every ten updates to report wealth distribution and social rank distribution
            if (updateCounter % 10 == 1)
            {
                wealthDistAnalysis.CreateWealthFile(updateCounter);
                socialRankAnalysis.CreateRankFile(updateCounter);
            }
            // Create new class on 500th update to report on social mobility
            if (updateCounter % 50 == 0)
            {
                foreach (Agent agent in Agent.AllAgents)
                {
                    SocialRankChange socRankChange = new SocialRankChange(agent.IsChild, agent.BegSocialRank, agent.BegSocialRank, agent.SocialRank, agent.NumberRankChanges);
                    socialMobilityAnalysis.socialMobiltyListClass.socialMobilityList.Add(socRankChange);
                }

                socialMobilityAnalysis.CreateMobilityFile(updateCounter);
            }
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



