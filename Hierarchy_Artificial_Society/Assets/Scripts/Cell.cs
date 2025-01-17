﻿using System;
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

    // cell co-ordinates - used in Manager class to set colour of cell
    private Vector3Int cellCoords;
    //private int cellNumber;

    // how many units of sugar and spice grows back per time frame
    private static int growbackFactor = 2;

    // reference of agent occupying cell. To be set when agents spawn (either initially or through reproduction/replacement).
    private Agent occupyingAgent;
    // shows if an agent is harvesting cell. To be set when agents look around to find best location (and refreshed each update)
    private bool occupiedHarvest;

    // colour variable which will change depending on how much resource is at that location
    public Color cellColor = new Color();

    /*
     * GETTERS AND SETTERS
     */

    public int MaxSugar { get => maxSugar; set => maxSugar = value; }
    public int MaxSpice { get => maxSpice; set => maxSpice = value; }
    public int CurSugar { get => curSugar; set => curSugar = value; }
    public int CurSpice { get => curSpice; set => curSpice = value; }
    public Agent OccupyingAgent { get => occupyingAgent; set => occupyingAgent = value; }
    public bool OccupiedHarvest { get => occupiedHarvest; set => occupiedHarvest = value; }
    public Vector3Int CellCoords { get => cellCoords; set => cellCoords = value; }
    //public Color CellColor { get => cellColor; set => cellColor = value; }

    /*
     * CONSTRUCTOR
     */

    public Cell(int i, int j)
    {
        cellCoords.Set(i, j, 0);
    }

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
        {
            return;
        }
        //if both are at max level then do nothing
        else if (curSugar == maxSugar && curSpice == maxSpice)
        {
            return;
        }
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


