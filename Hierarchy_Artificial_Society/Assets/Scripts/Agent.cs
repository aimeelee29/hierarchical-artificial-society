using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 
 *  Main Agent class
 *  
 */

public class Agent : MonoBehaviour
{
    /*
     * REFERENCES
     */

    //The following variables are assigned in the KnowWorld() function. 
    //They represent the world/environment - The agent will need to communicate with these.
    private World world;
    private GridLayout gridLayout;
    private Vector2Int cellPosition;
    //Need access to scriptable object Toggle to enable/disable certain things
    private static Toggle toggle;

    /* 
     * AGENT VARIABLES
     */

    // initial sugar and spice endowments. Used for reproduction
    private int sugarInit;
    private int spiceInit;

    // sugar and spice accumulations
    private int sugar;
    private int spice;

    // Metabolisms - how much sugar and spice the agent 'burns off' each time step
    private int sugarMetabolism;
    private int spiceMetabolism;

    //How far they can 'see' to eat sugar/spice (in number of cells)
    private int visionHarvest;
    // How far an agent can 'see' to reproduce and trade
    private int visionNeighbour;

    //Agent's lifespan (in time steps), age and flag for whether they are alive
    private int lifespan;
    private int age;
    private bool isAlive;

    // variables to store info on best location - initially set to current cell position values
    // used in Harvest
    private Vector2Int pos;
    private double maxWelfare;

    // Neighbours - updated each time step (since even though there is no movement, some will be born and others will die)
    private List<Agent> neighbourAgentList;

    //attributes for reproduction
    private int childBearingBegins;
    private int childBearingEnds;
    private SexEnum sex;

    //List of agents that current agent has mated with for that time step.
    private List<Agent> agentReproductionList;
    //List of children
    private List<Agent> agentChildList = new List<Agent>();

    // Time until death by sugar and spice. Needed for trading.
    private double timeUntilSugarDeath;
    private double timeUntilSpiceDeath;
    // marginal rate of substitution(MRS). An agent's MRS of spice for sugar is the amount of spice the agent considers to be as valuable as 
    // one unit of sugar, that is, the value of sugar in units of spice . 
    private double mrs;

    //hierarchy attributes
    private int dominance;
    private int influence;
    //others go here - will use wealth, vision
    private int hierarchyScore;

    // Maintain static list of 'dead' agents for object pooling
    // Child agents will take one of these agent's memory allocation
    private static List<Agent> availableAgents = new List<Agent>();

    /*
     * GETTERS AND SETTERS
     */

    public int SugarInit { get => sugarInit; set => sugarInit = value; }
    public int SpiceInit { get => spiceInit; set => spiceInit = value; }
    public int Sugar { get => sugar; set => sugar = value; }
    public int Spice { get => spice; set => spice = value; }
    public int SugarMetabolism { get => sugarMetabolism; set => sugarMetabolism = value; }
    public int SpiceMetabolism { get => spiceMetabolism; set => spiceMetabolism = value; }
    public int VisionHarvest { get => visionHarvest; set => visionHarvest = value; } //may not need 
    public int VisionNeighbour { get => visionNeighbour; set => visionNeighbour = value; }
    public int Lifespan { get => lifespan; set => lifespan = value; } //may not need 
    public int Age { get => age; set => age = value; }
    public bool IsAlive { get => isAlive; set => isAlive = value; }
    public List<Agent> NeighbourAgentList { get => neighbourAgentList; set => neighbourAgentList = value; }
    public int ChildBearingBegins { get => childBearingBegins; set => childBearingBegins = value; }
    public int ChildBearingEnds { get => childBearingEnds; set => childBearingEnds = value; }
    public SexEnum Sex { get => sex; set => sex = value; }
    public List<Agent> AgentReproductionList { get => agentReproductionList; set => agentReproductionList = value; }
    public List<Agent> AgentChildList { get => agentChildList; set => agentChildList = value; }
    public double TimeUntilSugarDeath { get => timeUntilSugarDeath; set => timeUntilSugarDeath = value; }
    public double TimeUntilSpiceDeath { get => timeUntilSpiceDeath; set => timeUntilSpiceDeath = value; }
    public double MRS { get => mrs; set => mrs = value; }
    public int Dominance { get => dominance; set => dominance = value; }
    public int Influence { get => influence; set => influence = value; }
    public int HierarchyScore { get => hierarchyScore; set => hierarchyScore = value; }
    public Vector2Int CellPosition { get => cellPosition; set => cellPosition = value; }


    /*
     * AWAKE & UPDATE
     * 
     */

    void Awake()
    {
        KnowWorld();
        // Reference to SO Toggle so we can turn various things on and off in the model
        toggle = Resources.Load<Toggle>("ScriptableObjects/Toggle");

        // 
        pos = cellPosition;
        maxWelfare = world.WorldArray[cellPosition.x, cellPosition.y].Welfare(Sugar, Spice, SugarMetabolism, SpiceMetabolism);
}

    void FixedUpdate()
    {
        //increase agent's age
        ++age;

        //decrease sugar & spice through metabolism
        sugar -= sugarMetabolism;
        spice -= spiceMetabolism;

        //check for death
        Death();

        if (IsAlive)
        {
            //Look around and harvest food
            Harvest();

            //time until death for each commodity
            timeUntilSugarDeath = sugar / sugarMetabolism;
            timeUntilSpiceDeath = spice / spiceMetabolism;
            if (timeUntilSugarDeath != 0) //avoids divide by zero error. Maybe could put this inside isalive if statement and then wouldn't need this
            {
                mrs = timeUntilSpiceDeath / timeUntilSugarDeath;
            }
            else
                mrs = 0;

            /*
            //find neighbouring agents 
            neighbourAgentList = FindNeighbours();
            //reproduce - only if selected in toggle SO
            if (toggle.GetReproduction())
            {
                ReproductionProcess();
            }
            //trade - only if selected in toggle SO
            if (toggle.GetTrade())
            {
                Trade();
            }  
            */
        }
    }

    /*
     * MAIN AGENT METHODS
     */
  
    // This method enables the Agent to communicate with its surroundings
    public void KnowWorld()
    {
        // Need access to the script attached to World GameObject
        world = GameObject.Find("World").GetComponent<World>();
        // Need access to the GridLayout component of Grid to be able to convert World location to cell location
        gridLayout = GameObject.Find("Grid").GetComponent<GridLayout>();
        // Assigns cellPosition to be transform position
        //cellPosition = new Vector2Int(gridLayout.WorldToCell(transform.position).x, gridLayout.WorldToCell(transform.position).y);
    }

    // Agent will die if it reaches its lifespan or runs out of either sugar or spice
    public void Death()
    {
        if (isAlive && (age == lifespan || sugar <= 0 || spice <= 0))
        {
            isAlive = false;
            // Add to available agent list for object pooling purposes
            availableAgents.Add(this);
            // Remove agent from its location on the grid

            // Deactivate Agent
            this.gameObject.SetActive(false);

        }
    }

    public double Welfare(int x, int y)
    {
        return Math.Pow(x + sugar, (double)sugarMetabolism / (sugarMetabolism + spiceMetabolism)) * Math.Pow(y + spice, (double)spiceMetabolism / (sugarMetabolism + spiceMetabolism));
    }


    // Agent will need to find sugar/spice to harvest from surroundings.
    public void Harvest()
    {
        //variables to keep track of how to iterate loop (to cope with agents situated at edges)
        int temp;
        int leftover;

        // LOOK NORTH
        // i.e. must increment y value of array (up)

        //if vision pushes you over the grid boundary to the north
        if (cellPosition.y + visionHarvest > World.Rows - 1)
        {
            temp = World.Rows - 1;
            leftover = cellPosition.y + visionHarvest - World.Rows;
        }
        else
        {
            temp = cellPosition.y + visionHarvest;
            leftover = 0;
        }


        for (int i = cellPosition.y+1; i <= temp; ++i)
        {
            //if location isn't already ane at location for another agent
            if (world.WorldArray[cellPosition.x, i].OccupyingAgent == null)
            {
                //if current cell will produce highest welfare so far
                double curWelfare = world.WorldArray[cellPosition.x, i].Welfare(sugar, spice, sugarMetabolism, spiceMetabolism);
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
                if (world.WorldArray[cellPosition.x, i].OccupyingAgent == null)
                {
                    //if current cell will produce highest welfare so far
                    double curWelfare = world.WorldArray[cellPosition.x, i].Welfare(sugar, spice, sugarMetabolism, spiceMetabolism);
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
        if (cellPosition.y - visionHarvest < 0)
        {
            temp = 0;
            leftover = visionHarvest - cellPosition.y;
        }
        else
        {
            temp = cellPosition.y - visionHarvest;
            leftover = 0;
        }

        for (int i = cellPosition.y - 1; i >= 0; --i)
        {
            //if location isn't already ane at location for another agent
            if (world.WorldArray[cellPosition.x, i].OccupyingAgent == null)
            {
                // if current cell will produce highest welfare so far
                double curWelfare = world.WorldArray[cellPosition.x, i].Welfare(sugar, spice, sugarMetabolism, spiceMetabolism);
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
            for (int i = World.Rows - 1; i >= leftover; --i)
            {
                //if location isn't already ane at location for another agent
                if (world.WorldArray[cellPosition.x, i].OccupyingAgent == null)
                {
                    // if current cell will produce highest welfare so far
                    double curWelfare = world.WorldArray[cellPosition.x, i].Welfare(sugar, spice, sugarMetabolism, spiceMetabolism);
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
        if (cellPosition.x + visionHarvest > World.Cols - 1)
        {
            temp = World.Cols - 1;
            leftover = cellPosition.x + visionHarvest - World.Cols;
        }
        else
        {
            temp = cellPosition.x + visionHarvest;
            leftover = 0;
        }


        for (int i = cellPosition.x + 1; i <= temp; ++i)
        {
            //if location isn't already ane at location for another agent
            if (world.WorldArray[i, cellPosition.y].OccupyingAgent == null)
            {
                //if current cell will produce highest welfare so far
                double curWelfare = world.WorldArray[i, cellPosition.y].Welfare(sugar, spice, sugarMetabolism, spiceMetabolism);
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
                if (world.WorldArray[i, cellPosition.y].OccupyingAgent == null)
                {
                    //if current cell will produce highest welfare so far
                    double curWelfare = world.WorldArray[i, cellPosition.y].Welfare(sugar, spice, sugarMetabolism, spiceMetabolism);
                    if (curWelfare > maxWelfare)
                    {
                        pos = new Vector2Int(i, cellPosition.y);
                        maxWelfare = curWelfare;
                    }
                }
            }
        }

        // LOOK WEST i.e. must increment x value of array (down)

        // if vision pushes you over the grid boundary to the west
        if (cellPosition.x - visionHarvest < 0)
        {
            temp = 0;
            leftover = visionHarvest - cellPosition.x;
        }
        else
        {
            temp = cellPosition.x - visionHarvest;
            leftover = 0;
        }

        for (int i = cellPosition.x - 1; i >= 0; --i)
        {
            //if location isn't already ane at location for another agent
            if (world.WorldArray[i, cellPosition.y].OccupyingAgent == null)
            {
                // if current cell will produce highest welfare so far
                double curWelfare = world.WorldArray[i, cellPosition.y].Welfare(sugar, spice, sugarMetabolism, spiceMetabolism);
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
            for (int i = World.Cols - 1; i >= leftover; --i)
            {
                //if location isn't already ane at location for another agent
                if (world.WorldArray[i, cellPosition.y].OccupyingAgent == null)
                {
                    // if current cell will produce highest welfare so far
                    double curWelfare = world.WorldArray[i, cellPosition.y].Welfare(sugar, spice, sugarMetabolism, spiceMetabolism);
                    if (curWelfare > maxWelfare)
                    {
                        pos = new Vector2Int(i, cellPosition.y);
                        maxWelfare = curWelfare;
                    }
                }
            }
        }

        //set agent as harvesting that cell
        world.WorldArray[pos.x, pos.y].OccupyingAgent = this;
        //agent harvests as much as possible from cell
        sugar += world.WorldArray[pos.x, pos.y].DepleteSugar();
        spice += world.WorldArray[pos.x, pos.y].DepleteSpice();
    }
}

public enum SexEnum { Male, Female };



