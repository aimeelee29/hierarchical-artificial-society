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

    private Vector2 transformPosition;
    private SocialMobilityAnalysis socialMobilityAnalysis;
    private HarvestAnalysis harvestAnalysis;

    [SerializeField] private Toggle toggle = null;

    // Reference to child gameobject (needed to set its circle collider radius for finding neighbours)
    private GameObject neighbourUpdater;

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

    // How far they can 'see' to eat sugar/spice (in number of cells)
    private int visionHarvest;
    // How far an agent can 'see' to reproduce and trade
    private int visionNeighbour;

    // Agent's lifespan (in time steps), age and flag for whether they are alive
    private int lifespan;
    private int age;
    private bool isAlive;

    // Boundary variables for harvest
    private int northBoundary;
    private int southBoundary;
    private int eastBoundary;
    private int westBoundary;
    // Variables to store info on best location - initially set to current cell position values
    // used in Harvest
    private Vector2Int pos;
    private double maxWelfare;
    private double curWelfare; // Will constantly update as agent looks around cells 
    // Variables to store amounts harvested
    private int sugarHarvested; 
    private int spiceHarvested;

    // Attributes for reproduction
    private int childBearingBegins;
    private int childBearingEnds;
    private SexEnum sex;
    // Set to true if agent resued memory in object pooling

    // Time until death by sugar and spice. Needed for trading.
    private double timeUntilSugarDeath;
    private double timeUntilSpiceDeath;
    // marginal rate of substitution (MRS). An agent's MRS of spice for sugar is the amount of spice the agent considers to be as valuable as 
    // one unit of sugar, that is, the value of sugar in units of spice . 
    private double mrs;

    // social hierarchy attributes
    private int dominance;
    private int influence;
    private int wealthScore;
    private int begSocialRank; // social rank at birth (or initial spawn).
    private int socialRank; // current social rank
    private int trackSocialRank; // helper variable used for keeping tracking of how many times agent has changed social rank.
    private int numberRankChanges = 0; // keeps track of how many times an agent has changed social rank.


    /*
     * LISTS
     */

    // Neighbours - updated each time step (since even though there is no movement, some will be born and others will die)
    private List<Agent> neighbourAgentList = new List<Agent>();
    // List of agents that current agent has mated with for that time step.
    private List<Agent> agentReproductionList = new List<Agent>();
    // List of children of agent
    private List<Agent> agentChildList = new List<Agent>();
    // List of agents that current agent has traded with for that time step.
    private List<Agent> agentTradeList = new List<Agent>();

    /*
     * STATIC LISTS
     */

    // Maintain static list of 'dead' agents for object pooling
    // Child agents will take one of these agent's memory allocation
    private static List<GameObject> availableAgents = new List<GameObject>();

    // Maintain static list of 'live' agents so the Agent Manager can run through them and call appropriate methods. Also used for agent count analysis.
    private static List<Agent> liveAgents = new List<Agent>();

    // Static list of live agents to be ordered for assigning wealth bands
    private static List<Agent> liveAgentsOrdered  = new List<Agent>();

    // Maintain static list of child agents (refreshed each time step)
    // Needed for manager, since can't alter live agent list while you are iterating through it.
    private static List<Agent> childAgents = new List<Agent>();

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
    public int SocialRank { get => socialRank; set => socialRank = value; }
    public Vector2Int CellPosition { get => cellPosition; set => cellPosition = value; }
    public static List<GameObject> AvailableAgents { get => availableAgents; set => availableAgents = value; }
    public static List<Agent> LiveAgents { get => liveAgents; set => liveAgents = value; }
    public static List<Agent> ChildAgents { get => childAgents; set => childAgents = value; }
    public List<Agent> AgentTradeList { get => agentTradeList; set => agentTradeList = value; }
    public int BegSocialRank { get => begSocialRank; set => begSocialRank = value; }
    public int NumberRankChanges { get => numberRankChanges; set => numberRankChanges = value; }
    public Vector2 TransformPosition { get => transformPosition; set => transformPosition = value; }
    public Toggle Toggle { get => toggle; set => toggle = value; }
    public GameObject NeighbourUpdater { get => neighbourUpdater; set => neighbourUpdater = value; }
    public int TrackSocialRank { get => trackSocialRank; set => trackSocialRank = value; }
    public int WealthScore { get => wealthScore; set => wealthScore = value; }
    public static List<Agent> LiveAgentsOrdered { get => liveAgentsOrdered; set => liveAgentsOrdered = value; }

    /*
     * AWAKE, START & UPDATE
     * 
     */

    void Awake()
    {
        KnowWorld();
        neighbourUpdater = this.transform.GetChild(0).gameObject;
    }

    void Start()
    {
        socialMobilityAnalysis = GameObject.Find("Analysis: Social Mobility").GetComponent<SocialMobilityAnalysis>();
        harvestAnalysis = GameObject.Find("Analysis: Harvest").GetComponent<HarvestAnalysis>();
    }

    void FixedUpdate()
    {
        //if current social ranking has changed
        if (socialRank != trackSocialRank)
        {
            trackSocialRank = socialRank;
            ++numberRankChanges;
        }
    }

    /*
     * MAIN AGENT METHODS
     */

    // This method enables the Agent to communicate with its surroundings.
    public void KnowWorld()
    {
        // Need access to the script attached to World GameObject
        world = GameObject.Find("World").GetComponent<World>();
        // Need access to the GridLayout component of Grid to be able to convert World location to cell location
        gridLayout = GameObject.Find("Grid").GetComponent<GridLayout>();
    }

    // Sets values (Initial - Random).
    public void InitVars(Toggle toggle)
    {
        sugar = UnityEngine.Random.Range(25, 51); // max exclusive
        spice = UnityEngine.Random.Range(25, 51);
        sugarInit = sugar;
        spiceInit = spice;
        sugarMetabolism = 3;
        spiceMetabolism = 3;
        visionNeighbour = 6;
        // Set vision for harvest - sets upper ranks higher vision if that particular setting is on.
        if (toggle.GetGreaterVisionHigherRank())
        {
            if (socialRank >= 7)
            {
                visionHarvest = 11;
            }
            else
            {
                visionHarvest = 9;
            }
        }
        else
        {
            visionHarvest = 9;
        }

        int sexRand = UnityEngine.Random.Range(1, 3);
        if (sexRand == 1)
            sex = SexEnum.Female;
        else if (sexRand == 2)
            sex = SexEnum.Male;

        isAlive = true;
        childBearingBegins = UnityEngine.Random.Range(12, 16);
        childBearingEnds = UnityEngine.Random.Range(35, 46);
        lifespan = UnityEngine.Random.Range(60, 101);
        age = UnityEngine.Random.Range(1, lifespan + 1);
        dominance = UnityEngine.Random.Range(1, 4);
        influence = UnityEngine.Random.Range(1, 4);

        //Sets boundaries used in harvest
        Boundaries();

        // Set radius of child circle collider - this will be used to add new children to other agents' list of neighbours
        neighbourUpdater.GetComponent<CircleCollider2D>().radius = visionNeighbour;

        return;
    }

    // Sets values (from inheritance given two parents).
    public void InitVars(Agent parentOne, Agent parentTwo)
    {
        if (toggle.GetReproductionWealthReduction())
        {
            sugar = 20 + 20;
            spice = 20 + 20;
            parentOne.Sugar -= 20;
            parentOne.Spice -= 20;
            parentTwo.Sugar -= 20;
            parentTwo.Spice -= 20;
        }
        else
        {
            // initial endowment is half of mother's + half of father's initial endowment
            sugar = (parentOne.SugarInit / 2) + (parentTwo.SugarInit / 2);
            spice = (parentOne.SpiceInit / 2) + (parentTwo.SpiceInit / 2);
            parentOne.Sugar -= (parentOne.SugarInit / 2);
            parentOne.Spice -= (parentOne.SpiceInit / 2);
            parentTwo.Sugar -= (parentTwo.SugarInit / 2);
            parentTwo.Spice -= (parentTwo.SpiceInit / 2);
        }
        
        sugarInit = sugar;
        spiceInit = spice;
  
        if (UnityEngine.Random.Range(1, 3) == 1)
            sugarMetabolism = parentOne.SugarMetabolism;
        else
            sugarMetabolism = parentTwo.SugarMetabolism;

        if (UnityEngine.Random.Range(1, 3) == 1)
            spiceMetabolism = parentOne.SpiceMetabolism;
        else
            spiceMetabolism = parentTwo.SpiceMetabolism;

        if (UnityEngine.Random.Range(1, 3) == 1)
            visionHarvest = parentOne.VisionHarvest;
        else
            visionHarvest = parentTwo.VisionHarvest;

        if (UnityEngine.Random.Range(1, 3) == 1)
            visionNeighbour = parentOne.VisionNeighbour;
        else
            visionNeighbour = parentTwo.VisionNeighbour;

        if (UnityEngine.Random.Range(1, 3) == 1)
            childBearingBegins = parentOne.ChildBearingBegins;
        else
            childBearingBegins = parentTwo.ChildBearingBegins;

        if (UnityEngine.Random.Range(1, 3) == 1)
            childBearingEnds = parentOne.ChildBearingEnds;
        else
            childBearingEnds = parentTwo.ChildBearingEnds;

        if (UnityEngine.Random.Range(1, 3) == 1)
            dominance = parentOne.Dominance;
        else
            dominance = parentTwo.Dominance;

        if (UnityEngine.Random.Range(1, 3) == 1)
            influence = parentOne.Influence;
        else
            influence = parentTwo.Influence;

        // Sex and lifespan is still random
        int sexRand = UnityEngine.Random.Range(1, 3);
        if (sexRand == 1)
            sex = SexEnum.Female;
        else if (sexRand == 2)
            sex = SexEnum.Male;
        lifespan = UnityEngine.Random.Range(60, 101);
        isAlive = true;
        age = 0;

        Boundaries();
        CreateWealthScore();
        Rank();
        begSocialRank = socialRank;
        trackSocialRank = socialRank;

        // Set radius of child circle collider - this will be used to add new children to other agents' list of neighbours
        neighbourUpdater.GetComponent<CircleCollider2D>().radius = visionNeighbour;

        return;
    }

    // Sets position (Initial - Random). First checks if there is already an agent there. Works recursively.
    public void InitPosition()
    {
        // generate random grid position
        int x = UnityEngine.Random.Range(0, World.Rows);
        int y = UnityEngine.Random.Range(0, World.Cols);

        // if no agent currently in that position then set transform to that position
        if (world.WorldArray[x, y].OccupyingAgent == null)
        {
            this.gameObject.transform.position = gridLayout.CellToWorld(new Vector3Int(x, y, 0));
            transformPosition.Set(this.gameObject.transform.position.x, this.gameObject.transform.position.y);
            // Set the agent as occupying agent in the grid location
            world.WorldArray[x, y].OccupyingAgent = this;
            // Set the cell position for agent
            cellPosition.Set(x, y);
            return;
        }
        // else repeat process
        else
            InitPosition();
    }
    
    // Sets position by spacing agents evenly throughout the grid.
    public void InitPosition(int numberOfAgents, int agentNo)
    {
        // Agent number multiplied by spacing between cells tells us what cell number the agent spawns into
        int cellNumber = agentNo * ((World.Rows * World.Cols) / numberOfAgents);
        int x = cellNumber % World.Rows;
        int y = cellNumber / World.Cols;

        this.gameObject.transform.position = gridLayout.CellToWorld(new Vector3Int(x, y, 0));
        transformPosition.Set(this.gameObject.transform.position.x, this.gameObject.transform.position.y);
        // Set the agent as occupying agent in the grid location
        world.WorldArray[x, y].OccupyingAgent = this;
        // Set the cell position for agent
        cellPosition.Set(x, y);
        return;
    }
   
    // Sets values (given two parents).
    public void InitPosition(Vector2Int freeVector1, Vector2Int freeVector2)
    {
        // If neither parent have a free neighbouring cell then return
        if (freeVector1.x == -1 && freeVector2.x == -1)
            return;
        
        else
        {
            int x;
            int y;

            // If both have free neighbouring cell then pick at random
            if (freeVector1.x != -1 && freeVector2.x != -1)
            {
                int rand = UnityEngine.Random.Range(1, 3);
                if (rand == 1)
                {
                    x = freeVector1.x;
                    y = freeVector1.y;
                }
                else
                {
                    x = freeVector2.x;
                    y = freeVector2.y;
                }
            }
            // If agent 1 has free neighbouring cell
            else if (freeVector1.x != -1)
            {
                x = freeVector1.x;
                y = freeVector1.y;
            }
            // If agent 2 has free neighbouring cell
            else
            {
                x = freeVector2.x;
                y = freeVector2.y;
            }
            // sets position to free vector chosen
            this.gameObject.transform.position = gridLayout.CellToWorld(new Vector3Int(x, y, 0));
            transformPosition.Set(this.gameObject.transform.position.x, this.gameObject.transform.position.y);
            // tells world that agent is in that cell
            world.WorldArray[x, y].OccupyingAgent = this;
            // Set cell position for agent
            cellPosition.Set(x, y);
        }
    }

    // Agent will die if it reaches its lifespan or runs out of either sugar or spice.
    public void Death()
    {
        if (isAlive && (age >= lifespan || sugar <= 0 || spice <= 0))
        {
            isAlive = false;
            // Appends agent to social mobility analysis file
            SocMobAppend();
            // Add to available agent list for object pooling purposes
            availableAgents.Add(this.gameObject);
            // Remove agent from its location on the grid
            world.WorldArray[cellPosition.x, cellPosition.y].OccupyingAgent = null;
            // If inheritance is turned on divide wealth among children
            if (toggle.GetInheritance())
            {
                Inheritance();
            }
            // Deactivate agent
            this.gameObject.SetActive(false);
        }
    }

    // Agent will die if it reaches its lifespan or runs out of either sugar or spice.
    public void DeathandReplacement()
    {
        if (isAlive && (age >= lifespan || sugar <= 0 || spice <= 0))
        {
            // Add agent details to social mobility analysis list
            SocMobAppend();
            // just need to redefine variables. Position, memory and neighbour list can be directly taken
            this.InitVars(toggle);
            CreateWealthScore();
            Rank();
            begSocialRank = socialRank;
            trackSocialRank = socialRank;
        }
    }

    //inheritance
    public void Inheritance()
    {
        // If no wealth then return
        if (sugar + spice ==0)
        {
            return;
        }
        int childCounter =0;

        // First count how many children are still alive
        for (int i = 0; i < agentChildList.Count; ++i)
        {
            if (agentChildList[i].IsAlive == true)
            {
                childCounter++;
            }
        }

        // If no children then return
        if (childCounter == 0)
        {
            return;
        }

        int sugarDivided = sugar / childCounter;
        int spiceDivided = spice / childCounter;

        // Then divide wealth
        for (int i = 0; i < agentChildList.Count; ++i)
        {
            if (agentChildList[i].IsAlive == true)
            {
                agentChildList[i].Sugar += sugarDivided;
                agentChildList[i].Spice += spiceDivided;
            }
        }
    }

    public double Welfare(int x, int y)
    {
        return Math.Pow(x + sugar, (double)sugarMetabolism / (sugarMetabolism + spiceMetabolism)) * Math.Pow(y + spice, (double)spiceMetabolism / (sugarMetabolism + spiceMetabolism));
    }
   
    // Creates wealth score
    public void CreateWealthScore()
    {
        // If in bottom 40% of wealth
        if (sugar + spice < LiveAgentsOrdered[(int)(LiveAgentsOrdered.Count/2.5)].Sugar + LiveAgentsOrdered[(int)(LiveAgentsOrdered.Count/2.5)].Spice)
        {
            wealthScore = 1;
        }
        // If 41% to 80%
        else if (sugar + spice < LiveAgentsOrdered[(int)(LiveAgentsOrdered.Count/1.25)].Sugar + LiveAgentsOrdered[(int)(LiveAgentsOrdered.Count/1.25)].Spice)
        {
            wealthScore = 2;
        }
        // If 80% to 95%
        else if (sugar + spice < LiveAgentsOrdered[(int)(LiveAgentsOrdered.Count/1.05)].Sugar + LiveAgentsOrdered[(int)(LiveAgentsOrdered.Count/1.05)].Spice)
        {
            wealthScore = 3;
        }
        else
        {
            wealthScore = 4;
        }
    }
    
    // Adds up vars to create a social ranking
    public void Rank()
    {
        socialRank = dominance + influence + wealthScore;

        // Reapply vision harvest if its the greater vision scenario
        if (toggle.GetGreaterVisionHigherRank())
        {
            if (socialRank >= 7)
            {
                visionHarvest = 11;
            }
            else
            {
                visionHarvest = 9;
            }
        }
    }

    // Deals with appending agent to social mobility analysis XML file
    public void SocMobAppend()
    {
        SocialMobility socMob = new SocialMobility(this.BegSocialRank, this.SocialRank, this.NumberRankChanges, this.Age);
        socialMobilityAnalysis.socialMobiltyListClass.socialMobilityList.Add(socMob);
    }

    public void Boundaries()
    {
        // NORTH
        // If vision pushes you over the grid boundary to the north
        if (cellPosition.y + visionHarvest > World.Rows - 1)
        {
            northBoundary = World.Rows - 1;
            //leftover = cellPosition.y + visionHarvest - World.Rows; // Was used when boundaries were periodic
        }
        else
        {
            northBoundary = cellPosition.y + visionHarvest;
            //leftover = 0;
        }

        // SOUTH
        // if vision pushes you over the grid boundary to the south
        if (cellPosition.y - visionHarvest < 0)
        {
            southBoundary = 0;
            //leftover = visionHarvest - cellPosition.y;
        }
        else
        {
            southBoundary = cellPosition.y - visionHarvest;
            //leftover = 0;
        }

        // EAST
        //if vision pushes you over the grid boundary to the east
        if (cellPosition.x + visionHarvest > World.Cols - 1)
        {
            eastBoundary = World.Cols - 1;
            //leftover = cellPosition.x + visionHarvest - World.Cols;
        }
        else
        {
            eastBoundary = cellPosition.x + visionHarvest;
            //leftover = 0;
        }

        // WEST
        // if vision pushes you over the grid boundary to the west
        if (cellPosition.x - visionHarvest < 0)
        {
            westBoundary = 0;
            //leftover = visionHarvest - cellPosition.x;
        }
        else
        {
            westBoundary = cellPosition.x - visionHarvest;
            //leftover = 0;
        }
    }

    public void HarvestResource()
    {
        // Resets pos and maxWelfare to be that of current cell
        // Set intial pos and maxwelfare used for harvest method. Used to be in awake of agent but needed variables not yet assigned.
        pos = cellPosition;
        maxWelfare = Welfare(world.WorldArray[cellPosition.x, cellPosition.y].CurSugar, world.WorldArray[cellPosition.x, cellPosition.y].CurSpice);
        // Reset sugar harvested
        sugarHarvested = 0;
        spiceHarvested = 0;

        // LOOK NORTH
        // i.e. must increment y value of array (up)
        for (int i = cellPosition.y + 1; i <= northBoundary; ++i)
        {
            //if location isn't already ane at location for another agent
            if (world.WorldArray[cellPosition.x, i].OccupiedHarvest == false)
            {
                // Calculate potential welfare at cell
                curWelfare = Welfare(world.WorldArray[cellPosition.x, i].CurSugar, world.WorldArray[cellPosition.x, i].CurSpice);

                //if current cell will produce highest welfare so far
                if (curWelfare > maxWelfare)
                {
                    pos.Set(cellPosition.x, i);
                    maxWelfare = curWelfare;
                }
            }
        }
        /*
        if (leftover >0)
        {
            //iterate over
            for (int i = 0; i <= leftover; ++i)
            {
                //if location isn't already ane at location for another agent
                if (world.WorldArray[cellPosition.x, i].OccupiedHarvest == false)
                {
                    //if current cell will produce highest welfare so far
                    curWelfare = Welfare(world.WorldArray[cellPosition.x, i].CurSugar, world.WorldArray[cellPosition.x, i].CurSpice);
                    //print("curWelfare = " + curWelfare);
                    if (curWelfare > maxWelfare)
                    {
                        pos.Set(cellPosition.x, i);
                        maxWelfare = curWelfare;
                    }
                }
            }
        }
        */

        // LOOK SOUTH
        // i.e. must increment y value of array (down)
        for (int i = cellPosition.y - 1; i >= southBoundary; --i)
        {
            // If location isn't already ane at location for another agent
            if (world.WorldArray[cellPosition.x, i].OccupiedHarvest == false)
            {
                // Calculate potential welfare at cell
                curWelfare = Welfare(world.WorldArray[cellPosition.x, i].CurSugar, world.WorldArray[cellPosition.x, i].CurSpice);

                // If current cell will produce highest welfare so far
                if (curWelfare > maxWelfare)
                {
                    pos.Set(cellPosition.x, i);
                    maxWelfare = curWelfare;
                }
            }
        }
        /*
        if (leftover > 0)
        {
            // iterate over
            for (int i = World.Rows - 1; i >= leftover; --i)
            {
                //if location isn't already ane at location for another agent
                if (world.WorldArray[cellPosition.x, i].OccupiedHarvest == false)
                {
                    // if current cell will produce highest welfare so far
                    curWelfare = Welfare(world.WorldArray[cellPosition.x, i].CurSugar, world.WorldArray[cellPosition.x, i].CurSpice);
                    //print("curWelfare = " + curWelfare);
                    if (curWelfare > maxWelfare)
                    {
                        pos.Set(cellPosition.x, i);
                        maxWelfare = curWelfare;
                        //print(maxWelfare);
                    }
                }
            }
        }
        */

        // LOOK EAST
        // i.e. must increment x value of array (up)
        for (int i = cellPosition.x + 1; i <= eastBoundary; ++i)
        {
            //if location isn't already a harvest location for another agent
            if (world.WorldArray[i, cellPosition.y].OccupiedHarvest == false)
            {
                // Calculate potential welfare at cell
                curWelfare = Welfare(world.WorldArray[i, cellPosition.y].CurSugar, world.WorldArray[i, cellPosition.y].CurSpice);

                //if current cell will produce highest welfare so far
                if (curWelfare > maxWelfare)
                {
                    pos.Set(i, cellPosition.y);
                    maxWelfare = curWelfare;
                }
            }
        }
        /*
        if (leftover > 0)
        {
            //iterate over
            for (int i = 0; i <= leftover; ++i)
            {
                //if location isn't already ane at location for another agent
                if (world.WorldArray[i, cellPosition.y].OccupiedHarvest == false)
                {
                    //if current cell will produce highest welfare so far
                    curWelfare = Welfare(world.WorldArray[i, cellPosition.y].CurSugar, world.WorldArray[i, cellPosition.y].CurSpice);
                    //print("curWelfare = " + curWelfare);
                    if (curWelfare > maxWelfare)
                    {
                        pos.Set(i, cellPosition.y);
                        maxWelfare = curWelfare;
                    }
                }
            }
        }
        */

        // LOOK WEST 
        // i.e. must increment x value of array (down)
        for (int i = cellPosition.x - 1; i >= westBoundary; --i)
        {
            //if location isn't already ane at location for another agent
            if (world.WorldArray[i, cellPosition.y].OccupiedHarvest == false)
            {
                // Calculate potential welfare at cell
                curWelfare = Welfare(world.WorldArray[i, cellPosition.y].CurSugar, world.WorldArray[i, cellPosition.y].CurSpice);

                // If current cell will produce highest welfare so far
                if (curWelfare > maxWelfare)
                {
                    pos.Set(i, cellPosition.y);
                    maxWelfare = curWelfare;
                }
            }
        }
        /*
        if (leftover > 0)
        {
            // iterate over
            for (int i = World.Cols - 1; i >= leftover; --i)
            {
                //if location isn't already ane at location for another agent
                if (world.WorldArray[i, cellPosition.y].OccupiedHarvest == false)
                {
                    // if current cell will produce highest welfare so far
                    curWelfare = Welfare(world.WorldArray[i, cellPosition.y].CurSugar, world.WorldArray[i, cellPosition.y].CurSpice);
                    //print("curWelfare = " + curWelfare);
                    if (curWelfare > maxWelfare)
                    {
                        pos.Set(i, cellPosition.y);
                        maxWelfare = curWelfare;
                    }
                }
            }
        }
        */

        // Set agent as harvesting that cell
        world.WorldArray[pos.x, pos.y].OccupiedHarvest = true;
        // Agent harvests as much as possible from cell
        sugarHarvested = world.WorldArray[pos.x, pos.y].DepleteSugar();
        spiceHarvested = world.WorldArray[pos.x, pos.y].DepleteSpice();
        sugar += sugarHarvested;
        spice += spiceHarvested;
        // Add to analysis files
        harvestAnalysis.AddSugar(sugarHarvested);
        harvestAnalysis.AddSpice(spiceHarvested);
    }
}

// Enum used for sex variable
// More efficient than using strings
public enum SexEnum { Male, Female };