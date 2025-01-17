﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

/*
 * 
 * Class containing methods needed for agent trading
 * 
 */

public static class Trade
{
    public static void MakeTrade(Agent agent, TradeAnalysis tradeAnalysis, bool biasToggle)
    {
        Agent neighbour;
        int tradesInOne;

        // For every neighbour
        for (int i = 0; i < agent.NeighbourAgentList.Count; ++i)
        {
            // each new neighbour reset number of trades in one go
            tradesInOne = 0;
            neighbour = agent.NeighbourAgentList[i];

            // If they have already traded then skip
            // If neighbour is dead then skip
            if (neighbour.AgentTradeList.Contains(agent)
                || neighbour.IsAlive == false)
                continue;
            // If MRSA = MRSB then no trade. Continue skips that iteration of the loop
            if (agent.MRS == neighbour.MRS)
                continue;

            // otherwise 
            // Set up variables needed
            double price; 
            int sugarUnits; 
            int spiceUnits;
            double currentWelfareA;
            double currentWelfareB;
            double potentialWelfareA;
            double potentialWelfareB;
            double potentialMRSA;
            double potentialMRSB;

            bool goodTrade;

            // If MRSA > MRSB then agent A buys sugar, sells spice (A considers sugar to be relatively more valuable than agent B)
            if (agent.MRS > neighbour.MRS)
            {
                // while mrss don't cross over each other, trade with agent
                while (agent.MRS > neighbour.MRS)
                {
                    // Calculate price (geometric mean of the two MRSs)
                    price = Price(agent.MRS, neighbour.MRS);
                    // Calculate quantities to be traded
                    sugarUnits = SugarUnits(price);
                    spiceUnits = SpiceUnits(price);
                    // Calculate current welfare to be able to compare with potential welfare
                    currentWelfareA = agent.Welfare(0, 0);
                    currentWelfareB = neighbour.Welfare(0, 0);
                    //Calculate potential welfare after trade
                    potentialWelfareA = agent.Welfare(sugarUnits, -spiceUnits);
                    potentialWelfareB = neighbour.Welfare(-sugarUnits, spiceUnits);

                    if (neighbour.TimeUntilSugarDeath - sugarUnits > 0)
                    {
                        potentialMRSA = CalcMRS(agent, sugarUnits, -spiceUnits);
                        potentialMRSB = CalcMRS(neighbour, -sugarUnits, spiceUnits);
                    }

                    // if agent has higher social ranking then trade only needs to benefit agent
                    if (biasToggle && agent.SocialRank > neighbour.SocialRank)
                        goodTrade = potentialWelfareA > currentWelfareA;
                    // if neighbour has higher ranking then trade only needs to benefit neighbour
                    else if (biasToggle && neighbour.SocialRank > agent.SocialRank)
                        goodTrade = potentialWelfareB > currentWelfareB;
                    // If they have the same social rank then only make trade if it benefits both agents
                    else
                        goodTrade = potentialWelfareA > currentWelfareA && potentialWelfareB > currentWelfareB;

                    // Trade
                    if (goodTrade)
                    {
                        tradesInOne = 1;
                        agent.Sugar += sugarUnits;
                        neighbour.Sugar -= sugarUnits;
                        agent.Spice -= spiceUnits;
                        neighbour.Spice += spiceUnits;
                        tradeAnalysis.AddToPrice(price);
                        tradeAnalysis.AddToUnits(sugarUnits);
                        tradeAnalysis.AddToSpiceUnits(spiceUnits);
                        agent.AgentTradeList.Add(neighbour);

                        // Update MRS
                        agent.MRS = CalcMRS(agent);
                        neighbour.MRS = CalcMRS(neighbour);
                    }
                    else
                        break;
                }
            }
            // Else MRSA < MRSB
            else
            {
                while (agent.MRS < neighbour.MRS)
                {
                    // Calculate price (geometric mean of the two MRSs)
                    price = Price(agent.MRS, neighbour.MRS);
                    // Calculate quantities to be traded
                    sugarUnits = SugarUnits(price);
                    spiceUnits = SpiceUnits(price);
                    // Calculate current welfare to be able to compare with potential welfare
                    currentWelfareA = agent.Welfare(0, 0);
                    currentWelfareB = neighbour.Welfare(0, 0);
                    // Calculate potential welfare after trade
                    potentialWelfareA = agent.Welfare(-sugarUnits, spiceUnits);
                    potentialWelfareB = neighbour.Welfare(sugarUnits, -spiceUnits);

                    if (agent.TimeUntilSugarDeath - sugarUnits > 0)
                    {
                        potentialMRSA = CalcMRS(agent, -sugarUnits, spiceUnits);
                        potentialMRSB = CalcMRS(neighbour, sugarUnits, -spiceUnits);
                    }

                    // if agent has higher social ranking then trade only needs to benefit agent
                    if (biasToggle && agent.SocialRank > neighbour.SocialRank)
                        goodTrade = potentialWelfareA > currentWelfareA;
                    // if neighbour has higher ranking then trade only needs to benefit neighbour
                    else if (biasToggle && neighbour.SocialRank > agent.SocialRank)
                        goodTrade = potentialWelfareB > currentWelfareB;
                    // If they have the same social rank then only make trade if it benefits both agents
                    else
                        goodTrade = potentialWelfareA > currentWelfareA && potentialWelfareB > currentWelfareB;

                    // Trade
                    if (goodTrade)
                    {
                        tradesInOne = 1;
                        agent.Sugar -= sugarUnits;
                        neighbour.Sugar += sugarUnits;
                        agent.Spice += spiceUnits;
                        neighbour.Spice -= spiceUnits;
                        tradeAnalysis.AddToPrice(price);
                        tradeAnalysis.AddToUnits(sugarUnits);
                        tradeAnalysis.AddToSpiceUnits(spiceUnits);
                        agent.AgentTradeList.Add(neighbour);

                        // Update MRS
                        agent.MRS = CalcMRS(agent);
                        neighbour.MRS = CalcMRS(neighbour);
                    }
                    else
                        break;
                }
            }
            if (tradesInOne == 1)
            {
                tradeAnalysis.IncrementQty();
            }
        }
    }

    public static double CalcMRS(Agent agent)
    {
        //time until death for each commodity - used for trading
        agent.TimeUntilSugarDeath = (double)agent.Sugar / agent.SugarMetabolism;
        agent.TimeUntilSpiceDeath = (double)agent.Spice / agent.SpiceMetabolism;
        double MRS;

        if (agent.TimeUntilSugarDeath != 0) //avoids divide by zero error. Maybe could put this inside isalive if statement and then wouldn't need this
        {
            MRS = agent.TimeUntilSpiceDeath / agent.TimeUntilSugarDeath;
        }
        else
            MRS = 0;

        return MRS;
    }

    // For calculating potential MRS (must include signs with parameters)
    public static double CalcMRS(Agent agent, int sugUn, int spiUn)
    {
            //time until death for each commodity - used for trading
            double timeUntilSugarDeath = (double)(agent.Sugar + sugUn) / agent.SugarMetabolism;
            double timeUntilSpiceDeath = (double)(agent.Spice + spiUn) / agent.SpiceMetabolism;

            double MRS;

            if (timeUntilSugarDeath != 0) //avoids divide by zero error. Maybe could put this inside isalive if statement and then wouldn't need this
            {
                MRS = timeUntilSpiceDeath / timeUntilSugarDeath;
            }
            else
                MRS = 0;

            return MRS;
    }

    private static double Price(double agent1MRS, double agent2MRS)
    {
        return Math.Sqrt(agent1MRS * agent2MRS);
    }

    private static int SugarUnits(double p)
    {
        // If price(p) > 1, p units of spice are exchanged for 1 unit of sugar.
        if (p > 1)
            return 1;
        // If p < 1, then 1 unit of spice is exchanged for 1/p units of sugar
        else
        {
            return (int)Math.Round(1 / p);
        }
    }

    private static int SpiceUnits(double p)
    {
        if (p > 1)
        {
            return (int)Math.Round(p);
        }
        else
            return 1;
    }
}