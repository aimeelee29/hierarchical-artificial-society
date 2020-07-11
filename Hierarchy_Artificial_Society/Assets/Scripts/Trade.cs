using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 
 * Class containing methods needed for agent trading
 * 
 */

public class Trade : MonoBehaviour
{
    //Need access to TradeAnalysis to report on trade metrics
    //private static TradeAnalysis tradeAnalysis;

    void Start()
    {
        //agent = GetComponent<Agent>();
        //Defined in start since find cannot be used directly in MonoBehaviour
       // tradeAnalysis = GameObject.Find("Analysis: Trading").GetComponent<TradeAnalysis>();
    }

    public void MakeTrade(Agent agent, TradeAnalysis tradeAnalysis)
    {
        // For every neighbour
        foreach (Agent neighbour in agent.NeighbourAgentList)
        {
            //TradeAnalysis tradeAnalysis = GameObject.Find("Analysis: Trading").GetComponent<TradeAnalysis>();
            //print(agent.NeighbourAgentList.Count);
            //print(agent.MRS);
            //print(neighbour.MRS);
            //print(agent == neighbour);
            // If MRSA = MRSB then no trade. Continue skips that iteration of the loop
            if (agent.MRS == neighbour.MRS)
                continue;
            print("about to trade");
            print("agent sugar = " + agent.Sugar + "(" + agent.SugarMetabolism + ")" + " agent spice = " + agent.Spice + "(" + agent.SpiceMetabolism + ")");
            print("neighbour sugar = " + neighbour.Sugar + "(" + neighbour.SugarMetabolism + ")" + " neighbour spice = " + neighbour.Spice + "(" + neighbour.SpiceMetabolism + ")");
            print("agent MRS =" + agent.MRS);
            print("neighbour MRS =" + neighbour.MRS);
            // otherwise 
            // calculate price (geometric mean of the two MRSs)
            double price = Price(agent.MRS, neighbour.MRS);
            print(price);

            // vars for how many sugar units are traded for spice units (and vice versa)
            int sugarUnits = SugarUnits(price);
            int spiceUnits = SpiceUnits(price);

            print("sug units = " + sugarUnits);
            print("spi units = " + spiceUnits);

            double currentWelfareA = agent.Welfare(0, 0);
            double currentWelfareB = neighbour.Welfare(0, 0);

            print("agent cur welf = " + currentWelfareA);
            print("neighbour cur welf = " + currentWelfareB);

            // If MRSA > MRSB then agent A buys sugar, sells spice (A considers sugar to be relatively more valuable than agent B)
            if (agent.MRS > neighbour.MRS)
            {
                // If this trade will:
                // (a) make both agents better off(increases the welfare of both agents), and
                // (b) not cause the agents' MRSs to cross over one another, then the trade is made and return to start, else end.

                double potentialWelfareA = agent.Welfare(sugarUnits, -spiceUnits);
                double potentialWelfareB = neighbour.Welfare(-sugarUnits, spiceUnits);

                print("agent pot welf = " + currentWelfareA);
                print("neighbour pot welf = " + currentWelfareB);

                if (neighbour.TimeUntilSugarDeath - sugarUnits > 0)
                {
                    double potentialMRSA = (agent.TimeUntilSpiceDeath - spiceUnits) / (agent.TimeUntilSugarDeath + sugarUnits);
                    double potentialMRSB = (neighbour.TimeUntilSpiceDeath + spiceUnits) / (neighbour.TimeUntilSugarDeath - sugarUnits);

                    if (potentialWelfareA > currentWelfareA && potentialWelfareB > currentWelfareB &&
                        potentialMRSA >= potentialMRSB)
                    {
                        print("trade");
                        agent.Sugar += sugarUnits;
                        neighbour.Sugar -= sugarUnits;
                        agent.Spice -= spiceUnits;
                        neighbour.Spice += spiceUnits;
                        tradeAnalysis.AddToPrice(price);
                        tradeAnalysis.IncrementQty();

                        print("agent new sugar = " + agent.Sugar + " agent spice = " + agent.Spice);
                        print("neighbour new sugar = " + neighbour.Sugar + " neighbour spice = " + neighbour.Spice);
                    }
                }
            }
            // Else MRSA < MRSB
            else
            {
                double potentialWelfareA = agent.Welfare(-sugarUnits, +spiceUnits);
                double potentialWelfareB = neighbour.Welfare(+sugarUnits, -spiceUnits);

                print("agent pot welf = " + currentWelfareA);
                print("neighbour pot welf = " + currentWelfareB);

                if (agent.TimeUntilSugarDeath - sugarUnits > 0)
                {
                    double potentialMRSA = (agent.TimeUntilSpiceDeath + spiceUnits) / (agent.TimeUntilSugarDeath - sugarUnits);
                    double potentialMRSB = (neighbour.TimeUntilSpiceDeath - spiceUnits) / (neighbour.TimeUntilSugarDeath + sugarUnits);

                    if (potentialWelfareA > currentWelfareA && potentialWelfareB > currentWelfareB &&
                        potentialMRSA <= potentialMRSB)
                    {
                        print("trade");
                        agent.Sugar -= sugarUnits;
                        neighbour.Sugar += sugarUnits;
                        agent.Spice += spiceUnits;
                        neighbour.Spice -= spiceUnits;
                        tradeAnalysis.AddToPrice(price);
                        tradeAnalysis.IncrementQty();

                        print("agent new sugar = " + agent.Sugar + " agent spice = " + agent.Spice);
                        print("neighbour new sugar = " + neighbour.Sugar + " neighbour spice = " + neighbour.Spice);
                    }
                }
            }
        }
    }

    private double Price(double agent1MRS, double agent2MRS)
    {
        return Math.Sqrt(agent1MRS * agent2MRS);
    }

    private int SugarUnits(double p)
    {
        // If price(p) > 1, p units of spice are exchanged for 1 unit of sugar.
        if (p > 1)
            return 1;
        // If p < 1, then 1 unit of spice is exchanged for 1/p units of sugar
        else
            return (int)(1 / p);
    }

    private int SpiceUnits(double p)
    {
        if (p > 1)
            return (int)p;
        else
            return 1;
    }

}

//trading

//The ratio of the spice to sugar quantities exchanged is simply the price. This price must, of necessity , fall in the range [MRSA , MRSB]. 
//( change if rules were unfair?).
//While all prices within the feasible range are " agreeable " to the agents, not all prices appear to be equally "fair." 
//Prices near either end of the range would seem to be a better deal for one of the agents, particularly when the price range is very large. 

//When an agent following M moves to a new location it has from 0 to 4 (von Neumann) neighbors. 
//It interacts through T exactly once with each of its neighbors, selected in random order.

