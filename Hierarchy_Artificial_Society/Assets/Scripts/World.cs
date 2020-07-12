using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class World : MonoBehaviour
{
    /*
     * REFERENCES
     */

    //Needs to know about TileMap in order to do colour shading for mountains
    private GameObject objTileMap;
    private Tilemap envTilemap;

    /*
     * VARIABLES
     */

    // Determines the size of the world
    [SerializeField] private static int rows = 50;
    [SerializeField] private static int cols = 50;

    // Represents world as a 2D array of cells
    private Cell[,] worldArray = new Cell[rows, cols];

    // Amount of sugar/spice at top of mountain
    [SerializeField] private static int mountainTops = 20;
    // Amount of sugar/spice on other tiers
    private static int secondTier = mountainTops - mountainTops / 4;
    private static int thirdTier = secondTier - mountainTops / 4;
    private static int fourthTier = thirdTier - mountainTops / 4;
    // Amount of sugar/spice between mountains
    [SerializeField] private static int wasteland = 2;

    /*
     * GETTERS AND SETTERS
     */

    public Cell[,] WorldArray { get => worldArray; set => worldArray = value; }
    public static int Rows { get => rows; set => rows = value; }
    public static int Cols { get => cols; set => cols = value; }
    public static int MountainTops { get => mountainTops; set => mountainTops = value; }
    public static int Wasteland { get => wasteland; set => wasteland = value; }

    /* 
     * METHODS
     */

    // Start is called before the first frame update
    void Awake()
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
    private void PopulateArray()
    {
        for (int i = 0; i < rows; ++i)
        {
            for (int j = 0; j < cols; ++j)
            {
                worldArray[i, j] = new Cell();                  
            }
        }
    }

    //populates cells with sugar and spice levels in the form of sugar and spice mountains
    //also populates max sugar and spice levels
    private void Mountains()
    {
        //SUGAR MOUNTAINS
        //points of centre co-ords
        int a = 13;
        int b = 13;
        //co-ords of mountain 2
        int c = 37;
        int d = 37;

        //SPICE MOUNTAINS
        //centre co-ords of Mountain 1
        int e = 13;
        int f = 37;
        //centre co-ords of Mountain 2
        int g = 37;
        int h = 13;

        //sets radius for each mountain tier
        int innerR = 5;
        int secondR = innerR + 2;
        int thirdR = secondR + 2;
        int outerR = thirdR + 2;
  
        // checks to see if tile is in circle (using equation for a circle)
        for (int i = 0; i < rows; ++i)
        {
            for (int j = 0; j < cols; ++j)
            {
                // smallest sugar circle
                if (((i - a) * (i - a) + (j - b) * (j - b)) <= innerR * innerR ||
                    ((i - c) * (i - c) + (j - d) * (j - d)) <= innerR * innerR)
                {
                    worldArray[i, j].CurSugar = mountainTops;
                    worldArray[i, j].MaxSugar = mountainTops;
                    // set spice to 1 for these locations
                    worldArray[i, j].CurSpice = wasteland;
                    worldArray[i, j].MaxSpice = wasteland;
                    envTilemap.SetColor(new Vector3Int(i, j, 0), new Color(0.25f, 0.25f, 0, 0.75f));
                }
                
                else if (((i - a) * (i - a) + (j - b) * (j - b)) <= secondR * secondR || 
                         ((i - c) * (i - c) + (j - d) * (j - d)) <= secondR * secondR)
                {
                    worldArray[i, j].CurSugar = secondTier;
                    worldArray[i, j].MaxSugar = secondTier;
                    worldArray[i, j].CurSpice = wasteland;
                    worldArray[i, j].MaxSpice = wasteland;
                    envTilemap.SetColor(new Vector3Int(i, j, 0), new Color(0.5f, 0.5f, 0, 0.75f));
                }

                else if (((i - a) * (i - a) + (j - b) * (j - b)) <= thirdR * thirdR ||
                         ((i - c) * (i - c) + (j - d) * (j - d)) <= thirdR * thirdR)
                {
                    worldArray[i, j].CurSugar = thirdTier;
                    worldArray[i, j].MaxSugar = thirdTier;
                    worldArray[i, j].CurSpice = wasteland;
                    worldArray[i, j].MaxSpice = wasteland;
                    envTilemap.SetColor(new Vector3Int(i, j, 0), new Color(0.75f, 0.75f, 0, 0.75f));
                }
                //widest sugar circle
                else if (((i - a) * (i - a) + (j - b) * (j - b)) <= outerR * outerR ||
                         ((i - c) * (i - c) + (j - d) * (j - d)) <= outerR * outerR)
                {
                    worldArray[i, j].CurSugar = fourthTier;
                    worldArray[i, j].MaxSugar = fourthTier;
                    worldArray[i, j].CurSpice = wasteland;
                    worldArray[i, j].MaxSpice = wasteland;
                    envTilemap.SetColor(new Vector3Int(i, j, 0), new Color(1, 1, 0, 0.75f));
                }
                //smallest spice circle
                else if (((i - e) * (i - e) + (j - f) * (j - f)) <= innerR * innerR ||
                         ((i - g) * (i - g) + (j - h) * (j - h)) <= innerR * innerR)
                {
                    worldArray[i, j].CurSpice = mountainTops;
                    worldArray[i, j].MaxSpice = mountainTops;
                    worldArray[i, j].CurSugar = wasteland;
                    worldArray[i, j].MaxSugar = wasteland;
                    envTilemap.SetColor(new Vector3Int(i, j, 0), new Color(0, 0.25f, 0, 0.75f));
                }
                else if (((i - e) * (i - e) + (j - f) * (j - f)) <= secondR * secondR ||
                         ((i - g) * (i - g) + (j - h) * (j - h)) <= secondR * secondR)
                {
                    worldArray[i, j].CurSpice = secondTier;
                    worldArray[i, j].MaxSpice = secondTier;
                    worldArray[i, j].CurSugar = wasteland;
                    worldArray[i, j].MaxSugar = wasteland;
                    envTilemap.SetColor(new Vector3Int(i, j, 0), new Color(0, 0.5f, 0, 0.75f));
                }
                else if (((i - e) * (i - e) + (j - f) * (j - f)) <= thirdR * thirdR ||
                         ((i - g) * (i - g) + (j - h) * (j - h)) <= thirdR * thirdR)
                {
                    worldArray[i, j].CurSpice = thirdTier;
                    worldArray[i, j].MaxSpice = thirdTier;
                    worldArray[i, j].CurSugar = wasteland;
                    worldArray[i, j].MaxSugar = wasteland;
                    envTilemap.SetColor(new Vector3Int(i, j, 0), new Color(0, 0.75f, 0, 0.75f));
                }
                //widest spice circle
                else if (((i - e) * (i - e) + (j - f) * (j - f)) <= outerR * outerR ||
                         ((i - g) * (i - g) + (j - h) * (j - h)) <= outerR * outerR)
                {
                    worldArray[i, j].CurSpice = fourthTier;
                    worldArray[i, j].MaxSpice = fourthTier;
                    worldArray[i, j].CurSugar = wasteland;
                    worldArray[i, j].MaxSugar = wasteland;
                    envTilemap.SetColor(new Vector3Int(i, j, 0), new Color(0, 0.9f, 0, 0.75f));
                }
                else
                {
                    worldArray[i, j].CurSugar = wasteland;
                    worldArray[i, j].MaxSugar = wasteland;
                    worldArray[i, j].CurSpice = wasteland;
                    worldArray[i, j].MaxSpice = wasteland;
                    envTilemap.SetColor(new Vector3Int(i, j, 0), new Color(0, 0, 0, 0));
                }
            }
        }
    }

    // Checks if any neighbouring cell is empty
    public Vector2Int CheckEmptyCell(int x, int y)
    {
        Vector2Int empty;

        // Check around the cell in each direction
        // Also includes a check that it is in bounds
        if (x + 1 < cols && worldArray[x + 1, y].OccupyingAgent == null)
            empty = new Vector2Int(x + 1, y);
        else if (x - 1 >= 0 && worldArray[x - 1, y].OccupyingAgent == null)
            empty = new Vector2Int(x - 1, y);
        else if (y + 1 < rows && worldArray[x, y + 1].OccupyingAgent == null)
            empty = new Vector2Int(x, y + 1);
        else if (y - 1 >= 0 && worldArray[x, y - 1].OccupyingAgent == null)
            empty = new Vector2Int(x, y - 1);
        else
            empty = new Vector2Int(-1, -1);

        // If any of the above cells are empty then Vector2 empty will be assigned and we return it.
        // Otherwise will return (-1, -1) since Vector2 is non nullable.
        return empty;
    }
}
