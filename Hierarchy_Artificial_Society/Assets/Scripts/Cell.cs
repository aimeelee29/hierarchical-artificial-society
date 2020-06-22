﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{

    // maximum levels of sugar and spice that can exist in the cell
    private int maxSugar;
    private int maxSpice;
    // current levels of sugar and spice in the cell
    private int curSugar;
    private int curSpice;

    // x and y co ordinates - no sure I need these
    public int x;
    public int y;

    //how many units of sugar and spice grows back per time frame
    private static int growbackFactor = 1;

    //reference of agent occupying cell. To be set when agents spawn (either initially or through reproduction/replacement).
    private GameObject occupyingAgent;

    //constructor
    public Cell(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    // Update is called once per frame
    void Update()
    {
        Growback();
    }

    private void Growback()
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

    //Setters

    public void SetSugar(int s)
    {
        curSugar = s;
    }

    public void SetSpice(int s)
    {
        curSpice = s;
    }

    public void SetMaxSugar(int s)
    {
        maxSugar = s;
    }

    public void SetMaxSpice(int s)
    {
        maxSpice = s;
    }
    
    public void SetAgent(GameObject agentObj)
    {
        occupyingAgent = agentObj;
    }

    //Getters

    public int GetSugar(int s)
    {
        return curSugar;
    }

    public int GetSpice(int s)
    {
        return curSpice;
    }

    public GameObject GetAgent()
    {
        return occupyingAgent;
    }

}