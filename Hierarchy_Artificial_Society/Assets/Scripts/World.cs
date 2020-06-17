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
    //represents world as a 2D array of cells
    public Cell[,] worldArray = new Cell[rows,cols];

    //Needs to know about TileMap in order to do shading for mountains
    private GameObject objTileMap;
    Tilemap envTilemap;

    // Start is called before the first frame update
    void Start()
    {
        //calls populate array to populate the world with cells
        PopulateArray();
        //Finds tilemap object
        objTileMap = GameObject.Find("Environment");
        envTilemap = objTileMap.GetComponent<Tilemap>(); 
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
    //also populates max sugar and spice levels
    public void Mountains()
    {
        //SUGAR MOUNTAINS
        
        //centre co-ords of Mountain 1
        int a = 13;
        int b = 37;
        //centre co-ords of Mountain 2
        int c = 37;
        int d = 13;
 
        //amount of sugar/spice at top of mountain
        int mountainTops = 4;

        int innerR = 4;
        int secondR = 6;
        int thirdR = 8;
        int outerR = 10;
  
        // checks to see if tile is in circle (using equation for a circle)
        for (int i = 0; i < rows; ++i)
        {
            for (int j = 0; j < cols; ++j)
            {
                //smallest circle
                //Mountain 1
                if (((i - a) * (i - a) + (j - b) * (j - b)) <= innerR * innerR)
                {
                    worldArray[i, j].curSugar = mountainTops;
                    worldArray[i, j].maxSugar = mountainTops;
                    envTilemap.SetColor(new Vector3Int(i, j, 0), new Color(0.25f, 0.25f, 0, 0.75f));
                }
                //could make these ORs
                //Mountain 2
                else if (((i - c) * (i - c) + (j - d) * (j - d)) <= innerR * innerR)
                {
                    worldArray[i, j].curSugar = mountainTops;
                    worldArray[i, j].maxSugar = mountainTops;
                    envTilemap.SetColor(new Vector3Int(i, j, 0), new Color(0.25f, 0.25f, 0, 0.75f));
                }


                else if (((i - a) * (i - a) + (j - b) * (j - b)) <= secondR * secondR)
                {
                    worldArray[i, j].curSugar = mountainTops - mountainTops/4;
                    worldArray[i, j].maxSugar = mountainTops - mountainTops / 4;
                    envTilemap.SetColor(new Vector3Int(i, j, 0), new Color(0.5f, 0.5f, 0, 0.75f));
                }
                else if (((i - c) * (i - c) + (j - d) * (j - d)) <= secondR * secondR)
                {
                    worldArray[i, j].curSugar = mountainTops - mountainTops / 4;
                    worldArray[i, j].maxSugar = mountainTops - mountainTops / 4;
                    envTilemap.SetColor(new Vector3Int(i, j, 0), new Color(0.5f, 0.5f, 0, 0.75f));
                }


                else if (((i - a) * (i - a) + (j - b) * (j - b)) <= thirdR * thirdR)
                {
                    worldArray[i, j].curSugar = mountainTops - (2 * (mountainTops / 4));
                    worldArray[i, j].maxSugar = mountainTops - (2 * (mountainTops / 4));
                    envTilemap.SetColor(new Vector3Int(i, j, 0), new Color(0.75f, 0.75f, 0, 0.75f));
                }
                else if (((i - c) * (i - c) + (j - d) * (j - d)) <= thirdR * thirdR)
                {
                    worldArray[i, j].curSugar = mountainTops - (2 * (mountainTops / 4));
                    worldArray[i, j].maxSugar = mountainTops - (2 * (mountainTops / 4));
                    envTilemap.SetColor(new Vector3Int(i, j, 0), new Color(0.75f, 0.75f, 0, 0.75f));
                }

                //widest circle
                else if (((i - a) * (i - a) + (j - b) * (j - b)) <= outerR * outerR)
                {
                    worldArray[i, j].curSugar = mountainTops - (3 * (mountainTops / 4));
                    worldArray[i, j].maxSugar = mountainTops - (3 * (mountainTops / 4));
                    envTilemap.SetColor(new Vector3Int(i, j, 0), new Color(1, 1, 0, 0.75f));
                }
                else if (((i - c) * (i - c) + (j - d) * (j - d)) <= outerR * outerR)
                {
                    worldArray[i, j].curSugar = mountainTops - (3 * (mountainTops / 4));
                    worldArray[i, j].maxSugar = mountainTops - (3 * (mountainTops / 4));
                    envTilemap.SetColor(new Vector3Int(i, j, 0), new Color(1, 1, 0, 0.75f));
                }
            }
        }
        //SPICE MOUNTAINS

        //points of centre co-ords
        a = 13;
        b = 13;
        //co-ords of mountain 2
        c = 37;
        d = 37;

        for (int i = 0; i < rows; ++i)
        {
            for (int j = 0; j < cols; ++j)
            {
                //smallest circle
                if (((i - a) * (i - a) + (j - b) * (j - b)) <= innerR * innerR)
                {
                    worldArray[i, j].curSpice = mountainTops;
                    worldArray[i, j].maxSpice = mountainTops;
                    envTilemap.SetColor(new Vector3Int(i, j, 0), new Color(0.25f, 0, 0, 0.75f));
                }
                //could make these ORs
                else if (((i - c) * (i - c) + (j - d) * (j - d)) <= innerR * innerR)
                {
                    worldArray[i, j].curSpice = mountainTops;
                    worldArray[i, j].maxSpice = mountainTops;
                    envTilemap.SetColor(new Vector3Int(i, j, 0), new Color(0.25f, 0, 0, 0.75f));
                }


                else if (((i - a) * (i - a) + (j - b) * (j - b)) <= secondR * secondR)
                {
                    worldArray[i, j].curSpice = mountainTops - mountainTops / 4;
                    worldArray[i, j].maxSpice = mountainTops - mountainTops / 4;
                    envTilemap.SetColor(new Vector3Int(i, j, 0), new Color(0.5f, 0, 0, 0.75f));
                }
                else if (((i - c) * (i - c) + (j - d) * (j - d)) <= secondR * secondR)
                {
                    worldArray[i, j].curSpice = mountainTops - mountainTops / 4;
                    worldArray[i, j].maxSpice = mountainTops - mountainTops / 4;
                    envTilemap.SetColor(new Vector3Int(i, j, 0), new Color(0.5f, 0, 0, 0.75f));
                }


                else if (((i - a) * (i - a) + (j - b) * (j - b)) <= thirdR * thirdR)
                {
                    worldArray[i, j].curSpice = mountainTops - (2 * (mountainTops / 4));
                    worldArray[i, j].maxSpice = mountainTops - (2 * (mountainTops / 4));
                    envTilemap.SetColor(new Vector3Int(i, j, 0), new Color(0.75f, 0, 0, 0.75f));
                }
                else if (((i - c) * (i - c) + (j - d) * (j - d)) <= thirdR * thirdR)
                {
                    worldArray[i, j].curSpice = mountainTops - (2 * (mountainTops / 4));
                    worldArray[i, j].maxSpice = mountainTops - (2 * (mountainTops / 4));
                    envTilemap.SetColor(new Vector3Int(i, j, 0), new Color(0.75f, 0, 0, 0.75f));
                }

                //widest circle
                else if (((i - a) * (i - a) + (j - b) * (j - b)) <= outerR * outerR)
                {
                    worldArray[i, j].curSpice = mountainTops - (3 * (mountainTops / 4));
                    worldArray[i, j].maxSpice = mountainTops - (3 * (mountainTops / 4));
                    envTilemap.SetColor(new Vector3Int(i, j, 0), new Color(0.9f, 0, 0, 0.75f));
                }
                else if (((i - c) * (i - c) + (j - d) * (j - d)) <= outerR * outerR)
                {
                    worldArray[i, j].curSpice = mountainTops - (3 * (mountainTops / 4));
                    worldArray[i, j].maxSpice = mountainTops - (3 * (mountainTops / 4));
                    envTilemap.SetColor(new Vector3Int(i, j, 0), new Color(0.9f, 0, 0, 0.75f));
                }
            }
        }


    }
}
