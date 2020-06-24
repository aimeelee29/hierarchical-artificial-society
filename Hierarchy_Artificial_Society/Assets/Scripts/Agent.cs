using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    //The following GameObjects are assigned in the KnowWorld() function. 
    //They represent the world/environment. The agent will need to communicate with these.
    private GameObject objWorld;
    private GameObject objGrid;
    private World world;
    private GridLayout gridLayout;
    public Vector2Int cellPosition;

    //Need access to scriptable object Toggle to enable/disable certain things
    private Toggle toggle;

    //initial sugar and spice endowments. Used for reproduction
    public int sugarInit;
    public int spiceInit;

    // sugar and spice accumulations
    public int sugar;
    public int spice;

    //Metabolisms - how much sugar and spice the agent 'burns off' each time step
    public int sugarMetabolism;
    public int spiceMetabolism;

    //Position - not sure I need this now
    private int x;
    private int y;

    //How far they can 'see' to eat sugar/spice (in number of cells)
    public int vision;

    //Agent's lifespan (in time steps), age and flag for whether they are alive
    public int lifespan;
    public int age;
    public bool isAlive;

    //attributes for reproduction
    public int childBearingBegins;
    public int childBearingEnds;
    public string sex;

    //hierarchy attributes
    private int dominance;
    private int influence;
    //others go here - will use wealth, vision, influence

    // variables to store info on best location
    // used for LookAround and eat
    Vector2Int pos;
    double maxWelfare;

    void Awake()
    {
        KnowWorld();
    }

    // Start is called before the first frame update
    //could this be moved to awake
    void Start()
    {
        //check its working ok
        //print(cellPosition);
        //print(world.worldArray[cellPosition.x, cellPosition.y].curSugar);
        
    }

    // Update is called once per frame
    void Update()
    {
        //update agent's age
        ++age;

        //decrease sugar & spice through metabolism
        sugar -= sugarMetabolism;
        spice -= spiceMetabolism;

        //check for death
        Death();

        //print("sug pre harvest" + sugar);
        //print("spi pre harvest" + spice);
        
        //Look around and harvest food
        if (isAlive)
            Harvest();

        FindFertileNeighbours();
    }

    //this method enables the Agent to communicate with its surroundings
    public void KnowWorld()
    {
        //Assign GameObjects through Find. 
        objWorld = GameObject.Find("World");
        objGrid = GameObject.Find("Grid");
        //Need access to the script attached to World GameObject
        world = objWorld.GetComponent<World>();
        //Need access to the GridLayout component to be able to convert World location to cell location
        gridLayout = objGrid.GetComponent<GridLayout>();
        cellPosition = new Vector2Int(gridLayout.WorldToCell(transform.position).x, gridLayout.WorldToCell(transform.position).y);

        /*
        print("agent cell position =" + cellPosition);
        print("agent transform position = " + transform.position);
        */
    }

    //Agent will die if it reaches its lifespan or runs out of either sugar or spice
    public void Death()
    {
        if (isAlive && (age == lifespan || sugar <= 0 || spice <= 0))
        {
            isAlive = false;
            Destroy(gameObject);
        }
    }

    /*
    //Used to determine which site would produce most benefit to agent
    public double Welfare(int su, int sp)
    {
        return Math.Pow(sugar + su, (double)sugarMetabolism / (sugarMetabolism + spiceMetabolism)) * Math.Pow(spice, (double)spiceMetabolism / (sugarMetabolism + spiceMetabolism));

    }
    */

    // Agent will need to find sugar/spice to 'eat' from surroundings.
    public void Harvest()
    {
        // variables to store info on best location
        // initially set to current cell position values
        pos = cellPosition;
        maxWelfare = world.worldArray[cellPosition.x, cellPosition.y].Welfare(sugar, spice, sugarMetabolism, spiceMetabolism);

        //variables to keep track of how to iterate loop (to cope with agents situated at edges)
        int temp;
        int leftover;

        // LOOK NORTH
        // i.e. must increment y value of array (up)

        //edges e.g. if vision was 10 and you were at (20, 45). you wold want all the way to 49 (so look up4) and then (20,0),(20,1),(20,2), (20,3), (20,4), (20,5)


        //if vision pushes you over the grid boundary to the north
        if (cellPosition.y + vision > world.GetRows() - 1)
        {
            temp = world.GetRows() - 1;
            leftover = cellPosition.y + vision - world.GetRows();
        }
        else
        {
            temp = cellPosition.y + vision;
            leftover = 0;
        }


        for (int i = cellPosition.y+1; i <= temp; ++i)
        {
            //if location isn't already ane at location for another agent
            if (world.worldArray[cellPosition.x, i].GetOccupied() == false)
            {
                //if current cell will produce highest welfare so far
                double curWelfare = world.worldArray[cellPosition.x, i].Welfare(sugar, spice, sugarMetabolism, spiceMetabolism);
                if (curWelfare > maxWelfare)
                {
                    pos = new Vector2Int(cellPosition.x, i);
                    maxWelfare = curWelfare;
                }
            }
        }
        if (leftover >0)
        {
            //iterate over
            for (int i = 0; i <= leftover; ++i)
            {
                //if location isn't already ane at location for another agent
                if (world.worldArray[cellPosition.x, i].GetOccupied() == false)
                {
                    //if current cell will produce highest welfare so far
                    double curWelfare = world.worldArray[cellPosition.x, i].Welfare(sugar, spice, sugarMetabolism, spiceMetabolism);
                    if (curWelfare > maxWelfare)
                    {
                        pos = new Vector2Int(cellPosition.x, i);
                        maxWelfare = curWelfare;
                    }
                }
            }
        }

        // LOOK SOUTH
        // i.e. must increment y value of array (down)

        // if vision pushes you over the grid boundary to the south
        // e.g. if you were on (10, 3) and vision was 10, you would look 3 down to 0, then 7 to go
        if (cellPosition.y - vision < 0)
        {
            temp = 0;
            leftover = vision - cellPosition.y;
        }
        else
        {
            temp = cellPosition.y - vision;
            leftover = 0;
        }

        for (int i = cellPosition.y - 1; i >= 0; --i)
        {
            //if location isn't already ane at location for another agent
            if (world.worldArray[cellPosition.x, i].GetOccupied() == false)
            {
                // if current cell will produce highest welfare so far
                double curWelfare = world.worldArray[cellPosition.x, i].Welfare(sugar, spice, sugarMetabolism, spiceMetabolism);
                if (curWelfare > maxWelfare)
                {
                    pos = new Vector2Int(cellPosition.x, i);
                    maxWelfare = curWelfare;
                }
            }
        }

        if (leftover > 0)
        {
            // iterate over
            for (int i = world.GetRows()-1; i >= leftover; --i)
            {
                //if location isn't already ane at location for another agent
                if (world.worldArray[cellPosition.x, i].GetOccupied() == false)
                {
                    // if current cell will produce highest welfare so far
                    double curWelfare = world.worldArray[cellPosition.x, i].Welfare(sugar, spice, sugarMetabolism, spiceMetabolism);
                    if (curWelfare > maxWelfare)
                    {
                        pos = new Vector2Int(cellPosition.x, i);
                        maxWelfare = curWelfare;
                    }
                }
            }
        }

        // LOOK EAST
        // i.e. must increment x value of array (up)

        //if vision pushes you over the grid boundary to the east
        if (cellPosition.x + vision > world.GetCols() - 1)
        {
            temp = world.GetCols() - 1;
            leftover = cellPosition.x + vision - world.GetCols();
        }
        else
        {
            temp = cellPosition.x + vision;
            leftover = 0;
        }


        for (int i = cellPosition.x + 1; i <= temp; ++i)
        {
            //if location isn't already ane at location for another agent
            if (world.worldArray[i, cellPosition.y].GetOccupied() == false)
            {
                //if current cell will produce highest welfare so far
                double curWelfare = world.worldArray[i, cellPosition.y].Welfare(sugar, spice, sugarMetabolism, spiceMetabolism);
                if (curWelfare > maxWelfare)
                {
                    pos = new Vector2Int(i, cellPosition.y);
                    maxWelfare = curWelfare;
                }
            }
        }
        if (leftover > 0)
        {
            //iterate over
            for (int i = 0; i <= leftover; ++i)
            {
                //if location isn't already ane at location for another agent
                if (world.worldArray[i, cellPosition.y].GetOccupied() == false)
                {
                    //if current cell will produce highest welfare so far
                    double curWelfare = world.worldArray[i, cellPosition.y].Welfare(sugar, spice, sugarMetabolism, spiceMetabolism);
                    if (curWelfare > maxWelfare)
                    {
                        pos = new Vector2Int(i, cellPosition.y);
                        maxWelfare = curWelfare;
                    }
                }
            }
        }

        // LOOK WEST
        // i.e. must increment x value of array (down)

        // if vision pushes you over the grid boundary to the west
        if (cellPosition.x - vision < 0)
        {
            temp = 0;
            leftover = vision - cellPosition.x;
        }
        else
        {
            temp = cellPosition.x - vision;
            leftover = 0;
        }

        for (int i = cellPosition.x - 1; i >= 0; --i)
        {
            //if location isn't already ane at location for another agent
            if (world.worldArray[i, cellPosition.y].GetOccupied() == false)
            {
                // if current cell will produce highest welfare so far
                double curWelfare = world.worldArray[i, cellPosition.y].Welfare(sugar, spice, sugarMetabolism, spiceMetabolism);
                if (curWelfare > maxWelfare)
                {
                    pos = new Vector2Int(i, cellPosition.y);
                    maxWelfare = curWelfare;
                }
            }
        }

        if (leftover > 0)
        {
            // iterate over
            for (int i = world.GetCols()-1; i >= leftover; --i)
            {
                //if location isn't already ane at location for another agent
                if (world.worldArray[i, cellPosition.y].GetOccupied() == false)
                {
                    // if current cell will produce highest welfare so far
                    double curWelfare = world.worldArray[i, cellPosition.y].Welfare(sugar, spice, sugarMetabolism, spiceMetabolism);
                    if (curWelfare > maxWelfare)
                    {
                        pos = new Vector2Int(i, cellPosition.y);
                        maxWelfare = curWelfare;
                    }
                }
            }
        }

        //set agent as harvesting that cell
        world.worldArray[pos.x, pos.y].SetOccupied(true);
        //agent harvests as much as possible from cell

        sugar += world.worldArray[pos.x, pos.y].DepleteSugar();
        spice += world.worldArray[pos.x, pos.y].DepleteSpice();
        //print(pos);
        //print("sugar post harvest = " + sugar);
        //print("spice post harvest = " + spice);
    }



    private bool Fertile()
    {
        return (childBearingBegins <= age && childBearingEnds > age && sugar >= sugarInit && spice >= spiceInit);
    }

            /*
    private void Reproduce()
    {
        if (reproduction)
        {

        }
    }
    */

    private List<Agent> FindFertileNeighbours()
    {
        //Generate array of colliders within radius (set to vision)
        Collider2D[] colliderList = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), vision);
        //Create empty List into which fertile agents will go
        List<Agent> fertileAgentList = new List<Agent>();

        //goes through each collider within radius and adds fertile agents only to the list
        foreach (Collider2D neighbour in colliderList)
        {
            if(neighbour.tag == "Agent" && neighbour.gameObject.GetComponent<Agent>().Fertile() == true)
            {
                fertileAgentList.Add(neighbour.gameObject.GetComponent<Agent>());
            }
        }
        return fertileAgentList;
    }
}
