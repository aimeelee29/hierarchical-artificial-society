using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Tilemaps;

public class World : MonoBehaviour
{
    //determines the size of the world
    public static int rows = 50;
    public static int cols = 50;
    //represents it as a 2D array of cells
    public Cell[,] worldArray = new Cell[rows,cols];

    // Start is called before the first frame update
    void Start()
    {
        //calls populate array to populate the world with cells
        PopulateArray();
        //calls Mountains method to populate the cells with sugar/spice
        Mountains();
    }

    //Create cells
    public void PopulateArray()
    {
        for (int i = 0; i < rows; ++i)
        {
            for (int j = 0; j < cols; ++j)
            {
                worldArray[i, j] = new Cell(i, j);
            }
        }
    }

    //populates cells with sugar and spice levels in the form of sugar and spice mountains
    public void Mountains()
    {
        //MOUNTAIN 1
        
        //points of centre co-ords
        int a = 13;
        int b = 37;
 
        //amount of sugar/spice at top of mountain
        int mountainTops = 4;

        int innerR = 4;
        int outerR = 10;
  
       //use mountain top / radius

       //change loop below to not run through everything
        for (int i = 0; i < rows; ++i)
        {
            for (int j = 0; j < cols; ++j)
            {
                //smallest circle
                if (((i - a) * (i - a) + (j - b) * (j - b)) <= innerR * innerR)
                    worldArray[i, j].curSugar = mountainTops;
                //widest circle
                else if (((i - a) * (i - a) + (j - b) * (j - b)) <= outerR * outerR)
                    worldArray[i, j].curSugar = 1;
            }
        }

    }
}
