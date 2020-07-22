using System;
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
            UnityEngine.Debug.Log("about to do new trade");
            UnityEngine.Debug.Log("agent sugar = " + agent.Sugar + "(" + agent.SugarMetabolism + ")" + " agent spice = " + agent.Spice + "(" + agent.SpiceMetabolism + ")");
            UnityEngine.Debug.Log("neighbour sugar = " + neighbour.Sugar + "(" + neighbour.SugarMetabolism + ")" + " neighbour spice = " + neighbour.Spice + "(" + neighbour.SpiceMetabolism + ")");
            UnityEngine.Debug.Log("agent MRS =" + agent.MRS);
            UnityEngine.Debug.Log("neighbour MRS =" + neighbour.MRS);

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

            bool goodTrade;


            // If MRSA > MRSB then agent A buys sugar, sells spice (A considers sugar to be relatively more valuable than agent B)
            if (agent.MRS > neighbour.MRS)
            {
                // If this trade will:
                // (a) make both agents better off(increases the welfare of both agents), and
                // (b) not cause the agents' MRSs to cross over one another, then the trade is made and return to start, else end.

                // while mrss don't cross over each other, trade with agent
                while (agent.MRS > neighbour.MRS)
                {
                    UnityEngine.Debug.Log("still in loop");
                    // Calculate price (geometric mean of the two MRSs)
                    price = Price(agent.MRS, neighbour.MRS);
                    // Calculate quantities to be traded
                    sugarUnits = SugarUnits(price);
                    spiceUnits = SpiceUnits(price);
                    UnityEngine.Debug.Log("price = " + price);
                    UnityEngine.Debug.Log("sug units = " + sugarUnits);
                    UnityEngine.Debug.Log("spi units = " + spiceUnits);

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
                        UnityEngine.Debug.Log("trade");
                        agent.Sugar += sugarUnits;
                        neighbour.Sugar -= sugarUnits;
                        agent.Spice -= spiceUnits;
                        neighbour.Spice += spiceUnits;
                        tradeAnalysis.IncrementQty();
                        tradeAnalysis.AddToPrice(price);
                        //UnityEngine.Debug.Log("neg sug " + (sugarUnits < 0));
                        tradeAnalysis.AddToUnits(sugarUnits);
                        agent.AgentTradeList.Add(neighbour);

                        UnityEngine.Debug.Log("agent new sugar = " + agent.Sugar + " agent spice = " + agent.Spice);
                        UnityEngine.Debug.Log("neighbour new sugar = " + neighbour.Sugar + " neighbour spice = " + neighbour.Spice);

                        // Update MRS
                        agent.MRS = CalcMRS(agent);
                        neighbour.MRS = CalcMRS(neighbour);
                        UnityEngine.Debug.Log("agent MRS =" + agent.MRS);
                        UnityEngine.Debug.Log("neighbour MRS =" + neighbour.MRS);

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
                    UnityEngine.Debug.Log("still in loop");
                    // Calculate price (geometric mean of the two MRSs)
                    price = Price(agent.MRS, neighbour.MRS);
                    // Calculate quantities to be traded
                    sugarUnits = SugarUnits(price);
                    spiceUnits = SpiceUnits(price);
                    UnityEngine.Debug.Log("price = " + price);
                    UnityEngine.Debug.Log("sug units = " + sugarUnits);
                    UnityEngine.Debug.Log("spi units = " + spiceUnits);

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
                        //potentialMRSA = (agent.TimeUntilSpiceDeath + spiceUnits) / (agent.TimeUntilSugarDeath - sugarUnits);
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
                        UnityEngine.Debug.Log("trade");
                        agent.Sugar -= sugarUnits;
                        neighbour.Sugar += sugarUnits;
                        agent.Spice += spiceUnits;
                        neighbour.Spice -= spiceUnits;
                        tradeAnalysis.IncrementQty();
                        tradeAnalysis.AddToPrice(price);
                        //UnityEngine.Debug.Log("neg sug " + (sugarUnits < 0));
                        tradeAnalysis.AddToUnits(sugarUnits);
                        agent.AgentTradeList.Add(neighbour);

                        UnityEngine.Debug.Log("agent new sugar = " + agent.Sugar + " agent spice = " + agent.Spice);
                        UnityEngine.Debug.Log("neighbour new sugar = " + neighbour.Sugar + " neighbour spice = " + neighbour.Spice);

                        // Update MRS
                        agent.MRS = CalcMRS(agent);
                        neighbour.MRS = CalcMRS(neighbour);
                        UnityEngine.Debug.Log("agent MRS =" + agent.MRS);
                        UnityEngine.Debug.Log("neighbour MRS =" + neighbour.MRS);
                    }
                    else
                        break;
                }
            }
        }
    }

    public static void MakeTradeBiased(Agent agent, TradeAnalysis tradeAnalysis)
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

            /*
            UnityEngine.Debug.Log("about to do new trade");
            UnityEngine.Debug.Log("agent sugar = " + agent.Sugar + "(" + agent.SugarMetabolism + ")" + " agent spice = " + agent.Spice + "(" + agent.SpiceMetabolism + ")");
            UnityEngine.Debug.Log("neighbour sugar = " + neighbour.Sugar + "(" + neighbour.SugarMetabolism + ")" + " neighbour spice = " + neighbour.Spice + "(" + neighbour.SpiceMetabolism + ")");
            UnityEngine.Debug.Log("agent MRS =" + agent.MRS);
            UnityEngine.Debug.Log("neighbour MRS =" + neighbour.MRS);
            */

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

            bool goodTrade;

            // If MRSA > MRSB then agent A buys sugar, sells spice (A considers sugar to be relatively more valuable than agent B)
            if (agent.MRS > neighbour.MRS)
            {
                // If this trade will:
                // (a) make both agents better off(increases the welfare of both agents), and
                // (b) not cause the agents' MRSs to cross over one another, then the trade is made and return to start, else end.

                // while mrss don't cross over each other, trade with agent
                while (agent.MRS > neighbour.MRS)
                {
                    UnityEngine.Debug.Log("still in loop");
                    // Calculate price (geometric mean of the two MRSs)
                    price = Price(agent.MRS, neighbour.MRS);
                    // Calculate quantities to be traded
                    sugarUnits = SugarUnits(price);
                    spiceUnits = SpiceUnits(price);
                    UnityEngine.Debug.Log("price = " + price);
                    UnityEngine.Debug.Log("sug units = " + sugarUnits);
                    UnityEngine.Debug.Log("spi units = " + spiceUnits);

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
                        potentialMRSA = CalcMRS(agent, sugarUnits, -spiceUnits);
                        potentialMRSB = CalcMRS(neighbour, -sugarUnits, spiceUnits);
                    }

                    

                    // Only make trade if it benefits both agents
                    if (potentialWelfareA > currentWelfareA && potentialWelfareB > currentWelfareB)
                    {
                        UnityEngine.Debug.Log("trade");
                        agent.Sugar += sugarUnits;
                        neighbour.Sugar -= sugarUnits;
                        agent.Spice -= spiceUnits;
                        neighbour.Spice += spiceUnits;
                        tradeAnalysis.IncrementQty();
                        tradeAnalysis.AddToPrice(price);
                        //UnityEngine.Debug.Log("neg sug " + (sugarUnits < 0));
                        tradeAnalysis.AddToUnits(sugarUnits);
                        agent.AgentTradeList.Add(neighbour);

                        UnityEngine.Debug.Log("agent new sugar = " + agent.Sugar + " agent spice = " + agent.Spice);
                        UnityEngine.Debug.Log("neighbour new sugar = " + neighbour.Sugar + " neighbour spice = " + neighbour.Spice);

                        // Update MRS
                        agent.MRS = CalcMRS(agent);
                        neighbour.MRS = CalcMRS(neighbour);
                        UnityEngine.Debug.Log("agent MRS =" + agent.MRS);
                        UnityEngine.Debug.Log("neighbour MRS =" + neighbour.MRS);

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
                    UnityEngine.Debug.Log("still in loop");
                    // Calculate price (geometric mean of the two MRSs)
                    price = Price(agent.MRS, neighbour.MRS);
                    // Calculate quantities to be traded
                    sugarUnits = SugarUnits(price);
                    spiceUnits = SpiceUnits(price);
                    UnityEngine.Debug.Log("price = " + price);
                    UnityEngine.Debug.Log("sug units = " + sugarUnits);
                    UnityEngine.Debug.Log("spi units = " + spiceUnits);

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
                        //potentialMRSA = (agent.TimeUntilSpiceDeath + spiceUnits) / (agent.TimeUntilSugarDeath - sugarUnits);
                        potentialMRSA = CalcMRS(agent, -sugarUnits, spiceUnits);
                        potentialMRSB = CalcMRS(neighbour, sugarUnits, -spiceUnits);
                    }


                    if (potentialWelfareA > currentWelfareA && potentialWelfareB > currentWelfareB)
                    {
                        UnityEngine.Debug.Log("trade");
                        agent.Sugar -= sugarUnits;
                        neighbour.Sugar += sugarUnits;
                        agent.Spice += spiceUnits;
                        neighbour.Spice -= spiceUnits;
                        tradeAnalysis.IncrementQty();
                        tradeAnalysis.AddToPrice(price);
                        //UnityEngine.Debug.Log("neg sug " + (sugarUnits < 0));
                        tradeAnalysis.AddToUnits(sugarUnits);
                        agent.AgentTradeList.Add(neighbour);

                        UnityEngine.Debug.Log("agent new sugar = " + agent.Sugar + " agent spice = " + agent.Spice);
                        UnityEngine.Debug.Log("neighbour new sugar = " + neighbour.Sugar + " neighbour spice = " + neighbour.Spice);

                        // Update MRS
                        agent.MRS = CalcMRS(agent);
                        neighbour.MRS = CalcMRS(neighbour);
                        UnityEngine.Debug.Log("agent MRS =" + agent.MRS);
                        UnityEngine.Debug.Log("neighbour MRS =" + neighbour.MRS);
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
        agent.TimeUntilSugarDeath = (double)agent.Sugar / agent.SugarMetabolism;
        agent.TimeUntilSpiceDeath = (double)agent.Spice / agent.SpiceMetabolism;
        //UnityEngine.Debug.Log("Time until sugar death" + agent.TimeUntilSugarDeath);
        //UnityEngine.Debug.Log("Time until spice death" + agent.TimeUntilSpiceDeath);
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
            //return (int)(1 / p);
            return (int)Math.Round(1 / p);
        }
    }

    private static int SpiceUnits(double p)
    {
        if (p > 1)
        {
            //return (int)p;
            return (int)Math.Round(p);
        }
        else
            return 1;
    }

}
//The ratio of the spice to sugar quantities exchanged is simply the price. This price must, of necessity , fall in the range [MRSA , MRSB]. 
//( change if rules were unfair?).
//While all prices within the feasible range are " agreeable " to the agents, not all prices appear to be equally "fair." 
//Prices near either end of the range would seem to be a better deal for one of the agents, particularly when the price range is very large. 


