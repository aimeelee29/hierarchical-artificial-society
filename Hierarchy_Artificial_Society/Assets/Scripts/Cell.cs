﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{

    // maximum levels of sugar and spice that can exist in the cell
    private int maxSugar;
    private int maxSpice;
    // current levels of sugar and spice in the cell
    public int curSugar;
    public int curSpice;
    // x and y co ordinates - no sure I need these
    public int x;
    public int y;
    //how many units of sugar and spice grows back per time frame
    public int growbackFactor;

    //constructor
    public Cell(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       //growback goes here
       
    }
}
