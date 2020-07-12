using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

/*
 * 
 * Class which represents each cell of grid
 * 
 */

public class Cell
{
    // maximum levels of sugar and spice that can exist in the cell
    private int maxSugar;
    private int maxSpice;
    
    // current levels of sugar and spice in the cell
    private int curSugar;
    private int curSpice;

    //how many units of sugar and spice grows back per time frame
    private static int growbackFactor = 3;

    //reference of agent occupying cell. To be set when agents spawn (either initially or through reproduction/replacement).
    private Agent occupyingAgent;

    //shows if an agent is harvesting cell. To be set when agents look around to find best location (and refreshed each update)
    private bool occupiedHarvest;

    /*
     * GETTERS AND SETTERS
     */

    public int MaxSugar { get => maxSugar; set => maxSugar = value; }
    public int MaxSpice { get => maxSpice; set => maxSpice = value; }
    public int CurSugar { get => curSugar; set => curSugar = value; }
    public int CurSpice { get => curSpice; set => curSpice = value; }
    public Agent OccupyingAgent { get => occupyingAgent; set => occupyingAgent = value; }
    public bool OccupiedHarvest { get => occupiedHarvest; set => occupiedHarvest = value; }

    /*
     * MAIN METHODS
     */

    public int DepleteSugar()
    {
        int temp = curSugar;
        curSugar = 0;
        return temp;
    }

    public int DepleteSpice()
    {
        int temp = curSpice;
        curSpice = 0;
        return temp;
    }

    public void Growback()
    {
        //if there is no growback then do nothing
        if (growbackFactor == 0)
            return;

        //if both are at max level then do nothing
        else if (curSugar == maxSugar && curSpice == maxSpice)
            return;

        //do i need to factor in less than 1 growback?? It would mean changing the variables to float though and altering things
        //Could be that it grows 1 every 4 time steps, in which case growback would be 0.25 and you would
        //increment a counter. when reaches four (1/growback), you add sugar/spice.
        else
        {
            //sugar growback
            //if adding growback pushes over the max then just assign it to max (also covers the case if its already at max level)
            if (curSugar + growbackFactor >= maxSugar)
                curSugar = maxSugar;
            else
                curSugar += growbackFactor;
            //spice growback
            if (curSpice + growbackFactor >= maxSpice)
                curSpice = maxSpice;
            else
                curSpice += growbackFactor;
        }
    }
}


