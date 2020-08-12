using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This class will allow analysis on proportion of total wealth by wealth band and average wealth by wealth band
 */

public class WealthInequality
{
    public int wealthBand;
    public int wealth;
    // Need number of agents to computer average
    public int numberAgents;

    // Constructor
    public WealthInequality(int band)
    {
        wealthBand = band;
    }

    // Also needs parameterless constructor for serialisation
    public WealthInequality()
    {
    }

    public void AddToWealth(int w)
    {
        wealth += w;
        ++numberAgents;
    }

}
