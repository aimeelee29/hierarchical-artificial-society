using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 
 * Class containing methods needed for agent trading
 * 
 */

public static class Trade
{
    public static void MakeTrade(Agent agent, TradeAnalysis tradeAnalysis)
    {
        //keeps track of number of trades made
       // int potentialTradeCount = 0;

        // For every neighbour
        foreach (Agent neighbour in agent.NeighbourAgentList)
        {
            //increment potential trade count
           // ++potentialTradeCount;

            // If they have already traded then skip
            if (neighbour.AgentTradeList.Contains(agent))
                continue;
            // If MRSA = MRSB then no trade. Continue skips that iteration of the loop
            if (agent.MRS == neighbour.MRS)
                continue;

            //if (potentialTradeCount == 5)
              //  break;

            // If already traded in that time step then no trade
            //print("about to trade");
            //print("agent sugar = " + agent.Sugar + "(" + agent.SugarMetabolism + ")" + " agent spice = " + agent.Spice + "(" + agent.SpiceMetabolism + ")");
            //print("neighbour sugar = " + neighbour.Sugar + "(" + neighbour.SugarMetabolism + ")" + " neighbour spice = " + neighbour.Spice + "(" + neighbour.SpiceMetabolism + ")");
            //print("agent MRS =" + agent.MRS);
            //print("neighbour MRS =" + neighbour.MRS);

            // otherwise 
            // Set up vars needed
            double price; 
            int sugarUnits; 
            int spiceUnits;
            double currentWelfareA;
            double currentWelfareB;
            double potentialWelfareA;
            double potentialWelfareB;
            double potentialMRSA;
            double potentialMRSB;

            // If MRSA > MRSB then agent A buys sugar, sells spice (A considers sugar to be relatively more valuable than agent B)
            if (agent.MRS > neighbour.MRS)
            {
                // If this trade will:
                // (a) make both agents better off(increases the welfare of both agents), and
                // (b) not cause the agents' MRSs to cross over one another, then the trade is made and return to start, else end.

                // while mrss don't cross over each other, trade with agent
                while (agent.MRS > neighbour.MRS)
                {
                    // Calculate price (geometric mean of the two MRSs)
                    price = Price(agent.MRS, neighbour.MRS);
                    // Calculate quantities to be traded
                    sugarUnits = SugarUnits(price);
                    spiceUnits = SpiceUnits(price);
                    //print("price = " + price);
                    //print("sug units = " + sugarUnits);
                    //print("spi units = " + spiceUnits);

                    // Calculate current welfare to be able to compare with potential welfare
                    currentWelfareA = agent.Welfare(0, 0);
                    currentWelfareB = neighbour.Welfare(0, 0);
                    //print("agent cur welf = " + currentWelfareA);
                    //print("neighbour cur welf = " + currentWelfareB);

                    //Calculate potential welfare after trade
                    potentialWelfareA = agent.Welfare(sugarUnits, -spiceUnits);
                    potentialWelfareB = neighbour.Welfare(-sugarUnits, spiceUnits);
                    //print("agent pot welf = " + potentialWelfareA);
                    //print("neighbour pot welf = " + potentialWelfareB);

                    if (neighbour.TimeUntilSugarDeath - sugarUnits > 0)
                    {
                        potentialMRSA = (agent.TimeUntilSpiceDeath - spiceUnits) / (agent.TimeUntilSugarDeath + sugarUnits);
                        potentialMRSB = (neighbour.TimeUntilSpiceDeath + spiceUnits) / (neighbour.TimeUntilSugarDeath - sugarUnits);
                    }

                    // Only make trade if it benefits both agents
                    if (potentialWelfareA > currentWelfareA && potentialWelfareB > currentWelfareB)
                    {
                        //print("trade");
                        agent.Sugar += sugarUnits;
                        neighbour.Sugar -= sugarUnits;
                        agent.Spice -= spiceUnits;
                        neighbour.Spice += spiceUnits;
                        tradeAnalysis.AddToPrice(price);
                        tradeAnalysis.IncrementQty();
                        agent.AgentTradeList.Add(neighbour);

                        //print("agent new sugar = " + agent.Sugar + " agent spice = " + agent.Spice);
                        //print("neighbour new sugar = " + neighbour.Sugar + " neighbour spice = " + neighbour.Spice);

                        // Update MRS
                        agent.MRS = CalcMRS(agent);
                        neighbour.MRS = CalcMRS(neighbour);
                        //print("agent MRS =" + agent.MRS);
                        //print("neighbour MRS =" + neighbour.MRS);

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
                    //print("price = " + price);
                    //print("sug units = " + sugarUnits);
                    //print("spi units = " + spiceUnits);

                    // Calculate current welfare to be able to compare with potential welfare
                    currentWelfareA = agent.Welfare(0, 0);
                    currentWelfareB = neighbour.Welfare(0, 0);
                    //print("agent cur welf = " + currentWelfareA);
                    //print("neighbour cur welf = " + currentWelfareB);

                    // Calculate potential welfare after trade
                    potentialWelfareA = agent.Welfare(-sugarUnits, spiceUnits);
                    potentialWelfareB = neighbour.Welfare(sugarUnits, -spiceUnits);
                    //print("agent pot welf = " + potentialWelfareA);
                    //print("neighbour pot welf = " + potentialWelfareB);

                    if (agent.TimeUntilSugarDeath - sugarUnits > 0)
                    {
                        potentialMRSA = (agent.TimeUntilSpiceDeath + spiceUnits) / (agent.TimeUntilSugarDeath - sugarUnits);
                        potentialMRSB = (neighbour.TimeUntilSpiceDeath - spiceUnits) / (neighbour.TimeUntilSugarDeath + sugarUnits);
                    }
                

                    if (potentialWelfareA > currentWelfareA && potentialWelfareB > currentWelfareB)
                    {
                        //print("trade");
                        agent.Sugar -= sugarUnits;
                        neighbour.Sugar += sugarUnits;
                        agent.Spice += spiceUnits;
                        neighbour.Spice -= spiceUnits;
                        tradeAnalysis.AddToPrice(price);
                        tradeAnalysis.IncrementQty();
                        agent.AgentTradeList.Add(neighbour);

                        //print("agent new sugar = " + agent.Sugar + " agent spice = " + agent.Spice);
                        //print("neighbour new sugar = " + neighbour.Sugar + " neighbour spice = " + neighbour.Spice);

                        // Update MRS
                        agent.MRS = CalcMRS(agent);
                        neighbour.MRS = CalcMRS(neighbour);
                        //print("agent MRS =" + agent.MRS);
                        //print("neighbour MRS =" + neighbour.MRS);
                    }
                    else
                        break;
                }
            }
        }
    }

    public static double CalcMRS(Agent agent)
    {
        //time until death for each commodity - used for trading
        agent.TimeUntilSugarDeath = agent.Sugar / agent.SugarMetabolism;
        agent.TimeUntilSpiceDeath = agent.Spice / agent.SpiceMetabolism;
        double MRS;

        if (agent.TimeUntilSugarDeath != 0) //avoids divide by zero error. Maybe could put this inside isalive if statement and then wouldn't need this
        {
            MRS = agent.TimeUntilSpiceDeath / agent.TimeUntilSugarDeath;
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
            return (int)(1 / p);
    }

    private static int SpiceUnits(double p)
    {
        if (p > 1)
            return (int)p;
        else
            return 1;
    }

}
//The ratio of the spice to sugar quantities exchanged is simply the price. This price must, of necessity , fall in the range [MRSA , MRSB]. 
//( change if rules were unfair?).
//While all prices within the feasible range are " agreeable " to the agents, not all prices appear to be equally "fair." 
//Prices near either end of the range would seem to be a better deal for one of the agents, particularly when the price range is very large. 


